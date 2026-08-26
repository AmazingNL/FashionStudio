using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FashionStudio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkSpaceMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_WorkSpaces_WorkSpaceId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSpaces_Users_OwnerId",
                table: "WorkSpaces");

            migrationBuilder.DropIndex(
                name: "IX_WorkSpaces_OwnerId",
                table: "WorkSpaces");

            migrationBuilder.DropIndex(
                name: "IX_Users_WorkSpaceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "WorkSpaces");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkSpaceId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "WorkSpaceMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkSpaceId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSpaceMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSpaceMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkSpaceMemberships_WorkSpaces_WorkSpaceId",
                        column: x => x.WorkSpaceId,
                        principalTable: "WorkSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkSpaceMemberships_UserId",
                table: "WorkSpaceMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSpaceMemberships_WorkSpaceId",
                table: "WorkSpaceMemberships",
                column: "WorkSpaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkSpaceMemberships");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "WorkSpaces",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkSpaceId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSpaces_OwnerId",
                table: "WorkSpaces",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_WorkSpaceId",
                table: "Users",
                column: "WorkSpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_WorkSpaces_WorkSpaceId",
                table: "Users",
                column: "WorkSpaceId",
                principalTable: "WorkSpaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSpaces_Users_OwnerId",
                table: "WorkSpaces",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
