using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models.ML;

namespace QuanLySuCoMayTinh.Services
{
    /// <summary>
    /// Xuất dữ liệu chẩn đoán của hệ thống theo hợp đồng dữ liệu của bộ mô hình chuẩn (MoHinh-Chuan):
    /// id,text,label,source_type,group. Không tự sinh nhãn: chỉ xuất MaLoiThucTe đã có, phân loại nguồn theo quy tắc nhãn đã xác minh.
    /// </summary>
    public static class TrainingDataExporter
    {
        private static string Csv(string s) => "\"" + (s ?? "").Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ") + "\"";

        public static async Task<byte[]> BuildZipAsync(ApplicationDbContext db, bool kemMoTaSuCo)
        {
            var rows = await db.LichSuChanDoans
                .Where(x => x.MaLoiThucTe != null && x.NguonDuLieu != TrainingDataSource.LegacyRandom)
                .Include(x => x.TrieuChungs)
                .Include(x => x.PhieuTiepNhan)
                .OrderBy(x => x.MaLichSuChanDoan)
                .AsNoTracking()
                .ToListAsync();

            var csv = new StringBuilder("id,text,label,source_type,group\n");
            var count = new Dictionary<string, int>();
            int skippedNoSymptom = 0;
            foreach (var r in rows)
            {
                var symptoms = r.TrieuChungs.Select(t => t.TenTrieuChungLuuTru).Where(s => !string.IsNullOrWhiteSpace(s)).OrderBy(s => s, StringComparer.Ordinal).ToList();
                var text = string.Join("; ", symptoms);
                // Mô tả sự cố do khách nhập có thể chứa thông tin cá nhân: chỉ xuất khi người dùng chủ động chọn.
                if (kemMoTaSuCo && !string.IsNullOrWhiteSpace(r.PhieuTiepNhan?.MoTaSuCo)) text = (text + " | " + r.PhieuTiepNhan!.MoTaSuCo).Trim(' ', '|');
                if (string.IsNullOrWhiteSpace(text)) { skippedNoSymptom++; continue; }

                // Cùng định nghĩa với TrainingEligibility.ForTraining: chỉ RealRepair đã xác minh đầy đủ mới là real_verified.
                var verified = r.NguonDuLieu == TrainingDataSource.RealRepair && r.TrangThaiXacMinh == "Đã xác minh" && r.NgayXacMinh != null && r.MaNguoiDungXacMinh != null;
                var sourceType = r.NguonDuLieu switch
                {
                    TrainingDataSource.RealRepair => verified ? "real_verified" : "real_unverified",
                    TrainingDataSource.UserHistory => "real_unverified",
                    TrainingDataSource.SyntheticRule => "synthetic",
                    TrainingDataSource.PublicDataset => "public",
                    _ => "real_unverified"
                };
                // Nhóm độc lập: cùng thiết bị không được nằm ở hai tập khác nhau; synthetic mỗi dòng một nhóm.
                var group = (r.NguonDuLieu == TrainingDataSource.RealRepair || r.NguonDuLieu == TrainingDataSource.UserHistory) && r.PhieuTiepNhan != null
                    ? $"tb{r.PhieuTiepNhan.MaTB}" : $"ls{r.MaLichSuChanDoan}";
                csv.Append(r.MaLichSuChanDoan).Append(',').Append(Csv(text)).Append(',').Append($"L{r.MaLoiThucTe}").Append(',').Append(sourceType).Append(',').Append(Csv(group)).Append('\n');
                count[sourceType] = count.GetValueOrDefault(sourceType) + 1;
            }

            var labelMap = new StringBuilder("label,MaLoi,TenLoi,PhanLoai\n");
            foreach (var m in await db.MaLois.OrderBy(m => m.MaLoiID).AsNoTracking().ToListAsync())
                labelMap.Append($"L{m.MaLoiID},{m.MaLoiID},").Append(Csv(m.TenLoi)).Append(',').Append(Csv(m.PhanLoai)).Append('\n');

            var info = JsonSerializer.Serialize(new
            {
                exported_utc = DateTime.UtcNow.ToString("o"),
                contract = "MoHinh-Chuan: id,text,label,source_type,group",
                rows = count.Values.Sum(),
                by_source_type = count,
                skipped_no_symptom_text = skippedNoSymptom,
                include_mo_ta_su_co = kemMoTaSuCo,
                note = "real_verified chỉ gồm RealRepair có TrangThaiXacMinh='Đã xác minh', NgayXacMinh và MaNguoiDungXacMinh. LegacyRandom bị loại. Nhãn không được tạo thêm."
            }, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                void Add(string name, string content)
                {
                    var e = zip.CreateEntry(name);
                    using var w = new StreamWriter(e.Open(), new UTF8Encoding(false));
                    w.Write(content);
                }
                Add("training.csv", csv.ToString());
                Add("label_map.csv", labelMap.ToString());
                Add("export_info.json", info);
            }
            return ms.ToArray();
        }
    }
}
