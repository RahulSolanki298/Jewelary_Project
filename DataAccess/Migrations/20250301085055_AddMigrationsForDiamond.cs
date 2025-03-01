using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddMigrationsForDiamond : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diamonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Polish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Carat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Clarity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shape = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Table = table.Column<int>(type: "int", nullable: true),
                    Depth = table.Column<int>(type: "int", nullable: true),
                    Ratio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    INWDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MarketDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertiType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TableWhite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SideWhite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableBlack = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SideBlack = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PavOpen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GirdleOpen = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CAngle = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PAngle = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PHt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CHt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Girdle = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CrownExFac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PavExFac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableSpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SideSpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NT_INT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Culet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Milky = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Luster = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Graining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RatePct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diamonds", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diamonds");
        }
    }
}
