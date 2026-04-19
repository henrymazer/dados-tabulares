#!/usr/bin/env python3
"""Carrega um dataset agregado do IBGE por setor no schema ibge."""

from __future__ import annotations

import argparse
import csv
import io
import subprocess
import sys
import tempfile
import textwrap
import urllib.parse
import zipfile
from pathlib import Path


ROOT = Path(__file__).resolve().parent.parent
SCHEMA = "ibge"
DICT_TABLE = "dicionario_variaveis"
GEOM_TABLE = "setores_censitarios"

DATASETS: dict[str, dict[str, str]] = {
    "basico": {
        "zip": "Agregados_por_setores_basico_BR_20250417.zip",
        "dictionary": "dicionario_de_dados_agregados_por_setores_censitarios_basico.csv",
        "title": "Básico",
        "keep_mode": "v_only",
    },
    "caracteristicas_domicilio1": {
        "zip": "Agregados_por_setores_caracteristicas_domicilio1_BR.zip",
        "dictionary": "dicionario_Agregados_por_setores_caracteristicas_domicilio1_BR.csv",
        "title": "Características do Domicílio 1",
        "keep_mode": "all_after_first",
    },
    "caracteristicas_domicilio2": {
        "zip": "Agregados_por_setores_caracteristicas_domicilio2_BR_20250417.zip",
        "dictionary": "dicionario_Agregados_por_setores_caracteristicas_domicilio2_BR.csv",
        "title": "Características do Domicílio 2",
        "keep_mode": "all_after_first",
    },
    "caracteristicas_domicilio3": {
        "zip": "Agregados_por_setores_caracteristicas_domicilio3_BR_20250417.zip",
        "dictionary": "dicionario_Agregados_por_setores_caracteristicas_domicilio3_BR.csv",
        "title": "Características do Domicílio 3",
        "keep_mode": "all_after_first",
    },
    "cor_ou_raca": {
        "zip": "Agregados_por_setores_cor_ou_raca_BR.zip",
        "dictionary": "dicionario_Agregados_por_setores_cor_ou_raca_BR.csv",
        "title": "Cor ou Raça",
        "keep_mode": "all_after_first",
    },
    "demografia": {
        "zip": "Agregados_por_setores_demografia_BR.zip",
        "dictionary": "dicionario_Agregados_por_setores_demografia_BR.csv",
        "title": "Demografia",
        "keep_mode": "all_after_first",
    },
    "domicilios_indigenas": {
        "zip": "Agregados_por_setores_domicilios_indigenas_BR.zip",
        "dictionary": "dicionario_de_dados_agregados_por_setores_censitarios_indigenas_domicilios.csv",
        "title": "Domicílios Indígenas",
        "keep_mode": "all_after_first",
    },
    "domicilios_quilombolas": {
        "zip": "Agregados_por_setores_domicilios_quilombolas_BR.zip",
        "dictionary": "dicionario_de_dados_agregados_por_setores_censitarios_quilombolas_domicilios.csv",
        "title": "Domicílios Quilombolas",
        "keep_mode": "all_after_first",
    },
    "entorno_moradores": {
        "zip": "Agregados_por_setores_entorno_moradores_BR.zip",
        "dictionary": "dicionario_entorno_pessoas.csv",
        "title": "Entorno dos Moradores",
        "keep_mode": "all_after_first",
    },
    "obitos": {
        "zip": "Agregados_por_setores_obitos_BR.zip",
        "dictionary": "dicionario_Agregados_por_setores_obitos_BR.csv",
        "title": "Óbitos",
        "keep_mode": "all_after_first",
    },
    "parentesco": {
        "zip": "Agregados_por_setores_parentesco_BR.zip",
        "dictionary": "dicionario_Agregados_por_setores_parentesco_BR.csv",
        "title": "Parentesco",
        "keep_mode": "all_after_first",
    },
    "pessoas_indigenas": {
        "zip": "Agregados_por_setores_pessoas_indigenas_BR.zip",
        "dictionary": "dicionario_de_dados_agregados_por_setores_censitarios_indigenas_pessoas.csv",
        "title": "Pessoas Indígenas",
        "keep_mode": "all_after_first",
    },
    "pessoas_quilombolas": {
        "zip": "Agregados_por_setores_pessoas_quilombolas_BR.zip",
        "dictionary": "dicionario_de_dados_agregados_por_setores_censitarios_quilombolas_pessoas.csv",
        "title": "Pessoas Quilombolas",
        "keep_mode": "all_after_first",
    },
    "renda_responsavel": {
        "zip": "Agregados_por_setores_renda_responsavel_BR_csv.zip",
        "dictionary": "dicionario_de_dados_renda_responsavel.csv",
        "title": "Renda do Responsável",
        "keep_mode": "all_after_first",
    },
}


