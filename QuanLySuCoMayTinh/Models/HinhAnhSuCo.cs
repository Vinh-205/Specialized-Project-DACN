using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("HINH_ANH_SU_CO")]
    public class HinhAnhSuCo
    {
        [Key]
        public int MaHA { get; set; }

        [Required]
        public int MaPhieu { get; set; }

        [Required, StringLength(500)]
        [Display(Name = "Đường dẫn ảnh")]
        public string DuongDanAnh { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Mô tả hình ảnh")]
        public string? MoTa { get; set; }

        [Required]
        [Display(Name = "Ngày chụp")]
        public DateTime NgayChup { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("MaPhieu")]
        public virtual PhieuTiepNhan? PhieuTiepNhan { get; set; }
    }
}
