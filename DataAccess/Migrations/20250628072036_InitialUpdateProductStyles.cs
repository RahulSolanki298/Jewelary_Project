using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialUpdateProductStyles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisplayHome",
                table: "ProductStyles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplayHome",
                table: "ProductCollections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProductType",
                table: "ProductCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplayHome",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisplayHome",
                table: "ProductStyles");

            migrationBuilder.DropColumn(
                name: "IsDisplayHome",
                table: "ProductCollections");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "ProductCollections");

            migrationBuilder.DropColumn(
                name: "IsDisplayHome",
                table: "Category");
        }
    }
}
