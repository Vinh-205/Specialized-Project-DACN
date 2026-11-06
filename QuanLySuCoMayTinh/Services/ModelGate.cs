using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Services
{
    /// <summary>Một chốt của cổng duyệt khi kích hoạt mô hình. Trang thai: "ok" (đạt), "bad" (không đạt), "skip" (bỏ qua / chưa đo được).</summary>
    public sealed record CongChot(string Ten, string ChiTiet, string TrangThai)
    {
        public bool Dat => TrangThai != "bad";
    }

    /// <summary>
    /// Quy tắc duy nhất cho cổng duyệt mô hình ML.NET (dùng chung cho trang Mô hình AI, kích hoạt và chẩn đoán):
    /// mô hình KHỞI TẠO (dữ liệu tổng hợp) được kích hoạt nhưng CHỈ HIỂN THỊ; mô hình thường phải có ≥ 100 bản ghi thực,
    /// Micro ≥ 60%, Macro ≥ 50% và không kém Rule quá 10 điểm. Mô hình CHƯA KIỂM CHỨNG (khởi tạo hoặc dưới 100 bản ghi thực) không được
    /// cộng điểm hay chèn gợi ý vào kết quả chẩn đoán, dù đang ở trạng thái "đang kích hoạt".
    /// </summary>
    public static class ModelGate
    {
        public const int MinBanGhiThuc = 100;
        public const double MaxKemRule = 10;
        public const double MinMicro = 0.6;
        public const double MinMacro = 0.5;

        public static bool LaKhoiTao(MLModelInfo m) => m.PhienBanBoDuLieu != null && m.PhienBanBoDuLieu.StartsWith("KHOI_TAO_TONG_HOP");

        /// <summary>Mô hình chưa đủ bằng chứng để ảnh hưởng thứ hạng chẩn đoán.</summary>
        public static bool ChuaKiemChung(MLModelInfo m) => LaKhoiTao(m) || m.SoBanGhiThucTe < MinBanGhiThuc;

        public static List<CongChot> TinhCong(MLModelInfo m)
        {
            bool kt = LaKhoiTao(m);
            var ds = new List<CongChot>
            {
                kt ? new("Dữ liệu thực tế", "Bỏ qua với mô hình khởi tạo", "skip")
                   : new("Dữ liệu thực tế", $"Cần ≥ {MinBanGhiThuc} bản ghi thật (hiện {m.SoBanGhiThucTe})", m.SoBanGhiThucTe >= MinBanGhiThuc ? "ok" : "bad")
            };
            if (m.DoChinhXacRule <= 0)
                ds.Add(new("Không kém luật quá 10 điểm", "Chưa đo được Rule Engine (0%), chốt này không kiểm chứng được", "skip"));
            else
                ds.Add(new("Không kém luật quá 10 điểm", $"AI {m.DoChinhXacAI:0.0}% so với Rule {m.DoChinhXacRule:0.0}%", m.DoChinhXacAI >= m.DoChinhXacRule - MaxKemRule ? "ok" : "bad"));
            ds.Add(kt ? new("Micro Accuracy ≥ 60%", "Bỏ qua với mô hình khởi tạo", "skip")
                      : new("Micro Accuracy ≥ 60%", $"Hiện {Math.Round(m.DoChinhXacMicro * 100, 1)}%", m.DoChinhXacMicro >= MinMicro ? "ok" : "bad"));
            ds.Add(kt ? new("Macro Accuracy ≥ 50%", "Bỏ qua với mô hình khởi tạo", "skip")
                      : new("Macro Accuracy ≥ 50%", $"Hiện {Math.Round(m.DoChinhXacMacro * 100, 1)}%", m.DoChinhXacMacro >= MinMacro ? "ok" : "bad"));
            return ds;
        }

        public static bool QuaCong(IEnumerable<CongChot> cong) => cong.All(c => c.Dat);
    }
}
