using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    // Bảng trung gian với Composite Key (MaCT + MaLK)
    // Chứa giá xuất tại thời điểm sử dụng để bảo toàn lịch sử kế toán
    [Table("SU_DUNG_LINH_KIEN")]
    public class SuDungLinhKien
    {
        // Phần của Composite Key - FK đến ChiTietSuaChua
        [Required]
        public int MaCT { get; set; }

        // Phần của Composite Key - FK đến LinhKien
        [Required]
        public int MaLK { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải ít nhất là 1")]
        [Display(Name = "Số lượng dùng")]
        public int SoLuong { get; set; }

        // Chốt giá tại thời điểm xuất kho (không thay đổi theo giá kho sau này)
        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Đơn giá xuất")]
        public decimal DonGiaXuat { get; set; }

        // Navigation properties
        [ForeignKey("MaCT")]
        public virtual ChiTietSuaChua? ChiTietSuaChua { get; set; }

        [ForeignKey("MaLK")]
        public virtual LinhKien? LinhKien { get; set; }
    }
}
