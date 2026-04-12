using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

[DbContext(typeof(PublicDataDbContext))]
[Migration("20260412170000_AddIbgeSpatialModel")]
public partial class AddIbgeSpatialModel : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS postgis;");

        migrationBuilder.CreateTable(
            name: "setores_censitarios",
            schema: "ibge",
            columns: table => new
            {
                CodigoSetor = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                MunicipioCodigoIbge = table.Column<int>(type: "integer", nullable: false),
                MunicipioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UfSigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                Geometria = table.Column<NetTopologySuite.Geometries.MultiPolygon>(type: "geometry(MultiPolygon,4674)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_setores_censitarios", x => x.CodigoSetor);
            });

        migrationBuilder.CreateIndex(
            name: "IX_setores_censitarios_Geometria",
            schema: "ibge",
            table: "setores_censitarios",
            column: "Geometria")
            .Annotation("Npgsql:IndexMethod", "gist");

        migrationBuilder.CreateIndex(
            name: "IX_setores_censitarios_MunicipioCodigoIbge",
            schema: "ibge",
            table: "setores_censitarios",
            column: "MunicipioCodigoIbge");

        migrationBuilder.CreateIndex(
            name: "IX_setores_censitarios_UfSigla",
            schema: "ibge",
            table: "setores_censitarios",
            column: "UfSigla");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "setores_censitarios",
            schema: "ibge");
    }
}
