#!/usr/bin/env python3
"""Carga inicial do IBGE: alfabetizacao por setor + malha de setores."""

from __future__ import annotations

import argparse
import csv
import io
import os
import subprocess
import sys
import tempfile
import textwrap
import urllib.parse
import zipfile
from pathlib import Path


ROOT = Path(__file__).resolve().parent.parent
DATA_ZIP = ROOT / "Agregados_por_setores_alfabetizacao_BR.zip"
DICT_CSV = ROOT / "dicionario_Agregados_por_setores_alfabetizacao_BR.csv"
GPKG = ROOT / "BR_setores_CD2022.gpkg"
MALHA_DICT = ROOT / "Dicionario_de_dados_malha_agregados.csv"

SCHEMA = "ibge"
DATA_TABLE = "alfabetizacao_setores"
DICT_TABLE = "dicionario_variaveis"
RAW_GEOM_TABLE = "setores_censitarios_raw"
GEOM_TABLE = "setores_censitarios"
VIEW_NAME = "alfabetizacao_setores_com_geometria"


def qident(name: str) -> str:
    return '"' + name.replace('"', '""') + '"'


def qliteral(value: str) -> str:
    return "'" + value.replace("'", "''") + "'"


def run(cmd: list[str], *, env: dict[str, str] | None = None) -> None:
    print("+", " ".join(cmd), file=sys.stderr)
    subprocess.run(cmd, check=True, env=env)


def psql(connection_string: str, sql: str) -> None:
    run(["psql", connection_string, "-v", "ON_ERROR_STOP=1", "-c", sql])


def psql_file(connection_string: str, sql_path: Path) -> None:
    run(["psql", connection_string, "-v", "ON_ERROR_STOP=1", "-f", str(sql_path)])


def to_ogr_pg_connection_string(connection_string: str) -> str:
    parsed = urllib.parse.urlparse(connection_string)
    if parsed.scheme not in {"postgresql", "postgres"}:
        raise ValueError("A connection string precisa usar o esquema postgresql:// ou postgres://")
    pieces = []
    if parsed.hostname:
        pieces.append(f"host={qliteral(parsed.hostname)}")
    if parsed.port:
        pieces.append(f"port={qliteral(str(parsed.port))}")
    if parsed.username:
        pieces.append(f"user={qliteral(urllib.parse.unquote(parsed.username))}")
    if parsed.password:
        pieces.append(f"password={qliteral(urllib.parse.unquote(parsed.password))}")
    dbname = parsed.path.lstrip("/")
    if dbname:
        pieces.append(f"dbname={qliteral(dbname)}")
    query = urllib.parse.parse_qs(parsed.query, keep_blank_values=True)
    for key, values in query.items():
        if values:
            pieces.append(f"{key}={qliteral(values[-1])}")
    return "PG:" + " ".join(pieces)


def load_dictionary_rows(path: Path) -> list[dict[str, str]]:
    with path.open("r", encoding="utf-8", newline="") as handle:
        return list(csv.DictReader(handle))


def require_postgis_available(connection_string: str) -> None:
    sql = (
        "select exists ("
        "select 1 from pg_available_extensions where name = 'postgis'"
        ") as postgis_available;"
    )
    completed = subprocess.run(
        ["psql", connection_string, "-tA", "-v", "ON_ERROR_STOP=1", "-c", sql],
        check=True,
        capture_output=True,
        text=True,
    )
    if completed.stdout.strip() != "t":
        raise RuntimeError(
            "O servidor PostgreSQL de destino não disponibiliza a extensão PostGIS em "
            "pg_available_extensions. Sem PostGIS não é possível carregar a malha "
            "georreferenciada nem criar o schema espacial solicitado."
        )


