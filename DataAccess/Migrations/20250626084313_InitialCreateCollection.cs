using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialCreateCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverPageTitle",
                table: "ProductStyles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductStyles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverPageTitle",
                table: "ProductCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductCollections",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPageTitle",
                table: "ProductStyles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductStyles");

            migrationBuilder.DropColumn(
                name: "CoverPageTitle",
                table: "ProductCollections");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductCollections");
        }
    }
}
