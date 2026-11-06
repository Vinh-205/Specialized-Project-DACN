using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("CHI_TIET_SUA_CHUA")]
    public class ChiTietSuaChua
    {
        [Key]
        public int MaCT { get; set; }

        [Required]
        public int MaPhieu { get; set; }

        [Required, StringLength(255)]
        public string TenHangMuc { get; set; }

        [Required]
        public decimal ChiPhi { get; set; }

        [StringLength(255)]
        public string? GhiChu { get; set; }

        [StringLength(1000)] public string? ThamChieuBangChungKiemTraLai { get; set; }
        [StringLength(50)] public string? TrangThaiKiemTraLai { get; set; }
        public DateTime? ThoiGianKiemTraLai { get; set; }

        [ForeignKey("MaPhieu")]
        public virtual PhieuTiepNhan? PhieuTiepNhan { get; set; }

        public virtual ICollection<SuDungLinhKien> SuDungLinhKiens { get; set; } = new List<SuDungLinhKien>();
    }
}