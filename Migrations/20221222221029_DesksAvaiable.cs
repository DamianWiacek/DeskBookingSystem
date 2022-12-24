using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeskBookingSystem.Migrations
{
    public partial class DesksAvaiable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Desks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Desks");
        }
    }
}
