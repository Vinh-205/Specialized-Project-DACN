using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("PHIEU_TIEP_NHAN")]
    public class PhieuTiepNhan
    {
        [Key]
        public int MaPhieu { get; set; }

        [Required]
        public int MaTB { get; set; }

        [Required]
        public int MaUser_KTV { get; set; }

        [Required]
        public DateTime NgayNhan { get; set; }

        public DateTime? NgayTra_DK { get; set; }

        [Required, StringLength(50)]
        public string TrangThai { get; set; }

        public int? MaLoi_CD { get; set; }

        public string? GhiChuNoiBo { get; set; }

        public string? MoTaSuCo { get; set; }
        [StringLength(100)] public string? PhanLoaiYeuCau { get; set; }
        [StringLength(30)] public string? MucDoUuTien { get; set; }
        [StringLength(30)] public string? MucDoUuTienDeXuat { get; set; }

        [ForeignKey("MaTB")]
        public virtual ThietBi? ThietBi { get; set; }

        [ForeignKey("MaUser_KTV")]
        public virtual User? User_KTV { get; set; }

        [ForeignKey("MaLoi_CD")]
        public virtual MaLoi? MaLoiNavigation { get; set; }

        public virtual ICollection<ChiTietSuaChua> ChiTietSuaChuas { get; set; } = new List<ChiTietSuaChua>();
        public virtual ICollection<HinhAnhSuCo> HinhAnhSuCos { get; set; } = new List<HinhAnhSuCo>();
        public virtual ICollection<LichSuTrangThai> LichSuTrangThais { get; set; } = new List<LichSuTrangThai>();
        public virtual DanhGiaDichVu? DanhGiaDichVu { get; set; }
    }
}