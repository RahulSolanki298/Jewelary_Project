using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyDiamondsCarat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaratId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "CaratSize",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "CaratSizeId",
                table: "Diamonds");

            migrationBuilder.AddColumn<decimal>(
                name: "Carat",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carat",
                table: "Diamonds");

            migrationBuilder.AddColumn<int>(
                name: "CaratId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaratSize",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CaratSizeId",
                table: "Diamonds",
                type: "int",
                nullable: true);
        }
    }
}