def load_malha_dictionary(path: Path) -> dict[str, dict[str, object]]:
    definitions: dict[str, dict[str, object]] = {}
    with path.open("r", encoding="utf-8", newline="") as handle:
        reader = csv.reader(handle)
        next(reader)
        for row in reader:
            if not row:
                continue
            variable = row[0].strip()
            category = row[1].strip() if len(row) > 1 else ""
            description = row[2].strip() if len(row) > 2 else ""
            entry = definitions.setdefault(variable, {"description": "", "categories": []})
            if category:
                entry["categories"].append((category, description))
            elif description and not entry["description"]:
                entry["description"] = description
    return definitions


def build_dataset_comment(row: dict[str, str]) -> str:
    description = row["Descrição"].strip()
    return (
        f"Variável IBGE {row['Variável']} do conjunto Agregados por Setores Censitários - "
        f"Alfabetização. Tema: {row['Tema']}. Tipo: {row['Tipo']}. "
        f"Representa a contagem do agregado descrito como '{description}' no nível do setor "
        "censitário. No arquivo de origem, o valor 'X' indica dado suprimido ou não "
        "divulgado pelo IBGE e foi convertido para NULL nesta tabela."
    )


def build_malha_comment(column_name: str, entry: dict[str, object] | None) -> str:
    if column_name == "geom":
        return (
            "Geometria consolidada do setor censitário em SIRGAS 2000 (EPSG:4674). "
            "Quando a malha original possui múltiplas feições para o mesmo código de setor, "
            "as geometrias foram agregadas em uma única geometria por CD_SETOR."
        )
    if not entry:
        return (
            "Coluna importada da malha de setores censitários do IBGE 2022. "
            "Sem descrição detalhada disponível no dicionário local."
        )
    base = str(entry["description"]).strip()
    categories = entry.get("categories") or []
    if categories:
        rendered = "; ".join(f"{code} = {desc}" for code, desc in categories)
        return f"{base}. Domínio conhecido: {rendered}."
    return base


def build_bootstrap_sql(dataset_rows: list[dict[str, str]]) -> str:
    data_columns = []
    for row in dataset_rows:
        data_columns.append(f"    {qident(row['Variável'].lower())} integer")
    joined_columns = ",\n".join(["    cd_setor text PRIMARY KEY"] + data_columns)
    return textwrap.dedent(
        f"""
        CREATE EXTENSION IF NOT EXISTS postgis;
        CREATE SCHEMA IF NOT EXISTS {SCHEMA};

        CREATE TABLE IF NOT EXISTS {SCHEMA}.{DICT_TABLE} (
            dataset_slug text NOT NULL,
            tipo text NOT NULL,
            tema text NOT NULL,
            variavel text NOT NULL,
            descricao text NOT NULL,
            comentario_coluna text NOT NULL,
            PRIMARY KEY (dataset_slug, variavel)
        );

        CREATE TABLE IF NOT EXISTS {SCHEMA}.{DATA_TABLE} (
        {joined_columns}
        );

        COMMENT ON TABLE {SCHEMA}.{DICT_TABLE} IS
        'Dicionário de variáveis dos conjuntos agregados do IBGE carregados no schema ibge. Cada linha documenta uma variável publicada pelo IBGE, incluindo a descrição analítica usada como comentário das colunas das tabelas de fatos.';
        COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.dataset_slug IS
        'Identificador estável do conjunto de dados dentro do schema ibge. Para esta carga inicial, o valor é alfabetizacao.';
        COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.tipo IS
        'Categoria principal do agregado conforme o dicionário do IBGE. Neste arquivo, indica que as variáveis se referem a Pessoas.';
        COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.tema IS
        'Tema analítico do conjunto de variáveis conforme o dicionário do IBGE. Neste arquivo, o tema é Alfabetização.';
        COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.variavel IS
        'Código original da variável no dicionário e no arquivo de origem do IBGE, como V00644.';
        COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.descricao IS
        'Descrição textual curta da variável conforme publicada pelo IBGE.';
        COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.comentario_coluna IS
        'Texto analítico expandido aplicado no COMMENT da coluna correspondente da tabela de dados, pensado para consumo humano e por agentes de IA.';

        COMMENT ON TABLE {SCHEMA}.{DATA_TABLE} IS
        'Tabela de agregados do IBGE por setor censitário para o tema Alfabetização. Cada linha representa um CD_SETOR presente no arquivo Agregados_por_setores_alfabetizacao_BR.zip, e cada coluna Vxxxxx armazena um agregado publicado pelo IBGE. Valores X do arquivo original foram convertidos para NULL.';
        COMMENT ON COLUMN {SCHEMA}.{DATA_TABLE}.cd_setor IS
        'Código do setor censitário (CD_SETOR) com 15 dígitos, usado como chave primária desta tabela e como elo de junção com a malha geográfica de setores do IBGE.';

        DROP VIEW IF EXISTS {SCHEMA}.{VIEW_NAME};
        DROP TABLE IF EXISTS {SCHEMA}.{GEOM_TABLE};
        DROP TABLE IF EXISTS {SCHEMA}.{RAW_GEOM_TABLE};
        """
    ).strip()


