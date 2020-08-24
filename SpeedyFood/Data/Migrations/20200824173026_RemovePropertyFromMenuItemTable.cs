using Microsoft.EntityFrameworkCore.Migrations;

namespace SpeedyFood.Data.Migrations
{
    public partial class RemovePropertyFromMenuItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Spicyness",
                table: "MenuItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Spicyness",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
