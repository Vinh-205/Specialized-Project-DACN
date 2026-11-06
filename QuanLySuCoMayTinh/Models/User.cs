using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    [Table("NGUOI_DUNG")]
    public class User
    {
        [Key]
        public int USERID { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(255)]
        public string Password_Hash { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(20)]
        public string Role { get; set; }

        [Required]
        public bool Status { get; set; }

        public virtual ICollection<PhieuTiepNhan> PhieuTiepNhans { get; set; } = new List<PhieuTiepNhan>();
    }
}