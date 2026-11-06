using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class AddGhiChuNoiBo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GhiChuNoiBo",
                table: "PHIEU_TIEP_NHAN",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GhiChuNoiBo",
                table: "PHIEU_TIEP_NHAN");
        }
    }
}
