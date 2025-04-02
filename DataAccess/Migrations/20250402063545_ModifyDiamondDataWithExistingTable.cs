using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyDiamondDataWithExistingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Symmetry",
                table: "Diamonds",
                newName: "SymmetryId");

            migrationBuilder.RenameColumn(
                name: "Ratio",
                table: "Diamonds",
                newName: "RatioId");

            migrationBuilder.RenameColumn(
                name: "Depth",
                table: "Diamonds",
                newName: "PriceNameId");

            migrationBuilder.RenameColumn(
                name: "Carat",
                table: "Diamonds",
                newName: "ORAP");

            migrationBuilder.AddColumn<string>(
                name: "CaratSize",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CaratSizeId",
                table: "Diamonds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClarityId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepthId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiamondImagePath",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiamondVideoPath",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flo",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LabId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfgRemark",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OLD_PID",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaratSize",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "CaratSizeId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "ClarityId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "DepthId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "DiamondImagePath",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "DiamondVideoPath",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Flo",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "LabId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "MfgRemark",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "OLD_PID",
                table: "Diamonds");

            migrationBuilder.RenameColumn(
                name: "SymmetryId",
                table: "Diamonds",
                newName: "Symmetry");

            migrationBuilder.RenameColumn(
                name: "RatioId",
                table: "Diamonds",
                newName: "Ratio");

            migrationBuilder.RenameColumn(
                name: "PriceNameId",
                table: "Diamonds",
                newName: "Depth");

            migrationBuilder.RenameColumn(
                name: "ORAP",
                table: "Diamonds",
                newName: "Carat");
        }
    }
}
