-- ============================================================
--  PATCH TỪ ĐIỂN CHẨN ĐOÁN — ĐỀ XUẤT, CẦN KTV/GVHD DUYỆT TRƯỚC KHI ÁP DỤNG
--  Ngày tạo: 2026-11-05. Bổ sung (không sửa/xóa dòng cũ), chạy lại nhiều lần an toàn (NOT EXISTS).
--  Mục tiêu: nối triệu chứng chưa liên kết, cấp triệu chứng cho lỗi chưa có, thêm triệu chứng phân biệt các cặp lỗi
--  không tách được bằng triệu chứng cũ. Liên kết dựa trên hiểu biết sửa chữa PC, CHƯA kiểm chứng bằng ca thật.
--  Tên bảng/cột theo schema HIỆN TẠI của seed (TenTC, MaTC, MaLoiID). KHÔNG chạy trên database nghiệp vụ khi chưa sao lưu.
--  Sau migration AlignErdV2 cột đổi tên (MaTrieuChung/TenTrieuChung/MaLoi): cần bản patch tương ứng.
-- ============================================================
SET NOCOUNT ON;
BEGIN TRANSACTION;

-- 1. Triệu chứng mới
INSERT INTO TRIEU_CHUNG (TenTC, MoTa)
SELECT v.TenTC, v.MoTa FROM (VALUES
(N'BSOD hiển thị mã dừng 0x0000007E', N'Màn hình xanh ghi SYSTEM_THREAD_EXCEPTION_NOT_HANDLED / mã 0x0000007E'),
(N'BSOD hiển thị mã dừng 0x000000D1', N'Màn hình xanh ghi DRIVER_IRQL_NOT_LESS_OR_EQUAL / mã 0x000000D1'),
(N'BSOD hiển thị mã dừng 0x00000050', N'Màn hình xanh ghi PAGE_FAULT_IN_NONPAGED_AREA / mã 0x00000050'),
(N'BSOD hiển thị mã dừng 0x0000001A', N'Màn hình xanh ghi MEMORY_MANAGEMENT / mã 0x0000001A'),
(N'BSOD hiển thị mã dừng 0xC000021A', N'Màn hình xanh ghi STATUS_SYSTEM_PROCESS_TERMINATED / mã 0xC000021A'),
(N'Cổng sạc lỏng, phải giữ hoặc lắc dây mới nhận sạc', N'Cắm sạc nhưng chập chờn, nhúc nhích dây thì lúc nhận lúc không'),
(N'Báo cắm sạc nhưng không sạc dù đổi sạc và cổng còn chắc chắn', N'Đã thử bộ sạc khác, đầu cắm chắc chắn nhưng pin vẫn không nạp'),
(N'Ứng dụng báo thiếu file .dll (VCRUNTIME/MSVCP) hoặc yêu cầu .NET Framework', N'Thông báo lỗi thiếu thư viện khi mở một ứng dụng cụ thể'),
(N'Cài đặt Windows/ứng dụng hệ thống không mở được sau cập nhật hoặc nhiễm virus', N'Settings, Control Panel hoặc ứng dụng hệ thống tự đóng hoặc không phản hồi'),
(N'Mất card WiFi sau khi cập nhật hoặc cài lại driver', N'Device Manager báo dấu chấm than hoặc không còn mục Wireless sau thao tác driver'),
(N'WiFi chỉ bắt được khi đứng sát router, tín hiệu yếu dù driver chuẩn', N'Driver nhận đúng nhưng tín hiệu yếu bất thường, nghi anten/card lỏng')
) v(TenTC, MoTa) WHERE NOT EXISTS (SELECT 1 FROM TRIEU_CHUNG t WHERE t.TenTC = v.TenTC);

