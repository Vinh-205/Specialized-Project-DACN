using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class SeedKhoLinhKien_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_CHI_TIET_SUA_CHUA_LINH_KIEN_MaLK",
            //     table: "CHI_TIET_SUA_CHUA");

            // migrationBuilder.DropTable(
            //     name: "LICH_SU_KHO");

            // migrationBuilder.DropIndex(
            //     name: "IX_CHI_TIET_SUA_CHUA_MaLK",
            //     table: "CHI_TIET_SUA_CHUA");

            // migrationBuilder.DropColumn(
            //     name: "DonGia",
            //     table: "CHI_TIET_SUA_CHUA");

            // migrationBuilder.DropColumn(
            //     name: "IsDeleted",
            //     table: "CHI_TIET_SUA_CHUA");

            // migrationBuilder.DropColumn(
            //     name: "MaLK",
            //     table: "CHI_TIET_SUA_CHUA");

            // migrationBuilder.DropColumn(
            //     name: "ProcessingType",
            //     table: "CHI_TIET_SUA_CHUA");

            // migrationBuilder.DropColumn(
            //     name: "SoLuong",
            //     table: "CHI_TIET_SUA_CHUA");

            // migrationBuilder.CreateTable(
            //     name: "SU_DUNG_LINH_KIEN",
            //     columns: table => new
            //     {
            //         MaCT = table.Column<int>(type: "int", nullable: false),
            //         MaLK = table.Column<int>(type: "int", nullable: false),
            //         SoLuong = table.Column<int>(type: "int", nullable: false),
            //         DonGiaXuat = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_SU_DUNG_LINH_KIEN", x => new { x.MaCT, x.MaLK });
            //         table.ForeignKey(
            //             name: "FK_SU_DUNG_LINH_KIEN_CHI_TIET_SUA_CHUA_MaCT",
            //             column: x => x.MaCT,
            //             principalTable: "CHI_TIET_SUA_CHUA",
            //             principalColumn: "MaCT",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_SU_DUNG_LINH_KIEN_LINH_KIEN_MaLK",
            //             column: x => x.MaLK,
            //             principalTable: "LINH_KIEN",
            //             principalColumn: "MaLK",
            //             onDelete: ReferentialAction.Restrict);
            //     });

            migrationBuilder.InsertData(
                table: "LINH_KIEN",
                columns: new[] { "MaLK", "CoTrangThai", "DonGia", "LoaiLK", "SoLuongTon", "TenLK" },
                values: new object[,]
                {
                    { 1, true, 650000m, "RAM", 50, "RAM Corsair Vengeance LPX 8GB DDR4 3200MHz" },
                    { 2, true, 1150000m, "RAM", 30, "RAM Kingston Fury Beast 16GB DDR4 3200MHz" },
                    { 3, true, 1450000m, "SSD", 25, "SSD Samsung 970 EVO Plus 500GB NVMe" },
                    { 4, true, 850000m, "SSD", 40, "SSD Kingston NV2 250GB PCIe 4.0 NVMe" },
                    { 5, true, 1100000m, "HDD", 20, "HDD WD Blue 1TB 3.5\" SATA 3" },
                    { 6, true, 1250000m, "Nguồn", 15, "Nguồn Corsair CV550 550W 80 Plus Bronze" },
                    { 7, true, 6200000m, "Màn hình", 10, "Màn hình Dell UltraSharp U2422H 24\" IPS" },
                    { 8, true, 850000m, "Pin", 10, "Pin Laptop Dell Latitude E5470 (Zin)" },
                    { 9, true, 350000m, "Sạc", 30, "Sạc Laptop Dell 65W Kim to" },
                    { 10, true, 450000m, "Bàn phím", 15, "Bàn phím Laptop Dell Inspiron 15 3000" },
                    { 11, true, 1950000m, "Mainboard", 10, "Mainboard ASUS PRIME H610M-K D4" },
                    { 12, true, 2450000m, "CPU", 10, "CPU Intel Core i3-12100F" },
                    { 13, true, 180000m, "Vật tư khác", 100, "Keo tản nhiệt Thermal Grizzly Kryonaut 1g" },
                    { 14, true, 350000m, "Tản nhiệt", 20, "Quạt tản nhiệt CPU Jonsbo CR-1000" },
                    { 15, true, 450000m, "Card Mạng", 15, "Card Wi-Fi Intel AX210 NGW M.2" }
                });

            // migrationBuilder.CreateIndex(
            //     name: "IX_SU_DUNG_LINH_KIEN_MaLK",
            //     table: "SU_DUNG_LINH_KIEN",
            //     column: "MaLK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SU_DUNG_LINH_KIEN");

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "LINH_KIEN",
                keyColumn: "MaLK",
                keyValue: 15);

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
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LoaiGiaoDich = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongThayDoi = table.Column<int>(type: "int", nullable: false)
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
    }
}
