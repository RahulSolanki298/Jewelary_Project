using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class MergeB2C_And_B2B : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "BuyerOrderStatus");

            migrationBuilder.RenameColumn(
                name: "BuyerCode",
                table: "Orders",
                newName: "OrderCode");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "OrderItems",
                newName: "BusinessCode");

            migrationBuilder.AddColumn<string>(
                name: "BusinessCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessCode",
                table: "BuyerOrderStatus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomer",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BusinessCode",
                table: "BuyerOrderStatus");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsCustomer",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OrderCode",
                table: "Orders",
                newName: "BuyerCode");

            migrationBuilder.RenameColumn(
                name: "BusinessCode",
                table: "OrderItems",
                newName: "BuyerId");

            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                table: "BuyerOrderStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
