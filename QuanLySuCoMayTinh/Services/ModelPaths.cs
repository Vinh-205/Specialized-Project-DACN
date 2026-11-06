namespace QuanLySuCoMayTinh.Services
{
    /// <summary>
    /// Thư mục chứa MỌI mô hình AI (ML.NET: ActiveModel.zip + các bản DiagnosticModel_*.zip; ONNX song ngữ: thư mục SongNgu).
    /// Đặt khóa cấu hình <c>AI:ModelRoot</c> (ví dụ D:\Code\Đồ án chuyên nghành\AI-Models) để huấn luyện/lưu mô hình ở ngoài mã nguồn rồi để website nạp từ đó.
    /// Không đặt thì dùng Data/MLModels trong thư mục chạy như trước.
    /// </summary>
    public static class ModelPaths
    {
        public static string Root(IConfiguration cfg, string thuMucGoc)
        {
            var r = cfg["AI:ModelRoot"];
            return string.IsNullOrWhiteSpace(r) ? Path.Combine(thuMucGoc, "Data", "MLModels") : r;
        }
        public static string SongNgu(IConfiguration cfg, string thuMucGoc)
        {
            var d = cfg["AI:SongNguDir"];
            return string.IsNullOrWhiteSpace(d) ? Path.Combine(Root(cfg, thuMucGoc), "SongNgu") : d;
        }
    }
}
