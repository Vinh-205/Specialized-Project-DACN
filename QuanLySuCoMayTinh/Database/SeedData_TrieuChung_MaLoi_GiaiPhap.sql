-- ============================================================
--  SEED DATA: Triệu chứng, Mã lỗi, Giải pháp
--  Nguồn: Tổng hợp từ Microsoft Docs, iFixit, Tom's Hardware,
--         Reddit r/techsupport, HP/Dell/Lenovo Support KB
--  Ngày tạo: 2026-05-26
-- ============================================================
SET NOCOUNT ON;
BEGIN TRANSACTION;
-- ============================================================
-- 1. TRIỆU CHỨNG (TRIEU_CHUNG)
-- ============================================================
-- Nhóm 1: Màn hình
INSERT INTO TRIEU_CHUNG (TenTC, MoTa) VALUES
(N'Màn hình không lên sau khi bật máy', N'Máy vẫn chạy nhưng màn hình tối đen, không có tín hiệu'),
(N'Màn hình nhấp nháy / bị giật', N'Hình ảnh trên màn hình nhấp nháy liên tục hoặc giật nhẹ'),
(N'Màn hình có sọc ngang hoặc dọc', N'Xuất hiện các đường kẻ màu bất thường chạy ngang hoặc dọc'),
(N'Màn hình bị mờ, tối hơn bình thường', N'Độ sáng màn hình bị giảm đột ngột, khó nhìn rõ nội dung'),
(N'Màn hình bị vỡ / đập vỡ vật lý', N'Màn hình bị nứt, vỡ kính, hiện hình ảnh bị lệch hoặc loang màu'),
(N'Phân giải màn hình bị đặt sai', N'Màn hình hiển thị với độ phân giải thấp, hình bị vỡ pixcel'),
(N'Dead pixel / điểm chết trên màn hình', N'Xuất hiện các chấm đen hoặc màu cố định không thay đổi trên màn hình'),
(N'Màn hình đơn sắc (xanh/đỏ/xanh lá)', N'Toàn bộ màn hình hiển thị một màu đơn sắc bất thường');
-- Nhóm 2: Nguồn & Pin
INSERT INTO TRIEU_CHUNG (TenTC, MoTa) VALUES
(N'Máy không bật được', N'Nhấn nút nguồn nhưng máy không phản ứng gì'),
(N'Máy tự tắt đột ngột', N'Máy đang dùng thì tự nhiên tắt ngay, không thông báo'),
(N'Máy bật lên lại tắt ngay (boot loop)', N'Máy liên tục khởi động rồi tắt, không vào được hệ điều hành'),
(N'Pin không sạc được', N'Cắm sạc vào nhưng icon pin không hiện đang sạc, không tăng %'),
(N'Pin hao nhanh bất thường', N'Thời lượng pin sụt giảm đột ngột, sạc đầy nhưng chỉ dùng được vài giờ'),
(N'Máy bị nóng bất thường', N'Vỏ máy hoặc bàn phím nóng rát, quạt tản nhiệt kêu to'),
(N'Sạc đúng loại nhưng không nhận sạc', N'Máy báo "Đang không sạc" hoặc "Kết nối sạc nhưng không sạc"');
-- Nhóm 3: Bộ nhớ & Lưu trữ
INSERT INTO TRIEU_CHUNG (TenTC, MoTa) VALUES
(N'Máy chạy chậm bất thường', N'Máy phản hồi rất chậm, mở ứng dụng lâu, đơ thường xuyên'),
(N'Máy bị đơ (Freeze/Hang)', N'Màn hình đứng yên, chuột bàn phím không phản hồi'),
(N'Màn hình xanh chết chóc (BSOD)', N'Máy hiện màn hình xanh với mã lỗi và khởi động lại'),
(N'Xuất hiện thông báo "Low Disk Space"', N'Windows cảnh báo ổ đĩa gần đầy'),
(N'Ổ cứng bị kêu lạ (click, grind)', N'Nghe thấy tiếng kêu click, lạch cạch hoặc mài từ ổ cứng'),
(N'Máy không nhận ổ cứng khi khởi động', N'BIOS không tìm thấy ổ cứng, thông báo "No Boot Device"'),
(N'File/thư mục bị mất hoặc corrupt', N'Các file không mở được, báo lỗi dữ liệu hoặc biến mất'),
(N'SSD chậm hơn dự kiến', N'Tốc độ đọc/ghi SSD thấp hơn thông số nhà sản xuất nhiều');
-- Nhóm 4: Hệ điều hành & Phần mềm
INSERT INTO TRIEU_CHUNG (TenTC, MoTa) VALUES
(N'Windows không khởi động được', N'Máy không vào được Windows, dừng ở màn hình loading'),
(N'Bị lỗi "Missing operating system"', N'Thông báo "Missing operating system" hoặc "BOOTMGR is missing"'),
(N'Windows Update bị lỗi', N'Bản cập nhật Windows không cài được, bị treo giữa chừng'),
(N'Ứng dụng bị crash liên tục', N'Phần mềm tự đóng đột ngột, hiện thông báo lỗi hoặc "Not Responding"'),
(N'Máy bị nhiễm virus/malware', N'Xuất hiện quảng cáo lạ, máy chậm, file bị mã hóa đòi tiền chuộc'),
(N'Desktop/icon bị mất sau khởi động', N'Sau khi khởi động, màn hình nền trống, không có biểu tượng nào'),
(N'Driver thiết bị bị lỗi', N'Thiết bị xuất hiện dấu chấm than vàng trong Device Manager');
-- Nhóm 5: Mạng & Kết nối
INSERT INTO TRIEU_CHUNG (TenTC, MoTa) VALUES
(N'Không kết nối được WiFi', N'Máy không tìm thấy mạng WiFi hoặc kết nối nhưng không có internet'),
(N'WiFi yếu, hay bị ngắt kết nối', N'Tín hiệu WiFi lúc có lúc không, hay bị mất kết nối tự nhiên'),
(N'Không kết nối được mạng LAN (dây)', N'Cắm cáp mạng nhưng máy không nhận, không có internet'),
(N'Không vào được một số trang web', N'Trình duyệt báo lỗi DNS hoặc "This site can''t be reached"'),
(N'Bluetooth không kết nối được thiết bị', N'Không ghép nối được với loa, tai nghe, chuột Bluetooth');
-- Nhóm 6: Phần cứng khác
INSERT INTO TRIEU_CHUNG (TenTC, MoTa) VALUES
(N'Bàn phím một số phím bị liệt', N'Một số phím bấm không có tác dụng hoặc bấm ra ký tự sai'),
(N'Touchpad không hoạt động', N'Bàn di chuột không phản hồi khi chạm hoặc vuốt'),
(N'Loa không có âm thanh', N'Không nghe thấy âm thanh từ loa máy tính mặc dù âm lượng đã bật'),
(N'Âm thanh bị rè, méo', N'Âm thanh phát ra bị rè, bị méo, nghe không rõ'),
(N'Webcam không hoạt động', N'Ứng dụng không nhận được webcam, hình ảnh đen hoặc bị vỡ'),
(N'Cổng USB không nhận thiết bị', N'Cắm USB vào nhưng máy không phát hiện thiết bị'),
(N'Máy in không phản hồi', N'Gửi lệnh in nhưng máy in không làm gì, trạng thái offline'),
(N'Quạt tản nhiệt kêu to liên tục', N'Quạt chạy 100% công suất liên tục kể cả khi máy nhàn rỗi'),
(N'RAM bị lỗi / không ổn định', N'Máy thường xuyên BSOD với lỗi khác nhau, random crash');
-- ============================================================
-- 2. MÃ LỖI (MA_LOI)
-- Cấu trúc: TenLoi, PhanLoai, Status=1 (active)
-- PhanLoai: 'Phần cứng', N'Hệ điều hành', N'Phần mềm', N'Mạng', N'Nguồn & Pin'
-- ============================================================
INSERT INTO MA_LOI (TenLoi, PhanLoai, Status) VALUES
-- Màn hình
(N'GPU/Card đồ họa bị lỗi hoặc slot PCIe hỏng', N'Phần cứng', 1),
(N'Cáp tín hiệu màn hình (HDMI/DP/VGA) lỏng hoặc hỏng', N'Phần cứng', 1),
(N'Driver đồ họa bị lỗi hoặc không tương thích', N'Hệ điều hành', 1),
(N'Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền', N'Phần cứng', 1),
(N'Kết nối nội bộ màn hình laptop bị lỏng', N'Phần cứng', 1),
-- Nguồn & Pin
(N'Bộ nguồn PSU bị hỏng hoặc không đủ công suất', N'Nguồn & Pin', 1),
(N'Pin Lithium bị phồng, chai hoặc hỏng tế bào', N'Nguồn & Pin', 1),
(N'IC sạc (Charging IC) trên mainboard bị lỗi', N'Nguồn & Pin', 1),
(N'Cổng sạc DC-in bị lỏng hoặc hỏng', N'Nguồn & Pin', 1),
(N'Hệ thống tản nhiệt bị nghẹt bụi, quá nhiệt CPU/GPU', N'Phần cứng', 1),
(N'Thermal paste (keo tản nhiệt) bị khô, cần thay mới', N'Phần cứng', 1),
-- Bộ nhớ & Lưu trữ
(N'RAM lỏng slot hoặc bị lỗi chip nhớ', N'Phần cứng', 1),
(N'Ổ cứng HDD bị bad sector (vật lý)', N'Phần cứng', 1),
(N'Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ', N'Phần cứng', 1),
(N'File hệ thống bị corrupt (chkdsk lỗi)', N'Hệ điều hành', 1),
(N'MBR/GPT bị ghi đè hoặc hỏng', N'Hệ điều hành', 1),
-- Hệ điều hành & Phần mềm
(N'BOOTMGR bị hỏng hoặc mất', N'Hệ điều hành', 1),
(N'Windows Registry bị lỗi sau cập nhật hoặc virus', N'Hệ điều hành', 1),
(N'Xung đột phần mềm hoặc driver sau Windows Update', N'Hệ điều hành', 1),
(N'Nhiễm Ransomware (mã hóa file đòi tiền chuộc)', N'Phần mềm', 1),
(N'Nhiễm Adware/Spyware (hiện quảng cáo, thu thập dữ liệu)', N'Phần mềm', 1),
(N'Thiếu Visual C++ Redistributable hoặc .NET Framework', N'Phần mềm', 1),
(N'Lỗi BSOD 0x0000007E: SYSTEM_THREAD_EXCEPTION_NOT_HANDLED', N'Hệ điều hành', 1),
(N'Lỗi BSOD 0x000000D1: DRIVER_IRQL_NOT_LESS_OR_EQUAL', N'Hệ điều hành', 1),
(N'Lỗi BSOD 0xC000021A: STATUS_SYSTEM_PROCESS_TERMINATED', N'Hệ điều hành', 1),
(N'Lỗi BSOD 0x0000001A: MEMORY_MANAGEMENT', N'Hệ điều hành', 1),
(N'Lỗi BSOD 0x00000050: PAGE_FAULT_IN_NONPAGED_AREA', N'Hệ điều hành', 1),
-- Mạng
(N'Driver WiFi card bị lỗi hoặc không tương thích', N'Mạng', 1),
(N'WiFi card bị lỏng hoặc hỏng anten', N'Phần cứng', 1),
(N'Lỗi cài đặt DNS sai hoặc DNS bị cache', N'Mạng', 1),
(N'TCP/IP stack bị hỏng (Winsock corrupt)', N'Mạng', 1),
(N'Driver Ethernet card bị lỗi hoặc hỏng card LAN', N'Mạng', 1),
-- Phần cứng khác
(N'Bàn phím bị tràn nước hoặc hỏng mạch', N'Phần cứng', 1),
(N'Driver touchpad bị lỗi hoặc touchpad bị khóa', N'Phần cứng', 1),
(N'Driver âm thanh (audio driver) bị lỗi', N'Phần mềm', 1),
(N'Lỗi driver USB hoặc USB hub trên mainboard', N'Phần cứng', 1),
(N'Driver máy in bị lỗi hoặc cổng in bị tắc', N'Phần mềm', 1);
-- ============================================================
-- 3. GIẢI PHÁP (GIAI_PHAP)
-- Mỗi mã lỗi có ít nhất 1 giải pháp
-- MucDoKho: 1=Dễ, 2=Trung bình, 3=Khó, 4=Chuyên gia
-- ============================================================
-- Giải pháp cho: GPU/Card đồ họa bị lỗi (MaLoi = 1)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID, 
N'1. Tắt máy hoàn toàn và rút dây nguồn.
2. Mở thùng máy, xác định vị trí card đồ họa rời (nếu có).
3. Rút card ra khỏi khe PCIe, dùng tẩy gôm hoặc cọ sạch tiếp điểm vàng.
4. Kiểm tra khe PCIe trên mainboard có bụi bẩn không, thổi sạch bằng cồn IPA.
5. Cắm lại card chắc chắn, đảm bảo chốt khe PCIe đã bật lên.
6. Cắm đủ dây nguồn 6-pin/8-pin vào card nếu cần.
7. Bật máy và cài lại driver từ trang chủ NVIDIA/AMD (DDU để gỡ driver cũ trước).
8. Nếu vẫn lỗi → thay card đồ họa.',
3, 1
FROM MA_LOI WHERE TenLoi = N'GPU/Card đồ họa bị lỗi hoặc slot PCIe hỏng';
-- Giải pháp cho: Cáp tín hiệu màn hình lỏng/hỏng (MaLoi = 2)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Kiểm tra cả 2 đầu cáp HDMI/DisplayPort/VGA (đầu máy tính và đầu màn hình).
2. Rút cáp ra và cắm lại chắc chắn, vặn vít cố định nếu có.
3. Thử cổng output khác trên máy tính (ví dụ từ VGA sang HDMI).
4. Thử cáp khác để loại trừ khả năng cáp hỏng.
5. Test màn hình với máy tính khác để xác định nguyên nhân.
6. Nếu cáp hỏng → mua cáp mới (ưu tiên HDMI 2.0 hoặc DP 1.4).
7. Với laptop: kiểm tra cáp LVDS/eDP nội bộ, cần tháo máy.',
1, 1
FROM MA_LOI WHERE TenLoi = N'Cáp tín hiệu màn hình (HDMI/DP/VGA) lỏng hoặc hỏng';
-- Giải pháp cho: Driver đồ họa bị lỗi (MaLoi = 3)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tải công cụ DDU (Display Driver Uninstaller) từ trang wagnardsoft.com.
2. Khởi động Windows ở chế độ Safe Mode.
3. Chạy DDU, chọn GPU của bạn (NVIDIA/AMD/Intel), chọn "Clean and Restart".
4. Sau khi restart, tải driver mới nhất từ:
   - NVIDIA: nvidia.com/drivers
   - AMD: amd.com/support  
   - Intel: intel.com/content/drivers
