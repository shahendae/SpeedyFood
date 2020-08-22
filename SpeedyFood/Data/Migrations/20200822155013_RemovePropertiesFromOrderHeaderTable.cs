using Microsoft.EntityFrameworkCore.Migrations;

namespace SpeedyFood.Data.Migrations
{
    public partial class RemovePropertiesFromOrderHeaderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponDiscount",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "OrderHeaders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CouponDiscount",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
