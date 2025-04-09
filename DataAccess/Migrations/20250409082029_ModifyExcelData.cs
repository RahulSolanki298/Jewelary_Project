using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyExcelData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaratId",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CaratName",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "RatioId",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "RatioName",
                table: "Diamond");

            migrationBuilder.AddColumn<string>(
                name: "CadDesigner",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignNo",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfgDesign",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occasion",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Package",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentDesignNo",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductCollectionsId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductSize",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Carat",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ratio",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCollectionsId",
                table: "Product",
                column: "ProductCollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductCollections_ProductCollectionsId",
                table: "Product",
                column: "ProductCollectionsId",
                principalTable: "ProductCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductCollections_ProductCollectionsId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ProductCollectionsId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CadDesigner",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DesignNo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MfgDesign",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Occasion",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Package",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ParentDesignNo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductCollectionsId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductSize",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Carat",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Ratio",
                table: "Diamond");

            migrationBuilder.AddColumn<int>(
                name: "CaratId",
                table: "Diamond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaratName",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatioId",
                table: "Diamond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RatioName",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
