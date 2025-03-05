using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddExtrafieldsOnStyle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CTW",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CenterCaratId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CenterShapeId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenderId",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CTW",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CenterCaratId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CenterShapeId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "VenderId",
                table: "Product");
        }
    }
}
