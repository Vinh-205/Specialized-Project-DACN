using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Text.Json;

namespace QuanLySuCoMayTinh.Services
{
    public sealed record SongNguKetQua(string PhanLoai, float XacSuatPhanLoai, string MucDo, float XacSuatMucDo, float XacSuatCao = 0);

    /// <summary>
    /// Mô hình song ngữ (Việt/Anh): bộ mã hóa đa ngôn ngữ + hai đầu phân loại, chạy bằng ONNX Runtime ngay trong website
    /// (không gọi dịch vụ AI ngoài). Tệp nằm ở Data/MLModels/SongNgu/. Thiếu tệp thì trả null và hệ thống dùng luật từ khóa.
    /// Chỉ dùng để GỢI Ý cho KTV; không tự quyết định mã lỗi.
    /// </summary>
    public sealed class SongNguClassifier : IDisposable
    {
        private readonly InferenceSession? _session;
        private readonly SongNguTokenizer? _tokenizer;
        private readonly string[] _nhanPhanLoai = Array.Empty<string>(), _nhanMucDo = Array.Empty<string>();
        public string[] NhanNhomLoi { get; private set; } = Array.Empty<string>();
        /// <summary>Tên hiển thị tiếng Việt của các nhóm lỗi chi tiết (30 nhóm) do mô hình học máy dự đoán.</summary>
        public static readonly Dictionary<string, string> TenNhomLoi = new()
        {
            ["o_cung"] = "Ổ cứng", ["ssd"] = "SSD", ["ram"] = "RAM/bộ nhớ", ["ban_phim"] = "Bàn phím", ["man_hinh"] = "Màn hình/hiển thị", ["usb"] = "USB", ["am_thanh"] = "Âm thanh", ["cpu"] = "CPU",
            ["nguon_pin"] = "Nguồn/Pin", ["wifi"] = "WiFi", ["mang"] = "Mạng", ["router"] = "Router/modem", ["dns"] = "DNS", ["vpn"] = "VPN", ["khoi_dong"] = "Khởi động hệ điều hành",
            ["card_do_hoa"] = "Card đồ họa", ["mainboard"] = "Mainboard/BIOS", ["chuot_touchpad"] = "Chuột/Touchpad", ["bluetooth"] = "Bluetooth", ["windows_update"] = "Windows Update", ["driver"] = "Driver",
            ["sao_luu"] = "Sao lưu/khôi phục dữ liệu", ["chay_cham"] = "Máy chạy chậm", ["mat_khau"] = "Mật khẩu/đăng nhập", ["explorer"] = "Windows Explorer/tệp", ["may_in"] = "Máy in", ["virus"] = "Virus/mã độc", ["cai_dat"] = "Cài đặt phần mềm",
            ["excel"] = "Excel", ["word"] = "Word", ["outlook"] = "Outlook/email", ["trinh_duyet"] = "Trình duyệt"
        };
        public bool SanSang => _session != null && _tokenizer != null;
        public string TenMoHinh { get; } = "";

        public SongNguClassifier(string thuMuc, string tenFileOnnx = "song_ngu_int8.onnx")
        {
            try
            {
                var onnx = Path.Combine(thuMuc, tenFileOnnx);
                var tok = Path.Combine(thuMuc, "tokenizer.json");
                var nhan = Path.Combine(thuMuc, "nhan.json");
                if (!File.Exists(onnx) || !File.Exists(tok) || !File.Exists(nhan)) return;
                using (var doc = JsonDocument.Parse(File.ReadAllText(nhan)))
                {
                    _nhanPhanLoai = doc.RootElement.GetProperty("phan_loai").EnumerateArray().Select(x => x.GetString()!).ToArray();
                    _nhanMucDo = doc.RootElement.GetProperty("muc_do").EnumerateArray().Select(x => x.GetString()!).ToArray();
                    if (doc.RootElement.TryGetProperty("nhom_loi", out var nl)) NhanNhomLoi = nl.EnumerateArray().Select(x => x.GetString()!).ToArray();
                }
                _tokenizer = new SongNguTokenizer(tok);
                _session = new InferenceSession(onnx);
                TenMoHinh = tenFileOnnx;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[AI] Không nạp được mô hình song ngữ: " + ex.Message);
                _session = null;
            }
        }

        private IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? Chay(string text)
        {
            var ids = _tokenizer!.Encode(text.Length > 2000 ? text[..2000] : text);
            var idT = new DenseTensor<long>(new[] { 1, ids.Length });
            var maskT = new DenseTensor<long>(new[] { 1, ids.Length });
            for (int i = 0; i < ids.Length; i++) { idT[0, i] = ids[i]; maskT[0, i] = 1; }
            return _session!.Run(new[] { NamedOnnxValue.CreateFromTensor("input_ids", idT), NamedOnnxValue.CreateFromTensor("attention_mask", maskT) });
        }

