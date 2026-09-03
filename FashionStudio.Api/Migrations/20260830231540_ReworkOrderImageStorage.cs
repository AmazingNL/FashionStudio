using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStudio.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReworkOrderImageStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "OrderImages",
                newName: "StoredFileName");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "OrderImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderImages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "OrderImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderImages");

            migrationBuilder.RenameColumn(
                name: "StoredFileName",
                table: "OrderImages",
                newName: "ImageUrl");
        }
    }
}
