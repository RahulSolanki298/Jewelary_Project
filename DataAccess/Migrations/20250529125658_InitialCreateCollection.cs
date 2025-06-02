using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialCreateCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductStyles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StyleImage",
                table: "ProductStyles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StyleId",
                table: "ProductStyleItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductCollections",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductStyles");

            migrationBuilder.DropColumn(
                name: "StyleImage",
                table: "ProductStyles");

            migrationBuilder.DropColumn(
                name: "StyleId",
                table: "ProductStyleItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductCollections");
        }
    }
}