5. Cài đặt driver và khởi động lại máy.
6. Nếu lỗi vẫn còn → thử roll back về driver cũ hơn (phiên bản trước đó).',
2, 1
FROM MA_LOI WHERE TenLoi = N'Driver đồ họa bị lỗi hoặc không tương thích';
-- Giải pháp cho: Màn hình LCD hỏng đèn nền (MaLoi = 4)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Sử dụng đèn pin rọi vào màn hình trong phòng tối để kiểm tra: nếu thấy hình mờ thì đèn nền hỏng, không phải màn hình hỏng hoàn toàn.
2. Kiểm tra cáp nối màn hình với mainboard bên trong laptop (yêu cầu tháo máy).
3. Nếu cáp tốt → lỗi do:
   a. Inverter board (màn hình CCFL cũ) → thay inverter.
   b. LED driver trên bo mạch màn hình → thay màn hình.
4. Đặt mua màn hình thay thế đúng model (tra model number phía sau màn hình).
5. Thay màn hình theo hướng dẫn iFixit.com theo model máy.',
4, 1
FROM MA_LOI WHERE TenLoi = N'Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền';
-- Giải pháp cho: Kết nối nội bộ màn hình laptop lỏng (MaLoi = 5)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tắt máy, rút pin (nếu được).
2. Mở nắp màn hình, tháo viền nhựa xung quanh màn hình.
3. Xác định cáp kết nối màn hình (cáp LVDS/eDP) và cáp camera/WiFi.
4. Kiểm tra đầu cắm của cáp vào bo mạch màn hình và mainboard.
5. Rút và cắm lại chắc chắn, dán keo chịu nhiệt 2 mặt để cố định nếu lỏng.
6. Lắp lại viền và test trước khi vặn vít hoàn toàn.
7. Nếu cáp đứt ngầm (uốn nhiều lần) → đặt cáp thay thế đúng model.',
4, 1
FROM MA_LOI WHERE TenLoi = N'Kết nối nội bộ màn hình laptop bị lỏng';
-- Giải pháp cho: PSU hỏng (MaLoi = 6)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Test PSU bằng cách paperclip test: dùng dây kẹp giấy nối chân màu xanh lá (PS_ON) và chân đen (GND) trên đầu cắm 24-pin. Nếu quạt PSU không quay = PSU chết.
2. Dùng máy đo PSU chuyên dụng để đo điện áp các đường +12V/+5V/+3.3V.
3. Nếu PSU hỏng → thay PSU mới có công suất phù hợp (tính công suất theo GPU + CPU).
4. Khuyến nghị: dùng PSU đạt chứng nhận 80 PLUS (Bronze trở lên).
5. Đảm bảo cắm đủ connector: dây 24-pin mainboard, dây CPU 4/8-pin.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Bộ nguồn PSU bị hỏng hoặc không đủ công suất';
-- Giải pháp cho: Pin bị phồng/chai (MaLoi = 7)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Nếu pin phồng: KHÔNG sử dụng tiếp, pin phồng có nguy cơ cháy nổ.
2. Tắt máy ngay lập tức, không sạc thêm.
3. Đặt máy ở nơi thoáng mát, tránh xa vật dễ cháy.
4. Đặt mua pin chính hãng hoặc pin tương thích chất lượng cao (tránh pin giá rẻ không rõ nguồn gốc).
5. Tháo pin cũ: với pin rời thì tháo trực tiếp; với pin dán (built-in) cần dụng cụ mở máy chuyên dụng.
6. Pin chai (dung lượng < 40% so với ban đầu): thay pin mới.
7. Để kiểm tra sức khỏe pin: dùng BatteryReport (powercfg /batteryreport) hoặc HWiNFO64.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Pin Lithium bị phồng, chai hoặc hỏng tế bào';
-- Giải pháp cho: IC sạc hỏng (MaLoi = 8)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Thử sạc bằng adapter khác cùng công suất để loại trừ lỗi adapter.
2. Kiểm tra cổng sạc xem có bụi bẩn, cong vênh chân tiếp xúc không.
3. Thổi sạch cổng sạc bằng khí nén.
4. Nếu dùng sạc USB-C: thử cổng USB-C khác trên máy.
5. Nếu vẫn không sạc: cần đo dòng sạc bằng đồng hồ vạn năng hoặc USB meter.
6. Lỗi IC sạc đòi hỏi hàn chip SMD trên mainboard → yêu cầu kỹ thuật viên có kinh nghiệm microsoldering.',
4, 1
FROM MA_LOI WHERE TenLoi = N'IC sạc (Charging IC) trên mainboard bị lỗi';
-- Giải pháp cho: Cổng sạc DC-in hỏng (MaLoi = 9)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Kiểm tra đầu sạc có bị cong, lệch, gãy chân không.
2. Dùng đèn pin soi vào cổng sạc trên máy, xem có chân cong, vỡ nhựa không.
3. Thử cắm sạc ở nhiều góc độ khác nhau để xác nhận tiếp xúc lỏng.
4. Tháo máy và kiểm tra bo mạch DC-in jack:
   a. Nếu cổng sạc rời: tháo và hàn/thay cổng mới.
   b. Nếu cổng sạc liền mainboard: cần hàn lại chân sạc hoặc thay mainboard.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Cổng sạc DC-in bị lỏng hoặc hỏng';
