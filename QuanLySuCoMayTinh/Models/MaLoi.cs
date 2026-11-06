using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("MA_LOI")]
    public class MaLoi
    {
        [Key]
        public int MaLoiID { get; set; } 

        [Required, StringLength(150)]
        public string TenLoi { get; set; }

        [Required, StringLength(50)]
        public string PhanLoai { get; set; }

        [Required]
        public bool Status { get; set; }

        public virtual ICollection<TrieuChungLoi> TrieuChungLois { get; set; } = new List<TrieuChungLoi>();
        public virtual ICollection<GiaiPhap> GiaiPhaps { get; set; } = new List<GiaiPhap>();
        public virtual ICollection<PhieuTiepNhan> PhieuTiepNhans { get; set; } = new List<PhieuTiepNhan>();
    }
}