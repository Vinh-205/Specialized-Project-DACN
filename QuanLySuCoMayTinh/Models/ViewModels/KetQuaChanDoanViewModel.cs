using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Models.ViewModels
{
    public class KetQuaChanDoanViewModel
    {
        public int MaPhieu { get; set; }
        public PhieuTiepNhan? Phieu { get; set; }
        public List<TrieuChung> TrieuChungs { get; set; } = new();
        public List<GoiYLoiViewModel> GoiYLoi { get; set; } = new();
        public List<Services.NhomLoiDuDoan> NhomLoiAI { get; set; } = new();
        public string? MoTaTuDo { get; set; }
    }

    public class GoiYLoiViewModel
    {
        public int MaLoiID { get; set; }
        public string TenLoi { get; set; } = string.Empty;
        public string PhanLoai { get; set; } = string.Empty;
        public int Diem { get; set; }
        public bool IsAIPredicted { get; set; } // Flag to indicate ML.NET prediction
        public string? DoTinCayAI { get; set; } // Mức tin cậy của xếp hạng AI (chỉ mã lỗi đứng đầu): Cao / Trung bình / Thấp, theo biên điểm với mã lỗi đứng thứ hai
        public double? BienDiemAI { get; set; }
        public double? XacSuatDungAI { get; set; } // Xác suất mã lỗi đứng đầu là đúng, do bộ ước lượng tin cậy tính
        public double? XacSuatMaLoiAI { get; set; } // Xác suất mô hình học máy dự đoán trực tiếp mã lỗi (nhãn yếu, chỉ gợi ý)
        public double? DoTuongDongNhomAI { get; set; } // Độ tương đồng nhóm lỗi do mô hình học máy dự đoán (chỉ có khi KTV nhập mô tả)
        public int? SoTrieuChungKhop { get; set; }
        public List<GiaiPhap> GiaiPhaps { get; set; } = new();
    }
}