def qident(name: str) -> str:
    return '"' + name.replace('"', '""') + '"'


def qliteral(value: str) -> str:
    return "'" + value.replace("'", "''") + "'"


def run(cmd: list[str]) -> None:
    print("+", " ".join(cmd), file=sys.stderr)
    subprocess.run(cmd, check=True)


def psql(connection_string: str, sql: str) -> None:
    run(["psql", connection_string, "-v", "ON_ERROR_STOP=1", "-c", sql])


def psql_file(connection_string: str, sql_path: Path) -> None:
    run(["psql", connection_string, "-v", "ON_ERROR_STOP=1", "-f", str(sql_path)])


def fetch_scalar(connection_string: str, sql: str) -> str:
    completed = subprocess.run(
        ["psql", connection_string, "-tA", "-v", "ON_ERROR_STOP=1", "-c", sql],
        check=True,
        capture_output=True,
        text=True,
    )
    return completed.stdout.strip()


def require_postgis_available(connection_string: str) -> None:
    available = fetch_scalar(
        connection_string,
        "select exists (select 1 from pg_available_extensions where name = 'postgis');",
    )
    if available != "t":
        raise RuntimeError("O banco de destino não disponibiliza a extensão PostGIS.")


def require_shared_geometry_table(connection_string: str) -> None:
    exists = fetch_scalar(
        connection_string,
        f"select to_regclass('{SCHEMA}.{GEOM_TABLE}') is not null;",
    )
    if exists != "t":
        raise RuntimeError(
            f"A tabela compartilhada {SCHEMA}.{GEOM_TABLE} não existe. "
            "Carregue primeiro a malha de setores."
        )


def normalize_dictionary_rows(path: Path) -> list[dict[str, str]]:
    with path.open("r", encoding="utf-8", newline="") as handle:
        reader = csv.DictReader(handle)
        rows: list[dict[str, str]] = []
        for row in reader:
            normalized = {str(key).strip(): (value or "").strip() for key, value in row.items()}
            rows.append(
                {
                    "tipo": normalized.get("Tipo", "") or "não informado",
                    "tema": normalized.get("Tema", ""),
                    "variavel": normalized["Variável"].upper(),
                    "descricao": normalized["Descrição"],
                }
            )
        return rows


def build_dataset_comment(dataset_slug: str, dataset_title: str, row: dict[str, str]) -> str:
    tipo = row["tipo"] or "não informado no dicionário"
    tema = row["tema"] or dataset_title
    comment = (
        f"Variável IBGE {row['variavel']} do conjunto Agregados por Setores Censitários - "
        f"{dataset_title}. Dataset slug: {dataset_slug}. Tema: {tema}. Tipo: {tipo}. "
        f"Representa o agregado descrito como '{row['descricao']}' no nível do setor "
        "censitário. No arquivo de origem, o valor 'X' indica dado suprimido ou não "
        "divulgado pelo IBGE e foi convertido para NULL nesta tabela."
    )
    if dataset_slug == "renda_responsavel":
        comment += (
            " Neste dataset, valores decimais no padrão brasileiro com vírgula "
            "foram convertidos para ponto decimal na carga para o tipo numeric do "
            "PostgreSQL. O símbolo ',' isolado, sem dígitos antes ou depois, foi "
            "interpretado como valor ausente e convertido para NULL."
        )
    return comment


