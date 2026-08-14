using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class FeedDelivery_StallNullable_AmountKg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedDeliveries_Stalls_StallId",
                table: "FeedDeliveries");

            migrationBuilder.RenameColumn(
                name: "AmountTons",
                table: "FeedDeliveries",
                newName: "AmountKg");

            migrationBuilder.AlterColumn<int>(
                name: "StallId",
                table: "FeedDeliveries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedDeliveries_Stalls_StallId",
                table: "FeedDeliveries",
                column: "StallId",
                principalTable: "Stalls",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedDeliveries_Stalls_StallId",
                table: "FeedDeliveries");

            migrationBuilder.RenameColumn(
                name: "AmountKg",
                table: "FeedDeliveries",
                newName: "AmountTons");

            migrationBuilder.AlterColumn<int>(
                name: "StallId",
                table: "FeedDeliveries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedDeliveries_Stalls_StallId",
                table: "FeedDeliveries",
                column: "StallId",
                principalTable: "Stalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
