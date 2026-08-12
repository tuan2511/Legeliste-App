using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegelisteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddStallIdToFeedDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StallId",
                table: "FeedDeliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FeedDeliveries_StallId",
                table: "FeedDeliveries",
                column: "StallId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedDeliveries_Stalls_StallId",
                table: "FeedDeliveries",
                column: "StallId",
                principalTable: "Stalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedDeliveries_Stalls_StallId",
                table: "FeedDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_FeedDeliveries_StallId",
                table: "FeedDeliveries");

            migrationBuilder.DropColumn(
                name: "StallId",
                table: "FeedDeliveries");
        }
    }
}
