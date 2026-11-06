using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("KHACH_HANG")]
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; }

        [Required, StringLength(15)]
        public string SDT { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public virtual ICollection<ThietBi> ThietBis { get; set; } = new List<ThietBi>();
    }
}