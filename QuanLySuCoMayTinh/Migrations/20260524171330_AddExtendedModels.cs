using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DANH_GIA_DICH_VU",
                columns: table => new
                {
                    MaDG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    SoSao = table.Column<int>(type: "int", nullable: false),
                    BinhLuan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NgayDG = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_GIA_DICH_VU", x => x.MaDG);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_DICH_VU_PHIEU_TIEP_NHAN_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PHIEU_TIEP_NHAN",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HINH_ANH_SU_CO",
                columns: table => new
                {
                    MaHA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayChup = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HINH_ANH_SU_CO", x => x.MaHA);
                    table.ForeignKey(
                        name: "FK_HINH_ANH_SU_CO_PHIEU_TIEP_NHAN_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PHIEU_TIEP_NHAN",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LICH_SU_TRANG_THAI",
                columns: table => new
                {
                    MaLS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    TrangThaiCu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiMoi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ma_KTV = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LICH_SU_TRANG_THAI", x => x.MaLS);
                    table.ForeignKey(
                        name: "FK_LICH_SU_TRANG_THAI_PHIEU_TIEP_NHAN_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PHIEU_TIEP_NHAN",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LICH_SU_TRANG_THAI_USER_Ma_KTV",
                        column: x => x.Ma_KTV,
                        principalTable: "USER",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LINH_KIEN",
                columns: table => new
                {
                    MaLK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLK = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LoaiLK = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    CoTrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LINH_KIEN", x => x.MaLK);
                });

            migrationBuilder.CreateTable(
                name: "SU_DUNG_LINH_KIEN",
                columns: table => new
                {
                    MaCT = table.Column<int>(type: "int", nullable: false),
                    MaLK = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaXuat = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
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
                name: "IX_DANH_GIA_DICH_VU_MaPhieu",
                table: "DANH_GIA_DICH_VU",
                column: "MaPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HINH_ANH_SU_CO_MaPhieu",
                table: "HINH_ANH_SU_CO",
                column: "MaPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_TRANG_THAI_Ma_KTV",
                table: "LICH_SU_TRANG_THAI",
                column: "Ma_KTV");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_TRANG_THAI_MaPhieu",
                table: "LICH_SU_TRANG_THAI",
                column: "MaPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_SU_DUNG_LINH_KIEN_MaLK",
                table: "SU_DUNG_LINH_KIEN",
                column: "MaLK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DANH_GIA_DICH_VU");

            migrationBuilder.DropTable(
                name: "HINH_ANH_SU_CO");

            migrationBuilder.DropTable(
                name: "LICH_SU_TRANG_THAI");

            migrationBuilder.DropTable(
                name: "SU_DUNG_LINH_KIEN");

            migrationBuilder.DropTable(
                name: "LINH_KIEN");
        }
    }
}