-- Giải pháp cho: Quá nhiệt do bụi (MaLoi = 10)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tắt máy và rút nguồn hoàn toàn.
2. Mở vỏ máy tính (laptop: tháo nắp lưng, PC: mở thùng máy).
3. Dùng bình khí nén thổi sạch bụi trong các cánh quạt, tản nhiệt, lưới lọc bụi.
4. Chú ý thổi từ trong ra ngoài để đẩy bụi ra theo lỗ thông gió.
5. Dùng cọ mềm để làm sạch các vùng khó tiếp cận.
6. Kiểm tra quạt: quay tay thử xem có trơn tru không, thay quạt nếu cần.
7. Sau khi lắp lại, dùng HWiNFO64 hoặc Core Temp để theo dõi nhiệt độ.
8. Nhiệt độ CPU idle <50°C, load <85°C là bình thường.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Hệ thống tản nhiệt bị nghẹt bụi, quá nhiệt CPU/GPU';
-- Giải pháp cho: Thermal paste khô (MaLoi = 11)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tháo cụm tản nhiệt ra khỏi CPU/GPU.
2. Dùng cồn isopropyl 90%+ và tăm bông lau sạch thermal paste cũ trên cả CPU và mặt đế tản nhiệt.
3. Đợi khô hoàn toàn.
4. Bôi thermal paste mới (khuyến nghị: Thermal Grizzly Kryonaut, Arctic MX-4, hoặc Noctua NT-H1).
5. Lượng paste: bằng hạt gạo đặt giữa CPU. Không bôi quá nhiều.
6. Lắp tản nhiệt lại, vặn vít theo hình chữ X (không vặn một vít hết rồi mới qua vít khác).
7. Kiểm tra nhiệt độ sau 30 phút chạy load cao.
Lưu ý: nên thay thermal paste mỗi 2-3 năm.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Thermal paste (keo tản nhiệt) bị khô, cần thay mới';
-- Giải pháp cho: RAM lỗi (MaLoi = 12)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tắt máy, rút nguồn/pin.
2. Tháo tất cả thanh RAM ra.
3. Dùng tẩy gôm chà nhẹ các tiếp điểm vàng của RAM, thổi sạch bụi trong khe RAM.
4. Cắm lại từng thanh RAM một, test thử từng thanh để tìm thanh hỏng.
5. Thử cắm RAM vào khe khác nếu có nhiều khe.
6. Chạy MemTest86 (boot từ USB) để kiểm tra lỗi RAM chuyên sâu (chạy ít nhất 2 vòng).
7. Nếu MemTest86 báo lỗi → thanh RAM hoặc khe cắm bị hỏng, cần thay RAM.',
2, 1
FROM MA_LOI WHERE TenLoi = N'RAM lỏng slot hoặc bị lỗi chip nhớ';
-- Giải pháp cho: HDD bad sector (MaLoi = 13)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. NGAY LẬP TỨC sao lưu toàn bộ dữ liệu quan trọng ra ổ khác hoặc cloud.
2. Chạy CrystalDiskInfo để kiểm tra SMART status của ổ cứng. 
   - Chú ý: Reallocated Sectors Count, Pending Sectors, Uncorrectable Sectors.
