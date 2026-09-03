using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStudio.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixFittingCustomerReferenceAndOutcomeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fittings_Users_CustomerId",
                table: "Fittings");

            migrationBuilder.RenameColumn(
                name: "Approved",
                table: "Fittings",
                newName: "Outcome");

            migrationBuilder.AddForeignKey(
                name: "FK_Fittings_Customers_CustomerId",
                table: "Fittings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fittings_Customers_CustomerId",
                table: "Fittings");

            migrationBuilder.RenameColumn(
                name: "Outcome",
                table: "Fittings",
                newName: "Approved");

            migrationBuilder.AddForeignKey(
                name: "FK_Fittings_Users_CustomerId",
                table: "Fittings",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
