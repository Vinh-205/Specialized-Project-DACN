using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("GIAI_PHAP")]
    public class GiaiPhap
    {
        [Key]
        public int MaGP { get; set; }

        [Required]
        public int MaLoi { get; set; }

        [Required]
        public string NoiDungStepByStep { get; set; }

        public int? MucDoKho { get; set; }

        [Required]
        public bool Status { get; set; }

        [ForeignKey("MaLoi")]
        public virtual MaLoi? MaLoiNavigation { get; set; }
    }
}