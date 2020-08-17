using Microsoft.EntityFrameworkCore.Migrations;

namespace SpeedyFood.Data.Migrations
{
    public partial class AddPropertyToMenuItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Spicyness",
                table: "MenuItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Spicyness",
                table: "MenuItems");
        }
    }
}