3. Chạy chkdsk /f /r /b C: (từ Command Prompt quyền Admin, restart để scan).
4. Nếu số bad sector nhiều và tăng liên tục → ổ cứng sắp chết, phải thay ngay.
5. Thay ổ cứng HDD bằng SSD để có hiệu năng tốt hơn và bền hơn.
6. Clone dữ liệu từ ổ cũ sang ổ mới bằng Macrium Reflect Free hoặc Clonezilla.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Ổ cứng HDD bị bad sector (vật lý)';
-- Giải pháp cho: SSD lỗi firmware/hết tuổi (MaLoi = 14)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tải phần mềm quản lý SSD từ nhà sản xuất:
   - Samsung: Samsung Magician
   - Western Digital: WD Dashboard
   - Kingston: Kingston SSD Manager
2. Kiểm tra SMART health, TBW (Terabytes Written) còn lại.
3. Cập nhật firmware SSD nếu có bản mới.
4. Nếu SSD trong trạng thái Read-Only (ổ đọc-only để bảo vệ dữ liệu trước khi chết):
   a. Sao lưu dữ liệu ngay.
   b. Secure Erase SSD để reset về trạng thái mới (mất toàn bộ dữ liệu).
5. Nếu SSD không nhận dạng: thử trên máy khác, kiểm tra khe M.2/SATA.
6. Cân nhắc thay SSD mới nếu TBW đã dùng > 80%.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Ổ cứng SSD bị lỗi firmware hoặc hết tuổi thọ';
-- Giải pháp cho: File hệ thống corrupt (MaLoi = 15)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Mở Command Prompt quyền Administrator.
2. Chạy: sfc /scannow → chờ quá trình kiểm tra và sửa file hệ thống Windows.
3. Nếu SFC báo lỗi không sửa được, chạy tiếp:
   DISM /Online /Cleanup-Image /RestoreHealth
4. Restart máy và chạy lại sfc /scannow lần nữa.
5. Nếu vẫn còn lỗi, boot từ USB Windows và vào Recovery Environment:
   - Chọn Startup Repair
   - Hoặc mở CMD và chạy: bootrec /fixmbr, bootrec /fixboot, bootrec /rebuildbcd
6. Trường hợp xấu nhất: reinstall Windows (nên chọn "Keep files" để bảo toàn dữ liệu).',
2, 1
FROM MA_LOI WHERE TenLoi = N'File hệ thống bị corrupt (chkdsk lỗi)';
-- Giải pháp cho: MBR/GPT hỏng (MaLoi = 16)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Boot từ USB cài Windows (tải Media Creation Tool từ microsoft.com).
2. Chọn "Repair your computer" → Troubleshoot → Advanced Options → Command Prompt.
3. Gõ các lệnh sau:
   diskpart
   list disk
   select disk 0 (chọn ổ cứng chứa Windows)
   list vol
   exit
4. Với MBR:
   bootrec /fixmbr
   bootrec /fixboot
   bootrec /scanos
   bootrec /rebuildbcd
5. Với GPT (UEFI): dùng bcdboot C:\Windows /s G: /f UEFI (G: là phân vùng EFI).
6. Restart và thử boot bình thường.',
3, 1
FROM MA_LOI WHERE TenLoi = N'MBR/GPT bị ghi đè hoặc hỏng';
-- Giải pháp cho: BOOTMGR mất (MaLoi = 17)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Boot từ USB/DVD cài Windows.
2. Chọn "Repair your computer" → Command Prompt.
3. Gõ: bootrec /fixmbr → Enter.
4. Gõ: bootrec /fixboot → Enter (có thể báo "Access is denied" với UEFI, bỏ qua).
5. Gõ: bootrec /rebuildbcd → Enter.
6. Nếu vẫn lỗi, thử: bcdboot C:\Windows /s C: /f ALL.
7. Restart không có USB và thử boot.
8. Trường hợp thất bại: cài lại Windows hoặc clone lại bản backup.',
2, 1
FROM MA_LOI WHERE TenLoi = N'BOOTMGR bị hỏng hoặc mất';
-- Giải pháp cho: Windows Registry lỗi (MaLoi = 18)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Boot vào Safe Mode (F8 hoặc Shift+Restart → Troubleshoot → Advanced → Startup Settings → F4).
2. Chạy sfc /scannow trong Safe Mode.
3. Dùng công cụ: CCleaner (Registry Cleaner) hoặc Wise Registry Cleaner để dọn lỗi registry.
4. Khôi phục Registry từ backup:
   - Mở regedit → File → Import → chọn file .reg backup.
   - Hoặc từ Recovery: copy registry từ thư mục C:\Windows\System32\config\RegBack.
