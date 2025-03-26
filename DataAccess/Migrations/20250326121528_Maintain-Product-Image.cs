using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class MaintainProductImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LengthId",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "Length",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Product");

            migrationBuilder.AddColumn<int>(
                name: "LengthId",
                table: "Product",
                type: "int",
                nullable: true);
        }
    }
}
