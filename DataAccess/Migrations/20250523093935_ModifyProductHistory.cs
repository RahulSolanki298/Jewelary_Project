using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyProductHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductHistory_ProductFileUploadHistory_ProductFileUploadHistoryId",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "IsFileUploadHistoryId",
                table: "ProductHistory");

            migrationBuilder.RenameColumn(
                name: "ProductFileUploadHistoryId",
                table: "ProductHistory",
                newName: "FileUploadHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductHistory_ProductFileUploadHistoryId",
                table: "ProductHistory",
                newName: "IX_ProductHistory_FileUploadHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductHistory_ProductFileUploadHistory_FileUploadHistoryId",
                table: "ProductHistory",
                column: "FileUploadHistoryId",
                principalTable: "ProductFileUploadHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductHistory_ProductFileUploadHistory_FileUploadHistoryId",
                table: "ProductHistory");

            migrationBuilder.RenameColumn(
                name: "FileUploadHistoryId",
                table: "ProductHistory",
                newName: "ProductFileUploadHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductHistory_FileUploadHistoryId",
                table: "ProductHistory",
                newName: "IX_ProductHistory_ProductFileUploadHistoryId");

            migrationBuilder.AddColumn<int>(
                name: "IsFileUploadHistoryId",
                table: "ProductHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductHistory_ProductFileUploadHistory_ProductFileUploadHistoryId",
                table: "ProductHistory",
                column: "ProductFileUploadHistoryId",
                principalTable: "ProductFileUploadHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