def create_dictionary_table_if_needed(connection_string: str) -> None:
    sql = f"""
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
    COMMENT ON TABLE {SCHEMA}.{DICT_TABLE} IS
    'Dicionário de variáveis dos conjuntos agregados do IBGE carregados no schema ibge. Cada linha documenta uma variável publicada pelo IBGE, incluindo a descrição analítica usada como comentário das colunas das tabelas de fatos.';
    COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.dataset_slug IS
    'Identificador estável do conjunto de dados dentro do schema ibge.';
    COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.tipo IS
    'Categoria principal do agregado conforme o dicionário do IBGE. Pode vir vazia quando o dicionário não traz essa coluna.';
    COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.tema IS
    'Tema analítico do conjunto de variáveis conforme o dicionário do IBGE. Pode vir vazio quando o dicionário não traz essa coluna.';
    COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.variavel IS
    'Código original da variável no dicionário e no arquivo de origem do IBGE.';
    COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.descricao IS
    'Descrição textual curta da variável conforme publicada pelo IBGE.';
    COMMENT ON COLUMN {SCHEMA}.{DICT_TABLE}.comentario_coluna IS
    'Texto analítico expandido aplicado no COMMENT da coluna correspondente da tabela de dados, pensado para consumo humano e por agentes de IA.';
    """
    psql(connection_string, sql)


def resolve_selected_columns(header: list[str], keep_mode: str, dictionary_rows: list[dict[str, str]]) -> tuple[list[int], list[str]]:
    dictionary_vars = {row["variavel"].lower() for row in dictionary_rows}
    selected_indices = [0]
    selected_columns = ["cd_setor"]
    for index, column in enumerate(header[1:], start=1):
        normalized = column.strip().lower()
        if keep_mode == "v_only":
            if normalized not in dictionary_vars:
                continue
        selected_indices.append(index)
        selected_columns.append(normalized)
    return selected_indices, selected_columns


def write_dictionary_csv(dataset_slug: str, dataset_title: str, rows: list[dict[str, str]], output_path: Path) -> None:
    fieldnames = ["dataset_slug", "tipo", "tema", "variavel", "descricao", "comentario_coluna"]
    with output_path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=fieldnames)
        writer.writeheader()
        for row in rows:
            writer.writerow(
                {
                    "dataset_slug": dataset_slug,
                    "tipo": row["tipo"] or "não informado",
                    "tema": row["tema"] or dataset_title,
                    "variavel": row["variavel"],
                    "descricao": row["descricao"],
                    "comentario_coluna": build_dataset_comment(dataset_slug, dataset_title, row),
                }
            )


def write_normalized_data_csv(
    zip_path: Path,
    keep_mode: str,
    dictionary_rows: list[dict[str, str]],
    output_path: Path,
) -> list[str]:
    with zipfile.ZipFile(zip_path) as archive:
        member = archive.namelist()[0]
        with archive.open(member) as compressed, output_path.open("w", encoding="utf-8", newline="") as target:
            reader = csv.reader(io.TextIOWrapper(compressed, encoding="latin-1"), delimiter=";")
            writer = csv.writer(target)
            header = next(reader)
            selected_indices, selected_columns = resolve_selected_columns(header, keep_mode, dictionary_rows)
            writer.writerow(selected_columns)
            for row in reader:
                normalized_row = [row[selected_indices[0]]]
                for index in selected_indices[1:]:
                    value = row[index].strip()
                    if value in {"X", "", ".", ","}:
                        normalized_row.append("")
                    else:
                        normalized_row.append(value.replace(",", "."))
                writer.writerow(normalized_row)
    return selected_columns


def build_table_sql(table_name: str, dataset_slug: str, dataset_title: str, columns: list[str]) -> str:
    rendered_cols = ",\n".join(
        ["    cd_setor text PRIMARY KEY"] + [f"    {qident(column)} numeric" for column in columns[1:]]
    )
    return textwrap.dedent(
        f"""
        CREATE SCHEMA IF NOT EXISTS {SCHEMA};
        DROP VIEW IF EXISTS {SCHEMA}.{table_name}_com_geometria;
        DROP TABLE IF EXISTS {SCHEMA}.{table_name};
        CREATE TABLE {SCHEMA}.{table_name} (
        {rendered_cols}
        );
        COMMENT ON TABLE {SCHEMA}.{table_name} IS
        {qliteral(
            f'Tabela de agregados do IBGE por setor censitário para o tema {dataset_title}. '
            f'Cada linha representa um CD_SETOR presente no arquivo original do dataset {dataset_slug}. '
            'Todas as colunas de variáveis foram carregadas como numeric para acomodar contagens, percentuais e médias; '
            "valores X do arquivo original foram convertidos para NULL."
        )};
        COMMENT ON COLUMN {SCHEMA}.{table_name}.cd_setor IS
        'Código do setor censitário (CD_SETOR) com 15 dígitos, usado como chave primária desta tabela e como elo de junção com a tabela ibge.setores_censitarios.';
        """
    ).strip()


