using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropertyReservedByToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_AspNetUsers_ReservedById",
                table: "RoomReservations");

            migrationBuilder.DropIndex(
                name: "IX_RoomReservations_ReservedById",
                table: "RoomReservations");

            migrationBuilder.DropColumn(
                name: "ReservedById",
                table: "RoomReservations");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_UserId",
                table: "RoomReservations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_AspNetUsers_UserId",
                table: "RoomReservations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_AspNetUsers_UserId",
                table: "RoomReservations");

            migrationBuilder.DropIndex(
                name: "IX_RoomReservations_UserId",
                table: "RoomReservations");

            migrationBuilder.AddColumn<int>(
                name: "ReservedById",
                table: "RoomReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_ReservedById",
                table: "RoomReservations",
                column: "ReservedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_AspNetUsers_ReservedById",
                table: "RoomReservations",
                column: "ReservedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
