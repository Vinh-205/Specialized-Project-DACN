using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("LICH_SU_TRANG_THAI")]
    public class LichSuTrangThai
    {
        [Key]
        public int MaLS { get; set; }

        [Required]
        public int MaPhieu { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Trạng thái cũ")]
        public string TrangThaiCu { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Trạng thái mới")]
        public string TrangThaiMoi { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Thời gian chuyển")]
        public DateTime ThoiGian { get; set; } = DateTime.Now;

        // FK đến KTV thực hiện thay đổi trạng thái (nullable: hệ thống tự động)
        public int? Ma_KTV { get; set; }

        // Navigation properties
        [StringLength(50)] public string? LoaiSuKien { get; set; }
        public string? DuLieuBoSungJSON { get; set; }

        [ForeignKey("MaPhieu")]
        public virtual PhieuTiepNhan? PhieuTiepNhan { get; set; }

        [ForeignKey("Ma_KTV")]
        public virtual User? KTV { get; set; }
    }
}
