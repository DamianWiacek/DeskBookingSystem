using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeskBookingSystem.Migrations
{
    public partial class relationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Locations_LocationId",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Reservations",
                newName: "DeskId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_LocationId",
                table: "Reservations",
                newName: "IX_Reservations_DeskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Desks_DeskId",
                table: "Reservations",
                column: "DeskId",
                principalTable: "Desks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Desks_DeskId",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "DeskId",
                table: "Reservations",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_DeskId",
                table: "Reservations",
                newName: "IX_Reservations_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Locations_LocationId",
                table: "Reservations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
