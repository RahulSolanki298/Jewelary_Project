using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdateProductMasterHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductMasterHistory",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ProductMasterHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductMaster",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ProductMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"CREATE TYPE ProductDTOType AS TABLE(
	                    [Sku] [nvarchar](100) NULL,
	                    [Title] [nvarchar](255) NULL,
	                    [ColorName] [nvarchar](100) NULL,
	                    [Karat] [nvarchar](50) NULL,
	                    [CenterCaratName] [nvarchar](50) NULL,
	                    [CenterShapeName] [nvarchar](100) NULL,
	                    [AccentStoneShapeName] [nvarchar](100) NULL,
	                    [WholesaleCost] [decimal](18, 2) NULL,
	                    [Price] [decimal](18, 2) NULL,
	                    [UnitPrice] [decimal](18, 2) NULL,
	                    [Length] [nvarchar](255) NULL,
	                    [BandWidth] [nvarchar](255) NULL,
	                    [ProductType] [nvarchar](100) NULL,
	                    [GoldWeight] [nvarchar](255) NULL,
	                    [Grades] [nvarchar](255) NULL,
	                    [MMSize] [nvarchar](50) NULL,
	                    [NoOfStones] [int] NULL,
	                    [DiaWT] [decimal](10, 2) NULL,
	                    [Certificate] [nvarchar](255) NULL,
	                    [IsReadyforShip] [bit] NULL,
	                    [CTW] [decimal](10, 2) NULL,
	                    [VenderName] [nvarchar](255) NULL,
	                    [VenderStyle] [nvarchar](255) NULL,
	                    [Diameter] [nvarchar](50) NULL,
	                    [FileHistoryId] [int] NULL,
	                    [StyleName] [nvarchar](255) NULL,
	                    [CollectionName] [nvarchar](255) NULL,
	                    [Description] [nvarchar](max) NULL,
	                    [Type] [nvarchar](100) NULL
                    )
                    GO");
            migrationBuilder.Sql(@"Create PROCEDURE SaveNewProductList
    @Products dbo.ProductDTOType READONLY,
    @CategoryName NVARCHAR(255),
    @UserId NVARCHAR(100),
    @FileHistoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CategoryId INT = (SELECT TOP 1 Id FROM Category WHERE Name = @CategoryName);
    IF @CategoryId IS NULL
    BEGIN
        RAISERROR('Invalid category name.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Step 1: Generate shape-based ProductKeys
        SELECT DISTINCT
            p.Sku,
            p.CenterShapeName,
            ps.Id AS CenterShapeId,
            NEWID() AS ProductKey
        INTO #ShapeGroup
        FROM @Products p
        INNER JOIN ProductProperty ps ON ps.Name = p.CenterShapeName;

        WITH RankedShapes AS (
            SELECT *, ROW_NUMBER() OVER (PARTITION BY Sku, CenterShapeName ORDER BY Sku) AS rn
            FROM #ShapeGroup
        )
        DELETE FROM RankedShapes WHERE rn > 1;

        -- Step 2: Group Mapping
        SELECT DISTINCT
            CONCAT(p.Sku, '_', p.CenterShapeName, '_', p.ColorName) AS GroupId,
            p.Sku,
            p.CenterShapeName,
            p.ColorName,
            ps.Id AS CenterShapeId,
            pc.Id AS ColorId,
            sg.ProductKey
        INTO #GroupMapping
        FROM @Products p
        JOIN ProductProperty ps ON ps.Name = p.CenterShapeName
        JOIN ProductProperty pc ON pc.Name = p.ColorName
        JOIN #ShapeGroup sg ON sg.Sku = p.Sku AND sg.CenterShapeName = p.CenterShapeName;

        -- Step 3: Insert into ProductMaster
        INSERT INTO ProductMaster (
            ProductKey, ColorId, ColorName, GroupId, ProductStatus,
            CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
            CategoryId, IsActive, Sku, FileHistoryId, IsSale, CenterShapeId
        )
        SELECT 
            gm.ProductKey,
            gm.ColorId,
            gm.ColorName,
            gm.GroupId,
            'Pending',
            @UserId,
            GETDATE(),
            @UserId,
            GETDATE(),
            @CategoryId,
            0,
            gm.Sku,
            @FileHistoryId,
            0,
            gm.CenterShapeId
        FROM #GroupMapping gm
        LEFT JOIN ProductMaster pm ON pm.ProductKey = gm.ProductKey AND pm.Sku = gm.Sku
        WHERE pm.ProductKey IS NULL;

        -- Step 4: Merge into Product
        MERGE Product AS target
        USING (
            SELECT 
                p.Sku,
                pc.Id AS ColorId,
                pk.Id AS KaratId,
                ps.Id AS CenterShapeId,
                pas.Id AS AccentStoneShapeId,
                p.Title,
                p.Description,
                p.WholesaleCost,
                ISNULL(p.Price, 0) AS Price,
                ISNULL(p.UnitPrice, 0) AS UnitPrice,
                p.Type,
                p.ProductType,
                p.NoOfStones,
                CONCAT(p.Sku, '_', p.CenterShapeName, '_', p.ColorName) AS GroupId,
                gm.ProductKey,
                ISNULL(p.FileHistoryId, @FileHistoryId) AS FileHistoryId,
                p.Length,
                p.BandWidth,
                p.GoldWeight,
                p.CTW,
                pcc.Id AS CenterCaratId,
                p.Certificate,
                p.MMSize,
                p.DiaWT,
                p.Grades,
				p.VenderName
            FROM @Products p
            JOIN ProductProperty pc ON pc.Name = p.ColorName
            JOIN ProductProperty pk ON pk.Name = p.Karat
            JOIN ProductProperty ps ON ps.Name = p.CenterShapeName
            JOIN ProductProperty pcc ON pcc.Name = p.CenterCaratName
            JOIN ProductProperty pas ON pas.Name = p.AccentStoneShapeName
            JOIN #GroupMapping gm ON gm.GroupId = CONCAT(p.Sku, '_', p.CenterShapeName, '_', p.ColorName)
        ) AS src
        ON target.Sku = src.Sku AND target.CenterShapeId = src.CenterShapeId AND target.FileHistoryId = src.FileHistoryId

        WHEN MATCHED THEN
            UPDATE SET
                Title = src.Title,
                ColorId = src.ColorId,
                KaratId = src.KaratId,
                AccentStoneShapeId = src.AccentStoneShapeId,
                Description = src.Description,
                WholesaleCost = src.WholesaleCost,
                Price = src.Price,
                UnitPrice = src.UnitPrice,
                UpdatedBy = @UserId,
                UpdatedDate = GETDATE(),
                UploadStatus = 'Pending',
                ProductType = src.ProductType,
                NoOfStones = src.NoOfStones,
                ProductKey = src.ProductKey,
                GroupId = src.GroupId,
                Type = src.Type,
                Length = src.Length,
                BandWidth = src.BandWidth,
                GoldWeight = src.GoldWeight,
                CTW = src.CTW,
                CenterCaratId = src.CenterCaratId,
                Certificate = src.Certificate,
                MMSize = src.MMSize,
                DiaWT = src.DiaWT,
                Grades = src.Grades,
				Vendor=src.VenderName

        WHEN NOT MATCHED THEN
            INSERT (
                Id, Sku, Title, ColorId, KaratId, CenterCaratId, CenterShapeId, AccentStoneShapeId,
                CategoryId, Description, WholesaleCost, Price, UnitPrice, CreatedBy, CreatedDate,
                UpdatedBy, UpdatedDate, FileHistoryId, UploadStatus, IsActivated, IsSuccess,
                GroupId, ProductKey, Type, ProductDate, ProductType, NoOfStones,
                Length, BandWidth, GoldWeight, CTW, Certificate, MMSize, DiaWT, Grades,Vendor
            )
            VALUES (
                NEWID(), src.Sku, src.Title, src.ColorId, src.KaratId, src.CenterCaratId, src.CenterShapeId, src.AccentStoneShapeId,
                @CategoryId, src.Description, src.WholesaleCost, src.Price, src.UnitPrice,
                @UserId, GETDATE(), @UserId, GETDATE(),
                @FileHistoryId, 'Pending', 0, 1,
                src.GroupId, src.ProductKey, src.Type, GETDATE(),
                src.ProductType, src.NoOfStones,
                src.Length, src.BandWidth, src.GoldWeight, src.CTW, src.Certificate, src.MMSize, src.DiaWT, src.Grades,src.VenderName
            );

        -- Step 5: Insert into ProductHistory
        INSERT INTO ProductHistory (
            Id, ProductId, Title, Sku, CategoryId, KaratId, CenterCaratId,
            Length, ColorId, Description, BandWidth, ProductType, GoldWeight,
            Grades, Price, UnitPrice, MMSize, NoOfStones, DiaWT, CenterShapeId,
            Certificate, AccentStoneShapeId, IsReadyforShip, CTW, Vendor,
            VenderStyle, Diameter, UpdatedBy, UpdatedDate, CreatedBy, CreatedDate,
            FileUploadHistoryId, UploadStatus, IsActivated, IsSuccess, GroupId,
            ProductKey, Type, ProductDate
        )
        SELECT
            NEWID(),
            p.Id,
            p.Title,
            p.Sku,
            p.CategoryId,
            p.KaratId,
            p.CenterCaratId,
            p.Length,
            p.ColorId,
            p.Description,
            p.BandWidth,
            p.ProductType,
            p.GoldWeight,
            p.Grades,
            p.Price,
            p.UnitPrice,
            p.MMSize,
            p.NoOfStones,
            p.DiaWT,
            p.CenterShapeId,
            p.Certificate,
            p.AccentStoneShapeId,
            p.IsReadyforShip,
            p.CTW,
            p.Vendor,
            p.VenderStyle,
            p.Diameter,
            @UserId,
            GETDATE(),
            @UserId,
            GETDATE(),
            @FileHistoryId,
            p.UploadStatus,
            p.IsActivated,
            p.IsSuccess,
            p.GroupId,
            p.ProductKey,
            p.Type,
            GETDATE()
        FROM Product p
        WHERE p.FileHistoryId = @FileHistoryId;

        -- Cleanup
        DROP TABLE IF EXISTS #GroupMapping;
        DROP TABLE IF EXISTS #ShapeGroup;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductMasterHistory");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ProductMasterHistory");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductMaster");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ProductMaster");

            migrationBuilder.Sql("DROP TYPE IF EXISTS ProductDTOType");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SaveNewProductList");
        }
    }
}
