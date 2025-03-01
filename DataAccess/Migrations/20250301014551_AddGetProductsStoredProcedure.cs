using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddGetProductsStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE PROCEDURE GetProductsFiltered
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
                cat.ProductType,
                p.StyleId
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
        END;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetProductsFiltered;");

        }
    }
}
