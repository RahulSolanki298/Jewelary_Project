using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdateDataWithExistingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Flor",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Polish",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Shape",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Sym",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Table",
                table: "Diamonds");

            migrationBuilder.AlterColumn<int>(
                name: "Ratio",
                table: "Diamonds",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Depth",
                table: "Diamonds",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "Diamonds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FluorId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PolishId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShapeId",
                table: "Diamonds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Symmetry",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TableId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "CustomerOrderStatus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "FluorId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "PolishId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "ShapeId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Symmetry",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "TableId",
                table: "Diamonds");

            migrationBuilder.AlterColumn<decimal>(
                name: "Ratio",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Depth",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flor",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Polish",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shape",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sym",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Table",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "CustomerOrderStatus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
