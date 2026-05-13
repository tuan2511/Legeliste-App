using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class SplitLichtKontrollzeiten : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnzahlSelektiert",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "AnzahlVerendet",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "BodenEier",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Brucheier",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Doppeldotter",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Knickeier",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Lichtstunden",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Schmutzeier",
                table: "DailyEntries");

            migrationBuilder.RenameColumn(
                name: "Windeier",
                table: "DailyEntries",
                newName: "Verluste");

            migrationBuilder.RenameColumn(
                name: "Kontrollzeiten",
                table: "DailyEntries",
                newName: "LichtVon");

            migrationBuilder.AlterColumn<decimal>(
                name: "Koerpergewicht",
                table: "DailyEntries",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Eigewicht",
                table: "DailyEntries",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Bemerkungen",
                table: "DailyEntries",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Auslaufzeit",
                table: "DailyEntries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KontrollzeitenBis",
                table: "DailyEntries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KontrollzeitenVon",
                table: "DailyEntries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LichtBis",
                table: "DailyEntries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KontrollzeitenBis",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "KontrollzeitenVon",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "LichtBis",
                table: "DailyEntries");

            migrationBuilder.RenameColumn(
                name: "Verluste",
                table: "DailyEntries",
                newName: "Windeier");

            migrationBuilder.RenameColumn(
                name: "LichtVon",
                table: "DailyEntries",
                newName: "Kontrollzeiten");

            migrationBuilder.AlterColumn<decimal>(
                name: "Koerpergewicht",
                table: "DailyEntries",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Eigewicht",
                table: "DailyEntries",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bemerkungen",
                table: "DailyEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Auslaufzeit",
                table: "DailyEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnzahlSelektiert",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnzahlVerendet",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BodenEier",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Brucheier",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Doppeldotter",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Knickeier",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Lichtstunden",
                table: "DailyEntries",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Schmutzeier",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
