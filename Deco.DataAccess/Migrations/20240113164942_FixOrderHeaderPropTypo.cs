using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderHeaderPropTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sessionId",
                table: "OrderHeaders",
                newName: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "OrderHeaders",
                newName: "sessionId");
        }
    }
}
