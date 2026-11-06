# Kho mô hình AI của hệ thống (ngoài mã nguồn)

Trỏ website vào thư mục này bằng cấu hình `AI:ModelRoot` (ví dụ biến môi trường `AI__ModelRoot=D:\Code\Đồ án chuyên nghành\AI-Models`
hoặc khóa `"AI": { "ModelRoot": "D:\Code\Đồ án chuyên nghành\AI-Models" }` trong `appsettings.Development.json`).

- `SongNgu\` — mô hình song ngữ ONNX (song_ngu_int8.onnx, tokenizer.json, nhan.json). Tạo bằng `MoHinh-SongNgu\xuat_onnx.py`.
- `ActiveModel.zip`, `DiagnosticModel_*.zip` — mô hình ML.NET (huấn luyện nền trong website lưu vào đây khi đã đặt `AI:ModelRoot`).

Không đặt `AI:ModelRoot` thì website dùng `Data\MLModels` trong thư mục chạy như trước.
