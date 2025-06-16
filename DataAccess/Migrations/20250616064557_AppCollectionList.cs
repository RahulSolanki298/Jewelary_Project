using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AppCollectionList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductCollections");

            migrationBuilder.CreateTable(
                name: "DiamondData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    StoneId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DNA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Step = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabShape = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabId = table.Column<int>(type: "int", nullable: true),
                    LabName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShapeId = table.Column<int>(type: "int", nullable: true),
                    ShapeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RAP = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RapAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Carat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClarityId = table.Column<int>(type: "int", nullable: true),
                    ClarityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    ColorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CutId = table.Column<int>(type: "int", nullable: true),
                    CutName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PolishId = table.Column<int>(type: "int", nullable: true),
                    PolishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SymmetryId = table.Column<int>(type: "int", nullable: true),
                    SymmetyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FluorId = table.Column<int>(type: "int", nullable: true),
                    FluorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Table = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Depth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Ratio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Shade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RatePct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiamondImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiamondVideoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispUpdatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiamondData");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductCollections",
                type: "int",
                nullable: true);
        }
    }
}
