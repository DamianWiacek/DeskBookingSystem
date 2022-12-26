using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeskBookingSystem.Migrations
{
    public partial class ReservationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservationDate",
                table: "Reservations",
                newName: "ReservationStart");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationEnd",
                table: "Reservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationEnd",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "ReservationStart",
                table: "Reservations",
                newName: "ReservationDate");
        }
    }
}
