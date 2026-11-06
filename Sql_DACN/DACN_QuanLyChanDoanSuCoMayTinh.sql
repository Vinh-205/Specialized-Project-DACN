-- PHIEN BAN BIEU DIEN ERD/SRD TIENG VIET KHONG DAU, 26/10/2026.
-- CHI DUNG CHO POWERDESIGNER HOAC CSDL KIEM THU MOI RONG. KHONG PHAI MIGRATION WEBSITE.
-- 25 BANG / 213 THUOC TINH / 41 KHOA NGOAI / 13 DIEU KIEN KIEM TRA.
-- DATABASE DESIGN BASELINE V1 / implemented LocalDB schema, 2026-10-25.
-- Business tables only: 25 tables, 213 columns, 41 FK, 13 CHECK.
-- Companion DATABASE_EXTENDED_DDL.sql contains the deployed immutable-input trigger.
-- No CREATE DATABASE / USE / data INSERT / credentials / migration history rows.
-- Reverse Engineer SQL Server syntax in PowerDesigner; direct PowerDesigner verification not performed.
-- SQL Server schema captured from sys catalog; no business data.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

GO

CREATE TABLE [dbo].[TRUONG_HOP_KIEM_THU_AI] (
    [MaTruongHopKiemThuAI] [int] IDENTITY(1,1) NOT NULL,
    [TrieuChung] [nvarchar](max) NOT NULL,
    [LoiKyVong] [nvarchar](max) NOT NULL,
    [DuDoanTheoLuat] [nvarchar](max) NULL,
    [DuDoanAIXepHang1] [nvarchar](max) NULL,
    [BaDuDoanAIDau] [nvarchar](max) NULL,
    [DuDoanAIDung] [bit] NULL,
    [ThoiGianTao] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_TRUONG_HOP_KIEM_THU_AI] PRIMARY KEY CLUSTERED ([MaTruongHopKiemThuAI] ASC)
);

GO

CREATE TABLE [dbo].[TAC_VU_HUAN_LUYEN_AI] (
    [MaTacVuHuanLuyen] [int] IDENTITY(1,1) NOT NULL,
    [ThoiGianTao] [datetime2](7) NOT NULL,
    [ThoiGianBatDau] [datetime2](7) NULL,
    [ThoiGianHoanTat] [datetime2](7) NULL,
    [TrangThaiTacVu] [nvarchar](50) NOT NULL,
    [ThongBaoLoi] [nvarchar](max) NULL,
    [MaMoHinh] [int] NULL,
    [ThamChieuQuyenThucThi] [nvarchar](255) NULL,
    [MaKiemTraTapDuLieu] [nvarchar](64) NULL,
    [PhienBanTapDuLieu] [nvarchar](64) NULL,
    [ThamChieuBanKeBenNgoai] [nvarchar](255) NULL,
    [DinhNghiaTacVu] [nvarchar](100) NULL,
    CONSTRAINT [PK_TAC_VU_HUAN_LUYEN_AI] PRIMARY KEY CLUSTERED ([MaTacVuHuanLuyen] ASC)
);

GO

CREATE TABLE [dbo].[CHI_TIET_DU_DOAN_AI] (
    [MaChiTietDuDoan] [bigint] IDENTITY(1,1) NOT NULL,
    [MaLanDuDoan] [bigint] NOT NULL,
    [MaNhomSuCo] [int] NOT NULL,
    [TacVu] [nvarchar](100) NOT NULL,
    [PhienBanDinhNghiaNhan] [nvarchar](64) NOT NULL,
    [ThuTuDuDoan] [int] NOT NULL,
    [DiemDuDoan] [float] NOT NULL,
    CONSTRAINT [PK_CHI_TIET_DU_DOAN_AI] PRIMARY KEY CLUSTERED ([MaChiTietDuDoan] ASC)
);

GO

CREATE TABLE [dbo].[CHI_TIET_SUA_CHUA] (
    [MaChiTietSuaChua] [int] IDENTITY(1,1) NOT NULL,
    [MaPhieu] [int] NOT NULL,
    [TenHangMuc] [nvarchar](255) NOT NULL,
    [ChiPhi] [decimal](18,0) NOT NULL,
    [GhiChu] [nvarchar](255) NULL,
    [DonGiaLegacy] [decimal](18,0) NOT NULL CONSTRAINT [DF_CHI_TIET_SUA_CHUA_DonGiaLegacy] DEFAULT ((0.0)),
    [CoXoaLegacy] [bit] NOT NULL CONSTRAINT [DF_CHI_TIET_SUA_CHUA_CoXoaLegacy] DEFAULT (CONVERT([bit],(0))),
    [MaLinhKienLegacy] [int] NULL,
    [LoaiXuLyLegacy] [int] NOT NULL CONSTRAINT [DF_CHI_TIET_SUA_CHUA_LoaiXuLyLegacy] DEFAULT ((0)),
    [SoLuongLegacy] [int] NOT NULL CONSTRAINT [DF_CHI_TIET_SUA_CHUA_SoLuongLegacy] DEFAULT ((0)),
    [ThamChieuBangChungKiemTraLai] [nvarchar](255) NULL,
    [TrangThaiKiemTraLai] [nvarchar](32) NULL,
    [ThoiGianKiemTraLaiUTC] [datetime2](7) NULL,
    [MaNguoiDungKiemTraLai] [int] NULL,
    CONSTRAINT [PK_CHI_TIET_SUA_CHUA] PRIMARY KEY CLUSTERED ([MaChiTietSuaChua] ASC)
);

GO

