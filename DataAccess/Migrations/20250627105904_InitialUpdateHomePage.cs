using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialUpdateHomePage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slider1MetaUrl",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slider1Title",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slider2MetaUrl",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slider2Title",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slider3MetaUrl",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slider3Title",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VdoMetaUrl",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VdoTitle",
                table: "HomePageSetting",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slider1MetaUrl",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "Slider1Title",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "Slider2MetaUrl",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "Slider2Title",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "Slider3MetaUrl",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "Slider3Title",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "VdoMetaUrl",
                table: "HomePageSetting");

            migrationBuilder.DropColumn(
                name: "VdoTitle",
                table: "HomePageSetting");
        }
    }
}
