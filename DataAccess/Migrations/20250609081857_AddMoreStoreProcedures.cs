using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddMoreStoreProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
Create PROCEDURE [dbo].[SP_GetDiamondDataBY_NEW_DiamondFilters]
    @Shapes NVARCHAR(MAX) = NULL,
    @Colors NVARCHAR(MAX) = NULL,
    @FromCarat DECIMAL(18,2) = NULL,
    @ToCarat DECIMAL(18,2) = NULL,
    @FromPrice DECIMAL(18,2) = NULL,
    @ToPrice DECIMAL(18,2) = NULL,
    @Cuts NVARCHAR(MAX) = NULL,
    @Clarities NVARCHAR(MAX) = NULL,
    @FromRatio DECIMAL(18,2) = NULL,
    @ToRatio DECIMAL(18,2) = NULL,
    @FromTable DECIMAL(18,2) = NULL,
    @ToTable DECIMAL(18,2) = NULL,
    @FromDepth DECIMAL(18,2) = NULL,
    @ToDepth DECIMAL(18,2) = NULL,
    @Polish NVARCHAR(MAX) = NULL,
    @Fluor NVARCHAR(MAX) = NULL,
    @Symmeties NVARCHAR(MAX) = NULL,
    @LabNames NVARCHAR(MAX) = NULL
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
            d.[DiamondVideoPath], dpShape.IconPath, d.[IsActivated]
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
            (d.UploadStatus='Active' And d.IsActivated=1
			AND @Shapes IS NULL OR EXISTS (SELECT Id FROM @ShapeList WHERE Id = d.ShapeId))
            AND (EXISTS (SELECT Id FROM @ColorList WHERE Id = d.ColorId))
            AND (EXISTS (SELECT Id FROM @ClarityList WHERE Id = d.ClarityId))
            AND (EXISTS (SELECT Id FROM @PolishList WHERE Id = d.PolishId))
            AND (EXISTS (SELECT Id FROM @CutList WHERE Id = d.CutId))
            AND (EXISTS (SELECT Id FROM @FluorList WHERE Id = d.FluorId))
            AND (EXISTS (SELECT Id FROM @SymList WHERE Id = d.SymmetryId))
            AND (EXISTS (SELECT Name FROM @LabNameList WHERE Name = dpLab.Name))
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


            migrationBuilder.Sql(@"
Create PROCEDURE [dbo].[SP_GetDiamondDataBY_TEST_DiamondFilters]
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
END
");

            migrationBuilder.Sql(@"
Create PROCEDURE [dbo].[SP_GetDiamondDataById]
@Id INT
AS
BEGIN
    SELECT
        d.*, dpType.[Name] AS TypeName, dpCut.[Name] AS CutName, dpClr.[Name] AS ClarityName,
        dpColor.[Name] AS ColorName, dpFlo.[Name] AS FluorName, dpPolish.[Name] AS PolishName,
        dpShape.[Name] AS ShapeName, dpSym.[Name] AS SymmetyName, dpLab.Name AS LabName,
        dpShape.IconPath, usr.FirstName AS UploadedBy,
        FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS UpdatedDate
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
END
");

            migrationBuilder.Sql(@"
Create PROCEDURE [dbo].[SP_PendingDiamonds]
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
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataBY_NEW_DiamondFilters];");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataBY_TEST_DiamondFilters];");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_GetDiamondDataById];");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_PendingDiamonds];");
        }
    }
}