CREATE TABLE [dbo].[DANH_GIA_DICH_VU] (
    [MaDanhGia] [int] IDENTITY(1,1) NOT NULL,
    [MaPhieu] [int] NOT NULL,
    [SoSao] [int] NOT NULL,
    [BinhLuan] [nvarchar](1000) NULL,
    [NgayDanhGia] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_DANH_GIA_DICH_VU] PRIMARY KEY CLUSTERED ([MaDanhGia] ASC)
);

GO

CREATE TABLE [dbo].[GIAI_PHAP] (
    [MaGiaiPhap] [int] IDENTITY(1,1) NOT NULL,
    [MaLoi] [int] NOT NULL,
    [NoiDungTungBuoc] [nvarchar](max) NOT NULL,
    [MucDoKho] [int] NULL,
    [TrangThaiSuDung] [bit] NOT NULL,
    CONSTRAINT [PK_GIAI_PHAP] PRIMARY KEY CLUSTERED ([MaGiaiPhap] ASC)
);

GO

CREATE TABLE [dbo].[HINH_ANH_SU_CO] (
    [MaHinhAnh] [int] IDENTITY(1,1) NOT NULL,
    [MaPhieu] [int] NOT NULL,
    [DuongDanAnh] [nvarchar](500) NOT NULL,
    [MoTa] [nvarchar](255) NULL,
    [NgayChup] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_HINH_ANH_SU_CO] PRIMARY KEY CLUSTERED ([MaHinhAnh] ASC)
);

GO

CREATE TABLE [dbo].[KET_QUA_KIEM_THU_AI] (
    [MaKetQuaKiemThuAI] [bigint] IDENTITY(1,1) NOT NULL,
    [MaTruongHopKiemThuAI] [int] NOT NULL,
    [MaMoHinh] [int] NOT NULL,
    [PhienBanDinhNghiaKiemThu] [nvarchar](64) NOT NULL,
    [PhienBanTapDuLieu] [nvarchar](64) NOT NULL,
    [ThamChieuQuyenThucThi] [nvarchar](255) NOT NULL,
    [DuLieuDuDoanJSON] [nvarchar](max) NOT NULL,
    [DuLieuDanhGiaJSON] [nvarchar](max) NULL,
    [ThoiGianDanhGiaUTC] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_KET_QUA_KIEM_THU_AI] PRIMARY KEY CLUSTERED ([MaKetQuaKiemThuAI] ASC)
);

GO

CREATE TABLE [dbo].[KHACH_HANG] (
    [MaKhachHang] [int] IDENTITY(1,1) NOT NULL,
    [HoTen] [nvarchar](100) NOT NULL,
    [SoDienThoai] [nvarchar](15) NOT NULL,
    [DiaChi] [nvarchar](255) NULL,
    CONSTRAINT [PK_KHACH_HANG] PRIMARY KEY CLUSTERED ([MaKhachHang] ASC)
);

GO

CREATE TABLE [dbo].[LAN_DU_DOAN_AI] (
    [MaLanDuDoan] [bigint] IDENTITY(1,1) NOT NULL,
    [MaLichSuChanDoan] [int] NOT NULL,
    [MaPhieu] [int] NOT NULL,
    [MaBanChupDauVao] [bigint] NOT NULL,
    [MaMoHinh] [int] NULL,
    [TacVu] [nvarchar](100) NOT NULL,
    [PhienBanDinhNghiaNhan] [nvarchar](64) NOT NULL,
    [TrangThaiDuDoan] [nvarchar](32) NOT NULL,
    [LyDoTuChoiDuDoan] [nvarchar](255) NULL,
    [LoaiDiemDuDoan] [nvarchar](32) NOT NULL,
    [ThoiGianTaoUTC] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_LAN_DU_DOAN_AI] PRIMARY KEY CLUSTERED ([MaLanDuDoan] ASC),
    CONSTRAINT [UQ_LAN_DU_DOAN_AI_MaLanDuDoan_TacVu_PhienBanDinhNghiaNhan] UNIQUE NONCLUSTERED ([MaLanDuDoan] ASC, [TacVu] ASC, [PhienBanDinhNghiaNhan] ASC)
);

GO

CREATE TABLE [dbo].[LICH_SU_CHAN_DOAN] (
    [MaLichSuChanDoan] [int] IDENTITY(1,1) NOT NULL,
    [MaPhieu] [int] NOT NULL,
    [MaLoiTheoLuat] [int] NULL,
    [MaLoiAIXepHang3] [int] NULL,
    [MaLoiThucTe] [int] NULL,
    [TenLoiThucTeLuuLai] [nvarchar](255) NULL,
    [ThoiGian] [datetime2](7) NOT NULL,
    [DiemTinCayXepHang1] [real] NULL,
    [DiemTinCayXepHang2] [real] NULL,
    [DiemTinCayXepHang3] [real] NULL,
    [NguonDuLieu] [int] NOT NULL CONSTRAINT [DF_LICH_SU_CHAN_DOAN_NguonDuLieu] DEFAULT ((0)),
    [MaLoiAIXepHang1] [int] NULL,
    [MaLoiAIXepHang2] [int] NULL,
    [MaNguoiDungThucHien] [int] NULL,
    [MaLoiNguoiVanHanhChanDoan] [int] NULL,
    [MaLoiKyThuatVienBaoCao] [int] NULL,
    [ThoiGianBaoCaoKetLuanUTC] [datetime2](7) NULL,
    [MaNguoiDungBaoCaoKetLuan] [int] NULL,
    CONSTRAINT [PK_LICH_SU_CHAN_DOAN] PRIMARY KEY CLUSTERED ([MaLichSuChanDoan] ASC),
    CONSTRAINT [UQ_LICH_SU_CHAN_DOAN_MaLichSuChanDoan_MaPhieu] UNIQUE NONCLUSTERED ([MaLichSuChanDoan] ASC, [MaPhieu] ASC)
);

