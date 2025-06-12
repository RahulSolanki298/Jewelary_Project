using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ManageCollectionInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProductCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ProductCollections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProductCollectionItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductCollectionItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProductCollectionItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ProductCollectionItems",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProductCollections");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ProductCollections");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProductCollectionItems");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ProductCollectionItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProductCollectionItems");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ProductCollectionItems");
        }
    }
}
