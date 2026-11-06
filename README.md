# Quản lý chẩn đoán và giải pháp sự cố máy tính (Đồ án chuyên ngành – HUTECH)

ASP.NET Core 8 MVC + EF Core + SQL Server, kèm bộ mô hình AI chạy ngay trong website (ONNX Runtime, ML.NET).

## Cài đặt
1. Cài Git LFS trước khi clone (`git lfs install`) vì mô hình ONNX (~113 MB) lưu bằng LFS.
2. `git clone https://github.com/Vinh-205/Specialized-Project-DACN.git`
3. Vào `QuanLySuCoMayTinh/`, sao chép `appsettings.Development.example.json` thành `appsettings.Development.json` (và `appsettings.example.json` thành `appsettings.json`), điền chuỗi kết nối SQL Server thật. Hai tệp này bị `.gitignore`, không đưa lên git.
4. `AI:ModelRoot` trong tệp mẫu đã trỏ `../AI-Models`. Khi triển khai, đặt biến môi trường `AI__ModelRoot` tới thư mục `AI-Models` tuyệt đối.
5. `dotnet ef database update` (hoặc chạy script trong `Sql_DACN/`), rồi `dotnet run`.

## Bộ mô hình AI (thư mục `AI-Models/`)
| Thư mục | Nội dung |
|---|---|
| `SongNgu` | Bộ mã hóa song ngữ ONNX: phân loại yêu cầu, mức ưu tiên, nhóm lỗi |
| `MaLoiTrucTiep` | Dự đoán trực tiếp 47 mã lỗi (ghép theo tên) và bộ ước lượng độ tin cậy |
| `KetQuaSua` | MLP dự đoán khả năng sửa |

AI tự chốt mã lỗi chỉ khi độ tin cậy Cao (≥ 0,75), có nhật ký `AI_TU_CHOT`, tắt bằng `AI:TuDongChot=false`, tự tạm dừng khi ≥ 10 ca tự chốt và > 30% bị sửa lại.
Lưu ý: nhãn mã lỗi là nhãn yếu, chưa có ca thật; độ chính xác Top-1 chung khoảng 37–62%. Xem `AI-Models/*/README.md`.
