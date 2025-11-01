using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Powered_Hospital_Delivery_Robot.Migrations
{
    /// <inheritdoc />
    public partial class Fix_MedicineStatus_And_ExpiryDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "expiry_date",
                table: "medicines",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "expiry_date",
                table: "medicines",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
