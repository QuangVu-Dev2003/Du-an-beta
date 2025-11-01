using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Powered_Hospital_Delivery_Robot.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "priority",
                table: "tasks",
                type: "enum('Normal','Urgent','Critical')",
                nullable: false,
                defaultValue: "Normal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "priority",
                table: "tasks");
        }
    }
}