5. Dùng System Restore để quay về điểm phục hồi trước khi xảy ra lỗi.
6. Nếu không được: Refresh Windows (giữ lại ứng dụng và file) hoặc cài lại Windows.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Windows Registry bị lỗi sau cập nhật hoặc virus';
-- Giải pháp cho: Xung đột phần mềm/driver (MaLoi = 19)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Vào Windows Update → Xem lịch sử cập nhật, ghi nhớ bản update gần nhất.
2. Thử uninstall bản update đó: Settings → Windows Update → Update History → Uninstall updates.
3. Nếu máy không vào được Windows:
   a. Boot vào Safe Mode.
   b. Gỡ driver hoặc phần mềm vừa cài bằng Device Manager hoặc Programs & Features.
4. Dùng System Restore về thời điểm trước khi cài update.
5. Dùng công cụ Windows Update Troubleshooter từ Microsoft.
6. Tạm thời tắt Windows Update để tránh cài lại bản lỗi (Settings → Pause Updates).',
2, 1
FROM MA_LOI WHERE TenLoi = N'Xung đột phần mềm hoặc driver sau Windows Update';
-- Giải pháp cho: Ransomware (MaLoi = 20)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'⚠️ QUAN TRỌNG: KHÔNG trả tiền chuộc, không đảm bảo lấy lại dữ liệu.
1. Ngắt kết nối mạng NGAY LẬP TỨC (rút cáp LAN, tắt WiFi) để ngăn lan rộng.
2. Không tắt máy đột ngột (một số ransomware mã hóa trong RAM, tắt máy có thể mất cơ hội giải mã).
3. Xác định tên ransomware: truy cập https://id-ransomware.malwarehunterteam.com/ và upload file mã hóa.
4. Tìm công cụ giải mã tại: https://www.nomoreransom.org/
5. Nếu không có công cụ giải mã: Xóa máy sạch và cài lại Windows.
6. Khôi phục dữ liệu từ bản backup (nếu có).
7. Sau xử lý: cài đặt antivirus tốt, bật Windows Defender, bật tường lửa.',
4, 1
FROM MA_LOI WHERE TenLoi = N'Nhiễm Ransomware (mã hóa file đòi tiền chuộc)';
-- Giải pháp cho: Adware/Spyware (MaLoi = 21)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tải và chạy Malwarebytes Free (malwarebytes.com) để quét toàn diện.
2. Tải và chạy AdwCleaner (malwarebytes.com/adwcleaner) để loại bỏ adware.
3. Vào Browser: Settings → Extensions → gỡ tất cả extension lạ không rõ nguồn gốc.
4. Reset browser về mặc định (Chrome: Settings → Reset settings).
5. Kiểm tra Startup programs (Task Manager → Startup) gỡ chương trình lạ.
6. Kiểm tra Scheduled Tasks (Task Scheduler) xóa task độc hại.
7. Kiểm tra lại hosts file: C:\Windows\System32\drivers\etc\hosts.
8. Cài Windows Defender và bật Real-time Protection.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Nhiễm Adware/Spyware (hiện quảng cáo, thu thập dữ liệu)';
-- Giải pháp cho: Thiếu Visual C++ / .NET (MaLoi = 22)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Đọc thông báo lỗi để xác định phiên bản cần thiết (VD: "MSVCP140.dll not found" → cần VC++ 2015-2022).
2. Tải Visual C++ Redistributable Runtimes All-in-One từ GitHub (abbodi1406/vcredist).
3. Cài đặt tất cả phiên bản (2005 đến 2022, cả x86 lẫn x64).
4. Với .NET Framework:
   - Tải từ: https://dotnet.microsoft.com/download/dotnet-framework
   - Hoặc bật qua Windows Features: Turn Windows features on or off → .NET Framework 3.5.
5. Khởi động lại máy và thử chạy lại phần mềm.',
1, 1
FROM MA_LOI WHERE TenLoi = N'Thiếu Visual C++ Redistributable hoặc .NET Framework';
-- Giải pháp cho: BSOD 0x0000007E (MaLoi = 23)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Ghi lại tên file gây lỗi hiện trên màn hình xanh (thường có đuôi .sys).
2. Vào Device Manager, tìm driver liên quan đến file .sys đó.
3. Cập nhật hoặc gỡ driver đó.
4. Chạy sfc /scannow và DISM /Online /Cleanup-Image /RestoreHealth.
5. Phân tích file dump: C:\Windows\Minidump\ bằng công cụ WinDbg (Microsoft Store).
6. Kiểm tra RAM bằng MemTest86.
7. Thử gỡ phần mềm bảo mật (antivirus) và test thử.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Lỗi BSOD 0x0000007E: SYSTEM_THREAD_EXCEPTION_NOT_HANDLED';
-- Giải pháp cho: BSOD 0x000000D1 (MaLoi = 24)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Lỗi này thường do driver không tương thích. Khởi động vào Safe Mode.
2. Mở Event Viewer (eventvwr.msc) → Windows Logs → System → tìm lỗi màu đỏ gần thời điểm crash.
3. Cập nhật toàn bộ driver bằng Windows Update hoặc từ trang nhà sản xuất.
4. Đặc biệt chú ý: driver mạng (Ethernet/WiFi), driver GPU, driver âm thanh.
5. Gỡ bỏ phần mềm antivirus bên thứ ba và dùng Windows Defender thử.
6. Kiểm tra Windows Update và cài đủ các bản vá.
7. Phân tích minidump bằng WinDbg để xác định driver gây ra.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Lỗi BSOD 0x000000D1: DRIVER_IRQL_NOT_LESS_OR_EQUAL';
-- Giải pháp cho: BSOD 0xC000021A (MaLoi = 25)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Boot từ USB cài Windows → Repair your computer → Startup Repair (thử tự động sửa).
2. Nếu không được, vào Command Prompt từ Recovery:
   sfc /scannow /offbootdir=C:\ /offwindir=C:\Windows
   DISM /Image:C:\ /Cleanup-Image /RestoreHealth /Source:esd:D:\sources\install.esd