-- 2. Mã lỗi mới
INSERT INTO MA_LOI (TenLoi, PhanLoai, Status)
SELECT v.TenLoi, v.PhanLoai, 1 FROM (VALUES
(N'Ổ đĩa đầy dung lượng, cần dọn dẹp hoặc nâng cấp ổ', N'Hệ điều hành'),
(N'Module hoặc driver Bluetooth bị lỗi', N'Phần cứng'),
(N'Webcam bị hỏng hoặc bị chặn quyền, driver webcam lỗi', N'Phần cứng'),
(N'Loa hoặc jack âm thanh bị hỏng vật lý', N'Phần cứng')
) v(TenLoi, PhanLoai) WHERE NOT EXISTS (SELECT 1 FROM MA_LOI m WHERE m.TenLoi = v.TenLoi);

-- 3. Giải pháp cho mã lỗi mới (MucDoKho: 1=Dễ, 2=Trung bình, 3=Khó, 4=Chuyên gia)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID, N'1. Mở This PC, kiểm tra dung lượng trống của ổ hệ thống.
2. Dùng Storage Sense hoặc Disk Cleanup để xóa tệp tạm, thùng rác, bản cập nhật cũ.
3. Dùng công cụ phân tích dung lượng để tìm thư mục lớn, di chuyển dữ liệu sang ổ khác hoặc lưu trữ ngoài.
4. Gỡ ứng dụng không dùng.
5. Nếu vẫn thiếu chỗ → nâng cấp dung lượng ổ cứng/SSD.', 1, 1 FROM MA_LOI WHERE TenLoi = N'Ổ đĩa đầy dung lượng, cần dọn dẹp hoặc nâng cấp ổ'
AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoiID);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID, N'1. Bật Bluetooth trong Settings và kiểm tra chế độ máy bay đã tắt.
2. Mở Device Manager, xem mục Bluetooth có dấu chấm than không.
3. Gỡ và cài lại driver Bluetooth từ trang chủ hãng máy.
4. Khởi động lại dịch vụ Bluetooth Support Service.
5. Nếu vẫn lỗi → kiểm tra module Bluetooth/WiFi combo, thay module.', 2, 1 FROM MA_LOI WHERE TenLoi = N'Module hoặc driver Bluetooth bị lỗi'
AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoiID);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID, N'1. Kiểm tra Settings > Privacy > Camera đã cho phép ứng dụng truy cập.
2. Kiểm tra phím tắt/khóa webcam của hãng máy.
3. Mở Device Manager, kiểm tra mục Camera, cập nhật hoặc cài lại driver.
4. Thử webcam trên ứng dụng khác để loại trừ lỗi ứng dụng.
5. Nếu vẫn không nhận → kiểm tra cáp webcam trong màn hình laptop, thay module webcam.', 2, 1 FROM MA_LOI WHERE TenLoi = N'Webcam bị hỏng hoặc bị chặn quyền, driver webcam lỗi'
AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoiID);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID, N'1. Thử tai nghe/loa ngoài để xác định lỗi ở loa trong hay mạch âm thanh.
2. Làm sạch jack âm thanh, kiểm tra có bị rè do bụi hoặc tiếp xúc kém.
3. Kiểm tra cáp loa trong máy.
4. Nếu loa rè hoặc méo ở mọi mức âm lượng → thay loa.
5. Nếu cả tai nghe cũng lỗi → kiểm tra mạch audio trên mainboard.', 3, 1 FROM MA_LOI WHERE TenLoi = N'Loa hoặc jack âm thanh bị hỏng vật lý'
AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoiID);

