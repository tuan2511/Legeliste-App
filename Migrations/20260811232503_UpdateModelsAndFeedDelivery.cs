using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsAndFeedDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Auslaufzeit",
                table: "DailyEntries",
                newName: "AuslaufzeitMorgensVon");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AuslaufzeitAbendsBis",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuslaufzeitAbendsVon",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuslaufzeitMorgensBis",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DailyEntries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FutterWasserZyklusBis",
                table: "DailyEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IstAusgestallt",
                table: "DailyEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FeedDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountTons = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedDeliveries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedDeliveries");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuslaufzeitAbendsBis",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "AuslaufzeitAbendsVon",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "AuslaufzeitMorgensBis",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "FutterWasserZyklusBis",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "IstAusgestallt",
                table: "DailyEntries");

            migrationBuilder.RenameColumn(
                name: "AuslaufzeitMorgensVon",
                table: "DailyEntries",
                newName: "Auslaufzeit");
        }
    }
}
