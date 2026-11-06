using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("LICH_SU_CHAN_DOAN_TRIEU_CHUNG")]
    public class LichSuChanDoanTrieuChung
    {
        [NotMapped]
        public int Id { get; set; }

        [Required]
        public int MaLichSuChanDoan { get; set; }

        [Required]
        public int MaTrieuChung { get; set; }

        [StringLength(255)]
        public string? TenTrieuChungLuuTru { get; set; }

        [ForeignKey("MaLichSuChanDoan")]
        public virtual LichSuChanDoan? LichSuChanDoan { get; set; }

        [ForeignKey("MaTrieuChung")]
        public virtual TrieuChung? TrieuChung { get; set; }
    }
}
