using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFKToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_HotelId",
                table: "AspNetUsers",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hotels_HotelId",
                table: "AspNetUsers",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hotels_HotelId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_HotelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "AspNetUsers");
        }
    }
}
