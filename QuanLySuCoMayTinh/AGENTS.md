\# Ngữ cảnh dự án "Đồ án chuyên ngành" (dán vào phần Instructions của dự án ChatGPT)



\## 1. Thông tin chung

\- Đề tài: \*\*Quản lý chẩn đoán và giải pháp sự cố máy tính\*\*. Trường ĐH Công nghệ TP.HCM (HUTECH), khoa CNTT, chuyên ngành Hệ thống thông tin ứng dụng.

\- Sinh viên: Trương Tuấn Vĩnh (2380602577, 23DTHE1) và Trần Thị Diểm Trinh (2380602337, 23DTHA2). GVHD: ThS. Võ Hoàng Khang.

\- Đồ án chuyên ngành kế thừa \*\*Đồ án cơ sở\*\* (hệ thống quản lý tiếp nhận – sửa chữa + chẩn đoán Rule-based + ML.NET cơ bản).

\- Mốc thời gian HK1 2026-2027: thực hiện 12/10 – 20/12/2026; nộp báo cáo LMS 21 – 27/12/2026; báo cáo hội đồng 28/12/2026 – 10/01/2027; GV nộp bảng điểm trước 11g00 ngày 16/01/2027.

\- Cách làm việc: \*\*thiết kế ERD (PowerDesigner) trước, rồi mới sửa code/backend cho khớp ERD\*\*. ERD là nguồn chuẩn.



\## 2. Công nghệ

ASP.NET Core MVC, C#, Entity Framework Core, SQL Server (2017 trong PowerDesigner), ML.NET. Phạm vi: không phát triển LLM/RAG, không phụ thuộc dịch vụ AI ngoài; AI gợi ý và, khi độ tin cậy Cao (≥ 0,75) cùng điều kiện an toàn, TỰ CHỐT mã lỗi (có nhật ký LoaiSuKien=AI_TU_CHOT, tắt bằng AI:TuDongChot=false, tự tạm dừng nếu ≥ 10 ca tự chốt và > 30% bị sửa lại); chủ đề tài đã chấp thuận.



\## 3. Thư mục và file

\- Mã nguồn: `D:\\Code\\Đồ án chuyên nghành\\QuanLySuCoMayTinh` (Controllers, Models, Data, Services, Views, Migrations, Data/MLModels).

\- Dữ liệu huấn luyện công khai: `D:\\Code\\Đồ án chuyên nghành\\Datasets Đồ án chuyên nghành\\`.

\- ERD/SRD: `D:\\SAP\\ERDDiagram\\` (CDM hiện dùng: `DACN\_Quanlychandoansucomaytinh.cdm`; PDM: `ERDQUANLYCHUANDOANSUCOMAYTINH2.pdm`; ảnh SRD xuất: `SRD\_xuat\\`).

\- Báo cáo: `C:\\Users\\tuanv\\OneDrive\\Máy tính\\Đồ án chuyên nghành\\` (file `...ĐACN\_TruongTuanVinh\_2380602577\_23DTHE1\_Chuong2\_v3.docx` là bản đã soạn lại Chương 1–2, có phiếu theo dõi tiến độ).

\- Cấu trúc báo cáo theo hướng dẫn khoa: Chương 1 Tổng quan; Chương 2 Cơ sở lý thuyết (2.1 thực thể, 2.2 ERD, 2.3 PowerDesigner, 2.4 mô hình quan hệ + SRD, 2.5 RBTV, 2.6 UML); Chương 3 Kết quả thực nghiệm; Chương 4 Kết luận. Mỗi chương bắt đầu trang mới.

\- Lời thầy: ERD chỉ vẽ những gì cần \*\*lưu trữ\*\*; AI thuộc \*\*xử lý\*\* nên không vẽ luồng AI vào ERD, chỉ chỉ ra AI gắn vào đâu trong ERD.



\## 4. Nghiệp vụ chính

\- Vai trò: `Admin KTV`, `KTV`. Khách hàng đánh giá qua liên kết công khai của phiếu đã hoàn thành.

\- Trạng thái phiếu: `Chờ khám` → `Đang kiểm tra` (KTV nhận ca) → `Đang sửa chữa` (chốt đơn) → `Hoàn thành` (chốt phí).

\- Chẩn đoán: KTV chọn triệu chứng → Rule-based (đếm triệu chứng khớp trong TRIEU\_CHUNG\_LOI) + ML.NET (vector multi-hot 500 chiều, `PredictionEnginePool` "DiagnosticModel", Top-3) → hợp nhất xếp hạng → KTV chốt mã lỗi (ghi LICH\_SU\_CHAN\_DOAN, LICH\_SU\_TRANG\_THAI).

\- Huấn luyện chạy nền (Channel + background service), thuật toán `SdcaMaximumEntropy`, chỉ số Micro/Macro Accuracy, LogLoss, Top-3/Top-5. Kích hoạt mô hình có ngưỡng an toàn (đủ bản ghi thực, Micro ≥ 0,6, Macro ≥ 0,5, AI không kém Rule quá nhiều), sao chép thành `ActiveModel.zip`, ghi nhật ký kích hoạt.

\- Xuất linh kiện: kiểm tra tồn kho, trừ tồn, ghi SU\_DUNG\_LINH\_KIEN với đơn giá xuất.



\## 5. ERD V2 (20 bảng, 23 quan hệ + 3 thực thể kết hợp)

Quy ước: tên thực thể/ thuộc tính tiếng Việt PascalCase không dấu theo ERD, kiểu dữ liệu SQL Server.

\- Cơ sở: NGUOI\_DUNG, TRIEU\_CHUNG, MA\_LOI, GIAI\_PHAP, LINH\_KIEN.

