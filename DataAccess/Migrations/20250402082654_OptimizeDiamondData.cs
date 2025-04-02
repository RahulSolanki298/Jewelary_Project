using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class OptimizeDiamondData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cut",
                table: "Diamonds",
                newName: "StrLan");

            migrationBuilder.AddColumn<int>(
                name: "CutId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Diam",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EyeClean",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GirdleDesc",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyToSymbol",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabComment",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabShape",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LrHalf",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MAmt",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MDisc",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MRate",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NT_OR_INT",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenCrown",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenGirdle",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenPavallion",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTable",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pav_Ex_Fac",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CutId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Diam",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "EyeClean",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "GirdleDesc",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "KeyToSymbol",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "LabComment",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "LabShape",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "LrHalf",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "MAmt",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "MDisc",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "MRate",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "NT_OR_INT",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "OpenCrown",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "OpenGirdle",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "OpenPavallion",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "OpenTable",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Pav_Ex_Fac",
                table: "Diamonds");

            migrationBuilder.RenameColumn(
                name: "StrLan",
                table: "Diamonds",
                newName: "Cut");
        }
    }
}
