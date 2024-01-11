using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Deco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAndSeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    SetPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price", "SetPrice" },
                values: new object[,]
                {
                    { 1, "Drawn by Van Goh", "Stary Night", 55000.0, 45000.0 },
                    { 2, "Famous screaming person", "Scream", 65000.0, 55000.0 },
                    { 3, "Fox fur carpet 8*5M", "Fox fur 8*5", 3500.0, 3000.0 },
                    { 4, "Bear fur carpet 9*10M", "Bear fur 9*10", 5000.0, 4000.0 },
                    { 5, "White Cabinet 170*40*70", "Tall White Cab170", 6000.0, 5000.0 },
                    { 6, "Dark Cabinet 45*40*70", "Short Dark Cab45", 2000.0, 1000.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
