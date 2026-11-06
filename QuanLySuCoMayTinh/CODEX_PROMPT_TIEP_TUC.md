# OWNER DIRECTIVE — ĐỒ ÁN CHUYÊN NGÀNH: ĐỒNG BỘ CODE THEO ERD V2 VÀ HUẤN LUYỆN AI TRÊN DỮ LIỆU CÔNG KHAI

Dự án: Quản lý chẩn đoán và giải pháp sự cố máy tính (HUTECH). Đọc `AGENTS.md` và `CHATGPT_PROJECT_CONTEXT.md` trong thư mục mã nguồn trước khi làm.
Trả lời bằng tiếng Việt, ngắn gọn, nêu rõ việc đã làm và việc chưa kiểm chứng.

## 0. QUYẾT ĐỊNH CỦA OWNER (ghi đè mọi chỉ đạo trước)

1. **ERD chuẩn là bản 20 bảng (ERD V2)**, không phải bản 25 bảng. Năm bảng `CHI_TIET_DU_DOAN_AI`, `KET_QUA_KIEM_THU_AI`, `LAN_DU_DOAN_AI`, `NHOM_SU_CO_AI`, `SNAPSHOT_DAU_VAO_CHAN_DOAN` và bản `DACS_ERD_CHUYEN_NGANH_V2.cdm` của Codex chỉ là tài liệu tham khảo. Không đưa chúng vào code hay migration. Không sửa các file CDM/PDM của owner trong `D:\SAP\ERDDiagram\`.
2. ERD đi trước, code đi sau. Khi code và ERD khác nhau, sửa code theo ERD.
3. Quy tắc đặt tên: tên bảng mới dùng cụm danh từ. Thuộc tính không bị ràng buộc này.
4. Hướng dữ liệu: dùng **bộ dữ liệu công khai**, không tự thu thập ca thật. Owner **đã cho phép tải Zenodo "Classification of IT Support Tickets"** (kiểm tra giấy phép trước khi dùng). Các nguồn khác chỉ được kiểm tra metadata, không tải, cho tới khi owner cho phép riêng.
5. Không xây thêm quy trình chữ ký, grant, receipt, pilot intake hay kiểm soát privacy kiểu doanh nghiệp. Dữ liệu công khai đã khử thông tin cá nhân chỉ cần: ghi nguồn, giấy phép, quét nhanh PII còn sót, khử trùng, tách tập độc lập.

## 1. VIỆC 1 — ĐỐI CHIẾU ERD V2 VỚI CODE (làm trước, chưa sửa gì)

Đọc `D:\SAP\ERDDiagram\DACN_Quanlychandoansucomaytinh.cdm` và `ERDQUANLYCHUANDOANSUCOMAYTINH2.pdm`, đối chiếu với `Models/`, `Data/ApplicationDbContext.cs`, `Migrations/`. Xuất bảng khác biệt ngắn gọn (bảng, cột, kiểu, khóa, quan hệ) và báo cho owner **trước khi sửa**. Nếu CDM/PDM mới nhất không còn là 20 bảng, dừng lại và hỏi owner.

Các điểm cần kiểm tra (ERD đi trước code):
- `PHIEU_TIEP_NHAN`: thêm `MoTaSuCo`, `PhanLoaiYeuCau`, `MucDoUuTien`, `MucDoUuTienDeXuat`, `GhiChuNoiBo`.
- `LICH_SU_CHAN_DOAN`: thêm `TrangThaiXacMinh`, `NgayXacMinh`, `MaNguoiDungXacMinh`, `MaMoHinh`. Khóa ngoại theo vai trò: `MaPhieu`, `MaLoiRule`, `MaLoiAITop1/2/3`, `MaLoiThucTe`.
- `ML_MODEL_INFO.LoaiBaiToan`; `AI_TEST_CASE.MaMoHinh`; `AI_TRAINING_JOB` và `MODEL_ACTIVATION_LOG` có `MaMoHinh`, `MaNguoiDung`.
- `CHI_TIET_SUA_CHUA`: `ThamChieuBangChungKiemTraLai`, `TrangThaiKiemTraLai`, `ThoiGianKiemTraLai` (tên theo PDM, thay cho `ThoiGianKiemTra` ghi nhầm trước đó).
- `LICH_SU_TRANG_THAI`: `LoaiSuKien`, `DuLieuBoSungJSON`. `THIET_BI.NgoaiQuan`.
- Đổi tên thuộc tính bảng AI sang tiếng Việt theo ERD (ví dụ `DoTinCayTop1`, `NgayTao`, `PhienBan`, `DoChinhXacMicro`, `SoBanGhiHuanLuyen`, `TrieuChungNhap`, `LoiKyVong`).
- `DANH_GIA_DICH_VU.MaPhieu` duy nhất (quan hệ 1–1, Phiếu là bên cha). `NGUOI_DUNG.TenDangNhap` và `KHACH_HANG.SoDienThoai` UNIQUE.

## 2. VIỆC 2 — CẬP NHẬT CODE VÀ MIGRATION (sau khi owner xem bảng khác biệt)

- Sao lưu database trước. Chạy trên database kiểm thử, không đụng dữ liệu đang dùng.
- Sửa Model, `DbContext`, Controller, Service, View bị ảnh hưởng bởi việc đổi tên. Tạo **một migration riêng**, dùng rename (không drop rồi tạo lại) để giữ dữ liệu.
- Build phải 0 lỗi. Chạy lại các luồng chính: tiếp nhận → chẩn đoán → chốt mã lỗi → xuất linh kiện → hoàn thành → đánh giá công khai.
- Dịch vụ huấn luyện (`AITrainingBackgroundService`): chỉ dùng nhãn thực tế làm nhãn huấn luyện khi `TrangThaiXacMinh = Đã xác minh`. Dữ liệu tổng hợp và dữ liệu công khai phải có nhóm nguồn và trọng số riêng, không được vào tập kiểm thử chính.

## 3. VIỆC 3 — DỮ LIỆU CÔNG KHAI (Zenodo trước)

1. Tải Zenodo "Classification of IT Support Tickets" (Pereira, 2022) vào thư mục mới trong `D:\Code\Đồ án chuyên nghành\Datasets Đồ án chuyên nghành\`. Ghi URL, DOI, giấy phép, SHA-256, ngày tải.
2. Báo cáo: số phiếu, 7 nhóm nhãn và phân bố, độ dài văn bản, ngôn ngữ, trùng lặp, PII còn sót.
3. Ánh xạ nhãn về hệ thống: dò từng nhóm nhãn với `MA_LOI`/`TRIEU_CHUNG`/`PhanLoaiYeuCau` của đồ án. **Không dùng keyword để tự sinh nhãn đúng.** Nhóm không ánh xạ được thì loại và ghi lý do. Ghi rõ có bao nhiêu phiếu thuộc phạm vi PC sau khi lọc.
4. Hai bài toán tách biệt:
   - (a) Phân loại/ưu tiên từ `MoTaSuCo` (bài toán này hợp với Zenodo).
   - (b) Chẩn đoán triệu chứng → mã lỗi (chỉ dùng dữ liệu đã ánh xạ được, nếu quá ít thì nói thẳng là không đủ).
5. Các nguồn còn lại trong thư mục (`aa_dataset…`, `Enhanced_IT_Troubleshooting…`, `root_cause_analysis`, `Help Desk Tickets`) chỉ dùng phụ trợ. Dữ liệu SMART ổ cứng (`hard_drive_failure_data`, `data_Q2_2026`) tách khỏi mô hình chẩn đoán này.

## 4. VIỆC 4 — HUẤN LUYỆN VÀ SO SÁNH

- Cùng một bộ chia train/validation/test (độc lập, khử trùng trước khi chia) cho cả hai mô hình.
- Mô hình 1: DistilBERT (ưu tiên bản đã có sẵn; chỉ tải weights đa ngôn ngữ ~542 MB khi owner duyệt riêng). Mô hình 2: ML.NET `SdcaMaximumEntropy`. Thêm baseline Rule-based để so sánh.
- Chỉ số: Micro/Macro Accuracy, Macro-F1, LogLoss, Top-3/Top-5, ma trận nhầm lẫn, khoảng tin cậy. Báo cáo cả kết quả kém nếu có.
- Xuất ONNX và nạp vào ML.NET qua `PredictionEnginePool` chỉ sau khi mô hình qua ngưỡng an toàn (đủ dữ liệu, Micro ≥ 0,6, Macro ≥ 0,5, AI không kém Rule quá nhiều).
- Không tuyên bố hiệu quả khi chưa kiểm chứng. Phân biệt rõ: dữ liệu thực, công khai, tổng hợp, kết quả gợi ý và nhãn đã xác minh.

## 5. GIỚI HẠN

- Không mở tập TEST niêm phong cũ, không dùng lại receipt/CompCat head làm kết quả chính.
- Không xóa Master Corpus 60.307 và các artifact Phase 1–4; chỉ đọc khi cần.
- Không sửa ERD/SRD/CDM/PDM của owner, không đụng báo cáo Word.
- Nếu cần quyết định ngoài phạm vi trên (tải nguồn khác, tải weights, đổi cấu trúc bảng), dừng và hỏi owner thay vì tự quyết.

## 6. BÀN GIAO

Một báo cáo ngắn `BAO_CAO_TIEP_TUC.md` gồm: bảng khác biệt ERD ↔ code (Việc 1), danh sách file đã sửa và migration (Việc 2), thống kê dữ liệu và bảng ánh xạ nhãn (Việc 3), kết quả so sánh mô hình và giới hạn (Việc 4), và danh sách quyết định còn chờ owner. Mỗi mục ghi rõ **đã kiểm chứng** hay **chưa kiểm chứng**.
