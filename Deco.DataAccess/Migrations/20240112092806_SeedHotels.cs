using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Deco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedHotels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "Province", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "Burkley", "Mariatt", "522-488-1245", "544-765", "Iowa", "19/1 LakeVille South Wesley 5 Rd." },
                    { 2, "Weiling", "Grande", "134-570-5701", "125-987", "Toronto", "5 SpringFiled GoldenCrown 8 Rd." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