GO

CREATE TABLE [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] (
    [MaLienKetLichSuTrieuChung] [int] IDENTITY(1,1) NOT NULL,
    [MaLichSuChanDoan] [int] NOT NULL,
    [MaTrieuChung] [int] NOT NULL,
    [TenTrieuChungLuuLai] [nvarchar](255) NULL,
    CONSTRAINT [PK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG] PRIMARY KEY CLUSTERED ([MaLienKetLichSuTrieuChung] ASC)
);

GO

CREATE TABLE [dbo].[LICH_SU_TRANG_THAI] (
    [MaLichSuTrangThai] [int] IDENTITY(1,1) NOT NULL,
    [MaPhieu] [int] NOT NULL,
    [TrangThaiCu] [nvarchar](50) NOT NULL,
    [TrangThaiMoi] [nvarchar](50) NOT NULL,
    [ThoiGian] [datetime2](7) NOT NULL,
    [MaKyThuatVienThucHien] [int] NULL,
    [LoaiSuKien] [nvarchar](50) NULL,
    [DuLieuBoSungJSON] [nvarchar](max) NULL,
    CONSTRAINT [PK_LICH_SU_TRANG_THAI] PRIMARY KEY CLUSTERED ([MaLichSuTrangThai] ASC)
);

GO

CREATE TABLE [dbo].[LINH_KIEN] (
    [MaLinhKien] [int] IDENTITY(1,1) NOT NULL,
    [TenLinhKien] [nvarchar](255) NOT NULL,
    [LoaiLinhKien] [nvarchar](100) NOT NULL,
    [SoLuongTon] [int] NOT NULL,
    [DonGia] [decimal](18,0) NOT NULL,
    [TrangThaiSuDung] [bit] NOT NULL,
    CONSTRAINT [PK_LINH_KIEN] PRIMARY KEY CLUSTERED ([MaLinhKien] ASC)
);

GO

CREATE TABLE [dbo].[MA_LOI] (
    [MaLoi] [int] IDENTITY(1,1) NOT NULL,
    [TenLoi] [nvarchar](150) NOT NULL,
    [PhanLoai] [nvarchar](50) NOT NULL,
    [TrangThaiSuDung] [bit] NOT NULL,
    CONSTRAINT [PK_MA_LOI] PRIMARY KEY CLUSTERED ([MaLoi] ASC)
);

GO

CREATE TABLE [dbo].[THONG_TIN_MO_HINH_HOC_MAY] (
    [MaMoHinh] [int] IDENTITY(1,1) NOT NULL,
    [PhienBanMoHinh] [nvarchar](50) NOT NULL,
    [DoChinhXacToanBoMau] [float] NOT NULL,
    [DoChinhXacTrungBinhTheoLop] [float] NOT NULL,
    [MatMatLogarit] [float] NOT NULL,
    [MucGiamMatMatLogarit] [float] NOT NULL,
    [SoLuongBanGhiHuanLuyen] [int] NOT NULL,
    [ThoiGianTao] [datetime2](7) NOT NULL,
    [DangKichHoat] [bit] NOT NULL,
    [DuongDanTep] [nvarchar](255) NULL,
    [DoChinhXacAI] [float] NOT NULL CONSTRAINT [DF_THONG_TIN_MO_HINH_HOC_MAY_DoChinhXacAI] DEFAULT ((0.0000000000000000e+000)),
    [MaTranNhanLanJSON] [nvarchar](max) NULL,
    [ThoiGianTaoTapDuLieu] [datetime2](7) NULL,
    [PhienBanTapDuLieu] [nvarchar](50) NULL,
    [SoLuongBanGhiThuc] [int] NOT NULL CONSTRAINT [DF_THONG_TIN_MO_HINH_HOC_MAY_SoLuongBanGhiThuc] DEFAULT ((0)),
    [DoChinhXacTheoLuat] [float] NOT NULL CONSTRAINT [DF_THONG_TIN_MO_HINH_HOC_MAY_DoChinhXacTheoLuat] DEFAULT ((0.0000000000000000e+000)),
    [SoLuongBanGhiGiaLap] [int] NOT NULL CONSTRAINT [DF_THONG_TIN_MO_HINH_HOC_MAY_SoLuongBanGhiGiaLap] DEFAULT ((0)),
    [SoLuongBanGhiLichSuNguoiDung] [int] NOT NULL CONSTRAINT [DF_THONG_TIN_MO_HINH_HOC_MAY_SoLuongBanGhiLichSuNguoiDung] DEFAULT ((0)),
    [DoChinhXacBaDuDoanDau] [float] NULL,
    [DoChinhXacNamDuDoanDau] [float] NULL,
    [TrangThaiPheDuyet] [nvarchar](32) NOT NULL CONSTRAINT [DF_THONG_TIN_MO_HINH_HOC_MAY_TrangThaiPheDuyet] DEFAULT (N'UNREVIEWED'),
    [MaBamTepMoHinhSHA256] [nvarchar](64) NULL,
    [ThamChieuBanKeTapDuLieu] [nvarchar](255) NULL,
    [MaBamTapDuLieuSHA256] [nvarchar](64) NULL,
    [PhienBanDinhNghiaNhan] [nvarchar](64) NULL,
    [DinhDanhMoHinh] [nvarchar](100) NULL,
    [LoaiMoHinh] [nvarchar](50) NULL,
    [DinhNghiaTacVu] [nvarchar](100) NULL,
    [NenTangHuanLuyen] [nvarchar](50) NULL,
    [ThamChieuNguonGocHuanLuyen] [nvarchar](255) NULL,
    CONSTRAINT [PK_THONG_TIN_MO_HINH_HOC_MAY] PRIMARY KEY CLUSTERED ([MaMoHinh] ASC)
);

