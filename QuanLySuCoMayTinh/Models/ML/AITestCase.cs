using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySuCoMayTinh.Models.ML
{
    [Table("AI_TEST_CASE")]
    public class AITestCase
    {
        [Key]
        public int MaTestCase { get; set; }

        public int? MaMoHinh { get; set; }
        public virtual MLModelInfo? MoHinh { get; set; }

        [Required]
        public string TrieuChungNhap { get; set; } // Comma-separated list of symptom names

        [Required]
        public string LoiKyVong { get; set; } // The fault entered by the Admin (Expected)

        public string? DuDoanRule { get; set; } // The fault predicted by Rule Engine

        public string? DuDoanAITop1 { get; set; } // The fault predicted by AI

        public string? DuDoanAITop3JSON { get; set; } // JSON list of Top 3

        public bool? AIDuDoanDung { get; set; } // Whether AI matched ExpectedFault

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
