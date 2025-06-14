using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddStoreProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[GetProductsFiltered]
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
                    END;");
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[InsertDiamondsFromJson]
    @JsonData NVARCHAR(MAX),
    @HistoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Insert parsed JSON data into the Diamonds table
        INSERT INTO Diamonds (
            StoneId, DNA, Step, TypeId, LabId, ShapeId, Carat, ColorId, ClarityId, CutId, 
            PolishId, SymmetryId, FluorId, RAP, Discount, Price, Amount, Measurement, 
            Ratio, Depth, [Table], Shade, LabShape, RapAmount, DiamondVideoPath, 
            [Certificate], IsActivated, CreatedBy, CreatedDate, IsSuccess,
            FileUploadHistoryId, UploadStatus, UpdatedBy, UpdatedDate
        )
        SELECT 
            StoneId, DNA, Step, TypeId, LabId, ShapeId, Carat, ColorId, ClarityId, CutId, 
            PolishId, SymmetryId, FluorId, RAP, Discount, Price, Amount, Measurement, 
            Ratio, Depth, [Table], Shade, LabShape, RapAmount, DiamondVideoPath, 
            [Certificate], IsActivated, CreatedBy, GETDATE(), IsSuccess,
            @HistoryId, UploadStatus, UpdatedBy, UpdatedDate
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
            IsSuccess BIT,
            FileUploadHistoryId INT, 
            UploadStatus NVARCHAR(200), 
            UpdatedBy NVARCHAR(200),
            UpdatedDate DATETIME2
        );

        -- Return inserted records for verification/logging
        SELECT * FROM Diamonds WHERE FileUploadHistoryId = @HistoryId;
    END TRY
    BEGIN CATCH
        -- Capture and raise the error
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;");
            migrationBuilder.Sql(@"Create Procedure [dbo].[SP_Add_DIAMOND_HISTORY]
	@FileUploadHistoryId int
As
Begin

INSERT INTO DiamondHistory (
    [DiamondId],
    [StoneId],
    [DNA],
    [Step],
    [TypeId],
    [Measurement],
    [LabShape],
    [LabId],
    [RAP],
    [RapAmount],
    [Carat],
    [ClarityId],
    [ColorId],
    [CutId],
    [PolishId],
    [SymmetryId],
    [FluorId],
    [Price],
    [Table],
    [Depth],
    [Ratio],
    [Quantity],
    [Shade],
    [Certificate],
    [Discount],
    [RatePct],
    [Amount],
    [DiamondImagePath],
    [DiamondVideoPath],
    [UploadStatus],
    [UpdatedDate],
    [IsSuccess],
    [IsActivated],
	[IsDelete],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy]
)
SELECT
	[Id],
    [StoneId],
    [DNA],
    [Step],
    [TypeId],
    [Measurement],
    [LabShape],
    [LabId],
    [RAP],
    [RapAmount],
    [Carat],
    [ClarityId],
    [ColorId],
    [CutId],
    [PolishId],
    [SymmetryId],
    [FluorId],
    [Price],
    [Table],
    [Depth],
    [Ratio],
    [Quantity],
    [Shade],
    [Certificate],
    [Discount],
    [RatePct],
    [Amount],
    [DiamondImagePath],
    [DiamondVideoPath],
    [UploadStatus],
    [UpdatedDate],
    [IsSuccess],
    [IsActivated],
	[IsDelete],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy]
FROM Diamonds where FileUploadHistoryId=@FileUploadHistoryId

