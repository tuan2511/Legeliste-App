using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class NovogenUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Verluste",
                table: "DailyEntries",
                newName: "Schmutzeier");

            migrationBuilder.AddColumn<DateTime>(
                name: "Einstallungsdatum",
                table: "Stalls",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "Knickeier",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Einstallungsdatum",
                table: "Stalls");

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
                name: "Knickeier",
                table: "DailyEntries");

            migrationBuilder.RenameColumn(
                name: "Schmutzeier",
                table: "DailyEntries",
                newName: "Verluste");
        }
    }
}
