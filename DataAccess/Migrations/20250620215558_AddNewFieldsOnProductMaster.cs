using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddNewFieldsOnProductMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProductMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductMaster",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProductMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ProductMaster",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductMaster");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProductMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ProductMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProductMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ProductMaster");
        }
    }
}
