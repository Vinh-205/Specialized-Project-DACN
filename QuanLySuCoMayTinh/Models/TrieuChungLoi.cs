using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("TRIEU_CHUNG_LOI")]
    public class TrieuChungLoi
    {
        public int MaTC { get; set; }
        public int MaLoi { get; set; }

        [ForeignKey("MaTC")]
        public virtual TrieuChung? TrieuChung { get; set; }

        [ForeignKey("MaLoi")]
        public virtual MaLoi? MaLoiNavigation { get; set; }
    }
}