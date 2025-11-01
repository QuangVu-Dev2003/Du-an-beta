using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Powered_Hospital_Delivery_Robot.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionItemIdToAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "prescription_item_id",
                table: "alerts",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_alerts_prescription_item_id",
                table: "alerts",
                column: "prescription_item_id");

            migrationBuilder.AddForeignKey(
                name: "FK_alerts_prescription_items_prescription_item_id",
                table: "alerts",
                column: "prescription_item_id",
                principalTable: "prescription_items",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alerts_prescription_items_prescription_item_id",
                table: "alerts");

            migrationBuilder.DropIndex(
                name: "IX_alerts_prescription_item_id",
                table: "alerts");

            migrationBuilder.DropColumn(
                name: "prescription_item_id",
                table: "alerts");
        }
    }
}