def build_post_load_sql(
    table_name: str,
    view_name: str,
    dataset_slug: str,
    dataset_title: str,
    dictionary_rows: list[dict[str, str]],
) -> str:
    comments = [
        f"CREATE INDEX IF NOT EXISTS idx_{table_name}_cd_setor ON {SCHEMA}.{table_name} (cd_setor);",
        textwrap.dedent(
            f"""
            CREATE OR REPLACE VIEW {SCHEMA}.{view_name} AS
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
            FROM {SCHEMA}.{table_name} d
            LEFT JOIN {SCHEMA}.{GEOM_TABLE} g ON g.cd_setor = d.cd_setor;
            """
        ).strip(),
        f"COMMENT ON VIEW {SCHEMA}.{view_name} IS {qliteral(f'Visão analítica que combina o dataset {dataset_slug} ({dataset_title}) com a geometria consolidada e os atributos administrativos dos setores censitários do IBGE.')};",
    ]
    for row in dictionary_rows:
        comments.append(
            f"COMMENT ON COLUMN {SCHEMA}.{table_name}.{qident(row['variavel'].lower())} IS "
            f"{qliteral(build_dataset_comment(dataset_slug, dataset_title, row))};"
        )
    return "\n".join(comments) + "\n"


def copy_csv_to_table(connection_string: str, csv_path: Path, table_name: str) -> None:
    sql = f"\\copy {SCHEMA}.{table_name} FROM {qliteral(str(csv_path))} CSV HEADER"
    run(["psql", connection_string, "-v", "ON_ERROR_STOP=1", "-c", sql])


def load_one_dataset(connection_string: str, dataset_slug: str) -> None:
    config = DATASETS[dataset_slug]
    zip_path = ROOT / config["zip"]
    dictionary_path = ROOT / config["dictionary"]
    dataset_title = config["title"]
    table_name = f"{dataset_slug}_setores"
    view_name = f"{table_name}_com_geometria"
    dictionary_rows = normalize_dictionary_rows(dictionary_path)

    with tempfile.TemporaryDirectory(prefix=f"ibge_{dataset_slug}_") as temp_dir_str:
        temp_dir = Path(temp_dir_str)
        dictionary_csv = temp_dir / "dictionary.csv"
        data_csv = temp_dir / "data.csv"
        table_sql = temp_dir / "table.sql"
        post_load_sql = temp_dir / "post_load.sql"

        write_dictionary_csv(dataset_slug, dataset_title, dictionary_rows, dictionary_csv)
        selected_columns = write_normalized_data_csv(zip_path, config["keep_mode"], dictionary_rows, data_csv)
        table_sql.write_text(build_table_sql(table_name, dataset_slug, dataset_title, selected_columns) + "\n", encoding="utf-8")
        post_load_sql.write_text(
            build_post_load_sql(table_name, view_name, dataset_slug, dataset_title, dictionary_rows),
            encoding="utf-8",
        )

        psql_file(connection_string, table_sql)
        psql(connection_string, f"DELETE FROM {SCHEMA}.{DICT_TABLE} WHERE dataset_slug = {qliteral(dataset_slug)};")
        copy_csv_to_table(connection_string, dictionary_csv, DICT_TABLE)
        copy_csv_to_table(connection_string, data_csv, table_name)
        psql_file(connection_string, post_load_sql)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--connection-string", required=True)
    parser.add_argument("--dataset", action="append", choices=sorted(DATASETS), required=True)
    args = parser.parse_args()

    require_postgis_available(args.connection_string)
    require_shared_geometry_table(args.connection_string)
    create_dictionary_table_if_needed(args.connection_string)

    for dataset_slug in args.dataset:
        load_one_dataset(args.connection_string, dataset_slug)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
