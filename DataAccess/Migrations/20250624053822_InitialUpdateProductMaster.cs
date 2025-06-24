using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialUpdateProductMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductHistory_ProductFileUploadHistory_FileUploadHistoryId",
                table: "ProductHistory");

            migrationBuilder.DropIndex(
                name: "IX_ProductHistory_FileUploadHistoryId",
                table: "ProductHistory");

            migrationBuilder.AddColumn<string>(
                name: "CoverPageImage",
                table: "ProductStyles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileHistoryId",
                table: "ProductMaster",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductMasterHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    ColorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProductStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSale = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMasterHistory");

            migrationBuilder.DropColumn(
                name: "CoverPageImage",
                table: "ProductStyles");

            migrationBuilder.DropColumn(
                name: "FileHistoryId",
                table: "ProductMaster");

            migrationBuilder.CreateIndex(
                name: "IX_ProductHistory_FileUploadHistoryId",
                table: "ProductHistory",
                column: "FileUploadHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductHistory_ProductFileUploadHistory_FileUploadHistoryId",
                table: "ProductHistory",
                column: "FileUploadHistoryId",
                principalTable: "ProductFileUploadHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
