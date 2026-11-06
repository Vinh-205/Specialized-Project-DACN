using QuanLySuCoMayTinh.Models;
using QuanLySuCoMayTinh.Models.ML;
namespace QuanLySuCoMayTinh.Services;
public static class TrainingEligibility
{
    public static IQueryable<LichSuChanDoan> ForTraining(IQueryable<LichSuChanDoan> source) => source.Where(x =>
        x.MaLoiThucTe != null &&
        ((x.NguonDuLieu == TrainingDataSource.RealRepair && x.TrangThaiXacMinh == "Đã xác minh" && x.NgayXacMinh != null && x.MaNguoiDungXacMinh != null) ||
         x.NguonDuLieu == TrainingDataSource.SyntheticRule || x.NguonDuLieu == TrainingDataSource.PublicDataset));
}
