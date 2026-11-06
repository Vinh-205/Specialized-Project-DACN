using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("LICH_SU_CHAN_DOAN")]
    public class LichSuChanDoan
    {
        [Key]
        public int MaLichSuChanDoan { get; set; }

        [Required]
        public int MaPhieu { get; set; }

        public int? MaLoiRule { get; set; }

        public int? MaLoiAITop1 { get; set; }
        public double? DoTinCayTop1 { get; set; }
        
        public int? MaLoiAITop2 { get; set; }
        public double? DoTinCayTop2 { get; set; }
        
        public int? MaLoiAITop3 { get; set; }
        public double? DoTinCayTop3 { get; set; }

        public QuanLySuCoMayTinh.Models.ML.TrainingDataSource NguonDuLieu { get; set; } = QuanLySuCoMayTinh.Models.ML.TrainingDataSource.UserHistory;

        public int? MaLoiThucTe { get; set; }

        [StringLength(255)]
        public string? TenLoiThucTeLuuTru { get; set; }

        [Required]
        public DateTime ThoiGian { get; set; }

        [Required, StringLength(30)] public string TrangThaiXacMinh { get; set; } = "Chưa xác minh";
        public DateTime? NgayXacMinh { get; set; }
        public int? MaNguoiDungXacMinh { get; set; }
        public int? MaMoHinh { get; set; }
        public virtual User? NguoiDungXacMinh { get; set; }
        public virtual MLModelInfo? MoHinh { get; set; }
        public virtual MaLoi? LoiRule { get; set; }
        public virtual MaLoi? LoiAITop1 { get; set; }
        public virtual MaLoi? LoiAITop2 { get; set; }
        public virtual MaLoi? LoiAITop3 { get; set; }
        public virtual MaLoi? LoiThucTe { get; set; }

        [ForeignKey("MaPhieu")]
        public virtual PhieuTiepNhan? PhieuTiepNhan { get; set; }

        public virtual ICollection<LichSuChanDoanTrieuChung> TrieuChungs { get; set; } = new List<LichSuChanDoanTrieuChung>();
    }
}
