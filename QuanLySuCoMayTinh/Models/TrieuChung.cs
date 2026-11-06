using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("TRIEU_CHUNG")]
    public class TrieuChung
    {
        [Key]
        public int MaTC { get; set; }

        [Required, StringLength(100)]
        public string TenTC { get; set; }

        public string? MoTa { get; set; }

        public virtual ICollection<TrieuChungLoi> TrieuChungLois { get; set; } = new List<TrieuChungLoi>();
    }
}