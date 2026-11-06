using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLySuCoMayTinh.Services
{
    /// <summary>Kết quả đề xuất cho một mô tả sự cố.</summary>
    public sealed record YeuCauGoiY(string PhanLoai, string MucDoUuTien, IReadOnlyList<string> LyDo, string Nguon);

    /// <summary>
    /// Đề xuất PHÂN LOẠI yêu cầu và MỨC ĐỘ ƯU TIÊN từ mô tả sự cố tiếng Việt (có dấu hoặc không dấu).
    /// Đây là bộ quy tắc từ khóa minh bạch (mỗi đề xuất kèm lý do), KHÔNG phải mô hình học máy: hiện chưa có dữ liệu tiếng Việt có nhãn.
    /// Chỉ là gợi ý cho kỹ thuật viên; không ghi đè lựa chọn của KTV.
    /// </summary>
    public static class YeuCauSuggester
    {
        public const string Nguon = "Quy tắc từ khóa tiếng Việt (chưa huấn luyện từ ca thật)";

        // Nhãn phân loại khớp MA_LOI.PhanLoai của hệ thống.
        public static readonly string[] PhanLoais = { "Phần cứng", "Hệ điều hành", "Nguồn & Pin", "Phần mềm", "Mạng", "Chưa rõ" };
        public static readonly string[] MucDos = { "Cao", "Trung bình", "Thấp" };

        public static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var d = s.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(d.Length);
            foreach (var c in d) if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
            return Regex.Replace(sb.ToString().Normalize(NormalizationForm.FormC), @"[^a-z0-9]+", " ").Trim();
        }

        // (cụm từ đã chuẩn hóa không dấu, trọng số, lý do hiển thị)
        private static readonly Dictionary<string, (string Phrase, int W)[]> PhanLoaiRules = new()
        {
            ["Nguồn & Pin"] = new[]
            {
                ("khong len nguon", 3), ("khong bat duoc may", 3), ("tu tat nguon", 3), ("sut nguon", 3), ("phong pin", 4), ("chai pin", 3), ("hao pin", 3),
                ("cong sac", 3), ("bo nguon", 3), ("tu tat", 3), ("bam nut nguon", 2), ("thay pin", 2), ("khong nhan sac", 3), ("khong sac", 3), ("sac", 2), ("pin", 2), ("adapter", 2), ("nguon dien", 2), ("nguon", 1)
            },
            ["Mạng"] = new[]
            {
                ("wifi", 3), ("wi fi", 3), ("internet", 3), ("cap mang", 3), ("mat ket noi", 3), ("khong vao duoc web", 3), ("khong vao mang", 3), ("dns", 3),
                ("mang lan", 3), ("rot mang", 3), ("luot web", 3), ("mat mang", 3), ("bluetooth", 2), ("router", 2), ("modem", 2)
            },
            ["Hệ điều hành"] = new[]
            {
                ("man hinh xanh", 4), ("bsod", 4), ("windows", 3), ("he dieu hanh", 3), ("khong khoi dong", 3), ("treo may", 3), ("bi treo", 2), ("dung hinh", 2),
                ("boot", 3), ("cap nhat windows", 3), ("update", 2), ("driver", 3), ("khoi dong lai lien tuc", 4), ("khong vao duoc windows", 4), ("chay cham", 3), ("khoi dong cham", 3), ("khoi dong rat cham", 3),
                ("quen mat khau", 2), ("dang nhap", 1), ("cai lai windows", 3), ("cai lai win", 3), ("cai win", 3)
            },
            ["Phần mềm"] = new[]
            {
                ("phan mem", 3), ("ung dung", 3), ("cai dat", 2), ("office", 3), ("word", 2), ("excel", 2), ("powerpoint", 2), ("virus", 3), ("malware", 3),
                ("quang cao", 2), ("trinh duyet", 2), ("chrome", 2), ("ban quyen", 3), ("photoshop", 3), ("adobe", 3), ("autocad", 3), ("bi loi khi mo", 3), ("bi crash", 3),
                ("tu thoat", 2), ("lien tuc dong", 2), ("ma hoa", 3), ("ransomware", 4), ("diet virus", 3)
            },
            ["Phần cứng"] = new[]
            {
                ("man hinh", 2), ("ban phim", 3), ("touchpad", 3), ("chuot", 2), ("quat", 3), ("nong", 2), ("o cung", 3), ("keo tan nhiet", 3), ("tan nhiet", 3), ("vo may", 2), ("ssd", 3), ("hdd", 3), ("ram", 3),
                ("mainboard", 3), ("card do hoa", 3), ("loa", 2), ("am thanh", 2), ("webcam", 3), ("usb", 3), ("cong usb", 3), ("be man hinh", 4), ("vo man hinh", 4),
                ("bi roi", 2), ("roi xuong", 2), ("lam roi", 2), ("te nuoc", 4), ("do nuoc", 4), ("lieu", 1), ("tieng keu", 2), ("keu to", 2), ("hu", 1), ("hong", 1), ("vao nuoc", 4), ("soc", 2), ("dead pixel", 3),
                ("lap ram", 3), ("thay o cung", 3), ("khong nhan o cung", 4), ("man hinh khong len", 4), ("man hinh den", 3)
            }
        };

        // Tín hiệu ưu tiên CAO (mạnh = một mình đủ để đề xuất Cao).
        private static readonly (string Phrase, int W, string Ly)[] CaoRules =
        {
            ("mat du lieu", 5, "có nguy cơ mất dữ liệu"), ("mat het du lieu", 5, "có nguy cơ mất dữ liệu"), ("xoa nham", 4, "xóa nhầm dữ liệu"), ("khong doc duoc du lieu", 5, "không đọc được dữ liệu"),
            ("o cung keu", 4, "ổ cứng có tiếng kêu lạ"), ("o cung hong", 4, "ổ cứng hỏng"), ("khong nhan o cung", 4, "không nhận ổ cứng"), ("tieng keu lach cach", 4, "ổ cứng có tiếng kêu lạ"),
            ("khong len nguon", 4, "máy không lên nguồn"), ("khong bat duoc may", 4, "máy không bật được"), ("khong khoi dong", 4, "máy không khởi động"), ("khong vao duoc windows", 4, "không vào được hệ điều hành"),
            ("man hinh xanh", 3, "màn hình xanh (BSOD)"), ("bsod", 3, "màn hình xanh (BSOD)"), ("khoi dong lai lien tuc", 4, "máy khởi động lại liên tục"),
            ("chay no", 5, "nghi cháy nổ"), ("vao nuoc", 5, "máy bị vào nước"), ("tu tat", 3, "máy tự tắt đột ngột"), ("bi chay", 5, "máy/pin bị cháy"), ("mui chay", 5, "có mùi cháy"), ("khet", 5, "có mùi khét"), ("chap dien", 5, "nghi chập điện"), ("phong pin", 5, "pin phồng, nguy hiểm"), ("pin bi phong", 5, "pin phồng, nguy hiểm"), ("noi pin", 4, "pin có dấu hiệu nguy hiểm"),
            ("nhiem virus", 2, "nghi nhiễm virus"), ("ma hoa", 4, "nghi mã hóa tống tiền"), ("ransomware", 5, "nghi mã hóa tống tiền"), ("do nuoc", 5, "máy bị đổ nước"), ("te nuoc", 5, "máy bị tràn nước"),
            ("can gap", 3, "khách cần gấp"), ("rat gap", 3, "khách cần gấp"), ("gap lam", 3, "khách cần gấp"), ("gap qua", 3, "khách cần gấp"), ("lay gap", 3, "khách cần gấp"), ("dang gap", 3, "khách cần gấp"), ("khan cap", 4, "khẩn cấp"), ("can ngay", 4, "khách cần ngay"), ("deadline", 4, "có hạn chót"), ("bao ve", 3, "sắp bảo vệ/thi"),
            ("may chu", 3, "ảnh hưởng máy chủ/công việc"), ("hom nay", 2, "cần trong hôm nay"), ("di thi", 2, "sắp thi"), ("ngay mai thi", 3, "sắp thi"), ("khong lam viec duoc", 3, "không làm việc được")
        };

        // Tín hiệu ưu tiên THẤP.
        private static readonly (string Phrase, int W, string Ly)[] ThapRules =
        {
            ("tu van", 3, "chỉ cần tư vấn"), ("hoi gia", 3, "chỉ hỏi giá"), ("hoi thong tin", 3, "chỉ hỏi thông tin"), ("chi hoi", 3, "chỉ hỏi thông tin"), ("muon hoi", 2, "chỉ hỏi thông tin"), ("bao gia", 3, "chỉ hỏi báo giá"), ("ve sinh", 3, "vệ sinh/bảo dưỡng"), ("bao duong", 3, "bảo dưỡng định kỳ"),
            ("muon thay", 2, "chỉ muốn thay/nâng cấp"), ("nang cap", 3, "nâng cấp, không phải lỗi"), ("cai them", 3, "cài thêm, không phải lỗi"), ("khi ranh", 4, "không gấp, khi rảnh"), ("khong gap", 5, "khách nói không gấp"),
            ("khong khan", 5, "khách nói không khẩn"), ("tham my", 3, "chỉ thẩm mỹ"), ("tray xuoc", 3, "chỉ trầy xước"), ("lau lau", 2, "lỗi đã lâu, không khẩn"), ("thinh thoang", 2, "lỗi thỉnh thoảng"),
            ("doi mau", 2, "chỉ đổi màu/giao diện"), ("cai dat them", 3, "cài đặt thêm"), ("kiem tra dinh ky", 4, "kiểm tra định kỳ"), ("can cai", 2, "yêu cầu cài đặt thường quy")
        };

        public static YeuCauGoiY Suggest(string? moTa)
        {
            var t = Normalize(moTa);
            if (t.Length == 0) return new YeuCauGoiY("Chưa rõ", "Trung bình", new[] { "Chưa có mô tả sự cố" }, Nguon);
            // "không gấp/khẩn" phải xử lý trước để không bị tính là tín hiệu "gấp/khẩn"
            t = t.Replace("khong can gap", "khong gap").Replace("chua gap", "khong gap").Replace("khong can ngay", "khi ranh").Replace("khong qua gap", "khong gap");


            // --- Phân loại ---
            var rawLower = (moTa ?? "").ToLowerInvariant();
            var scores = new Dictionary<string, int>(); var hitsBy = new Dictionary<string, List<string>>();
            foreach (var kv in PhanLoaiRules)
            {
                int score = 0; var hits = new List<string>();
                foreach (var (ph, w) in kv.Value)
                {
                    if (!ContainsPhrase(t, ph)) continue;
                    score += w; hits.Add(ph);
                }
                scores[kv.Key] = score; hitsBy[kv.Key] = hits;
            }
            // "mạng" (có dấu) khác "mang" (cầm/đem); "phồng" khác "phòng".
            if (rawLower.Contains("mạng")) { scores["Mạng"] += 3; hitsBy["Mạng"].Add("mạng"); }
            if (rawLower.Contains("phồng")) { scores["Nguồn & Pin"] += 2; hitsBy["Nguồn & Pin"].Add("phồng"); }
            string phanLoai = "Chưa rõ"; int best = 1; var lyPl = new List<string>();   // cần tối thiểu 2 điểm mới phân loại
            foreach (var kv in scores)
                if (kv.Value > best) { best = kv.Value; phanLoai = kv.Key; lyPl = hitsBy[kv.Key]; }
            var ly = new List<string>();
            if (phanLoai != "Chưa rõ") ly.Add($"Phân loại \"{phanLoai}\" do từ khóa: {string.Join(", ", lyPl.Take(4))}");

            // --- Mức ưu tiên ---
            int cao = 0, thap = 0; var lyCao = new List<string>(); var lyThap = new List<string>();
            foreach (var (ph, w, l) in CaoRules)
            {
                if (!ContainsPhrase(t, ph)) continue;
                cao += w; if (!lyCao.Contains(l)) lyCao.Add(l);
            }
            foreach (var (ph, w, l) in ThapRules)
            {
                if (!ContainsPhrase(t, ph)) continue;
                thap += w; if (!lyThap.Contains(l)) lyThap.Add(l);
            }
            // Từ nhạy cảm khi bỏ dấu chỉ tính khi người dùng gõ CÓ DẤU (gấp ≠ gặp, cháy ≠ chạy, khẩn).
            var raw = (moTa ?? "").ToLowerInvariant();
            if (Regex.IsMatch(raw, @"(?<!không\s)(?<!không cần\s)(?<!chưa\s)(?<!ko\s)(?<!k\s)gấp")) { cao += 3; if (!lyCao.Contains("khách cần gấp")) lyCao.Add("khách cần gấp"); }
            if (Regex.IsMatch(raw, @"(?<!không\s)(?<!không cần\s)(?<!chưa\s)(?<!ko\s)khẩn")) { cao += 4; if (!lyCao.Contains("khách yêu cầu khẩn")) lyCao.Add("khách yêu cầu khẩn"); }
            if (raw.Contains("phồng") && ContainsPhrase(t, "pin")) { cao += 5; if (!lyCao.Contains("pin phồng, nguy hiểm")) lyCao.Add("pin phồng, nguy hiểm"); }
            if (Regex.IsMatch(raw, @"cháy|chập cháy")) { cao += 5; if (!lyCao.Contains("dấu hiệu cháy")) lyCao.Add("dấu hiệu cháy"); }
            string muc;
            if (cao >= 3 && cao > thap) { muc = "Cao"; ly.Add("Ưu tiên Cao: " + string.Join("; ", lyCao)); }
            else if (thap >= 2 && thap >= cao) { muc = "Thấp"; ly.Add("Ưu tiên Thấp: " + string.Join("; ", lyThap)); }
            else if (cao > 0 && cao < 3) { muc = "Trung bình"; ly.Add("Ưu tiên Trung bình (có dấu hiệu nhẹ: " + string.Join("; ", lyCao) + ")"); }
            else { muc = "Trung bình"; ly.Add("Ưu tiên Trung bình: không có tín hiệu khẩn cấp hay nhẹ rõ ràng"); }
            return new YeuCauGoiY(phanLoai, muc, ly, Nguon);
        }

        private static bool ContainsPhrase(string normalized, string phrase) =>
            (" " + normalized + " ").Contains(" " + phrase + " ");
    }
}