GO

CREATE TABLE [dbo].[LICH_SU_KICH_HOAT_MO_HINH] (
    [MaLichSuKichHoat] [int] IDENTITY(1,1) NOT NULL,
    [PhienBanMoHinhCu] [nvarchar](50) NULL,
    [PhienBanMoHinhMoi] [nvarchar](50) NOT NULL,
    [ThoiGianKichHoat] [datetime2](7) NOT NULL,
    [NguoiKichHoatGhiNhan] [nvarchar](255) NULL,
    [MaNguoiDungThucHien] [int] NULL,
    [MaMoHinhMoi] [int] NULL,
    [MaMoHinhCu] [int] NULL,
    [LyDo] [nvarchar](500) NULL,
    [KetQua] [nvarchar](32) NULL,
    CONSTRAINT [PK_LICH_SU_KICH_HOAT_MO_HINH] PRIMARY KEY CLUSTERED ([MaLichSuKichHoat] ASC)
);

GO

CREATE TABLE [dbo].[NHOM_SU_CO_AI] (
    [MaNhomSuCo] [int] IDENTITY(1,1) NOT NULL,
    [TacVu] [nvarchar](100) NOT NULL,
    [PhienBanDinhNghiaNhan] [nvarchar](64) NOT NULL,
    [MaQuyUocNhom] [nvarchar](64) NOT NULL,
    [TenNhomSuCo] [nvarchar](255) NOT NULL,
    [MoTaNhomSuCo] [nvarchar](2000) NULL,
    [DuocBat] [bit] NOT NULL,
    CONSTRAINT [PK_NHOM_SU_CO_AI] PRIMARY KEY CLUSTERED ([MaNhomSuCo] ASC),
    CONSTRAINT [UQ_NHOM_SU_CO_AI_MaNhomSuCo_TacVu_PhienBanDinhNghiaNhan] UNIQUE NONCLUSTERED ([MaNhomSuCo] ASC, [TacVu] ASC, [PhienBanDinhNghiaNhan] ASC)
);

GO

CREATE TABLE [dbo].[PHIEU_TIEP_NHAN] (
    [MaPhieu] [int] IDENTITY(1,1) NOT NULL,
    [MaThietBi] [int] NOT NULL,
    [MaKyThuatVienDuocGiao] [int] NOT NULL,
    [NgayNhan] [datetime2](7) NOT NULL,
    [NgayTraDuKien] [datetime2](7) NULL,
    [TrangThai] [nvarchar](50) NOT NULL,
    [MaLoiChanDoan] [int] NULL,
    [MaNguoiDungLegacy] [int] NULL,
    [GhiChuNoiBo] [nvarchar](max) NULL,
    CONSTRAINT [PK_PHIEU_TIEP_NHAN] PRIMARY KEY CLUSTERED ([MaPhieu] ASC)
);

GO

CREATE TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] (
    [MaBanChupDauVao] [bigint] IDENTITY(1,1) NOT NULL,
    [MaPhieu] [int] NOT NULL,
    [MaDotSuCo] [uniqueidentifier] NOT NULL,
    [SoPhienBan] [int] NOT NULL,
    [MaNguoiDungGhiNhan] [int] NOT NULL,
    [ThoiGianGhiNhanUTC] [datetime2](7) NOT NULL CONSTRAINT [DF_BAN_CHUP_DAU_VAO_CHAN_DOAN_ThoiGianGhiNhanUTC] DEFAULT (sysutcdatetime()),
    [DuLieuTrieuChungJSON] [nvarchar](max) NOT NULL,
    [MaBamDauVaoSHA256] [nvarchar](64) NOT NULL,
    [MaBamDanhMucSHA256] [nvarchar](64) NOT NULL,
    [PhienBanCauTrucDauVao] [nvarchar](64) NOT NULL,
    [NgonNgu] [nvarchar](16) NOT NULL,
    [LoaiMocThoiGian] [nvarchar](64) NOT NULL,
    [TrangThaiQuyenNghienCuu] [nvarchar](32) NOT NULL,
    [DuDieuKienUngVien] [bit] NOT NULL,
    [DuDieuKienHuanLuyen] [bit] NOT NULL,
    [PhienBanDongDuLieu] [timestamp] NOT NULL,
    [MoTaSuCoBanDau] [nvarchar](4000) NULL,
    CONSTRAINT [PK_BAN_CHUP_DAU_VAO_CHAN_DOAN] PRIMARY KEY CLUSTERED ([MaBanChupDauVao] ASC),
    CONSTRAINT [UQ_BAN_CHUP_DAU_VAO_CHAN_DOAN_MaBanChupDauVao_MaPhieu] UNIQUE NONCLUSTERED ([MaBanChupDauVao] ASC, [MaPhieu] ASC)
);

GO

CREATE TABLE [dbo].[SU_DUNG_LINH_KIEN] (
    [MaChiTietSuaChua] [int] NOT NULL,
    [MaLinhKien] [int] NOT NULL,
    [SoLuong] [int] NOT NULL,
    [DonGiaXuat] [decimal](18,0) NOT NULL,
    CONSTRAINT [PK_SU_DUNG_LINH_KIEN] PRIMARY KEY CLUSTERED ([MaChiTietSuaChua] ASC, [MaLinhKien] ASC)
);

