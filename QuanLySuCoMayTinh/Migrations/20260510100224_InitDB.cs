using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KHACH_HANG",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KHACH_HANG", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "MA_LOI",
                columns: table => new
                {
                    MaLoiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhanLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MA_LOI", x => x.MaLoiID);
                });

            migrationBuilder.CreateTable(
                name: "TRIEU_CHUNG",
                columns: table => new
                {
                    MaTC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRIEU_CHUNG", x => x.MaTC);
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    USERID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password_Hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.USERID);
                });

            migrationBuilder.CreateTable(
                name: "THIET_BI",
                columns: table => new
                {
                    MaTB = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTB = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CauHinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaKH = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THIET_BI", x => x.MaTB);
                    table.ForeignKey(
                        name: "FK_THIET_BI_KHACH_HANG_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KHACH_HANG",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GIAI_PHAP",
                columns: table => new
                {
                    MaGP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoi = table.Column<int>(type: "int", nullable: false),
                    NoiDungStepByStep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MucDoKho = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GIAI_PHAP", x => x.MaGP);
                    table.ForeignKey(
                        name: "FK_GIAI_PHAP_MA_LOI_MaLoi",
                        column: x => x.MaLoi,
                        principalTable: "MA_LOI",
                        principalColumn: "MaLoiID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRIEU_CHUNG_LOI",
                columns: table => new
                {
                    MaTC = table.Column<int>(type: "int", nullable: false),
                    MaLoi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRIEU_CHUNG_LOI", x => new { x.MaTC, x.MaLoi });
                    table.ForeignKey(
                        name: "FK_TRIEU_CHUNG_LOI_MA_LOI_MaLoi",
                        column: x => x.MaLoi,
                        principalTable: "MA_LOI",
                        principalColumn: "MaLoiID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRIEU_CHUNG_LOI_TRIEU_CHUNG_MaTC",
                        column: x => x.MaTC,
                        principalTable: "TRIEU_CHUNG",
                        principalColumn: "MaTC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PHIEU_TIEP_NHAN",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTB = table.Column<int>(type: "int", nullable: false),
                    MaUser_KTV = table.Column<int>(type: "int", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTra_DK = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoi_CD = table.Column<int>(type: "int", nullable: true),
                    USERID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHIEU_TIEP_NHAN", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PHIEU_TIEP_NHAN_MA_LOI_MaLoi_CD",
                        column: x => x.MaLoi_CD,
                        principalTable: "MA_LOI",
                        principalColumn: "MaLoiID");
                    table.ForeignKey(
                        name: "FK_PHIEU_TIEP_NHAN_THIET_BI_MaTB",
                        column: x => x.MaTB,
                        principalTable: "THIET_BI",
                        principalColumn: "MaTB",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PHIEU_TIEP_NHAN_USER_MaUser_KTV",
                        column: x => x.MaUser_KTV,
                        principalTable: "USER",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PHIEU_TIEP_NHAN_USER_USERID",
                        column: x => x.USERID,
                        principalTable: "USER",
                        principalColumn: "USERID");
                });

            migrationBuilder.CreateTable(
                name: "CHI_TIET_SUA_CHUA",
                columns: table => new
                {
                    MaCT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    TenHangMuc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ChiPhi = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHI_TIET_SUA_CHUA", x => x.MaCT);
                    table.ForeignKey(
                        name: "FK_CHI_TIET_SUA_CHUA_PHIEU_TIEP_NHAN_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PHIEU_TIEP_NHAN",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MA_LOI",
                columns: new[] { "MaLoiID", "PhanLoai", "Status", "TenLoi" },
                values: new object[,]
                {
                    { 1, "Phần cứng", true, "Chân tiếp xúc RAM bị lỏng hoặc bám bụi bẩn" },
                    { 2, "Phần mềm", true, "Xung đột trình điều khiển (Driver) hoặc lỗi tệp hệ thống" },
                    { 3, "Phần cứng", true, "Hỏng bộ nguồn (PSU) hoặc mạch IC nguồn trên Mainboard" },
                    { 4, "Phần cứng", true, "Keo tản nhiệt CPU/GPU khô, hệ thống tản nhiệt bám bụi" },
                    { 5, "Phần cứng", true, "Cáp tín hiệu màn hình (Cáp lụa) bị lỏng hoặc đứt ngầm" },
                    { 6, "Phần cứng", true, "Chập mạch bàn phím do ẩm ướt hoặc bụi kẹt dưới gầm phím" },
                    { 7, "Phần mềm", true, "Lỗi dịch vụ Windows Audio hoặc hỏng chip xử lý âm thanh" },
                    { 8, "Mạng", true, "Card mạng Wi-Fi bị lỏng hoặc cấu hình sai dải IP/DNS" },
                    { 9, "Mạng", true, "Cổng RJ45 bị oxy hóa chân đồng hoặc đứt cáp LAN ngầm" },
                    { 10, "Thiết bị ngoại vi", true, "Tràn bộ đệm Print Spooler hoặc mất kết nối cổng cáp in" },
                    { 11, "Phần mềm", true, "Hệ thống nhiễm mã độc quảng cáo (Adware/Malware)" },
                    { 12, "Phần cứng", true, "Cổng kết nối USB mất nguồn cấp 5V hoặc gãy chân tiếp xúc" },
                    { 13, "Phần cứng", true, "Lỗi phân vùng khởi động (MBR/GPT) hoặc cáp SATA lỏng" },
                    { 14, "Phần cứng", true, "Hỏng mạch cao áp (Inverter) hoặc dải đèn nền LED màn hình" },
                    { 15, "Phần cứng", true, "Cell pin bị chai hoàn toàn hoặc lỗi mạch sạc tích hợp" },
                    { 16, "Phần cứng", true, "Hết dung lượng pin CMOS lưu trữ cấu hình phần cứng" }
                });

            migrationBuilder.InsertData(
                table: "TRIEU_CHUNG",
                columns: new[] { "MaTC", "MoTa", "TenTC" },
                values: new object[,]
                {
                    { 1, "Máy đang hoạt động đột ngột xuất hiện màn hình xanh kèm mã lỗi và tự khởi động lại.", "Màn hình xanh chết chóc (BSOD)" },
                    { 2, "Bật nút nguồn phát ra chuỗi âm thanh bíp dài/ngắn từ mainboard, màn hình không hiển thị.", "Kêu tít tít liên tục khi khởi động" },
                    { 3, "Nhấn nút nguồn nhưng hệ thống im lìm, không quay quạt, đèn báo nguồn và sạc không sáng.", "Bật nguồn hoàn toàn không lên" },
                    { 4, "Thời gian khởi động vào hệ điều hành quá lâu, mở các ứng dụng cơ bản thường xuyên bị Not Responding.", "Máy hoạt động cực kỳ chậm, đơ/treo ứng dụng" },
                    { 5, "Hệ thống quạt tản nhiệt rít mạnh liên tục, nhiệt độ vỏ máy cao bất thường gây sập nguồn đột ngột.", "Quạt kêu to, vỏ máy cực kỳ nóng rát" },
                    { 6, "Hình ảnh bị cắt ngang bởi các đường sọc màu cố định hoặc giật chớp, sai lệch màu sắc hiển thị.", "Màn hình xuất hiện sọc ngang/dọc hoặc nhiễu màu" },
                    { 7, "Một số phím bấm không phản hồi hoặc tự động gõ liên tục một chuỗi ký tự dù không thao tác.", "Bàn phím gõ không nhận hoặc tự động nhảy chữ" },
                    { 8, "Hệ thống báo nhận thiết bị âm thanh nhưng không có tiếng xuất ra ngoài, hoặc âm thanh bị vỡ, nhiễu.", "Loa hoàn toàn không phát ra tiếng hoặc bị rè" },
                    { 9, "Không tìm thấy danh sách mạng không dây xung quanh hoặc kết nối thành công nhưng báo No Internet.", "Không thể kết nối Wi-Fi (Báo chấm than vàng)" },
                    { 10, "Đèn tín hiệu tại cổng RJ45 không sáng khi cắm cáp, biểu tượng mạng hiển thị dấu chéo đỏ.", "Cắm dây mạng LAN nhưng không nhận tín hiệu" },
                    { 11, "Lệnh in bị treo trong danh sách chờ (Queue), thông báo lỗi Print Spooler Service.", "Máy in không phản hồi lệnh in hoặc báo lỗi Spooler" },
                    { 12, "Trang chủ mặc định bị thay đổi, liên tục xuất hiện các cửa sổ pop-up quảng cáo không rõ nguồn gốc.", "Trình duyệt tự động bật tab quảng cáo lạ" },
                    { 13, "Vào được Windows hoạt động bình thường nhưng tắt ngang hoàn toàn sau 5 đến 10 phút.", "Máy tính tự động tắt nguồn sau vài phút sử dụng" },
                    { 14, "Cắm chuột, bàn phím hoặc USB lưu trữ vào cổng nhưng hệ thống không phát ra âm thanh nhận diện.", "Cổng USB không nhận thiết bị ngoại vi" },
                    { 15, "Dừng lại ở màn hình đen cảnh báo 'Reboot and Select proper Boot device' ngay khi bật máy.", "Không nhận ổ cứng khởi động (No Boot Device)" },
                    { 16, "Nhìn trực diện thấy tối thui nhưng khi chiếu đèn flash hoặc nhìn nghiêng vẫn thấy nội dung mờ bên trong.", "Màn hình tối đen nhưng vẫn thấy bóng mờ mờ" },
                    { 17, "Biểu tượng pin hiển thị 'Plugged in, not charging', rút sạc ra máy lập tức mất nguồn.", "Pin laptop không sạc vào điện (Not charging)" },
                    { 18, "Thực hiện cuộc gọi trực tuyến hoặc ghi âm nhưng đầu dây bên kia không nghe thấy bất kỳ tín hiệu nào.", "Micro hoàn toàn không thu được âm thanh" },
                    { 19, "Con trỏ chuột không di chuyển khi thao tác trên vùng touchpad, các nút bấm trái/phải không tác dụng.", "Bàn di chuột (Touchpad) hoàn toàn bị liệt" },
                    { 20, "Sau khi ngắt nguồn điện và bật lại, đồng hồ Windows bị trả về các năm cũ gây lỗi xác thực chứng chỉ web.", "Ngày giờ hệ thống liên tục bị sai lệch" }
                });

            migrationBuilder.InsertData(
                table: "USER",
                columns: new[] { "USERID", "FullName", "Password_Hash", "Role", "Status", "Username" },
                values: new object[,]
                {
                    { 1, "Trương Tuấn Vĩnh", "123456", "Admin KTV", true, "tuanvinh" },
                    { 2, "Trần Văn B", "123456", "KTV", true, "vanb" }
                });

            migrationBuilder.InsertData(
                table: "GIAI_PHAP",
                columns: new[] { "MaGP", "MaLoi", "MucDoKho", "NoiDungStepByStep", "Status" },
                values: new object[,]
                {
                    { 1, 1, null, "Bước 1: Ngắt nguồn điện, tháo pin ra khỏi thiết bị.\nBước 2: Mở lẫy giữ thanh RAM, lấy thiết bị ra ngoài.\nBước 3: Dùng tẩy cao su làm sạch chân đồng tiếp xúc và chổi mềm quét khe cắm.\nBước 4: Lắp lại thanh RAM chắc chắn cho đến khi lẫy tự động khóa.", false },
                    { 2, 2, null, "Bước 1: Khởi động hệ thống vào chế độ Safe Mode.\nBước 2: Gỡ cài đặt các phần mềm hoặc bản cập nhật trình điều khiển vừa được nạp.\nBước 3: Mở Command Prompt với quyền Quản trị (Administrator), thực thi lệnh: sfc /scannow để kiểm tra và khôi phục tệp hệ thống.", false },
                    { 3, 3, null, "Bước 1: Sử dụng đồng hồ vạn năng (VOM) kiểm tra điện áp đầu ra của bộ cấp nguồn.\nBước 2: Thử nghiệm bằng một bộ nguồn/sạc tiêu chuẩn khác.\nBước 3: Nếu Mainboard không nhận nguồn, tiến hành đo kiểm tra các linh kiện Mosfet, IC nguồn đầu vào.", false },
                    { 4, 4, null, "Bước 1: Tháo rời cụm tản nhiệt ra khỏi vi xử lý CPU/GPU.\nBước 2: Sử dụng cồn chuyên dụng lau sạch lớp keo tản nhiệt cũ đã bị khô cứng.\nBước 3: Vệ sinh kỹ các khe thoát gió trên lá đồng tản nhiệt và tra lớp keo tản nhiệt mới (Ví dụ: MX-4), sau đó siết chặt ốc cố định.", false },
                    { 5, 5, null, "Bước 1: Tháo khung viền màn hình, tiếp cận giắc cắm cáp tín hiệu phía sau Panel.\nBước 2: Vệ sinh tiếp xúc và cắm lại giắc kết nối, đảm bảo dán băng keo cố định.\nBước 3: Dùng đồng hồ đo thông mạch, nếu đứt ngầm tại khu vực bản lề thì thay thế mới dây cáp tín hiệu.", false },
                    { 6, 6, null, "Bước 1: Tách cụm bàn phím ra khỏi khung máy, sử dụng khí nén thổi sạch dị vật.\nBước 2: Tháo các phím bấm có hiện tượng kẹt cơ học để làm sạch cơ cấu xương phím.\nBước 3: Nếu chập mạch điện tử gây liệt dải phím, tiến hành thay thế cụm bàn phím phần cứng mới.", false },
                    { 7, 7, null, "Bước 1: Mở Device Manager, gỡ bỏ (Uninstall) trình điều khiển âm thanh hiện tại.\nBước 2: Tải và cài đặt trình điều khiển tương thích từ trang chủ nhà sản xuất thiết bị.\nBước 3: Truy cập cấu hình Services (services.msc), tìm dịch vụ 'Windows Audio' và khởi động lại (Restart).", false },
                    { 8, 8, null, "Bước 1: Tháo lắp và làm sạch chân cắm tiếp xúc của card Wi-Fi trên bo mạch chủ.\nBước 2: Thực thi chuỗi lệnh đặt lại mạng trong Command Prompt: 'netsh winsock reset' và 'ipconfig /flushdns'.\nBước 3: Cập nhật lại mật khẩu hoặc thiết lập lại địa chỉ IP tĩnh/DHCP phù hợp với bộ định tuyến.", false },
                    { 9, 9, null, "Bước 1: Kiểm tra ngoại quan cổng cắm RJ45, làm sạch các lá đồng tiếp xúc bên trong.\nBước 2: Sử dụng kìm bấm mạng cắt bỏ đầu cũ và bấm lại đầu RJ45 mới theo chuẩn T568B.\nBước 3: Đo kiểm tra thông mạch đường dây cáp từ vị trí máy trạm đến bộ chuyển mạch (Switch).", false },
                    { 10, 10, null, "Bước 1: Mở hộp thoại Run gõ 'services.msc', tìm dịch vụ 'Print Spooler' và chọn Stop.\nBước 2: Truy cập thư mục 'C:\\Windows\\System32\\spool\\PRINTERS' và xóa toàn bộ các tệp hàng đợi.\nBước 3: Khởi động lại (Start) dịch vụ Print Spooler và kiểm tra sự chắc chắn của cáp tín hiệu máy in.", false },
                    { 11, 11, null, "Bước 1: Khởi chạy phần mềm chuyên dụng quét và loại bỏ mã độc (AdwCleaner, Malwarebytes).\nBước 2: Truy cập phần quản lý tiện ích mở rộng của trình duyệt, gỡ bỏ các mục không xác định.\nBước 3: Thực hiện thao tác đặt lại (Reset) toàn bộ cấu hình trình duyệt về trạng thái nguyên bản.", false },
                    { 12, 12, null, "Bước 1: Kiểm tra ngoại quan để phát hiện các chân đồng bên trong cổng USB có bị gãy chạm hay không.\nBước 2: Đo điện áp cấp nguồn tại chân VCC, nếu mất nguồn 5V cần hàn nối hoặc thay thế cầu chì bảo vệ trên Mainboard.\nBước 3: Thử gỡ cài đặt mục 'USB Root Hub' trong Device Manager và cho máy quét lại phần cứng.", false },
                    { 13, 13, null, "Bước 1: Mở vỏ máy, kiểm tra và cắm lại chắc chắn cáp dữ liệu SATA cùng cáp nguồn ổ cứng.\nBước 2: Khởi động từ USB cứu hộ (WinPE), sử dụng công cụ phân vùng để kiểm tra tình trạng sức khỏe (SMART) của ổ đĩa.\nBước 3: Xây dựng lại bản ghi khởi động bằng lệnh: 'bootrec /rebuildbcd' hoặc tiến hành thay thế ổ cứng nếu xuất hiện Bad Sector nặng.", false },
                    { 14, 14, null, "Bước 1: Sử dụng màn hình ngoài kết nối qua cổng HDMI/VGA để xác định lỗi xuất phát từ Panel hay vi xử lý đồ họa.\nBước 2: Đo điện áp cấp cho bo mạch cao áp (Inverter) hoặc đường nguồn cấp cho dải LED nền.\nBước 3: Tiến hành thay thế bo mạch cao áp hoặc tháo rời Panel để thay mới dải đèn LED nền bên trong.", false },
                    { 15, 15, null, "Bước 1: Sử dụng phần mềm chẩn đoán (Ví dụ: BatteryMon) kiểm tra mức độ hao mòn (Wear Level) của viên pin.\nBước 2: Cập nhật BIOS bo mạch chủ lên phiên bản mới nhất để loại trừ lỗi giao tiếp quản lý năng lượng.\nBước 3: Nếu pin bị phồng hoặc dung lượng lưu trữ cạn kiệt, ngắt kết nối và thay thế viên pin mới cùng thông số.", false },
                    { 16, 16, null, "Bước 1: Ngắt hoàn toàn cáp nguồn hệ thống máy tính ra khỏi ổ cắm điện.\nBước 2: Xác định vị trí viên pin CMOS (thường là mã CR2032) trên bo mạch chủ, sử dụng lẫy nhựa gỡ pin ra ngoài.\nBước 3: Lắp đặt viên pin CMOS mới, khởi động máy và truy cập vào BIOS để thiết lập lại chính xác thông số ngày giờ cùng thứ tự khởi động (Boot Priority).", false }
                });

            migrationBuilder.InsertData(
                table: "TRIEU_CHUNG_LOI",
                columns: new[] { "MaLoi", "MaTC" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 1, 2 },
                    { 3, 3 },
                    { 2, 4 },
                    { 4, 4 },
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

            migrationBuilder.CreateIndex(
                name: "IX_CHI_TIET_SUA_CHUA_MaPhieu",
                table: "CHI_TIET_SUA_CHUA",
                column: "MaPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_GIAI_PHAP_MaLoi",
                table: "GIAI_PHAP",
                column: "MaLoi");

            migrationBuilder.CreateIndex(
                name: "IX_KHACH_HANG_SDT",
                table: "KHACH_HANG",
                column: "SDT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_TIEP_NHAN_MaLoi_CD",
                table: "PHIEU_TIEP_NHAN",
                column: "MaLoi_CD");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_TIEP_NHAN_MaTB",
                table: "PHIEU_TIEP_NHAN",
                column: "MaTB");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_TIEP_NHAN_MaUser_KTV",
                table: "PHIEU_TIEP_NHAN",
                column: "MaUser_KTV");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_TIEP_NHAN_USERID",
                table: "PHIEU_TIEP_NHAN",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_THIET_BI_MaKH",
                table: "THIET_BI",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_TRIEU_CHUNG_LOI_MaLoi",
                table: "TRIEU_CHUNG_LOI",
                column: "MaLoi");

            migrationBuilder.CreateIndex(
                name: "IX_USER_Username",
                table: "USER",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHI_TIET_SUA_CHUA");

            migrationBuilder.DropTable(
                name: "GIAI_PHAP");

            migrationBuilder.DropTable(
                name: "TRIEU_CHUNG_LOI");

            migrationBuilder.DropTable(
                name: "PHIEU_TIEP_NHAN");

            migrationBuilder.DropTable(
                name: "TRIEU_CHUNG");

            migrationBuilder.DropTable(
                name: "MA_LOI");

            migrationBuilder.DropTable(
                name: "THIET_BI");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "KHACH_HANG");
        }
    }
}