def write_dictionary_csv(rows: list[dict[str, str]], output_path: Path) -> None:
    fieldnames = ["dataset_slug", "tipo", "tema", "variavel", "descricao", "comentario_coluna"]
    with output_path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=fieldnames)
        writer.writeheader()
        for row in rows:
            writer.writerow(
                {
                    "dataset_slug": "alfabetizacao",
                    "tipo": row["Tipo"],
                    "tema": row["Tema"],
                    "variavel": row["Variável"],
                    "descricao": row["Descrição"],
                    "comentario_coluna": build_dataset_comment(row),
                }
            )


def write_normalized_data_csv(output_path: Path) -> None:
    with zipfile.ZipFile(DATA_ZIP) as archive:
        member = archive.namelist()[0]
        with archive.open(member) as compressed, output_path.open("w", encoding="utf-8", newline="") as target:
            reader = csv.reader(io.TextIOWrapper(compressed, encoding="latin-1"), delimiter=";")
            writer = csv.writer(target)
            header = next(reader)
            writer.writerow([header[0].lower(), *[column.lower() for column in header[1:]]])
            for row in reader:
                writer.writerow([row[0], *["" if value == "X" else value for value in row[1:]]])


def write_post_import_sql(
    dataset_rows: list[dict[str, str]],
    malha_dictionary: dict[str, dict[str, object]],
    output_path: Path,
) -> None:
    comments: list[str] = []
    comments.append(
        f"COMMENT ON TABLE {SCHEMA}.{RAW_GEOM_TABLE} IS "
        + qliteral(
            "Tabela bruta importada diretamente do arquivo BR_setores_CD2022.gpkg. "
            "Pode conter múltiplas feições para o mesmo CD_SETOR e serve como staging para a tabela consolidada."
        )
        + ";"
    )
    comments.append(
        f"COMMENT ON TABLE {SCHEMA}.{GEOM_TABLE} IS "
        + qliteral(
            "Tabela consolidada da malha de setores censitários do IBGE 2022. "
            "Cada linha representa um CD_SETOR único, com atributos administrativos e geometria agregada por setor."
        )
        + ";"
    )
    comments.append(
        f"COMMENT ON VIEW {SCHEMA}.{VIEW_NAME} IS "
        + qliteral(
            "Visão analítica que combina os agregados de alfabetização por setor censitário com a geometria consolidada e os atributos administrativos da malha do IBGE."
        )
        + ";"
    )
    comments.append(
        f"COMMENT ON COLUMN {SCHEMA}.{GEOM_TABLE}.feature_count_original IS "
        + qliteral(
            "Quantidade de feições da malha bruta que foram agregadas para formar a geometria consolidada deste CD_SETOR."
        )
        + ";"
    )

    raw_to_dict = {
        "cd_setor": "CD_SETOR",
        "situacao": "SITUACAO",
        "cd_sit": "CD_SITUACAO",
        "cd_tipo": "CD_TIPO",
        "area_km2": "AREA_KM2",
        "cd_regiao": "CD_REGIAO",
        "nm_regiao": "NM_REGIAO",
        "cd_uf": "CD_UF",
        "nm_uf": "NM_UF",
        "cd_mun": "CD_MUN",
        "nm_mun": "NM_MUN",
        "cd_dist": "CD_DIST",
        "nm_dist": "NM_DIST",
        "cd_subdist": "CD_SUBDIST",
        "nm_subdist": "NM_SUBDIST",
        "cd_bairro": "CD_BAIRRO",
        "nm_bairro": "NM_BAIRRO",
        "cd_nu": "CD_NU",
        "nm_nu": "NM_NU",
        "cd_fcu": "CD_FCU",
        "nm_fcu": "NM_FCU",
        "cd_aglom": "CD_AGLOM",
        "nm_aglom": "NM_AGLOM",
        "cd_rgint": "CD_RGINT",
        "nm_rgint": "NM_RGINT",
        "cd_rgi": "CD_RGI",
        "nm_rgi": "NM_RGI",
        "cd_concurb": "CD_CONCURB",
        "nm_concurb": "NM_CONCURB",
        "geom": "geom",
    }
    for target_column, dict_key in raw_to_dict.items():
        comment = build_malha_comment(dict_key, malha_dictionary.get(dict_key))
        comments.append(
            f"COMMENT ON COLUMN {SCHEMA}.{GEOM_TABLE}.{qident(target_column)} IS {qliteral(comment)};"
        )

    comments.append(
        f"CREATE INDEX IF NOT EXISTS idx_{DATA_TABLE}_cd_setor ON {SCHEMA}.{DATA_TABLE} (cd_setor);"
    )
    comments.append(
        f"CREATE INDEX IF NOT EXISTS idx_{GEOM_TABLE}_geom ON {SCHEMA}.{GEOM_TABLE} USING GIST (geom);"
    )

    for row in dataset_rows:
        comments.append(
            f"COMMENT ON COLUMN {SCHEMA}.{DATA_TABLE}.{qident(row['Variável'].lower())} IS "
            f"{qliteral(build_dataset_comment(row))};"
        )

    output_path.write_text(
        textwrap.dedent(
            f"""
            CREATE TABLE {SCHEMA}.{GEOM_TABLE} AS
            SELECT
                cd_setor,
                MIN(situacao) AS situacao,
                MIN(cd_sit) AS cd_sit,
                MIN(cd_tipo) AS cd_tipo,
                SUM(area_km2) AS area_km2,
                MIN(cd_regiao) AS cd_regiao,
                MIN(nm_regiao) AS nm_regiao,
                MIN(cd_uf) AS cd_uf,
                MIN(nm_uf) AS nm_uf,
                MIN(cd_mun) AS cd_mun,
                MIN(nm_mun) AS nm_mun,
                MIN(cd_dist) AS cd_dist,
                MIN(nm_dist) AS nm_dist,
                MIN(cd_subdist) AS cd_subdist,
                MIN(nm_subdist) AS nm_subdist,
                MIN(cd_bairro) AS cd_bairro,
                MIN(nm_bairro) AS nm_bairro,
                MIN(cd_nu) AS cd_nu,
                MIN(nm_nu) AS nm_nu,
                MIN(cd_fcu) AS cd_fcu,
                MIN(nm_fcu) AS nm_fcu,
                MIN(cd_aglom) AS cd_aglom,
                MIN(nm_aglom) AS nm_aglom,
                MIN(cd_rgint) AS cd_rgint,
                MIN(nm_rgint) AS nm_rgint,
                MIN(cd_rgi) AS cd_rgi,
                MIN(nm_rgi) AS nm_rgi,
                MIN(cd_concurb) AS cd_concurb,
                MIN(nm_concurb) AS nm_concurb,
                COUNT(*) AS feature_count_original,
                ST_Multi(ST_UnaryUnion(ST_Collect(geom)))::geometry(MultiPolygon, 4674) AS geom
            FROM {SCHEMA}.{RAW_GEOM_TABLE}
            GROUP BY cd_setor;

            ALTER TABLE {SCHEMA}.{GEOM_TABLE} ADD PRIMARY KEY (cd_setor);

            CREATE VIEW {SCHEMA}.{VIEW_NAME} AS
            SELECT
                d.*,
                g.situacao,
                g.cd_sit,
                g.cd_tipo,
                g.area_km2,
                g.cd_regiao,
                g.nm_regiao,
                g.cd_uf,
                g.nm_uf,
                g.cd_mun,
                g.nm_mun,
                g.cd_dist,
                g.nm_dist,
                g.cd_subdist,
                g.nm_subdist,
                g.cd_bairro,
                g.nm_bairro,
                g.cd_nu,
                g.nm_nu,
                g.cd_fcu,
                g.nm_fcu,
                g.cd_aglom,
                g.nm_aglom,
                g.cd_rgint,
                g.nm_rgint,
                g.cd_rgi,
                g.nm_rgi,
                g.cd_concurb,
                g.nm_concurb,
                g.feature_count_original,
                g.geom
            FROM {SCHEMA}.{DATA_TABLE} d
            LEFT JOIN {SCHEMA}.{GEOM_TABLE} g ON g.cd_setor = d.cd_setor;

            {os.linesep.join(comments)}
            """
        ).strip()
        + "\n",
        encoding="utf-8",
    )


