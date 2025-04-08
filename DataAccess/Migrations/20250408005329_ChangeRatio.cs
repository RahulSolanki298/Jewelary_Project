using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ChangeRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatioId",
                table: "Diamonds");

            migrationBuilder.AddColumn<decimal>(
                name: "Ratio",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ratio",
                table: "Diamonds");

            migrationBuilder.AddColumn<int>(
                name: "RatioId",
                table: "Diamonds",
                type: "int",
                nullable: true);
        }
    }
}
