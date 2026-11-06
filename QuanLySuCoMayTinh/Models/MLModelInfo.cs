using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("ML_MODEL_INFO")]
    public class MLModelInfo
    {
        [Key]
        public int MaMoHinh { get; set; }

        [Required]
        [StringLength(50)]
        public string PhienBan { get; set; }

        public double DoChinhXacMicro { get; set; }

        public double DoChinhXacMacro { get; set; }

        public double LogLoss { get; set; }

        public double LogLossReduction { get; set; }

        public int SoBanGhiHuanLuyen { get; set; }

        public int SoBanGhiThucTe { get; set; }
        public int SoBanGhiLichSuNguoiDung { get; set; }
        public int SoBanGhiTongHop { get; set; }

        public double DoChinhXacRule { get; set; }
        public double DoChinhXacAI { get; set; }

        public double? DoChinhXacTop3 { get; set; }
        public double? DoChinhXacTop5 { get; set; }

        [StringLength(50)]
        public string? PhienBanBoDuLieu { get; set; }
        public DateTime? NgayTaoBoDuLieu { get; set; }

        public string? MaTranNhamLanJSON { get; set; }

        [Required, StringLength(100)] public string LoaiBaiToan { get; set; } = "CHAN_DOAN_TRIEU_CHUNG_MA_LOI";

        [Required]
        public DateTime NgayTao { get; set; }

        public bool DangSuDung { get; set; }
        
        [StringLength(255)]
        public string? DuongDanTep { get; set; }
    }
}
