using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialUpdateForDiamonds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DNA",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Measurement",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RAP",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RapAmount",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Step",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoneId",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Diamond",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabId = table.Column<int>(type: "int", nullable: true),
                    LabName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CutId = table.Column<int>(type: "int", nullable: true),
                    CutName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PolishId = table.Column<int>(type: "int", nullable: true),
                    PolishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SymmetryId = table.Column<int>(type: "int", nullable: true),
                    SymmetryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CaratSizeId = table.Column<int>(type: "int", nullable: true),
                    CaratSizeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClarityId = table.Column<int>(type: "int", nullable: true),
                    ClarityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    ColorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShapeId = table.Column<int>(type: "int", nullable: true),
                    ShapeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableId = table.Column<int>(type: "int", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepthId = table.Column<int>(type: "int", nullable: true),
                    DepthName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatioId = table.Column<int>(type: "int", nullable: true),
                    RatioName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarketDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MDisc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MAmt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertiType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TableWhite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SideWhite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableBlack = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SideBlack = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PavOpen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GirdleOpen = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CAngle = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PAngle = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PHt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CHt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Girdle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GirdleDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CrownExFac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PavExFac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableSpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SideSpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NT_INT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Culet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FluorId = table.Column<int>(type: "int", nullable: true),
                    FluorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Milky = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Luster = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Graining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RatePct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiamondImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiamondVideoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OLD_PID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ORAP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MfgRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InwDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PriceNameId = table.Column<int>(type: "int", nullable: true),
                    PriceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EyeClean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: true),
                    StrLan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LrHalf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyToSymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabShape = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenTable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenCrown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenPavallion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenGirdle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NT_OR_INT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pav_Ex_Fac = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diamond");

            migrationBuilder.DropColumn(
                name: "DNA",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Measurement",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "RAP",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "RapAmount",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "StoneId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Diamonds");
        }
    }
}
