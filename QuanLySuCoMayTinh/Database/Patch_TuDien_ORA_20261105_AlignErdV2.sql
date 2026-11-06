-- Patch bổ sung từ điển từ khoảng trống phát hiện trên dữ liệu thật ORA (schema sau AlignErdV2). ĐỀ XUẤT, chưa kiểm chứng bằng ca thật. Idempotent.
SET NOCOUNT ON;
BEGIN TRANSACTION;
INSERT INTO TRIEU_CHUNG (TenTrieuChung, MoTa) SELECT v.a, v.b FROM (VALUES
(N'Quên mật khẩu hoặc không đăng nhập được Windows', N'Nhập mật khẩu không được, tài khoản bị khóa hoặc quên mã PIN, không vào được màn hình làm việc'),
(N'Cần sao lưu, khôi phục hoặc chuyển dữ liệu sang máy khác', N'Khách cần cứu dữ liệu, sao lưu trước khi sửa hoặc chuyển dữ liệu sang máy/ổ mới'),
(N'Cần cài lại hoặc nâng cấp hệ điều hành', N'Hệ điều hành quá cũ hoặc hết hỗ trợ, máy đầy rác hoặc lỗi lặt vặt, khách muốn cài lại Windows/Linux hoặc nâng cấp phiên bản'),
(N'Bấm nút nguồn không có phản ứng, không đèn', N'Nhấn nút nguồn nhiều lần nhưng máy không sáng đèn, không quạt quay, dù đã cắm sạc'),
(N'Ổ cứng hoặc SSD hỏng, cần thay ổ', N'Ổ lưu trữ không đọc được, báo hỏng hoặc đã được xác định phải thay ổ mới'),
(N'Bản lề hoặc vỏ máy bị gãy, nứt', N'Bản lề lỏng, kẹt hoặc gãy; vỏ máy nứt, bung ốc, nắp màn hình không giữ được góc mở'),
(N'Quạt bị kẹt hoặc không quay, cần bôi trơn hoặc thay quạt', N'Quạt phát tiếng lạ, quay yếu hoặc không quay do bụi, khô dầu hoặc hỏng ổ trục')
) v(a, b) WHERE NOT EXISTS (SELECT 1 FROM TRIEU_CHUNG t WHERE t.TenTrieuChung = v.a);
INSERT INTO MA_LOI (TenLoi, PhanLoai, TrangThaiSuDung) SELECT v.a, v.b, 1 FROM (VALUES
(N'Quên mật khẩu hoặc tài khoản Windows bị khóa', N'Hệ điều hành'),
(N'Cần sao lưu, khôi phục hoặc chuyển dữ liệu', N'Hệ điều hành'),
(N'Hệ điều hành cũ hoặc hỏng nặng, cần cài lại hoặc nâng cấp', N'Hệ điều hành'),
(N'Nút nguồn (power button) hỏng hoặc mất tiếp xúc', N'Nguồn & Pin'),
(N'Bản lề hoặc vỏ máy bị gãy, nứt', N'Phần cứng'),
(N'Quạt tản nhiệt kẹt hoặc hỏng, cần bôi trơn hoặc thay', N'Phần cứng')
) v(a, b) WHERE NOT EXISTS (SELECT 1 FROM MA_LOI m WHERE m.TenLoi = v.a);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung) SELECT MaLoi, N'1. Kiểm tra xem máy dùng tài khoản Microsoft hay tài khoản cục bộ.
2. Với tài khoản Microsoft: đặt lại mật khẩu tại account.live.com/password/reset bằng thiết bị khác.
3. Với tài khoản cục bộ: dùng câu hỏi bảo mật hoặc đĩa đặt lại mật khẩu đã tạo trước.
4. Nếu không còn cách: sao lưu dữ liệu (nếu được phép) rồi cài lại Windows.
5. Chỉ thực hiện khi xác minh khách là chủ máy; ghi lại vào phiếu.', 2, 1 FROM MA_LOI WHERE TenLoi = N'Quên mật khẩu hoặc tài khoản Windows bị khóa' AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoi);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung) SELECT MaLoi, N'1. Hỏi khách loại dữ liệu cần giữ và dung lượng; tuyệt đối không ghi thêm vào ổ nghi hỏng.
2. Tháo ổ hoặc khởi động từ USB cứu hộ để đọc dữ liệu, ưu tiên chép nguyên ổ (clone) trước.
3. Sao chép dữ liệu sang ổ ngoài đã được khách đồng ý.
4. Nếu ổ phát tiếng lạ hoặc không nhận: dừng lại, đề nghị dịch vụ cứu dữ liệu chuyên dụng.
5. Kiểm tra lại dữ liệu đã chép trước khi xóa hoặc sửa máy.', 3, 1 FROM MA_LOI WHERE TenLoi = N'Cần sao lưu, khôi phục hoặc chuyển dữ liệu' AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoi);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung) SELECT MaLoi, N'1. Sao lưu dữ liệu quan trọng của khách trước khi cài.
2. Kiểm tra máy đủ cấu hình cho phiên bản mới (Windows 11 cần TPM 2.0, Secure Boot).
3. Tạo USB cài đặt từ trang chính thức, khởi động từ USB.
4. Cài sạch, cài driver chipset, đồ họa, mạng từ trang của hãng.
5. Chạy Windows Update, cài lại phần mềm khách cần và bàn giao.', 2, 1 FROM MA_LOI WHERE TenLoi = N'Hệ điều hành cũ hoặc hỏng nặng, cần cài lại hoặc nâng cấp' AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoi);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung) SELECT MaLoi, N'1. Rút sạc, tháo pin (nếu tháo được), giữ nút nguồn 30 giây để xả điện dư rồi thử lại.
2. Thử nguồn bằng sạc khác để loại trừ lỗi sạc.
3. Mở máy, kiểm tra cáp/ruy băng nút nguồn có lỏng, gỉ hoặc đứt.
4. Dùng đồng hồ đo thông mạch nút nguồn; thay nút hoặc cáp nếu hỏng.
5. Nếu nút tốt mà vẫn không lên nguồn: chuyển sang kiểm tra bộ nguồn/IC sạc/mainboard.', 3, 1 FROM MA_LOI WHERE TenLoi = N'Nút nguồn (power button) hỏng hoặc mất tiếp xúc' AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoi);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung) SELECT MaLoi, N'1. Kiểm tra mức độ: bản lề chỉ lỏng hay đã gãy, vỏ nứt có làm đứt cáp màn hình không.
2. Không mở/đóng nắp mạnh để tránh làm đứt cáp màn hình.
3. Tháo vỏ, siết lại ốc bản lề; nếu gãy, thay bộ bản lề hoặc ốp vỏ đúng mã máy.
4. Có thể dán/gia cố vỏ nứt nhẹ bằng keo chuyên dụng nhưng cần báo khách là chỉ tạm thời.
5. Kiểm tra màn hình hiển thị bình thường sau khi lắp lại.', 3, 1 FROM MA_LOI WHERE TenLoi = N'Bản lề hoặc vỏ máy bị gãy, nứt' AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoi);
INSERT INTO GIAI_PHAP (MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung) SELECT MaLoi, N'1. Tắt máy, tháo vỏ đáy, vệ sinh bụi ở quạt và khe tản nhiệt.
2. Xoay cánh quạt bằng tay: nếu kẹt hoặc nặng thì bôi vài giọt dầu máy vào trục rồi thử lại.
3. Kiểm tra giắc cắm quạt trên mainboard.
4. Nếu quạt vẫn kêu, quay yếu hoặc không quay: thay quạt đúng mã máy.
5. Thay keo tản nhiệt nếu đã khô, kiểm tra nhiệt độ CPU/GPU khi chạy tải.', 2, 1 FROM MA_LOI WHERE TenLoi = N'Quạt tản nhiệt kẹt hoặc hỏng, cần bôi trơn hoặc thay' AND NOT EXISTS (SELECT 1 FROM GIAI_PHAP g WHERE g.MaLoi = MA_LOI.MaLoi);
INSERT INTO TRIEU_CHUNG_LOI (MaTrieuChung, MaLoi) SELECT t.MaTrieuChung, m.MaLoi FROM (VALUES
(N'Quên mật khẩu hoặc không đăng nhập được Windows', N'Quên mật khẩu hoặc tài khoản Windows bị khóa'),
(N'Cần sao lưu, khôi phục hoặc chuyển dữ liệu sang máy khác', N'Cần sao lưu, khôi phục hoặc chuyển dữ liệu'),
(N'File/thư mục bị mất hoặc corrupt', N'Cần sao lưu, khôi phục hoặc chuyển dữ liệu'),
(N'Cần cài lại hoặc nâng cấp hệ điều hành', N'Hệ điều hành cũ hoặc hỏng nặng, cần cài lại hoặc nâng cấp'),
(N'Windows Update bị lỗi', N'Hệ điều hành cũ hoặc hỏng nặng, cần cài lại hoặc nâng cấp'),
(N'Windows không khởi động được', N'Hệ điều hành cũ hoặc hỏng nặng, cần cài lại hoặc nâng cấp'),
(N'Bấm nút nguồn không có phản ứng, không đèn', N'Nút nguồn (power button) hỏng hoặc mất tiếp xúc'),
(N'Bấm nút nguồn không có phản ứng, không đèn', N'Bộ nguồn PSU bị hỏng hoặc không đủ công suất'),
(N'Ổ cứng hoặc SSD hỏng, cần thay ổ', N'Ổ cứng HDD bị bad sector (vật lý)'),
(N'Ổ cứng hoặc SSD hỏng, cần thay ổ', N'Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ'),
(N'Bản lề hoặc vỏ máy bị gãy, nứt', N'Bản lề hoặc vỏ máy bị gãy, nứt'),
(N'Quạt bị kẹt hoặc không quay, cần bôi trơn hoặc thay quạt', N'Quạt tản nhiệt kẹt hoặc hỏng, cần bôi trơn hoặc thay'),
(N'Quạt tản nhiệt kêu to liên tục', N'Quạt tản nhiệt kẹt hoặc hỏng, cần bôi trơn hoặc thay'),
(N'Máy bị nóng bất thường', N'Quạt tản nhiệt kẹt hoặc hỏng, cần bôi trơn hoặc thay')
) v(a, b) JOIN TRIEU_CHUNG t ON t.TenTrieuChung = v.a JOIN MA_LOI m ON m.TenLoi = v.b WHERE NOT EXISTS (SELECT 1 FROM TRIEU_CHUNG_LOI x WHERE x.MaTrieuChung = t.MaTrieuChung AND x.MaLoi = m.MaLoi);
COMMIT TRANSACTION;