GO

CREATE TABLE [dbo].[THIET_BI] (
    [MaThietBi] [int] IDENTITY(1,1) NOT NULL,
    [TenThietBi] [nvarchar](100) NOT NULL,
    [CauHinh] [nvarchar](255) NULL,
    [MaKhachHang] [int] NOT NULL,
    [NgoaiQuan] [nvarchar](500) NULL,
    CONSTRAINT [PK_THIET_BI] PRIMARY KEY CLUSTERED ([MaThietBi] ASC)
);

GO

CREATE TABLE [dbo].[TRIEU_CHUNG] (
    [MaTrieuChung] [int] IDENTITY(1,1) NOT NULL,
    [TenTrieuChung] [nvarchar](100) NOT NULL,
    [MoTa] [nvarchar](max) NULL,
    CONSTRAINT [PK_TRIEU_CHUNG] PRIMARY KEY CLUSTERED ([MaTrieuChung] ASC)
);

GO

CREATE TABLE [dbo].[TRIEU_CHUNG_LOI] (
    [MaTrieuChung] [int] NOT NULL,
    [MaLoi] [int] NOT NULL,
    CONSTRAINT [PK_TRIEU_CHUNG_LOI] PRIMARY KEY CLUSTERED ([MaTrieuChung] ASC, [MaLoi] ASC)
);

GO

CREATE TABLE [dbo].[NGUOI_DUNG] (
    [MaNguoiDung] [int] IDENTITY(1,1) NOT NULL,
    [TenDangNhap] [nvarchar](50) NOT NULL,
    [MatKhauBam] [nvarchar](255) NOT NULL,
    [HoTen] [nvarchar](100) NOT NULL,
    [VaiTro] [nvarchar](20) NOT NULL,
    [TrangThaiHoatDong] [bit] NOT NULL,
    CONSTRAINT [PK_NGUOI_DUNG] PRIMARY KEY CLUSTERED ([MaNguoiDung] ASC)
);

GO

