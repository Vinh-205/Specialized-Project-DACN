using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDiagnosticsSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 5, 6 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 6, 7 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 7, 8 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 8, 9 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 9, 10 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 10, 11 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 11, 12 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 3, 13 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 4, 13 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 12, 14 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 13, 15 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 14, 16 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 15, 17 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 7, 18 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 2, 19 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 16, 20 });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 1,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 3, "1. Tắt máy hoàn toàn và rút dây nguồn.\n2. Mở thùng máy, xác định vị trí card đồ họa rời (nếu có).\n3. Rút card ra khỏi khe PCIe, dùng tẩy gôm hoặc cọ sạch tiếp điểm vàng.\n4. Kiểm tra khe PCIe trên mainboard có bụi bẩn không, thổi sạch bằng cồn IPA.\n5. Cắm lại card chắc chắn, đảm bảo chốt khe PCIe đã bật lên.\n6. Cắm đủ dây nguồn 6-pin/8-pin vào card nếu cần.\n7. Bật máy và cài lại driver từ trang chủ NVIDIA/AMD (DDU để gỡ driver cũ trước).\n8. Nếu vẫn lỗi → thay card đồ họa.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 2,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 1, "1. Kiểm tra cả 2 đầu cáp HDMI/DisplayPort/VGA (đầu máy tính và đầu màn hình).\n2. Rút cáp ra và cắm lại chắc chắn, vặn vít cố định nếu có.\n3. Thử cổng output khác trên máy tính (ví dụ từ VGA sang HDMI).\n4. Thử cáp khác để loại trừ khả năng cáp hỏng.\n5. Test màn hình với máy tính khác để xác định nguyên nhân.\n6. Nếu cáp hỏng → mua cáp mới (ưu tiên HDMI 2.0 hoặc DP 1.4).\n7. Với laptop: kiểm tra cáp LVDS/eDP nội bộ, cần tháo máy.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 3,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 2, "1. Tải công cụ DDU (Display Driver Uninstaller) từ trang wagnardsoft.com.\n2. Khởi động Windows ở chế độ Safe Mode.\n3. Chạy DDU, chọn GPU của bạn (NVIDIA/AMD/Intel), chọn \"Clean and Restart\".\n4. Sau khi restart, tải driver mới nhất từ:\n   - NVIDIA: nvidia.com/drivers\n   - AMD: amd.com/support  \n   - Intel: intel.com/content/drivers\n5. Cài đặt driver và khởi động lại máy.\n6. Nếu lỗi vẫn còn → thử roll back về driver cũ hơn (phiên bản trước đó).", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 4,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 4, "1. Sử dụng đèn pin rọi vào màn hình trong phòng tối để kiểm tra: nếu thấy hình mờ thì đèn nền hỏng, không phải màn hình hỏng hoàn toàn.\n2. Kiểm tra cáp nối màn hình với mainboard bên trong laptop (yêu cầu tháo máy).\n3. Nếu cáp tốt → lỗi do:\n   a. Inverter board (màn hình CCFL cũ) → thay inverter.\n   b. LED driver trên bo mạch màn hình → thay màn hình.\n4. Đặt mua màn hình thay thế đúng model (tra model number phía sau màn hình).\n5. Thay màn hình theo hướng dẫn iFixit.com theo model máy.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 5,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 4, "1. Tắt máy, rút pin (nếu được).\n2. Mở nắp màn hình, tháo viền nhựa xung quanh màn hình.\n3. Xác định cáp kết nối màn hình (cáp LVDS/eDP) và cáp camera/WiFi.\n4. Kiểm tra đầu cắm của cáp vào bo mạch màn hình và mainboard.\n5. Rút và cắm lại chắc chắn, dán keo chịu nhiệt 2 mặt để cố định nếu lỏng.\n6. Lắp lại viền và test trước khi vặn vít hoàn toàn.\n7. Nếu cáp đứt ngầm (uốn nhiều lần) → đặt cáp thay thế đúng model.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 6,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 3, "1. Test PSU bằng cách paperclip test: dùng dây kẹp giấy nối chân màu xanh lá (PS_ON) và chân đen (GND) trên đầu cắm 24-pin. Nếu quạt PSU không quay = PSU chết.\n2. Dùng máy đo PSU chuyên dụng để đo điện áp các đường +12V/+5V/+3.3V.\n3. Nếu PSU hỏng → thay PSU mới có công suất phù hợp (tính công suất theo GPU + CPU).\n4. Khuyến nghị: dùng PSU đạt chứng nhận 80 PLUS (Bronze trở lên).\n5. Đảm bảo cắm đủ connector: dây 24-pin mainboard, dây CPU 4/8-pin.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 7,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 3, "1. Nếu pin phồng: KHÔNG sử dụng tiếp, pin phồng có nguy cơ cháy nổ.\n2. Tắt máy ngay lập tức, không sạc thêm.\n3. Đặt máy ở nơi thoáng mát, tránh xa vật dễ cháy.\n4. Đặt mua pin chính hãng hoặc pin tương thích chất lượng cao (tránh pin giá rẻ không rõ nguồn gốc).\n5. Tháo pin cũ: với pin rời thì tháo trực tiếp; với pin dán (built-in) cần dụng cụ mở máy chuyên dụng.\n6. Pin chai (dung lượng < 40% so với ban đầu): thay pin mới.\n7. Để kiểm tra sức khỏe pin: dùng BatteryReport (powercfg /batteryreport) hoặc HWiNFO64.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 8,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 4, "1. Thử sạc bằng adapter khác cùng công suất để loại trừ lỗi adapter.\n2. Kiểm tra cổng sạc xem có bụi bẩn, cong vênh chân tiếp xúc không.\n3. Thổi sạch cổng sạc bằng khí nén.\n4. Nếu dùng sạc USB-C: thử cổng USB-C khác trên máy.\n5. Nếu vẫn không sạc: cần đo dòng sạc bằng đồng hồ vạn năng hoặc USB meter.\n6. Lỗi IC sạc đòi hỏi hàn chip SMD trên mainboard → yêu cầu kỹ thuật viên có kinh nghiệm microsoldering.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 9,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 3, "1. Kiểm tra đầu sạc có bị cong, lệch, gãy chân không.\n2. Dùng đèn pin soi vào cổng sạc trên máy, xem có chân cong, vỡ nhựa không.\n3. Thử cắm sạc ở nhiều góc độ khác nhau để xác nhận tiếp xúc lỏng.\n4. Tháo máy và kiểm tra bo mạch DC-in jack:\n   a. Nếu cổng sạc rời: tháo và hàn/thay cổng mới.\n   b. Nếu cổng sạc liền mainboard: cần hàn lại chân sạc hoặc thay mainboard.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 10,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 2, "1. Tắt máy và rút nguồn hoàn toàn.\n2. Mở vỏ máy tính (laptop: tháo nắp lưng, PC: mở thùng máy).\n3. Dùng bình khí nén thổi sạch bụi trong các cánh quạt, tản nhiệt, lưới lọc bụi.\n4. Chú ý thổi từ trong ra ngoài để đẩy bụi ra theo lỗ thông gió.\n5. Dùng cọ mềm để làm sạch các vùng khó tiếp cận.\n6. Kiểm tra quạt: quay tay thử xem có trơn tru không, thay quạt nếu cần.\n7. Sau khi lắp lại, dùng HWiNFO64 hoặc Core Temp để theo dõi nhiệt độ.\n8. Nhiệt độ CPU idle <50°C, load <85°C là bình thường.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 11,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 2, "1. Tháo cụm tản nhiệt ra khỏi CPU/GPU.\n2. Dùng cồn isopropyl 90%+ và tăm bông lau sạch thermal paste cũ trên cả CPU và mặt đế tản nhiệt.\n3. Đợi khô hoàn toàn.\n4. Bôi thermal paste mới (khuyến nghị: Thermal Grizzly Kryonaut, Arctic MX-4, hoặc Noctua NT-H1).\n5. Lượng paste: bằng hạt gạo đặt giữa CPU. Không bôi quá nhiều.\n6. Lắp tản nhiệt lại, vặn vít theo hình chữ X (không vặn một vít hết rồi mới qua vít khác).\n7. Kiểm tra nhiệt độ sau 30 phút chạy load cao.\nLưu ý: nên thay thermal paste mỗi 2-3 năm.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 12,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 2, "1. Tắt máy, rút nguồn/pin.\n2. Tháo tất cả thanh RAM ra.\n3. Dùng tẩy gôm chà nhẹ các tiếp điểm vàng của RAM, thổi sạch bụi trong khe RAM.\n4. Cắm lại từng thanh RAM một, test thử từng thanh để tìm thanh hỏng.\n5. Thử cắm RAM vào khe khác nếu có nhiều khe.\n6. Chạy MemTest86 (boot từ USB) để kiểm tra lỗi RAM chuyên sâu (chạy ít nhất 2 vòng).\n7. Nếu MemTest86 báo lỗi → thanh RAM hoặc khe cắm bị hỏng, cần thay RAM.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 13,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 3, "1. NGAY LẬP TỨC sao lưu toàn bộ dữ liệu quan trọng ra ổ khác hoặc cloud.\n2. Chạy CrystalDiskInfo để kiểm tra SMART status của ổ cứng. \n   - Chú ý: Reallocated Sectors Count, Pending Sectors, Uncorrectable Sectors.\n3. Chạy chkdsk /f /r /b C: (từ Command Prompt quyền Admin, restart để scan).\n4. Nếu số bad sector nhiều và tăng liên tục → ổ cứng sắp chết, phải thay ngay.\n5. Thay ổ cứng HDD bằng SSD để có hiệu năng tốt hơn và bền hơn.\n6. Clone dữ liệu từ ổ cũ sang ổ mới bằng Macrium Reflect Free hoặc Clonezilla.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 14,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 3, "1. Tải phần mềm quản lý SSD từ nhà sản xuất:\n   - Samsung: Samsung Magician\n   - Western Digital: WD Dashboard\n   - Kingston: Kingston SSD Manager\n2. Kiểm tra SMART health, TBW (Terabytes Written) còn lại.\n3. Cập nhật firmware SSD nếu có bản mới.\n4. Nếu SSD trong trạng thái Read-Only (ổ đọc-only để bảo vệ dữ liệu trước khi chết):\n   a. Sao lưu dữ liệu ngay.\n   b. Secure Erase SSD để reset về trạng thái mới (mất toàn bộ dữ liệu).\n5. Nếu SSD không nhận dạng: thử trên máy khác, kiểm tra khe M.2/SATA.\n6. Cân nhắc thay SSD mới nếu TBW đã dùng > 80%.", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 15,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 2, "1. Mở Command Prompt quyền Administrator.\n2. Chạy: sfc /scannow → chờ quá trình kiểm tra và sửa file hệ thống Windows.\n3. Nếu SFC báo lỗi không sửa được, chạy tiếp:\n   DISM /Online /Cleanup-Image /RestoreHealth\n4. Restart máy và chạy lại sfc /scannow lần nữa.\n5. Nếu vẫn còn lỗi, boot từ USB Windows và vào Recovery Environment:\n   - Chọn Startup Repair\n   - Hoặc mở CMD và chạy: bootrec /fixmbr, bootrec /fixboot, bootrec /rebuildbcd\n6. Trường hợp xấu nhất: reinstall Windows (nên chọn \"Keep files\" để bảo toàn dữ liệu).", true });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 16,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { 3, "1. Boot từ USB cài Windows (tải Media Creation Tool từ microsoft.com).\n2. Chọn \"Repair your computer\" → Troubleshoot → Advanced Options → Command Prompt.\n3. Gõ các lệnh sau:\n   diskpart\n   list disk\n   select disk 0 (chọn ổ cứng chứa Windows)\n   list vol\n   exit\n4. Với MBR:\n   bootrec /fixmbr\n   bootrec /fixboot\n   bootrec /scanos\n   bootrec /rebuildbcd\n5. Với GPT (UEFI): dùng bcdboot C:\\Windows /s G: /f UEFI (G: là phân vùng EFI).\n6. Restart và thử boot bình thường.", true });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 1,
                column: "TenLoi",
                value: "GPU/Card đồ họa bị lỗi hoặc slot PCIe hỏng");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 2,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần cứng", "Cáp tín hiệu màn hình (HDMI/DP/VGA) lỏng hoặc hỏng" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 3,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Hệ điều hành", "Driver đồ họa bị lỗi hoặc không tương thích" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 4,
                column: "TenLoi",
                value: "Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 5,
                column: "TenLoi",
                value: "Kết nối nội bộ màn hình laptop bị lỏng");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 6,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Nguồn & Pin", "Bộ nguồn PSU bị hỏng hoặc không đủ công suất" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 7,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Nguồn & Pin", "Pin Lithium bị phồng, chai hoặc hỏng tế bào" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 8,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Nguồn & Pin", "IC sạc (Charging IC) trên mainboard bị lỗi" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 9,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Nguồn & Pin", "Cổng sạc DC-in bị lỏng hoặc hỏng" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 10,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần cứng", "Hệ thống tản nhiệt bị nghẹt bụi, quá nhiệt CPU/GPU" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 11,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần cứng", "Thermal paste (keo tản nhiệt) bị khô, cần thay mới" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 12,
                column: "TenLoi",
                value: "RAM lỏng slot hoặc bị lỗi chip nhớ");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 13,
                column: "TenLoi",
                value: "Ổ cứng HDD bị bad sector (vật lý)");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 14,
                column: "TenLoi",
                value: "Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 15,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Hệ điều hành", "File hệ thống bị corrupt (chkdsk lỗi)" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 16,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Hệ điều hành", "MBR/GPT bị ghi đè hoặc hỏng" });

            migrationBuilder.InsertData(
                table: "MA_LOI",
                columns: new[] { "MaLoiID", "PhanLoai", "Status", "TenLoi" },
                values: new object[,]
                {
                    { 17, "Hệ điều hành", true, "BOOTMGR bị hỏng hoặc mất" },
                    { 18, "Hệ điều hành", true, "Windows Registry bị lỗi sau cập nhật hoặc virus" },
                    { 19, "Hệ điều hành", true, "Xung đột phần mềm hoặc driver sau Windows Update" },
                    { 20, "Phần mềm", true, "Nhiễm Ransomware (mã hóa file đòi tiền chuộc)" },
                    { 21, "Phần mềm", true, "Nhiễm Adware/Spyware (hiện quảng cáo, thu thập dữ liệu)" },
                    { 22, "Phần mềm", true, "Thiếu Visual C++ Redistributable hoặc .NET Framework" },
                    { 23, "Hệ điều hành", true, "Lỗi BSOD 0x0000007E: SYSTEM_THREAD_EXCEPTION_NOT_HANDLED" },
                    { 24, "Hệ điều hành", true, "Lỗi BSOD 0x000000D1: DRIVER_IRQL_NOT_LESS_OR_EQUAL" },
                    { 25, "Hệ điều hành", true, "Lỗi BSOD 0xC000021A: STATUS_SYSTEM_PROCESS_TERMINATED" },
                    { 26, "Hệ điều hành", true, "Lỗi BSOD 0x0000001A: MEMORY_MANAGEMENT" },
                    { 27, "Hệ điều hành", true, "Lỗi BSOD 0x00000050: PAGE_FAULT_IN_NONPAGED_AREA" },
                    { 28, "Mạng", true, "Driver WiFi card bị lỗi hoặc không tương thích" },
                    { 29, "Phần cứng", true, "WiFi card bị lỏng hoặc hỏng anten" },
                    { 30, "Mạng", true, "Lỗi cài đặt DNS sai hoặc DNS bị cache" },
                    { 31, "Mạng", true, "TCP/IP stack bị hỏng (Winsock corrupt)" },
                    { 32, "Mạng", true, "Driver Ethernet card bị lỗi hoặc hỏng card LAN" },
                    { 33, "Phần cứng", true, "Bàn phím bị tràn nước hoặc hỏng mạch" },
                    { 34, "Phần cứng", true, "Driver touchpad bị lỗi hoặc touchpad bị khóa" },
                    { 35, "Phần mềm", true, "Driver âm thanh (audio driver) bị lỗi" },
                    { 36, "Phần cứng", true, "Lỗi driver USB hoặc USB hub trên mainboard" },
                    { 37, "Phần mềm", true, "Driver máy in bị lỗi hoặc cổng in bị tắc" }
                });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 1,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Máy vẫn chạy nhưng màn hình tối đen, không có tín hiệu", "Màn hình không lên sau khi bật máy" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 2,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Hình ảnh trên màn hình nhấp nháy liên tục hoặc giật nhẹ", "Màn hình nhấp nháy / bị giật" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 3,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Xuất hiện các đường kẻ màu bất thường chạy ngang hoặc dọc", "Màn hình có sọc ngang hoặc dọc" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 4,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Độ sáng màn hình bị giảm đột ngột, khó nhìn rõ nội dung", "Màn hình bị mờ, tối hơn bình thường" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 5,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Màn hình bị nứt, vỡ kính, hiện hình ảnh bị lệch hoặc loang màu", "Màn hình bị vỡ / đập vỡ vật lý" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 6,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Màn hình hiển thị với độ phân giải thấp, hình bị vỡ pixcel", "Phân giải màn hình bị đặt sai" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 7,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Xuất hiện các chấm đen hoặc màu cố định không thay đổi trên màn hình", "Dead pixel / điểm chết trên màn hình" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 8,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Toàn bộ màn hình hiển thị một màu đơn sắc bất thường", "Màn hình đơn sắc (xanh/đỏ/xanh lá)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 9,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Nhấn nút nguồn nhưng máy không phản ứng gì", "Máy không bật được" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 10,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Máy đang dùng thì tự nhiên tắt ngay, không thông báo", "Máy tự tắt đột ngột" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 11,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Máy liên tục khởi động rồi tắt, không vào được hệ điều hành", "Máy bật lên lại tắt ngay (boot loop)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 12,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Cắm sạc vào nhưng icon pin không hiện đang sạc, không tăng %", "Pin không sạc được" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 13,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Thời lượng pin sụt giảm đột ngột, sạc đầy nhưng chỉ dùng được vài giờ", "Pin hao nhanh bất thường" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 14,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Vỏ máy hoặc bàn phím nóng rát, quạt tản nhiệt kêu to", "Máy bị nóng bất thường" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 15,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Máy báo \"Đang không sạc\" hoặc \"Kết nối sạc nhưng không sạc\"", "Sạc đúng loại nhưng không nhận sạc" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 16,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Máy phản hồi rất chậm, mở ứng dụng lâu, đơ thường xuyên", "Máy chạy chậm bất thường" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 17,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Màn hình đứng yên, chuột bàn phím không phản hồi", "Máy bị đơ (Freeze/Hang)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 18,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Máy hiện màn hình xanh với mã lỗi và khởi động lại", "Màn hình xanh chết chóc (BSOD)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 19,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Windows cảnh báo ổ đĩa gần đầy", "Xuất hiện thông báo \"Low Disk Space\"" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 20,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Nghe thấy tiếng kêu click, lạch cạch hoặc mài từ ổ cứng", "Ổ cứng bị kêu lạ (click, grind)" });

            migrationBuilder.InsertData(
                table: "TRIEU_CHUNG",
                columns: new[] { "MaTC", "MoTa", "TenTC" },
                values: new object[,]
                {
                    { 21, "BIOS không tìm thấy ổ cứng, thông báo \"No Boot Device\"", "Máy không nhận ổ cứng khi khởi động" },
                    { 22, "Các file không mở được, báo lỗi dữ liệu hoặc biến mất", "File/thư mục bị mất hoặc corrupt" },
                    { 23, "Tốc độ đọc/ghi SSD thấp hơn thông số nhà sản xuất nhiều", "SSD chậm hơn dự kiến" },
                    { 24, "Máy không vào được Windows, dừng ở màn hình loading", "Windows không khởi động được" },
                    { 25, "Thông báo \"Missing operating system\" hoặc \"BOOTMGR is missing\"", "Bị lỗi \"Missing operating system\"" },
                    { 26, "Bản cập nhật Windows không cài được, bị treo giữa chừng", "Windows Update bị lỗi" },
                    { 27, "Phần mềm tự đóng đột ngột, hiện thông báo lỗi hoặc \"Not Responding\"", "Ứng dụng bị crash liên tục" },
                    { 28, "Xuất hiện quảng cáo lạ, máy chậm, file bị mã hóa đòi tiền chuộc", "Máy bị nhiễm virus/malware" },
                    { 29, "Sau khi khởi động, màn hình nền trống, không có biểu tượng nào", "Desktop/icon bị mất sau khởi động" },
                    { 30, "Thiết bị xuất hiện dấu chấm than vàng trong Device Manager", "Driver thiết bị bị lỗi" },
                    { 31, "Máy không tìm thấy mạng WiFi hoặc kết nối nhưng không có internet", "Không kết nối được WiFi" },
                    { 32, "Tín hiệu WiFi lúc có lúc không, hay bị mất kết nối tự nhiên", "WiFi yếu, hay bị ngắt kết nối" },
                    { 33, "Cắm cáp mạng nhưng máy không nhận, không có internet", "Không kết nối được mạng LAN (dây)" },
                    { 34, "Trình duyệt báo lỗi DNS hoặc \"This site can't be reached\"", "Không vào được một số trang web" },
                    { 35, "Không ghép nối được với loa, tai nghe, chuột Bluetooth", "Bluetooth không kết nối được thiết bị" },
                    { 36, "Một số phím bấm không có tác dụng hoặc bấm ra ký tự sai", "Bàn phím một số phím bị liệt" },
                    { 37, "Bàn di chuột không phản hồi khi chạm hoặc vuốt", "Touchpad không hoạt động" },
                    { 38, "Không nghe thấy âm thanh từ loa máy tính mặc dù âm lượng đã bật", "Loa không có âm thanh" },
                    { 39, "Âm thanh phát ra bị rè, bị méo, nghe không rõ", "Âm thanh bị rè, méo" },
                    { 40, "Ứng dụng không nhận được webcam, hình ảnh đen hoặc bị vỡ", "Webcam không hoạt động" },
                    { 41, "Cắm USB vào nhưng máy không phát hiện thiết bị", "Cổng USB không nhận thiết bị" },
                    { 42, "Gửi lệnh in nhưng máy in không làm gì, trạng thái offline", "Máy in không phản hồi" },
                    { 43, "Quạt chạy 100% công suất liên tục kể cả khi máy nhàn rỗi", "Quạt tản nhiệt kêu to liên tục" },
                    { 44, "Máy thường xuyên BSOD với lỗi khác nhau, random crash", "RAM bị lỗi / không ổn định" }
                });

            migrationBuilder.InsertData(
                table: "TRIEU_CHUNG_LOI",
                columns: new[] { "MaLoi", "MaTC" },
                values: new object[,]
                {
                    { 5, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 1, 3 },
                    { 4, 3 },
                    { 3, 4 },
                    { 6, 9 },
                    { 6, 10 },
                    { 10, 10 },
                    { 12, 10 },
                    { 15, 11 },
                    { 16, 11 },
                    { 7, 12 },
                    { 8, 12 },
                    { 9, 12 },
                    { 7, 13 },
                    { 10, 14 },
                    { 11, 14 },
                    { 12, 16 },
                    { 13, 16 },
                    { 12, 18 }
                });

            migrationBuilder.InsertData(
                table: "GIAI_PHAP",
                columns: new[] { "MaGP", "MaLoi", "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[,]
                {
                    { 17, 17, 2, "1. Boot từ USB/DVD cài Windows.\n2. Chọn \"Repair your computer\" → Command Prompt.\n3. Gõ: bootrec /fixmbr → Enter.\n4. Gõ: bootrec /fixboot → Enter (có thể báo \"Access is denied\" với UEFI, bỏ qua).\n5. Gõ: bootrec /rebuildbcd → Enter.\n6. Nếu vẫn lỗi, thử: bcdboot C:\\Windows /s C: /f ALL.\n7. Restart không có USB và thử boot.\n8. Trường hợp thất bại: cài lại Windows hoặc clone lại bản backup.", true },
                    { 18, 18, 3, "1. Boot vào Safe Mode (F8 hoặc Shift+Restart → Troubleshoot → Advanced → Startup Settings → F4).\n2. Chạy sfc /scannow trong Safe Mode.\n3. Dùng công cụ: CCleaner (Registry Cleaner) hoặc Wise Registry Cleaner để dọn lỗi registry.\n4. Khôi phục Registry từ backup:\n   - Mở regedit → File → Import → chọn file .reg backup.\n   - Hoặc từ Recovery: copy registry từ thư mục C:\\Windows\\System32\\config\\RegBack.\n5. Dùng System Restore để quay về điểm phục hồi trước khi xảy ra lỗi.\n6. Nếu không được: Refresh Windows (giữ lại ứng dụng và file) hoặc cài lại Windows.", true },
                    { 19, 19, 2, "1. Vào Windows Update → Xem lịch sử cập nhật, ghi nhớ bản update gần nhất.\n2. Thử uninstall bản update đó: Settings → Windows Update → Update History → Uninstall updates.\n3. Nếu máy không vào được Windows:\n   a. Boot vào Safe Mode.\n   b. Gỡ driver hoặc phần mềm vừa cài bằng Device Manager hoặc Programs & Features.\n4. Dùng System Restore về thời điểm trước khi cài update.\n5. Dùng công cụ Windows Update Troubleshooter từ Microsoft.\n6. Tạm thời tắt Windows Update để tránh cài lại bản lỗi (Settings → Pause Updates).", true },
                    { 20, 20, 4, "⚠️ QUAN TRỌNG: KHÔNG trả tiền chuộc, không đảm bảo lấy lại dữ liệu.\n1. Ngắt kết nối mạng NGAY LẬP TỨC (rút cáp LAN, tắt WiFi) để ngăn lan rộng.\n2. Không tắt máy đột ngột (một số ransomware mã hóa trong RAM, tắt máy có thể mất cơ hội giải mã).\n3. Xác định tên ransomware: truy cập https://id-ransomware.malwarehunterteam.com/ và upload file mã hóa.\n4. Tìm công cụ giải mã tại: https://www.nomoreransom.org/\n5. Nếu không có công cụ giải mã: Xóa máy sạch và cài lại Windows.\n6. Khôi phục dữ liệu từ bản backup (nếu có).\n7. Sau xử lý: cài đặt antivirus tốt, bật Windows Defender, bật tường lửa.", true },
                    { 21, 21, 2, "1. Tải và chạy Malwarebytes Free (malwarebytes.com) để quét toàn diện.\n2. Tải và chạy AdwCleaner (malwarebytes.com/adwcleaner) để loại bỏ adware.\n3. Vào Browser: Settings → Extensions → gỡ tất cả extension lạ không rõ nguồn gốc.\n4. Reset browser về mặc định (Chrome: Settings → Reset settings).\n5. Kiểm tra Startup programs (Task Manager → Startup) gỡ chương trình lạ.\n6. Kiểm tra Scheduled Tasks (Task Scheduler) xóa task độc hại.\n7. Kiểm tra lại hosts file: C:\\Windows\\System32\\drivers\\etc\\hosts.\n8. Cài Windows Defender và bật Real-time Protection.", true },
                    { 22, 22, 1, "1. Đọc thông báo lỗi để xác định phiên bản cần thiết (VD: \"MSVCP140.dll not found\" → cần VC++ 2015-2022).\n2. Tải Visual C++ Redistributable Runtimes All-in-One từ GitHub (abbodi1406/vcredist).\n3. Cài đặt tất cả phiên bản (2005 đến 2022, cả x86 lẫn x64).\n4. Với .NET Framework:\n   - Tải từ: https://dotnet.microsoft.com/download/dotnet-framework\n   - Hoặc bật qua Windows Features: Turn Windows features on or off → .NET Framework 3.5.\n5. Khởi động lại máy và thử chạy lại phần mềm.", true },
                    { 23, 23, 3, "1. Ghi lại tên file gây lỗi hiện trên màn hình xanh (thường có đuôi .sys).\n2. Vào Device Manager, tìm driver liên quan đến file .sys đó.\n3. Cập nhật hoặc gỡ driver đó.\n4. Chạy sfc /scannow và DISM /Online /Cleanup-Image /RestoreHealth.\n5. Phân tích file dump: C:\\Windows\\Minidump\\ bằng công cụ WinDbg (Microsoft Store).\n6. Kiểm tra RAM bằng MemTest86.\n7. Thử gỡ phần mềm bảo mật (antivirus) và test thử.", true },
                    { 24, 24, 3, "1. Lỗi này thường do driver không tương thích. Khởi động vào Safe Mode.\n2. Mở Event Viewer (eventvwr.msc) → Windows Logs → System → tìm lỗi màu đỏ gần thời điểm crash.\n3. Cập nhật toàn bộ driver bằng Windows Update hoặc từ trang nhà sản xuất.\n4. Đặc biệt chú ý: driver mạng (Ethernet/WiFi), driver GPU, driver âm thanh.\n5. Gỡ bỏ phần mềm antivirus bên thứ ba và dùng Windows Defender thử.\n6. Kiểm tra Windows Update và cài đủ các bản vá.\n7. Phân tích minidump bằng WinDbg để xác định driver gây ra.", true },
                    { 25, 25, 3, "1. Boot từ USB cài Windows → Repair your computer → Startup Repair (thử tự động sửa).\n2. Nếu không được, vào Command Prompt từ Recovery:\n   sfc /scannow /offbootdir=C:\\ /offwindir=C:\\Windows\n   DISM /Image:C:\\ /Cleanup-Image /RestoreHealth /Source:esd:D:\\sources\\install.esd\n3. Thử System Restore về điểm trước khi lỗi.\n4. Kiểm tra file winlogon.exe và csrss.exe còn nguyên vẹn không (có thể bị virus thay thế).\n5. Nếu thất bại: cài lại Windows và giữ lại file cá nhân.", true },
                    { 26, 26, 2, "1. Chạy Windows Memory Diagnostic: nhấn Win+R, gõ mdsched.exe, chọn Restart and check.\n2. Chạy MemTest86 từ USB (để qua đêm cho chắc chắn).\n3. Nếu RAM lỗi: thay RAM mới.\n4. Kiểm tra ổ cứng: chkdsk /f /r C: (lập lịch kiểm tra khi restart).\n5. Cập nhật driver đồ họa.\n6. Kiểm tra nhiệt độ CPU và GPU (quá nhiệt cũng gây lỗi này).\n7. Nếu ổn định trong Safe Mode nhưng lỗi ở mode thường → lỗi driver/phần mềm.", true },
                    { 27, 27, 3, "1. Khởi động vào Safe Mode.\n2. Chạy sfc /scannow.\n3. Cập nhật hoặc gỡ driver mới nhất cài gần đây (đặc biệt GPU driver).\n4. Kiểm tra RAM: rút từng thanh, test riêng lẻ.\n5. Vô hiệu hóa tính năng overclocking nếu có (XMP/DOCP trong BIOS).\n6. Đặt pagefile về automatic: System Properties → Advanced → Performance Settings → Virtual Memory.\n7. Kiểm tra ổ cứng bằng chkdsk.\n8. Phân tích file dump bằng WinDbg.", true },
                    { 28, 28, 2, "1. Device Manager → Network Adapters → chuột phải WiFi adapter → Update Driver.\n2. Gỡ driver (Uninstall Device → tích \"Delete driver software\") → restart để Windows tự cài lại.\n3. Tải driver WiFi từ trang web nhà sản xuất laptop (theo đúng model máy và hệ điều hành).\n4. Kiểm tra xem WiFi có bị tắt bởi phím Fn không (thường là Fn+F2, F5, F12 tùy hãng).\n5. Vào Services.msc, tìm \"WLAN AutoConfig\" đảm bảo đang Running và Automatic.\n6. Chạy: netsh winsock reset catalog → netsh int ip reset → ipconfig /flushdns → restart.", true },
                    { 29, 29, 3, "1. Tắt máy, tháo nắp lưng laptop.\n2. Xác định WiFi card (thường nằm gần góc trên, có 2 dây anten màu trắng và đen/xám).\n3. Kiểm tra 2 đầu dây anten đã cắm chắc vào card chưa (nhấn nhẹ cho đến khi nghe tiếng click).\n4. Kiểm tra dây anten không bị đứt, cắt, hoặc kẹp bởi bản lề màn hình.\n5. Nếu dây đứt: thay dây anten WiFi mới (đặt trên Shopee/Lazada theo model máy).\n6. Nếu WiFi card hỏng: thay card WiFi mới (phổ biến: Intel AX210, AX200, MediaTek MT7921).", true },
                    { 30, 30, 1, "1. Mở Command Prompt quyền Admin, chạy: ipconfig /flushdns.\n2. Đổi DNS thủ công:\n   - Network Settings → Change adapter options → Chuột phải kết nối → Properties → IPv4.\n   - Đổi sang DNS của Google: 8.8.8.8 và 8.8.4.4.\n   - Hoặc Cloudflare: 1.1.1.1 và 1.0.0.1.\n3. Chạy: netsh int ip reset → netsh winsock reset → restart.\n4. Kiểm tra file hosts: mở C:\\Windows\\System32\\drivers\\etc\\hosts bằng Notepad (Admin), xóa dòng lạ.\n5. Tắt thử proxy nếu đang dùng (Settings → Proxy → tắt Use a proxy server).", true },
                    { 31, 31, 2, "1. Mở Command Prompt quyền Administrator.\n2. Chạy lần lượt:\n   netsh winsock reset\n   netsh int ip reset\n   ipconfig /release\n   ipconfig /flushdns\n   ipconfig /renew\n3. Restart máy.\n4. Nếu vẫn lỗi, chạy: netsh int ipv4 reset reset.log\n5. Kiểm tra xem phần mềm antivirus hoặc VPN có can thiệp vào kết nối mạng không.\n6. Thử tắt Windows Firewall tạm thời để test.", true },
                    { 32, 32, 2, "1. Device Manager → Network Adapters → chuột phải Ethernet adapter → Update Driver.\n2. Kiểm tra cáp LAN: thử cáp khác, thử cổng khác trên switch/router.\n3. Kiểm tra đèn LED ở cổng LAN: đèn xanh = có kết nối vật lý.\n4. Gỡ và cài lại driver Ethernet từ trang nhà sản xuất.\n5. Kiểm tra BIOS: một số máy có thể tắt LAN trong BIOS.\n6. Nếu chip LAN trên mainboard hỏng → dùng USB LAN adapter (Ethernet Adapter USB 3.0) làm giải pháp tạm.", true },
                    { 33, 33, 3, "⚠️ KHẨN CẤP - Tràn nước vào máy tính:\n1. TẮT NGUỒN NGAY LẬP TỨC (giữ nút nguồn 5 giây), KHÔNG shutdown bình thường.\n2. Rút dây sạc ra ngay.\n3. Lật ngược máy, dốc nước ra.\n4. Tháo pin nếu có thể.\n5. Dùng khăn khô lau xung quanh, KHÔNG dùng máy sấy tóc (nhiệt độ cao phá bo mạch).\n6. Để máy dốc ngược 24-48 tiếng trong môi trường khô ráo.\n7. Nếu có hạt hút ẩm silica gel → bỏ máy vào túi kín cùng hạt hút ẩm 48 giờ.\n8. SAU KHI KHÔ HOÀN TOÀN mới bật máy lên test.\n9. Nếu máy vẫn lỗi sau khi khô: mang ra trung tâm sửa chữa để vệ sinh bo mạch bằng siêu âm.", true },
                    { 34, 34, 2, "1. Kiểm tra phím tắt touchpad: thường là Fn+F6, Fn+F7, hoặc Fn+F9 tùy hãng máy.\n2. Settings → Bluetooth & devices → Touchpad → bật \"Touchpad\".\n3. Device Manager → Mice and other pointing devices → cập nhật driver touchpad.\n4. Gỡ driver (Uninstall Device) và restart để cài lại tự động.\n5. Tải driver touchpad chính hãng từ trang web nhà sản xuất laptop.\n6. Nếu dùng Synaptics/Elan touchpad: tải driver từ trang synaptics.com hoặc elan.com.\n7. Vào BIOS xem có tùy chọn Enable/Disable touchpad không.\n8. Nếu touchpad vẫn không phản hồi → có thể cáp kết nối touchpad bị lỏng hoặc touchpad hỏng vật lý.", true },
                    { 35, 35, 1, "1. Chuột phải vào biểu tượng loa góc dưới phải → Troubleshoot sound problems.\n2. Kiểm tra âm lượng: Sound Mixer (chuột phải biểu tượng loa → Open volume mixer).\n3. Device Manager → Sound, video and game controllers → cập nhật/cài lại driver âm thanh.\n4. Tải driver âm thanh từ trang nhà sản xuất (Realtek thường được dùng nhất).\n5. Vào Sound settings → kiểm tra Output device đúng chưa (không bị chuyển sang HDMI output).\n6. Chạy: msconfig → Services → bật Windows Audio và Windows Audio Endpoint Builder.\n7. Nếu dùng loa ngoài: kiểm tra cáp, thử cổng jack khác, test loa với thiết bị khác.", true },
                    { 36, 36, 2, "1. Thử thiết bị USB ở cổng USB khác trên máy.\n2. Thử thiết bị USB trên máy tính khác để xác nhận thiết bị còn hoạt động.\n3. Device Manager → Universal Serial Bus controllers → chuột phải từng USB Root Hub → Enable.\n4. Gỡ tất cả USB Root Hub (Uninstall), restart để Windows tự cài lại.\n5. Vào BIOS: tắt legacy USB support rồi bật lại.\n6. Chạy: sfc /scannow để kiểm tra file hệ thống liên quan USB.\n7. Tắt tính năng Power Management của USB: Device Manager → USB Root Hub → Properties → Power Management → bỏ tích \"Allow the computer to turn off this device to save power\".\n8. Nếu cổng USB bị hỏng vật lý: hàn lại hoặc thay cổng mới.", true },
                    { 37, 37, 1, "1. Tắt máy in, rút dây nguồn 30 giây, bật lại.\n2. Xóa hàng đợi in: Services.msc → Print Spooler → Stop → xóa nội dung C:\\Windows\\System32\\spool\\PRINTERS → Start lại Print Spooler.\n3. Gỡ driver máy in: Settings → Printers & scanners → chọn máy in → Remove device.\n4. Tải và cài driver máy in mới từ trang chủ nhà sản xuất (HP, Canon, Epson, Brother...).\n5. Thử kết nối USB trực tiếp thay vì qua WiFi (hoặc ngược lại).\n6. Kiểm tra mực in, giấy, tình trạng kẹt giấy.\n7. In test page: Printers & scanners → chọn máy in → Manage → Print a test page.", true }
                });

            migrationBuilder.InsertData(
                table: "TRIEU_CHUNG_LOI",
                columns: new[] { "MaLoi", "MaTC" },
                values: new object[,]
                {
                    { 21, 16 },
                    { 23, 18 },
                    { 24, 18 },
                    { 26, 18 },
                    { 27, 18 },
                    { 15, 24 },
                    { 17, 24 },
                    { 16, 25 },
                    { 17, 25 },
                    { 18, 27 },
                    { 22, 27 },
                    { 20, 28 },
                    { 21, 28 },
                    { 28, 31 },
                    { 29, 31 },
                    { 31, 33 },
                    { 32, 33 },
                    { 30, 34 },
                    { 31, 34 },
                    { 33, 36 },
                    { 34, 37 },
                    { 35, 38 },
                    { 36, 41 },
                    { 37, 42 },
                    { 10, 43 },
                    { 11, 43 },
                    { 12, 44 },
                    { 26, 44 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 6, 9 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 6, 10 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 10, 10 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 12, 10 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 15, 11 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 16, 11 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 7, 12 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 8, 12 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 9, 12 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 7, 13 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 10, 14 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 11, 14 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 12, 16 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 13, 16 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 21, 16 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 12, 18 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 23, 18 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 24, 18 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 26, 18 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 27, 18 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 15, 24 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 17, 24 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 16, 25 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 17, 25 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 18, 27 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 22, 27 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 20, 28 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 21, 28 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 28, 31 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 29, 31 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 31, 33 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 32, 33 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 30, 34 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 31, 34 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 33, 36 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 34, 37 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 35, 38 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 36, 41 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 37, 42 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 10, 43 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 11, 43 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 12, 44 });

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG_LOI",
                keyColumns: new[] { "MaLoi", "MaTC" },
                keyValues: new object[] { 26, 44 });

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 44);

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 1,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Ngắt nguồn điện, tháo pin ra khỏi thiết bị.\nBước 2: Mở lẫy giữ thanh RAM, lấy thiết bị ra ngoài.\nBước 3: Dùng tẩy cao su làm sạch chân đồng tiếp xúc và chổi mềm quét khe cắm.\nBước 4: Lắp lại thanh RAM chắc chắn cho đến khi lẫy tự động khóa.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 2,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Khởi động hệ thống vào chế độ Safe Mode.\nBước 2: Gỡ cài đặt các phần mềm hoặc bản cập nhật trình điều khiển vừa được nạp.\nBước 3: Mở Command Prompt với quyền Quản trị (Administrator), thực thi lệnh: sfc /scannow để kiểm tra và khôi phục tệp hệ thống.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 3,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Sử dụng đồng hồ vạn năng (VOM) kiểm tra điện áp đầu ra của bộ cấp nguồn.\nBước 2: Thử nghiệm bằng một bộ nguồn/sạc tiêu chuẩn khác.\nBước 3: Nếu Mainboard không nhận nguồn, tiến hành đo kiểm tra các linh kiện Mosfet, IC nguồn đầu vào.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 4,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Tháo rời cụm tản nhiệt ra khỏi vi xử lý CPU/GPU.\nBước 2: Sử dụng cồn chuyên dụng lau sạch lớp keo tản nhiệt cũ đã bị khô cứng.\nBước 3: Vệ sinh kỹ các khe thoát gió trên lá đồng tản nhiệt và tra lớp keo tản nhiệt mới (Ví dụ: MX-4), sau đó siết chặt ốc cố định.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 5,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Tháo khung viền màn hình, tiếp cận giắc cắm cáp tín hiệu phía sau Panel.\nBước 2: Vệ sinh tiếp xúc và cắm lại giắc kết nối, đảm bảo dán băng keo cố định.\nBước 3: Dùng đồng hồ đo thông mạch, nếu đứt ngầm tại khu vực bản lề thì thay thế mới dây cáp tín hiệu.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 6,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Tách cụm bàn phím ra khỏi khung máy, sử dụng khí nén thổi sạch dị vật.\nBước 2: Tháo các phím bấm có hiện tượng kẹt cơ học để làm sạch cơ cấu xương phím.\nBước 3: Nếu chập mạch điện tử gây liệt dải phím, tiến hành thay thế cụm bàn phím phần cứng mới.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 7,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Mở Device Manager, gỡ bỏ (Uninstall) trình điều khiển âm thanh hiện tại.\nBước 2: Tải và cài đặt trình điều khiển tương thích từ trang chủ nhà sản xuất thiết bị.\nBước 3: Truy cập cấu hình Services (services.msc), tìm dịch vụ 'Windows Audio' và khởi động lại (Restart).", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 8,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Tháo lắp và làm sạch chân cắm tiếp xúc của card Wi-Fi trên bo mạch chủ.\nBước 2: Thực thi chuỗi lệnh đặt lại mạng trong Command Prompt: 'netsh winsock reset' và 'ipconfig /flushdns'.\nBước 3: Cập nhật lại mật khẩu hoặc thiết lập lại địa chỉ IP tĩnh/DHCP phù hợp với bộ định tuyến.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 9,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Kiểm tra ngoại quan cổng cắm RJ45, làm sạch các lá đồng tiếp xúc bên trong.\nBước 2: Sử dụng kìm bấm mạng cắt bỏ đầu cũ và bấm lại đầu RJ45 mới theo chuẩn T568B.\nBước 3: Đo kiểm tra thông mạch đường dây cáp từ vị trí máy trạm đến bộ chuyển mạch (Switch).", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 10,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Mở hộp thoại Run gõ 'services.msc', tìm dịch vụ 'Print Spooler' và chọn Stop.\nBước 2: Truy cập thư mục 'C:\\Windows\\System32\\spool\\PRINTERS' và xóa toàn bộ các tệp hàng đợi.\nBước 3: Khởi động lại (Start) dịch vụ Print Spooler và kiểm tra sự chắc chắn của cáp tín hiệu máy in.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 11,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Khởi chạy phần mềm chuyên dụng quét và loại bỏ mã độc (AdwCleaner, Malwarebytes).\nBước 2: Truy cập phần quản lý tiện ích mở rộng của trình duyệt, gỡ bỏ các mục không xác định.\nBước 3: Thực hiện thao tác đặt lại (Reset) toàn bộ cấu hình trình duyệt về trạng thái nguyên bản.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 12,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Kiểm tra ngoại quan để phát hiện các chân đồng bên trong cổng USB có bị gãy chạm hay không.\nBước 2: Đo điện áp cấp nguồn tại chân VCC, nếu mất nguồn 5V cần hàn nối hoặc thay thế cầu chì bảo vệ trên Mainboard.\nBước 3: Thử gỡ cài đặt mục 'USB Root Hub' trong Device Manager và cho máy quét lại phần cứng.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 13,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Mở vỏ máy, kiểm tra và cắm lại chắc chắn cáp dữ liệu SATA cùng cáp nguồn ổ cứng.\nBước 2: Khởi động từ USB cứu hộ (WinPE), sử dụng công cụ phân vùng để kiểm tra tình trạng sức khỏe (SMART) của ổ đĩa.\nBước 3: Xây dựng lại bản ghi khởi động bằng lệnh: 'bootrec /rebuildbcd' hoặc tiến hành thay thế ổ cứng nếu xuất hiện Bad Sector nặng.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 14,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Sử dụng màn hình ngoài kết nối qua cổng HDMI/VGA để xác định lỗi xuất phát từ Panel hay vi xử lý đồ họa.\nBước 2: Đo điện áp cấp cho bo mạch cao áp (Inverter) hoặc đường nguồn cấp cho dải LED nền.\nBước 3: Tiến hành thay thế bo mạch cao áp hoặc tháo rời Panel để thay mới dải đèn LED nền bên trong.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 15,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Sử dụng phần mềm chẩn đoán (Ví dụ: BatteryMon) kiểm tra mức độ hao mòn (Wear Level) của viên pin.\nBước 2: Cập nhật BIOS bo mạch chủ lên phiên bản mới nhất để loại trừ lỗi giao tiếp quản lý năng lượng.\nBước 3: Nếu pin bị phồng hoặc dung lượng lưu trữ cạn kiệt, ngắt kết nối và thay thế viên pin mới cùng thông số.", false });

            migrationBuilder.UpdateData(
                table: "GIAI_PHAP",
                keyColumn: "MaGP",
                keyValue: 16,
                columns: new[] { "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[] { null, "Bước 1: Ngắt hoàn toàn cáp nguồn hệ thống máy tính ra khỏi ổ cắm điện.\nBước 2: Xác định vị trí viên pin CMOS (thường là mã CR2032) trên bo mạch chủ, sử dụng lẫy nhựa gỡ pin ra ngoài.\nBước 3: Lắp đặt viên pin CMOS mới, khởi động máy và truy cập vào BIOS để thiết lập lại chính xác thông số ngày giờ cùng thứ tự khởi động (Boot Priority).", false });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 1,
                column: "TenLoi",
                value: "Chân tiếp xúc RAM bị lỏng hoặc bám bụi bẩn");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 2,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần mềm", "Xung đột trình điều khiển (Driver) hoặc lỗi tệp hệ thống" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 3,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần cứng", "Hỏng bộ nguồn (PSU) hoặc mạch IC nguồn trên Mainboard" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 4,
                column: "TenLoi",
                value: "Keo tản nhiệt CPU/GPU khô, hệ thống tản nhiệt bám bụi");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 5,
                column: "TenLoi",
                value: "Cáp tín hiệu màn hình (Cáp lụa) bị lỏng hoặc đứt ngầm");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 6,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần cứng", "Chập mạch bàn phím do ẩm ướt hoặc bụi kẹt dưới gầm phím" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 7,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần mềm", "Lỗi dịch vụ Windows Audio hoặc hỏng chip xử lý âm thanh" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 8,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Mạng", "Card mạng Wi-Fi bị lỏng hoặc cấu hình sai dải IP/DNS" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 9,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Mạng", "Cổng RJ45 bị oxy hóa chân đồng hoặc đứt cáp LAN ngầm" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 10,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Thiết bị ngoại vi", "Tràn bộ đệm Print Spooler hoặc mất kết nối cổng cáp in" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 11,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần mềm", "Hệ thống nhiễm mã độc quảng cáo (Adware/Malware)" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 12,
                column: "TenLoi",
                value: "Cổng kết nối USB mất nguồn cấp 5V hoặc gãy chân tiếp xúc");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 13,
                column: "TenLoi",
                value: "Lỗi phân vùng khởi động (MBR/GPT) hoặc cáp SATA lỏng");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 14,
                column: "TenLoi",
                value: "Hỏng mạch cao áp (Inverter) hoặc dải đèn nền LED màn hình");

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 15,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần cứng", "Cell pin bị chai hoàn toàn hoặc lỗi mạch sạc tích hợp" });

            migrationBuilder.UpdateData(
                table: "MA_LOI",
                keyColumn: "MaLoiID",
                keyValue: 16,
                columns: new[] { "PhanLoai", "TenLoi" },
                values: new object[] { "Phần cứng", "Hết dung lượng pin CMOS lưu trữ cấu hình phần cứng" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 1,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Máy đang hoạt động đột ngột xuất hiện màn hình xanh kèm mã lỗi và tự khởi động lại.", "Màn hình xanh chết chóc (BSOD)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 2,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Bật nút nguồn phát ra chuỗi âm thanh bíp dài/ngắn từ mainboard, màn hình không hiển thị.", "Kêu tít tít liên tục khi khởi động" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 3,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Nhấn nút nguồn nhưng hệ thống im lìm, không quay quạt, đèn báo nguồn và sạc không sáng.", "Bật nguồn hoàn toàn không lên" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 4,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Thời gian khởi động vào hệ điều hành quá lâu, mở các ứng dụng cơ bản thường xuyên bị Not Responding.", "Máy hoạt động cực kỳ chậm, đơ/treo ứng dụng" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 5,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Hệ thống quạt tản nhiệt rít mạnh liên tục, nhiệt độ vỏ máy cao bất thường gây sập nguồn đột ngột.", "Quạt kêu to, vỏ máy cực kỳ nóng rát" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 6,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Hình ảnh bị cắt ngang bởi các đường sọc màu cố định hoặc giật chớp, sai lệch màu sắc hiển thị.", "Màn hình xuất hiện sọc ngang/dọc hoặc nhiễu màu" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 7,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Một số phím bấm không phản hồi hoặc tự động gõ liên tục một chuỗi ký tự dù không thao tác.", "Bàn phím gõ không nhận hoặc tự động nhảy chữ" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 8,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Hệ thống báo nhận thiết bị âm thanh nhưng không có tiếng xuất ra ngoài, hoặc âm thanh bị vỡ, nhiễu.", "Loa hoàn toàn không phát ra tiếng hoặc bị rè" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 9,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Không tìm thấy danh sách mạng không dây xung quanh hoặc kết nối thành công nhưng báo No Internet.", "Không thể kết nối Wi-Fi (Báo chấm than vàng)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 10,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Đèn tín hiệu tại cổng RJ45 không sáng khi cắm cáp, biểu tượng mạng hiển thị dấu chéo đỏ.", "Cắm dây mạng LAN nhưng không nhận tín hiệu" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 11,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Lệnh in bị treo trong danh sách chờ (Queue), thông báo lỗi Print Spooler Service.", "Máy in không phản hồi lệnh in hoặc báo lỗi Spooler" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 12,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Trang chủ mặc định bị thay đổi, liên tục xuất hiện các cửa sổ pop-up quảng cáo không rõ nguồn gốc.", "Trình duyệt tự động bật tab quảng cáo lạ" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 13,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Vào được Windows hoạt động bình thường nhưng tắt ngang hoàn toàn sau 5 đến 10 phút.", "Máy tính tự động tắt nguồn sau vài phút sử dụng" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 14,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Cắm chuột, bàn phím hoặc USB lưu trữ vào cổng nhưng hệ thống không phát ra âm thanh nhận diện.", "Cổng USB không nhận thiết bị ngoại vi" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 15,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Dừng lại ở màn hình đen cảnh báo 'Reboot and Select proper Boot device' ngay khi bật máy.", "Không nhận ổ cứng khởi động (No Boot Device)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 16,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Nhìn trực diện thấy tối thui nhưng khi chiếu đèn flash hoặc nhìn nghiêng vẫn thấy nội dung mờ bên trong.", "Màn hình tối đen nhưng vẫn thấy bóng mờ mờ" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 17,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Biểu tượng pin hiển thị 'Plugged in, not charging', rút sạc ra máy lập tức mất nguồn.", "Pin laptop không sạc vào điện (Not charging)" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 18,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Thực hiện cuộc gọi trực tuyến hoặc ghi âm nhưng đầu dây bên kia không nghe thấy bất kỳ tín hiệu nào.", "Micro hoàn toàn không thu được âm thanh" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 19,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Con trỏ chuột không di chuyển khi thao tác trên vùng touchpad, các nút bấm trái/phải không tác dụng.", "Bàn di chuột (Touchpad) hoàn toàn bị liệt" });

            migrationBuilder.UpdateData(
                table: "TRIEU_CHUNG",
                keyColumn: "MaTC",
                keyValue: 20,
                columns: new[] { "MoTa", "TenTC" },
                values: new object[] { "Sau khi ngắt nguồn điện và bật lại, đồng hồ Windows bị trả về các năm cũ gây lỗi xác thực chứng chỉ web.", "Ngày giờ hệ thống liên tục bị sai lệch" });

            migrationBuilder.InsertData(
                table: "TRIEU_CHUNG_LOI",
                columns: new[] { "MaLoi", "MaTC" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 3, 3 },
                    { 2, 4 },
                    { 4, 5 },
                    { 5, 6 },
                    { 6, 7 },
                    { 7, 8 },
                    { 8, 9 },
                    { 9, 10 },
                    { 10, 11 },
                    { 11, 12 },
                    { 3, 13 },
                    { 4, 13 },
                    { 12, 14 },
                    { 13, 15 },
                    { 14, 16 },
                    { 15, 17 },
                    { 7, 18 },
                    { 2, 19 },
                    { 16, 20 }
                });
        }
    }
}
