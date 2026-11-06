using Microsoft.ML.Data;

namespace QuanLySuCoMayTinh.Models.ML
{
    public class ChanDoanPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedMaLoi { get; set; }

        [ColumnName("Score")]
        public float[] Scores { get; set; }
    }
}
