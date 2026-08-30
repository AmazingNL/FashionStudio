using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStudio.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeMeasurementSetWorkSpaceIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementSets_WorkSpaces_WorkSpaceId",
                table: "MeasurementSets");

            migrationBuilder.AlterColumn<int>(
                name: "WorkSpaceId",
                table: "MeasurementSets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementSets_WorkSpaces_WorkSpaceId",
                table: "MeasurementSets",
                column: "WorkSpaceId",
                principalTable: "WorkSpaces",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementSets_WorkSpaces_WorkSpaceId",
                table: "MeasurementSets");

            migrationBuilder.AlterColumn<int>(
                name: "WorkSpaceId",
                table: "MeasurementSets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementSets_WorkSpaces_WorkSpaceId",
                table: "MeasurementSets",
                column: "WorkSpaceId",
                principalTable: "WorkSpaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
