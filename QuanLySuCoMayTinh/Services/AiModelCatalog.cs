using System.Globalization;
using System.Text.Json;

namespace QuanLySuCoMayTinh.Services
{
    public sealed record ChiSoMoHinh(string Nhan, string GiaTri);

    /// <summary>Một mô hình trong bộ AI thật của hệ thống (đọc từ tệp trong kho mô hình AI:ModelRoot).</summary>
    public sealed record MoHinhAI(string Ten, string VaiTro, string Loai, string Tep, bool TonTai, long DungLuong, DateTime? NgaySua,
        string DuLieu, List<ChiSoMoHinh> ChiSo, string GhiChu);

    /// <summary>
    /// Danh mục bộ mô hình AI đang dùng thật. Các mô hình này là tệp (ONNX/JSON), không nằm trong bảng ML_MODEL_INFO (chỉ dành cho ML.NET),
    /// nên trang Mô hình AI đọc trực tiếp tệp và các thông số đo được lưu kèm (nguồn dữ liệu, độ chính xác, ngưỡng tin cậy). Không thêm bảng hay cột nào.
    /// </summary>
    public static class AiModelCatalog
    {
        private static readonly object Khoa = new();
        private static (string Khoa, DateTime Ngay, JsonDocument Doc)? _cacheKetQuaSua;
        private static (string Khoa, DateTime Ngay, JsonDocument Doc)? _cacheMaLoi;

        public static List<MoHinhAI> Doc(string goc)
        {
            var kq = new List<MoHinhAI>();
            kq.Add(SongNgu(Path.Combine(goc, "SongNgu")));
            kq.Add(MaLoiTrucTiep(Path.Combine(goc, "MaLoiTrucTiep")));
            kq.Add(KetQuaSua(Path.Combine(goc, "KetQuaSua")));
            return kq;
        }

        private static (bool Co, long Size, DateTime? Ngay) Tep(string duongDan)
        {
            try { var f = new FileInfo(duongDan); return f.Exists ? (true, f.Length, f.LastWriteTime) : (false, 0, null); } catch { return (false, 0, null); }
        }

        private static string Pct(double v) => (v * 100).ToString("0.#", CultureInfo.GetCultureInfo("vi-VN")) + "%";

        private static JsonDocument? DocJson(string duongDan, ref (string Khoa, DateTime Ngay, JsonDocument Doc)? cache)
        {
            try
            {
                var f = new FileInfo(duongDan);
                if (!f.Exists) return null;
                lock (Khoa)
                {
                    if (cache is { } c && c.Khoa == duongDan && c.Ngay == f.LastWriteTimeUtc) return c.Doc;
                    var doc = JsonDocument.Parse(File.ReadAllText(duongDan));
                    cache = (duongDan, f.LastWriteTimeUtc, doc);
                    return doc;
                }
            }
            catch { return null; }
        }

        private static MoHinhAI SongNgu(string thuMuc)
        {
            var onnx = Tep(Path.Combine(thuMuc, "song_ngu_int8.onnx"));
            var chiSo = new List<ChiSoMoHinh>();
            try
            {
                var nhan = Path.Combine(thuMuc, "nhan.json");
                if (File.Exists(nhan))
                {
                    using var d = JsonDocument.Parse(File.ReadAllText(nhan));
                    var r = d.RootElement;
                    if (r.TryGetProperty("phan_loai", out var a)) chiSo.Add(new("Phân loại yêu cầu", a.GetArrayLength() + " nhóm"));
                    if (r.TryGetProperty("muc_do", out var b)) chiSo.Add(new("Mức ưu tiên", b.GetArrayLength() + " mức"));
                    if (r.TryGetProperty("nhom_loi", out var c)) chiSo.Add(new("Nhóm lỗi", c.GetArrayLength() + " nhóm"));
                }
            }
            catch { /* bỏ qua, chỉ thiếu phần thông số */ }
            return new MoHinhAI("Bộ mã hóa song ngữ (ONNX)", "Hiểu mô tả tiếng Việt/Anh: phân loại yêu cầu, mức ưu tiên, nhóm lỗi, gợi ý triệu chứng",
                "Học sâu (MiniLM đa ngôn ngữ, lượng tử int8)", "SongNgu/song_ngu_int8.onnx", onnx.Co, onnx.Size, onnx.Ngay,
                "Bộ mã hóa paraphrase-multilingual-MiniLM-L12-v2 giữ nguyên; các đầu phân loại học trên câu hỏi kỹ thuật thật và mô tả tự soạn",
                chiSo, "Chỉ số đo của đầu phân loại nằm trong báo cáo kiểm thử, chưa lưu kèm tệp mô hình.");
        }

