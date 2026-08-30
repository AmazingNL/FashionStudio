using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStudio.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeCustomerWorkSpaceIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_WorkSpaces_WorkSpaceId",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "WorkSpaceId",
                table: "Customers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_WorkSpaces_WorkSpaceId",
                table: "Customers",
                column: "WorkSpaceId",
                principalTable: "WorkSpaces",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_WorkSpaces_WorkSpaceId",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "WorkSpaceId",
                table: "Customers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_WorkSpaces_WorkSpaceId",
                table: "Customers",
                column: "WorkSpaceId",
                principalTable: "WorkSpaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
