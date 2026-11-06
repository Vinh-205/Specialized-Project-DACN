# BÀN GIAO TỪ PHIÊN CLAUDE SANG CODEX (07/11/2026)

Đọc theo thứ tự: `AGENTS.md` → file này → `CODEX_PROMPT_TIEP_TUC.md` (chỉ đạo ERD V2 và AI, vẫn là việc chính tiếp theo).
Owner đã nói: **giao diện tạm ổn, quay lại ERD**. Việc kế tiếp của Codex là Việc 1 trong `CODEX_PROMPT_TIEP_TUC.md` (đối chiếu ERD V2 với code, báo bảng khác biệt, chưa sửa gì khi chưa được xem).

## 1. Cách làm việc owner muốn
- Trả lời tiếng Việt, ngắn gọn, nêu rõ **đã kiểm chứng / chưa kiểm chứng**, cuối câu trả lời có mục "Kết luận".
- **Không hỏi commit/push.** Owner tự nói "commit" khi muốn. Không commit `appsettings.json`, `appsettings.Development.json` (đã git-ignore).
- Không xóa file ngoài phạm vi yêu cầu; trước khi xóa phải kiểm tra file còn được dùng không.
- ERD là nguồn chuẩn (20 bảng). Không thêm bảng/cột ngoài ERD. Không sửa CDM/PDM/báo cáo Word của owner.
- AI phải là học máy thật, không coi từ điển/Rule là AI. AI tự chốt mã lỗi chỉ khi độ tin cậy Cao (xem `AGENTS.md`). Không tạo gói nhờ KTV/thầy duyệt nhãn. Không thu thập ca thật.
- Khi chạy thử website: dùng `dotnet build -o <thư mục tạm>` rồi chạy từ thư mục đó; không build vào `bin` khi `dotnet watch` đang chạy; Razor hot reload hay không nhận file mới nên cần restart server rồi Ctrl+F5.

## 2. Đã làm trong phiên Claude (chỉ phần giao diện công khai, chưa commit)
Trang công khai gồm hai trang, **không lặp nội dung**:
- **`/Home/Welcome`** (`Views/Home/Welcome.cshtml`, `wwwroot/css/welcome-cartoon.css`, `wwwroot/js/welcome-cartoon.js`, ảnh ở `wwwroot/images/welcome-cartoon/`): hành trình cuộn 3 bước Tiếp nhận / Chẩn đoán / Sửa chữa với laptop hoạt hình (mở nắp khi vào, xoay đối xứng giữa các bước, đóng nắp ở cuối), linh vật CRT (`welcome-mascot.bundle.js`, giữ nguyên animation), dòng chữ "Chào mừng" chạy ngang, 2 mũi tên lên/xuống, thước chỉ gồm các chấm. Minh họa tự né header và dòng chữ chạy. Cuối trang: 3 thẻ dẫn sang Landing.
- **`/Home/Landing`** (`Views/Home/Landing.cshtml`): sổ tay hệ thống đi sâu (Vai trò, DeskAI và quyết định cuối, Kho tri thức, Linh kiện và chi phí), mổ xẻ laptop, 4 trạng thái phiếu. Đã bỏ các chương trùng với Welcome.
- **Header dùng chung**: `Views/Shared/_PublicNav.cshtml` + `wwwroot/css/public-nav.css` (kính mờ, chỉ có các liên kết ở giữa). Truyền mục qua `ViewData["PublicNavItems"]`.
- Trang đăng nhập và các trang trong hệ thống giữ nguyên.
- Đã xóa file không dùng: các CSS/JS/View thử nghiệm cũ (`welcome-meadow/cine/sylva`, `notebook3d`, `scroll-showcase`, `landing-saas`, `public-home`, `dashboard-overview`, `site.css`, `computer3d`, `landing-gsap/scroll`...), video giới thiệu và ảnh không dùng, thư mục `wwwroot/preview`, và hai action `WebGLTest`, `LandingAnatomy` trong `HomeController`. Build 0 lỗi.
- `Tools/LaptopTeardown` còn giữ (sinh `_LaptopFigure`, `_JourneyFigure`, `iso-figures.css` cho Landing).

## 3. Việc khác đang có trong cây làm việc (từ các phiên trước, chưa commit)
- `Services/ModelGate.cs`, `Services/AiModelCatalog.cs`, `Controllers/AdminController.cs` (trang `MLModels` chỉ hiện phiên bản đang dùng, nút "Hiện N phiên bản cũ"), `Controllers/TroLyChanDoanController.cs`.
- Nhiều file `Views/*` đã chỉnh giao diện (Admin, Auth, LinhKien, PhieuTiepNhan, TroLyChanDoan, `_Layout`...), `wwwroot/css/scene.css` + `scene.js` (cảnh chuyển đăng nhập/đăng xuất).
- `KiemThu-HTTP/` có script kiểm thử và dựng bản chạy thử (`ui_server.py`); log của nó nằm ngoài git và đã dọn.

## 4. Chưa kiểm chứng / còn mở
- Welcome ở bước 02, 03 và màn cuối sau các lần sửa cuối chỉ được đo bố cục bằng số (không chồng lấn ở 5 cỡ màn hình); chưa xem trực tiếp trên máy owner. Bản điện thoại chưa xem lại.
- Mốc đổi chấm của thước cuộn mới sửa theo tính toán, chưa xem lúc chạy.
- Phần AI: độ chính xác thật chưa đủ để gọi là hệ thống thật (nhãn yếu), xem `AGENTS.md` mục 8.
- Việc ERD V2 → code/migration chưa bắt đầu trong phiên này.

## 5. Việc tiếp theo cho Codex
1. Làm Việc 1 của `CODEX_PROMPT_TIEP_TUC.md`: đối chiếu `D:\SAP\ERDDiagram\DACN_Quanlychandoansucomaytinh.cdm` và `ERDQUANLYCHUANDOANSUCOMAYTINH2.pdm` với `Models/`, `Data/ApplicationDbContext.cs`, `Migrations/`; xuất bảng khác biệt và **dừng để owner xem** trước khi sửa.
2. Chỉ sửa giao diện khi owner yêu cầu lại.
