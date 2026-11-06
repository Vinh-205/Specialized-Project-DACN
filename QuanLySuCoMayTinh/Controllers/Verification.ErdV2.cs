using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Models;
using QuanLySuCoMayTinh.Models.ML;
using System.Security.Claims;
using System.Text.Json;
namespace QuanLySuCoMayTinh.Controllers;
public partial class ChiTietSuaChuaController
{
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles="Admin KTV,KTV")]
    public async Task<IActionResult> CapNhatKiemTraLai(int maCT,string trangThai,string? bangChung)
    {
        if (trangThai is not ("Đạt" or "Không đạt") || string.IsNullOrWhiteSpace(bangChung) || bangChung.Length>1000) return BadRequest("Cần kết quả và tham chiếu bằng chứng kiểm tra lại.");
        var ct=await _context.ChiTietSuaChuas.Include(x=>x.PhieuTiepNhan).FirstOrDefaultAsync(x=>x.MaCT==maCT);
        if(ct==null)return NotFound();
        if(!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var actor))return Forbid();
        if(!User.IsInRole("Admin KTV")&&ct.PhieuTiepNhan?.MaUser_KTV!=actor)return Forbid();
        await using var tx=await _context.Database.BeginTransactionAsync();
        ct.TrangThaiKiemTraLai=trangThai;ct.ThamChieuBangChungKiemTraLai=bangChung.Trim();ct.ThoiGianKiemTraLai=DateTime.Now;
        var histories=await _context.LichSuChanDoans.Where(x=>x.MaPhieu==ct.MaPhieu&&x.TrangThaiXacMinh=="Đã xác minh").ToListAsync();
        foreach(var h in histories){
            _context.LichSuTrangThais.Add(new LichSuTrangThai{MaPhieu=ct.MaPhieu,Ma_KTV=actor,TrangThaiCu=ct.PhieuTiepNhan!.TrangThai,TrangThaiMoi=ct.PhieuTiepNhan.TrangThai,ThoiGian=DateTime.Now,LoaiSuKien="ThuHoiXacMinh",DuLieuBoSungJSON=JsonSerializer.Serialize(new{h.MaLichSuChanDoan,h.MaLoiThucTe,h.TenLoiThucTeLuuTru,h.NgayXacMinh,h.MaNguoiDungXacMinh,Reason="Bằng chứng kiểm tra lại thay đổi"})});
            h.TrangThaiXacMinh="Chưa xác minh";h.NgayXacMinh=null;h.MaNguoiDungXacMinh=null;h.MaLoiThucTe=null;h.TenLoiThucTeLuuTru=null;h.NguonDuLieu=TrainingDataSource.UserHistory;
        }
        await _context.SaveChangesAsync();await tx.CommitAsync();
        return RedirectToAction("Details","PhieuTiepNhan",new{id=ct.MaPhieu});
    }
}
public partial class TroLyChanDoanController
{
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles="Admin KTV,KTV")]
    public async Task<IActionResult> XacMinhNguyenNhan(int maLichSuChanDoan,int maLoiThucTe)
    {
        var h=await _context.LichSuChanDoans.Include(x=>x.PhieuTiepNhan).FirstOrDefaultAsync(x=>x.MaLichSuChanDoan==maLichSuChanDoan);
        if(h==null)return NotFound();
        if(!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var actor))return Forbid();
        if(!User.IsInRole("Admin KTV")&&h.PhieuTiepNhan?.MaUser_KTV!=actor)return Forbid();
        if(!await _context.Users.AnyAsync(x=>x.USERID==actor&&x.Status))return Forbid();
        if(!await _context.MaLois.AnyAsync(x=>x.MaLoiID==maLoiThucTe&&x.Status))return BadRequest("Mã lỗi không hợp lệ.");
        var checks=await _context.ChiTietSuaChuas.Where(x=>x.MaPhieu==h.MaPhieu).ToListAsync();
        if(checks.Any(x=>x.TrangThaiKiemTraLai=="Không đạt")||!checks.Any(x=>x.TrangThaiKiemTraLai=="Đạt"&&!string.IsNullOrWhiteSpace(x.ThamChieuBangChungKiemTraLai)&&x.ThoiGianKiemTraLai>=h.ThoiGian))return BadRequest("Chưa có kiểm tra lại đạt với bằng chứng phù hợp.");
        await using var tx=await _context.Database.BeginTransactionAsync();
        h.MaLoiThucTe=maLoiThucTe;h.TrangThaiXacMinh="Đã xác minh";h.NgayXacMinh=DateTime.Now;h.MaNguoiDungXacMinh=actor;h.NguonDuLieu=TrainingDataSource.RealRepair;
        h.TenLoiThucTeLuuTru=await _context.MaLois.Where(x=>x.MaLoiID==maLoiThucTe).Select(x=>x.TenLoi).SingleAsync();
        _context.LichSuTrangThais.Add(new LichSuTrangThai{MaPhieu=h.MaPhieu,Ma_KTV=actor,TrangThaiCu=h.PhieuTiepNhan!.TrangThai,TrangThaiMoi=h.PhieuTiepNhan.TrangThai,ThoiGian=DateTime.Now,LoaiSuKien="XacMinhNguyenNhan",DuLieuBoSungJSON=JsonSerializer.Serialize(new{h.MaLichSuChanDoan,h.MaLoiThucTe,h.MaNguoiDungXacMinh})});
        await _context.SaveChangesAsync();await tx.CommitAsync();return RedirectToAction("Details","PhieuTiepNhan",new{id=h.MaPhieu});
    }
}
