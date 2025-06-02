using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class intialupdateProgramItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistoryId",
                table: "DiamondHistory");

            migrationBuilder.RenameColumn(
                name: "HistoryId",
                table: "Diamonds",
                newName: "LiveOnId");

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "ProductHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileUploadHistoryId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiamondId",
                table: "DiamondHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryImage",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SEO_Title",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SEO_Url",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductStyleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsHomePage = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStyleItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationPlatforms");

            migrationBuilder.DropTable(
                name: "ProductStyleItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "FileUploadHistoryId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "DiamondId",
                table: "DiamondHistory");

            migrationBuilder.DropColumn(
                name: "CategoryImage",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "SEO_Title",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "SEO_Url",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "LiveOnId",
                table: "Diamonds",
                newName: "HistoryId");

            migrationBuilder.AddColumn<int>(
                name: "HistoryId",
                table: "DiamondHistory",
                type: "int",
                nullable: true);
        }
    }
}