CREATE NONCLUSTERED INDEX [IX_TAC_VU_HUAN_LUYEN_AI_MaMoHinh] ON [dbo].[TAC_VU_HUAN_LUYEN_AI] ([MaMoHinh] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_CHI_TIET_DU_DOAN_AI_MaNhomSuCo_TacVu_PhienBanDinhNghiaNhan] ON [dbo].[CHI_TIET_DU_DOAN_AI] ([MaNhomSuCo] ASC, [TacVu] ASC, [PhienBanDinhNghiaNhan] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_CHI_TIET_DU_DOAN_AI_MaLanDuDoan_MaNhomSuCo] ON [dbo].[CHI_TIET_DU_DOAN_AI] ([MaLanDuDoan] ASC, [MaNhomSuCo] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_CHI_TIET_DU_DOAN_AI_MaLanDuDoan_ThuTuDuDoan] ON [dbo].[CHI_TIET_DU_DOAN_AI] ([MaLanDuDoan] ASC, [ThuTuDuDoan] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_CHI_TIET_DU_DOAN_AI_MaLanDuDoan_TacVu_PhienBanDinhNghiaNhan] ON [dbo].[CHI_TIET_DU_DOAN_AI] ([MaLanDuDoan] ASC, [TacVu] ASC, [PhienBanDinhNghiaNhan] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_CHI_TIET_SUA_CHUA_MaPhieu] ON [dbo].[CHI_TIET_SUA_CHUA] ([MaPhieu] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_CHI_TIET_SUA_CHUA_MaLinhKienLegacy] ON [dbo].[CHI_TIET_SUA_CHUA] ([MaLinhKienLegacy] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_CHI_TIET_SUA_CHUA_MaNguoiDungKiemTraLai] ON [dbo].[CHI_TIET_SUA_CHUA] ([MaNguoiDungKiemTraLai] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_DANH_GIA_DICH_VU_MaPhieu] ON [dbo].[DANH_GIA_DICH_VU] ([MaPhieu] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_GIAI_PHAP_MaLoi] ON [dbo].[GIAI_PHAP] ([MaLoi] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_HINH_ANH_SU_CO_MaPhieu] ON [dbo].[HINH_ANH_SU_CO] ([MaPhieu] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_KET_QUA_KIEM_THU_AI_MaMoHinh] ON [dbo].[KET_QUA_KIEM_THU_AI] ([MaMoHinh] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_KET_QUA_KIEM_THU_AI_MaTruongHopKiemThuAI] ON [dbo].[KET_QUA_KIEM_THU_AI] ([MaTruongHopKiemThuAI] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_KHACH_HANG_SoDienThoai] ON [dbo].[KHACH_HANG] ([SoDienThoai] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LAN_DU_DOAN_AI_MaLichSuChanDoan_MaPhieu] ON [dbo].[LAN_DU_DOAN_AI] ([MaLichSuChanDoan] ASC, [MaPhieu] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LAN_DU_DOAN_AI_MaBanChupDauVao_MaPhieu] ON [dbo].[LAN_DU_DOAN_AI] ([MaBanChupDauVao] ASC, [MaPhieu] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LAN_DU_DOAN_AI_MaMoHinh] ON [dbo].[LAN_DU_DOAN_AI] ([MaMoHinh] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaPhieu] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaPhieu] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaNguoiDungThucHien] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaNguoiDungThucHien] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaLoiAIXepHang1] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaLoiAIXepHang1] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaLoiAIXepHang2] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaLoiAIXepHang2] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaLoiAIXepHang3] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaLoiAIXepHang3] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaLoiTheoLuat] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaLoiTheoLuat] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaLoiNguoiVanHanhChanDoan] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaLoiNguoiVanHanhChanDoan] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaLoiKyThuatVienBaoCao] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaLoiKyThuatVienBaoCao] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaLoiThucTe] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaLoiThucTe] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_MaNguoiDungBaoCaoKetLuan] ON [dbo].[LICH_SU_CHAN_DOAN] ([MaNguoiDungBaoCaoKetLuan] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaLichSuChanDoan] ON [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] ([MaLichSuChanDoan] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaTrieuChung] ON [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] ([MaTrieuChung] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_TRANG_THAI_MaKyThuatVienThucHien] ON [dbo].[LICH_SU_TRANG_THAI] ([MaKyThuatVienThucHien] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_TRANG_THAI_MaPhieu] ON [dbo].[LICH_SU_TRANG_THAI] ([MaPhieu] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_THONG_TIN_MO_HINH_HOC_MAY_DinhDanhMoHinh_PhienBanMoHinh] ON [dbo].[THONG_TIN_MO_HINH_HOC_MAY] ([DinhDanhMoHinh] ASC, [PhienBanMoHinh] ASC) WHERE ([DinhDanhMoHinh] IS NOT NULL);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_KICH_HOAT_MO_HINH_MaNguoiDungThucHien] ON [dbo].[LICH_SU_KICH_HOAT_MO_HINH] ([MaNguoiDungThucHien] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_KICH_HOAT_MO_HINH_MaMoHinhMoi] ON [dbo].[LICH_SU_KICH_HOAT_MO_HINH] ([MaMoHinhMoi] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_LICH_SU_KICH_HOAT_MO_HINH_MaMoHinhCu] ON [dbo].[LICH_SU_KICH_HOAT_MO_HINH] ([MaMoHinhCu] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_NHOM_SU_CO_AI_TacVu_PhienBanDinhNghiaNhan_MaQuyUocNhom] ON [dbo].[NHOM_SU_CO_AI] ([TacVu] ASC, [PhienBanDinhNghiaNhan] ASC, [MaQuyUocNhom] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_MaLoiChanDoan] ON [dbo].[PHIEU_TIEP_NHAN] ([MaLoiChanDoan] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_MaThietBi] ON [dbo].[PHIEU_TIEP_NHAN] ([MaThietBi] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_MaKyThuatVienDuocGiao] ON [dbo].[PHIEU_TIEP_NHAN] ([MaKyThuatVienDuocGiao] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_PHIEU_TIEP_NHAN_MaNguoiDungLegacy] ON [dbo].[PHIEU_TIEP_NHAN] ([MaNguoiDungLegacy] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_BAN_CHUP_DAU_VAO_CHAN_DOAN_MaNguoiDungGhiNhan] ON [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] ([MaNguoiDungGhiNhan] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_BAN_CHUP_DAU_VAO_CHAN_DOAN_MaPhieu_SoPhienBan] ON [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] ([MaPhieu] ASC, [SoPhienBan] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_SU_DUNG_LINH_KIEN_MaLinhKien] ON [dbo].[SU_DUNG_LINH_KIEN] ([MaLinhKien] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_THIET_BI_MaKhachHang] ON [dbo].[THIET_BI] ([MaKhachHang] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_TRIEU_CHUNG_LOI_MaLoi] ON [dbo].[TRIEU_CHUNG_LOI] ([MaLoi] ASC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_NGUOI_DUNG_TenDangNhap] ON [dbo].[NGUOI_DUNG] ([TenDangNhap] ASC);

GO

