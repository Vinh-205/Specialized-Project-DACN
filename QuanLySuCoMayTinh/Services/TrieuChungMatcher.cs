namespace QuanLySuCoMayTinh.Services
{
    public sealed record TrieuChungKhop(int MaTC, string Ten, float DoTuongDong);

    /// <summary>
    /// Khớp mô tả sự cố tự do (tiếng Việt hoặc tiếng Anh) với danh mục TRIEU_CHUNG của hệ thống bằng độ tương đồng cosine
    /// giữa các vector câu. Chỉ GỢI Ý để KTV tích chọn; mã lỗi vẫn do Rule chấm điểm từ các triệu chứng KTV xác nhận.
    /// Tham số đặt trước khi đo: ngưỡng 0,45, tối đa 4 triệu chứng, luôn lấy ít nhất 1.
    /// </summary>
    public sealed class TrieuChungMatcher
    {
        public const float Nguong = 0.45f; public const int ToiDa = 4;
        private readonly SongNguClassifier _sn;
        private string _dauVan = ""; private List<(int Ma, string Ten, float[] Vec)> _cache = new();
        private readonly object _khoa = new();
        public TrieuChungMatcher(SongNguClassifier sn) { _sn = sn; }
        public bool SanSang => _sn.SanSang;

        public List<TrieuChungKhop> GoiY(string? moTa, IReadOnlyList<(int Ma, string Ten, string? MoTa)> danhMuc)
        {
            var kq = new List<TrieuChungKhop>();
            if (!SanSang || string.IsNullOrWhiteSpace(moTa) || danhMuc.Count == 0) return kq;
            var q = _sn.Embed(moTa); if (q == null) return kq;
            List<(int Ma, string Ten, float[] Vec)> dm;
            lock (_khoa)
            {
                var dauVan = string.Join("\u0001", danhMuc.Select(x => $"{x.Ma}\u0002{x.Ten}\u0002{x.MoTa}"));
                if (dauVan != _dauVan)
                {
                    var moi = new List<(int, string, float[])>();
                    foreach (var t in danhMuc)
                    {
                        var van = string.IsNullOrWhiteSpace(t.MoTa) ? t.Ten : t.Ten + ". " + t.MoTa;
                        var v = _sn.Embed(van); if (v != null) moi.Add((t.Ma, t.Ten, v));
                    }
                    _cache = moi; _dauVan = dauVan;
                }
                dm = _cache;
            }
            var diem = dm.Select(t => (t.Ma, t.Ten, S: Dot(q, t.Vec))).OrderByDescending(x => x.S).ToList();
            for (int i = 0; i < diem.Count && i < ToiDa; i++)
                if (i == 0 || diem[i].S >= Nguong) kq.Add(new TrieuChungKhop(diem[i].Ma, diem[i].Ten, diem[i].S));
            return kq;
        }

        private static float Dot(float[] a, float[] b) { float s = 0; for (int i = 0; i < a.Length && i < b.Length; i++) s += a[i] * b[i]; return s; }
    }
}
