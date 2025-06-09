using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddMoreStoreProceduresLast2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                Create PROCEDURE [dbo].[SP_SelectAllDiamonds]
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
                            d.[DiamondVideoPath], dpShape.IconPath, d.[IsActivated],d.UploadStatus,usr.FirstName as UploadedBy,
			                FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS UpdatedDate,d.IsSuccess
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
                END
            ");

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
			                ,FORMAT(d.UpdatedDate, 'dd-MM-yyyy hh:mm:ss tt') AS UpdatedDate,d.IsSuccess	
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
		                where d.IsActivated=@isActive and d.UploadStatus=@status
                        ORDER BY d.Id DESC;

                    END TRY
                    BEGIN CATCH
                        SELECT ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_SelectAllDiamonds];");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_SelectDiamondsByStatus];");
        }
    }
}
