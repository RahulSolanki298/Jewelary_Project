using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddNoOfStoneInStyles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldPurity",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "GoldWeight",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "CTW",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoldPurityId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoldWeightId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfStones",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldPurityId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "GoldWeightId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "NoOfStones",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "CTW",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoldPurity",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoldWeight",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
