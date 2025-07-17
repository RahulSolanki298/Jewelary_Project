using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyStoreProcedureWithData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER PROCEDURE [dbo].[SP_GetDiamondDataBY_NEW_DiamondFilters]
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
        DECLARE @ShapeList TABLE (Id INT);
        DECLARE @ColorList TABLE (Id INT);
        DECLARE @CutList TABLE (Id INT);
        DECLARE @ClarityList TABLE (Id INT);
        DECLARE @PolishList TABLE (Id INT);
        DECLARE @FluorList TABLE (Id INT);
        DECLARE @SymList TABLE (Id INT);
        DECLARE @LabNameList TABLE ([Name] VARCHAR(250));

        -- Clean JSON-style string inputs
        SET @Shapes = REPLACE(REPLACE(REPLACE(@Shapes, '[', ''), ']', ''), '""', '');
        SET @Colors = REPLACE(REPLACE(REPLACE(@Colors, '[', ''), ']', ''), '""', '');
        SET @Cuts = REPLACE(REPLACE(REPLACE(@Cuts, '[', ''), ']', ''), '""', '');
        SET @Clarities = REPLACE(REPLACE(REPLACE(@Clarities, '[', ''), ']', ''), '""', '');
        SET @Polish = REPLACE(REPLACE(REPLACE(@Polish, '[', ''), ']', ''), '""', '');
        SET @Fluor = REPLACE(REPLACE(REPLACE(@Fluor, '[', ''), ']', ''), '""', '');
        SET @Symmeties = REPLACE(REPLACE(REPLACE(@Symmeties, '[', ''), ']', ''), '""', '');
        SET @LabNames = REPLACE(REPLACE(REPLACE(@LabNames, '[', ''), ']', ''), '""', '');

        -- Fill table variables
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
            INSERT INTO @LabNameList SELECT TRY_CAST(value AS VARCHAR(MAX)) FROM STRING_SPLIT(@LabNames, ',') WHERE TRY_CAST(value AS VARCHAR(MAX)) IS NOT NULL;

        -- Main SELECT query
        SELECT
            d.[Id], d.[StoneId], d.[DNA], d.[Step], d.[TypeId], dpType.[Name] AS TypeName,
            d.[Measurement], d.LabShape, d.[LabId], dpLab.Name AS LabName, d.[ShapeId], dpShape.Name AS ShapeName,
            d.[RAP], d.[RapAmount], d.[Carat], d.[ClarityId], dpClr.[Name] AS ClarityName,
            d.[ColorId], dpColor.[Name] AS ColorName, d.[CutId], dpCut.[Name] AS CutName,
            d.[PolishId], dpPolish.Name AS PolishName, d.[SymmetryId], dpSym.[Name] AS SymmetyName,
            d.[FluorId], dpFlo.[Name] AS FluorName, d.[Price], d.[Table], d.[Depth],
            d.[Ratio], d.[Quantity], d.[Shade], d.[Certificate], d.[Discount], d.[RatePct],
            d.[Amount], d.[DiamondImagePath], d.[DiamondVideoPath], dpShape.IconPath,
            d.[IsActivated], FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS DispUpdatedDate,
            d.IsSuccess, d.UpdatedDate, d.UploadStatus, d.UpdatedBy, d.UpdatedBy AS UploadedBy
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
            d.UploadStatus = 'Active' AND d.IsActivated = 1
            AND (@Shapes IS NULL OR EXISTS (SELECT Id FROM @ShapeList WHERE Id = d.ShapeId))
            AND EXISTS (SELECT Id FROM @ColorList WHERE Id = d.ColorId)
            AND EXISTS (SELECT Id FROM @ClarityList WHERE Id = d.ClarityId)
            AND EXISTS (SELECT Id FROM @PolishList WHERE Id = d.PolishId)
            AND EXISTS (SELECT Id FROM @CutList WHERE Id = d.CutId)
            AND EXISTS (SELECT Id FROM @FluorList WHERE Id = d.FluorId)
            AND EXISTS (SELECT Id FROM @SymList WHERE Id = d.SymmetryId)
            AND EXISTS (SELECT Name FROM @LabNameList WHERE Name = dpLab.Name)
            AND (d.Price >= @FromPrice)
            AND (d.Price <= @ToPrice)
            AND (d.Carat >= @FromCarat)
            AND (d.Carat <= @ToCarat)
            AND (d.Ratio >= @FromRatio)
            AND (d.Ratio <= @ToRatio)
            AND (d.[Table] >= @FromTable)
            AND (d.[Table] <= @ToTable)
            AND (d.Depth >= @FromDepth)
            AND (d.Depth <= @ToDepth)
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
            migrationBuilder.Sql(@"ALTER PROCEDURE [dbo].[SP_GetDiamondDataBY_NEW_DiamondFilters]
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
		
        ORDER BY d.Id DESC;

    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
");
        }
    }
}
