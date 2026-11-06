using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("THIET_BI")]
    public class ThietBi
    {
        [Key]
        public int MaTB { get; set; }

        [Required, StringLength(100)]
        public string TenTB { get; set; }

        [StringLength(255)]
        public string? CauHinh { get; set; }

        [StringLength(500)]
        public string? NgoaiQuan { get; set; }

        [Required]
        public int MaKH { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang? KhachHang { get; set; }

        public virtual ICollection<PhieuTiepNhan> PhieuTiepNhans { get; set; } = new List<PhieuTiepNhan>();
    }
}