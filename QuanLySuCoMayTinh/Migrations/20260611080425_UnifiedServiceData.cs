using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedServiceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SU_DUNG_LINH_KIEN");

            migrationBuilder.AddColumn<decimal>(
                name: "DonGia",
                table: "CHI_TIET_SUA_CHUA",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CHI_TIET_SUA_CHUA",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaLK",
                table: "CHI_TIET_SUA_CHUA",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingType",
                table: "CHI_TIET_SUA_CHUA",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SoLuong",
                table: "CHI_TIET_SUA_CHUA",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LICH_SU_KHO",
                columns: table => new
                {
                    MaGD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLK = table.Column<int>(type: "int", nullable: false),
                    SoLuongThayDoi = table.Column<int>(type: "int", nullable: false),
                    LoaiGiaoDich = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LICH_SU_KHO", x => x.MaGD);
                    table.ForeignKey(
                        name: "FK_LICH_SU_KHO_LINH_KIEN_MaLK",
                        column: x => x.MaLK,
                        principalTable: "LINH_KIEN",
                        principalColumn: "MaLK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHI_TIET_SUA_CHUA_MaLK",
                table: "CHI_TIET_SUA_CHUA",
                column: "MaLK");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_KHO_MaLK",
                table: "LICH_SU_KHO",
                column: "MaLK");

            migrationBuilder.AddForeignKey(
                name: "FK_CHI_TIET_SUA_CHUA_LINH_KIEN_MaLK",
                table: "CHI_TIET_SUA_CHUA",
                column: "MaLK",
                principalTable: "LINH_KIEN",
                principalColumn: "MaLK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CHI_TIET_SUA_CHUA_LINH_KIEN_MaLK",
                table: "CHI_TIET_SUA_CHUA");

            migrationBuilder.DropTable(
                name: "LICH_SU_KHO");

            migrationBuilder.DropIndex(
                name: "IX_CHI_TIET_SUA_CHUA_MaLK",
                table: "CHI_TIET_SUA_CHUA");

            migrationBuilder.DropColumn(
                name: "DonGia",
                table: "CHI_TIET_SUA_CHUA");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CHI_TIET_SUA_CHUA");

            migrationBuilder.DropColumn(
                name: "MaLK",
                table: "CHI_TIET_SUA_CHUA");

            migrationBuilder.DropColumn(
                name: "ProcessingType",
                table: "CHI_TIET_SUA_CHUA");

            migrationBuilder.DropColumn(
                name: "SoLuong",
                table: "CHI_TIET_SUA_CHUA");

            migrationBuilder.CreateTable(
                name: "SU_DUNG_LINH_KIEN",
                columns: table => new
                {
                    MaCT = table.Column<int>(type: "int", nullable: false),
                    MaLK = table.Column<int>(type: "int", nullable: false),
                    DonGiaXuat = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SU_DUNG_LINH_KIEN", x => new { x.MaCT, x.MaLK });
                    table.ForeignKey(
                        name: "FK_SU_DUNG_LINH_KIEN_CHI_TIET_SUA_CHUA_MaCT",
                        column: x => x.MaCT,
                        principalTable: "CHI_TIET_SUA_CHUA",
                        principalColumn: "MaCT",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SU_DUNG_LINH_KIEN_LINH_KIEN_MaLK",
                        column: x => x.MaLK,
                        principalTable: "LINH_KIEN",
                        principalColumn: "MaLK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SU_DUNG_LINH_KIEN_MaLK",
                table: "SU_DUNG_LINH_KIEN",
                column: "MaLK");
        }
    }
}
