using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("MODEL_ACTIVATION_LOG")]
    public class ModelActivationLog
    {
        [Key]
        public int MaKichHoat { get; set; }

        public int MaMoHinh { get; set; }
        public int? MaNguoiDung { get; set; }
        public virtual MLModelInfo? MoHinh { get; set; }
        public virtual User? NguoiDung { get; set; }

        [StringLength(50)]
        public string? PhienBanCu { get; set; }

        [NotMapped]
        public string PhienBanMoiLuuTru { get; set; } = string.Empty;

        [Required]
        public DateTime ThoiGianKichHoat { get; set; }

        [NotMapped]
        public string? NguoiKichHoatLuuTru { get; set; }
    }
}
