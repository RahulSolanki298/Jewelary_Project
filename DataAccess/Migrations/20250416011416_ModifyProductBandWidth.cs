using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyProductBandWidth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldWeightId",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "GoldWeight",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldWeight",
                table: "Product");

            migrationBuilder.AddColumn<int>(
                name: "GoldWeightId",
                table: "Product",
                type: "int",
                nullable: true);
        }
    }
}
