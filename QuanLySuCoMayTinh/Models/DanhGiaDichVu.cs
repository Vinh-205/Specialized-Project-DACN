using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    // Quan hệ 1:1 với PhieuTiepNhan - một phiếu chỉ có một đánh giá
    [Table("DANH_GIA_DICH_VU")]
    public class DanhGiaDichVu
    {
        [Key]
        // MaDG đồng thời là FK 1:1 đến PhieuTiepNhan (Shared Primary Key pattern)
        public int MaDG { get; set; }

        [Required]
        public int MaPhieu { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
        [Display(Name = "Số sao")]
        public int SoSao { get; set; }

        [StringLength(1000)]
        [Display(Name = "Bình luận")]
        public string? BinhLuan { get; set; }

        [Required]
        [Display(Name = "Ngày đánh giá")]
        public DateTime NgayDG { get; set; } = DateTime.Now;

        // Navigation property - quan hệ 1:1 ngược lại
        [ForeignKey("MaPhieu")]
        public virtual PhieuTiepNhan? PhieuTiepNhan { get; set; }
    }
}
