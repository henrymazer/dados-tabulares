using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tse");

            migrationBuilder.EnsureSchema(
                name: "pnad");

            migrationBuilder.EnsureSchema(
                name: "ibge");

            migrationBuilder.CreateTable(
                name: "anos_eleicao",
                schema: "tse",
                columns: table => new
                {
                    Ano = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anos_eleicao", x => x.Ano);
                });

            migrationBuilder.CreateTable(
                name: "bens_declarados",
                schema: "tse",
                columns: table => new
                {
                    CandidatoCpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    AnoEleicao = table.Column<int>(type: "integer", nullable: false),
                    TipoBem = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CandidatoNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CandidatoNomeUrna = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CandidatoNumero = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PartidoNumero = table.Column<int>(type: "integer", nullable: false),
                    PartidoSigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PartidoNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bens_declarados", x => new { x.CandidatoCpf, x.AnoEleicao, x.TipoBem, x.Descricao });
                });

            migrationBuilder.CreateTable(
                name: "candidatos",
                schema: "tse",
                columns: table => new
                {
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    AnoEleicao = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NomeUrna = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PartidoNumero = table.Column<int>(type: "integer", nullable: false),
                    PartidoSigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PartidoNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidatos", x => new { x.Cpf, x.AnoEleicao });
                });

            migrationBuilder.CreateTable(
                name: "coligacoes",
                schema: "tse",
                columns: table => new
                {
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AnoEleicao = table.Column<int>(type: "integer", nullable: false),
                    PartidosJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coligacoes", x => new { x.Nome, x.AnoEleicao });
                });

            migrationBuilder.CreateTable(
                name: "dados_desemprego",
                schema: "pnad",
                columns: table => new
                {
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    TrimestreAno = table.Column<int>(type: "integer", nullable: false),
                    TrimestreNumero = table.Column<int>(type: "integer", nullable: false),
                    TaxaDesemprego = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_desemprego", x => new { x.UfSigla, x.TrimestreAno, x.TrimestreNumero });
                });

            migrationBuilder.CreateTable(
                name: "dados_escolaridade",
                schema: "ibge",
                columns: table => new
                {
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    NivelEscolaridade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_escolaridade", x => new { x.MunicipioCodigoIbge, x.NivelEscolaridade });
                });

            migrationBuilder.CreateTable(
                name: "dados_informalidade",
                schema: "pnad",
                columns: table => new
                {
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    TrimestreAno = table.Column<int>(type: "integer", nullable: false),
                    TrimestreNumero = table.Column<int>(type: "integer", nullable: false),
                    TaxaInformalidade = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_informalidade", x => new { x.UfSigla, x.TrimestreAno, x.TrimestreNumero });
                });

            migrationBuilder.CreateTable(
                name: "dados_infraestrutura",
                schema: "ibge",
                columns: table => new
                {
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    TipoInfraestrutura = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    DomiciliosAtendidos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_infraestrutura", x => new { x.MunicipioCodigoIbge, x.TipoInfraestrutura });
                });

            migrationBuilder.CreateTable(
                name: "dados_populacionais",
                schema: "ibge",
                columns: table => new
                {
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    FaixaEtaria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Raca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_populacionais", x => new { x.MunicipioCodigoIbge, x.FaixaEtaria, x.Raca });
                });

            migrationBuilder.CreateTable(
                name: "dados_renda",
                schema: "ibge",
                columns: table => new
                {
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    FaixaRenda = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_renda", x => new { x.MunicipioCodigoIbge, x.FaixaRenda });
                });

            migrationBuilder.CreateTable(
                name: "dados_renda_media",
                schema: "pnad",
                columns: table => new
                {
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    TrimestreAno = table.Column<int>(type: "integer", nullable: false),
                    TrimestreNumero = table.Column<int>(type: "integer", nullable: false),
                    RendaMedia = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_renda_media", x => new { x.UfSigla, x.TrimestreAno, x.TrimestreNumero });
                });

            migrationBuilder.CreateTable(
                name: "dados_saneamento",
                schema: "ibge",
                columns: table => new
                {
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    TipoSaneamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    DomiciliosAtendidos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_saneamento", x => new { x.MunicipioCodigoIbge, x.TipoSaneamento });
                });

            migrationBuilder.CreateTable(
                name: "dados_urbanizacao",
                schema: "ibge",
                columns: table => new
                {
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    TipoArea = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Populacao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_urbanizacao", x => new { x.MunicipioCodigoIbge, x.TipoArea });
                });

            migrationBuilder.CreateTable(
                name: "municipios",
                schema: "tse",
                columns: table => new
                {
                    CodigoIbge = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipios", x => x.CodigoIbge);
                });

            migrationBuilder.CreateTable(
                name: "partidos",
                schema: "tse",
                columns: table => new
                {
                    Numero = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partidos", x => x.Numero);
                });

            migrationBuilder.CreateTable(
                name: "perfis_eleitorado",
                schema: "tse",
                columns: table => new
                {
                    AnoEleicao = table.Column<int>(type: "integer", nullable: false),
                    NumeroZona = table.Column<int>(type: "integer", nullable: false),
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    FaixaEtaria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Escolaridade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Genero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    QuantidadeEleitores = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis_eleitorado", x => new { x.AnoEleicao, x.NumeroZona, x.MunicipioCodigoIbge, x.FaixaEtaria, x.Escolaridade, x.Genero });
                });

            migrationBuilder.CreateTable(
                name: "prestacoes_contas",
                schema: "tse",
                columns: table => new
                {
                    CandidatoCpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    AnoEleicao = table.Column<int>(type: "integer", nullable: false),
                    TipoReceita = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoMovimentacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CandidatoNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CandidatoNomeUrna = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CandidatoNumero = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PartidoNumero = table.Column<int>(type: "integer", nullable: false),
                    PartidoSigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PartidoNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prestacoes_contas", x => new { x.CandidatoCpf, x.AnoEleicao, x.TipoReceita, x.Descricao, x.TipoMovimentacao });
                });

            migrationBuilder.CreateTable(
                name: "resultados_eleitorais",
                schema: "tse",
                columns: table => new
                {
                    CandidatoCpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    AnoEleicao = table.Column<int>(type: "integer", nullable: false),
                    Turno = table.Column<int>(type: "integer", nullable: false),
                    NumeroZona = table.Column<int>(type: "integer", nullable: false),
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    NumeroSecao = table.Column<int>(type: "integer", nullable: false),
                    CandidatoNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CandidatoNomeUrna = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CandidatoNumero = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PartidoNumero = table.Column<int>(type: "integer", nullable: false),
                    PartidoSigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PartidoNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    QuantidadeVotos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultados_eleitorais", x => new { x.CandidatoCpf, x.AnoEleicao, x.Turno, x.NumeroZona, x.MunicipioCodigoIbge, x.NumeroSecao });
                });

            migrationBuilder.CreateTable(
                name: "secoes_eleitorais",
                schema: "tse",
                columns: table => new
                {
                    NumeroSecao = table.Column<int>(type: "integer", nullable: false),
                    NumeroZona = table.Column<int>(type: "integer", nullable: false),
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secoes_eleitorais", x => new { x.NumeroSecao, x.NumeroZona, x.MunicipioCodigoIbge });
                });

            migrationBuilder.CreateTable(
                name: "trimestres",
                schema: "pnad",
                columns: table => new
                {
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trimestres", x => new { x.Ano, x.Numero });
                });

            migrationBuilder.CreateTable(
                name: "ufs",
                schema: "tse",
                columns: table => new
                {
                    Sigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ufs", x => x.Sigla);
                });

            migrationBuilder.CreateTable(
                name: "zonas_eleitorais",
                schema: "tse",
                columns: table => new
                {
                    NumeroZona = table.Column<int>(type: "integer", nullable: false),
                    MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                    MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zonas_eleitorais", x => new { x.NumeroZona, x.MunicipioCodigoIbge });
                });

            migrationBuilder.CreateIndex(
                name: "IX_dados_desemprego_TrimestreAno_TrimestreNumero",
                schema: "pnad",
                table: "dados_desemprego",
                columns: new[] { "TrimestreAno", "TrimestreNumero" });

            migrationBuilder.CreateIndex(
                name: "IX_dados_escolaridade_UfSigla",
                schema: "ibge",
                table: "dados_escolaridade",
                column: "UfSigla");

            migrationBuilder.CreateIndex(
                name: "IX_dados_informalidade_TrimestreAno_TrimestreNumero",
                schema: "pnad",
                table: "dados_informalidade",
                columns: new[] { "TrimestreAno", "TrimestreNumero" });

            migrationBuilder.CreateIndex(
                name: "IX_dados_infraestrutura_UfSigla",
                schema: "ibge",
                table: "dados_infraestrutura",
                column: "UfSigla");

            migrationBuilder.CreateIndex(
                name: "IX_dados_populacionais_UfSigla",
                schema: "ibge",
                table: "dados_populacionais",
                column: "UfSigla");

            migrationBuilder.CreateIndex(
                name: "IX_dados_renda_UfSigla",
                schema: "ibge",
                table: "dados_renda",
                column: "UfSigla");

            migrationBuilder.CreateIndex(
                name: "IX_dados_renda_media_TrimestreAno_TrimestreNumero",
                schema: "pnad",
                table: "dados_renda_media",
                columns: new[] { "TrimestreAno", "TrimestreNumero" });

            migrationBuilder.CreateIndex(
                name: "IX_dados_saneamento_UfSigla",
                schema: "ibge",
                table: "dados_saneamento",
                column: "UfSigla");

            migrationBuilder.CreateIndex(
                name: "IX_dados_urbanizacao_UfSigla",
                schema: "ibge",
                table: "dados_urbanizacao",
                column: "UfSigla");

            migrationBuilder.CreateIndex(
                name: "IX_municipios_UfSigla_Nome",
                schema: "tse",
                table: "municipios",
                columns: new[] { "UfSigla", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_partidos_Sigla",
                schema: "tse",
                table: "partidos",
                column: "Sigla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_perfis_eleitorado_MunicipioCodigoIbge_AnoEleicao",
                schema: "tse",
                table: "perfis_eleitorado",
                columns: new[] { "MunicipioCodigoIbge", "AnoEleicao" });

            migrationBuilder.CreateIndex(
                name: "IX_resultados_eleitorais_MunicipioCodigoIbge_AnoEleicao",
                schema: "tse",
                table: "resultados_eleitorais",
                columns: new[] { "MunicipioCodigoIbge", "AnoEleicao" });

            migrationBuilder.CreateIndex(
                name: "IX_resultados_eleitorais_NumeroZona_MunicipioCodigoIbge_AnoEle~",
                schema: "tse",
                table: "resultados_eleitorais",
                columns: new[] { "NumeroZona", "MunicipioCodigoIbge", "AnoEleicao" });

            migrationBuilder.CreateIndex(
                name: "IX_resultados_eleitorais_PartidoNumero_AnoEleicao",
                schema: "tse",
                table: "resultados_eleitorais",
                columns: new[] { "PartidoNumero", "AnoEleicao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "anos_eleicao",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "bens_declarados",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "candidatos",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "coligacoes",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "dados_desemprego",
                schema: "pnad");

            migrationBuilder.DropTable(
                name: "dados_escolaridade",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "dados_informalidade",
                schema: "pnad");

            migrationBuilder.DropTable(
                name: "dados_infraestrutura",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "dados_populacionais",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "dados_renda",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "dados_renda_media",
                schema: "pnad");

            migrationBuilder.DropTable(
                name: "dados_saneamento",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "dados_urbanizacao",
                schema: "ibge");

            migrationBuilder.DropTable(
                name: "municipios",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "partidos",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "perfis_eleitorado",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "prestacoes_contas",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "resultados_eleitorais",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "secoes_eleitorais",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "trimestres",
                schema: "pnad");

            migrationBuilder.DropTable(
                name: "ufs",
                schema: "tse");

            migrationBuilder.DropTable(
                name: "zonas_eleitorais",
                schema: "tse");
        }
    }
}
