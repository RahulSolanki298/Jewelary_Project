using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddStoreProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                  @"Create PROCEDURE [dbo].[GetProductsFiltered]
                        @CategoryId INT = NULL,
                        @SubCategoryId INT = NULL,
                        @CaratId INT = NULL,
                        @ColorId INT = NULL,
                        @ClarityId INT = NULL,
                        @ShapeId INT = NULL,
                        @IsActivated BIT = NULL
                    AS
                    BEGIN
                        SET NOCOUNT ON;

                        SELECT 
                            p.Id,
                            p.Title,
                            c.Id AS CaratId,
                            c.Name AS CaratName,
                            cat.Id AS CategoryId,
                            cat.Name AS CategoryName,
                            col.Id AS ColorId,
                            col.Name AS ColorName,
                            subcat.Id AS SubCategoryId,
                            subcat.Name AS SubCategoryName,
                            cl.Id AS ClarityId,
                            cl.Name AS ClarityName,
                            s.Id AS ShapeId,
                            s.Name AS ShapeName,
                            p.UnitPrice,
                            p.Price,
                            p.IsActivated,
                            p.CaratSizeId,
                            c.Name AS CaratSizeName,
                            p.Description,
                            p.Sku,
                            cat.ProductType
                        FROM Product p
                        JOIN Category cat ON p.CategoryId = cat.Id
                        JOIN SubCategory subcat ON p.SubCategoryId = subcat.Id
                        JOIN ProductProperty col ON p.ColorId = col.Id
                        JOIN ProductProperty c ON p.CaratId = c.Id
                        JOIN ProductProperty s ON p.ShapeId = s.Id
                        JOIN ProductProperty cl ON p.ClarityId = cl.Id
                        WHERE 
                            (@CategoryId IS NULL OR p.CategoryId = @CategoryId) AND
                            (@SubCategoryId IS NULL OR p.SubCategoryId = @SubCategoryId) AND
                            (@CaratId IS NULL OR p.CaratId = @CaratId) AND
                            (@ColorId IS NULL OR p.ColorId = @ColorId) AND
                            (@ClarityId IS NULL OR p.ClarityId = @ClarityId) AND
                            (@ShapeId IS NULL OR p.ShapeId = @ShapeId) AND
                            (@IsActivated IS NULL OR p.IsActivated = @IsActivated)
                        ORDER BY p.Title ASC;
                    END;"
              );
            migrationBuilder.Sql(@"
        CREATE PROCEDURE [dbo].[InsertDiamondsFromJson]
            @JsonData NVARCHAR(MAX),
            @HistoryId INT
        AS
        BEGIN
            SET NOCOUNT ON;

            BEGIN TRY
                INSERT INTO Diamonds (
                    StoneId, DNA, Step, TypeId, LabId, ShapeId, Carat, ColorId, ClarityId, CutId, 
                    PolishId, SymmetryId, FluorId, RAP, Discount, Price, Amount, Measurement, 
                    Ratio, Depth, [Table], Shade, LabShape, RapAmount, DiamondVideoPath, 
                    [Certificate], IsActivated, CreatedBy, CreatedDate, IsSuccess,
                    FileUploadHistoryId, UploadStatus, UpdatedBy, UpdatedDate)
                SELECT 
                    StoneId, DNA, Step, TypeId, LabId, ShapeId, Carat, ColorId, ClarityId, CutId, 
                    PolishId, SymmetryId, FluorId, RAP, Discount, Price, Amount, Measurement, 
                    Ratio, Depth, [Table], Shade, LabShape, RapAmount, DiamondVideoPath, 
                    [Certificate], IsActivated, CreatedBy, GETDATE(), IsSuccess,
                    @HistoryId, 'Pending', CreatedBy, GETDATE()
                FROM OPENJSON(@JsonData)
                WITH (
                    StoneId NVARCHAR(50),
                    DNA NVARCHAR(1000),
                    Step NVARCHAR(100),
                    TypeId INT,
                    LabId INT,
                    ShapeId INT,
                    Carat DECIMAL(10,2),
                    ColorId INT,
                    ClarityId INT,
                    CutId INT,
                    PolishId INT,
                    SymmetryId INT,
                    FluorId INT,
                    RAP DECIMAL(20,2),
                    Discount DECIMAL(20,2),
                    Price DECIMAL(20,2),
                    Amount DECIMAL(20,2),
                    Measurement NVARCHAR(200),
                    Ratio DECIMAL(10,2),
                    Depth DECIMAL(5,2),
                    [Table] DECIMAL(5,2),
                    Shade NVARCHAR(200),
                    LabShape NVARCHAR(200),
                    RapAmount DECIMAL(20,2),
                    DiamondVideoPath NVARCHAR(MAX),
                    [Certificate] NVARCHAR(MAX),
                    IsActivated BIT,
                    CreatedBy NVARCHAR(MAX),
                    CreatedDate DATETIME2,
                    IsSuccess BIT
                );

                SELECT * FROM Diamonds WHERE FileUploadHistoryId = @HistoryId;
            END TRY
            BEGIN CATCH
                DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
                SELECT 
                    @ErrorMessage = ERROR_MESSAGE(),
                    @ErrorSeverity = ERROR_SEVERITY(),
                    @ErrorState = ERROR_STATE();
                RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
            END CATCH
        END
    ");

            migrationBuilder.Sql(@"
              Create PROCEDURE [dbo].[SP_GetDiamondDataBY_DiamondFilters]
              AS
              BEGIN
                    SELECT
			            d.[Id]
			            ,d.DNA
			            ,d.Step
			            ,d.StoneId
			            ,d.TypeId
			            ,d.Measurement
			            ,d.RAP
			            ,d.RapAmount
                        ,dpType.[Name] as TypeName
                        ,d.[LotNo]
                        ,d.[LabDate]
                        ,d.[Quality]
                        ,d.[LotType]
                        ,d.[CutId]
                        ,dpCut.[Name] AS CutName
			            ,d.Carat As CaratName
                        ,d.[Sku]
                        ,d.[Shade]
                        ,d.[HA]
                        ,d.[UnitPrice]
			            ,d.Diam
                        ,d.[Quantity]
                        ,d.[ORAP]
                        ,d.[Clarity]
                        ,dpClr.[Name] AS ClarityName
                        ,d.[PriceNameId]
			            ,d.EyeClean
                        ,d.Ratio As RatioName
                        ,d.[Length]
                        ,d.[Width]
                        ,d.[Height]
                        ,d.[Location]
                        ,d.[Status]
                        ,d.[INWDate]
                        ,d.[MarketDate]
                        ,d.[ReportDate]
                        ,d.[Type]
                        ,d.[CertificateNo]
                        ,d.[Stock]
                        ,d.[CertiType]
                        ,d.[DisplayOrder]
                        ,d.[Dia]
                        ,d.[TableWhite]
                        ,d.[SideWhite]
                        ,d.[TableBlack]
                        ,d.[SideBlack]
                        ,d.[PavOpen]
                        ,d.[GirdleOpen]
                        ,d.[CAngle]
                        ,d.[PAngle]
                        ,d.[PHt]
                        ,d.[CHt]
                        ,d.[Girdle]
                        ,d.[CrownExFac]
                        ,d.[PavExFac]
                        ,d.[TableSpot]
                        ,d.[SideSpot]
                        ,d.[NT_INT]
                        ,d.[Culet]
                        ,d.[Milky]
                        ,d.[Luster]
                        ,d.[Graining]
                        ,d.[DaysType]
                        ,d.[Discount]
                        ,d.[RatePct]
                        ,d.[Amount]
                        ,d.[ColorId]
                        ,dpColor.[Name] AS ColorName
                        ,d.[FluorId]
                        ,dpFlo.[Name] AS FluorName
                        ,d.[PolishId]
                        ,dpPolish.[Name] AS PolishName
                        ,d.[ShapeId]
			            ,dpShape.Name As ShapeName
                        ,d.[SymmetryId]
                        ,dpSym.[Name] AS SymmetryName
                        ,d.[Table]
                        ,d.[ClarityId]
			            ,d.Depth
                        ,d.[DiamondImagePath]
                        ,d.[DiamondVideoPath]
                        ,d.[Flo]
                        ,d.[MfgRemark]
                        ,d.[OLD_PID]
			            ,d.GirdleDesc
			            ,d.IsActivated
			            ,d.KeyToSymbol
			            ,d.LabComment
			            ,d.LabShape
			            ,d.OpenTable
			            ,d.OpenCrown
			            ,d.OpenPavallion
			            ,d.OpenGirdle
			            ,d.NT_OR_INT
			            ,d.Pav_Ex_Fac
			            ,d.LabId
			            ,dpLab.Name as LabName
			            ,d.LrHalf
			            ,d.MRate
			            ,d.MAmt
			            ,d.MDisc
			            ,d.Price
			            ,d.StrLan
			            ,d.[Certificate]
                    FROM Diamonds d
                    LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id
                    LEFT JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id
                    LEFT JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id
                    LEFT JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id
                    LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id
                    LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id
                    LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id
                    LEFT JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id
                    LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id
                END
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[GetProductsFiltered]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[InsertDiamondsFromJson]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataBY_DiamondFilters]");
        }
    }
}
