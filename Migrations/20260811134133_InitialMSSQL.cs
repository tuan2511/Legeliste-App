using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialMSSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnfangsbestandTiere = table.Column<int>(type: "int", nullable: false),
                    Einstallungsdatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StallId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    Verluste = table.Column<int>(type: "int", nullable: false),
                    Eier1Wahl = table.Column<int>(type: "int", nullable: false),
                    Eier2Wahl = table.Column<int>(type: "int", nullable: false),
                    FutterKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WasserLiter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FutterlieferungKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Bemerkungen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Auslaufzeit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LichtVon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LichtBis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eigewicht = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Koerpergewicht = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ZugaengeTiere = table.Column<int>(type: "int", nullable: true),
                    KontrollzeitenVon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KontrollzeitenBis = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyEntries_Stalls_StallId",
                        column: x => x.StallId,
                        principalTable: "Stalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyEntries_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DailyEntries_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyEntries_ApprovedById",
                table: "DailyEntries",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_DailyEntries_CreatorId",
                table: "DailyEntries",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyEntries_StallId_Date",
                table: "DailyEntries",
                columns: new[] { "StallId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyEntries");

            migrationBuilder.DropTable(
                name: "Stalls");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
