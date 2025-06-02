using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddProductAddedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsHistoryId",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SymbolName",
                table: "DiamondProperties",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DiamondProperties",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IconPath",
                table: "DiamondProperties",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ColorType",
                table: "DiamondProperties",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DiamondHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoneId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DNA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Step = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabShape = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RAP = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RapAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LabId = table.Column<int>(type: "int", nullable: true),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CutId = table.Column<int>(type: "int", nullable: true),
                    PolishId = table.Column<int>(type: "int", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SymmetryId = table.Column<int>(type: "int", nullable: true),
                    Shade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClarityId = table.Column<int>(type: "int", nullable: true),
                    Clarity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    ShapeId = table.Column<int>(type: "int", nullable: true),
                    Table = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Depth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Ratio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    INWDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MarketDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertiType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flo = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    CrownExFac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PavExFac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableSpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SideSpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NT_INT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Culet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FluorId = table.Column<int>(type: "int", nullable: true),
                    Milky = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Luster = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Graining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RatePct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiamondImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiamondVideoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OLD_PID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ORAP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MfgRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceNameId = table.Column<int>(type: "int", nullable: true),
                    MDisc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MAmt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EyeClean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StrLan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LrHalf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyToSymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenTable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenCrown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenPavallion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenGirdle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NT_OR_INT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pav_Ex_Fac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GirdleDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: true),
                    UploadStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HistoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiamondHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DesignNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentDesign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VenderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occasion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Package = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MfgDesign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CadDesigner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Length = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaratId = table.Column<int>(type: "int", nullable: true),
                    BandWidth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoldPurityId = table.Column<int>(type: "int", nullable: true),
                    GoldWeight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CTW = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CenterShapeId = table.Column<int>(type: "int", nullable: true),
                    CenterCaratId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SubCategoryId = table.Column<int>(type: "int", nullable: true),
                    CaratId = table.Column<int>(type: "int", nullable: true),
                    CaratSizeId = table.Column<int>(type: "int", nullable: true),
                    ClarityId = table.Column<int>(type: "int", nullable: true),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    ShapeId = table.Column<int>(type: "int", nullable: true),
                    StyleId = table.Column<int>(type: "int", nullable: true),
                    CollectionsId = table.Column<int>(type: "int", nullable: true),
                    DiaWT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MMSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grades = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfStones = table.Column<int>(type: "int", nullable: false),
                    Component = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Setting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    IsReadyforShip = table.Column<bool>(type: "bit", nullable: true),
                    AccentStoneShapeId = table.Column<int>(type: "int", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
                    VenderStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WholesaleCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Diameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFileUploadHistoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    ProductFileUploadHistoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductHistory_ProductFileUploadHistory_ProductFileUploadHistoryId",
                        column: x => x.ProductFileUploadHistoryId,
                        principalTable: "ProductFileUploadHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductHistory_ProductFileUploadHistoryId",
                table: "ProductHistory",
                column: "ProductFileUploadHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiamondHistory");

            migrationBuilder.DropTable(
                name: "ProductHistory");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHistoryId",
                table: "Product",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SymbolName",
                table: "DiamondProperties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DiamondProperties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IconPath",
                table: "DiamondProperties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ColorType",
                table: "DiamondProperties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
