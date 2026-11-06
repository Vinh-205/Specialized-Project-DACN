# Dựng bản chạy riêng để xem giao diện: database LocalDB tạm + dữ liệu mẫu (FIXTURE) + website trên cổng 5299.
# Không đụng database của chủ dự án. Tài khoản mẫu: ui_admin / ui_ktv (mật khẩu FixtureOnly-2026!).
import os, subprocess, sys, time, urllib.request
from pathlib import Path
import http_workflow_main as h

DB = "ITHelpdesk_UI_20261105"
h.DB = DB
PORT = 5299
BUILD = Path(os.environ.get("BUILD_DIR", r"C:\Users\tuanv\AppData\Local\Temp\qlsc_build"))
env = dict(os.environ, ASPNETCORE_ENVIRONMENT="Production", ALLOW_ALIGN_ERDV2="1",
           ConnectionStrings__DefaultConnection=f"Server={h.SERVER};Database={DB};Trusted_Connection=True;TrustServerCertificate=True")


def start():
    log = open(h.OUT / "ui_server.log", "w", encoding="utf-8")
    p = subprocess.Popen(["dotnet", str(BUILD / "QuanLySuCoMayTinh.dll"), "--urls", f"http://127.0.0.1:{PORT}"], cwd=h.PROJ, env=env, stdout=log, stderr=subprocess.STDOUT)
    for _ in range(120):
        if p.poll() is not None:
            raise RuntimeError("website không khởi động được; xem ui_server.log")
        try:
            urllib.request.urlopen(f"http://127.0.0.1:{PORT}/Auth/Login", timeout=3); return p
        except Exception:
            time.sleep(1)
    raise RuntimeError("quá thời gian")


if os.environ.get("UI_REUSE") == "1":
    p = start(); open(h.OUT / "ui_server.pid", "w").write(str(p.pid)); print(f"READY http://127.0.0.1:{PORT} (dùng lại dữ liệu)", flush=True); p.wait(); sys.exit(0)
h.sql(f"IF DB_ID('{DB}') IS NOT NULL BEGIN ALTER DATABASE {DB} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {DB} END; CREATE DATABASE {DB}", db=None)
p = start(); p.terminate(); p.wait(timeout=20)   # lần 1: migration
PASS = h.PASS
h.sql(f"""
INSERT INTO NGUOI_DUNG(TenDangNhap,MatKhauBam,HoTen,VaiTro,TrangThaiHoatDong) VALUES (N'ui_admin',N'{PASS}',N'Trần Minh Quân',N'Admin KTV',1),(N'ui_ktv',N'{PASS}',N'Lê Thu Hà',N'KTV',1);
DECLARE @u1 int=(SELECT MaNguoiDung FROM NGUOI_DUNG WHERE TenDangNhap=N'ui_admin'), @u2 int=(SELECT MaNguoiDung FROM NGUOI_DUNG WHERE TenDangNhap=N'ui_ktv');
DECLARE @i int=1;
WHILE @i<=8 BEGIN
  INSERT INTO KHACH_HANG(HoTen,SoDienThoai,DiaChi) VALUES (CONCAT(N'Khách hàng mẫu ',@i), CONCAT(N'090000',RIGHT('0000'+CAST(@i AS varchar),4)), N'TP.HCM');
  SET @i+=1;
END
SET @i=1;
WHILE @i<=30 BEGIN
  DECLARE @kh int=(SELECT MaKhachHang FROM KHACH_HANG WHERE SoDienThoai=CONCAT(N'090000',RIGHT('0000'+CAST(((@i-1)%8)+1 AS varchar),4)));
  INSERT INTO THIET_BI(TenThietBi,CauHinh,MaKhachHang,NgoaiQuan) VALUES (CASE @i%4 WHEN 0 THEN N'Dell Inspiron 15' WHEN 1 THEN N'Laptop ASUS TUF F15' WHEN 2 THEN N'PC Gaming MSI' ELSE N'MacBook Air M1' END, N'Mẫu', @kh, N'');
  DECLARE @tb int=SCOPE_IDENTITY();
  DECLARE @st nvarchar(30)=CASE WHEN @i<=6 THEN N'Chờ khám' WHEN @i<=13 THEN CASE WHEN @i%2=0 THEN N'Đang kiểm tra' ELSE N'Đang sửa chữa' END ELSE N'Hoàn thành' END;
  INSERT INTO PHIEU_TIEP_NHAN(MaThietBi,MaNguoiDung,NgayNhan,TrangThai,MaLoi,MoTaSuCo,MucDoUuTien) VALUES (@tb, CASE WHEN @i%3=0 THEN @u2 ELSE @u1 END, DATEADD(day, -((@i-1)*6), SYSDATETIME()), @st, ((@i-1)%36)+1, N'Mẫu: máy gặp sự cố số '+CAST(@i AS nvarchar), CASE @i%3 WHEN 0 THEN N'Cao' WHEN 1 THEN N'Trung bình' ELSE N'Thấp' END);
  DECLARE @ph int=SCOPE_IDENTITY();
  IF @st=N'Hoàn thành' BEGIN
    INSERT INTO CHI_TIET_SUA_CHUA(MaPhieu,TenHangMuc,ChiPhi,DonGia,IsDeleted,ProcessingType,SoLuong) VALUES (@ph,N'Công sửa chữa',150000+(@i*37000)%900000,0,0,0,1);
    IF @i%2=0 INSERT INTO DANH_GIA_DICH_VU(MaPhieu,SoSao,BinhLuan,NgayDanhGia) VALUES (@ph, 3+(@i%3), N'Mẫu', SYSDATETIME());
  END
  SET @i+=1;
END
""")
p = start()
open(h.OUT / "ui_server.pid", "w").write(str(p.pid))
print(f"READY http://127.0.0.1:{PORT}  (tài khoản mẫu: ui_admin / {PASS})", flush=True)
p.wait()
