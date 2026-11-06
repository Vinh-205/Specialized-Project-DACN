# Ngữ cảnh dự án "Đồ án chuyên ngành" (dán vào phần Instructions của dự án ChatGPT)

## 1. Thông tin chung
- Đề tài: **Quản lý chẩn đoán và giải pháp sự cố máy tính**. Trường ĐH Công nghệ TP.HCM (HUTECH), khoa CNTT, chuyên ngành Hệ thống thông tin ứng dụng.
- Sinh viên: Trương Tuấn Vĩnh (2380602577, 23DTHE1) và Trần Thị Diểm Trinh (2380602337, 23DTHA2). GVHD: ThS. Võ Hoàng Khang.
- Đồ án chuyên ngành kế thừa **Đồ án cơ sở** (hệ thống quản lý tiếp nhận – sửa chữa + chẩn đoán Rule-based + ML.NET cơ bản).
- Mốc thời gian HK1 2026-2027: thực hiện 12/10 – 20/12/2026; nộp báo cáo LMS 21 – 27/12/2026; báo cáo hội đồng 28/12/2026 – 10/01/2027; GV nộp bảng điểm trước 11g00 ngày 16/01/2027.
- Cách làm việc: **thiết kế ERD (PowerDesigner) trước, rồi mới sửa code/backend cho khớp ERD**. ERD là nguồn chuẩn.

## 2. Công nghệ
ASP.NET Core MVC, C#, Entity Framework Core, SQL Server (2017 trong PowerDesigner), ML.NET. Phạm vi: không phát triển LLM/RAG, không phụ thuộc dịch vụ AI ngoài; AI chỉ hỗ trợ quyết định của kỹ thuật viên.

