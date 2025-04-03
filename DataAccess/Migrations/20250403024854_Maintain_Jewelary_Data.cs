using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class Maintain_Jewelary_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ShapeId",
                table: "Diamonds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ColorId",
                table: "Diamonds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CaratSizeId",
                table: "Diamonds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            string procedure = @"
        CREATE PROCEDURE InsertDiamondsFromJson
    @JsonData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert into the Diamond table using OPENJSON
    INSERT INTO Diamonds(
        ReportType, LotNo, LabDate, LabId, Quality, LotType, CutId, PolishId, Sku, SymmetryId,
        Shade, HA, UnitPrice, Price, Quantity, CaratSizeId, CaratSize, ClarityId, Clarity, ColorId,
        ShapeId, TableId, DepthId, RatioId, Length, Width, Height, Location, Status, INWDate,
        MarketDate, ReportDate, Type, CertificateNo, Stock, CertiType, DisplayOrder, Flo, Dia,
        TableWhite, SideWhite, TableBlack, SideBlack, PavOpen, GirdleOpen, CAngle, PAngle, PHt, CHt,
        Girdle, CrownExFac, PavExFac, TableSpot, SideSpot, NT_INT, Culet, FluorId, Milky, Luster,
        Graining, DaysType, Discount, RatePct, Amount, DiamondImagePath, DiamondVideoPath, Diam,
        OLD_PID, ORAP, MfgRemark, PriceNameId, MDisc, MRate, MAmt, EyeClean, StrLan, LrHalf,
        KeyToSymbol, LabComment, LabShape, OpenTable, OpenCrown, OpenPavallion, OpenGirdle,
        NT_OR_INT, Pav_Ex_Fac, GirdleDesc, IsActivated
    )
    SELECT 
        ReportType, LotNo, LabDate, LabId, Quality, LotType, CutId, PolishId, Sku, SymmetryId,
        Shade, HA, UnitPrice, Price, Quantity, CaratSizeId, CaratSize, ClarityId, Clarity, ColorId,
        ShapeId, TableId, DepthId, RatioId, Length, Width, Height, Location, Status, INWDate,
        MarketDate, ReportDate, Type, CertificateNo, Stock, CertiType, DisplayOrder, Flo, Dia,
        TableWhite, SideWhite, TableBlack, SideBlack, PavOpen, GirdleOpen, CAngle, PAngle, PHt, CHt,
        Girdle, CrownExFac, PavExFac, TableSpot, SideSpot, NT_INT, Culet, FluorId, Milky, Luster,
        Graining, DaysType, Discount, RatePct, Amount, DiamondImagePath, DiamondVideoPath, Diam,
        OLD_PID, ORAP, MfgRemark, PriceNameId, MDisc, MRate, MAmt, EyeClean, StrLan, LrHalf,
        KeyToSymbol, LabComment, LabShape, OpenTable, OpenCrown, OpenPavallion, OpenGirdle,
        NT_OR_INT, Pav_Ex_Fac, GirdleDesc, IsActivated
    FROM OPENJSON(@JsonData)
    WITH (
        ReportType NVARCHAR(50),
        LotNo NVARCHAR(50),
        LabDate DATETIME2,
        LabId INT,
        Quality NVARCHAR(50),
        LotType NVARCHAR(50),
        CutId INT,
        PolishId INT,
        Sku NVARCHAR(50),
        SymmetryId INT,
        Shade NVARCHAR(50),
        HA NVARCHAR(50),
        UnitPrice DECIMAL(18,2),
        Price DECIMAL(18,2),
        Quantity DECIMAL(18,2),
        CaratSizeId INT,
        CaratSize NVARCHAR(50),
        ClarityId INT,
        Clarity NVARCHAR(50),
        ColorId INT,
        ShapeId INT,
        TableId INT,
        DepthId INT,
        RatioId INT,
        Length DECIMAL(18,2),
        Width DECIMAL(18,2),
        Height DECIMAL(18,2),
        Location NVARCHAR(50),
        Status NVARCHAR(50),
        INWDate DATETIME2,
        MarketDate DATETIME2,
        ReportDate DATETIME2,
        Type NVARCHAR(50),
        CertificateNo NVARCHAR(50),
        Stock NVARCHAR(50),
        CertiType NVARCHAR(50),
        DisplayOrder NVARCHAR(50),
        Flo NVARCHAR(50),
        Dia DECIMAL(18,2),
        TableWhite NVARCHAR(50),
        SideWhite NVARCHAR(50),
        TableBlack NVARCHAR(50),
        SideBlack NVARCHAR(50),
        PavOpen NVARCHAR(50),
        GirdleOpen DECIMAL(18,2),
        CAngle DECIMAL(18,2),
        PAngle DECIMAL(18,2),
        PHt DECIMAL(18,2),
        CHt DECIMAL(18,2),
        Girdle NVARCHAR(50),
        CrownExFac NVARCHAR(50),
        PavExFac NVARCHAR(50),
        TableSpot NVARCHAR(50),
        SideSpot NVARCHAR(50),
        NT_INT NVARCHAR(50),
        Culet NVARCHAR(50),
        FluorId INT,
        Milky NVARCHAR(50),
        Luster NVARCHAR(50),
        Graining NVARCHAR(50),
        DaysType NVARCHAR(50),
        Discount DECIMAL(18,2),
        RatePct DECIMAL(18,2),
        Amount DECIMAL(18,2),
        DiamondImagePath NVARCHAR(255),
        DiamondVideoPath NVARCHAR(255),
        Diam NVARCHAR(50),
        OLD_PID NVARCHAR(50),
        ORAP NVARCHAR(50),
        MfgRemark NVARCHAR(255),
        PriceNameId INT,
        MDisc NVARCHAR(50),
        MRate NVARCHAR(50),
        MAmt NVARCHAR(50),
        EyeClean NVARCHAR(50),
        StrLan NVARCHAR(50),
        LrHalf NVARCHAR(50),
        KeyToSymbol NVARCHAR(255),
        LabComment NVARCHAR(255),
        LabShape NVARCHAR(50),
        OpenTable NVARCHAR(50),
        OpenCrown NVARCHAR(50),
        OpenPavallion NVARCHAR(50),
        OpenGirdle NVARCHAR(50),
        NT_OR_INT NVARCHAR(50),
        Pav_Ex_Fac NVARCHAR(50),
        GirdleDesc NVARCHAR(50),
        IsActivated BIT
    );
END;
    ";

            migrationBuilder.Sql(procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ShapeId",
                table: "Diamonds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ColorId",
                table: "Diamonds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CaratSizeId",
                table: "Diamonds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            string dropProcedure = "DROP PROCEDURE IF EXISTS InsertDiamondsFromJson;";
            migrationBuilder.Sql(dropProcedure);
        }
    }
}
