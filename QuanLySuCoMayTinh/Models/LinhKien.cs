using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("LINH_KIEN")]
    public class LinhKien
    {
        [Key]
        public int MaLK { get; set; }

        [Required, StringLength(255)]
        [Display(Name = "Tên linh kiện")]
        public string TenLK { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Loại linh kiện")]
        public string LoaiLK { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm")]
        [Display(Name = "Đơn giá (VNĐ)")]
        public decimal DonGia { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public bool CoTrangThai { get; set; } = true;

        // Navigation property
        public virtual ICollection<SuDungLinhKien> SuDungLinhKiens { get; set; } = new List<SuDungLinhKien>();
    }
}