ALTER TABLE [dbo].[TAC_VU_HUAN_LUYEN_AI] WITH CHECK ADD CONSTRAINT [FK_TAC_VU_HUAN_LUYEN_AI_MaMoHinh] FOREIGN KEY ([MaMoHinh]) REFERENCES [dbo].[THONG_TIN_MO_HINH_HOC_MAY] ([MaMoHinh]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[CHI_TIET_DU_DOAN_AI] WITH CHECK ADD CONSTRAINT [FK_CHI_TIET_DU_DOAN_AI_MaLanDuDoan_TacVu_PhienBanDinhNghiaNhan] FOREIGN KEY ([MaLanDuDoan], [TacVu], [PhienBanDinhNghiaNhan]) REFERENCES [dbo].[LAN_DU_DOAN_AI] ([MaLanDuDoan], [TacVu], [PhienBanDinhNghiaNhan]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[CHI_TIET_DU_DOAN_AI] WITH CHECK ADD CONSTRAINT [FK_CHI_TIET_DU_DOAN_AI_MaNhomSuCo_TacVu_PhienBanDinhNghiaNhan] FOREIGN KEY ([MaNhomSuCo], [TacVu], [PhienBanDinhNghiaNhan]) REFERENCES [dbo].[NHOM_SU_CO_AI] ([MaNhomSuCo], [TacVu], [PhienBanDinhNghiaNhan]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[CHI_TIET_SUA_CHUA] WITH CHECK ADD CONSTRAINT [FK_CHI_TIET_SUA_CHUA_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[CHI_TIET_SUA_CHUA] WITH CHECK ADD CONSTRAINT [FK_CHI_TIET_SUA_CHUA_MaNguoiDungKiemTraLai] FOREIGN KEY ([MaNguoiDungKiemTraLai]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[DANH_GIA_DICH_VU] WITH CHECK ADD CONSTRAINT [FK_DANH_GIA_DICH_VU_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[GIAI_PHAP] WITH CHECK ADD CONSTRAINT [FK_GIAI_PHAP_MaLoi] FOREIGN KEY ([MaLoi]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE CASCADE ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[HINH_ANH_SU_CO] WITH CHECK ADD CONSTRAINT [FK_HINH_ANH_SU_CO_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[KET_QUA_KIEM_THU_AI] WITH CHECK ADD CONSTRAINT [FK_KET_QUA_KIEM_THU_AI_MaTruongHopKiemThuAI] FOREIGN KEY ([MaTruongHopKiemThuAI]) REFERENCES [dbo].[TRUONG_HOP_KIEM_THU_AI] ([MaTruongHopKiemThuAI]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[KET_QUA_KIEM_THU_AI] WITH CHECK ADD CONSTRAINT [FK_KET_QUA_KIEM_THU_AI_MaMoHinh] FOREIGN KEY ([MaMoHinh]) REFERENCES [dbo].[THONG_TIN_MO_HINH_HOC_MAY] ([MaMoHinh]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LAN_DU_DOAN_AI] WITH CHECK ADD CONSTRAINT [FK_LAN_DU_DOAN_AI_MaLichSuChanDoan_MaPhieu] FOREIGN KEY ([MaLichSuChanDoan], [MaPhieu]) REFERENCES [dbo].[LICH_SU_CHAN_DOAN] ([MaLichSuChanDoan], [MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LAN_DU_DOAN_AI] WITH CHECK ADD CONSTRAINT [FK_LAN_DU_DOAN_AI_MaMoHinh] FOREIGN KEY ([MaMoHinh]) REFERENCES [dbo].[THONG_TIN_MO_HINH_HOC_MAY] ([MaMoHinh]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LAN_DU_DOAN_AI] WITH CHECK ADD CONSTRAINT [FK_LAN_DU_DOAN_AI_MaBanChupDauVao_MaPhieu] FOREIGN KEY ([MaBanChupDauVao], [MaPhieu]) REFERENCES [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] ([MaBanChupDauVao], [MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaLoiAIXepHang1] FOREIGN KEY ([MaLoiAIXepHang1]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaLoiAIXepHang2] FOREIGN KEY ([MaLoiAIXepHang2]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaLoiAIXepHang3] FOREIGN KEY ([MaLoiAIXepHang3]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaLoiTheoLuat] FOREIGN KEY ([MaLoiTheoLuat]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaLoiThucTe] FOREIGN KEY ([MaLoiThucTe]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaLoiNguoiVanHanhChanDoan] FOREIGN KEY ([MaLoiNguoiVanHanhChanDoan]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaLoiKyThuatVienBaoCao] FOREIGN KEY ([MaLoiKyThuatVienBaoCao]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaLichSuChanDoan] FOREIGN KEY ([MaLichSuChanDoan]) REFERENCES [dbo].[LICH_SU_CHAN_DOAN] ([MaLichSuChanDoan]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN_TRIEU_CHUNG] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaTrieuChung] FOREIGN KEY ([MaTrieuChung]) REFERENCES [dbo].[TRIEU_CHUNG] ([MaTrieuChung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaNguoiDungThucHien] FOREIGN KEY ([MaNguoiDungThucHien]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_CHAN_DOAN_MaNguoiDungBaoCaoKetLuan] FOREIGN KEY ([MaNguoiDungBaoCaoKetLuan]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_TRANG_THAI] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_TRANG_THAI_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_TRANG_THAI] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_TRANG_THAI_MaKyThuatVienThucHien] FOREIGN KEY ([MaKyThuatVienThucHien]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_KICH_HOAT_MO_HINH] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_KICH_HOAT_MO_HINH_MaMoHinhMoi] FOREIGN KEY ([MaMoHinhMoi]) REFERENCES [dbo].[THONG_TIN_MO_HINH_HOC_MAY] ([MaMoHinh]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_KICH_HOAT_MO_HINH] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_KICH_HOAT_MO_HINH_MaMoHinhCu] FOREIGN KEY ([MaMoHinhCu]) REFERENCES [dbo].[THONG_TIN_MO_HINH_HOC_MAY] ([MaMoHinh]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[LICH_SU_KICH_HOAT_MO_HINH] WITH CHECK ADD CONSTRAINT [FK_LICH_SU_KICH_HOAT_MO_HINH_MaNguoiDungThucHien] FOREIGN KEY ([MaNguoiDungThucHien]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[PHIEU_TIEP_NHAN] WITH CHECK ADD CONSTRAINT [FK_PHIEU_TIEP_NHAN_MaLoiChanDoan] FOREIGN KEY ([MaLoiChanDoan]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[PHIEU_TIEP_NHAN] WITH CHECK ADD CONSTRAINT [FK_PHIEU_TIEP_NHAN_MaThietBi] FOREIGN KEY ([MaThietBi]) REFERENCES [dbo].[THIET_BI] ([MaThietBi]) ON DELETE CASCADE ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[PHIEU_TIEP_NHAN] WITH CHECK ADD CONSTRAINT [FK_PHIEU_TIEP_NHAN_MaKyThuatVienDuocGiao] FOREIGN KEY ([MaKyThuatVienDuocGiao]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[PHIEU_TIEP_NHAN] WITH CHECK ADD CONSTRAINT [FK_PHIEU_TIEP_NHAN_MaNguoiDungLegacy] FOREIGN KEY ([MaNguoiDungLegacy]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_BAN_CHUP_DAU_VAO_CHAN_DOAN_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [dbo].[PHIEU_TIEP_NHAN] ([MaPhieu]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [FK_BAN_CHUP_DAU_VAO_CHAN_DOAN_MaNguoiDungGhiNhan] FOREIGN KEY ([MaNguoiDungGhiNhan]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[SU_DUNG_LINH_KIEN] WITH CHECK ADD CONSTRAINT [FK_SU_DUNG_LINH_KIEN_MaChiTietSuaChua] FOREIGN KEY ([MaChiTietSuaChua]) REFERENCES [dbo].[CHI_TIET_SUA_CHUA] ([MaChiTietSuaChua]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[SU_DUNG_LINH_KIEN] WITH CHECK ADD CONSTRAINT [FK_SU_DUNG_LINH_KIEN_MaLinhKien] FOREIGN KEY ([MaLinhKien]) REFERENCES [dbo].[LINH_KIEN] ([MaLinhKien]) ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[THIET_BI] WITH CHECK ADD CONSTRAINT [FK_THIET_BI_MaKhachHang] FOREIGN KEY ([MaKhachHang]) REFERENCES [dbo].[KHACH_HANG] ([MaKhachHang]) ON DELETE CASCADE ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[TRIEU_CHUNG_LOI] WITH CHECK ADD CONSTRAINT [FK_TRIEU_CHUNG_LOI_MaLoi] FOREIGN KEY ([MaLoi]) REFERENCES [dbo].[MA_LOI] ([MaLoi]) ON DELETE CASCADE ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[TRIEU_CHUNG_LOI] WITH CHECK ADD CONSTRAINT [FK_TRIEU_CHUNG_LOI_MaTrieuChung] FOREIGN KEY ([MaTrieuChung]) REFERENCES [dbo].[TRIEU_CHUNG] ([MaTrieuChung]) ON DELETE CASCADE ON UPDATE NO ACTION;

GO

ALTER TABLE [dbo].[CHI_TIET_DU_DOAN_AI] WITH CHECK ADD CONSTRAINT [CK_ThuTuDuDoanDuong] CHECK ([ThuTuDuDoan]>(0));

GO

ALTER TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [CK_DoDaiMaBamDauVaoVaDanhMuc] CHECK (len([MaBamDauVaoSHA256])=(64) AND len([MaBamDanhMucSHA256])=(64));

GO

ALTER TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [CK_NgonNguDauVaoHoTro] CHECK ([NgonNgu]=N'und' OR [NgonNgu]=N'en' OR [NgonNgu]=N'vi');

GO

ALTER TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [CK_KhongTuCapDieuKienHuanLuyen] CHECK ([DuDieuKienUngVien]=(0) AND [DuDieuKienHuanLuyen]=(0) AND [TrangThaiQuyenNghienCuu]=N'NOT_GRANTED');

GO

ALTER TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [CK_PhienBanBanChupDuong] CHECK ([SoPhienBan]>(0));

GO

ALTER TABLE [dbo].[BAN_CHUP_DAU_VAO_CHAN_DOAN] WITH CHECK ADD CONSTRAINT [CK_DinhDangTrieuChungJSON] CHECK (isjson([DuLieuTrieuChungJSON])=(1));

GO

ALTER TABLE [dbo].[SU_DUNG_LINH_KIEN] WITH CHECK ADD CONSTRAINT [CK_SoLuongVaDonGiaLinhKien] CHECK ([SoLuong]>(0) AND [DonGiaXuat]>=(0));

GO

ALTER TABLE [dbo].[CHI_TIET_SUA_CHUA] WITH CHECK ADD CONSTRAINT [CK_ChiPhiSuaChuaKhongAm] CHECK ([ChiPhi]>=(0));

GO

ALTER TABLE [dbo].[CHI_TIET_SUA_CHUA] WITH CHECK ADD CONSTRAINT [CK_ThongTinKiemTraLaiHopLe] CHECK ([TrangThaiKiemTraLai] IS NULL OR ([TrangThaiKiemTraLai]=N'INCONCLUSIVE' OR [TrangThaiKiemTraLai]=N'FAIL' OR [TrangThaiKiemTraLai]=N'PASS') AND [ThoiGianKiemTraLaiUTC] IS NOT NULL AND [MaNguoiDungKiemTraLai] IS NOT NULL);

GO

ALTER TABLE [dbo].[KET_QUA_KIEM_THU_AI] WITH CHECK ADD CONSTRAINT [CK_ThamChieuQuyenVaDuDoanKiemThu] CHECK (len([ThamChieuQuyenThucThi])>(0) AND len([PhienBanDinhNghiaKiemThu])>(0) AND isjson([DuLieuDuDoanJSON])=(1));

GO

ALTER TABLE [dbo].[LAN_DU_DOAN_AI] WITH CHECK ADD CONSTRAINT [CK_TrangThaiLanDuDoanHopLe] CHECK ([TrangThaiDuDoan]=N'COMPLETED' AND [MaMoHinh] IS NOT NULL OR [TrangThaiDuDoan]=N'ABSTAINED' AND [LyDoTuChoiDuDoan] IS NOT NULL);

GO

ALTER TABLE [dbo].[LICH_SU_TRANG_THAI] WITH CHECK ADD CONSTRAINT [CK_DuLieuBoSungSuKienJSON] CHECK ([DuLieuBoSungJSON] IS NULL OR isjson([DuLieuBoSungJSON])=(1));

GO

ALTER TABLE [dbo].[LINH_KIEN] WITH CHECK ADD CONSTRAINT [CK_TonKhoVaDonGiaKhongAm] CHECK ([SoLuongTon]>=(0) AND [DonGia]>=(0));

GO
