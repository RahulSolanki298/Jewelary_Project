using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ModifyDiamondTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepthId",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "CAngle",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CHt",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CaratSizeId",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CaratSizeName",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CertiType",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CertificateNo",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CrownExFac",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Culet",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "DaysType",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "DepthId",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "DepthName",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Dia",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Diam",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "EyeClean",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Girdle",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "GirdleDesc",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "GirdleOpen",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Graining",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "HA",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "InwDate",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "KeyToSymbol",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "LabComment",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "LabDate",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "LotNo",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "LotType",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "LrHalf",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Luster",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "MAmt",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "MDisc",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "MRate",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "MarketDate",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "MfgRemark",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Milky",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "NT_INT",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "NT_OR_INT",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "OLD_PID",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "ORAP",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "OpenCrown",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "OpenGirdle",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "OpenPavallion",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "OpenTable",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "PavExFac",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "PavOpen",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Pav_Ex_Fac",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "PriceName",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "PriceNameId",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Quality",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "ReportDate",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "ReportType",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "SideBlack",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "SideSpot",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "SideWhite",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Diamond");

            migrationBuilder.RenameColumn(
                name: "TableId",
                table: "Diamonds",
                newName: "CaratId");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "Diamond",
                newName: "Table");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Diamond",
                newName: "RapAmount");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Diamond",
                newName: "TypeName");

            migrationBuilder.RenameColumn(
                name: "TableWhite",
                table: "Diamond",
                newName: "TypeId");

            migrationBuilder.RenameColumn(
                name: "TableSpot",
                table: "Diamond",
                newName: "StoneId");

            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "Diamond",
                newName: "Step");

            migrationBuilder.RenameColumn(
                name: "TableId",
                table: "Diamond",
                newName: "CaratId");

            migrationBuilder.RenameColumn(
                name: "TableBlack",
                table: "Diamond",
                newName: "Measurement");

            migrationBuilder.RenameColumn(
                name: "StrLan",
                table: "Diamond",
                newName: "DNA");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Diamond",
                newName: "Certificate");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Diamond",
                newName: "CaratName");

            migrationBuilder.RenameColumn(
                name: "PHt",
                table: "Diamond",
                newName: "RAP");

            migrationBuilder.RenameColumn(
                name: "PAngle",
                table: "Diamond",
                newName: "Depth");

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Diamonds",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Certificate",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Depth",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Table",
                table: "Diamonds",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Diamonds");

            migrationBuilder.DropColumn(
                name: "Table",
                table: "Diamonds");

            migrationBuilder.RenameColumn(
                name: "CaratId",
                table: "Diamonds",
                newName: "TableId");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "Diamond",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Diamond",
                newName: "TableWhite");

            migrationBuilder.RenameColumn(
                name: "Table",
                table: "Diamond",
                newName: "Width");

            migrationBuilder.RenameColumn(
                name: "StoneId",
                table: "Diamond",
                newName: "TableSpot");

            migrationBuilder.RenameColumn(
                name: "Step",
                table: "Diamond",
                newName: "TableName");

            migrationBuilder.RenameColumn(
                name: "RapAmount",
                table: "Diamond",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "RAP",
                table: "Diamond",
                newName: "PHt");

            migrationBuilder.RenameColumn(
                name: "Measurement",
                table: "Diamond",
                newName: "TableBlack");

            migrationBuilder.RenameColumn(
                name: "Depth",
                table: "Diamond",
                newName: "PAngle");

            migrationBuilder.RenameColumn(
                name: "DNA",
                table: "Diamond",
                newName: "StrLan");

            migrationBuilder.RenameColumn(
                name: "Certificate",
                table: "Diamond",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "CaratName",
                table: "Diamond",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "CaratId",
                table: "Diamond",
                newName: "TableId");

            migrationBuilder.AlterColumn<string>(
                name: "TypeId",
                table: "Diamonds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepthId",
                table: "Diamonds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CAngle",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CHt",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CaratSizeId",
                table: "Diamond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaratSizeName",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertiType",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateNo",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrownExFac",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Culet",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DaysType",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepthId",
                table: "Diamond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepthName",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Dia",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Diam",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayOrder",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EyeClean",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Girdle",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GirdleDesc",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GirdleOpen",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Graining",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HA",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InwDate",
                table: "Diamond",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyToSymbol",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabComment",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LabDate",
                table: "Diamond",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "Diamond",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotNo",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotType",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LrHalf",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Luster",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MAmt",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MDisc",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MRate",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MarketDate",
                table: "Diamond",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfgRemark",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Milky",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NT_INT",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NT_OR_INT",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OLD_PID",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ORAP",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenCrown",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenGirdle",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenPavallion",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTable",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PavExFac",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PavOpen",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pav_Ex_Fac",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceName",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceNameId",
                table: "Diamond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quality",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportDate",
                table: "Diamond",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportType",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SideBlack",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SideSpot",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SideWhite",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
