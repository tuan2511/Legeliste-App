using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auslaufzeit",
                table: "DailyEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Eigewicht",
                table: "DailyEntries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Koerpergewicht",
                table: "DailyEntries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ZugaengeTiere",
                table: "DailyEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KontrollzeitenVon",
                table: "DailyEntries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KontrollzeitenBis",
                table: "DailyEntries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Auslaufzeit", table: "DailyEntries");

            migrationBuilder.DropColumn(name: "Eigewicht", table: "DailyEntries");
            migrationBuilder.DropColumn(name: "Koerpergewicht", table: "DailyEntries");
            migrationBuilder.DropColumn(name: "ZugaengeTiere", table: "DailyEntries");
            migrationBuilder.DropColumn(name: "KontrollzeitenVon", table: "DailyEntries");
            migrationBuilder.DropColumn(name: "KontrollzeitenBis", table: "DailyEntries");
        }
    }
}
