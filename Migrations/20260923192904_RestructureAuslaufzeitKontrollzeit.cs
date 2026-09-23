using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class RestructureAuslaufzeitKontrollzeit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KontrollzeitenVon",
                table: "DailyEntries",
                newName: "Kontrollzeit4");

            migrationBuilder.RenameColumn(
                name: "KontrollzeitenBis",
                table: "DailyEntries",
                newName: "Kontrollzeit3");

            migrationBuilder.RenameColumn(
                name: "AuslaufzeitMorgensVon",
                table: "DailyEntries",
                newName: "Auslaufzeit4Von");

            migrationBuilder.RenameColumn(
                name: "AuslaufzeitMorgensBis",
                table: "DailyEntries",
                newName: "Auslaufzeit4Bis");

            migrationBuilder.RenameColumn(
                name: "AuslaufzeitAbendsVon",
                table: "DailyEntries",
                newName: "Auslaufzeit3Von");

            migrationBuilder.RenameColumn(
                name: "AuslaufzeitAbendsBis",
                table: "DailyEntries",
                newName: "Auslaufzeit3Bis");

            migrationBuilder.AddColumn<string>(
                name: "Auslaufzeit1Bis",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auslaufzeit1Von",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auslaufzeit2Bis",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auslaufzeit2Von",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kontrollzeit1",
                table: "DailyEntries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kontrollzeit2",
                table: "DailyEntries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auslaufzeit1Bis",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Auslaufzeit1Von",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Auslaufzeit2Bis",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Auslaufzeit2Von",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Kontrollzeit1",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Kontrollzeit2",
                table: "DailyEntries");

            migrationBuilder.RenameColumn(
                name: "Kontrollzeit4",
                table: "DailyEntries",
                newName: "KontrollzeitenVon");

            migrationBuilder.RenameColumn(
                name: "Kontrollzeit3",
                table: "DailyEntries",
                newName: "KontrollzeitenBis");

            migrationBuilder.RenameColumn(
                name: "Auslaufzeit4Von",
                table: "DailyEntries",
                newName: "AuslaufzeitMorgensVon");

            migrationBuilder.RenameColumn(
                name: "Auslaufzeit4Bis",
                table: "DailyEntries",
                newName: "AuslaufzeitMorgensBis");

            migrationBuilder.RenameColumn(
                name: "Auslaufzeit3Von",
                table: "DailyEntries",
                newName: "AuslaufzeitAbendsVon");

            migrationBuilder.RenameColumn(
                name: "Auslaufzeit3Bis",
                table: "DailyEntries",
                newName: "AuslaufzeitAbendsBis");
        }
    }
}