3. Thử System Restore về điểm trước khi lỗi.
4. Kiểm tra file winlogon.exe và csrss.exe còn nguyên vẹn không (có thể bị virus thay thế).
5. Nếu thất bại: cài lại Windows và giữ lại file cá nhân.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Lỗi BSOD 0xC000021A: STATUS_SYSTEM_PROCESS_TERMINATED';
-- Giải pháp cho: BSOD Memory Management (MaLoi = 26)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Chạy Windows Memory Diagnostic: nhấn Win+R, gõ mdsched.exe, chọn Restart and check.
2. Chạy MemTest86 từ USB (để qua đêm cho chắc chắn).
3. Nếu RAM lỗi: thay RAM mới.
4. Kiểm tra ổ cứng: chkdsk /f /r C: (lập lịch kiểm tra khi restart).
5. Cập nhật driver đồ họa.
6. Kiểm tra nhiệt độ CPU và GPU (quá nhiệt cũng gây lỗi này).
7. Nếu ổn định trong Safe Mode nhưng lỗi ở mode thường → lỗi driver/phần mềm.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Lỗi BSOD 0x0000001A: MEMORY_MANAGEMENT';
-- Giải pháp cho: BSOD PAGE_FAULT (MaLoi = 27)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Khởi động vào Safe Mode.
2. Chạy sfc /scannow.
3. Cập nhật hoặc gỡ driver mới nhất cài gần đây (đặc biệt GPU driver).
4. Kiểm tra RAM: rút từng thanh, test riêng lẻ.
5. Vô hiệu hóa tính năng overclocking nếu có (XMP/DOCP trong BIOS).
6. Đặt pagefile về automatic: System Properties → Advanced → Performance Settings → Virtual Memory.
7. Kiểm tra ổ cứng bằng chkdsk.
8. Phân tích file dump bằng WinDbg.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Lỗi BSOD 0x00000050: PAGE_FAULT_IN_NONPAGED_AREA';
-- Giải pháp cho: Driver WiFi lỗi (MaLoi = 28)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Device Manager → Network Adapters → chuột phải WiFi adapter → Update Driver.
2. Gỡ driver (Uninstall Device → tích "Delete driver software") → restart để Windows tự cài lại.
3. Tải driver WiFi từ trang web nhà sản xuất laptop (theo đúng model máy và hệ điều hành).
4. Kiểm tra xem WiFi có bị tắt bởi phím Fn không (thường là Fn+F2, F5, F12 tùy hãng).
5. Vào Services.msc, tìm "WLAN AutoConfig" đảm bảo đang Running và Automatic.
6. Chạy: netsh winsock reset catalog → netsh int ip reset → ipconfig /flushdns → restart.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Driver WiFi card bị lỗi hoặc không tương thích';
-- Giải pháp cho: WiFi card hỏng anten (MaLoi = 29)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tắt máy, tháo nắp lưng laptop.
2. Xác định WiFi card (thường nằm gần góc trên, có 2 dây anten màu trắng và đen/xám).
3. Kiểm tra 2 đầu dây anten đã cắm chắc vào card chưa (nhấn nhẹ cho đến khi nghe tiếng click).
4. Kiểm tra dây anten không bị đứt, cắt, hoặc kẹp bởi bản lề màn hình.
5. Nếu dây đứt: thay dây anten WiFi mới (đặt trên Shopee/Lazada theo model máy).
6. Nếu WiFi card hỏng: thay card WiFi mới (phổ biến: Intel AX210, AX200, MediaTek MT7921).',
3, 1
FROM MA_LOI WHERE TenLoi = N'WiFi card bị lỏng hoặc hỏng anten';
-- Giải pháp cho: Lỗi DNS (MaLoi = 30)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Mở Command Prompt quyền Admin, chạy: ipconfig /flushdns.
2. Đổi DNS thủ công:
   - Network Settings → Change adapter options → Chuột phải kết nối → Properties → IPv4.
   - Đổi sang DNS của Google: 8.8.8.8 và 8.8.4.4.
   - Hoặc Cloudflare: 1.1.1.1 và 1.0.0.1.
3. Chạy: netsh int ip reset → netsh winsock reset → restart.
4. Kiểm tra file hosts: mở C:\Windows\System32\drivers\etc\hosts bằng Notepad (Admin), xóa dòng lạ.
5. Tắt thử proxy nếu đang dùng (Settings → Proxy → tắt Use a proxy server).',
1, 1
FROM MA_LOI WHERE TenLoi = N'Lỗi cài đặt DNS sai hoặc DNS bị cache';
-- Giải pháp cho: Winsock corrupt (MaLoi = 31)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Mở Command Prompt quyền Administrator.
2. Chạy lần lượt:
   netsh winsock reset
   netsh int ip reset
   ipconfig /release
   ipconfig /flushdns
   ipconfig /renew
