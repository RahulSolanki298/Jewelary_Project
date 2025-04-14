using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyCOlumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BandWidthId",
                table: "Product");

            migrationBuilder.AddColumn<decimal>(
                name: "BandWidth",
                table: "Product",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BandWidth",
                table: "Product");

            migrationBuilder.AddColumn<int>(
                name: "BandWidthId",
                table: "Product",
                type: "int",
                nullable: true);
        }
    }
}
