using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialModifyStoreProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE dbo.SaveNewProductList
(
    @Products dbo.ProductDTOType READONLY,
    @CategoryName NVARCHAR(255),
    @UserId NVARCHAR(100),
    @FileHistoryId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CategoryId INT = (SELECT TOP 1 Id FROM Category WHERE Name = @CategoryName);
    IF @CategoryId IS NULL
    BEGIN
        RAISERROR('Invalid category name: %s', 16, 1, @CategoryName);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1️⃣ Ensure ProductProperty entries exist
        DECLARE @props TABLE (Name NVARCHAR(100));
        INSERT INTO @props (Name)
        SELECT DISTINCT Name FROM (
            SELECT ColorName FROM @Products
            UNION SELECT Karat FROM @Products
            UNION SELECT CenterShapeName FROM @Products
            UNION SELECT AccentStoneShapeName FROM @Products
            UNION SELECT CenterCaratName FROM @Products
        ) AS Props(Name);

        INSERT INTO ProductProperty(Name, DisplayOrder)
        SELECT Name, 0 FROM @props p
        WHERE NOT EXISTS (
            SELECT 1 FROM ProductProperty pp WHERE pp.Name = p.Name
        );

        -- 2️⃣ Generate deterministic ProductKey & GroupId
        SELECT DISTINCT
            p.Sku,
            p.CenterShapeName,
            ps.Id AS CenterShapeId,
            CAST(HASHBYTES('SHA1', CONCAT(p.Sku, '_', p.CenterShapeName)) AS UNIQUEIDENTIFIER) AS ProductKey
        INTO #ShapeGroup
        FROM @Products p
        JOIN ProductProperty ps ON ps.Name = p.CenterShapeName;

        -- Remove duplicates
        ;WITH Ranked AS (
            SELECT *, ROW_NUMBER() OVER (PARTITION BY Sku, CenterShapeName ORDER BY Sku) AS rn
            FROM #ShapeGroup
        )
        DELETE FROM Ranked WHERE rn > 1;

        -- 3️⃣ Build GroupMapping
        SELECT DISTINCT
            CONCAT(p.Sku, '_', p.CenterShapeName, '_', p.ColorName) AS GroupId,
            p.Sku,
            p.CenterShapeName,
            p.ColorName,
            ps.Id AS CenterShapeId,
            pc.Id AS ColorId,
            sg.ProductKey,
            p.Title,
            p.Price
        INTO #GroupMapping
        FROM @Products p
        JOIN ProductProperty ps ON ps.Name = p.CenterShapeName
        JOIN ProductProperty pc ON pc.Name = p.ColorName
        JOIN #ShapeGroup sg ON sg.Sku = p.Sku AND sg.CenterShapeName = p.CenterShapeName;

        -- 4️⃣ Insert into ProductMaster if new
        INSERT INTO ProductMaster (
            ProductKey, ColorId, ColorName, GroupId, ProductStatus,
            CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
            CategoryId, IsActive, Sku, FileHistoryId, IsSale,
            CenterShapeId, Title, Price
        )
        SELECT
            gm.ProductKey,
            gm.ColorId,
            gm.ColorName,
            gm.GroupId,
            'Pending',
            @UserId, GETDATE(), @UserId, GETDATE(),
            @CategoryId, 0,
            gm.Sku,
            @FileHistoryId,
            0,
            gm.CenterShapeId,
            gm.Title,
            gm.Price
        FROM #GroupMapping gm
        LEFT JOIN ProductMaster pm ON pm.ProductKey = gm.ProductKey AND pm.Sku = gm.Sku
        WHERE pm.ProductKey IS NULL;

        -- 5️⃣ Prepare source data for merge
        SELECT 
            p.Sku,
            pc.Id AS ColorId,
            pk.Id AS KaratId,
            ps.Id AS CenterShapeId,
            pas.Id AS AccentStoneShapeId,
            p.Title, p.Description, p.WholesaleCost,
            COALESCE(p.Price, 0) AS Price,
            COALESCE(p.UnitPrice, 0) AS UnitPrice,
            p.Type, p.ProductType, p.NoOfStones,
            gm.GroupId, gm.ProductKey,
            COALESCE(p.FileHistoryId, @FileHistoryId) AS FileHistoryId,
            p.Length, p.BandWidth, p.GoldWeight, p.CTW,
            pcc.Id AS CenterCaratId,
            p.Certificate, p.MMSize, p.DiaWT, p.Grades,
            p.VenderName AS Vendor,
            p.VenderStyle,
            p.Diameter
        INTO #SrcProducts
        FROM @Products p
        JOIN ProductProperty pc ON pc.Name = p.ColorName
        JOIN ProductProperty pk ON pk.Name = p.Karat
        JOIN ProductProperty ps ON ps.Name = p.CenterShapeName
        JOIN ProductProperty pas ON pas.Name = p.AccentStoneShapeName
        JOIN ProductProperty pcc ON pcc.Name = p.CenterCaratName
        JOIN #GroupMapping gm ON gm.GroupId = CONCAT(p.Sku, '_', p.CenterShapeName, '_', p.ColorName);

        -- 6️⃣ Update existing Product records
		DECLARE @UpdatedProducts TABLE (ProductId UNIQUEIDENTIFIER);

        UPDATE tgt
        SET
            tgt.Title = src.Title,
            tgt.ColorId = src.ColorId,
            tgt.KaratId = src.KaratId,
            tgt.AccentStoneShapeId = src.AccentStoneShapeId,
            tgt.Description = src.Description,
            tgt.WholesaleCost = src.WholesaleCost,
            tgt.Price = src.Price,
            tgt.UnitPrice = src.UnitPrice,
            tgt.UpdatedBy = @UserId,
            tgt.UpdatedDate = GETDATE(),
            tgt.UploadStatus = 'Pending',
            tgt.ProductType = src.ProductType,
            tgt.NoOfStones = src.NoOfStones,
            tgt.ProductKey = src.ProductKey,
            tgt.GroupId = src.GroupId,
            tgt.Type = src.Type,
            tgt.Length = src.Length,
            tgt.BandWidth = src.BandWidth,
            tgt.GoldWeight = src.GoldWeight,
            tgt.CTW = src.CTW,
            tgt.CenterCaratId = src.CenterCaratId,
            tgt.Certificate = src.Certificate,
            tgt.MMSize = src.MMSize,
            tgt.DiaWT = src.DiaWT,
            tgt.Grades = src.Grades,
            tgt.Vendor = src.Vendor,
            tgt.VenderStyle = src.VenderStyle,
            tgt.Diameter = src.Diameter
			OUTPUT INSERTED.Id INTO @UpdatedProducts(ProductId)
        FROM Product tgt
        JOIN #SrcProducts src
            ON tgt.Sku = src.Sku AND tgt.CenterShapeId = src.CenterShapeId AND tgt.FileHistoryId = src.FileHistoryId;

        -- 7️⃣ Insert new Product records
		DECLARE @InsertedProducts TABLE (ProductId UNIQUEIDENTIFIER);

        INSERT INTO Product (
            Id, Sku, Title, ColorId, KaratId, CenterCaratId,
            CenterShapeId, AccentStoneShapeId, CategoryId, Description,
            WholesaleCost, Price, UnitPrice, CreatedBy, CreatedDate,
            UpdatedBy, UpdatedDate, FileHistoryId, UploadStatus,
            IsActivated, IsSuccess, GroupId, ProductKey, Type,
            ProductDate, ProductType, NoOfStones, Length, BandWidth,
            GoldWeight, CTW, Certificate, MMSize, DiaWT, Grades,
            Vendor, VenderStyle, Diameter
        )
		OUTPUT INSERTED.Id INTO @InsertedProducts(ProductId)
        SELECT
            NEWID(), src.Sku, src.Title, src.ColorId, src.KaratId, src.CenterCaratId,
            src.CenterShapeId, src.AccentStoneShapeId, @CategoryId, src.Description,
            src.WholesaleCost, src.Price, src.UnitPrice, @UserId, GETDATE(),
            @UserId, GETDATE(), src.FileHistoryId, 'Pending',
            0, 1, src.GroupId, src.ProductKey, src.Type,
            GETDATE(), src.ProductType, src.NoOfStones, src.Length, src.BandWidth,
            src.GoldWeight, src.CTW, src.Certificate, src.MMSize,
            src.DiaWT, src.Grades, src.Vendor, src.VenderStyle, src.Diameter
        FROM #SrcProducts src
        WHERE NOT EXISTS (
            SELECT 1 FROM Product p
            WHERE p.Sku = src.Sku AND p.CenterShapeId = src.CenterShapeId AND p.FileHistoryId = src.FileHistoryId
        );

        -- 8️⃣ Insert into ProductHistory
        INSERT INTO ProductHistory (
			Id, ProductId, Title, Sku, CategoryId, KaratId, CenterCaratId,
			Length, ColorId, Description, BandWidth, ProductType,
			GoldWeight, Grades, Price, UnitPrice, MMSize,
			NoOfStones, DiaWT, CenterShapeId, Certificate,
			AccentStoneShapeId, IsReadyforShip, CTW, Vendor,
			VenderStyle, Diameter, UpdatedBy, UpdatedDate,
			CreatedBy, CreatedDate, FileUploadHistoryId,
			UploadStatus, IsActivated, IsSuccess, GroupId,
			ProductKey, Type, ProductDate
		)
		SELECT
			NEWID(), p.Id, p.Title, p.Sku, p.CategoryId, p.KaratId, p.CenterCaratId,
			p.Length, p.ColorId, p.Description, p.BandWidth, p.ProductType,
			p.GoldWeight, p.Grades, p.Price, p.UnitPrice, p.MMSize,
			p.NoOfStones, p.DiaWT, p.CenterShapeId, p.Certificate,
			p.AccentStoneShapeId, p.IsReadyforShip, p.CTW, p.Vendor,
			p.VenderStyle, p.Diameter, @UserId, GETDATE(),
			@UserId, GETDATE(), @FileHistoryId,
			p.UploadStatus, p.IsActivated, p.IsSuccess,
			p.GroupId, p.ProductKey, p.Type, GETDATE()
		FROM Product p
		WHERE p.Id IN (
			SELECT ProductId FROM @InsertedProducts
			UNION
			SELECT ProductId FROM @UpdatedProducts
		);


        -- ✅ Clean up
        DROP TABLE IF EXISTS #SrcProducts, #ShapeGroup, #GroupMapping;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER PROCEDURE SaveNewProductList
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
            NEWID() AS ProductKey,
			p.Title,
			p.Price
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
            sg.ProductKey,
			p.Title,
			p.Price
        INTO #GroupMapping
        FROM @Products p
        JOIN ProductProperty ps ON ps.Name = p.CenterShapeName
        JOIN ProductProperty pc ON pc.Name = p.ColorName
        JOIN #ShapeGroup sg ON sg.Sku = p.Sku AND sg.CenterShapeName = p.CenterShapeName;

        -- Step 3: Insert into ProductMaster
        INSERT INTO ProductMaster (
    ProductKey, ColorId, ColorName, GroupId, ProductStatus,
    CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
    CategoryId, IsActive, Sku, FileHistoryId, IsSale, CenterShapeId,
    Title,
    Price
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
    gm.CenterShapeId,
    pm.Title,
    pm.Price
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
    }
}
