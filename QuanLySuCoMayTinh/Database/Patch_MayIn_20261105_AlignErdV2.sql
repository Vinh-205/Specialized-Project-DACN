-- Bổ sung mã lỗi "Driver máy in" còn thiếu trong database phát triển (schema sau AlignErdV2). Idempotent, nội dung lấy từ SeedData_TrieuChung_MaLoi_GiaiPhap.sql.
SET NOCOUNT ON;
BEGIN TRANSACTION;
INSERT INTO MA_LOI (TenLoi, PhanLoai, TrangThaiSuDung)
SELECT N'Driver máy in bị lỗi hoặc cổng in bị tắc', N'Phần mềm', 1
WHERE NOT EXISTS (SELECT 1 FROM MA_LOI WHERE TenLoi = N'Driver máy in bị lỗi hoặc cổng in bị tắc');
INSERT INTO GIAI_PHAP (MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung)
SELECT MaLoi, N'1. Tắt máy in, rút dây nguồn 30 giây, bật lại.
2. Xóa hàng đợi in: Services.msc → Print Spooler → Stop → xóa nội dung C:\Windows\System32\spool\PRINTERS → Start lại Print Spooler.
3. Gỡ driver máy in: Settings → Printers & scanners → chọn máy in → Remove device.
4. Tải và cài driver máy in mới từ trang chủ nhà sản xuất (HP, Canon, Epson, Brother...).
5. Thử kết nối USB trực tiếp thay vì qua WiFi (hoặc ngược lại).
6. Kiểm tra mực in, giấy, tình trạng kẹt giấy.
7. In test page: Printers & scanners → chọn máy in → Manage → Print a test page.', 1, 1
FROM MA_LOI WHERE TenLoi = N'Driver máy in bị lỗi hoặc cổng in bị tắc'
AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoi);
INSERT INTO TRIEU_CHUNG_LOI (MaTrieuChung, MaLoi)
SELECT t.MaTrieuChung, m.MaLoi FROM TRIEU_CHUNG t CROSS JOIN MA_LOI m
WHERE t.TenTrieuChung IN (N'Máy in không phản hồi', N'Driver thiết bị bị lỗi') AND m.TenLoi = N'Driver máy in bị lỗi hoặc cổng in bị tắc'
AND NOT EXISTS (SELECT 1 FROM TRIEU_CHUNG_LOI x WHERE x.MaTrieuChung = t.MaTrieuChung AND x.MaLoi = m.MaLoi);
COMMIT TRANSACTION;