## 3. Thư mục và file
- Mã nguồn: `D:\Code\Đồ án chuyên nghành\QuanLySuCoMayTinh` (Controllers, Models, Data, Services, Views, Migrations, Data/MLModels).
- Dữ liệu huấn luyện công khai: `D:\Code\Đồ án chuyên nghành\Datasets Đồ án chuyên nghành\`.
- ERD/SRD: `D:\SAP\ERDDiagram\` (CDM hiện dùng: `DACN_Quanlychandoansucomaytinh.cdm`; PDM: `ERDQUANLYCHUANDOANSUCOMAYTINH2.pdm`; ảnh SRD xuất: `SRD_xuat\`).
- Báo cáo: `C:\Users\tuanv\OneDrive\Máy tính\Đồ án chuyên nghành\` (file `...ĐACN_TruongTuanVinh_2380602577_23DTHE1_Chuong2_v3.docx` là bản đã soạn lại Chương 1–2, có phiếu theo dõi tiến độ).
- Cấu trúc báo cáo theo hướng dẫn khoa: Chương 1 Tổng quan; Chương 2 Cơ sở lý thuyết (2.1 thực thể, 2.2 ERD, 2.3 PowerDesigner, 2.4 mô hình quan hệ + SRD, 2.5 RBTV, 2.6 UML); Chương 3 Kết quả thực nghiệm; Chương 4 Kết luận. Mỗi chương bắt đầu trang mới.
- Lời thầy: ERD chỉ vẽ những gì cần **lưu trữ**; AI thuộc **xử lý** nên không vẽ luồng AI vào ERD, chỉ chỉ ra AI gắn vào đâu trong ERD.

## 4. Nghiệp vụ chính
- Vai trò: `Admin KTV`, `KTV`. Khách hàng đánh giá qua liên kết công khai của phiếu đã hoàn thành.
- Trạng thái phiếu: `Chờ khám` → `Đang kiểm tra` (KTV nhận ca) → `Đang sửa chữa` (chốt đơn) → `Hoàn thành` (chốt phí).
- Chẩn đoán: KTV chọn triệu chứng → Rule-based (đếm triệu chứng khớp trong TRIEU_CHUNG_LOI) + ML.NET (vector multi-hot 500 chiều, `PredictionEnginePool` "DiagnosticModel", Top-3) → hợp nhất xếp hạng → KTV chốt mã lỗi (ghi LICH_SU_CHAN_DOAN, LICH_SU_TRANG_THAI).
- Huấn luyện chạy nền (Channel + background service), thuật toán `SdcaMaximumEntropy`, chỉ số Micro/Macro Accuracy, LogLoss, Top-3/Top-5. Kích hoạt mô hình có ngưỡng an toàn (đủ bản ghi thực, Micro ≥ 0,6, Macro ≥ 0,5, AI không kém Rule quá nhiều), sao chép thành `ActiveModel.zip`, ghi nhật ký kích hoạt.
- Xuất linh kiện: kiểm tra tồn kho, trừ tồn, ghi SU_DUNG_LINH_KIEN với đơn giá xuất.

## 5. ERD V2 (20 bảng, 23 quan hệ + 3 thực thể kết hợp)
Quy ước: tên thực thể/ thuộc tính tiếng Việt PascalCase không dấu theo ERD, kiểu dữ liệu SQL Server.
- Cơ sở: NGUOI_DUNG, TRIEU_CHUNG, MA_LOI, GIAI_PHAP, LINH_KIEN.
- Đối tượng ngoài: KHACH_HANG, THIET_BI.
- Nghiệp vụ: PHIEU_TIEP_NHAN, CHI_TIET_SUA_CHUA, LICH_SU_TRANG_THAI, HINH_ANH_SU_CO, DANH_GIA_DICH_VU, LICH_SU_CHAN_DOAN, ML_MODEL_INFO, AI_TRAINING_JOB, MODEL_ACTIVATION_LOG, AI_TEST_CASE.
- Kết hợp: TRIEU_CHUNG_LOI, SU_DUNG_LINH_KIEN (SoLuong, DonGiaXuat), LICH_SU_CHAN_DOAN_TRIEU_CHUNG (TenTrieuChungLuuTru).
- Khóa ngoại đặt tên theo vai trò: LICH_SU_CHAN_DOAN có MaPhieu, MaLoiRule, MaLoiAITop1/2/3, MaLoiThucTe, MaNguoiDungXacMinh, MaMoHinh. AI_TRAINING_JOB, MODEL_ACTIVATION_LOG, AI_TEST_CASE có MaMoHinh; AI_TRAINING_JOB và MODEL_ACTIVATION_LOG có MaNguoiDung. DANH_GIA_DICH_VU.MaPhieu duy nhất (quan hệ 1–1, Phiếu là bên cha).
- Thuộc tính mới (ERD đi trước code, cần thêm vào model/migration): PHIEU_TIEP_NHAN (MoTaSuCo, PhanLoaiYeuCau, MucDoUuTien, MucDoUuTienDeXuat); LICH_SU_CHAN_DOAN (TrangThaiXacMinh, NgayXacMinh, MaNguoiDungXacMinh, MaMoHinh); ML_MODEL_INFO.LoaiBaiToan; AI_TEST_CASE.MaMoHinh; CHI_TIET_SUA_CHUA (ThamChieuBangChungKiemTraLai, TrangThaiKiemTraLai, ThoiGianKiemTraLai); LICH_SU_TRANG_THAI (LoaiSuKien, DuLieuBoSungJSON); THIET_BI.NgoaiQuan; PHIEU_TIEP_NHAN.GhiChuNoiBo.
- Đã đổi tên thuộc tính bảng AI sang tiếng Việt (ví dụ DoTinCayTop1, NgayTao, PhienBan, DoChinhXacMicro, SoBanGhiHuanLuyen, TrieuChungNhap, LoiKyVong). Code hiện dùng tên tiếng Anh cũ, cần đổi + migration.
- Định danh phụ: NGUOI_DUNG.TenDangNhap, KHACH_HANG.SoDienThoai (UNIQUE).
- Ghi chú nghiệp vụ trong mô hình: nguyên nhân thực tế chỉ làm nhãn huấn luyện khi TrangThaiXacMinh = Đã xác minh.
- Điểm gắn của AI vào ERD: LICH_SU_CHAN_DOAN là cầu nối nghiệp vụ ↔ mô hình (ML_MODEL_INFO).
- Lưu ý PowerDesigner: CDM mất độ dài nvarchar khi lưu; quan hệ 1–1 cần Dominant role; Check Model báo lỗi mã Data Item trùng (NGAYTAO, THOIGIAN, TRANGTHAI) nên khi sinh PDM phải tắt "Check model" hoặc đổi mã.

## 6. Tiến độ (cập nhật 04/11/2026)
- Đã xong: ERD V2, PDM/SRD nghiệp vụ và AI, Chương 1–2 (định dạng lại, mục lục tự động, UML kiểu StarUML, ERD/SRD xoay 90°, phiếu theo dõi tiến độ tuần 3–12 điền sẵn).
- Còn lại: cập nhật code/migration theo ERD; xử lý dữ liệu công khai và nhãn; huấn luyện + đánh giá mô hình chẩn đoán lỗi và mô hình phân loại/ưu tiên; ca kiểm thử AI; Chương 3, Chương 4, tài liệu tham khảo (IEEE); slide + demo.

## 7. Sở thích làm việc
- Trả lời bằng tiếng Việt, ngắn gọn, nêu rõ việc đã làm và việc chưa kiểm chứng.
- Báo cáo: Times New Roman 13, canh đều, giãn dòng 1,5, tên mã dài (ví dụ LICH_SU_CHAN_DOAN) canh trái để tránh giãn chữ; bảng 10–11 pt; hình có chú thích "Hình x.y".
- Không dùng số liệu chưa kiểm chứng để khẳng định hiệu quả mô hình; phân biệt dữ liệu thực, tổng hợp, kết quả gợi ý và nhãn đã xác minh.

## 8. Cập nhật 05/11/2026 (Claude) — trạng thái làm việc

- **Source chính đã đồng bộ** 29 file từ bản development của Codex (Model/Controller/Service/View/DbContext/migration `AlignErdV2`/snapshot); `Program.cs` và `appsettings*` giữ của owner. Bản sao lưu: `D:\Code\Đồ án chuyên nghành\Backup_truoc_dong_bo_ERDV2_20261105\`. Build 0 lỗi; smoke test trên database tạm (21 bảng, 31 FK, các trang công khai trả 200). **Chưa chạy trên database Somee/SQL Express.**
- **Migration `AlignErdV2` có chốt chặn**: chỉ chạy trên database test của Codex hoặc khi đặt biến môi trường `ALLOW_ALIGN_ERDV2=1` **sau khi sao lưu database**. `Program.cs` gọi `Database.Migrate()` lúc khởi động nên không đặt biến sẽ không khởi động được với database thật.
- Dịch vụ huấn luyện mặc định bật (`AITraining:Enabled` mặc định true); chỉ nhận nhãn `RealRepair` đã xác minh, Synthetic/Public có trọng số riêng; không dùng UserHistory.
- **Từ điển chẩn đoán**: `Database\Patch_TuDien_DoPhu_20261105.sql` (schema cũ) và `..._AlignErdV2.sql` (schema mới): +11 triệu chứng, +4 mã lỗi, +59 liên kết. Chưa áp dụng vào database nào; liên kết chưa kiểm chứng bằng ca thật.
- **Thực nghiệm AI phụ trợ** (kết quả trong `BAO_CAO_TIEP_TUC.md` mục 6–11, mã trong `Datasets Đồ án chuyên nghành\Zenodo-7648117-v2-20261104\thuc-nghiem\`, `MoHinh-UuTien\`, `MoHinh-ChanDoanLoi\`): phân loại nhóm dịch vụ Zenodo; phân loại mức ưu tiên (aa_dataset, tổng hợp); phân tích độ phủ rule. Đây là nghiên cứu thăm dò, không phải mô hình chẩn đoán lỗi PC.
- **Quy tắc bàn giao của Codex cần giữ**: không dùng synthetic/nhãn nguồn/kết quả AI làm ConfirmedFault; không huấn luyện mô hình 36 lớp chỉ từ catalog; không mở TEST đã niêm phong; không áp migration lên database nghiệp vụ khi chưa sao lưu.
- **Bộ mô hình chuẩn** (05/11/2026): `D:\Code\Đồ án chuyên nghành\MoHinh-Chuan\` — chạy `python run_suite.py configs/<ten>.json`; hợp đồng dữ liệu và cổng kích hoạt trong README. Đã kiểm tra tái lập trên Zenodo (trạng thái THAM_DO, không đủ điều kiện kích hoạt).
- **AI khởi tạo (bootstrap)**: huấn luyện từ dữ liệu tổng hợp, chỉ hiển thị (không +100, không đổi `MaLoi_CD`); mặc định bị chặn nếu kém Rule >10 điểm (`AI:BootstrapMaxRuleGap`). Kiểm thử HTTP: 38/38 mặc định, 41/41 khi `RULE_GAP=100` (`KiemThu-HTTP\`).
- **Bộ gợi ý yêu cầu tiếng Việt** (`Services/YeuCauSuggester.cs`, luật từ khóa, không phải ML): nối vào `PhieuTiepNhan/Create` (lưu `MucDoUuTienDeXuat`, điền phân loại khi trống; endpoint `GoiYYeuCau`). Tập giữ riêng tự soạn mới: phân loại 73%, ưu tiên 70%, bỏ sót ca Cao 7/11 → chỉ tham khảo. Chi tiết mục 24–26 của `BAO_CAO_TIEP_TUC.md`.
- **Giao diện**: đã chuyển màu gõ cứng sang biến theme, thiết kế lại trang tổng quan; kiểm tra tương phản WCAG ở các trang chính (mobile/tối chưa kiểm hết).
- **Thứ hạng Rule** nay xác định khi hòa điểm (tiêu chí phụ theo mã lỗi).
