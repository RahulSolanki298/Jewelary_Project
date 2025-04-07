using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddDispOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SymmetryName",
                table: "Diamond",
                newName: "SymmetyName");

            migrationBuilder.AddColumn<int>(
                name: "DispOrder",
                table: "DiamondProperties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Diamond",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconPath",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispOrder",
                table: "DiamondProperties");

            migrationBuilder.DropColumn(
                name: "IconPath",
                table: "Diamond");

            migrationBuilder.RenameColumn(
                name: "SymmetyName",
                table: "Diamond",
                newName: "SymmetryName");

            migrationBuilder.AlterColumn<string>(
                name: "TypeId",
                table: "Diamond",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
