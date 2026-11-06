using Microsoft.ML.Data;

namespace QuanLySuCoMayTinh.Models.ML
{
    public class ChanDoanData
    {
        [LoadColumn(0)]
        [VectorType(500)]
        public float[] Features { get; set; } // Multi-Hot Encoded Vector

        [NoColumn]
        public string? TrieuChungsText { get; set; } // For RuleEngine evaluation

        [LoadColumn(1)]
        public uint MaLoiThucTe { get; set; } // Actual fault (key type for multiclass classification)

        [LoadColumn(2)]
        public float Weight { get; set; } // Confidence weight
    }
}