def copy_csv_to_table(connection_string: str, csv_path: Path, table_name: str) -> None:
    sql = f"\\copy {SCHEMA}.{table_name} FROM {qliteral(str(csv_path))} CSV HEADER"
    run(["psql", connection_string, "-v", "ON_ERROR_STOP=1", "-c", sql])


def import_geopackage(connection_string: str) -> None:
    ogr_connection = to_ogr_pg_connection_string(connection_string)
    run(
        [
            "ogr2ogr",
            "-f",
            "PostgreSQL",
            ogr_connection,
            str(GPKG),
            "BR_setores_CD2022",
            "-lco",
            f"SCHEMA={SCHEMA}",
            "-nln",
            RAW_GEOM_TABLE,
            "-overwrite",
            "-progress",
        ]
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--connection-string", required=True)
    args = parser.parse_args()

    require_postgis_available(args.connection_string)

    dataset_rows = load_dictionary_rows(DICT_CSV)
    malha_dictionary = load_malha_dictionary(MALHA_DICT)

    with tempfile.TemporaryDirectory(prefix="ibge_alfabetizacao_") as temp_dir_str:
        temp_dir = Path(temp_dir_str)
        bootstrap_sql = temp_dir / "bootstrap.sql"
        dictionary_csv = temp_dir / "dictionary.csv"
        data_csv = temp_dir / "alfabetizacao.csv"
        post_import_sql = temp_dir / "post_import.sql"

        bootstrap_sql.write_text(build_bootstrap_sql(dataset_rows) + "\n", encoding="utf-8")
        write_dictionary_csv(dataset_rows, dictionary_csv)
        write_normalized_data_csv(data_csv)
        write_post_import_sql(dataset_rows, malha_dictionary, post_import_sql)

        psql_file(args.connection_string, bootstrap_sql)
        psql(args.connection_string, f"TRUNCATE TABLE {SCHEMA}.{DICT_TABLE};")
        copy_csv_to_table(args.connection_string, dictionary_csv, DICT_TABLE)
        psql(args.connection_string, f"TRUNCATE TABLE {SCHEMA}.{DATA_TABLE};")
        copy_csv_to_table(args.connection_string, data_csv, DATA_TABLE)
        import_geopackage(args.connection_string)
        psql_file(args.connection_string, post_import_sql)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