        private static MoHinhAI MaLoiTrucTiep(string thuMuc)
        {
            var tep = Tep(Path.Combine(thuMuc, "ma_loi_truc_tiep.json"));
            var chiSo = new List<ChiSoMoHinh>();
            string duLieu = "—";
            try
            {
                var root = DocJson(Path.Combine(thuMuc, "ma_loi_truc_tiep.json"), ref _cacheMaLoi)?.RootElement;
                if (root is { } r)
                {
                    if (r.TryGetProperty("du_lieu", out var dl)) duLieu = dl.GetString() ?? duLieu;
                    if (r.TryGetProperty("ma_loi", out var ml)) chiSo.Add(new("Số mã lỗi", ml.GetArrayLength().ToString()));
                    if (r.TryGetProperty("do_trong_mien_nhan_yeu", out var nm) && nm.TryGetProperty("acc", out var acc))
                        chiSo.Add(new("Đúng Top-1 trên nhãn yếu", Pct(acc.GetDouble()) + (nm.TryGetProperty("mau", out var mau) ? $" ({mau.GetInt32().ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))} bài)" : "")));
                    if (r.TryGetProperty("do_doc_lap", out var dlap) && dlap.TryGetProperty("phep_do", out var pd) && dlap.TryGetProperty("top1", out var t1))
                        chiSo.Add(new("Đúng Top-1 trên bộ tự soạn", $"{t1.GetInt32()}/{pd.GetInt32()} ({Pct(t1.GetInt32() / (double)pd.GetInt32())})"));
                }
                var tc = DocTinCay(Path.Combine(thuMuc, "tin_cay.json"));
                chiSo.AddRange(tc);
            }
            catch { /* bỏ qua */ }
            return new MoHinhAI("Mô hình mã lỗi trực tiếp + ước lượng độ tin cậy", "Dự đoán thẳng mã lỗi từ mô tả; ước lượng xác suất đúng để quyết định AI có được tự chốt hay không",
                "Hồi quy logistic trên vector câu (nhãn yếu)", "MaLoiTrucTiep/ma_loi_truc_tiep.json, tin_cay.json", tep.Co, tep.Size, tep.Ngay,
                duLieu, chiSo, "Nhãn huấn luyện là nhãn YẾU (suy từ dấu hiệu văn bản), chưa có nhãn mã lỗi do người xác minh; tự chốt chỉ khi mức tin cậy Cao.");
        }

        private static List<ChiSoMoHinh> DocTinCay(string duongDan)
        {
            var kq = new List<ChiSoMoHinh>();
            if (!File.Exists(duongDan)) return kq;
            using var d = JsonDocument.Parse(File.ReadAllText(duongDan));
            var r = d.RootElement;
            if (r.TryGetProperty("nguong_cao", out var nc)) kq.Add(new("Ngưỡng tin cậy Cao (tự chốt)", "≥ " + Pct(nc.GetDouble())));
            if (r.TryGetProperty("B", out var b) && b.TryGetProperty("phu", out var phu) && b.TryGetProperty("dung", out var dung))
                kq.Add(new("Mức Cao trên bài thật giữ riêng", $"đúng {Pct(dung.GetDouble())}, phủ {Pct(phu.GetDouble())} số ca"));
            if (r.TryGetProperty("tap80", out var t) && t.TryGetProperty("phu", out var p80) && t.TryGetProperty("dung", out var d80))
                kq.Add(new("Mức Cao trên 80 câu tự soạn", $"đúng {Pct(d80.GetDouble())}, phủ {Pct(p80.GetDouble())} số ca"));
            return kq;
        }

        private static MoHinhAI KetQuaSua(string thuMuc)
        {
            var mlp = Path.Combine(thuMuc, "ket_qua_sua_mlp.json");
            var duongDan = File.Exists(mlp) ? mlp : Path.Combine(thuMuc, "ket_qua_sua.json");
            var tep = Tep(duongDan);
            var chiSo = new List<ChiSoMoHinh>();
            string duLieu = "—", kieu = "Hồi quy logistic";
            try
            {
                var root = DocJson(duongDan, ref _cacheKetQuaSua)?.RootElement;
                if (root is { } r)
                {
                    if (r.TryGetProperty("du_lieu", out var dl)) duLieu = dl.GetString() ?? duLieu;
                    if (r.TryGetProperty("kieu", out var k) && k.GetString() == "mlp") kieu = "Mạng nơ-ron nhiều lớp (MLP, học sâu)";
                    if (r.TryGetProperty("nhan", out var nh)) chiSo.Add(new("Nhãn", string.Join(" / ", nh.EnumerateArray().Select(x => x.GetString()))));
                    if (r.TryGetProperty("test", out var t))
                    {
                        if (t.TryGetProperty("acc", out var acc)) chiSo.Add(new("Đúng trên tập kiểm tra", Pct(acc.GetDouble()) + (t.TryGetProperty("mau", out var m) ? $" ({m.GetInt32().ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))} mẫu)" : "")));
                        if (t.TryGetProperty("macro_f1", out var f1)) chiSo.Add(new("Macro F1", f1.GetDouble().ToString("0.000", CultureInfo.GetCultureInfo("vi-VN"))));
                    }
                }
            }
            catch { /* bỏ qua */ }
            return new MoHinhAI("Mô hình dự đoán khả năng sửa", "Hiện ở trang chi tiết phiếu: sửa được / có thể sửa / hết vòng đời",
                kieu, "KetQuaSua/" + Path.GetFileName(duongDan), tep.Co, tep.Size, tep.Ngay, duLieu, chiSo,
                "Học trên dữ liệu sửa chữa tiếng Anh (Open Repair Alliance); chưa đo trên ca tiếng Việt của hệ thống.");
        }
    }
}
