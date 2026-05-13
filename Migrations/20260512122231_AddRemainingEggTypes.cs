using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingEggTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Windeier",
                table: "DailyEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brucheier",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Doppeldotter",
                table: "DailyEntries");

            migrationBuilder.DropColumn(
                name: "Windeier",
                table: "DailyEntries");
        }
    }
}
