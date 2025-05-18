using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialCreateHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderNo",
                table: "ProductCollectionItems",
                newName: "Index");

            migrationBuilder.AddColumn<bool>(
                name: "IsHistoryId",
                table: "Product",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductFileUploadHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoOfSuccess = table.Column<int>(type: "int", nullable: false),
                    NoOfFailed = table.Column<int>(type: "int", nullable: false),
                    IsSuccess = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFileUploadHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFileUploadHistory");

            migrationBuilder.DropColumn(
                name: "IsHistoryId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "Index",
                table: "ProductCollectionItems",
                newName: "OrderNo");
        }
    }
}