3. Restart máy.
4. Nếu vẫn lỗi, chạy: netsh int ipv4 reset reset.log
5. Kiểm tra xem phần mềm antivirus hoặc VPN có can thiệp vào kết nối mạng không.
6. Thử tắt Windows Firewall tạm thời để test.',
2, 1
FROM MA_LOI WHERE TenLoi = N'TCP/IP stack bị hỏng (Winsock corrupt)';
-- Giải pháp cho: Driver Ethernet lỗi (MaLoi = 32)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Device Manager → Network Adapters → chuột phải Ethernet adapter → Update Driver.
2. Kiểm tra cáp LAN: thử cáp khác, thử cổng khác trên switch/router.
3. Kiểm tra đèn LED ở cổng LAN: đèn xanh = có kết nối vật lý.
4. Gỡ và cài lại driver Ethernet từ trang nhà sản xuất.
5. Kiểm tra BIOS: một số máy có thể tắt LAN trong BIOS.
6. Nếu chip LAN trên mainboard hỏng → dùng USB LAN adapter (Ethernet Adapter USB 3.0) làm giải pháp tạm.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Driver Ethernet card bị lỗi hoặc hỏng card LAN';
-- Giải pháp cho: Bàn phím tràn nước (MaLoi = 33)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'⚠️ KHẨN CẤP - Tràn nước vào máy tính:
1. TẮT NGUỒN NGAY LẬP TỨC (giữ nút nguồn 5 giây), KHÔNG shutdown bình thường.
2. Rút dây sạc ra ngay.
3. Lật ngược máy, dốc nước ra.
4. Tháo pin nếu có thể.
5. Dùng khăn khô lau xung quanh, KHÔNG dùng máy sấy tóc (nhiệt độ cao phá bo mạch).
6. Để máy dốc ngược 24-48 tiếng trong môi trường khô ráo.
7. Nếu có hạt hút ẩm silica gel → bỏ máy vào túi kín cùng hạt hút ẩm 48 giờ.
8. SAU KHI KHÔ HOÀN TOÀN mới bật máy lên test.
9. Nếu máy vẫn lỗi sau khi khô: mang ra trung tâm sửa chữa để vệ sinh bo mạch bằng siêu âm.',
3, 1
FROM MA_LOI WHERE TenLoi = N'Bàn phím bị tràn nước hoặc hỏng mạch';
-- Giải pháp cho: Driver touchpad lỗi (MaLoi = 34)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Kiểm tra phím tắt touchpad: thường là Fn+F6, Fn+F7, hoặc Fn+F9 tùy hãng máy.
2. Settings → Bluetooth & devices → Touchpad → bật "Touchpad".
3. Device Manager → Mice and other pointing devices → cập nhật driver touchpad.
4. Gỡ driver (Uninstall Device) và restart để cài lại tự động.
5. Tải driver touchpad chính hãng từ trang web nhà sản xuất laptop.
6. Nếu dùng Synaptics/Elan touchpad: tải driver từ trang synaptics.com hoặc elan.com.
7. Vào BIOS xem có tùy chọn Enable/Disable touchpad không.
8. Nếu touchpad vẫn không phản hồi → có thể cáp kết nối touchpad bị lỏng hoặc touchpad hỏng vật lý.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Driver touchpad bị lỗi hoặc touchpad bị khóa';
-- Giải pháp cho: Driver âm thanh lỗi (MaLoi = 35)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Chuột phải vào biểu tượng loa góc dưới phải → Troubleshoot sound problems.
2. Kiểm tra âm lượng: Sound Mixer (chuột phải biểu tượng loa → Open volume mixer).
3. Device Manager → Sound, video and game controllers → cập nhật/cài lại driver âm thanh.
4. Tải driver âm thanh từ trang nhà sản xuất (Realtek thường được dùng nhất).
5. Vào Sound settings → kiểm tra Output device đúng chưa (không bị chuyển sang HDMI output).
6. Chạy: msconfig → Services → bật Windows Audio và Windows Audio Endpoint Builder.
7. Nếu dùng loa ngoài: kiểm tra cáp, thử cổng jack khác, test loa với thiết bị khác.',
1, 1
FROM MA_LOI WHERE TenLoi = N'Driver âm thanh (audio driver) bị lỗi';
-- Giải pháp cho: Lỗi driver USB (MaLoi = 36)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Thử thiết bị USB ở cổng USB khác trên máy.
2. Thử thiết bị USB trên máy tính khác để xác nhận thiết bị còn hoạt động.
3. Device Manager → Universal Serial Bus controllers → chuột phải từng USB Root Hub → Enable.
4. Gỡ tất cả USB Root Hub (Uninstall), restart để Windows tự cài lại.
5. Vào BIOS: tắt legacy USB support rồi bật lại.
6. Chạy: sfc /scannow để kiểm tra file hệ thống liên quan USB.
7. Tắt tính năng Power Management của USB: Device Manager → USB Root Hub → Properties → Power Management → bỏ tích "Allow the computer to turn off this device to save power".
8. Nếu cổng USB bị hỏng vật lý: hàn lại hoặc thay cổng mới.',
2, 1
FROM MA_LOI WHERE TenLoi = N'Lỗi driver USB hoặc USB hub trên mainboard';
-- Giải pháp cho: Driver máy in lỗi (MaLoi = 37)
INSERT INTO GIAI_PHAP (MaLoi, NoiDungStepByStep, MucDoKho, Status)
SELECT MaLoiID,
N'1. Tắt máy in, rút dây nguồn 30 giây, bật lại.
2. Xóa hàng đợi in: Services.msc → Print Spooler → Stop → xóa nội dung C:\Windows\System32\spool\PRINTERS → Start lại Print Spooler.
3. Gỡ driver máy in: Settings → Printers & scanners → chọn máy in → Remove device.
4. Tải và cài driver máy in mới từ trang chủ nhà sản xuất (HP, Canon, Epson, Brother...).
5. Thử kết nối USB trực tiếp thay vì qua WiFi (hoặc ngược lại).
6. Kiểm tra mực in, giấy, tình trạng kẹt giấy.
7. In test page: Printers & scanners → chọn máy in → Manage → Print a test page.',
1, 1
FROM MA_LOI WHERE TenLoi = N'Driver máy in bị lỗi hoặc cổng in bị tắc';
-- ============================================================
-- LIÊN KẾT TRIỆU CHỨNG - MÃ LỖI (TRIEU_CHUNG_LOI)
-- ============================================================
-- Màn hình không lên ↔ GPU lỗi
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình không lên sau khi bật máy' AND ML.TenLoi = N'GPU/Card đồ họa bị lỗi hoặc slot PCIe hỏng';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình không lên sau khi bật máy' AND ML.TenLoi = N'Cáp tín hiệu màn hình (HDMI/DP/VGA) lỏng hoặc hỏng';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình không lên sau khi bật máy' AND ML.TenLoi = N'Kết nối nội bộ màn hình laptop bị lỏng';
-- Màn hình nhấp nháy ↔ GPU / Driver
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình nhấp nháy / bị giật' AND ML.TenLoi = N'Driver đồ họa bị lỗi hoặc không tương thích';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình nhấp nháy / bị giật' AND ML.TenLoi = N'Cáp tín hiệu màn hình (HDMI/DP/VGA) lỏng hoặc hỏng';
-- Màn hình có sọc ↔ GPU / LCD hỏng
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình có sọc ngang hoặc dọc' AND ML.TenLoi = N'GPU/Card đồ họa bị lỗi hoặc slot PCIe hỏng';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình có sọc ngang hoặc dọc' AND ML.TenLoi = N'Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền';
-- Màn hình mờ tối ↔ đèn nền / cáp nội bộ
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình bị mờ, tối hơn bình thường' AND ML.TenLoi = N'Màn hình LCD bị hỏng bảng mạch hoặc đứt đèn nền';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình bị mờ, tối hơn bình thường' AND ML.TenLoi = N'Driver đồ họa bị lỗi hoặc không tương thích';
-- Máy không bật ↔ PSU
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy không bật được' AND ML.TenLoi = N'Bộ nguồn PSU bị hỏng hoặc không đủ công suất';
-- Máy tự tắt ↔ quá nhiệt / PSU
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy tự tắt đột ngột' AND ML.TenLoi = N'Hệ thống tản nhiệt bị nghẹt bụi, quá nhiệt CPU/GPU';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy tự tắt đột ngột' AND ML.TenLoi = N'Bộ nguồn PSU bị hỏng hoặc không đủ công suất';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy tự tắt đột ngột' AND ML.TenLoi = N'RAM lỏng slot hoặc bị lỗi chip nhớ';
-- Boot loop ↔ MBR / file hệ thống
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy bật lên lại tắt ngay (boot loop)' AND ML.TenLoi = N'MBR/GPT bị ghi đè hoặc hỏng';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy bật lên lại tắt ngay (boot loop)' AND ML.TenLoi = N'File hệ thống bị corrupt (chkdsk lỗi)';
-- Pin không sạc ↔ IC sạc / cổng sạc / pin hỏng
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Pin không sạc được' AND ML.TenLoi = N'IC sạc (Charging IC) trên mainboard bị lỗi';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Pin không sạc được' AND ML.TenLoi = N'Cổng sạc DC-in bị lỏng hoặc hỏng';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Pin không sạc được' AND ML.TenLoi = N'Pin Lithium bị phồng, chai hoặc hỏng tế bào';
-- Pin hao nhanh ↔ pin chai
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Pin hao nhanh bất thường' AND ML.TenLoi = N'Pin Lithium bị phồng, chai hoặc hỏng tế bào';
-- Máy nóng ↔ nhiệt / thermal paste
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy bị nóng bất thường' AND ML.TenLoi = N'Hệ thống tản nhiệt bị nghẹt bụi, quá nhiệt CPU/GPU';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy bị nóng bất thường' AND ML.TenLoi = N'Thermal paste (keo tản nhiệt) bị khô, cần thay mới';
-- Máy chạy chậm ↔ RAM / HDD bad sector / malware
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy chạy chậm bất thường' AND ML.TenLoi = N'RAM lỏng slot hoặc bị lỗi chip nhớ';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy chạy chậm bất thường' AND ML.TenLoi = N'Ổ cứng HDD bị bad sector (vật lý)';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy chạy chậm bất thường' AND ML.TenLoi = N'Nhiễm Adware/Spyware (hiện quảng cáo, thu thập dữ liệu)';
-- BSOD ↔ RAM / driver
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình xanh chết chóc (BSOD)' AND ML.TenLoi = N'RAM lỏng slot hoặc bị lỗi chip nhớ';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình xanh chết chóc (BSOD)' AND ML.TenLoi = N'Lỗi BSOD 0x0000007E: SYSTEM_THREAD_EXCEPTION_NOT_HANDLED';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình xanh chết chóc (BSOD)' AND ML.TenLoi = N'Lỗi BSOD 0x000000D1: DRIVER_IRQL_NOT_LESS_OR_EQUAL';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình xanh chết chóc (BSOD)' AND ML.TenLoi = N'Lỗi BSOD 0x0000001A: MEMORY_MANAGEMENT';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Màn hình xanh chết chóc (BSOD)' AND ML.TenLoi = N'Lỗi BSOD 0x00000050: PAGE_FAULT_IN_NONPAGED_AREA';
-- Không khởi động Windows ↔ BOOTMGR / MBR
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Windows không khởi động được' AND ML.TenLoi = N'BOOTMGR bị hỏng hoặc mất';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Windows không khởi động được' AND ML.TenLoi = N'File hệ thống bị corrupt (chkdsk lỗi)';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Bị lỗi "Missing operating system"' AND ML.TenLoi = N'BOOTMGR bị hỏng hoặc mất';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Bị lỗi "Missing operating system"' AND ML.TenLoi = N'MBR/GPT bị ghi đè hoặc hỏng';
-- Virus / Malware
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy bị nhiễm virus/malware' AND ML.TenLoi = N'Nhiễm Ransomware (mã hóa file đòi tiền chuộc)';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy bị nhiễm virus/malware' AND ML.TenLoi = N'Nhiễm Adware/Spyware (hiện quảng cáo, thu thập dữ liệu)';
-- Không kết nối WiFi ↔ driver / card hỏng / DNS
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Không kết nối được WiFi' AND ML.TenLoi = N'Driver WiFi card bị lỗi hoặc không tương thích';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Không kết nối được WiFi' AND ML.TenLoi = N'WiFi card bị lỏng hoặc hỏng anten';
-- Không vào web ↔ DNS / Winsock
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Không vào được một số trang web' AND ML.TenLoi = N'Lỗi cài đặt DNS sai hoặc DNS bị cache';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Không vào được một số trang web' AND ML.TenLoi = N'TCP/IP stack bị hỏng (Winsock corrupt)';
-- Không kết nối LAN ↔ driver ethernet / Winsock
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Không kết nối được mạng LAN (dây)' AND ML.TenLoi = N'Driver Ethernet card bị lỗi hoặc hỏng card LAN';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Không kết nối được mạng LAN (dây)' AND ML.TenLoi = N'TCP/IP stack bị hỏng (Winsock corrupt)';
-- Bàn phím liệt ↔ tràn nước
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Bàn phím một số phím bị liệt' AND ML.TenLoi = N'Bàn phím bị tràn nước hoặc hỏng mạch';
-- Touchpad không hoạt động ↔ driver touchpad
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Touchpad không hoạt động' AND ML.TenLoi = N'Driver touchpad bị lỗi hoặc touchpad bị khóa';
-- Loa không có âm thanh ↔ driver âm thanh
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Loa không có âm thanh' AND ML.TenLoi = N'Driver âm thanh (audio driver) bị lỗi';
-- Cổng USB không nhận ↔ driver USB
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Cổng USB không nhận thiết bị' AND ML.TenLoi = N'Lỗi driver USB hoặc USB hub trên mainboard';
-- Máy in không phản hồi ↔ driver máy in
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Máy in không phản hồi' AND ML.TenLoi = N'Driver máy in bị lỗi hoặc cổng in bị tắc';
-- Ứng dụng crash ↔ thiếu runtime / registry lỗi
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Ứng dụng bị crash liên tục' AND ML.TenLoi = N'Thiếu Visual C++ Redistributable hoặc .NET Framework';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Ứng dụng bị crash liên tục' AND ML.TenLoi = N'Windows Registry bị lỗi sau cập nhật hoặc virus';
-- RAM lỗi ↔ BSOD memory
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'RAM bị lỗi / không ổn định' AND ML.TenLoi = N'RAM lỏng slot hoặc bị lỗi chip nhớ';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'RAM bị lỗi / không ổn định' AND ML.TenLoi = N'Lỗi BSOD 0x0000001A: MEMORY_MANAGEMENT';
-- Quạt kêu to ↔ quá nhiệt
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Quạt tản nhiệt kêu to liên tục' AND ML.TenLoi = N'Hệ thống tản nhiệt bị nghẹt bụi, quá nhiệt CPU/GPU';
INSERT INTO TRIEU_CHUNG_LOI (MaTC, MaLoi)
SELECT TC.MaTC, ML.MaLoiID FROM TRIEU_CHUNG TC, MA_LOI ML
WHERE TC.TenTC = N'Quạt tản nhiệt kêu to liên tục' AND ML.TenLoi = N'Thermal paste (keo tản nhiệt) bị khô, cần thay mới';
COMMIT;
PRINT N'✅ Đã seed xong dữ liệu: Triệu chứng, Mã lỗi, Giải pháp và Liên kết!';
GO