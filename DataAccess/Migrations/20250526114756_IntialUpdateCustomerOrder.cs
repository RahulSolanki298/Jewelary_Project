using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class IntialUpdateCustomerOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoupanId",
                table: "CustomerOrderItems");

            migrationBuilder.DropColumn(
                name: "Dicount",
                table: "CustomerOrderItems");

            migrationBuilder.DropColumn(
                name: "DicountAmount",
                table: "CustomerOrderItems");

            migrationBuilder.RenameColumn(
                name: "CoupanCode",
                table: "CustomerOrderItems",
                newName: "UpdatedBy");

            migrationBuilder.AddColumn<string>(
                name: "IsActived",
                table: "CustomerOrderStatus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoupanCode",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CoupanId",
                table: "CustomerOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Dicount",
                table: "CustomerOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DicountAmount",
                table: "CustomerOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Prices",
                table: "CustomerOrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActived",
                table: "CustomerOrderStatus");

            migrationBuilder.DropColumn(
                name: "CoupanCode",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "CoupanId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "Dicount",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "DicountAmount",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "Prices",
                table: "CustomerOrderItems");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "CustomerOrderItems",
                newName: "CoupanCode");

            migrationBuilder.AddColumn<int>(
                name: "CoupanId",
                table: "CustomerOrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Dicount",
                table: "CustomerOrderItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DicountAmount",
                table: "CustomerOrderItems",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
