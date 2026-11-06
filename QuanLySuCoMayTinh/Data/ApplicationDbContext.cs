using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) { }

        // Khai báo toàn bộ 9 bảng dữ liệu gốc
        public DbSet<User> Users { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<ThietBi> ThietBis { get; set; }
        public DbSet<TrieuChung> TrieuChungs { get; set; }
        public DbSet<MaLoi> MaLois { get; set; }
        public DbSet<TrieuChungLoi> TrieuChungLois { get; set; }
        public DbSet<GiaiPhap> GiaiPhaps { get; set; }
        public DbSet<PhieuTiepNhan> PhieuTiepNhans { get; set; }
        public DbSet<ChiTietSuaChua> ChiTietSuaChuas { get; set; }

        // 5 bảng mở rộng - 3NF hoàn chỉnh
        public DbSet<LinhKien> LinhKiens { get; set; }
        public DbSet<SuDungLinhKien> SuDungLinhKiens { get; set; }
        public DbSet<HinhAnhSuCo> HinhAnhSuCos { get; set; }
        public DbSet<LichSuTrangThai> LichSuTrangThais { get; set; }
        public DbSet<DanhGiaDichVu> DanhGiaDichVus { get; set; }

        // Bảng dữ liệu AI Hybrid
        public DbSet<LichSuChanDoan> LichSuChanDoans { get; set; }
        public DbSet<LichSuChanDoanTrieuChung> LichSuChanDoanTrieuChungs { get; set; }
        public DbSet<MLModelInfo> MLModelInfos { get; set; }
        public DbSet<AITrainingJob> AITrainingJobs { get; set; }
        public DbSet<ModelActivationLog> ModelActivationLogs { get; set; }
        public DbSet<QuanLySuCoMayTinh.Models.ML.AITestCase> AITestCases { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ERD V2: preserve base CLR APIs; rename relational columns only.
            modelBuilder.Entity<User>().ToTable("NGUOI_DUNG");
            modelBuilder.Entity<User>().Property(x => x.USERID).HasColumnName("MaNguoiDung");
            modelBuilder.Entity<User>().Property(x => x.Username).HasColumnName("TenDangNhap");
            modelBuilder.Entity<User>().Property(x => x.Password_Hash).HasColumnName("MatKhauBam");
            modelBuilder.Entity<User>().Property(x => x.FullName).HasColumnName("HoTen");
            modelBuilder.Entity<User>().Property(x => x.Role).HasColumnName("VaiTro");
            modelBuilder.Entity<User>().Property(x => x.Status).HasColumnName("TrangThaiHoatDong");
            modelBuilder.Entity<KhachHang>().Property(x => x.MaKH).HasColumnName("MaKhachHang");
            modelBuilder.Entity<KhachHang>().Property(x => x.SDT).HasColumnName("SoDienThoai");
            modelBuilder.Entity<ThietBi>().Property(x => x.MaTB).HasColumnName("MaThietBi");
            modelBuilder.Entity<ThietBi>().Property(x => x.MaKH).HasColumnName("MaKhachHang");
            modelBuilder.Entity<ThietBi>().Property(x => x.TenTB).HasColumnName("TenThietBi");
            modelBuilder.Entity<TrieuChung>().Property(x => x.MaTC).HasColumnName("MaTrieuChung");
            modelBuilder.Entity<TrieuChung>().Property(x => x.TenTC).HasColumnName("TenTrieuChung");
            modelBuilder.Entity<MaLoi>().Property(x => x.MaLoiID).HasColumnName("MaLoi");
            modelBuilder.Entity<MaLoi>().Property(x => x.Status).HasColumnName("TrangThaiSuDung");
            modelBuilder.Entity<GiaiPhap>().Property(x => x.MaGP).HasColumnName("MaGiaiPhap");
            modelBuilder.Entity<GiaiPhap>().Property(x => x.NoiDungStepByStep).HasColumnName("NoiDungTungBuoc");
            modelBuilder.Entity<GiaiPhap>().Property(x => x.Status).HasColumnName("TrangThaiSuDung");
            modelBuilder.Entity<PhieuTiepNhan>().Property(x => x.MaTB).HasColumnName("MaThietBi");
            modelBuilder.Entity<PhieuTiepNhan>().Property(x => x.MaUser_KTV).HasColumnName("MaNguoiDung");
            modelBuilder.Entity<PhieuTiepNhan>().Property(x => x.MaLoi_CD).HasColumnName("MaLoi");
            modelBuilder.Entity<PhieuTiepNhan>().Property(x => x.NgayTra_DK).HasColumnName("NgayTraDuKien");
            modelBuilder.Entity<ChiTietSuaChua>().Property(x => x.MaCT).HasColumnName("MaChiTietSuaChua");
            modelBuilder.Entity<LinhKien>().Property(x => x.MaLK).HasColumnName("MaLinhKien");
            modelBuilder.Entity<LinhKien>().Property(x => x.TenLK).HasColumnName("TenLinhKien");
            modelBuilder.Entity<LinhKien>().Property(x => x.LoaiLK).HasColumnName("LoaiLinhKien");
            modelBuilder.Entity<LinhKien>().Property(x => x.CoTrangThai).HasColumnName("TrangThaiSuDung");
            modelBuilder.Entity<LichSuTrangThai>().Property(x => x.MaLS).HasColumnName("MaLichSuTrangThai");
            modelBuilder.Entity<LichSuTrangThai>().Property(x => x.Ma_KTV).HasColumnName("MaNguoiDung");
            modelBuilder.Entity<HinhAnhSuCo>().Property(x => x.MaHA).HasColumnName("MaHinhAnh");
            modelBuilder.Entity<DanhGiaDichVu>().Property(x => x.MaDG).HasColumnName("MaDanhGia");
            modelBuilder.Entity<DanhGiaDichVu>().Property(x => x.NgayDG).HasColumnName("NgayDanhGia");
            modelBuilder.Entity<TrieuChungLoi>().Property(x => x.MaTC).HasColumnName("MaTrieuChung");
            modelBuilder.Entity<SuDungLinhKien>().Property(x => x.MaCT).HasColumnName("MaChiTietSuaChua");
            modelBuilder.Entity<SuDungLinhKien>().Property(x => x.MaLK).HasColumnName("MaLinhKien");
            modelBuilder.Entity<LichSuChanDoanTrieuChung>().HasKey(x => new { x.MaLichSuChanDoan, x.MaTrieuChung });
            modelBuilder.Entity<MLModelInfo>().Property(x => x.LoaiBaiToan).HasDefaultValue("CHAN_DOAN_TRIEU_CHUNG_MA_LOI");
            modelBuilder.Entity<LichSuChanDoan>().Property(x => x.TrangThaiXacMinh).HasDefaultValue("Chưa xác minh");
            modelBuilder.Entity<LichSuChanDoan>().HasOne(x => x.NguoiDungXacMinh).WithMany().HasForeignKey(x => x.MaNguoiDungXacMinh).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichSuChanDoan>().HasOne(x => x.MoHinh).WithMany().HasForeignKey(x => x.MaMoHinh).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichSuChanDoan>().HasOne(x => x.LoiRule).WithMany().HasForeignKey(x => x.MaLoiRule).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichSuChanDoan>().HasOne(x => x.LoiAITop1).WithMany().HasForeignKey(x => x.MaLoiAITop1).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichSuChanDoan>().HasOne(x => x.LoiAITop2).WithMany().HasForeignKey(x => x.MaLoiAITop2).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichSuChanDoan>().HasOne(x => x.LoiAITop3).WithMany().HasForeignKey(x => x.MaLoiAITop3).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichSuChanDoan>().HasOne(x => x.LoiThucTe).WithMany().HasForeignKey(x => x.MaLoiThucTe).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AITrainingJob>().HasOne(x => x.NguoiDung).WithMany().HasForeignKey(x => x.MaNguoiDung).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelActivationLog>().HasOne(x => x.MoHinh).WithMany().HasForeignKey(x => x.MaMoHinh).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelActivationLog>().HasOne(x => x.NguoiDung).WithMany().HasForeignKey(x => x.MaNguoiDung).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<QuanLySuCoMayTinh.Models.ML.AITestCase>().HasOne(x => x.MoHinh).WithMany().HasForeignKey(x => x.MaMoHinh).OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // PHẦN 1: CẤU HÌNH RÀNG BUỘC (FLUENT API)
            // ==========================================

            // 1. Ràng buộc Unique
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<KhachHang>().HasIndex(k => k.SDT).IsUnique();

            // 2. Cấu hình Khóa chính kép cho bảng trung gian TRIEU_CHUNG_LOI
            modelBuilder.Entity<TrieuChungLoi>()
                .HasKey(tl => new { tl.MaTC, tl.MaLoi });

            modelBuilder.Entity<TrieuChungLoi>()
                .HasOne(tl => tl.TrieuChung)
                .WithMany(tc => tc.TrieuChungLois)
                .HasForeignKey(tl => tl.MaTC);

            modelBuilder.Entity<TrieuChungLoi>()
                .HasOne(tl => tl.MaLoiNavigation)
                .WithMany(ml => ml.TrieuChungLois)
                .HasForeignKey(tl => tl.MaLoi);

            // 3. Ép kiểu dữ liệu chuẩn xác cho tiền tệ (Tránh sai số kế toán)
            modelBuilder.Entity<ChiTietSuaChua>()
                .Property(c => c.ChiPhi)
                .HasColumnType("decimal(18,0)");

            // 4. Ngăn chặn Cascade Delete (Bảo toàn dữ liệu lịch sử quan trọng)
            modelBuilder.Entity<PhieuTiepNhan>()
                .HasOne(p => p.User_KTV)
                .WithMany(u => u.PhieuTiepNhans)
                .HasForeignKey(p => p.MaUser_KTV)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // PHẦN 1B: CẤU HÌNH 5 MODELS MỚI
            // ==========================================

            // 5. Composite Key cho bảng SU_DUNG_LINH_KIEN
            modelBuilder.Entity<SuDungLinhKien>()
                .HasKey(s => new { s.MaCT, s.MaLK });

            modelBuilder.Entity<SuDungLinhKien>()
                .Property(s => s.DonGiaXuat)
                .HasColumnType("decimal(18,0)");

            modelBuilder.Entity<SuDungLinhKien>()
                .HasOne(s => s.ChiTietSuaChua)
                .WithMany(c => c.SuDungLinhKiens)
                .HasForeignKey(s => s.MaCT)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SuDungLinhKien>()
                .HasOne(s => s.LinhKien)
                .WithMany(lk => lk.SuDungLinhKiens)
                .HasForeignKey(s => s.MaLK)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa linh kiện nếu đã dùng

            // 6. Quan hệ 1:1 giữa DanhGiaDichVu và PhieuTiepNhan
            modelBuilder.Entity<DanhGiaDichVu>()
                .HasOne(d => d.PhieuTiepNhan)
                .WithOne(p => p.DanhGiaDichVu)
                .HasForeignKey<DanhGiaDichVu>(d => d.MaPhieu)
                .OnDelete(DeleteBehavior.Cascade);

            // Đảm bảo mỗi phiếu chỉ có 1 đánh giá duy nhất (unique constraint)
            modelBuilder.Entity<DanhGiaDichVu>()
                .HasIndex(d => d.MaPhieu)
                .IsUnique();

            // 7. LichSuTrangThai - cascade khi xóa phiếu
            modelBuilder.Entity<LichSuTrangThai>()
                .HasOne(ls => ls.PhieuTiepNhan)
                .WithMany(p => p.LichSuTrangThais)
                .HasForeignKey(ls => ls.MaPhieu)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LichSuTrangThai>()
                .HasOne(ls => ls.KTV)
                .WithMany()
                .HasForeignKey(ls => ls.Ma_KTV)
                .OnDelete(DeleteBehavior.SetNull);

            // 8. HinhAnhSuCo - cascade khi xóa phiếu
            modelBuilder.Entity<HinhAnhSuCo>()
                .HasOne(h => h.PhieuTiepNhan)
                .WithMany(p => p.HinhAnhSuCos)
                .HasForeignKey(h => h.MaPhieu)
                .OnDelete(DeleteBehavior.Cascade);

            // 9. Decimal type cho LinhKien.DonGia
            modelBuilder.Entity<LinhKien>()
                .Property(lk => lk.DonGia)
                .HasColumnType("decimal(18,0)");


            // ==========================================
            // PHẦN 2: BƠM DỮ LIỆU MỒI (SEED DATA)
            // ==========================================

            // 1. DỮ LIỆU USER (Tài khoản KTV)
            modelBuilder.Entity<User>().HasData(
                new User { 
                    USERID = 1, 
                    Username = "tuanvinh", 
                    FullName = "Trương Tuấn Vĩnh", 
                    Role = "Admin KTV",
                    Password_Hash = "123456",
                    Status = true
                },
                new User { 
                    USERID = 2, 
                    Username = "vanb", 
                    FullName = "Trần Văn B", 
                    Role = "KTV",
                    Password_Hash = "123456",
                    Status = true
                }
            );

            // Ghi chú: Đã lược bỏ dữ liệu mồi của KhachHang và ThietBi theo yêu cầu thiết kế mới.

            // 2. DỮ LIỆU TỪ ĐIỂN TRIỆU CHỨNG (20 Triệu chứng chuyên sâu)
            modelBuilder.Entity<TrieuChung>().HasData(
                new TrieuChung { MaTC = 1, TenTC = "Màn hình xanh chết chóc (BSOD)", MoTa = "Máy đang hoạt động đột ngột xuất hiện màn hình xanh kèm mã lỗi và tự khởi động lại." },
                new TrieuChung { MaTC = 2, TenTC = "Kêu tít tít liên tục khi khởi động", MoTa = "Bật nút nguồn phát ra chuỗi âm thanh bíp dài/ngắn từ mainboard, màn hình không hiển thị." },
                new TrieuChung { MaTC = 3, TenTC = "Bật nguồn hoàn toàn không lên", MoTa = "Nhấn nút nguồn nhưng hệ thống im lìm, không quay quạt, đèn báo nguồn và sạc không sáng." },
                new TrieuChung { MaTC = 4, TenTC = "Máy hoạt động cực kỳ chậm, đơ/treo ứng dụng", MoTa = "Thời gian khởi động vào hệ điều hành quá lâu, mở các ứng dụng cơ bản thường xuyên bị Not Responding." },
                new TrieuChung { MaTC = 5, TenTC = "Quạt kêu to, vỏ máy cực kỳ nóng rát", MoTa = "Hệ thống quạt tản nhiệt rít mạnh liên tục, nhiệt độ vỏ máy cao bất thường gây sập nguồn đột ngột." },
                new TrieuChung { MaTC = 6, TenTC = "Màn hình xuất hiện sọc ngang/dọc hoặc nhiễu màu", MoTa = "Hình ảnh bị cắt ngang bởi các đường sọc màu cố định hoặc giật chớp, sai lệch màu sắc hiển thị." },
                new TrieuChung { MaTC = 7, TenTC = "Bàn phím gõ không nhận hoặc tự động nhảy chữ", MoTa = "Một số phím bấm không phản hồi hoặc tự động gõ liên tục một chuỗi ký tự dù không thao tác." },
                new TrieuChung { MaTC = 8, TenTC = "Loa hoàn toàn không phát ra tiếng hoặc bị rè", MoTa = "Hệ thống báo nhận thiết bị âm thanh nhưng không có tiếng xuất ra ngoài, hoặc âm thanh bị vỡ, nhiễu." },
                new TrieuChung { MaTC = 9, TenTC = "Không thể kết nối Wi-Fi (Báo chấm than vàng)", MoTa = "Không tìm thấy danh sách mạng không dây xung quanh hoặc kết nối thành công nhưng báo No Internet." },
                new TrieuChung { MaTC = 10, TenTC = "Cắm dây mạng LAN nhưng không nhận tín hiệu", MoTa = "Đèn tín hiệu tại cổng RJ45 không sáng khi cắm cáp, biểu tượng mạng hiển thị dấu chéo đỏ." },
                new TrieuChung { MaTC = 11, TenTC = "Máy in không phản hồi lệnh in hoặc báo lỗi Spooler", MoTa = "Lệnh in bị treo trong danh sách chờ (Queue), thông báo lỗi Print Spooler Service." },
                new TrieuChung { MaTC = 12, TenTC = "Trình duyệt tự động bật tab quảng cáo lạ", MoTa = "Trang chủ mặc định bị thay đổi, liên tục xuất hiện các cửa sổ pop-up quảng cáo không rõ nguồn gốc." },
                new TrieuChung { MaTC = 13, TenTC = "Máy tính tự động tắt nguồn sau vài phút sử dụng", MoTa = "Vào được Windows hoạt động bình thường nhưng tắt ngang hoàn toàn sau 5 đến 10 phút." },
                new TrieuChung { MaTC = 14, TenTC = "Cổng USB không nhận thiết bị ngoại vi", MoTa = "Cắm chuột, bàn phím hoặc USB lưu trữ vào cổng nhưng hệ thống không phát ra âm thanh nhận diện." },
                new TrieuChung { MaTC = 15, TenTC = "Không nhận ổ cứng khởi động (No Boot Device)", MoTa = "Dừng lại ở màn hình đen cảnh báo 'Reboot and Select proper Boot device' ngay khi bật máy." },
                new TrieuChung { MaTC = 16, TenTC = "Màn hình tối đen nhưng vẫn thấy bóng mờ mờ", MoTa = "Nhìn trực diện thấy tối thui nhưng khi chiếu đèn flash hoặc nhìn nghiêng vẫn thấy nội dung mờ bên trong." },
                new TrieuChung { MaTC = 17, TenTC = "Pin laptop không sạc vào điện (Not charging)", MoTa = "Biểu tượng pin hiển thị 'Plugged in, not charging', rút sạc ra máy lập tức mất nguồn." },
                new TrieuChung { MaTC = 18, TenTC = "Micro hoàn toàn không thu được âm thanh", MoTa = "Thực hiện cuộc gọi trực tuyến hoặc ghi âm nhưng đầu dây bên kia không nghe thấy bất kỳ tín hiệu nào." },
                new TrieuChung { MaTC = 19, TenTC = "Bàn di chuột (Touchpad) hoàn toàn bị liệt", MoTa = "Con trỏ chuột không di chuyển khi thao tác trên vùng touchpad, các nút bấm trái/phải không tác dụng." },
                new TrieuChung { MaTC = 20, TenTC = "Ngày giờ hệ thống liên tục bị sai lệch", MoTa = "Sau khi ngắt nguồn điện và bật lại, đồng hồ Windows bị trả về các năm cũ gây lỗi xác thực chứng chỉ web." }
            );

            // 3. DỮ LIỆU MÃ LỖI (16 Chuẩn bệnh gốc)
            modelBuilder.Entity<MaLoi>().HasData(
                new MaLoi { MaLoiID = 1, TenLoi = "Chân tiếp xúc RAM bị lỏng hoặc bám bụi bẩn", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 2, TenLoi = "Xung đột trình điều khiển (Driver) hoặc lỗi tệp hệ thống", PhanLoai = "Phần mềm", Status = true },
                new MaLoi { MaLoiID = 3, TenLoi = "Hỏng bộ nguồn (PSU) hoặc mạch IC nguồn trên Mainboard", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 4, TenLoi = "Keo tản nhiệt CPU/GPU khô, hệ thống tản nhiệt bám bụi", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 5, TenLoi = "Cáp tín hiệu màn hình (Cáp lụa) bị lỏng hoặc đứt ngầm", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 6, TenLoi = "Chập mạch bàn phím do ẩm ướt hoặc bụi kẹt dưới gầm phím", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 7, TenLoi = "Lỗi dịch vụ Windows Audio hoặc hỏng chip xử lý âm thanh", PhanLoai = "Phần mềm", Status = true },
                new MaLoi { MaLoiID = 8, TenLoi = "Card mạng Wi-Fi bị lỏng hoặc cấu hình sai dải IP/DNS", PhanLoai = "Mạng", Status = true },
                new MaLoi { MaLoiID = 9, TenLoi = "Cổng RJ45 bị oxy hóa chân đồng hoặc đứt cáp LAN ngầm", PhanLoai = "Mạng", Status = true },
                new MaLoi { MaLoiID = 10, TenLoi = "Tràn bộ đệm Print Spooler hoặc mất kết nối cổng cáp in", PhanLoai = "Thiết bị ngoại vi", Status = true },
                new MaLoi { MaLoiID = 11, TenLoi = "Hệ thống nhiễm mã độc quảng cáo (Adware/Malware)", PhanLoai = "Phần mềm", Status = true },
                new MaLoi { MaLoiID = 12, TenLoi = "Cổng kết nối USB mất nguồn cấp 5V hoặc gãy chân tiếp xúc", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 13, TenLoi = "Lỗi phân vùng khởi động (MBR/GPT) hoặc cáp SATA lỏng", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 14, TenLoi = "Hỏng mạch cao áp (Inverter) hoặc dải đèn nền LED màn hình", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 15, TenLoi = "Cell pin bị chai hoàn toàn hoặc lỗi mạch sạc tích hợp", PhanLoai = "Phần cứng", Status = true },
                new MaLoi { MaLoiID = 16, TenLoi = "Hết dung lượng pin CMOS lưu trữ cấu hình phần cứng", PhanLoai = "Phần cứng", Status = true }
            );

            // 4. LIÊN KẾT TRIỆU CHỨNG -> MÃ LỖI (Tập luật ánh xạ cho AI suy luận)
            modelBuilder.Entity<TrieuChungLoi>().HasData(
                new TrieuChungLoi { MaTC = 1, MaLoi = 1 },
                new TrieuChungLoi { MaTC = 1, MaLoi = 2 },
                new TrieuChungLoi { MaTC = 2, MaLoi = 1 },
                new TrieuChungLoi { MaTC = 3, MaLoi = 3 },
                new TrieuChungLoi { MaTC = 4, MaLoi = 2 },
                new TrieuChungLoi { MaTC = 4, MaLoi = 4 },
                new TrieuChungLoi { MaTC = 5, MaLoi = 4 },
                new TrieuChungLoi { MaTC = 6, MaLoi = 5 },
                new TrieuChungLoi { MaTC = 7, MaLoi = 6 },
                new TrieuChungLoi { MaTC = 8, MaLoi = 7 },
                new TrieuChungLoi { MaTC = 9, MaLoi = 8 },
                new TrieuChungLoi { MaTC = 10, MaLoi = 9 },
                new TrieuChungLoi { MaTC = 11, MaLoi = 10 },
                new TrieuChungLoi { MaTC = 12, MaLoi = 11 },
                new TrieuChungLoi { MaTC = 13, MaLoi = 4 },
                new TrieuChungLoi { MaTC = 13, MaLoi = 3 },
                new TrieuChungLoi { MaTC = 14, MaLoi = 12 },
                new TrieuChungLoi { MaTC = 15, MaLoi = 13 },
                new TrieuChungLoi { MaTC = 16, MaLoi = 14 },
                new TrieuChungLoi { MaTC = 17, MaLoi = 15 },
                new TrieuChungLoi { MaTC = 18, MaLoi = 7 },
                new TrieuChungLoi { MaTC = 19, MaLoi = 2 },
                new TrieuChungLoi { MaTC = 20, MaLoi = 16 }
            );

            // 5. DỮ LIỆU GIẢI PHÁP (16 Toa thuốc tương ứng với Mã lỗi)
            modelBuilder.Entity<GiaiPhap>().HasData(
                new GiaiPhap { 
                    MaGP = 1, 
                    MaLoi = 1, 
                    NoiDungStepByStep = "Bước 1: Ngắt nguồn điện, tháo pin ra khỏi thiết bị.\nBước 2: Mở lẫy giữ thanh RAM, lấy thiết bị ra ngoài.\nBước 3: Dùng tẩy cao su làm sạch chân đồng tiếp xúc và chổi mềm quét khe cắm.\nBước 4: Lắp lại thanh RAM chắc chắn cho đến khi lẫy tự động khóa." 
                },
                new GiaiPhap { 
                    MaGP = 2, 
                    MaLoi = 2, 
                    NoiDungStepByStep = "Bước 1: Khởi động hệ thống vào chế độ Safe Mode.\nBước 2: Gỡ cài đặt các phần mềm hoặc bản cập nhật trình điều khiển vừa được nạp.\nBước 3: Mở Command Prompt với quyền Quản trị (Administrator), thực thi lệnh: sfc /scannow để kiểm tra và khôi phục tệp hệ thống." 
                },
                new GiaiPhap { 
                    MaGP = 3, 
                    MaLoi = 3, 
                    NoiDungStepByStep = "Bước 1: Sử dụng đồng hồ vạn năng (VOM) kiểm tra điện áp đầu ra của bộ cấp nguồn.\nBước 2: Thử nghiệm bằng một bộ nguồn/sạc tiêu chuẩn khác.\nBước 3: Nếu Mainboard không nhận nguồn, tiến hành đo kiểm tra các linh kiện Mosfet, IC nguồn đầu vào." 
                },
                new GiaiPhap { 
                    MaGP = 4, 
                    MaLoi = 4, 
                    NoiDungStepByStep = "Bước 1: Tháo rời cụm tản nhiệt ra khỏi vi xử lý CPU/GPU.\nBước 2: Sử dụng cồn chuyên dụng lau sạch lớp keo tản nhiệt cũ đã bị khô cứng.\nBước 3: Vệ sinh kỹ các khe thoát gió trên lá đồng tản nhiệt và tra lớp keo tản nhiệt mới (Ví dụ: MX-4), sau đó siết chặt ốc cố định." 
                },
                new GiaiPhap { 
                    MaGP = 5, 
                    MaLoi = 5, 
                    NoiDungStepByStep = "Bước 1: Tháo khung viền màn hình, tiếp cận giắc cắm cáp tín hiệu phía sau Panel.\nBước 2: Vệ sinh tiếp xúc và cắm lại giắc kết nối, đảm bảo dán băng keo cố định.\nBước 3: Dùng đồng hồ đo thông mạch, nếu đứt ngầm tại khu vực bản lề thì thay thế mới dây cáp tín hiệu." 
                },
                new GiaiPhap { 
                    MaGP = 6, 
                    MaLoi = 6, 
                    NoiDungStepByStep = "Bước 1: Tách cụm bàn phím ra khỏi khung máy, sử dụng khí nén thổi sạch dị vật.\nBước 2: Tháo các phím bấm có hiện tượng kẹt cơ học để làm sạch cơ cấu xương phím.\nBước 3: Nếu chập mạch điện tử gây liệt dải phím, tiến hành thay thế cụm bàn phím phần cứng mới." 
                },
                new GiaiPhap { 
                    MaGP = 7, 
                    MaLoi = 7, 
                    NoiDungStepByStep = "Bước 1: Mở Device Manager, gỡ bỏ (Uninstall) trình điều khiển âm thanh hiện tại.\nBước 2: Tải và cài đặt trình điều khiển tương thích từ trang chủ nhà sản xuất thiết bị.\nBước 3: Truy cập cấu hình Services (services.msc), tìm dịch vụ 'Windows Audio' và khởi động lại (Restart)." 
                },
                new GiaiPhap { 
                    MaGP = 8, 
                    MaLoi = 8, 
                    NoiDungStepByStep = "Bước 1: Tháo lắp và làm sạch chân cắm tiếp xúc của card Wi-Fi trên bo mạch chủ.\nBước 2: Thực thi chuỗi lệnh đặt lại mạng trong Command Prompt: 'netsh winsock reset' và 'ipconfig /flushdns'.\nBước 3: Cập nhật lại mật khẩu hoặc thiết lập lại địa chỉ IP tĩnh/DHCP phù hợp với bộ định tuyến." 
                },
                new GiaiPhap { 
                    MaGP = 9, 
                    MaLoi = 9, 
                    NoiDungStepByStep = "Bước 1: Kiểm tra ngoại quan cổng cắm RJ45, làm sạch các lá đồng tiếp xúc bên trong.\nBước 2: Sử dụng kìm bấm mạng cắt bỏ đầu cũ và bấm lại đầu RJ45 mới theo chuẩn T568B.\nBước 3: Đo kiểm tra thông mạch đường dây cáp từ vị trí máy trạm đến bộ chuyển mạch (Switch)." 
                },
                new GiaiPhap { 
                    MaGP = 10, 
                    MaLoi = 10, 
                    NoiDungStepByStep = "Bước 1: Mở hộp thoại Run gõ 'services.msc', tìm dịch vụ 'Print Spooler' và chọn Stop.\nBước 2: Truy cập thư mục 'C:\\Windows\\System32\\spool\\PRINTERS' và xóa toàn bộ các tệp hàng đợi.\nBước 3: Khởi động lại (Start) dịch vụ Print Spooler và kiểm tra sự chắc chắn của cáp tín hiệu máy in." 
                },
                new GiaiPhap { 
                    MaGP = 11, 
                    MaLoi = 11, 
                    NoiDungStepByStep = "Bước 1: Khởi chạy phần mềm chuyên dụng quét và loại bỏ mã độc (AdwCleaner, Malwarebytes).\nBước 2: Truy cập phần quản lý tiện ích mở rộng của trình duyệt, gỡ bỏ các mục không xác định.\nBước 3: Thực hiện thao tác đặt lại (Reset) toàn bộ cấu hình trình duyệt về trạng thái nguyên bản." 
                },
                new GiaiPhap { 
                    MaGP = 12, 
                    MaLoi = 12, 
                    NoiDungStepByStep = "Bước 1: Kiểm tra ngoại quan để phát hiện các chân đồng bên trong cổng USB có bị gãy chạm hay không.\nBước 2: Đo điện áp cấp nguồn tại chân VCC, nếu mất nguồn 5V cần hàn nối hoặc thay thế cầu chì bảo vệ trên Mainboard.\nBước 3: Thử gỡ cài đặt mục 'USB Root Hub' trong Device Manager và cho máy quét lại phần cứng." 
                },
                new GiaiPhap { 
                    MaGP = 13, 
                    MaLoi = 13, 
                    NoiDungStepByStep = "Bước 1: Mở vỏ máy, kiểm tra và cắm lại chắc chắn cáp dữ liệu SATA cùng cáp nguồn ổ cứng.\nBước 2: Khởi động từ USB cứu hộ (WinPE), sử dụng công cụ phân vùng để kiểm tra tình trạng sức khỏe (SMART) của ổ đĩa.\nBước 3: Xây dựng lại bản ghi khởi động bằng lệnh: 'bootrec /rebuildbcd' hoặc tiến hành thay thế ổ cứng nếu xuất hiện Bad Sector nặng." 
                },
                new GiaiPhap { 
                    MaGP = 14, 
                    MaLoi = 14, 
                    NoiDungStepByStep = "Bước 1: Sử dụng màn hình ngoài kết nối qua cổng HDMI/VGA để xác định lỗi xuất phát từ Panel hay vi xử lý đồ họa.\nBước 2: Đo điện áp cấp cho bo mạch cao áp (Inverter) hoặc đường nguồn cấp cho dải LED nền.\nBước 3: Tiến hành thay thế bo mạch cao áp hoặc tháo rời Panel để thay mới dải đèn LED nền bên trong." 
                },
                new GiaiPhap { 
                    MaGP = 15, 
                    MaLoi = 15, 
                    NoiDungStepByStep = "Bước 1: Sử dụng phần mềm chẩn đoán (Ví dụ: BatteryMon) kiểm tra mức độ hao mòn (Wear Level) của viên pin.\nBước 2: Cập nhật BIOS bo mạch chủ lên phiên bản mới nhất để loại trừ lỗi giao tiếp quản lý năng lượng.\nBước 3: Nếu pin bị phồng hoặc dung lượng lưu trữ cạn kiệt, ngắt kết nối và thay thế viên pin mới cùng thông số." 
                },
                new GiaiPhap { 
                    MaGP = 16, 
                    MaLoi = 16, 
                    NoiDungStepByStep = "Bước 1: Ngắt hoàn toàn cáp nguồn hệ thống máy tính ra khỏi ổ cắm điện.\nBước 2: Xác định vị trí viên pin CMOS (thường là mã CR2032) trên bo mạch chủ, sử dụng lẫy nhựa gỡ pin ra ngoài.\nBước 3: Lắp đặt viên pin CMOS mới, khởi động máy và truy cập vào BIOS để thiết lập lại chính xác thông số ngày giờ cùng thứ tự khởi động (Boot Priority)." 
                }
            );

            // 6. DỮ LIỆU LINH KIỆN (Kho vật tư)
            modelBuilder.Entity<LinhKien>().HasData(
                new LinhKien { MaLK = 1, TenLK = "RAM Corsair Vengeance LPX 8GB DDR4 3200MHz", LoaiLK = "RAM", SoLuongTon = 50, DonGia = 650000, CoTrangThai = true },
                new LinhKien { MaLK = 2, TenLK = "RAM Kingston Fury Beast 16GB DDR4 3200MHz", LoaiLK = "RAM", SoLuongTon = 30, DonGia = 1150000, CoTrangThai = true },
                new LinhKien { MaLK = 3, TenLK = "SSD Samsung 970 EVO Plus 500GB NVMe", LoaiLK = "SSD", SoLuongTon = 25, DonGia = 1450000, CoTrangThai = true },
                new LinhKien { MaLK = 4, TenLK = "SSD Kingston NV2 250GB PCIe 4.0 NVMe", LoaiLK = "SSD", SoLuongTon = 40, DonGia = 850000, CoTrangThai = true },
                new LinhKien { MaLK = 5, TenLK = "HDD WD Blue 1TB 3.5\" SATA 3", LoaiLK = "HDD", SoLuongTon = 20, DonGia = 1100000, CoTrangThai = true },
                new LinhKien { MaLK = 6, TenLK = "Nguồn Corsair CV550 550W 80 Plus Bronze", LoaiLK = "Nguồn", SoLuongTon = 15, DonGia = 1250000, CoTrangThai = true },
                new LinhKien { MaLK = 7, TenLK = "Màn hình Dell UltraSharp U2422H 24\" IPS", LoaiLK = "Màn hình", SoLuongTon = 10, DonGia = 6200000, CoTrangThai = true },
                new LinhKien { MaLK = 8, TenLK = "Pin Laptop Dell Latitude E5470 (Zin)", LoaiLK = "Pin", SoLuongTon = 10, DonGia = 850000, CoTrangThai = true },
                new LinhKien { MaLK = 9, TenLK = "Sạc Laptop Dell 65W Kim to", LoaiLK = "Sạc", SoLuongTon = 30, DonGia = 350000, CoTrangThai = true },
                new LinhKien { MaLK = 10, TenLK = "Bàn phím Laptop Dell Inspiron 15 3000", LoaiLK = "Bàn phím", SoLuongTon = 15, DonGia = 450000, CoTrangThai = true },
                new LinhKien { MaLK = 11, TenLK = "Mainboard ASUS PRIME H610M-K D4", LoaiLK = "Mainboard", SoLuongTon = 10, DonGia = 1950000, CoTrangThai = true },
                new LinhKien { MaLK = 12, TenLK = "CPU Intel Core i3-12100F", LoaiLK = "CPU", SoLuongTon = 10, DonGia = 2450000, CoTrangThai = true },
                new LinhKien { MaLK = 13, TenLK = "Keo tản nhiệt Thermal Grizzly Kryonaut 1g", LoaiLK = "Vật tư khác", SoLuongTon = 100, DonGia = 180000, CoTrangThai = true },
                new LinhKien { MaLK = 14, TenLK = "Quạt tản nhiệt CPU Jonsbo CR-1000", LoaiLK = "Tản nhiệt", SoLuongTon = 20, DonGia = 350000, CoTrangThai = true },
                new LinhKien { MaLK = 15, TenLK = "Card Wi-Fi Intel AX210 NGW M.2", LoaiLK = "Card Mạng", SoLuongTon = 15, DonGia = 450000, CoTrangThai = true }
            );
        }
    }
}