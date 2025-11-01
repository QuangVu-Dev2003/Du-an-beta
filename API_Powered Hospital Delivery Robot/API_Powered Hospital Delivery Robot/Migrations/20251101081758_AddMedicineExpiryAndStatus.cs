using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace API_Powered_Hospital_Delivery_Robot.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineExpiryAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "expiry_date",
                table: "medicines",
                type: "date",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "medicines",
                type: "enum('active','expired')",
                nullable: false,
                defaultValue: "active",
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expiry_date",
                table: "medicines");
            migrationBuilder.DropColumn(
                name: "status",
                table: "medicines");
        }
    }
}