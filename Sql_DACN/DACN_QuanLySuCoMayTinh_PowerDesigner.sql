-- MÔ HÌNH HIỆN TẠI CỦA ĐỀ TÀI QUẢN LÝ CHẨN ĐOÁN VÀ GIẢI PHÁP SỰ CỐ MÁY TÍNH
-- Xuất đối chiếu ngày 25/10/2026 từ QuanLySuCoMayTinh.zip do sinh viên cung cấp.
-- Căn cứ: Models/*.cs, Data/ApplicationDbContext.cs và ApplicationDbContextModelSnapshot.cs.
-- Hệ quản trị: Microsoft SQL Server. Mục đích: Reverse Engineer bằng script trong PowerDesigner.
-- Tên bảng và cột là tiếng Việt không dấu, theo quy ước của sơ đồ cơ sở.
-- Đây là bản ánh xạ tên phục vụ vẽ sơ đồ; không phải migration đổi tên CSDL của ứng dụng.
-- Tổng cộng: 20 bảng, 126 cột, 20 khóa chính, 19 khóa ngoại, 19 chỉ mục (3 duy nhất).
-- Giữ nguyên kiểu dữ liệu, độ dài, NULL, IDENTITY, khóa ghép và quy tắc xóa của EF.
-- PHIEU_TIEP_NHAN.MaNguoiDung_EF ánh xạ đúng cột ẩn USERID đã có trong EF và migration.
-- Các mã lỗi của lịch sử chẩn đoán KHÔNG được tự bổ sung thành khóa ngoại.
-- Không thêm bảng cho enum, lớp dữ liệu ML, ViewModel, JSON hoặc tệp mô hình.
-- Không thêm CHECK hoặc DEFAULT từ validation/giá trị khởi tạo C#.
-- Không chứa dữ liệu mẫu, tài khoản, lệnh DROP, ALTER hay thông tin kết nối.
-- Khóa ngoại đặt ngay trong CREATE TABLE để PowerDesigner đọc đầy đủ quan hệ.
-- Tạo mô hình PDM mới, không nhập chồng vào mô hình cũ nếu muốn đối chiếu số lượng chính xác.

-- Người dùng
-- Bảng trong source: USER
CREATE TABLE [dbo].[NGUOI_DUNG] (
    -- USERID: Mã người dùng
    [MaNguoiDung] int IDENTITY(1,1) NOT NULL,
    -- Username: Tên đăng nhập
    [TenDangNhap] nvarchar(50) NOT NULL,
    -- Password_Hash: Giá trị trường mật khẩu trong source; tên trường không chứng minh dữ liệu đã được băm
    [MatKhau] nvarchar(255) NOT NULL,
    -- FullName: Họ và tên
    [HoTen] nvarchar(100) NOT NULL,
    -- Role: Vai trò người dùng
    [VaiTro] nvarchar(20) NOT NULL,
    -- Status: Cờ trạng thái người dùng
    [CoTrangThai] bit NOT NULL,
    CONSTRAINT [PK_NGUOI_DUNG] PRIMARY KEY ([MaNguoiDung])
);
GO

-- Khách hàng
-- Bảng trong source: KHACH_HANG
CREATE TABLE [dbo].[KHACH_HANG] (
    -- MaKH: Mã khách hàng
    [MaKH] int IDENTITY(1,1) NOT NULL,
    -- HoTen: Họ và tên khách hàng
    [HoTen] nvarchar(100) NOT NULL,
    -- SDT: Số điện thoại
    [SDT] nvarchar(15) NOT NULL,
    -- DiaChi: Địa chỉ
    [DiaChi] nvarchar(255) NULL,
    CONSTRAINT [PK_KHACH_HANG] PRIMARY KEY ([MaKH])
);
GO

-- Triệu chứng
-- Bảng trong source: TRIEU_CHUNG
CREATE TABLE [dbo].[TRIEU_CHUNG] (
    -- MaTC: Mã triệu chứng
    [MaTC] int IDENTITY(1,1) NOT NULL,
    -- TenTC: Tên triệu chứng
    [TenTC] nvarchar(100) NOT NULL,
    -- MoTa: Mô tả triệu chứng
    [MoTa] nvarchar(max) NULL,
    CONSTRAINT [PK_TRIEU_CHUNG] PRIMARY KEY ([MaTC])
);
GO

-- Mã lỗi
-- Bảng trong source: MA_LOI
CREATE TABLE [dbo].[MA_LOI] (
    -- MaLoiID: Mã lỗi
    [MaLoi] int IDENTITY(1,1) NOT NULL,
    -- TenLoi: Tên lỗi
    [TenLoi] nvarchar(150) NOT NULL,
    -- PhanLoai: Phân loại lỗi
    [PhanLoai] nvarchar(50) NOT NULL,
    -- Status: Cờ trạng thái mã lỗi
    [CoTrangThai] bit NOT NULL,
    CONSTRAINT [PK_MA_LOI] PRIMARY KEY ([MaLoi])
);
GO

-- Linh kiện
-- Bảng trong source: LINH_KIEN
CREATE TABLE [dbo].[LINH_KIEN] (
    -- MaLK: Mã linh kiện
    [MaLK] int IDENTITY(1,1) NOT NULL,
    -- TenLK: Tên linh kiện
    [TenLK] nvarchar(255) NOT NULL,
    -- LoaiLK: Loại linh kiện
    [LoaiLK] nvarchar(100) NOT NULL,
    -- SoLuongTon: Số lượng tồn kho
    [SoLuongTon] int NOT NULL,
    -- DonGia: Đơn giá linh kiện
    [DonGia] decimal(18,0) NOT NULL,
    -- CoTrangThai: Cờ trạng thái linh kiện
    [CoTrangThai] bit NOT NULL,
    CONSTRAINT [PK_LINH_KIEN] PRIMARY KEY ([MaLK])
);
GO

-- Thông tin mô hình học máy
-- Bảng trong source: ML_MODEL_INFO
CREATE TABLE [dbo].[THONG_TIN_MO_HINH_HOC_MAY] (
    -- Id: Mã thông tin mô hình
    [MaMoHinh] int IDENTITY(1,1) NOT NULL,
    -- Version: Phiên bản mô hình
    [PhienBan] nvarchar(50) NOT NULL,
    -- MicroAccuracy: Độ chính xác micro do ML.NET trả về
    [DoChinhXacTongThe] float NOT NULL,
    -- MacroAccuracy: Độ chính xác macro, trung bình trên các lớp
    [DoChinhXacTrungBinhLop] float NOT NULL,
    -- LogLoss: Giá trị hàm mất mát logarit
    [MatMatLogarit] float NOT NULL,
    -- LogLossReduction: Mức giảm mất mát logarit
    [MucGiamMatMatLogarit] float NOT NULL,
    -- TrainingRecordCount: Giá trị tổng số bản ghi nguồn được service lưu vào trường TrainingRecordCount
    [SoMauHuanLuyen] int NOT NULL,
    -- RealRecordsCount: Số bản ghi nguồn thuộc nhóm sửa chữa thực tế
    [SoMauSuaChuaThucTe] int NOT NULL,
    -- UserHistoryRecordsCount: Số bản ghi nguồn thuộc nhóm lịch sử người dùng
    [SoMauLichSuNguoiDung] int NOT NULL,
    -- SyntheticRecordsCount: Số bản ghi nguồn thuộc nhóm tổng hợp theo luật
    [SoMauTongHop] int NOT NULL,
    -- RuleAccuracy: Độ chính xác theo luật được service lưu theo phần trăm
    [DoChinhXacTheoLuat] float NOT NULL,
    -- AIAccuracy: Độ chính xác AI được service lưu theo phần trăm
    [DoChinhXacAI] float NOT NULL,
    -- Top3Accuracy: Độ chính xác trong ba gợi ý đầu
    [DoChinhXac3GoiYDau] float NULL,
    -- Top5Accuracy: Độ chính xác trong năm gợi ý đầu
    [DoChinhXac5GoiYDau] float NULL,
    -- DatasetVersion: Phiên bản bộ dữ liệu
    [PhienBanBoDuLieu] nvarchar(50) NULL,
    -- DatasetGeneratedAt: Thời gian tạo bộ dữ liệu
    [ThoiGianTaoBoDuLieu] datetime2 NULL,
    -- ConfusionMatrixJson: Chuỗi JSON lưu thống kê nhầm lẫn; không phải bảng riêng
    [MaTranNhamLanJSON] nvarchar(max) NULL,
    -- CreatedAt: Thời gian tạo thông tin mô hình
    [ThoiGianTao] datetime2 NOT NULL,
    -- IsActive: Cờ đang sử dụng mô hình
    [DangSuDung] bit NOT NULL,
    -- FilePath: Đường dẫn tệp mô hình
    [DuongDanTep] nvarchar(255) NULL,
    CONSTRAINT [PK_THONG_TIN_MO_HINH_HOC_MAY] PRIMARY KEY ([MaMoHinh])
);
GO

-- Lịch sử kích hoạt mô hình
-- Bảng trong source: MODEL_ACTIVATION_LOG
CREATE TABLE [dbo].[LICH_SU_KICH_HOAT_MO_HINH] (
    -- Id: Mã lịch sử kích hoạt
    [MaLichSuKichHoat] int IDENTITY(1,1) NOT NULL,
    -- OldVersion: Chuỗi phiên bản cũ; không phải khóa ngoại
    [PhienBanCu] nvarchar(50) NULL,
    -- NewVersion: Chuỗi phiên bản mới; không phải khóa ngoại
    [PhienBanMoi] nvarchar(50) NOT NULL,
    -- ActivatedAt: Thời gian kích hoạt
    [ThoiGianKichHoat] datetime2 NOT NULL,
    -- ActivatedBy: Chuỗi tên người kích hoạt; không phải mã người dùng hoặc khóa ngoại
    [NguoiKichHoat] nvarchar(255) NULL,
    CONSTRAINT [PK_LICH_SU_KICH_HOAT_MO_HINH] PRIMARY KEY ([MaLichSuKichHoat])
);
GO

-- Trường hợp kiểm thử AI
-- Bảng trong source: AI_TEST_CASE
CREATE TABLE [dbo].[TRUONG_HOP_KIEM_THU_AI] (
    -- Id: Mã trường hợp kiểm thử
    [MaCaKiemThu] int IDENTITY(1,1) NOT NULL,
    -- Symptoms: Chuỗi tên triệu chứng phân tách bằng dấu phẩy
    [TrieuChung] nvarchar(max) NOT NULL,
    -- ExpectedFault: Chuỗi tên lỗi mong đợi
    [LoiMongDoi] nvarchar(max) NOT NULL,
    -- RulePrediction: Chuỗi tên lỗi dự đoán theo luật
    [DuDoanTheoLuat] nvarchar(max) NULL,
    -- AIPredictionTop1: Chuỗi dự đoán AI thứ nhất
    [DuDoanAIGoiY1] nvarchar(max) NULL,
    -- AIPredictionTop3: Chuỗi JSON của ba gợi ý AI
    [BaGoiYAI] nvarchar(max) NULL,
    -- IsAICorrect: Kết quả đúng sai của AI; có thể chưa xác định
    [KetQuaAIDung] bit NULL,
    -- CreatedAt: Thời gian tạo ca kiểm thử
    [ThoiGianTao] datetime2 NOT NULL,
    CONSTRAINT [PK_TRUONG_HOP_KIEM_THU_AI] PRIMARY KEY ([MaCaKiemThu])
);
GO

-- Thiết bị
-- Bảng trong source: THIET_BI
CREATE TABLE [dbo].[THIET_BI] (
    -- MaTB: Mã thiết bị
    [MaTB] int IDENTITY(1,1) NOT NULL,
    -- MaKH: Mã khách hàng sở hữu thiết bị
    [MaKH] int NOT NULL,
    -- TenTB: Tên thiết bị
    [TenTB] nvarchar(100) NOT NULL,
    -- CauHinh: Cấu hình thiết bị
    [CauHinh] nvarchar(255) NULL,
    -- NgoaiQuan: Tình trạng ngoại quan
    [NgoaiQuan] nvarchar(500) NULL,
    CONSTRAINT [PK_THIET_BI] PRIMARY KEY ([MaTB]),
    CONSTRAINT [FK_THIET_BI_MaKH] FOREIGN KEY ([MaKH])
        REFERENCES [dbo].[KHACH_HANG] ([MaKH])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Liên kết triệu chứng và lỗi
-- Bảng trong source: TRIEU_CHUNG_LOI
CREATE TABLE [dbo].[TRIEU_CHUNG_LOI] (
    -- MaTC: Mã triệu chứng
    [MaTC] int NOT NULL,
    -- MaLoi: Mã lỗi
    [MaLoi] int NOT NULL,
    CONSTRAINT [PK_TRIEU_CHUNG_LOI] PRIMARY KEY ([MaTC], [MaLoi]),
    CONSTRAINT [FK_TRIEU_CHUNG_LOI_MaLoi] FOREIGN KEY ([MaLoi])
        REFERENCES [dbo].[MA_LOI] ([MaLoi])
        ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT [FK_TRIEU_CHUNG_LOI_MaTC] FOREIGN KEY ([MaTC])
        REFERENCES [dbo].[TRIEU_CHUNG] ([MaTC])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Giải pháp
-- Bảng trong source: GIAI_PHAP
CREATE TABLE [dbo].[GIAI_PHAP] (
    -- MaGP: Mã giải pháp
    [MaGP] int IDENTITY(1,1) NOT NULL,
    -- MaLoi: Mã lỗi được giải quyết
    [MaLoi] int NOT NULL,
    -- NoiDungStepByStep: Nội dung các bước khắc phục
    [NoiDungCacBuoc] nvarchar(max) NOT NULL,
    -- MucDoKho: Mức độ khó
    [MucDoKho] int NULL,
    -- Status: Cờ trạng thái giải pháp
    [CoTrangThai] bit NOT NULL,
    CONSTRAINT [PK_GIAI_PHAP] PRIMARY KEY ([MaGP]),
    CONSTRAINT [FK_GIAI_PHAP_MaLoi] FOREIGN KEY ([MaLoi])
        REFERENCES [dbo].[MA_LOI] ([MaLoi])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Tác vụ huấn luyện AI
-- Bảng trong source: AI_TRAINING_JOB
CREATE TABLE [dbo].[TAC_VU_HUAN_LUYEN_AI] (
    -- Id: Mã tác vụ huấn luyện
    [MaTacVu] int IDENTITY(1,1) NOT NULL,
    -- CreatedAt: Thời gian tạo tác vụ
    [ThoiGianTao] datetime2 NOT NULL,
    -- StartedAt: Thời gian bắt đầu
    [ThoiGianBatDau] datetime2 NULL,
    -- CompletedAt: Thời gian hoàn thành
    [ThoiGianHoanThanh] datetime2 NULL,
    -- Status: Chuỗi trạng thái tác vụ
    [TrangThai] nvarchar(50) NOT NULL,
    -- ErrorMessage: Thông báo lỗi
    [ThongBaoLoi] nvarchar(max) NULL,
    -- ModelInfoId: Mã thông tin mô hình tạo ra; có thể trống
    [MaMoHinh] int NULL,
    CONSTRAINT [PK_TAC_VU_HUAN_LUYEN_AI] PRIMARY KEY ([MaTacVu]),
    CONSTRAINT [FK_TAC_VU_HUAN_LUYEN_AI_MaMoHinh] FOREIGN KEY ([MaMoHinh])
        REFERENCES [dbo].[THONG_TIN_MO_HINH_HOC_MAY] ([MaMoHinh])
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

-- Phiếu tiếp nhận
-- Bảng trong source: PHIEU_TIEP_NHAN
CREATE TABLE [dbo].[PHIEU_TIEP_NHAN] (
    -- MaPhieu: Mã phiếu tiếp nhận
    [MaPhieu] int IDENTITY(1,1) NOT NULL,
    -- MaTB: Mã thiết bị tiếp nhận
    [MaTB] int NOT NULL,
    -- MaUser_KTV: Mã kỹ thuật viên phụ trách; bắt buộc theo model hiện tại
    [Ma_KTV] int NOT NULL,
    -- MaLoi_CD: Mã lỗi chẩn đoán của phiếu
    [MaLoi_CD] int NULL,
    -- NgayNhan: Ngày tiếp nhận
    [NgayNhan] datetime2 NOT NULL,
    -- NgayTra_DK: Ngày trả dự kiến
    [NgayTra_DK] datetime2 NULL,
    -- TrangThai: Trạng thái xử lý phiếu
    [TrangThai] nvarchar(50) NOT NULL,
    -- GhiChuNoiBo: Ghi chú nội bộ
    [GhiChuNoiBo] nvarchar(max) NULL,
    -- USERID: Cột ẩn do EF sinh từ User.PhieuTiepNhans; quan hệ phụ chưa được đặt vai trò nghiệp vụ trong source
    [MaNguoiDung_EF] int NULL,
    CONSTRAINT [PK_PHIEU_TIEP_NHAN] PRIMARY KEY ([MaPhieu]),
    CONSTRAINT [FK_PHIEU_TIEP_NHAN_MaLoi_CD] FOREIGN KEY ([MaLoi_CD])
        REFERENCES [dbo].[MA_LOI] ([MaLoi])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_PHIEU_TIEP_NHAN_MaTB] FOREIGN KEY ([MaTB])
        REFERENCES [dbo].[THIET_BI] ([MaTB])
        ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT [FK_PHIEU_TIEP_NHAN_Ma_KTV] FOREIGN KEY ([Ma_KTV])
        REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_PHIEU_TIEP_NHAN_MaNguoiDung_EF] FOREIGN KEY ([MaNguoiDung_EF])
        REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung])
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

-- Chi tiết sửa chữa
-- Bảng trong source: CHI_TIET_SUA_CHUA
CREATE TABLE [dbo].[CHI_TIET_SUA_CHUA] (
    -- MaCT: Mã chi tiết sửa chữa
    [MaCT] int IDENTITY(1,1) NOT NULL,
    -- MaPhieu: Mã phiếu tiếp nhận
    [MaPhieu] int NOT NULL,
    -- TenHangMuc: Tên hạng mục sửa chữa
    [TenHangMuc] nvarchar(255) NOT NULL,
    -- ChiPhi: Chi phí hạng mục
    [ChiPhi] decimal(18,0) NOT NULL,
    -- GhiChu: Ghi chú hạng mục
    [GhiChu] nvarchar(255) NULL,
    CONSTRAINT [PK_CHI_TIET_SUA_CHUA] PRIMARY KEY ([MaCT]),
    CONSTRAINT [FK_CHI_TIET_SUA_CHUA_MaPhieu] FOREIGN KEY ([MaPhieu])
        REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Lịch sử trạng thái
-- Bảng trong source: LICH_SU_TRANG_THAI
CREATE TABLE [dbo].[LICH_SU_TRANG_THAI] (
    -- MaLS: Mã lịch sử trạng thái
    [MaLS] int IDENTITY(1,1) NOT NULL,
    -- MaPhieu: Mã phiếu tiếp nhận
    [MaPhieu] int NOT NULL,
    -- Ma_KTV: Mã kỹ thuật viên cập nhật; có thể trống
    [Ma_KTV] int NULL,
    -- TrangThaiCu: Trạng thái trước thay đổi
    [TrangThaiCu] nvarchar(50) NOT NULL,
    -- TrangThaiMoi: Trạng thái sau thay đổi
    [TrangThaiMoi] nvarchar(50) NOT NULL,
    -- ThoiGian: Thời gian thay đổi
    [ThoiGian] datetime2 NOT NULL,
    CONSTRAINT [PK_LICH_SU_TRANG_THAI] PRIMARY KEY ([MaLS]),
    CONSTRAINT [FK_LICH_SU_TRANG_THAI_MaPhieu] FOREIGN KEY ([MaPhieu])
        REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu])
        ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT [FK_LICH_SU_TRANG_THAI_Ma_KTV] FOREIGN KEY ([Ma_KTV])
        REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung])
        ON DELETE SET NULL ON UPDATE NO ACTION
);
GO

-- Hình ảnh sự cố
-- Bảng trong source: HINH_ANH_SU_CO
CREATE TABLE [dbo].[HINH_ANH_SU_CO] (
    -- MaHA: Mã hình ảnh
    [MaHA] int IDENTITY(1,1) NOT NULL,
    -- MaPhieu: Mã phiếu tiếp nhận
    [MaPhieu] int NOT NULL,
    -- DuongDanAnh: Đường dẫn ảnh
    [DuongDanAnh] nvarchar(500) NOT NULL,
    -- MoTa: Mô tả ảnh
    [MoTa] nvarchar(255) NULL,
    -- NgayChup: Ngày chụp ảnh
    [NgayChup] datetime2 NOT NULL,
    CONSTRAINT [PK_HINH_ANH_SU_CO] PRIMARY KEY ([MaHA]),
    CONSTRAINT [FK_HINH_ANH_SU_CO_MaPhieu] FOREIGN KEY ([MaPhieu])
        REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Đánh giá dịch vụ
-- Bảng trong source: DANH_GIA_DICH_VU
CREATE TABLE [dbo].[DANH_GIA_DICH_VU] (
    -- MaDG: Mã đánh giá; khóa chính độc lập với mã phiếu
    [MaDG] int IDENTITY(1,1) NOT NULL,
    -- MaPhieu: Mã phiếu được đánh giá; có chỉ mục duy nhất
    [MaPhieu] int NOT NULL,
    -- SoSao: Số sao đánh giá
    [SoSao] int NOT NULL,
    -- BinhLuan: Nội dung bình luận
    [BinhLuan] nvarchar(1000) NULL,
    -- NgayDG: Ngày đánh giá
    [NgayDG] datetime2 NOT NULL,
    CONSTRAINT [PK_DANH_GIA_DICH_VU] PRIMARY KEY ([MaDG]),
    CONSTRAINT [FK_DANH_GIA_DICH_VU_MaPhieu] FOREIGN KEY ([MaPhieu])
        REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Lịch sử chẩn đoán
-- Bảng trong source: LICH_SU_CHAN_DOAN
CREATE TABLE [dbo].[LICH_SU_CHAN_DOAN] (
    -- Id: Mã lịch sử chẩn đoán
    [MaLichSuChanDoan] int IDENTITY(1,1) NOT NULL,
    -- MaPhieu: Mã phiếu tiếp nhận
    [MaPhieu] int NOT NULL,
    -- MaLoiRule: Mã lỗi gợi ý theo luật; chưa có khóa ngoại trong source
    [MaLoiTheoLuat] int NULL,
    -- MaLoiAITop1: Mã lỗi AI gợi ý thứ nhất; chưa có khóa ngoại
    [MaLoiAIGoiY1] int NULL,
    -- ConfidenceTop1: Điểm tin cậy của gợi ý AI thứ nhất
    [DoTinCayGoiY1] real NULL,
    -- MaLoiAITop2: Mã lỗi AI gợi ý thứ hai; chưa có khóa ngoại
    [MaLoiAIGoiY2] int NULL,
    -- ConfidenceTop2: Điểm tin cậy của gợi ý AI thứ hai
    [DoTinCayGoiY2] real NULL,
    -- MaLoiAITop3: Mã lỗi AI gợi ý thứ ba; chưa có khóa ngoại
    [MaLoiAIGoiY3] int NULL,
    -- ConfidenceTop3: Điểm tin cậy của gợi ý AI thứ ba
    [DoTinCayGoiY3] real NULL,
    -- DataSource: Nguồn dữ liệu huấn luyện, lưu bằng số nguyên của enum
    [NguonDuLieu] int NOT NULL,
    -- MaLoiThucTe: Mã lỗi thực tế; chưa có khóa ngoại trong source
    [MaLoiThucTe] int NULL,
    -- TenLoiThucTeSnapshot: Tên lỗi thực tế được lưu tại thời điểm ghi nhận
    [TenLoiThucTeLuuVet] nvarchar(255) NULL,
    -- ThoiGian: Thời gian chẩn đoán
    [ThoiGian] datetime2 NOT NULL,
    CONSTRAINT [PK_LICH_SU_CHAN_DOAN] PRIMARY KEY ([MaLichSuChanDoan]),
    CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaPhieu] FOREIGN KEY ([MaPhieu])
        REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Sử dụng linh kiện
-- Bảng trong source: SU_DUNG_LINH_KIEN
CREATE TABLE [dbo].[SU_DUNG_LINH_KIEN] (
    -- MaCT: Mã chi tiết sửa chữa sử dụng linh kiện
    [MaCT] int NOT NULL,
    -- MaLK: Mã linh kiện được sử dụng
    [MaLK] int NOT NULL,
    -- SoLuong: Số lượng sử dụng
    [SoLuong] int NOT NULL,
    -- DonGiaXuat: Đơn giá tại thời điểm xuất kho
    [DonGiaXuat] decimal(18,0) NOT NULL,
    CONSTRAINT [PK_SU_DUNG_LINH_KIEN] PRIMARY KEY ([MaCT], [MaLK]),
    CONSTRAINT [FK_SU_DUNG_LINH_KIEN_MaCT] FOREIGN KEY ([MaCT])
        REFERENCES [dbo].[CHI_TIET_SUA_CHUA] ([MaCT])
        ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT [FK_SU_DUNG_LINH_KIEN_MaLK] FOREIGN KEY ([MaLK])
        REFERENCES [dbo].[LINH_KIEN] ([MaLK])
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

-- Triệu chứng trong lịch sử chẩn đoán
-- Bảng trong source: LICH_SU_CHAN_DOAN_TRIEU_CHUNG
CREATE TABLE [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] (
    -- Id: Mã dòng triệu chứng; khóa chính đơn, không phải khóa ghép
    [MaChiTietTrieuChung] int IDENTITY(1,1) NOT NULL,
    -- MaLichSuChanDoan: Mã lịch sử chẩn đoán
    [MaLichSuChanDoan] int NOT NULL,
    -- MaTrieuChung: Mã triệu chứng
    [MaTC] int NOT NULL,
    -- TenTrieuChungSnapshot: Tên triệu chứng được lưu tại thời điểm ghi nhận
    [TenTrieuChungLuuVet] nvarchar(255) NULL,
    CONSTRAINT [PK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG] PRIMARY KEY ([MaChiTietTrieuChung]),
    CONSTRAINT [FK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaLichSuChanDoan] FOREIGN KEY ([MaLichSuChanDoan])
        REFERENCES [dbo].[LICH_SU_CHAN_DOAN] ([MaLichSuChanDoan])
        ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT [FK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaTC] FOREIGN KEY ([MaTC])
        REFERENCES [dbo].[TRIEU_CHUNG] ([MaTC])
        ON DELETE CASCADE ON UPDATE NO ACTION
);
GO

-- Các chỉ mục đúng theo mô hình EF Core, gồm chỉ mục duy nhất của quan hệ đánh giá 1–0..1.

CREATE UNIQUE NONCLUSTERED INDEX [IX_NGUOI_DUNG_TenDangNhap]
    ON [dbo].[NGUOI_DUNG] ([TenDangNhap]);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_KHACH_HANG_SDT]
    ON [dbo].[KHACH_HANG] ([SDT]);
GO

CREATE NONCLUSTERED INDEX [IX_THIET_BI_MaKH]
    ON [dbo].[THIET_BI] ([MaKH]);
GO

CREATE NONCLUSTERED INDEX [IX_TRIEU_CHUNG_LOI_MaLoi]
    ON [dbo].[TRIEU_CHUNG_LOI] ([MaLoi]);
GO

CREATE NONCLUSTERED INDEX [IX_GIAI_PHAP_MaLoi]
    ON [dbo].[GIAI_PHAP] ([MaLoi]);
GO

CREATE NONCLUSTERED INDEX [IX_TAC_VU_HUAN_LUYEN_AI_MaMoHinh]
    ON [dbo].[TAC_VU_HUAN_LUYEN_AI] ([MaMoHinh]);
GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_MaLoi_CD]
    ON [dbo].[PHIEU_TIEP_NHAN] ([MaLoi_CD]);
GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_MaTB]
    ON [dbo].[PHIEU_TIEP_NHAN] ([MaTB]);
GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_Ma_KTV]
    ON [dbo].[PHIEU_TIEP_NHAN] ([Ma_KTV]);
GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_MaNguoiDung_EF]
    ON [dbo].[PHIEU_TIEP_NHAN] ([MaNguoiDung_EF]);
GO

CREATE NONCLUSTERED INDEX [IX_CHI_TIET_SUA_CHUA_MaPhieu]
    ON [dbo].[CHI_TIET_SUA_CHUA] ([MaPhieu]);
GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_TRANG_THAI_Ma_KTV]
    ON [dbo].[LICH_SU_TRANG_THAI] ([Ma_KTV]);
GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_TRANG_THAI_MaPhieu]
    ON [dbo].[LICH_SU_TRANG_THAI] ([MaPhieu]);
GO

CREATE NONCLUSTERED INDEX [IX_HINH_ANH_SU_CO_MaPhieu]
    ON [dbo].[HINH_ANH_SU_CO] ([MaPhieu]);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DANH_GIA_DICH_VU_MaPhieu]
    ON [dbo].[DANH_GIA_DICH_VU] ([MaPhieu]);
GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaPhieu]
    ON [dbo].[LICH_SU_CHAN_DOAN] ([MaPhieu]);
GO

CREATE NONCLUSTERED INDEX [IX_SU_DUNG_LINH_KIEN_MaLK]
    ON [dbo].[SU_DUNG_LINH_KIEN] ([MaLK]);
GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaLichSuChanDoan]
    ON [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] ([MaLichSuChanDoan]);
GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaTC]
    ON [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] ([MaTC]);
GO