\- Đối tượng ngoài: KHACH\_HANG, THIET\_BI.

\- Nghiệp vụ: PHIEU\_TIEP\_NHAN, CHI\_TIET\_SUA\_CHUA, LICH\_SU\_TRANG\_THAI, HINH\_ANH\_SU\_CO, DANH\_GIA\_DICH\_VU, LICH\_SU\_CHAN\_DOAN, ML\_MODEL\_INFO, AI\_TRAINING\_JOB, MODEL\_ACTIVATION\_LOG, AI\_TEST\_CASE.

\- Kết hợp: TRIEU\_CHUNG\_LOI, SU\_DUNG\_LINH\_KIEN (SoLuong, DonGiaXuat), LICH\_SU\_CHAN\_DOAN\_TRIEU\_CHUNG (TenTrieuChungLuuTru).

\- Khóa ngoại đặt tên theo vai trò: LICH\_SU\_CHAN\_DOAN có MaPhieu, MaLoiRule, MaLoiAITop1/2/3, MaLoiThucTe, MaNguoiDungXacMinh, MaMoHinh. AI\_TRAINING\_JOB, MODEL\_ACTIVATION\_LOG, AI\_TEST\_CASE có MaMoHinh; AI\_TRAINING\_JOB và MODEL\_ACTIVATION\_LOG có MaNguoiDung. DANH\_GIA\_DICH\_VU.MaPhieu duy nhất (quan hệ 1–1, Phiếu là bên cha).

\- Thuộc tính mới (ERD đi trước code, cần thêm vào model/migration): PHIEU\_TIEP\_NHAN (MoTaSuCo, PhanLoaiYeuCau, MucDoUuTien, MucDoUuTienDeXuat); LICH\_SU\_CHAN\_DOAN (TrangThaiXacMinh, NgayXacMinh, MaNguoiDungXacMinh, MaMoHinh); ML\_MODEL\_INFO.LoaiBaiToan; AI\_TEST\_CASE.MaMoHinh; CHI\_TIET\_SUA\_CHUA (ThamChieuBangChungKiemTraLai, TrangThaiKiemTraLai, ThoiGianKiemTra); LICH\_SU\_TRANG\_THAI (LoaiSuKien, DuLieuBoSungJSON); THIET\_BI.NgoaiQuan; PHIEU\_TIEP\_NHAN.GhiChuNoiBo.

\- Đã đổi tên thuộc tính bảng AI sang tiếng Việt (ví dụ DoTinCayTop1, NgayTao, PhienBan, DoChinhXacMicro, SoBanGhiHuanLuyen, TrieuChungNhap, LoiKyVong). Code hiện dùng tên tiếng Anh cũ, cần đổi + migration.

\- Định danh phụ: NGUOI\_DUNG.TenDangNhap, KHACH\_HANG.SoDienThoai (UNIQUE).

\- Ghi chú nghiệp vụ trong mô hình: nguyên nhân thực tế chỉ làm nhãn huấn luyện khi TrangThaiXacMinh = Đã xác minh.

\- Điểm gắn của AI vào ERD: LICH\_SU\_CHAN\_DOAN là cầu nối nghiệp vụ ↔ mô hình (ML\_MODEL\_INFO).

\- Lưu ý PowerDesigner: CDM mất độ dài nvarchar khi lưu; quan hệ 1–1 cần Dominant role; Check Model báo lỗi mã Data Item trùng (NGAYTAO, THOIGIAN, TRANGTHAI) nên khi sinh PDM phải tắt "Check model" hoặc đổi mã.



\## 6. Tiến độ (cập nhật 04/11/2026)

\- Đã xong: ERD V2, PDM/SRD nghiệp vụ và AI, Chương 1–2 (định dạng lại, mục lục tự động, UML kiểu StarUML, ERD/SRD xoay 90°, phiếu theo dõi tiến độ tuần 3–12 điền sẵn).

\- Còn lại: cập nhật code/migration theo ERD; xử lý dữ liệu công khai và nhãn; huấn luyện + đánh giá mô hình chẩn đoán lỗi và mô hình phân loại/ưu tiên; ca kiểm thử AI; Chương 3, Chương 4, tài liệu tham khảo (IEEE); slide + demo.



\## 7. Sở thích làm việc

\- Trả lời bằng tiếng Việt, ngắn gọn, nêu rõ việc đã làm và việc chưa kiểm chứng.

\- Báo cáo: Times New Roman 13, canh đều, giãn dòng 1,5, tên mã dài (ví dụ LICH\_SU\_CHAN\_DOAN) canh trái để tránh giãn chữ; bảng 10–11 pt; hình có chú thích "Hình x.y".

\- Không dùng số liệu chưa kiểm chứng để khẳng định hiệu quả mô hình; phân biệt dữ liệu thực, tổng hợp, kết quả gợi ý và nhãn đã xác minh.

\## 8. Bộ mô hình AI hiện hành (cập nhật 06/11/2026)
\- Kho mô hình ngoài mã nguồn: `D:\Code\Đồ án chuyên nghành\AI-Models` (cấu hình `AI:ModelRoot`; triển khai phải sao chép thư mục này và đặt lại khóa cấu hình). Gồm SongNgu (ONNX bộ mã hóa), MaLoiTrucTiep (47 lớp, ghép theo TÊN mã lỗi), KetQuaSua (MLP), ML.NET `ActiveModel.zip` (chỉ hiển thị, học từ dữ liệu tổng hợp).
\- Nhãn mã lỗi là nhãn YẾU; độ chính xác chung Top-1 khoảng 37–62%, chỉ mức Cao được tự chốt (đo đúng 83–100%, phủ 18–30%). Không có ca thật, không có KTV/thầy duyệt nhãn.
