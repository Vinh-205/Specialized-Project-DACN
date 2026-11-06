using System.Text.Json;

namespace QuanLySuCoMayTinh.Services
{
    public sealed record KhaNangSuaDuDoan(string Nhan, string NhanHienThi, double XacSuat, IReadOnlyList<(string Nhan, double XacSuat)> TatCa);

    /// <summary>
    /// Mô hình học máy dự đoán KẾT QUẢ SỬA (Fixed / Repairable / End of life) từ mô tả sự cố: hồi quy logistic trên vector câu song ngữ,
    /// huấn luyện trên ~9.100 mô tả sửa chữa thật của Open Repair Alliance (máy tính xách tay/để bàn). Bản chạy: mạng MLP acc 64,8%, Macro-F1 0,574 (hồi quy logistic cũ 59,8%/0,551) trên 2.599 mẫu giữ riêng.
    /// Mô tả huấn luyện bằng tiếng Anh; câu tiếng Việt đi qua bộ mã hóa đa ngôn ngữ nhưng CHƯA được đo riêng. Chỉ GỢI Ý cho KTV.
    /// </summary>
    public sealed class KetQuaSuaClassifier
    {
        private readonly SongNguClassifier _sn;
        private readonly string[] _nhan = Array.Empty<string>();
        private readonly float[][] _w = Array.Empty<float[]>();
        private readonly float[] _b = Array.Empty<float>();
        private float[][]? _w1, _w2, _w3; private float[]? _b1, _b2, _b3; // mạng MLP (nếu có ket_qua_sua_mlp.json)
        public string KieuMoHinh { get; private set; } = "hồi quy logistic";
        public bool SanSang => _sn.SanSang && _nhan.Length > 0;
        public static readonly Dictionary<string, string> TenHienThi = new()
        {
            ["Fixed"] = "Nhiều khả năng sửa được ngay", ["Repairable"] = "Sửa được nhưng cần thêm điều kiện (linh kiện, thời gian)", ["End of life"] = "Khó sửa / đã hết vòng đời"
        };

        public KetQuaSuaClassifier(SongNguClassifier sn, string thuMuc)
        {
            _sn = sn;
            try
            {
                var mlp = Path.Combine(thuMuc, "ket_qua_sua_mlp.json");
                if (File.Exists(mlp))
                {
                    using var d2 = JsonDocument.Parse(File.ReadAllText(mlp)); var r2 = d2.RootElement;
                    float[][] M(string k) => r2.GetProperty(k).EnumerateArray().Select(row => row.EnumerateArray().Select(x => x.GetSingle()).ToArray()).ToArray();
                    float[] V(string k) => r2.GetProperty(k).EnumerateArray().Select(x => x.GetSingle()).ToArray();
                    _nhan = r2.GetProperty("nhan").EnumerateArray().Select(x => x.GetString()!).ToArray();
                    _w1 = M("w1"); _w2 = M("w2"); _w3 = M("w3"); _b1 = V("b1"); _b2 = V("b2"); _b3 = V("b3"); KieuMoHinh = "mạng nơ-ron MLP"; return;
                }
                var f = Path.Combine(thuMuc, "ket_qua_sua.json");
                if (!File.Exists(f)) return;
                using var doc = JsonDocument.Parse(File.ReadAllText(f));
                var r = doc.RootElement;
                _nhan = r.GetProperty("nhan").EnumerateArray().Select(x => x.GetString()!).ToArray();
                _w = r.GetProperty("he_so").EnumerateArray().Select(row => row.EnumerateArray().Select(x => x.GetSingle()).ToArray()).ToArray();
                _b = r.GetProperty("he_so_chan").EnumerateArray().Select(x => x.GetSingle()).ToArray();
            }
            catch (Exception ex) { Console.Error.WriteLine("[AI] Không nạp được mô hình kết quả sửa: " + ex.Message); _nhan = Array.Empty<string>(); }
        }

        private static double[] Tang(double[] x, float[][] w, float[] b) { var o = new double[w.Length]; for (int k = 0; k < w.Length; k++) { double s = b[k]; var r = w[k]; for (int i = 0; i < x.Length && i < r.Length; i++) s += r[i] * x[i]; o[k] = s; } return o; }
        private static double[] Gelu(double[] x) => x.Select(v => 0.5 * v * (1.0 + Erf(v / Math.Sqrt(2.0)))).ToArray();
        // erf theo Abramowitz–Stegun 7.1.26 (sai số < 1,5e-7), đủ cho GELU
        private static double Erf(double x) { double t = 1.0 / (1.0 + 0.3275911 * Math.Abs(x)); double y = 1 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) * t - 0.284496736) * t + 0.254829592) * t * Math.Exp(-x * x); return x >= 0 ? y : -y; }

        public KhaNangSuaDuDoan? DuDoan(string? moTa)
        {
            if (!SanSang || string.IsNullOrWhiteSpace(moTa)) return null;
            var v = _sn.Embed(moTa.Length > 300 ? moTa[..300] : moTa); if (v == null) return null;
            // Hồi quy logistic đa lớp: softmax trên điểm tuyến tính. Với 3 lớp, scikit-learn cũng dùng softmax (multinomial).
            double[] z;
            if (_w1 != null) z = Tang(Gelu(Tang(Gelu(Tang(v.Select(x => (double)x).ToArray(), _w1, _b1!)), _w2!, _b2!)), _w3!, _b3!);
            else { z = new double[_nhan.Length]; for (int k = 0; k < z.Length; k++) { double s = _b[k]; var w = _w[k]; for (int i = 0; i < v.Length && i < w.Length; i++) s += w[i] * v[i]; z[k] = s; } }
            var mx = z.Max(); var e = z.Select(x => Math.Exp(x - mx)).ToArray(); var t = e.Sum();
            var tat = _nhan.Select((n, i) => (n, e[i] / t)).OrderByDescending(x => x.Item2).ToList();
            return new KhaNangSuaDuDoan(tat[0].n, TenHienThi.GetValueOrDefault(tat[0].n, tat[0].n), tat[0].Item2, tat);
        }
    }
}