        public SongNguKetQua? Predict(string? text)
        {
            if (!SanSang || string.IsNullOrWhiteSpace(text)) return null;
            try
            {
                using var res = Chay(text)!;
                var a = res.First(r => r.Name == "xac_suat_phan_loai").AsEnumerable<float>().ToArray();
                var b = res.First(r => r.Name == "xac_suat_uu_tien").AsEnumerable<float>().ToArray();
                int ia = Array.IndexOf(a, a.Max()), ib = Array.IndexOf(b, b.Max());
                int ic = Array.IndexOf(_nhanMucDo, "Cao"); return new SongNguKetQua(_nhanPhanLoai[ia], a[ia], _nhanMucDo[ib], b[ib], ic >= 0 ? b[ic] : 0);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[AI] Lỗi suy luận mô hình song ngữ: " + ex.Message);
                return null;
            }
        }

        /// <summary>Phân bố xác suất 19 nhóm lỗi chi tiết (đầu học máy huấn luyện trên câu hỏi thật của người dùng Super User) cho một câu mô tả.</summary>
        public float[]? PhanBoNhomLoi(string? text)
        {
            if (!SanSang || NhanNhomLoi.Length == 0 || string.IsNullOrWhiteSpace(text)) return null;
            try
            {
                using var res = Chay(text)!;
                var r = res.FirstOrDefault(x => x.Name == "xac_suat_nhom_loi");
                return r?.AsEnumerable<float>().ToArray();
            }
            catch (Exception ex) { Console.Error.WriteLine("[AI] Lỗi dự đoán nhóm lỗi: " + ex.Message); return null; }
        }

        /// <summary>Vector câu đã chuẩn hóa L2 (384 chiều) từ bộ mã hóa đa ngôn ngữ; dùng cho so khớp ngữ nghĩa Việt–Anh.</summary>
        public float[]? Embed(string? text)
        {
            if (!SanSang || string.IsNullOrWhiteSpace(text)) return null;
            try
            {
                using var res = Chay(text)!;
                return res.First(r => r.Name == "vector_cau").AsEnumerable<float>().ToArray();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[AI] Lỗi tạo vector câu: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Kết hợp luật và mô hình. Phân loại: lấy mô hình (xem ghi chú trong mã).
        /// Ưu tiên: luật có tín hiệu rõ (Cao/Thấp/nhẹ) thì giữ luật; chỉ khi luật KHÔNG có tín hiệu (mặc định Trung bình, ví dụ câu tiếng Anh)
        /// mới lấy mô hình khi xác suất ≥ 0,5. Quy tắc nâng lên Cao khi mô hình báo Cao được chọn theo nguyên tắc an toàn (xem báo cáo mục 31).
        /// </summary>
        /// <summary>Ngưỡng báo Cao chọn bằng kiểm định chéo 5 lần trên dữ liệu huấn luyện (không dùng heldout3): heldout3 acc 62,5%→65,0%, bắt Cao 6→8/15, Thấp bị báo Cao ≤ 1.</summary>
        public const float NguongCao = 0.35f;

        public static YeuCauGoiY KetHop(YeuCauGoiY luat, SongNguKetQua? ml, bool baoCaoCaoNeuMotTrongHai = true)
        {
            if (ml == null) return luat;
            var ly = new List<string>(luat.LyDo);
            var pl = luat.PhanLoai; var md = luat.MucDoUuTien;
            // Phân loại: luôn lấy mô hình khi có (đúng hơn luật từ khóa trên cả hai tập giữ riêng heldout2 và heldout3); luật chỉ là phương án dự phòng khi thiếu mô hình.
            if (ml.PhanLoai != pl) ly.RemoveAll(x => x.StartsWith("Phân loại \""));
            if (ml.PhanLoai != pl) ly.Add($"Mô hình song ngữ đề xuất phân loại \"{ml.PhanLoai}\" ({ml.XacSuatPhanLoai:P0})");
            pl = ml.PhanLoai;
            // An toàn: bỏ sót ca khẩn tốn kém hơn báo nhầm, nên mô hình báo Cao (xác suất ≥ 0,5) thì nâng lên Cao dù luật không thấy tín hiệu.
            if (baoCaoCaoNeuMotTrongHai && md != "Cao" && ml.XacSuatCao >= NguongCao)
            {
                ly.Add($"Mô hình song ngữ cảnh báo ưu tiên Cao ({ml.XacSuatCao:P0}, ngưỡng {NguongCao:P0}) — KTV kiểm tra lại");
                return new YeuCauGoiY(pl, "Cao", ly, "Luật từ khóa + mô hình song ngữ ONNX (chưa huấn luyện từ ca thật)");
            }
            bool luatKhongTinHieu = luat.LyDo.Any(x => x.Contains("không có tín hiệu"));
            if (luatKhongTinHieu && ml.XacSuatMucDo >= 0.5f) { if (ml.MucDo != md) ly.Add($"Mô hình song ngữ đề xuất ưu tiên \"{ml.MucDo}\" ({ml.XacSuatMucDo:P0})"); md = ml.MucDo; }
            return new YeuCauGoiY(pl, md, ly, "Luật từ khóa + mô hình song ngữ ONNX (chưa huấn luyện từ ca thật)");
        }

        public void Dispose() => _session?.Dispose();
    }
}
