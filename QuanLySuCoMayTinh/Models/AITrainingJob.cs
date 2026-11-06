using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models
{
    public enum TrainingJobStatus
    {
        Pending,
        Running,
        Completed,
        Failed
    }

    [Table("AI_TRAINING_JOB")]
    public class AITrainingJob
    {
        [Key]
        public int MaTacVu { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public DateTime? NgayHoanThanh { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; }

        public string? ThongBaoLoi { get; set; }

        public int? MaMoHinh { get; set; }

        public int? MaNguoiDung { get; set; }
        public virtual User? NguoiDung { get; set; }

        [ForeignKey("MaMoHinh")]
        public virtual MLModelInfo? MLModelInfo { get; set; }
    }
}
