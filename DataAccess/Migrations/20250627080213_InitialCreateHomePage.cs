using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialCreateHomePage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogIds",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryIds",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollectionIds",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiamondIds",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageFrontImg",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageName",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StylesIds",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogIds",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "CategoryIds",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "CollectionIds",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "DiamondIds",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "PageFrontImg",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "PageName",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "StylesIds",
                table: "HomePageSetting");
        }
    }
}
