using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialCreateCollections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductProperty");

            migrationBuilder.CreateTable(
                name: "DiamondColorData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SymbolName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DiamondShapeData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiamondColorData");

            migrationBuilder.DropTable(
                name: "DiamondShapeData");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductProperty",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
