﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionIdToOrderHeaderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sessionId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sessionId",
                table: "OrderHeaders");
        }
    }
}