namespace QuanLySuCoMayTinh.Services
{
    public sealed record NhomLoiDuDoan(string Ma, string Ten, float XacSuat);
    public sealed record LoiXepHang(int MaLoi, double Diem, int SoTrieuChungKhop, double DoTuongDongNhom, double XacSuatTrucTiep = 0, string TenLoi = "");

    /// <summary>
    /// Chẩn đoán có lõi HỌC MÁY: (1) dự đoán nhóm lỗi chi tiết (30 nhóm) từ mô tả tự do (bộ phân loại học trên câu hỏi thật tiếng Anh + tự huấn luyện bằng mô tả sửa chữa thật và câu hỏi tiếng Việt, chuyển sang tiếng Việt qua bộ mã hóa đa ngôn ngữ);
    /// (2) xếp hạng mã lỗi = số triệu chứng khớp + 2 × độ tương đồng giữa phân bố nhóm lỗi của mô tả và của tên mã lỗi (cùng bộ phân loại). Trọng số 2 đặt trước khi đo.
    /// Không có mô tả thì không dùng phần học máy (giữ nguyên Rule). Chỉ gợi ý, KTV quyết định.
    /// </summary>
    public sealed class ChanDoanAIService
    {
        public const double TrongSoNhom = 2.0;
        private readonly SongNguClassifier _sn;
        private readonly Dictionary<string, float[]?> _cacheLoi = new();
        private readonly object _khoa = new();
        /// <summary>Trọng số mô hình dự đoán trực tiếp mã lỗi (đặt trước = 1; đo lai 108 phép: Top-1 68→80, Top-3 101→103).</summary>
        public const double TrongSoTrucTiep = 1.0;
        /// <summary>
        /// Mức tin cậy. Cao = biên điểm (top1 − top2) ≥ 1,0 VÀ mô hình dự đoán trực tiếp mã lỗi đồng ý (cùng mã lỗi đứng đầu): chọn trên tập phát triển 108 phép
        /// (đúng 37/37, phủ 34%), đo MỘT LẦN trên tập kiểm chứng mới 80 phép soạn sau (đúng 26/26, phủ 32%). Trung bình = biên ≥ 0,4 (dev 89,7%, kiểm chứng ~95% khi có đồng ý). Thấp = còn lại.
        /// Tập kiểm chứng mới cho Top-1 chung chỉ 62,5% (dev 74%), nên các mức Trung bình/Thấp KHÔNG tự chốt.
        /// </summary>
        public static string MucTinCay(double bien, bool trucTiepDongY) => bien >= 1.0 && trucTiepDongY ? "Cao" : bien >= 0.4 ? "Trung bình" : "Thấp";
        /// <summary>
        /// Mức tin cậy theo ước lượng xác suất đúng: Cao ≥ 0,75 (trên 890 bài thật giữ riêng: phủ 18%, đúng 82,8%; trên 80 câu tự soạn đúng 24/24), Trung bình ≥ 0,5, còn lại Thấp.
        /// Không có tệp tin_cay.json thì dùng luật cũ (biên ≥ 1,0 và đồng ý).
        /// </summary>
        public (string Muc, double? XacSuat) DoTinCay(IReadOnlyList<LoiXepHang> xh, string? moTa, int soTrieuChungChon)
        {
            if (xh.Count < 2) return ("Thấp", null);
            var d = _truc?.DuDoan(moTa); double bien = xh[0].Diem - xh[1].Diem;
            if (d == null || d.Count < 2 || _truc == null) return (MucTinCay(bien, false), null);
            var sap = d.OrderByDescending(kv => kv.Value).ToList(); bool dongY = string.Equals(sap[0].Key, xh[0].TenLoi.Trim(), StringComparison.OrdinalIgnoreCase);
            if (!_truc.CoUocLuongTinCay) return (MucTinCay(bien, dongY), null);
            var p = _truc.UocLuongXacSuatDung(new[] { bien, dongY ? 1.0 : 0.0, d.GetValueOrDefault(xh[0].TenLoi.Trim()), sap[0].Value, sap[0].Value - sap[1].Value, xh[0].DoTuongDongNhom, xh[0].SoTrieuChungKhop, Math.Clamp(soTrieuChungChon, 1, 4) });
            return (p >= _truc.NguongCao ? "Cao" : p >= _truc.NguongTrungBinh ? "Trung bình" : "Thấp", p);
        }

        /// <summary>Mã lỗi đứng đầu của mô hình dự đoán trực tiếp (null nếu không có mô hình hoặc mô tả).</summary>
        public string? TenLoiTrucTiepDauTien(string? moTa) { var d = _truc?.DuDoan(moTa); return d == null || d.Count == 0 ? null : d.OrderByDescending(kv => kv.Value).First().Key; }
        private readonly MaLoiTrucTiepClassifier? _truc;
        public ChanDoanAIService(SongNguClassifier sn, MaLoiTrucTiepClassifier? truc = null) { _sn = sn; _truc = truc; }
        public bool SanSang => _sn.SanSang && _sn.NhanNhomLoi.Length > 0;

        public List<NhomLoiDuDoan> Top3Nhom(string? moTa)
        {
            var p = _sn.PhanBoNhomLoi(moTa); var kq = new List<NhomLoiDuDoan>();
            if (p == null) return kq;
            foreach (var i in Enumerable.Range(0, p.Length).OrderByDescending(i => p[i]).Take(3))
            { var ma = _sn.NhanNhomLoi[i]; kq.Add(new NhomLoiDuDoan(ma, SongNguClassifier.TenNhomLoi.GetValueOrDefault(ma, ma), p[i])); }
            return kq;
        }

        private float[]? PhanBoLoi(string ten)
        {
            lock (_khoa)
            {
                if (!_cacheLoi.TryGetValue(ten, out var v)) _cacheLoi[ten] = v = _sn.PhanBoNhomLoi(ten);
                return v;
            }
        }

        public List<LoiXepHang> XepHang(string? moTa, IEnumerable<int> trieuChungDaChon, IReadOnlyDictionary<int, string> tenLoi, IReadOnlyDictionary<int, HashSet<int>> trieuChungCuaLoi)
        {
            var chon = new HashSet<int>(trieuChungDaChon); var p = SanSang ? _sn.PhanBoNhomLoi(moTa) : null; var kq = new List<LoiXepHang>();
            var truc = _truc?.DuDoan(moTa);
            foreach (var (ma, ten) in tenLoi)
            {
                int khop = trieuChungCuaLoi.TryGetValue(ma, out var tc) ? tc.Count(chon.Contains) : 0; double cos = 0;
                if (p != null) { var q = PhanBoLoi(ten); if (q != null) cos = Cosine(p, q); }
                double pt = truc != null && truc.TryGetValue(ten.Trim(), out var pv) ? pv : 0;
                kq.Add(new LoiXepHang(ma, khop + TrongSoNhom * cos + TrongSoTrucTiep * pt, khop, cos, pt, ten));
            }
            return kq.OrderByDescending(x => x.Diem).ThenBy(x => x.MaLoi).ToList();
        }

        private static double Cosine(float[] a, float[] b)
        {
            double d = 0, na = 0, nb = 0; for (int i = 0; i < a.Length && i < b.Length; i++) { d += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
            return d / (Math.Sqrt(na) * Math.Sqrt(nb) + 1e-9);
        }
    }
}
