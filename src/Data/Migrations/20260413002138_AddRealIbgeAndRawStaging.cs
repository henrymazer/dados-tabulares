using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRealIbgeAndRawStaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MunicipioCodigoIbge",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<double>(
                name: "AreaKm2",
                schema: "ibge",
                table: "setores_censitarios",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CodigoAglomerado",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoBairro",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoConcentracaoUrbana",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoDistrito",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoFcu",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoNucleoUrbano",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoRegiao",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoRegiaoImediata",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoRegiaoIntermediaria",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoSituacao",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoSubdistrito",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoTipo",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoUf",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeAglomerado",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeBairro",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeConcentracaoUrbana",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeDistrito",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeFcu",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeNucleoUrbano",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeRegiao",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeRegiaoImediata",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeRegiaoIntermediaria",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeSubdistrito",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeUf",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Situacao",
                schema: "ibge",
                table: "setores_censitarios",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "catalogo_categorias",
                schema: "ibge",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FonteDicionario = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    Pacote = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Variavel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalogo_categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_variaveis",
                schema: "ibge",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FonteDicionario = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    Pacote = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Tema = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Variavel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalogo_variaveis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ibge_agregados_staging",
                schema: "etl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Pacote = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NomeArquivoInterno = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    CodigoSetor = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ibge_agregados_staging", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "malha_municipal",
                schema: "ibge",
                columns: table => new
                {
                    CodigoMunicipio = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    NomeMunicipio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoRegiaoImediata = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    NomeRegiaoImediata = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoRegiaoIntermediaria = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    NomeRegiaoIntermediaria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoUf = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    NomeUf = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CodigoRegiao = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    NomeRegiao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SiglaRegiao = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CodigoConcentracaoUrbana = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    NomeConcentracaoUrbana = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AreaKm2 = table.Column<double>(type: "double precision", nullable: false),
                    Geometria = table.Column<MultiPolygon>(type: "geometry(MultiPolygon,4674)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_malha_municipal", x => x.CodigoMunicipio);
                });

            migrationBuilder.CreateTable(
                name: "tse_locais_votacao_brutos",
                schema: "etl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnoEleicao = table.Column<int>(type: "integer", nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CodigoUnidadeEleitoral = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    NomeUnidadeEleitoral = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MunicipioCodigoIbge = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NumeroZona = table.Column<int>(type: "integer", nullable: false),
                    NumeroSecao = table.Column<int>(type: "integer", nullable: false),
                    NumeroLocalVotacao = table.Column<int>(type: "integer", nullable: false),
                    NomeLocalVotacao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    EnderecoLocalVotacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tse_locais_votacao_brutos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_categorias_Pacote_Variavel",
                schema: "ibge",
                table: "catalogo_categorias",
                columns: new[] { "Pacote", "Variavel" });

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_variaveis_Pacote_Variavel",
                schema: "ibge",
                table: "catalogo_variaveis",
                columns: new[] { "Pacote", "Variavel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ibge_agregados_staging_Pacote_CodigoSetor",
                schema: "etl",
                table: "ibge_agregados_staging",
                columns: new[] { "Pacote", "CodigoSetor" });

            migrationBuilder.CreateIndex(
                name: "IX_malha_municipal_Geometria",
                schema: "ibge",
                table: "malha_municipal",
                column: "Geometria")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_malha_municipal_UfSigla",
                schema: "ibge",
                table: "malha_municipal",
                column: "UfSigla");

            migrationBuilder.CreateIndex(
                name: "IX_tse_locais_votacao_brutos_AnoEleicao_UfSigla_MunicipioCodig~",
                schema: "etl",
                table: "tse_locais_votacao_brutos",
                columns: new[] { "AnoEleicao", "UfSigla", "MunicipioCodigoIbge", "NumeroZona", "NumeroLocalVotacao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalogo_categorias",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "catalogo_variaveis",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "ibge_agregados_staging",
                schema: "etl");

            migrationBuilder.DropTable(
                name: "malha_municipal",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "tse_locais_votacao_brutos",
                schema: "etl");

            migrationBuilder.DropColumn(
                name: "AreaKm2",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoAglomerado",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoBairro",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoConcentracaoUrbana",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoDistrito",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoFcu",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoNucleoUrbano",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoRegiao",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoRegiaoImediata",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoRegiaoIntermediaria",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoSituacao",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoSubdistrito",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoTipo",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "CodigoUf",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeAglomerado",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeBairro",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeConcentracaoUrbana",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeDistrito",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeFcu",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeNucleoUrbano",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeRegiao",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeRegiaoImediata",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeRegiaoIntermediaria",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeSubdistrito",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "NomeUf",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.DropColumn(
                name: "Situacao",
                schema: "ibge",
                table: "setores_censitarios");

            migrationBuilder.AlterColumn<int>(
                name: "MunicipioCodigoIbge",
                schema: "ibge",
                table: "setores_censitarios",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(7)",
                oldMaxLength: 7);
        }
    }
}