-- 4. Liên kết triệu chứng - mã lỗi
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM (VALUES
(N'Màn hình bị vỡ / đập vỡ vật lý', N'Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền'),
(N'Phân giải màn hình bị đặt sai', N'Driver đồ họa bị lỗi hoặc không tương thích'),
(N'Dead pixel / điểm chết trên màn hình', N'Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền'),
(N'Màn hình đơn sắc (xanh/đỏ/xanh lá)', N'GPU/Card đồ họa bị lỗi hoặc slot PCIe hỏng'),
(N'Màn hình đơn sắc (xanh/đỏ/xanh lá)', N'Cáp tín hiệu màn hình (HDMI/DP/VGA) lỏng hoặc hỏng'),
(N'Màn hình đơn sắc (xanh/đỏ/xanh lá)', N'Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền'),
(N'Sạc đúng loại nhưng không nhận sạc', N'IC sạc (Charging IC) trên mainboard bị lỗi'),
(N'Sạc đúng loại nhưng không nhận sạc', N'Cổng sạc DC-in bị lỏng hoặc hỏng'),
(N'Máy bị đơ (Freeze/Hang)', N'Hệ thống tản nhiệt bị nghẹt bụi, quá nhiệt CPU/GPU'),
(N'Máy bị đơ (Freeze/Hang)', N'RAM lỏng slot hoặc bị lỗi chip nhớ'),
(N'Máy bị đơ (Freeze/Hang)', N'Ổ cứng HDD bị bad sector (vật lý)'),
(N'Máy bị đơ (Freeze/Hang)', N'Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ'),
(N'Máy bị đơ (Freeze/Hang)', N'Xung đột phần mềm hoặc driver sau Windows Update'),
(N'Xuất hiện thông báo "Low Disk Space"', N'Ổ đĩa đầy dung lượng, cần dọn dẹp hoặc nâng cấp ổ'),
(N'Ổ cứng bị kêu lạ (click, grind)', N'Ổ cứng HDD bị bad sector (vật lý)'),
(N'Máy không nhận ổ cứng khi khởi động', N'Ổ cứng HDD bị bad sector (vật lý)'),
(N'Máy không nhận ổ cứng khi khởi động', N'Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ'),
(N'Máy không nhận ổ cứng khi khởi động', N'MBR/GPT bị ghi đè hoặc hỏng'),
(N'File/thư mục bị mất hoặc corrupt', N'Ổ cứng HDD bị bad sector (vật lý)'),
(N'File/thư mục bị mất hoặc corrupt', N'File hệ thống bị corrupt (chkdsk lỗi)'),
(N'File/thư mục bị mất hoặc corrupt', N'Nhiễm Ransomware (mã hóa file đòi tiền chuộc)'),
(N'SSD chậm hơn dự kiến', N'Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ'),
(N'Windows Update bị lỗi', N'Xung đột phần mềm hoặc driver sau Windows Update'),
(N'Windows Update bị lỗi', N'File hệ thống bị corrupt (chkdsk lỗi)'),
(N'Desktop/icon bị mất sau khởi động', N'Windows Registry bị lỗi sau cập nhật hoặc virus'),
(N'Desktop/icon bị mất sau khởi động', N'File hệ thống bị corrupt (chkdsk lỗi)'),
(N'Driver thiết bị bị lỗi', N'Driver đồ họa bị lỗi hoặc không tương thích'),
(N'Driver thiết bị bị lỗi', N'Xung đột phần mềm hoặc driver sau Windows Update'),
(N'Driver thiết bị bị lỗi', N'Driver WiFi card bị lỗi hoặc không tương thích'),
(N'Driver thiết bị bị lỗi', N'Driver Ethernet card bị lỗi hoặc hỏng card LAN'),
(N'Driver thiết bị bị lỗi', N'Driver touchpad bị lỗi hoặc touchpad bị khóa'),
(N'Driver thiết bị bị lỗi', N'Driver âm thanh (audio driver) bị lỗi'),
(N'Driver thiết bị bị lỗi', N'Lỗi driver USB hoặc USB hub trên mainboard'),
(N'Driver thiết bị bị lỗi', N'Driver máy in bị lỗi hoặc cổng in bị tắc'),
(N'WiFi yếu, hay bị ngắt kết nối', N'Driver WiFi card bị lỗi hoặc không tương thích'),
(N'WiFi yếu, hay bị ngắt kết nối', N'WiFi card bị lỏng hoặc hỏng anten'),
(N'Bluetooth không kết nối được thiết bị', N'Module hoặc driver Bluetooth bị lỗi'),
(N'Âm thanh bị rè, méo', N'Driver âm thanh (audio driver) bị lỗi'),
(N'Âm thanh bị rè, méo', N'Loa hoặc jack âm thanh bị hỏng vật lý'),
(N'Webcam không hoạt động', N'Webcam bị hỏng hoặc bị chặn quyền, driver webcam lỗi'),
(N'Loa không có âm thanh', N'Loa hoặc jack âm thanh bị hỏng vật lý'),
(N'Máy chạy chậm bất thường', N'Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ'),
(N'Máy bật lên lại tắt ngay (boot loop)', N'Lỗi BSOD 0xC000021A: STATUS_SYSTEM_PROCESS_TERMINATED'),
(N'Máy bật lên lại tắt ngay (boot loop)', N'Xung đột phần mềm hoặc driver sau Windows Update'),
(N'Màn hình xanh chết chóc (BSOD)', N'Lỗi BSOD 0xC000021A: STATUS_SYSTEM_PROCESS_TERMINATED'),
(N'Màn hình xanh chết chóc (BSOD)', N'Xung đột phần mềm hoặc driver sau Windows Update'),
(N'Windows không khởi động được', N'Lỗi BSOD 0xC000021A: STATUS_SYSTEM_PROCESS_TERMINATED'),
(N'Ứng dụng bị crash liên tục', N'Xung đột phần mềm hoặc driver sau Windows Update'),
(N'BSOD hiển thị mã dừng 0x0000007E', N'Lỗi BSOD 0x0000007E: SYSTEM_THREAD_EXCEPTION_NOT_HANDLED'),
(N'BSOD hiển thị mã dừng 0x000000D1', N'Lỗi BSOD 0x000000D1: DRIVER_IRQL_NOT_LESS_OR_EQUAL'),
(N'BSOD hiển thị mã dừng 0x00000050', N'Lỗi BSOD 0x00000050: PAGE_FAULT_IN_NONPAGED_AREA'),
(N'BSOD hiển thị mã dừng 0x0000001A', N'Lỗi BSOD 0x0000001A: MEMORY_MANAGEMENT'),
(N'BSOD hiển thị mã dừng 0xC000021A', N'Lỗi BSOD 0xC000021A: STATUS_SYSTEM_PROCESS_TERMINATED'),
(N'Cổng sạc lỏng, phải giữ hoặc lắc dây mới nhận sạc', N'Cổng sạc DC-in bị lỏng hoặc hỏng'),
(N'Báo cắm sạc nhưng không sạc dù đổi sạc và cổng còn chắc chắn', N'IC sạc (Charging IC) trên mainboard bị lỗi'),
(N'Ứng dụng báo thiếu file .dll (VCRUNTIME/MSVCP) hoặc yêu cầu .NET Framework', N'Thiếu Visual C++ Redistributable hoặc .NET Framework'),
(N'Cài đặt Windows/ứng dụng hệ thống không mở được sau cập nhật hoặc nhiễm virus', N'Windows Registry bị lỗi sau cập nhật hoặc virus'),
(N'Mất card WiFi sau khi cập nhật hoặc cài lại driver', N'Driver WiFi card bị lỗi hoặc không tương thích'),
(N'WiFi chỉ bắt được khi đứng sát router, tín hiệu yếu dù driver chuẩn', N'WiFi card bị lỏng hoặc hỏng anten')
) v(TenTC, TenLoi)
JOIN TRIEU_CHUNG TC ON TC.TenTC = v.TenTC JOIN MA_LOI ML ON ML.TenLoi = v.TenLoi
WHERE NOT EXISTS (SELECT 1 FROM TRIEU_CHUNG_LOI x WHERE x.MaTC = TC.MaTC AND x.MaLoi = ML.MaLoiID);

COMMIT TRANSACTION;
