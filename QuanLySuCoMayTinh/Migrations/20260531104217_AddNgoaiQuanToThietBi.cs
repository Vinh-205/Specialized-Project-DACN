using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    public partial class AddNgoaiQuanToThietBi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NgoaiQuan",
                table: "THIET_BI",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgoaiQuan",
                table: "THIET_BI");
        }
    }
}