end");
            migrationBuilder.Sql(@" Create PROCEDURE [dbo].[SP_GetDiamondDataBY_DiamondFilters]
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
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[SP_GetDiamondDataBY_NEW_DiamondFilters]
  @Shapes NVARCHAR(MAX) = Null,
  @Colors NVARCHAR(MAX) = Null,
  @FromCarat DECIMAL(18,2) = Null,
  @ToCarat DECIMAL(18,2) = Null,
  @FromPrice DECIMAL(18,2) = Null,
  @ToPrice DECIMAL(18,2) = Null,
  @Cuts NVARCHAR(MAX) = Null,
  @Clarities NVARCHAR(MAX) = Null,
  @FromRatio DECIMAL(18,2) = Null,
  @ToRatio DECIMAL(18,2) = Null,
  @FromTable DECIMAL(18,2) = Null,
  @ToTable DECIMAL(18,2) = Null,
  @FromDepth DECIMAL(18,2) = Null,
  @ToDepth DECIMAL(18,2) = Null,
  @Polish NVARCHAR(MAX) = Null,
  @Fluor NVARCHAR(MAX) = Null,
  @Symmeties NVARCHAR(MAX) = Null,
  @LabNames NVARCHAR(MAX) = Null
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Split filters into table variables
        DECLARE @ShapeList TABLE (Id INT);
        DECLARE @ColorList TABLE (Id INT);
        DECLARE @CutList TABLE (Id INT);
        DECLARE @ClarityList TABLE (Id INT);
        DECLARE @PolishList TABLE (Id INT);
        DECLARE @FluorList TABLE (Id INT);
        DECLARE @LabNameList TABLE ([Name] varchar(250));
        DECLARE @SymList TABLE (Id INT);

        IF @Shapes IS NOT NULL
            INSERT INTO @ShapeList SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@Shapes, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        IF @Colors IS NOT NULL
            INSERT INTO @ColorList SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@Colors, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        IF @Cuts IS NOT NULL
            INSERT INTO @CutList SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@Cuts, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        IF @Clarities IS NOT NULL
            INSERT INTO @ClarityList SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@Clarities, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        IF @Polish IS NOT NULL
            INSERT INTO @PolishList SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@Polish, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        IF @Fluor IS NOT NULL
            INSERT INTO @FluorList SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@Fluor, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        IF @Symmeties IS NOT NULL
            INSERT INTO @SymList SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@Symmeties, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
		IF @LabNames IS NOT NULL
            INSERT INTO @LabNameList SELECT TRY_CAST(value AS varchar(max)) FROM STRING_SPLIT(@LabNames, ',') WHERE TRY_CAST(value AS varchar(max)) IS NOT NULL;

        -- Main SELECT
        SELECT	
     		d.[Id], d.[StoneId], d.[DNA], d.[Step], d.[TypeId], dpType.[Name] AS TypeName,
            d.[Measurement], d.LabShape, d.[LabId], dpLab.Name AS LabName, d.[ShapeId],
            dpShape.Name AS ShapeName, d.[RAP], d.[RapAmount], d.[Carat], d.[ClarityId],
            dpClr.[Name] AS ClarityName, d.[ColorId], dpColor.[Name] AS ColorName,
            d.[CutId], dpCut.[Name] AS CutName, d.[PolishId], dpPolish.Name AS PolishName,
            d.[SymmetryId], dpSym.[Name] AS SymmetyName, d.[FluorId], dpFlo.[Name] AS FluorName,
            d.[Price], d.[Table], d.[Depth], d.[Ratio], d.[Quantity], d.[Shade],
            d.[Certificate], d.[Discount], d.[RatePct], d.[Amount], d.[DiamondImagePath],
            d.[DiamondVideoPath], dpShape.IconPath, d.[IsActivated],FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS DispUpdatedDate,
			d.IsSuccess,d.UpdatedDate,d.UploadStatus,d.UpdatedBy,d.UpdatedBy as UploadedBy
        FROM Diamonds d
        INNER JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id
        INNER JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id
        INNER JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id
        INNER JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id
        LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id
        LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id
        LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id
        LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id
        LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id
        WHERE
            d.UploadStatus='Active' And d.IsActivated=1
			AND (@Shapes IS NULL OR EXISTS (SELECT Id FROM @ShapeList WHERE Id = d.ShapeId))
            AND EXISTS (SELECT Id FROM @ColorList WHERE Id = d.ColorId)
            AND EXISTS (SELECT Id FROM @ClarityList WHERE Id = d.ClarityId)
            AND EXISTS (SELECT Id FROM @PolishList WHERE Id = d.PolishId)
            AND EXISTS (SELECT Id FROM @CutList WHERE Id = d.CutId)
            AND EXISTS (SELECT Id FROM @FluorList WHERE Id = d.FluorId)
            AND EXISTS (SELECT Id FROM @SymList WHERE Id = d.SymmetryId)
            AND EXISTS (SELECT Name FROM @LabNameList WHERE Name = dpLab.Name)
            AND (d.Amount >= @FromPrice)
            AND (d.Amount <= @ToPrice)
            AND (d.Carat >= @FromCarat)
            AND ( d.Carat <= @ToCarat)
            AND (d.Ratio >= @FromRatio)
            AND (d.Ratio <= @ToRatio)
            AND (d.[Table] >= @FromTable)
            AND (d.[Table] <= @ToTable)
            AND (d.Depth >= @FromDepth)
            AND (d.Depth <= @ToDepth)
			AND (d.LabId >= @FromRatio)
            AND (d.Ratio <= @ToRatio)
        ORDER BY d.Id DESC;

    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
");
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[SP_GetDiamondDataBY_TEST_DiamondFilters]
    @Shapes NVARCHAR(MAX) = '460,457',
    @Colors NVARCHAR(MAX) = NULL,
    @FromCarat NVARCHAR(50) = NULL,
    @ToCarat NVARCHAR(50) = NULL,
    @FromPrice DECIMAL(18, 2) = NULL,
    @ToPrice DECIMAL(18, 2) = NULL,
    @Cuts NVARCHAR(MAX) = NULL,
    @Clarities NVARCHAR(MAX) = NULL,
    @FromRatio NVARCHAR(50) = NULL,
    @ToRatio NVARCHAR(50) = NULL,
    @FromTable NVARCHAR(50) = NULL,
    @ToTable NVARCHAR(50) = NULL,
    @FromDepth NVARCHAR(50) = NULL,
    @ToDepth NVARCHAR(50) = NULL,
    @Polish NVARCHAR(MAX) = NULL,
    @Fluor NVARCHAR(MAX) = NULL,
    @Symmeties NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Split Shape input
        DECLARE @ShapeList TABLE (ShapeId INT);
        IF @Shapes IS NOT NULL
        BEGIN
            INSERT INTO @ShapeList (ShapeId)
            SELECT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@Shapes, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Split Color input
        DECLARE @ColorList TABLE (ColorId INT);
        IF @Colors IS NOT NULL
        BEGIN
            INSERT INTO @ColorList (ColorId)
            SELECT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@Colors, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Split Polish input
        DECLARE @PolishList TABLE (PolishId INT);
        IF @Polish IS NOT NULL
        BEGIN
            INSERT INTO @PolishList (PolishId)
            SELECT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@Polish, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Split Clarity input
        DECLARE @ClarityList TABLE (ClarityId INT);
        IF @Clarities IS NOT NULL
        BEGIN
            INSERT INTO @ClarityList (ClarityId)
            SELECT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@Clarities, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Split Cut input
        DECLARE @CutList TABLE (CutId INT);
        IF @Cuts IS NOT NULL
        BEGIN
            INSERT INTO @CutList (CutId)
            SELECT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@Cuts, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Split Fluor input
        DECLARE @FluorList TABLE (FluorId INT);
        IF @Fluor IS NOT NULL
        BEGIN
            INSERT INTO @FluorList (FluorId)
            SELECT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@Fluor, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Split Symmetry input
        DECLARE @SymmetryList TABLE (SymmetryId INT);
        IF @Symmeties IS NOT NULL
        BEGIN
            INSERT INTO @SymmetryList (SymmetryId)
            SELECT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@Symmeties, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Main query with filtering and GROUP BY
        SELECT 
            d.[Id], 
            d.[StoneId],
            d.[DNA],
            d.[Step],
            d.[TypeId],
            dpType.[Name] AS TypeName,
            d.[Measurement],
            d.LabShape,
            d.[LabId],
            dpLab.Name AS LabName,
            d.[ShapeId],
            dpShape.Name AS ShapeName,
            d.[RAP],
            d.[RapAmount],
            d.[Carat],
            d.[ClarityId],
            dpClr.[Name] AS ClarityName,
            d.[ColorId],
            dpColor.[Name] AS ColorName,
            d.[CutId],
            dpCut.[Name] AS CutName,
            d.[PolishId],
            dpPolish.Name AS PolishName,
            d.[SymmetryId],
            dpSym.[Name] AS SymmetyName,  
            d.[FluorId],
            dpFlo.[Name] AS FluorName,
            d.[Price],
            d.[Table],
            d.[Depth],
            d.[Ratio],
            d.[Quantity],
            d.[Shade],
            d.[Certificate],
            d.[Discount],
            d.[RatePct],
            d.[Amount],
            d.[DiamondImagePath],
            d.[DiamondVideoPath],
            dpShape.IconPath,
            d.[IsActivated]
        FROM Diamonds d 
        INNER JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id
        INNER JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id
        INNER JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id
        INNER JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id
        LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id
        LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id
        LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id
        LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id
        LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id
        WHERE
            @Shapes IS NULL OR EXISTS (SELECT * FROM @ShapeList s WHERE s.ShapeId = d.ShapeId)
            AND @Colors IS NULL OR EXISTS (SELECT * FROM @ColorList s WHERE s.ColorId = d.ColorId)
            AND @Clarities IS NULL OR EXISTS (SELECT * FROM @ClarityList s WHERE s.ClarityId = d.Clarity)
            AND @Polish IS NULL OR EXISTS (SELECT * FROM @PolishList s WHERE s.PolishId = d.PolishId)
            AND @Fluor IS NULL OR EXISTS (SELECT * FROM @FluorList s WHERE s.FluorId = d.FluorId)
            AND @Symmeties IS NULL OR EXISTS (SELECT * FROM @SymmetryList s WHERE s.SymmetryId = d.SymmetryId)
            AND (@Cuts IS NULL OR d.CutId IN (SELECT CutId FROM @CutList))
            AND (@FromPrice IS NULL OR d.Amount >= TRY_CAST(@FromPrice AS DECIMAL(18, 2)))
			AND (@ToPrice IS NULL OR d.Amount <= TRY_CAST(@ToPrice AS DECIMAL(18, 2)))
			AND (@FromCarat IS NULL OR d.Carat >= TRY_CAST(@FromCarat AS DECIMAL(18, 2)))
			AND (@ToCarat IS NULL OR d.Carat <= TRY_CAST(@ToCarat AS DECIMAL(18, 2)))
			AND (@FromRatio IS NULL OR d.Ratio >= TRY_CAST(@FromRatio AS DECIMAL(18, 2)))
			AND (@ToRatio IS NULL OR d.Ratio <= TRY_CAST(@ToRatio AS DECIMAL(18, 2)))
			AND (@FromTable IS NULL OR d.[Table] >= TRY_CAST(@FromTable AS DECIMAL(18, 2)))
			AND (@ToTable IS NULL OR d.[Table] <= TRY_CAST(@ToTable AS DECIMAL(18, 2)))
			AND (@FromDepth IS NULL OR d.[Depth] >= TRY_CAST(@FromDepth AS DECIMAL(18, 2)))
			AND (@ToDepth IS NULL OR d.[Depth] <= TRY_CAST(@ToDepth AS DECIMAL(18, 2)))
        GROUP BY 
            d.[Id], d.[StoneId], d.[DNA], d.[Step], d.[TypeId], dpType.[Name],
            d.[Measurement], d.LabShape, d.[LabId], dpLab.Name, d.[ShapeId], 
            dpShape.Name, d.[RAP], d.[RapAmount], d.[Carat], d.[ClarityId], 
            dpClr.[Name], d.[ColorId], dpColor.[Name], d.[CutId], dpCut.[Name], 
            d.[PolishId], dpPolish.Name, d.[SymmetryId], dpSym.[Name], d.[FluorId], 
            dpFlo.[Name], d.[Price], d.[Table], d.[Depth], d.[Ratio], d.[Quantity], 
            d.[Shade], d.[Certificate], d.[Discount], d.[RatePct], d.[Amount], 
            d.[DiamondImagePath], d.[DiamondVideoPath], dpShape.IconPath, d.[IsActivated]
        ORDER BY d.StoneId;
    END TRY

    BEGIN CATCH
        -- Error handling
        SELECT ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END");
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[SP_GetDiamondDataById]  
                                    @Id INT  
                                    AS  
                                    BEGIN  
                                        SELECT  
                                             FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS DispUpdatedDate,d.*, dpType.[Name] AS TypeName, dpCut.[Name] AS CutName, dpClr.[Name] AS ClarityName,  
                                            dpColor.[Name] AS ColorName, dpFlo.[Name] AS FluorName, dpPolish.[Name] AS PolishName,  
                                            dpShape.[Name] AS ShapeName, dpSym.[Name] AS SymmetyName, dpLab.Name AS LabName,  
                                            dpShape.IconPath, usr.FirstName AS UploadedBy  
         
                                        FROM Diamonds d  
                                        INNER JOIN AspNetUsers usr ON d.UpdatedBy = usr.Id  
                                        LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id  
                                        LEFT JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id  
                                        LEFT JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id  
                                        LEFT JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id  
                                        LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id  
                                        LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id  
                                        LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id  
                                        LEFT JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id  
                                        LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id  
                                        WHERE d.Id = @Id  
                                    END");
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[SP_GetDiamondHistoryDataById]
                        @Id varchar(100)
                        AS
                        BEGIN
                            SELECT
                                 FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS DispUpdatedDate,d.*, dpType.[Name] AS TypeName, dpCut.[Name] AS CutName, dpClr.[Name] AS ClarityName,
                                dpColor.[Name] AS ColorName, dpFlo.[Name] AS FluorName, dpPolish.[Name] AS PolishName,
                                dpShape.[Name] AS ShapeName, dpSym.[Name] AS SymmetyName, dpLab.Name AS LabName,
                                dpShape.IconPath, usr.FirstName AS UploadedBy
                            FROM DiamondHistory d
                            INNER JOIN AspNetUsers usr ON d.UpdatedBy = usr.Id
                            LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id
                            LEFT JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id
                            LEFT JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id
                            LEFT JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id
                            LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id
                            LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id
                            LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id
                            LEFT JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id
                            LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id
                            WHERE d.StoneId = @Id
                        END");
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[SP_PendingDiamonds]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT
            d.*, dpType.[Name] AS TypeName, dpLab.Name AS LabName, dpShape.Name AS ShapeName,
            dpClr.[Name] AS ClarityName, dpColor.[Name] AS ColorName, dpCut.[Name] AS CutName,
            dpPolish.Name AS PolishName, dpSym.[Name] AS SymmetyName, dpFlo.[Name] AS FluorName,
            dpShape.IconPath
        FROM Diamonds d
        INNER JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id
        INNER JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id
        INNER JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id
        INNER JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id
        LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id
        LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id
        LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id
        LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id
        LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id
        WHERE d.IsActivated = 0 AND d.UploadStatus = 'Pending'
        ORDER BY d.Id DESC;
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END");
            migrationBuilder.Sql(@" Create PROCEDURE [dbo].[SP_SelectAllDiamonds]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    BEGIN TRY
                        -- Main SELECT
                        SELECT
                            FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS DispUpdatedDate,d.[Id], d.[StoneId], d.[DNA], d.[Step], d.[TypeId], dpType.[Name] AS TypeName,
                            d.[Measurement], d.LabShape, d.[LabId], dpLab.Name AS LabName, d.[ShapeId],
                            dpShape.Name AS ShapeName, d.[RAP], d.[RapAmount], d.[Carat], d.[ClarityId],
                            dpClr.[Name] AS ClarityName, d.[ColorId], dpColor.[Name] AS ColorName,
                            d.[CutId], dpCut.[Name] AS CutName, d.[PolishId], dpPolish.Name AS PolishName,
                            d.[SymmetryId], dpSym.[Name] AS SymmetyName, d.[FluorId], dpFlo.[Name] AS FluorName,
                            d.[Price], d.[Table], d.[Depth], d.[Ratio], d.[Quantity], d.[Shade],
                            d.[Certificate], d.[Discount], d.[RatePct], d.[Amount], d.[DiamondImagePath],
                            d.[DiamondVideoPath], dpShape.IconPath, d.[IsActivated],d.UploadStatus,usr.FirstName as UploadedBy,
			                d.IsSuccess,d.UpdatedBy,d.UpdatedDate
                        FROM Diamonds d
                        INNER JOIN AspNetUsers usr ON d.UpdatedBy = usr.Id
                        INNER JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id
                        INNER JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id
                        INNER JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id
                        INNER JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id
                        LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id
		                LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id
                        LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id
                        LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id
                        LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id
		                where d.IsActivated=1 and d.UploadStatus='Active'
                        ORDER BY d.Id DESC;

                    END TRY
                    BEGIN CATCH
                        SELECT ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END");
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[SP_SelectDiamondsByStatus]  
                @status varchar(100),  
                @isActive bit  
                AS  
                BEGIN  
                    SET NOCOUNT ON;  
                    BEGIN TRY  
                        -- Main SELECT  
                        SELECT   
                            d.[Id], d.[StoneId], d.[DNA], d.[Step], d.[TypeId], dpType.[Name] AS TypeName,  
                            d.[Measurement], d.LabShape, d.[LabId], dpLab.Name AS LabName, d.[ShapeId],  
                            dpShape.Name AS ShapeName, d.[RAP], d.[RapAmount], d.[Carat], d.[ClarityId],  
                            dpClr.[Name] AS ClarityName, d.[ColorId], dpColor.[Name] AS ColorName,  
                            d.[CutId], dpCut.[Name] AS CutName, d.[PolishId], dpPolish.Name AS PolishName,  
                            d.[SymmetryId], dpSym.[Name] AS SymmetyName, d.[FluorId], dpFlo.[Name] AS FluorName,  
                            d.[Price], d.[Table], d.[Depth], d.[Ratio], d.[Quantity], d.[Shade],  
                            d.[Certificate], d.[Discount], d.[RatePct], d.[Amount], d.[DiamondImagePath],  
                            d.[DiamondVideoPath], dpShape.IconPath, d.[IsActivated],d.UploadStatus,usr.FirstName as UploadedBy  
								,FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS DispUpdatedDate,d.IsSuccess,d.UpdatedDate  
                        FROM Diamonds d  
                        LEFT JOIN AspNetUsers usr ON d.UpdatedBy = usr.Id  
                        LEFT JOIN DiamondProperties dpColor ON d.ColorId = dpColor.Id  
                        LEFT JOIN DiamondProperties dpClr ON d.ClarityId = dpClr.Id  
                        LEFT JOIN DiamondProperties dpShape ON d.ShapeId = dpShape.Id  
                        LEFT JOIN DiamondProperties dpCut ON d.CutId = dpCut.Id  
                        LEFT JOIN DiamondProperties dpLab ON d.LabId = dpLab.Id  
                  LEFT JOIN DiamondProperties dpPolish ON d.PolishId = dpPolish.Id  
                        LEFT JOIN DiamondProperties dpSym ON d.SymmetryId = dpSym.Id  
                        LEFT JOIN DiamondProperties dpFlo ON d.FluorId = dpFlo.Id  
                        LEFT JOIN DiamondProperties dpType ON d.TypeId = dpType.Id  
                  where d.IsActivated=@isActive and d.UploadStatus=@status  
                        ORDER BY d.Id DESC;  
  
                    END TRY  
                    BEGIN CATCH  
                        SELECT ERROR_MESSAGE() AS ErrorMessage;  
                    END CATCH  
                END  
");
           

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[GetProductsFiltered]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[InsertDiamondsFromJson]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Add_DIAMOND_HISTORY]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataBY_DiamondFilters]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataBY_NEW_DiamondFilters]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataBY_TEST_DiamondFilters]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataById]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondHistoryDataById]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_PendingDiamonds]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_SelectAllDiamonds]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_SelectDiamondsByStatus]");

        }

    }
}
