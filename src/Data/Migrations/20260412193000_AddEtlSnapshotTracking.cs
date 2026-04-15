using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

[DbContext(typeof(PublicDataDbContext))]
[Migration("20260412193000_AddEtlSnapshotTracking")]
public partial class AddEtlSnapshotTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "etl");

        migrationBuilder.CreateTable(
            name: "cargas_brutas_auditorias",
            schema: "etl",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SnapshotId = table.Column<long>(type: "bigint", nullable: false),
                Fonte = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Dataset = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Escopo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ChaveSnapshot = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                HashSnapshot = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                NomeArquivoOriginal = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                CaminhoArquivoOriginal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                RegistrosImportados = table.Column<int>(type: "integer", nullable: false),
                RegistradoEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_cargas_brutas_auditorias", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "cargas_brutas_snapshots",
            schema: "etl",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Fonte = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Dataset = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Escopo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ChaveSnapshot = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                HashSnapshot = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                NomeArquivoOriginal = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                CaminhoArquivoOriginal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                RegistrosImportados = table.Column<int>(type: "integer", nullable: false),
                IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                RegistradoEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                SubstituidoEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_cargas_brutas_snapshots", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_cargas_brutas_auditorias_SnapshotId",
            schema: "etl",
            table: "cargas_brutas_auditorias",
            column: "SnapshotId");

        migrationBuilder.CreateIndex(
            name: "IX_cargas_brutas_snapshots_Fonte_Dataset_Escopo_HashSnapshot",
            schema: "etl",
            table: "cargas_brutas_snapshots",
            columns: new[] { "Fonte", "Dataset", "Escopo", "HashSnapshot" });

        migrationBuilder.CreateIndex(
            name: "IX_cargas_brutas_snapshots_Fonte_Dataset_Escopo_IsCurrent",
            schema: "etl",
            table: "cargas_brutas_snapshots",
            columns: new[] { "Fonte", "Dataset", "Escopo", "IsCurrent" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "cargas_brutas_auditorias",
            schema: "etl");

        migrationBuilder.DropTable(
            name: "cargas_brutas_snapshots",
            schema: "etl");
    }
}
