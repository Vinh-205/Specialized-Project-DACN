using System.Text.Json;

namespace QuanLySuCoMayTinh.Services
{
    /// <summary>
    /// Mô hình học máy dự đoán TRỰC TIẾP mã lỗi từ mô tả tự do: hồi quy logistic (47 lớp) trên vector câu 384 chiều của bộ mã hóa song ngữ.
    /// Hệ số đọc từ JSON trong thư mục MaLoiTrucTiep của kho mô hình (AI:ModelRoot). Nhãn huấn luyện là nhãn YẾU (chưa người xác minh) nên chỉ GỢI Ý.
    /// Thiếu tệp thì không hoạt động và hệ thống giữ cách xếp hạng cũ.
    /// </summary>
    public sealed class MaLoiTrucTiepClassifier
    {
        private readonly SongNguClassifier _sn;
        private readonly int[] _maLoi = Array.Empty<int>();
        private readonly string[] _tenLoi = Array.Empty<string>();
        private readonly float[][] _w = Array.Empty<float[]>();
        private readonly float[] _b = Array.Empty<float>();
        private double[]? _tcHeSo; private double _tcChan; public double NguongCao { get; private set; } = 0.75; public double NguongTrungBinh { get; private set; } = 0.5;
        public bool CoUocLuongTinCay => _tcHeSo != null;
        public bool SanSang => _sn.SanSang && _maLoi.Length > 0;

        /// <summary>
        /// Ước lượng xác suất "mã lỗi đứng đầu là đúng" bằng hồi quy logistic 8 đặc trưng (biên điểm, đồng ý, xác suất trực tiếp, độ tương đồng nhóm, số triệu chứng khớp...),
        /// huấn luyện trên 889 bài thật giữ riêng (che dấu hiệu); kiểm trên 890 bài khác: ngưỡng 0,75 phủ 18%, đúng 82,8%.
        /// </summary>
        public double? UocLuongXacSuatDung(double[] dacTrung) { if (_tcHeSo == null || dacTrung.Length != _tcHeSo.Length) return null; double z = _tcChan; for (int i = 0; i < dacTrung.Length; i++) z += _tcHeSo[i] * dacTrung[i]; return 1.0 / (1.0 + Math.Exp(-z)); }

        public MaLoiTrucTiepClassifier(SongNguClassifier sn, string thuMuc)
        {
            _sn = sn;
            try
            {
                var tc = Path.Combine(thuMuc, "tin_cay.json");
                if (File.Exists(tc))
                {
                    using var dt = JsonDocument.Parse(File.ReadAllText(tc)); var rt = dt.RootElement;
                    _tcHeSo = rt.GetProperty("he_so").EnumerateArray().Select(x => x.GetDouble()).ToArray(); _tcChan = rt.GetProperty("he_so_chan").GetDouble();
                    NguongCao = rt.GetProperty("nguong_cao").GetDouble(); NguongTrungBinh = rt.GetProperty("nguong_trung_binh").GetDouble();
                }
                var f = Path.Combine(thuMuc, "ma_loi_truc_tiep.json");
                if (!File.Exists(f)) return;
                using var doc = JsonDocument.Parse(File.ReadAllText(f));
                var r = doc.RootElement;
                _maLoi = r.GetProperty("ma_loi").EnumerateArray().Select(x => x.GetInt32()).ToArray();
                // Ghép theo TÊN mã lỗi: số mã lỗi trong database có thể lệch với danh mục lúc huấn luyện.
                _tenLoi = r.GetProperty("ten_loi").EnumerateArray().Select(x => (x.GetString() ?? "").Trim()).ToArray();
                _w = r.GetProperty("he_so").EnumerateArray().Select(row => row.EnumerateArray().Select(x => x.GetSingle()).ToArray()).ToArray();
                _b = r.GetProperty("he_so_chan").EnumerateArray().Select(x => x.GetSingle()).ToArray();
            }
            catch (Exception ex) { Console.Error.WriteLine("[AI] Không nạp được mô hình mã lỗi trực tiếp: " + ex.Message); _maLoi = Array.Empty<int>(); }
        }

        /// <summary>Xác suất theo từng mã lỗi (TÊN mã lỗi → xác suất); null nếu không có mô hình hoặc mô tả.</summary>
        public Dictionary<string, double>? DuDoan(string? moTa)
        {
            if (!SanSang) return null;
            var v = _sn.Embed(moTa); if (v == null) return null;
            var z = new double[_maLoi.Length];
            for (int k = 0; k < z.Length; k++) { double s = _b[k]; var w = _w[k]; for (int i = 0; i < v.Length && i < w.Length; i++) s += w[i] * v[i]; z[k] = s; }
            var mx = z.Max(); var e = z.Select(x => Math.Exp(x - mx)).ToArray(); var tong = e.Sum();
            var kq = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            for (int k = 0; k < z.Length; k++) kq[_tenLoi[k]] = e[k] / tong;
            return kq;
        }
    }
}
