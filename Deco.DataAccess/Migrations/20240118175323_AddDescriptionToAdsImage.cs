using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToAdsImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AdsImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AdsImages");
        }
    }
}
