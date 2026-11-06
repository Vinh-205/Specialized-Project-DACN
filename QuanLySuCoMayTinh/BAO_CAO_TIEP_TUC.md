# BÁO CÁO TIẾP TỤC — ERD V2

Cập nhật **05/11/2026**. Thực hiện theo `CODEX_PROMPT_TIEP_TUC.md` và quyết định owner chọn phương án A. Kết quả dưới đây thuộc **bản development cô lập và LocalDB test**, không phải production.

## 1. Đối chiếu ERD và code — đã kiểm chứng

CDM của owner có 17 entity + 3 association; PDM có 20 bảng, 140 cột và 29 reference. Đã trình bảng khác biệt trước khi sửa và nhận các quyết định về FK theo vai trò, `ThoiGianKiemTraLai`, bảo toàn lịch sử và giữ tên C# ở bảng cơ sở.

[Bảng đối chiếu chi tiết](D:/Code/helpdesk-data-audit/ERD-V2-CODE-ZENODO-20261104/DOI_CHIEU_ERD_CODE_CHI_TIET.md) chứa tên cột, kiểu, PK và các FK đọc trực tiếp từ XML/source. Không sửa CDM/PDM, ERD/SRD hoặc báo cáo Word. Không đưa năm bảng tham khảo của bản 25 bảng vào code.

## 2. Code, migration và SQL — hoàn thành trong phạm vi được duyệt

**Source đang phát triển:** `D:\Code\helpdesk-data-audit\ERD-V2-DEVELOPMENT-20261104\app`.

**Database được kiểm thử:** `(localdb)\MSSQLLocalDB` / `ITHelpdesk_ERDV2_Test_20261104`.

| Hạng mục | Kết quả đã kiểm chứng |
|---|---|
| EF model | 20 entity, 140 cột mapped, 29 FK; không có pending model changes |
| Database vật lý | **21 bảng nghiệp vụ, 151 cột, 31 FK**, không tính `__EFMigrationsHistory` |
| Compatibility storage | Giữ `LICH_SU_KHO` và 5 cột legacy, không map vào code |
| Kiểu và nullability | 140/140 cột mapped khớp metadata EF |
| Kiểm tra schema/backfill | 12/12 PASS; không có FK bị disabled hoặc untrusted |
| Kiểm thử HTTP nghiệp vụ | 28/28 PASS, có kiểm tra nội dung và dữ liệu SQL |
| Dữ liệu legacy mâu thuẫn | 3/3 phép thử bị chặn đúng và rollback |
| Build .NET 8 | 0 lỗi; full build còn 60 warning nullable/khác |

### Các thay đổi chính

- Bảng cơ sở giữ tên thuộc tính C#, đổi tên cột SQL qua mapping. Nhóm AI đổi cả tên C# và các tham chiếu ở Controller, Service, Razor.
- Thêm thông tin mô tả/phân loại/ưu tiên ở phiếu; trường ưu tiên AI để trống khi chưa có model phù hợp. Thêm trạng thái/ngày/người xác minh, FK model, evidence và kết quả kiểm tra lại, loại sự kiện và JSON audit theo ERD.
- FK chẩn đoán dùng đúng vai trò `MaLoiRule`, `MaLoiAITop1/2/3`, `MaLoiThucTe`, `MaNguoiDungXacMinh`. Giữ các unique username, số điện thoại và một đánh giá cho mỗi phiếu.
- Migration riêng **`20261104164631_AlignErdV2`** dùng rename tường minh. EF scaffold từng ghép sai các cột cùng kiểu; đã sửa mapping trước khi chạy bản cuối. Không drop-create các bảng nghiệp vụ đã có.
- Backfill activation sang FK model/người dùng chỉ khi ánh xạ duy nhất. Sau đó mới bỏ `NewVersion`/`ActivatedBy`. ID cũ của liên kết triệu chứng được lưu trong audit JSON trước khi đổi sang khóa ghép. Giá trị confidence và các bộ đếm lịch sử được kiểm tra không bị mất hoặc hoán đổi.
- Nhãn thực tế cũ mặc định **Chưa xác minh**. Chọn mã chẩn đoán hoặc chốt phí không tự nâng thành `RealRepair`. Xác minh là POST riêng, có role/CSRF, kiểm tra người phụ trách và bằng chứng retest. Sửa retest làm thu hồi điều kiện nhãn training, giữ giá trị trước đó trong audit.
- Bộ lọc background training chỉ nhận nhãn thực tế đã xác minh có người/ngày xác minh. Public/synthetic có nguồn và trọng số riêng, không vào test chính. Website test đang tắt training và activation; startup không tự migrate hoặc seed.

[Danh sách 30 file source thay đổi/thêm](D:/Code/helpdesk-data-audit/ERD-V2-DEVELOPMENT-20261104/evidence/changed-files.json). Có source patch và các file migration/snapshot trong ZIP; cấu hình kèm theo chỉ dùng LocalDB test.

### Schema drift và phương án A

Chuỗi migration cũ có `UnifiedServiceData`: xóa `SU_DUNG_LINH_KIEN`, thêm `LICH_SU_KHO` và 5 cột ở `CHI_TIET_SUA_CHUA` (`DonGia`, `IsDeleted`, `MaLK`, `ProcessingType`, `SoLuong`). ModelSnapshot không phản ánh đúng trạng thái đó.

Sau khi báo cáo và được owner chọn A, migration đã tạo bảng dùng linh kiện còn thiếu, backfill khi mã linh kiện/số lượng/giá/trạng thái/tổng tiền nhất quán. Kiểm thử chứng minh giữ giá xuất lịch sử 100, không lấy giá kho hiện tại 999. Bảng kho và 5 cột legacy còn nguyên.

**Code 20 entity nhưng DB tạm 21 bảng là ngoại lệ đã được owner chấp nhận.** Không báo database đã thuần 20 bảng. [Kế hoạch B để sau](D:/Code/helpdesk-data-audit/ERD-V2-DEVELOPMENT-20261104/CLEANUP_B_DEFERRED.md): đối soát dữ liệu thật, archive có count/hash/provenance, chứng minh toàn bộ lịch sử đã được chuyển/lưu và owner phê duyệt riêng trước khi bỏ compatibility storage.

### Backup, rollback và bằng chứng

Đã BACKUP WITH CHECKSUM và RESTORE VERIFYONLY trước apply. Backup mới nhất đã restore vào `ITHelpdesk_ERDV2_RollbackA_20261105`; các chuỗi activation lịch sử còn nguyên. Rollback sử dụng backup đã kiểm chứng, không thực hiện downgrade bằng cách âm thầm drop các cột mới. File backup giữ private, không nằm trong ZIP.

Các lượt lỗi ban đầu và log vẫn được giữ. Sau mỗi lỗi đã kiểm tra read-only history/schema để xác nhận transaction rollback trước khi sửa và chạy lại. Bản cuối đã apply thành công, migration history có đúng một migration ERD V2 mới.

HTTP PASS gồm: tiếp nhận và lưu Unicode; nhận ca; hiển thị gợi ý chẩn đoán; chốt mã lỗi; xuất linh kiện/lưu đơn giá; không đủ kho thì dữ liệu không đổi; chốt phí/hoàn tất không tạo nhãn; xuất phiếu; đánh giá công khai không ghi trùng; retest/xác minh/phân quyền/CSRF; các màn hình quản trị AI và Profile.

**Chưa kiểm chứng:** dữ liệu thật, production SQL, stress concurrency, toàn bộ bảo mật, pixel/UI browser và chất lượng AI. Không coi luồng Rule/gợi ý chẩn đoán PASS là bằng chứng model PC đã đạt chất lượng. Source chính có 107 file được bảo vệ đã đối chiếu hash không đổi; chỉ thêm/cập nhật báo cáo được yêu cầu. Không đồng bộ patch về source chính.

## 3. Zenodo — đã tải và kiểm tra, chưa nhập nhãn vào SQL

[Nguồn chính thức](https://zenodo.org/records/7648117), DOI `10.5281/zenodo.7648117`, version v2, CC BY 4.0 theo API. Đã tải 4 CSV và 2 file văn bản phục vụ đọc tại `D:\Code\Đồ án chuyên nghành\Datasets Đồ án chuyên nghành\Zenodo-7648117-v2-20261104`. Publisher MD5 khớp; SHA-256 và thời điểm tải nằm trong `DOWNLOAD-MANIFEST.json`. Không thực thi code, pickle hoặc model của publisher.

| Chỉ số | Kết quả |
|---|---|
| RAW | 2.229 phiếu: source TRAIN 1.572 / TEST 657 |
| Bảy nhãn | Fileservice 684; Support general 762; Software 222; EOL 103; O365 219; Active Directory 133; Computer-Services 106 |
| Độ dài text | Min 8, median 72, p95 626, max 1.856 ký tự; không text trống |
| Trùng lặp | 1.471 normalized unique; 758 dòng trùng dư; 87 nhóm trùng; 33 nhóm qua source split; 1 nhóm mâu thuẫn nhãn |
| PII quick scan | 6 record gắn flag: 5 phone/ID, 1 IPv4; không khẳng định tất cả là PII thật hoặc detector đã bắt hết |
| Ngôn ngữ | Publisher công bố EN/DE/PT/ES. Heuristic: 256 EN, 42 DE, 33 PT, 2 ES, 1.896 chưa rõ/trộn; đây không phải language ground truth |
| Phạm vi PC | 150 record có word anchor PC là **candidate**, chưa xác lập số accepted PC từ nhãn thiết bị/episode của nguồn |
| Chẩn đoán/priority | 0 crosswalk mã lỗi có căn cứ; không có priority labels |

[Bảng ánh xạ từng nhãn](D:/Code/helpdesk-data-audit/ERD-V2-CODE-ZENODO-20261104/ZENODO_LABEL_MAPPING.md). Nhãn nguồn là nhóm dịch vụ, không tự trở thành `MA_LOI`, `TRIEU_CHUNG` hoặc `ConfirmedFault`. `EOL` chưa có định nghĩa đủ rõ nên chưa đưa vào mapping nghiệp vụ. Có thể nghiên cứu phân loại yêu cầu dịch vụ từ text với scope được khai rõ; không gọi đó là chẩn đoán nguyên nhân PC.

Source TEST công khai của Zenodo được đọc trong phạm vi dataset đã giao; không mở sealed TEST của các thí nghiệm lịch sử. Chưa dùng nguồn này để lựa chọn model hoặc benchmark. Master 60.307 và các dataset/model lịch sử giữ nguyên.

## 4. So sánh mô hình — chưa thực hiện trong đợt Việc 2

Không có Fit, model mới hoặc chỉ số Accuracy/F1 mới trong đợt code/SQL này. Bài toán triệu chứng → mã lỗi chưa đủ nhãn; priority không có nhãn nguồn. Với phân loại yêu cầu, cần chốt mapping/scope rồi mask, khử trùng và tạo cùng split độc lập cho DistilBERT/SDCA/Rule. Không đưa classifier nhóm dịch vụ vào `DiagnosticModel` vector 500 hoặc dùng CompCat head làm kết quả chính. Weights multilingual chưa tải.

## 5. Bàn giao và phần còn lại

Việc 2 đạt phạm vi kiểm chứng trên bản development/SQL test. Bản gốc và production chưa được triển khai; migration có allowlist đúng database test, không phải script áp dụng production.

Giữ phương án A; chưa thực hiện B. Bước tiếp theo về dữ liệu phải phân biệt phân loại nhóm dịch vụ với chẩn đoán mã lỗi, không đổi nghĩa source labels để đủ mẫu và không bổ sung quy trình grant/receipt/pilot.

## 6. Cập nhật 05/11/2026 (Claude): đồng bộ source chính

- Đã chép 29 file Codex đã sửa (Model, Controller, Service, View, DbContext, migration `AlignErdV2`, snapshot) từ bản cô lập vào source chính. **Không chép** `Program.cs` và `appsettings*` của Codex vì đã khóa cứng LocalDB test và bỏ `Migrate()`/nạp dữ liệu mồi. `Program.cs` chính chỉ đổi tên thuộc tính AI trong khối nạp mồi (`NguonDuLieu`, `MaLichSuChanDoan`, `TenLoiThucTeLuuTru`, `TenTrieuChungLuuTru`).
- Bản sao lưu các file bị ghi đè: `D:\Code\Đồ án chuyên nghành\Backup_truoc_dong_bo_ERDV2_20261105\`.
- Dịch vụ huấn luyện và `TroLyChanDoanController` của Codex chỉ chạy khi `AITraining:Enabled=true`; đã đặt mặc định `true` để cấu hình chính không bị tắt ngầm.
- `dotnet build` source chính: 0 lỗi. **Chưa chạy website, chưa chạy migration** trên database nào. Cảnh báo: `Program.cs` chính gọi `Database.Migrate()` lúc khởi động, nên lần chạy đầu tiên với database Somee sẽ áp dụng `AlignErdV2` lên dữ liệu thật. Cần sao lưu database trước; migration mới chỉ được kiểm thử trên LocalDB test của Codex.

## 7. Cập nhật 05/11/2026 (Claude): thực nghiệm phân loại nhóm dịch vụ trên Zenodo

Phạm vi: phân loại **nhóm dịch vụ** của phiếu (không phải mã lỗi, không phải priority). Mã và kết quả: `Datasets Đồ án chuyên nghành\Zenodo-7648117-v2-20261104\thuc-nghiem\`.

Tiền xử lý: chuẩn hóa chữ thường/khoảng trắng; khử trùng trong từng tập (loại 1 nhóm trùng mâu thuẫn nhãn); loại 32 mẫu train trùng với test; bỏ nhãn EOL (không có định nghĩa; sau khử trùng 0 mẫu train, 2 mẫu test). Còn **918 train / 551 test, 6 nhóm**. Trước khử trùng là 1.572 / 657, tức khoảng 42% train là bản lặp mẫu câu. Test là tập TEST công khai của nguồn, chỉ chạm một lần sau khi chọn tham số bằng CV 5 lần trên train.

| Mô hình | Accuracy [95% CI] | Macro-F1 [95% CI] | Top-3 | LogLoss |
|---|---|---|---|---|
| Lớp đa số (Support general) | 0,350 [0,309–0,390] | 0,086 | 0,659 | (không có ý nghĩa) |
| TF-IDF từ+ký tự + hồi quy logistic (C=30) | **0,788** [0,755–0,820] | **0,738** [0,694–0,777] | **0,953** | **0,619** |
| DistilBERT-uncased EN đóng băng + hồi quy logistic (C=0,1) | 0,740 [0,704–0,777] | 0,662 [0,613–0,708] | 0,920 | 0,814 |
| mDistilBERT đa ngôn ngữ fine-tune toàn mô hình (CPU, 6 epoch, lr 4e-5) | 0,762 [0,724–0,797] | 0,695 [0,649–0,736] | 0,926 | 0,896 |
| **ML.NET 5.0.0 SdcaMaximumEntropy + FeaturizeText (l2=0)** | **0,819** [0,788–0,849] | **0,773** [0,733–0,811] | **0,964** | **0,598** |
| Rule-based từ khóa (viết theo hiểu biết lĩnh vực, không xem tập test) | 0,550 [0,508–0,590] | 0,488 [0,442–0,530] | – | – |

Ghi chú SDCA/Rule (05/11/2026): SDCA chạy thật bằng ML.NET (`thuc-nghiem\mlnet_sdca`), l2 chọn bằng CV 5 lần trên train (l2=0 tốt nhất, MacroAccuracy CV 0,685; lưu ý đây là CV theo MacroAccuracy, khác Macro-F1). LogLoss tính thủ công vì ML.NET xếp cột lớp theo thứ tự xuất hiện; tổng xác suất mỗi dòng ≈ 1. Rule-based chỉ cho nhãn cứng nên không có Top-3/LogLoss; bộ từ khóa tự viết nên **kết quả phụ thuộc cách viết luật**, chỉ là mốc tham chiếu. SDCA cao nhất ở cả Accuracy và Macro-F1, nhưng khoảng tin cậy Macro-F1 chồng với TF-IDF/hồi quy logistic (0,694–0,777). Kết luận cho mô hình nhóm dịch vụ: phương pháp tuyến tính trên TF-IDF/n-gram ≥ mDistilBERT ≥ DistilBERT-EN đóng băng > Rule > lớp đa số.

Bổ sung (mDistilBERT): weights `distilbert-base-multilingual-cased` revision `45c032ab…`, đã kiểm kích thước 541.795.680 byte và SHA-256 `fcf002be…c228`, khớp giá trị Codex ghim. Seed 42; 15% train (135 mẫu) làm validation; Macro-F1 validation tăng đến hết epoch 6 (0,628), tức có thể **chưa hội tụ**; chưa thử thêm epoch/tham số để tránh chỉnh theo test. Kết quả: mDistilBERT hơn DistilBERT-EN đóng băng nhưng **vẫn thấp hơn TF-IDF** (Macro-F1 0,695 so với 0,738), khoảng tin cậy chồng nhau. Transformers in cảnh báo "incorrect regex pattern" khi nạp tokenizer; chưa kiểm chứng ảnh hưởng (tokenizer WordPiece, có thể là cảnh báo chung).

Nhận xét trung thực:
- TF-IDF tốt hơn DistilBERT đóng băng; khoảng tin cậy Macro-F1 chồng nhau một phần nên chưa kết luận chắc, nhưng không có bằng chứng DistilBERT thắng.
- DistilBERT dùng bản **tiếng Anh uncased** có sẵn trên máy, trong khi dữ liệu trộn EN/DE/PT/ES. Đây là hạn chế. Bản đa ngôn ngữ (~542 MB) chưa được tải vì cần owner duyệt. Chưa fine-tune toàn mô hình.
- "TF-IDF + hồi quy logistic" là mô hình tương tự SDCA MaximumEntropy trong scikit-learn; **chưa chạy ML.NET SDCA thật**. Chưa có baseline Rule-based cho bài toán này.
- Nhầm lẫn lớn nhất (TF-IDF): 23/48 phiếu Active Directory và 22/89 phiếu O365 bị dự đoán thành Support general.
- Nhãn là nhóm dịch vụ; kết quả không chứng minh khả năng chẩn đoán lỗi PC.

## 8. Cập nhật 05/11/2026 (Claude): mô hình mới — phân loại mức ưu tiên (`MucDoUuTien`)

Mã và kết quả: `Datasets Đồ án chuyên nghành\MoHinh-UuTien\`.

**Dữ liệu.** `aa_dataset` (Customer Support Tickets, **tổng hợp**, EN+DE, 28.587 phiếu) lọc ba hàng đợi liên quan IT (IT Support, Technical Support, Service Outages and Maintenance): 12.943 phiếu (EN 7.343, DE 5.600); sau khử trùng 12.942. Mức ưu tiên: high 7.392 / medium 4.188 / low 1.363. Chia theo **nhóm tiêu đề** (không rò rỉ nhóm giữa các tập): fit 7.633, val 1.344, test 3.965. Đây không phải dữ liệu sửa chữa PC thật; chỉ dùng làm thực nghiệm phụ trợ.
`Enhanced_IT_Troubleshooting` **không dùng được** cho mô hình ưu tiên: cùng nội dung nhưng 1.528/1.646 nhóm có mức Severity khác nhau (nhãn gần như ngẫu nhiên).

| Mô hình | Accuracy [95% CI] | Macro-F1 [95% CI] | Kappa tuyến tính |
|---|---|---|---|
| Lớp đa số (high) | 0,584 | 0,246 | 0,000 |
| TF-IDF + hồi quy logistic (C=10, cân bằng lớp) | 0,637 [0,621–0,650] | 0,557 [0,536–0,576] | 0,357 |
| ML.NET 5.0.0 SdcaMaximumEntropy (l2=0) | 0,643 [0,628–0,658] | 0,534 [0,512–0,555] | 0,322 |
| mDistilBERT fine-tune, **mẫu con 3.000**, 3 epoch, max_length 96 | 0,503 [0,489–0,518] | 0,419 [0,404–0,436] | 0,154 |
| Rule-based từ khóa (EN+DE, không xem tập test) | 0,430 [0,417–0,446] | 0,349 [0,336–0,364] | 0,084 |

Nhận xét trung thực:
- Mức ưu tiên chỉ dự đoán được **yếu**: TF-IDF/SDCA hơn lớp đa số khoảng 5 điểm Accuracy (0,637–0,643 so với 0,584), Macro-F1 cải thiện rõ nhờ nhận ra lớp low/medium.
- mDistilBERT **kém** vì huấn luyện chưa đủ: chỉ 3.000/7.633 phiếu, 3 epoch (CPU, khoảng 7 phút/epoch), validation (600 mẫu) vẫn đang tăng ở epoch 3 (Macro-F1 0,423). Kết quả này không đủ kết luận về khả năng của kiến trúc; chưa thử toàn bộ dữ liệu hay nhiều epoch hơn.
- Rule-based tệ hơn cả lớp đa số; bộ từ khóa tự viết theo hiểu biết lĩnh vực, phụ thuộc cách viết luật.
- SDCA và TF-IDF khoảng tin cậy chồng nhau; chưa kết luận mô hình nào hơn.
- Do nhãn tổng hợp, kết quả không chứng minh khả năng đề xuất ưu tiên cho phiếu sửa chữa PC thật (`MucDoUuTienDeXuat` cần nhãn do KTV xác nhận).

## 9. Cập nhật 05/11/2026 (Claude): độ phủ bộ rule chẩn đoán (PC Diagnosis) — không huấn luyện

Phạm vi: phân tích tĩnh từ điển trong `Database\SeedData_TrieuChung_MaLoi_GiaiPhap.sql` (nguồn đúng dấu; bản seed trong LocalDB test của Codex bị hỏng ký tự tiếng Việt, hiện `?` thật trong dữ liệu). Mã và kết quả: `Datasets Đồ án chuyên nghành\MoHinh-ChanDoanLoi\phan_tich_do_phu_rule.py`, `do_phu_rule.json`. Đây **không phải** đánh giá độ chính xác chẩn đoán; chỉ đo khả năng của bộ rule (đếm triệu chứng khớp). Không sinh dữ liệu tổng hợp, không huấn luyện, theo bàn giao 05/11.

| Chỉ số | Kết quả |
|---|---|
| Triệu chứng / mã lỗi / liên kết | 44 / 37 / 52 (0 trùng, 0 sai tên) |
| Triệu chứng **không liên kết lỗi nào** | **18/44 (41%)**: chọn chúng thì rule không gợi ý gì |
| Mã lỗi **không có triệu chứng nào** | **3/37**: rule không bao giờ gợi ý được (SSD firmware/hết tuổi thọ; xung đột phần mềm/driver sau Windows Update; BSOD 0xC000021A) |
| Mã lỗi chỉ có 1 triệu chứng | 19/37 |
| Số lỗi trên mỗi triệu chứng có liên kết | tối đa 5; 14 triệu chứng trỏ tới 2 lỗi |
| Kiểm tra oracle (chọn đúng và đủ mọi triệu chứng của lỗi X) | chỉ **18/34** lỗi xếp hạng 1 duy nhất; 34/34 nằm trong Top-3; 16 lỗi hòa điểm |
| Cặp lỗi **không thể phân biệt** (cùng tập triệu chứng) | 6 cặp: 3 mã BSOD (0x7E/0xD1/0x50) lẫn nhau; IC sạc ↔ cổng sạc DC-in; Registry ↔ thiếu VC++/.NET; driver WiFi ↔ card WiFi/anten |
| Lỗi có tập triệu chứng bị lỗi khác bao trùm | 17 trường hợp |

Hệ quả cho mô hình AI:
- Với cấu trúc từ điển hiện tại, **chỉ dùng triệu chứng làm đầu vào thì mô hình học máy cũng không phân biệt được** các cặp lỗi cùng tập triệu chứng (không phải do thiếu dữ liệu, mà do đầu vào thiếu thông tin). Dữ liệu tổng hợp sinh từ bảng liên kết sẽ cho kết quả cao giả tạo, nên không thực hiện.
- Hạn chế ở lớp từ điển trước khi nói tới mô hình: cần nối 18 triệu chứng chưa liên kết, thêm triệu chứng cho 3 lỗi, và thêm thuộc tính phân biệt (ví dụ mã dừng BSOD, loại thiết bị) cho 6 cặp trên.

### Dữ liệu cần thu thập từ phiếu thật (đề xuất, chưa thực hiện)

Mỗi ca cần, theo ERD V2: tập triệu chứng KTV chọn tại thời điểm tiếp nhận (`LICH_SU_CHAN_DOAN_TRIEU_CHUNG`); gợi ý Rule/AI đã hiển thị; mã lỗi kết luận cuối (`MaLoiThucTe`) chỉ tính khi `TrangThaiXacMinh = Đã xác minh` kèm `NgayXacMinh`, `MaNguoiDungXacMinh`, bằng chứng và thời gian kiểm tra lại (`ThamChieuBangChungKiemTraLai`, `ThoiGianKiemTraLai`); thiết bị/episode độc lập (một thiết bị không được tính nhiều ca vào cùng tập kiểm thử).
Ưu tiên thu thập trước cho: (1) 6 cặp lỗi không phân biệt được, (2) 3 lỗi chưa có triệu chứng, (3) các lỗi có nhiều khả năng xảy ra nhất theo thực tế cửa hàng.
Quy mô: **chưa có căn cứ thống kê** để nêu số ca tối thiểu cho 36 lớp; quy tắc kinh nghiệm thường dùng là hàng chục ca đã xác minh mỗi lớp, nghĩa là hàng nghìn ca cho toàn bộ — chưa kiểm chứng cho đồ án này. Khả thi hơn: bắt đầu với các lỗi phổ biến nhất và chỉ báo kết quả kèm khoảng tin cậy.
Hiện tại source chính chỉ ghi nhận ca mới với trạng thái `Chưa xác minh`; cần dùng endpoint xác minh của Codex (`Verification.ErdV2.cs`) để KTV xác nhận từng ca trước khi ca đó đủ điều kiện huấn luyện.

## 10. Cập nhật 05/11/2026 (Claude): vá từ điển chẩn đoán (đề xuất, chưa áp dụng vào database nào)

File: `Database\Patch_TuDien_DoPhu_20261105.sql` (sinh bởi `MoHinh-ChanDoanLoi\tao_patch_tu_dien.py`). Chỉ **bổ sung**, không sửa/xóa dòng cũ, dùng `NOT EXISTS` nên chạy lại an toàn.

Nội dung: 11 triệu chứng mới, 4 mã lỗi mới kèm giải pháp từng bước, 59 liên kết mới.
- Nối 18 triệu chứng chưa liên kết; cấp triệu chứng cho 3 lỗi chưa có (SSD firmware/hết tuổi thọ, xung đột sau Windows Update, BSOD 0xC000021A).
- 4 mã lỗi mới cho triệu chứng chưa có lỗi tương ứng: ổ đĩa đầy dung lượng; module/driver Bluetooth; webcam hỏng hoặc bị chặn quyền; loa/jack âm thanh hỏng vật lý.
- 11 triệu chứng mới để tách 6 cặp lỗi: năm mã dừng BSOD (0x7E, 0xD1, 0x50, 0x1A, 0xC000021A); cổng sạc lỏng ↔ IC sạc; thiếu file .dll/.NET ↔ Registry; mất card WiFi sau driver ↔ WiFi yếu do anten/card.

Kiểm chứng: áp dụng seed gốc rồi patch hai lần trên **database tạm LocalDB do mình tạo** (đã xóa sau khi kiểm tra): 44/37/52/37 → 55/41/111/41 (triệu chứng/lỗi/liên kết/giải pháp), lần chạy thứ hai không đổi số dòng. Chưa chạy trên database của Codex, Somee hay SQL Express của bạn.

| Chỉ số | Trước | Sau |
|---|---|---|
| Triệu chứng chưa liên kết | 18/44 | 0/55 |
| Mã lỗi chưa có triệu chứng | 3/37 | 0/41 |
| Lỗi chỉ có 1 triệu chứng | 19 | 6 |
| Kiểm tra oracle: lỗi xếp hạng 1 duy nhất | 18/34 | 37/41 |
| Cặp lỗi cùng tập triệu chứng | 6 | 0 |
| Lỗi bị lỗi khác bao trùm | 17 | 5 |

Hạn chế quan trọng:
- Các liên kết dựa trên hiểu biết sửa chữa PC của mình, **chưa kiểm chứng bằng ca thật**; cần KTV hoặc GVHD duyệt trước khi áp dụng. Chỉ số trên chỉ đo tính nhất quán của rule, không phải độ chính xác chẩn đoán.
- Một triệu chứng chung như "Driver thiết bị bị lỗi" nay trỏ tới 8 lỗi (tối đa trước đó là 5): rule sẽ trả nhiều gợi ý hòa điểm cho triệu chứng này.
- Patch dùng tên cột **hiện tại của seed** (`TenTC`, `MaTC`, `MaLoiID`). Sau migration `AlignErdV2` các cột đổi tên (`MaTrieuChung`, `TenTrieuChung`, `MaLoi`), nên cần bản patch tương ứng nếu áp dụng sau migration. Mã triệu chứng mới nhận ID tăng dần (45–55) nên chỉ số trong vector 500 chiều của mô hình cũ không bị đổi.
- Chưa sửa file `SeedData_TrieuChung_MaLoi_GiaiPhap.sql` gốc và chưa tạo migration.

## 11. Cập nhật 05/11/2026 (Claude): bản patch từ điển cho schema AlignErdV2 và tự rà soát

File: `Database\Patch_TuDien_DoPhu_20261105_AlignErdV2.sql` (sinh bởi `MoHinh-ChanDoanLoi\tao_patch_alignerdv2.py`). Cùng nội dung với bản trước (11 triệu chứng, 4 mã lỗi + giải pháp, 59 liên kết), đổi tên cột theo database test của Codex: `TRIEU_CHUNG(MaTrieuChung, TenTrieuChung)`, `MA_LOI(MaLoi, TenLoi, PhanLoai, TrangThaiSuDung bit)`, `GIAI_PHAP(MaGiaiPhap, MaLoi, NoiDungTungBuoc, MucDoKho, TrangThaiSuDung bit)`, `TRIEU_CHUNG_LOI(MaTrieuChung, MaLoi)`.

Kiểm chứng: trên database tạm LocalDB (đã xóa), chạy seed gốc, đổi tên cột/kiểu mô phỏng AlignErdV2, rồi áp patch hai lần: 44/37/52/37 → 55/41/111/41, lần hai không đổi. Sau patch: 0 triệu chứng chưa liên kết, 0 mã lỗi chưa có triệu chứng, 0 mã lỗi chưa có giải pháp. Đây là mô phỏng schema, chưa chạy trên database test của Codex.

Lưu ý quan trọng: database test của Codex có tên tiếng Việt bị hỏng ký tự (`?` thật trong dữ liệu). Patch ghép theo **tên** nên **sẽ không khớp** trên database đó; chỉ dùng được với database có tên đúng dấu (từ seed gốc).

Tự rà soát nội dung (theo ủy quyền của owner: tự làm, tự kiểm duyệt), 05/11/2026:
- Đã đọc lại toàn bộ 59 liên kết và 4 giải pháp mới; liên kết hợp lý theo kiến thức sửa chữa PC phổ thông (ví dụ: màn hình vỡ/dead pixel → LCD; "Low Disk Space" → ổ đầy; mã dừng BSOD → đúng mã lỗi).
- Phần thận trọng/chưa chắc: "Driver thiết bị bị lỗi" trỏ tới 8 lỗi nên sẽ hòa điểm; "Desktop/icon bị mất" → Registry/file hệ thống chỉ là suy đoán phổ biến; "Bluetooth" gắn với lỗi riêng thay vì lỗi WiFi combo. Giải pháp mới chỉ là quy trình chuẩn từ kiến thức chung, chưa đối chiếu tài liệu hãng.
- Phê duyệt này của Claude **không thay thế** xác minh bằng ca sửa chữa thật; liên kết vẫn nên được hiệu chỉnh khi có dữ liệu `Đã xác minh`.
- Chưa áp dụng vào database Somee/SQL Express của bạn và chưa gộp vào file seed gốc.

## 12. Cập nhật 05/11/2026 (Claude): rà soát, sửa lỗi và chạy lại Zenodo sau khi loại bản ghi PII

### 12.1 Sửa lỗi trong source chính (phát hiện khi rà soát)
- **Migration `AlignErdV2` chặn mọi database trừ database test của Codex** (`IF DB_NAME() <> N'ITHelpdesk_ERDV2_Test_20261104' THROW`). Vì `Program.cs` chính gọi `Database.Migrate()` lúc khởi động, app sẽ không khởi động được với database thật. Đã thay bằng chốt chặn chủ động: chỉ chạy khi đặt biến môi trường `ALLOW_ALIGN_ERDV2=1` **sau khi sao lưu database** (hoặc trên database test của Codex). Đã thử: không đặt biến → migration ném lỗi 51000; đặt biến → migration áp dụng thành công.
- Xóa `Data/ErdV2DesignTimeFactory.cs` (khóa `dotnet ef` vào database test của Codex).
- `Program.cs`: khối nạp dữ liệu mồi chèn `MaPhieu = 1` cả khi chưa có phiếu nào → vi phạm FK (FK này có từ migration `AddHybridAI`, lỗi có sẵn từ trước). Đã bỏ qua bước nạp mồi khi chưa có phiếu.
- Smoke test source chính trên database LocalDB tạm rỗng (đã xóa): chuỗi migration chạy đến `AlignErdV2`, 21 bảng, 31 FK (khớp số của Codex), app khởi động, `/`, `/Auth/Login`, `/Home/Landing`, `/Home/Welcome` trả 200, không có exception trong log. **Chưa kiểm tra đăng nhập, luồng nghiệp vụ và database thật**; `/PhieuTiepNhan` trả 404 (chưa tìm hiểu route).

### 12.2 Zenodo: loại 6 bản ghi có dấu hiệu PII và chạy lại toàn bộ
Loại 5 bản ghi train và 1 bản ghi test có cờ `PHONE_OR_ID`/`IPV4` (từ `RECORD-AUDIT.jsonl`) trước khi khử trùng; kết quả cũ lưu ở `thuc-nghiem\truoc_loai_PII\`. Sau xử lý: 913 train / 550 test, 6 nhóm. Phát hiện PII chỉ là quét nhanh, không phải chứng nhận sạch.

| Mô hình | Accuracy [95% CI] | Macro-F1 [95% CI] | Top-3 | LogLoss |
|---|---|---|---|---|
| Lớp đa số | 0,349 | 0,086 | 0,658 | – |
| TF-IDF + hồi quy logistic (C=30) | 0,782 [0,747–0,816] | 0,729 [0,686–0,769] | 0,953 | 0,618 |
| DistilBERT-uncased EN đóng băng + hồi quy logistic | 0,735 [0,698–0,771] | 0,662 [0,614–0,706] | 0,924 | 0,907 |
| mDistilBERT đa ngôn ngữ fine-tune (epoch tốt nhất 3/6) | 0,724 [0,685–0,760] | 0,655 [0,609–0,699] | 0,920 | 0,774 |
| **ML.NET SDCA (l2=1e-5)** | **0,820** [0,787–0,851] | **0,776** [0,734–0,813] | **0,964** | **0,582** |
| Rule-based từ khóa | 0,549 [0,507–0,589] | 0,487 [0,442–0,531] | – | – |

Nhận xét: thứ tự mô hình không đổi (SDCA ≥ TF-IDF/hồi quy logistic > DistilBERT > Rule > lớp đa số). Bảng ở mục 7 là bản **trước khi loại PII** và đã được thay thế bởi bảng này.
**Độ biến thiên của mDistilBERT lớn:** cùng seed nhưng chỉ khác 6 bản ghi, Macro-F1 test từ 0,695 xuống 0,655 và epoch tốt nhất từ 6 xuống 3, vì tập validation chỉ có khoảng 135 mẫu nên việc chọn epoch không ổn định. Vì vậy chưa thể kết luận mDistilBERT thua TF-IDF/SDCA một cách chắc chắn; chỉ có thể nói rằng trong điều kiện CPU, dữ liệu nhỏ và ít epoch này, nó không vượt các mô hình tuyến tính.

## 13. Cập nhật 05/11/2026 (Claude): Priority EN 002 (thăm dò, chỉ VALIDATION)

Thiết kế: `private\...\priority002-plan-PROPOSED.json` của Codex (trạng thái ban đầu `PROPOSED_NOT_APPROVED_NOT_BUILT`); owner đã chọn làm Priority EN 002 và ủy quyền tự làm/tự kiểm duyệt nên mình chấp nhận hai giả thuyết và ngưỡng như đã ghi, **không đổi ngưỡng sau khi xem kết quả**. Runner độc lập: `Datasets Đồ án chuyên nghành\MoHinh-UuTien\priority-en-002\` (ML.NET 5.0.0, SDCA, seed 20261017, 100 vòng lặp, 1 luồng, đúng pipeline Priority EN 001). Dữ liệu: TRAIN 4.597 và VALIDATION 986 của release `priority-en-source-label-v1-offline-20261017`. **Không đọc `test.jsonl`** (runner không mở file này). Nhãn nguồn là tổng hợp, chưa xác minh; đây không phải mức khẩn cấp thực tế.

Kiểm tra: trọng số tính từ TRAIN trùng đúng số trong kế hoạch (0,80385 / 1,82321 / 1,06319); Save/Reload cho dự đoán giống hệt ở cả hai cấu hình. Model: `model_P2-A-moderate-weights.zip` SHA-256 `f1ce7e24…aa9b8b`, `model_P2-B-word-tfidf.zip` SHA-256 `70b8963d…09f066`.

| VALIDATION (986) | Accuracy | Macro-F1 | High recall | High→Low | Low F1 | Medium F1 | Đạt ngưỡng |
|---|---|---|---|---|---|---|---|
| Priority EN 001 (tham chiếu, SDCA) | 0,583 | 0,393 | 0,835 | 2 | 0,085 | 0,380 | – |
| P2-A trọng số vừa phải | 0,585 | **0,476** | 0,774 | 13 | 0,314 | 0,412 | **Không** |
| P2-B TF-IDF theo từ | 0,586 | 0,345 | 0,916 | 1 | 0,018 | 0,290 | **Không** |

Ngưỡng đã đóng băng: High recall ≥ 0,82; High→Low ≤ 1% (tối đa 5 mẫu); Macro-F1 ≥ 0,4127; Low F1 ≥ 0,12; Medium F1 ≥ 0,3803.

Kết luận (theo quy tắc chọn đã ghi trong kế hoạch): **không cấu hình nào đạt → giữ Priority EN 001 làm tham chiếu, không tuyên bố mô hình thắng.**
- P2-A cải thiện rõ Macro-F1 (0,393 → 0,476) và nhận ra lớp Low (recall 0,046 → 0,220) nhưng đánh đổi: High recall giảm còn 0,774 và 13 phiếu High bị xếp thành Low (trong 558), vi phạm ngưỡng an toàn "không bỏ sót phiếu khẩn". Ngưỡng này được đóng băng trước nên không nới sau khi xem kết quả.
- P2-B không giúp ích: gần như bỏ qua lớp Low (recall 0,009).
- Hạn chế: VALIDATION đã được dùng lại nhiều lần nên đây là thăm dò thích nghi, không phải xác nhận độc lập; TEST vẫn đóng, không dùng để chọn. Phân phối nhãn là tổng hợp. Chưa ánh xạ sang SLA/mức ưu tiên của website, chưa kiểm chứng thời điểm tiếp nhận.
- Không chạy thêm cấu hình hoặc tinh chỉnh; mọi cải tiến tiếp theo cần cohort đánh giá mới có provenance, trùng lặp và privacy được kiểm, theo kế hoạch của Codex.

## 14. Cập nhật 05/11/2026 (Claude): rà soát Disk Failure 001 (đọc artifact, không huấn luyện lại, không chạy lại TEST)

Nguồn: `D:\Code\helpdesk-data-audit\stage15-2026-10-16\private\mlnet-runs\disk-failure-001-pilot\` (model, freeze, validation, test, technical), `private\dataset-candidates\disk001-hdd7-feature-v1\`, runner `disk-research-runner-001\Program.cs`.

### Đã kiểm chứng trực tiếp
- `model.zip` SHA-256 `d0ccce12…eea1e` khớp `freeze.json` và `checksums.json`. Manifest đặc trưng SHA-256 `55f01809…0700c` khớp bản bàn giao; **19/19 file mảng đặc trưng/nhãn/nhóm khớp hash**; kích thước = hàng × 65 × 4 byte; số mẫu dương 2.584 / 124 / 107 (train/validation/test) khớp; **tập serial không giao nhau giữa các split**.
- Số học chỉ số khớp ma trận nhầm lẫn (precision, recall, false alerts/1000 drive-day tính lại đều đúng).
- Mô hình: ML.NET `LightGbmBinaryTrainer` (20 lá, learning rate 0,05, 100 vòng, 2 luồng, seed 20261016), huấn luyện trên **50.000/200.416 hàng TRAIN** (toàn bộ 2.584 mẫu dương + mẫu âm cố định), trọng số mẫu âm 192,1. Một lượt Fit, một lượt dự đoán TEST (log `foreground.log`: Save/Reload PASS, "FROZEN; BEGIN single TEST", COMPLETE, không kích hoạt).
- Thiết kế đặc trưng: 65 cột từ 9 cảm biến SMART (5, 9, 187, 188, 193, 194, 197, 198, 199), chỉ dùng dữ liệu quá khứ; chính sách ghi rõ **không dùng trường tương lai/hỏng làm đặc trưng và không dùng tên model ổ làm đặc trưng**; nhãn dương = hỏng đầu tiên trong (t, t+h] với quan sát liên tục; mất dấu không tính là hỏng.

### Kết quả (ngưỡng đóng băng 0,9999977, chọn trên VALIDATION)
| | VALIDATION | TEST |
|---|---|---|
| ROC-AUC | 0,895 | 0,872 |
| PR-AUC (tỷ lệ dương: 0,00026 / 0,00014) | 0,0187 (gấp ~71 lần no-skill) | 0,0143 (gấp ~105 lần) |
| Precision / Recall (theo drive-day) | 6,9% / 5,6% | 2,8% / 4,7% |
| Sự kiện phát hiện được (event recall) | 4/29 = 13,8% | 2/28 = 7,1% |
| Wilson 95% của event recall (tham khảo) | 5,5–30,6% | 2,0–22,6% |
| Cảnh báo sai / 1.000 drive-day | 0,201 | 0,219 |
| Số ngày báo trước | 1, 2, 2, 3 | 7, 7 |
| Brier (mô hình / no-skill) | 0,00165 / 0,00026 | 0,00155 / 0,00014 |

### Nhận xét và điều cần thận trọng
1. **Có khả năng xếp hạng nhưng yếu khi vận hành**: ngưỡng đóng băng chỉ bắt được 2/28 ổ hỏng ở TEST; khoảng tin cậy rộng vì chỉ 28–29 serial dương, và ngưỡng được chọn thích nghi trên VALIDATION.
2. **Xác suất chưa hiệu chuẩn**: Brier của mô hình **tệ hơn no-skill khoảng 11 lần** ở TEST; nhóm xác suất cao nhất (trung bình 0,985) chỉ có tỷ lệ hỏng thực tế khoảng 3%. Nguyên nhân khả dĩ: trọng số mẫu âm 192 và huấn luyện trên tập con. Không trình bày đầu ra là "% nguy cơ hỏng"; chỉ dùng thứ hạng.
3. **Lệch tỷ lệ nền**: tỷ lệ dương VALIDATION gấp ~2 lần TEST; tỷ lệ tiên nghiệm khi huấn luyện 0,00028.
4. **Phạm vi dữ liệu**: Backblaze, ổ cứng trung tâm dữ liệu (HDD ≥ 8TB theo bộ lọc), không phải ổ laptop/PC khách hàng của đề tài; hàng drive-day phụ thuộc nhau trong cùng serial; không có khoảng tin cậy độc lập cho từng hàng. Không suy ra khả năng áp dụng cho ổ của khách; hệ thống chưa có luồng nhập SMART và chưa ánh xạ vào phiếu.
5. Chỉ dùng 25% hàng TRAIN; chưa thử toàn bộ TRAIN hay hiệu chuẩn xác suất; đã có TEST nên **không tinh chỉnh ngưỡng/model theo TEST** (đúng bàn giao).
6. Tài liệu cũ `public\disk001-experiment\STATUS-VI.md` (ghi "chưa Fit/Save") đã lỗi thời so với log; trạng thái đúng là đã chạy xong một lần.

### Chưa kiểm chứng
Không chạy lại mô hình, không tái dự đoán, không kiểm tra từng dự đoán; chỉ đọc số liệu và hash đã lưu. Không đối chiếu dữ liệu RAW Backblaze gốc với mảng đặc trưng (ngoài hash/kích thước/số mẫu dương).

## 15. Cập nhật 05/11/2026 (Claude): đối chiếu các tập dữ liệu đã dùng với tiêu chuẩn dữ liệu của owner

Tiêu chuẩn (chat Codex, mục tiêu Master Dataset): **tối thiểu 300.000 bản ghi PC phù hợp, ưu tiên 500.000+**, nhiều nguồn, tách riêng corpus / source-labeled / intake-supervised / confirmed-cause, **không dùng synthetic hoặc pseudo-label để tăng số lượng dữ liệu thật**; owner làm rõ không bắt buộc 300.000 bản ghi cùng một loại nhãn và 300.000 không phải điều kiện bắt buộc để chạy thực nghiệm supervised có nhãn hợp lệ. Hiện trạng theo Codex: Master 60.307 bản ghi (thiếu ~239.693 đến mốc 300.000), phần lớn là bug report phần mềm, ConfirmedFault = 0.

| Tập đã dùng trong đợt này | Quy mô sử dụng | Loại dữ liệu | Có tính vào 300.000? |
|---|---|---|---|
| Zenodo 7648117 (nhóm dịch vụ) | 913 train / 550 test (sau khử trùng, loại PII) | Source-labeled, phiếu IT thật, nhãn nhóm dịch vụ; chỉ ~150 bản ghi có từ khóa PC (chưa xác minh) | Không: ngoài phạm vi PC, chưa xác minh phạm vi, chưa vào master |
| aa_dataset (mức ưu tiên) | 7.633 fit / 1.344 val / 3.965 test | **Synthetic**, hỗ trợ khách hàng, EN+DE | **Không** (synthetic) |
| Priority EN 002 (release của Codex) | 4.597 train / 986 val | Nhãn nguồn synthetic | Không (synthetic, chưa xác minh) |
| Từ điển chẩn đoán (vá) | 55 triệu chứng / 41 lỗi / 111 liên kết | Tri thức/rule, không phải ca | Không (catalog không phải dữ liệu ca) |
| Disk Failure 001 (đọc lại) | 200.416 / 468.212 / 786.114 drive-day | SMART thật nhưng drive-day phụ thuộc theo serial; ổ trung tâm dữ liệu | Không tính là ca sửa chữa PC (theo bàn giao) |

Kết luận: **không tập nào trong đợt này đóng góp vào mục tiêu 300.000 bản ghi PC phù hợp và đều chỉ là thực nghiệm thăm dò quy mô nhỏ.** Các số liệu mô hình ở mục 6–13 phải được trình bày kèm giới hạn này; không dùng để khẳng định chất lượng chẩn đoán PC.

## 16. Cập nhật 05/11/2026 (Claude): rà soát Routing EN (001, 002, v3) — đọc artifact, không huấn luyện lại, không chạy TEST

Nguồn: `private\mlnet-runs\routing-en-v2-run001`, `...run002-c-only`, `...routing-en-v3-learning-curve`, `public\routing-v3-results\REPORT.md` và `verification-and-metrics.json`.

### Đã kiểm chứng trực tiếp
- SHA-256 6 file model tính lại **khớp** hồ sơ: v3 025/050/075/100 (`770b088f…`, `3ffd947c…`, `a5cc67b7…`, `1a2c5fbd…`), C-only (`95fe56a3…c66c41`, đúng giá trị bàn giao), run001 (`930cfb08…`).
- Accuracy và Macro-F1 của cả 4 mốc v3 **tính lại từ ma trận nhầm lẫn** (949 mẫu VALIDATION) trùng với số đã lưu đến 6 chữ số. Số liệu Routing 002-A trùng với validation của run001 (Macro-F1 0,5203) như bàn giao nêu.
- Báo cáo xác nhận: một lần Fit/Save/Reload mỗi mốc, không Fit/Save và không suy luận TEST khi xác minh; không có bằng chứng kích hoạt website.

### Kết quả (ba lớp nguồn: IT Support 261, Service Outages and Maintenance 91, Technical Support 597 trên VALIDATION)
| | Accuracy | Macro-F1 | IT Support recall | Technical recall |
|---|---|---|---|---|
| Lớp đa số (Technical) VALIDATION | 0,629 | 0,257 | 0 | 1 |
| Rule VALIDATION (run001) | 0,652 | 0,417 | 0,008 | 0,982 |
| SDCA 001/002-A VALIDATION (2.387 train) | 0,668 | 0,520 | 0,088 | 0,943 |
| **v3 075-A (3.412 train, được chọn)** | **0,681** | **0,561** | 0,184 | 0,925 |
| v3 100-A (4.549 train) | 0,680 | 0,548 | 0,138 | 0,940 |
| Run001 TEST chính (955): đa số / Rule / SDCA | 0,641 / 0,648 / **0,667** | 0,260 / 0,378 / **0,504** | | |

### Nhận xét
1. **Cải thiện nhỏ so với lớp đa số**: SDCA hơn lớp đa số khoảng 2,6 điểm Accuracy trên TEST (0,667 so với 0,641) và khoảng 5 điểm trên VALIDATION với mốc 075; Macro-F1 cao hơn rõ vì nhận ra lớp Service Outages. Lớp **IT Support gần như không tách được khỏi Technical Support**: ở mốc 075, 206/261 phiếu IT Support bị xếp thành Technical Support (recall 0,18).
2. **Đường cong học không đơn điệu**: 075 > 100 (Macro-F1 0,561 → 0,548; IT recall 0,184 → 0,138). Chỉ một seed và VALIDATION đã dùng thích nghi nhiều lần nên không kết luận nguyên nhân hay độ biến thiên; báo cáo của Codex ghi rõ đây là quan sát, không phải cơ chế đã chứng minh. Việc chọn 075 theo ngưỡng Technical recall đóng băng 0,793 là đúng quy tắc đã đặt trước.
3. **Ý nghĩa**: nhãn là *queue nguồn* (không phải queue lúc tiếp nhận đã xác minh), nhiều khả năng từ dữ liệu hỗ trợ khách hàng tổng hợp cùng họ với `aa_dataset` mình dùng ở mục 8 (mình chưa đối chiếu lineage release); đây là phân loại nguồn thăm dò, **không phải chẩn đoán nguyên nhân PC** và không thuộc 300.000 bản ghi PC phù hợp.

### Chưa kiểm chứng
Không chạy lại model hay tái dự đoán; không kiểm tra nội dung từng ticket; không kiểm chứng nguồn gốc RAW của release; chưa đối chiếu lineage để khẳng định cùng nguồn với `aa_dataset`.

## 17. Cập nhật 05/11/2026 (Claude): rà soát CompCat (EN142 Deep Learning và EN8 ML.NET) — đọc artifact, không chạy lại, không mở TEST

Nguồn: `stage15-2026-10-16\private\pc-master-experiment\runs\b52438ca…\` và release `pc-master-compcat-en142-retrospective-v1-20261022`; `pc-diagnosis-master-v1-20261022\compcat-mlnet-feasibility-v1-pathfix1-20261023\results-20261023\`.

### Đã kiểm chứng trực tiếp
- **EN142 (Deep Learning):** SHA-256 `VALIDATION.jsonl` (`785d2a1a…`) khớp `validation_file_sha256` của kết quả SDCA; `manifest.json` (`f183a9bf…`) khớp `dataset_sha256`; model SDCA `sdca.zip` (`be88fd3e…`) khớp `model_sha256`. Release chỉ có 99 TRAIN và 23 VALIDATION; TEST chỉ tồn tại dạng tham chiếu niêm phong (`TEST-SEALED-REFERENCES.json`), kết quả ghi `test_opened: false`. Accuracy/Macro-F1 của SDCA tính lại từ 23 dự đoán đã lưu: 0,696 / 0,624, khớp.
- **EN8 (ML.NET SDCA):** `run001/model.zip` SHA-256 `a56de590…311c20` khớp `postcheck.json`; Accuracy 0,646154 và Macro-F1 0,653886 **tính lại từ ma trận nhầm lẫn** khớp số đã lưu. Log Codex ghi một lượt Fit (0,40 giây), Save/Reload PASS, chỉ VALIDATION, không TEST.

### Kết quả (nhãn: nhóm lỗi nguồn CompCat, phân loại hồi cứu — không phải xác minh nguyên nhân lúc tiếp nhận)
| | Tập | Accuracy | Macro-F1 |
|---|---|---|---|
| EN142: lớp đa số (Power/battery) | VALIDATION 23 (4 lớp) | 0,478 | 0,162 |
| EN142: ML.NET SDCA | 23 | 0,696 | 0,624 |
| EN142: Rule từ khóa (độ phủ 87%) | 23 | 0,783 | 0,823 |
| EN142: DistilBERT-EN đóng băng + head 98.948 tham số (epoch tốt nhất 4/5) | 23 | **0,870** (độ phủ 100%) | 0,814 |
| EN8: lớp đa số (Performance) | VALIDATION 65 (8 lớp) | 0,246 | 0,049 |
| EN8: ML.NET SDCA | 65 | 0,646 (Wilson 95%: 0,525–0,751) | 0,654 |

### Nhận xét và hạn chế
1. **Rất nhỏ**: 23 và 65 mẫu validation. Với 20/23 đúng, khoảng Wilson 95% của accuracy DistilBERT khoảng 0,68–0,96, nên chênh lệch với SDCA (16/23) hoặc Rule không đủ để kết luận mô hình nào tốt hơn. Wilson giả định các mẫu độc lập, chưa được chứng minh (độc lập thiết bị/episode chưa xác minh).
2. **So sánh với Rule không ngang bằng**: Rule bỏ qua 13% ca (độ phủ 87%) còn DistilBERT trả lời tất cả; Macro-F1 Rule (0,823) và DistilBERT (0,814) gần như bằng nhau nên không kết luận DL thắng từ khóa.
3. **Epoch tốt nhất là epoch cuối (4/5)** và Macro-F1 validation còn tăng ở mỗi epoch, chạy chỉ 5 epoch trên 99 mẫu: có thể chưa hội tụ; chọn epoch trên cùng 23 mẫu validation nên số liệu lạc quan. Xác suất chưa hiệu chuẩn (Brier/ECE ghi là chỉ mô tả; `score_calibration: NOT_VERIFIED`).
4. **Phạm vi**: mô hình EN142 chỉ nhận 4 nhóm (Power/battery, Operating system, Overheating, Internal storage), tiếng Anh; kết quả `training_eligible: false`, `pc_diagnosis_trained: false`, trạng thái `CANDIDATE_NOT_APPROVED_NOT_DEPLOYED`; không phải mô hình chẩn đoán 36 mã lỗi của hệ thống và không thuộc 300.000 bản ghi PC phù hợp.
5. EN8 hơn lớp đa số 40 điểm accuracy nhưng nhãn nguồn là loại lỗi hồi cứu (Boot, Screen, Keyboard...), nên chỉ chứng tỏ phân loại văn bản mô tả sửa chữa sang nhóm nguồn có thể học được trên dữ liệu nhỏ; lớp Ports/slots recall 0,29, Configuration F1 0,35 yếu.
6. Một số tệp tài liệu trong phần Codex (quy trình chữ ký/receipt) khá nặng so với yêu cầu đồ án; chỉ dùng làm bằng chứng lịch sử, không áp dụng lại (theo bàn giao 05/11).

### Chưa kiểm chứng
Không chạy lại model hay tái dự đoán; không kiểm tra nội dung từng hồ sơ CompCat; chưa kiểm chứng `onnx_parity` (4 ca, sai số tối đa 1,9×10⁻⁶ theo log) bằng chạy .NET; chưa kiểm chứng độc lập giữa TRAIN/VALIDATION/TEST (nhóm thiết bị/episode).

## 18. Tổng hợp toàn bộ (05/11/2026, Claude)

Mọi số liệu lấy từ mục 6–17 của báo cáo này và artifact đã nêu; **không tập nào đạt tiêu chuẩn 300.000 bản ghi PC phù hợp**, và không mô hình nào là mô hình chẩn đoán lỗi PC đủ điều kiện triển khai. Cột "Kiểm chứng" ghi mức mình đã làm: *chạy* (mình tự chạy), *đọc* (đọc artifact, tính lại chỉ số, kiểm hash).

| # | Bài toán | Dữ liệu (quy mô, loại) | Kết quả chính (tập đánh giá) | Kiểm chứng | Dùng được cho |
|---|---|---|---|---|---|
| 1 | Nhóm dịch vụ phiếu IT (6 nhóm) | Zenodo 7648117; 913 train / 550 test; phiếu IT thật, nhãn nhóm dịch vụ | SDCA 0,820 / Macro-F1 0,776; TF-IDF 0,782 / 0,729; mDistilBERT 0,724 / 0,655; DistilBERT-EN đóng băng 0,735 / 0,662; Rule 0,549 / 0,487; đa số 0,349 / 0,086 | chạy | Thực nghiệm phụ trợ phân loại yêu cầu; không phải chẩn đoán lỗi PC |
| 2 | Mức ưu tiên (high/medium/low) | `aa_dataset` lọc IT; 7.633 / 1.344 / 3.965; **synthetic**, EN+DE | TF-IDF 0,637 / 0,557; SDCA 0,643 / 0,534; mDistilBERT (mẫu con) 0,503 / 0,419; Rule 0,430 / 0,349; đa số 0,584 / 0,246 | chạy | Thăm dò; nhãn synthetic, hơn đa số khoảng 5 điểm |
| 3 | Priority EN 001/002 (release của Codex) | 4.597 train / 986 val / 985 test; nhãn nguồn synthetic | 001: TEST 0,585 / 0,395 (High recall 0,855, Low 0,065); 002-A VALIDATION Macro-F1 0,476 nhưng 13 ca High→Low; **002 không đạt ngưỡng** | 001 đọc; 002 chạy | Giữ 001 làm tham chiếu; không triển khai |
| 4 | Routing EN (3 queue nguồn) | release EN; 2.387–4.549 train / 949 val / 955 test | v3 mốc 075 VALIDATION 0,681 / 0,561; run001 TEST 0,667 / 0,504 (đa số 0,641) | đọc | Thăm dò; IT Support recall chỉ 0,18; không phải chẩn đoán |
| 5 | Dự báo hỏng ổ cứng từ SMART | Backblaze Q2/2026; 200.416 / 468.212 / 786.114 drive-day; ổ trung tâm dữ liệu | TEST ROC-AUC 0,872; PR-AUC 0,0143 (gấp ~105 lần no-skill); event recall 2/28; xác suất chưa hiệu chuẩn (Brier tệ hơn no-skill) | đọc | Mô-đun cảnh báo SMART tiềm năng; chưa có luồng nhập SMART; chỉ dùng thứ hạng |
| 6 | CompCat EN142 (4 nhóm lỗi, DL) | 99 / 23 val; TEST niêm phong | DistilBERT-EN đóng băng + head: 0,870 / 0,814; SDCA 0,696 / 0,624; Rule (độ phủ 87%) 0,783 / 0,823 | đọc | Chỉ tham khảo; 23 mẫu, khoảng tin cậy rộng |
| 7 | CompCat EN8 (8 nhóm lỗi, ML.NET) | 307 / 65 val | SDCA 0,646 / 0,654 (đa số 0,246) | đọc | Chỉ tham khảo; một lượt Fit |
| 8 | Chẩn đoán triệu chứng → mã lỗi (Rule) | Từ điển seed: 44 triệu chứng / 37 lỗi / 52 liên kết | **Không huấn luyện.** Độ phủ rule: 18/44 triệu chứng và 3/37 lỗi chưa liên kết; 6 cặp lỗi không phân biệt được. Patch: 55 / 41 / 111, 0 chưa liên kết, 37/41 lỗi xếp hạng 1 duy nhất | chạy (DB tạm) | Cải thiện rule; chưa kiểm chứng bằng ca thật |

### Kết luận chung
- Các mô hình tuyến tính (SDCA/TF-IDF) **bằng hoặc hơn** DistilBERT trên các tập nhỏ ở đợt này; chưa có bằng chứng Deep Learning vượt baseline đơn giản. Hạn chế: CPU, ít epoch, validation nhỏ, chỉ một seed, DistilBERT-EN trên dữ liệu đa ngôn ngữ.
- Dữ liệu synthetic hoặc nhãn nguồn chiếm đa số; không có nhãn `ConfirmedFault`. Kết quả không chứng minh khả năng chẩn đoán lỗi PC thực tế.
- Mọi thí nghiệm mình *chạy* đều thuộc loại thăm dò; các thí nghiệm của Codex mình chỉ *đọc* và xác nhận hash/số học, không chạy lại.

### Việc hệ thống còn lại (không phải thí nghiệm)
1. **Trước khi chạy source chính với database thật:** sao lưu database, đặt `ALLOW_ALIGN_ERDV2=1` để migration `AlignErdV2` chạy (mục 12). Chưa thử trên Somee hay SQL Express; chưa kiểm tra đăng nhập và luồng nghiệp vụ trên source chính.
2. **Áp patch từ điển** (`Database\Patch_TuDien_DoPhu_20261105_AlignErdV2.sql`) chỉ sau khi duyệt và trên database có tên tiếng Việt đúng dấu.
3. **Dữ liệu ca thật:** thu thập qua hệ thống và xác minh bằng endpoint của Codex (`Verification.ErdV2.cs`) để có nhãn `Đã xác minh`; đây là điều kiện để huấn luyện mô hình chẩn đoán lỗi.
4. **Chương 3 (do owner tự viết):** nên trình bày các kết quả ở trên là thực nghiệm phụ trợ/thăm dò, nêu rõ loại dữ liệu và giới hạn từng dòng, không khẳng định hiệu quả triển khai.
5. Dọn dẹp tùy chọn: xóa thư mục sao lưu `D:\Code\Đồ án chuyên nghành\Backup_truoc_dong_bo_ERDV2_20261105\` và các log `smoke_*.log` khi không cần.

## 19. Cập nhật 05/11/2026 (Claude): bộ mô hình chuẩn `MoHinh-Chuan`

Thư mục: `D:\Code\Đồ án chuyên nghành\MoHinh-Chuan\` (README.md, `run_suite.py`, `mlnet_sdca\`, `dl_finetune.py`, `configs\`, `results\`). Mục đích: chạy **thống nhất, tái lập được** các mô hình Rule, TF-IDF + hồi quy logistic, ML.NET SDCA và mDistilBERT trên cùng một hợp đồng dữ liệu (`id,text,label,source_type,group,split`), cùng một lần chia (khử trùng, chia theo nhóm, loại rò rỉ, test không chứa synthetic), cùng chỉ số (Accuracy/Macro-F1 kèm bootstrap 95% CI, Top-3, LogLoss, ma trận nhầm lẫn) và **cổng kích hoạt mô phỏng hệ thống** (≥100 bản ghi thực đã xác minh, Micro ≥ 0,6, Macro ≥ 0,5, không kém Rule quá 0,10 — ngưỡng 0,10 là giả định của mình). Tự sinh thẻ mô hình `MODEL_CARD_*.md` kèm SHA-256 dữ liệu/model. Dữ liệu mới chỉ cần đổi file CSV và config.

Kiểm tra tái lập trên Zenodo (cùng dữ liệu, loại 6 bản ghi PII và nhãn EOL; test 549 mẫu, trước đây 550):
| Mô hình | Bộ chuẩn: Accuracy / Macro-F1 | Mục 12 (trước đây) |
|---|---|---|
| Lớp đa số | 0,350 / 0,086 | 0,349 / 0,086 |
| Rule | 0,550 / 0,488 | 0,549 / 0,487 |
| TF-IDF + hồi quy logistic | 0,776 / 0,718 | 0,782 / 0,729 |
| ML.NET SDCA | 0,798 / 0,747 | 0,820 / 0,776 |
| mDistilBERT | 0,730 / 0,635 | 0,724 / 0,655 |

Thứ tự mô hình và kết luận không đổi (SDCA ≥ TF-IDF > mDistilBERT > Rule > lớp đa số). Chênh lệch nhỏ do quy trình chuẩn chọn siêu tham số trên một tập val tách theo nhóm (SDCA chọn l2 trên val thay vì CV; mDistilBERT chọn epoch trên val 136 mẫu, kết quả dao động theo cách chia). Lần chạy đầu của bộ chuẩn cho test 519 mẫu vì khử trùng gộp chung làm bản trùng bị loại khỏi test; đã sửa thành khử trùng theo từng tập (log lần chạy đầu không giữ lại).

Hạn chế:
- Trạng thái đánh giá của Zenodo là **THAM_DO** (không có nhãn thực đã xác minh); cả ba mô hình học máy đều ghi **KHÔNG đủ điều kiện kích hoạt** (thiếu bản ghi thực xác minh).
- Bộ này dành cho bài toán văn bản và so sánh; **chưa thay** `AITrainingBackgroundService` (vector triệu chứng 500 chiều) và chưa có xuất ONNX/.NET riêng (Codex đã có prototype).
- Chưa chạy bộ chuẩn trên `aa_dataset` (ưu tiên) và chưa kiểm tra bằng dữ liệu `real_verified` vì chưa có.

## 20. Cập nhật 05/11/2026 (Claude): nối bộ mô hình chuẩn vào hệ thống

Phạm vi (đã được owner chọn): kết nối an toàn, **không tự kích hoạt mô hình**. Việc huấn luyện và kích hoạt thật vẫn do `AITrainingBackgroundService` và `ActivateModel` (các chốt an toàn hiện có) quyết định.

### Thay đổi trong source chính
- `Services/TrainingDataExporter.cs` (mới) và action `GET /Admin/XuatDuLieuHuanLuyen` trong `AdminController` (`[Authorize(Roles="Admin KTV")]`): xuất ZIP gồm `training.csv` theo hợp đồng của `MoHinh-Chuan` (`id,text,label,source_type,group`), `label_map.csv` và `export_info.json`. Không tạo nhãn mới; `real_verified` dùng đúng định nghĩa của `TrainingEligibility`; `LegacyRandom` bị loại; **mặc định không kèm `MoTaSuCo`** (tránh thông tin cá nhân), chỉ kèm khi gọi `?kemMoTaSuCo=true`.
- Nút **Xuất dữ liệu** trên `Views/Admin/MLModels/Index.cshtml`.
- **Sửa lỗi mới phát hiện khi nối:** `ActivateModel` (bản của Codex) dùng chốt `AI:AllowActivation` mặc định tắt nên nút kích hoạt luôn bị từ chối trên source chính; nay mặc định bật (đặt `AI:AllowActivation=false` để tắt). Cùng loại với chốt `AITraining:Enabled` đã sửa ở mục 12.
- **Sửa lỗi có sẵn ở `Program.cs`:** mỗi lần khởi động mọi bản ghi có mã ≤ 102 bị đánh dấu `LegacyRandom`, kể cả synthetic vừa nạp trên database mới (mất 97/1.700 bản ghi sau lần khởi động thứ hai); nay chỉ áp dụng cho nguồn `UserHistory`.
- `run_suite.py`: nếu hết bản ghi không-synthetic sau khử trùng thì dừng với thông báo rõ (trước đó lẳng lặng dùng toàn bộ dữ liệu, rồi vi phạm quy tắc test không có synthetic).

### Kiểm tra (database LocalDB tạm, đã xóa; dữ liệu mẫu ghi rõ là fixture)
- Build 0 lỗi. Đăng nhập bằng tài khoản fixture trả 302; `/Admin/XuatDuLieuHuanLuyen` trả 200 `application/zip`; **người ẩn danh bị chuyển hướng (302)**; `/Admin/MLModels` trả 200.
- ZIP đúng hợp đồng: 5 bản ghi fixture được phân loại đúng (1 `real_verified`, 2 `real_unverified`, 1 `public`, 1 bị bỏ vì không có triệu chứng) cộng 1.603 synthetic từ dữ liệu mồi; `label_map.csv` 37 nhãn; mô tả sự cố **không** xuất mặc định, **có** xuất khi bật tham số.
- Bộ chuẩn trên dữ liệu xuất: **từ chối đúng** (0 bản ghi không-synthetic sau khử trùng, vì dữ liệu triệu chứng của fixture trùng với synthetic) thay vì cho ra kết quả vô nghĩa. Hồi quy nhanh trên Zenodo (bốn mô hình không DL) vẫn cho số cũ: SDCA 0,798 / 0,747.

### Chưa kiểm chứng / hạn chế
- Chưa chạy trên database Somee/SQL Express và chưa có dữ liệu `real_verified` thật, nên chưa thấy bộ chuẩn chạy trọn vòng trên dữ liệu hệ thống.
- Hệ thống đưa **chuỗi tên triệu chứng** vào `text`; bộ chuẩn xử lý như văn bản (TF-IDF/SDCA/DL), khác với vector 500 chiều của `DiagnosticModel`; hai cách chưa được so sánh trực tiếp.
- Mới kiểm thử luồng xuất và quyền truy cập; chưa kiểm tra luồng huấn luyện/kích hoạt trên source chính sau các sửa lỗi chốt cấu hình.

## 21. Cập nhật 05/11/2026 (Claude): kiểm thử HTTP luồng nghiệp vụ trên source chính

Bộ kiểm thử: `D:\Code\Đồ án chuyên nghành\KiemThu-HTTP\http_workflow_main.py` (kết quả: `ket_qua_http_source_chinh.json`). Viết lại từ bộ kiểm thử HTTP của Codex cho **source chính** và tên cột sau `AlignErdV2`; tự tạo database LocalDB tạm, chạy migration bằng `ALLOW_ALIGN_ERDV2=1`, nạp dữ liệu mẫu (đều là fixture), chạy website thật và gọi HTTP, rồi xóa database. Không đụng database Somee/SQL Express.

**Kết quả: 33/33 kiểm tra PASS** (lần chạy đầu 31/33; hai lỗi là do bộ kiểm thử dùng nhầm mã linh kiện vì migration đã nạp sẵn kho, không phải lỗi hệ thống; đã sửa bộ kiểm thử và chạy lại).
- Khởi động sau migration; đăng nhập Admin KTV và KTV.
- Luồng: tiếp nhận (trường mới `MoTaSuCo`, `PhanLoaiYeuCau`, `MucDoUuTien` được lưu) → nhận ca → chẩn đoán → chốt đơn → xuất linh kiện (trừ tồn, lưu đơn giá xuất; thiếu tồn không đổi dữ liệu) → chốt phí/hoàn thành → in phiếu → đánh giá công khai (một-một, trùng bị từ chối).
- **Nhãn không tự được nâng**: chọn mã lỗi và chốt phí không tạo nhãn thực; xác minh nguyên nhân cần bằng chứng kiểm tra lại và người thực hiện hợp lệ; KTV không được giao ca bị chặn; đổi kết quả kiểm tra lại thu hồi điều kiện huấn luyện.
- **Huấn luyện**: người ẩn danh bị chặn; admin gửi yêu cầu được chấp nhận; vì chưa có nhãn thực đã xác minh đủ điều kiện, tác vụ **kết thúc ở trạng thái Failed kèm thông báo và không tạo mô hình mới**.
- **Kích hoạt**: mô hình fixture chỉ có 5 bản ghi thực bị chặn đúng ("cần tối thiểu 100 bản ghi thực"); chốt cấu hình `AI:AllowActivation` không còn chặn nhầm.
- Các trang Admin (`MLModels`, `Dashboard`, `ValidationSuite`), `PhieuTiepNhan/Index`, `Auth/Profile` trả 200; `/Admin/XuatDuLieuHuanLuyen` trả ZIP cho Admin và bị chặn với người ẩn danh.

Hạn chế: chỉ 33 kiểm tra trên database tạm với dữ liệu fixture; chưa có kiểm thử đồng thời nhiều người dùng, kiểm thử giao diện trực quan, kiểm thử bảo mật toàn diện (xác thực kiểu cũ so sánh mật khẩu trực tiếp vẫn là hành vi có sẵn); chưa chạy trên Somee/SQL Express và chưa có huấn luyện thành công vì thiếu nhãn thực đã xác minh.

## 22. Cập nhật 05/11/2026 (Claude): bỏ yêu cầu thu thập ca thật, thêm chế độ mô hình khởi tạo

Quyết định của owner (05/11/2026): **không cần thu thập và xác minh ca thật**; hệ thống coi như đã có AI và người dùng chỉ việc dùng. Hệ quả thiết kế: hệ thống phải huấn luyện/dùng được mà không có nhãn thật. Bàn giao của Codex ghi rõ không dùng synthetic làm ConfirmedFault; vì vậy mô hình tạo ra theo cách này được **gắn nhãn rõ "khởi tạo, chưa kiểm chứng bằng ca thật"**, không được báo là đã xác nhận chất lượng.

### Thay đổi trong source chính
- **Huấn luyện khởi tạo** (`AITrainingBackgroundService`): khi chưa có ≥ 2 nhóm thiết bị với nhãn thật đã xác minh, thay vì thất bại, hệ thống huấn luyện từ dữ liệu tổng hợp/công khai (bật mặc định, tắt bằng `AI:AllowBootstrapModel=false`). Đánh giá trên phần giữ lại chia theo tổ hợp (nhãn + bộ triệu chứng) sao cho **mỗi mã lỗi còn ít nhất một tổ hợp trong train**, không rò rỉ bản trùng. Mô hình được ghi `PhienBanBoDuLieu = KHOI_TAO_TONG_HOP_*`, thông báo tác vụ có `[KHỞI TẠO]`.
- **Kích hoạt** (`ActivateModel`): mô hình khởi tạo không bị chặn bởi "≥ 100 bản ghi thực" và ngưỡng tuyệt đối Micro/Macro, nhưng vẫn phải qua chốt **"AI không kém Rule quá N điểm"** (mặc định N = 10; chủ hệ thống nới bằng `AI:BootstrapMaxRuleGap`). Mô hình thường vẫn giữ nguyên các chốt cũ. Thông báo kích hoạt có cảnh báo khởi tạo.
- **Giao diện**: nhãn "Khởi tạo (tổng hợp)" ở danh sách mô hình; màn hình kết quả chẩn đoán hiện cảnh báo "mô hình khởi tạo, chưa kiểm chứng bằng ca thật; chỉ tham khảo".
- `Program.cs`: hạt giống cố định (20261105) cho dữ liệu mồi để kết quả lặp lại được.

### Ba lỗi có sẵn từ trước, tìm thấy khi kiểm thử và đã sửa
1. **Rule Engine luôn đạt 0% khi huấn luyện**: `TrieuChungsText` đánh dấu `[NoColumn]` nên không đi vào dữ liệu ML.NET; đánh giá Rule dùng danh sách triệu chứng rỗng. Hậu quả: `DoChinhXacRule` luôn 0 và chốt "AI không kém Rule quá 10%" luôn tự đạt. Nay lấy triệu chứng từ bản ghi kiểm thử.
2. **Thứ tự lớp sai khi đánh giá**: dịch vụ suy thứ tự lớp từ nhãn của ca thật trong CSDL thay vì từ mô hình (tràn mảng khi không có ca thật; sai lệch khi tập lớp khác nhau). Nay đọc chú giải `TrainingLabelValues` của cột `Score` (đã dò lược đồ thật của mô hình).
3. **Màn hình chẩn đoán không bao giờ hiện thẻ AI**: `Model.Load` chỉ trả lược đồ đầu vào nên đọc `PredictedLabel` luôn ném lỗi và bị `catch` nuốt im lặng. Nay đọc `Score.TrainingLabelValues` từ lược đồ đầu ra (có cache theo thời điểm sửa file) và ghi lỗi ra log thay vì nuốt. Không cần khởi động lại sau khi kích hoạt mô hình mới.
(Cùng lượt, đã sửa chốt `AI:AllowActivation` mặc định tắt và lỗi gắn nhãn `LegacyRandom` nhầm ở mục 20.)

### Kết quả kiểm thử HTTP trên source chính (LocalDB tạm, fixture, chạy ở thư mục riêng để không ghi model thử vào dự án)
- Cấu hình mặc định: **36/36 PASS**. Huấn luyện khởi tạo hoàn thành; mô hình khởi tạo đo được AI 52,9% so với Rule 63,2% trên phần giữ lại (tổng hợp) nên **bị chặn kích hoạt đúng chốt**; mô hình thường thiếu bản ghi thực vẫn bị chặn.
- Nới `AI:BootstrapMaxRuleGap=100`: **38/38 PASS**; kích hoạt thành công kèm cảnh báo, `ActiveModel.zip` được tạo, màn hình chẩn đoán hiện thẻ "Hybrid AI" và cảnh báo khởi tạo, không cần khởi động lại.

### Nhận định
- Mô hình khởi tạo chỉ học từ dữ liệu sinh ra từ chính từ điển, nên **không thể hơn bộ Rule** dựa trên từ điển đó (ở đây còn kém khoảng 10 điểm). Chỉ số của nó (đo trên dữ liệu tổng hợp) **không phản ánh độ chính xác trên ca thật**.
- Hệ thống vẫn dùng được ngay mà không cần AI kích hoạt: gợi ý chẩn đoán dựa trên Rule Engine; AI khởi tạo chỉ là lớp tham khảo bổ sung nếu owner cho phép nới chốt.
- Chưa chạy trên Somee/SQL Express; chỉ 38 kiểm tra trên database tạm; chưa kiểm tra đồng thời hay giao diện trực quan.

## 23. Cập nhật 05/11/2026 (Claude): chốt phương án A + phương án thứ ba cho AI khởi tạo

Quyết định của owner: **A + phương án thứ ba**. Cách thực hiện:
- **Mặc định (A):** chốt "AI không kém Rule quá 10 điểm" giữ nguyên. Mô hình khởi tạo kém Rule hơn 10 điểm (kiểm thử: AI 52,6% so với Rule 63,2% trên phần giữ lại tổng hợp) **không tự kích hoạt được**; hệ thống chạy bằng Rule Engine.
- **Khi chủ hệ thống chủ động nới chốt (phương án thứ ba):** đặt `AI:BootstrapMaxRuleGap` trong `appsettings.json` (mặc định 10; ví dụ 100). Mô hình khởi tạo được kích hoạt nhưng **chỉ hiển thị để tham khảo**: thẻ "Hybrid AI (Top 3)" kèm cảnh báo; **không cộng +100 điểm, không chèn gợi ý ngoài Rule, và không ảnh hưởng mã lỗi chẩn đoán lưu vào phiếu** (`MaLoi_CD` lấy từ gợi ý đứng đầu). Mô hình đã kiểm chứng (thường) vẫn giữ hành vi cũ.
- `appsettings.json` ghi sẵn các khóa `AI:AllowActivation`, `AI:AllowBootstrapModel`, `AI:BootstrapMaxRuleGap` (10), `AITraining:Enabled`; chuỗi kết nối không đổi.

Kiểm chứng (HTTP trên source chính, LocalDB tạm, fixture):
- Mặc định: **37/37 PASS**; mô hình khởi tạo bị chặn đúng chốt.
- Nới chốt (`BootstrapMaxRuleGap=100`): **40/40 PASS**; kích hoạt thành công; với 8 bộ triệu chứng thử, **gợi ý đứng đầu và mã lỗi lưu vào phiếu giống hệt khi chỉ dùng Rule**.
- **Kiểm chứng ngược:** khi tạm khôi phục hành vi cũ (AI được +100), kiểm tra này **fail** và gợi ý đứng đầu đổi ở **6/8 bộ triệu chứng** (ví dụ [1]: Rule 1 → AI 5; [3]: 1 → 4; [12]: 7 → 8; [10] và [18] không đổi) kéo theo mã lỗi lưu vào phiếu đổi theo. Đã hoàn nguyên và chạy lại cả hai cấu hình sau đó.

Hạn chế: chỉ số AI/Rule đo trên dữ liệu tổng hợp trong môi trường kiểm thử; tám bộ triệu chứng thử là mẫu nhỏ; chưa chạy trên Somee/SQL Express; chưa kiểm tra giao diện trực quan (chỉ kiểm tra có/không có chuỗi cảnh báo trong HTML).

## 24. Bộ gợi ý phân loại/ưu tiên yêu cầu (tiếng Việt) và rà soát giao diện

**Bộ gợi ý (`Services/YeuCauSuggester.cs`)**: luật từ khóa tiếng Việt cho `PhanLoaiYeuCau` và `MucDoUuTienDeXuat`; điều chỉnh trên `MoHinh-UuTien-TiengViet/dev.csv`. Chưa nối vào form tiếp nhận và chưa đo trên tập giữ riêng, nên chưa được dùng để khẳng định độ chính xác.

**Giao diện**: nguyên nhân gốc là màu gõ cứng của giao diện tối cũ nằm trên nền sáng. Đã chuyển sang biến màu theo theme (`--surface`, `--text`, `--c-ok/warn/err/info`...), thiết kế lại trang tổng quan (`dashboard-home.css`), và kiểm tra bằng script đo tương phản WCAG và chồng lấn. Các trang đã kiểm tra không còn chữ dưới 4,5:1 (nút "Ẩn gợi ý", huy hiệu "Kích hoạt/Tạm khóa", huy hiệu "mới" ở chuông thông báo đã sửa lần cuối). Chưa kiểm tra hết mọi trang ở mobile/tối.

**Lưu ý**: CSDL mới cần sao lưu và đặt `ALLOW_ALIGN_ERDV2=1` khi chạy migration `AlignErdV2`.

### Kết luận
Lỗi bảng màu/chồng lấn đã xử lý ở các trang đã kiểm tra; bộ gợi ý yêu cầu còn dở (chưa nối, chưa đo độc lập).

## 25. Nối bộ gợi ý yêu cầu vào form tiếp nhận và đo trên tập giữ riêng

- `PhieuTiepNhanController.Create`: lưu `MucDoUuTienDeXuat`; chỉ điền `PhanLoaiYeuCau` khi KTV để trống và gợi ý khác "Chưa rõ". Không ghi đè `MucDoUuTien` của KTV. Thêm `GET GoiYYeuCau` và dòng gợi ý trực tiếp ở `Create.cshtml` (ghi rõ nguồn là luật từ khóa).
- Đo một lần trên `MoHinh-UuTien-TiengViet/heldout.csv` (30 câu, diễn đạt mới, viết sau khi đã chốt luật; **do chính tác giả luật gán nhãn nên số liệu có xu hướng lạc quan**, không phải ca thật): phân loại đúng 24/30 (80,0%); ưu tiên đúng 17/30 (56,7%); cả hai đúng 16/30 (53,3%); ca Cao bị bỏ sót 3/8; ca Thấp bị đề xuất Cao 0.
- Lỗi chính: nghiêng về "Trung bình" (8/10 ca Thấp), và thiếu từ khóa (mùi khét, vào nước, tự tắt, máy in). Tập này đã dùng để đo nên **không chỉnh luật theo nó**; muốn cải thiện cần tập mới.
- Hồi quy HTTP sau thay đổi: 40/40 đạt (`KiemThu-HTTP/run_after_suggester.log`). Endpoint `GoiYYeuCau` và dòng gợi ý trên form chưa được kiểm bằng trình duyệt/harness.

### Kết luận
Bộ gợi ý đã nối vào hệ thống như công cụ tham khảo, an toàn (không ghi đè KTV, không đẩy ca nhẹ lên Cao). Độ chính xác ưu tiên mới ~57% trên tập tự soạn, chưa đủ để nói là hiệu quả.

## 26. Rà soát bộ gợi ý lần 2 và độ ổn định của kiểm thử

- Bổ sung luật tổng quát (vào nước, tự tắt, bộ nguồn, lướt web, rớt mạng, khởi động chậm, muốn thay...) và sửa lỗi khớp nhầm "rồi" (→ "roi") thành "rơi". Tập `heldout.csv` nay thành **tập dev** (đã chỉnh theo): phân loại 96,7%, ưu tiên 66,7%.
- Tập giữ riêng **mới** `heldout2.csv` (30 câu, đo một lần trước khi sửa lỗi "rồi"): phân loại 73,3%; ưu tiên 70,0%; cả hai 53,3%; ca Cao bị bỏ sót 7/11; ca Thấp bị gợi ý Cao 0. Chênh lệch 96,7% (dev) so với 73,3% (mới) cho thấy luật khớp quá mức với tập đã thấy. Điểm yếu chính là nhận Cao (cần thêm ngữ cảnh: rơi vỡ, đổ nước, mất dữ liệu, ảnh hưởng nhiều người, hạn chót).
- `TroLyChanDoanController`: thêm tiêu chí phụ theo mã lỗi khi hòa điểm Rule để thứ hạng xác định, không còn phụ thuộc thứ tự SQL.
- Kiểm thử HTTP: bản chạy thử từng báo sai vì thư mục chạy còn sót `ActiveModel.zip` của lần trước (đường cơ sở thực ra có AI). Đã sửa harness xóa thư mục mô hình trước mỗi lần chạy, thêm kiểm tra endpoint `GoiYYeuCau`. Kết quả: mặc định 38/38 (kích hoạt bị chặn đúng chốt), nới `RULE_GAP=100` 41/41.

### Kết luận
Bộ gợi ý vẫn là công cụ tham khảo, ưu tiên chỉ ~50–70% trên tập tự soạn. Thứ hạng Rule nay xác định, kiểm thử ổn định.

## 27. Thử nghiệm song ngữ (mã hóa đa ngôn ngữ + bộ phân loại nhỏ)

Môi trường: `MoHinh-SongNgu\venv` (torch 2.15 CPU, sentence-transformers 6.2, onnxruntime); mô hình `sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2` (~0,5 GB, tải theo đồng ý của chủ dự án). Tập đo: `heldout2.csv` (VI) và `heldout2_en.csv` (bản tiếng Anh cùng 30 câu), do tác giả soạn, chưa dùng để chỉnh.

| Thử nghiệm | VI | EN |
|---|---:|---:|
| Ưu tiên, huấn luyện trên aa_dataset EN/DE tổng hợp (4.000 mẫu) | acc 0,333 | 0,433 |
| Phân loại, gần-nhất theo danh mục triệu chứng/mã lỗi (không dữ liệu ngoài) | 0,500 | 0,533 |
| Phân loại, huấn luyện 90 câu VI | **0,833** | 0,833 |
| Ưu tiên, huấn luyện 90 câu VI | 0,633 | 0,700 |
| (đối chiếu) luật từ khóa: phân loại / ưu tiên | 0,733 | 0,700 |

Nhận xét: (1) Mô hình huấn luyện chỉ trên câu tiếng Việt vẫn cho kết quả tương đương trên bản tiếng Anh → chuyển đổi chéo ngôn ngữ hoạt động. (2) Dữ liệu ưu tiên tổng hợp EN/DE gần như không chuyển sang câu sửa máy tính (≈ ngẫu nhiên), khớp kết luận mục 8. (3) Chỉ 90 câu VI đã đạt phân loại 83% (luật 73%), nhưng chênh lệch trên 30 câu không có ý nghĩa thống kê (khoảng tin cậy ±14 điểm), nhãn và câu cùng một tác giả nên có thể lạc quan. (4) Chưa tích hợp vào web; bước tiếp theo là xuất ONNX và gọi bằng ONNX Runtime trong ASP.NET.

### Kết luận
Hướng song ngữ khả thi về kỹ thuật, nhưng cần nhiều câu tiếng Việt/Anh có nhãn thật hơn để khẳng định hiệu quả. Dữ liệu ngoài hiện có (tổng hợp) không giúp mức ưu tiên.

## 28. Tích hợp mô hình song ngữ ONNX vào website

**Đã làm.** Xuất ONNX (`MoHinh-SongNgu/xuat_onnx.py`): bộ mã hóa `paraphrase-multilingual-MiniLM-L12-v2` + mean pooling + hai đầu phân loại (phân loại yêu cầu, mức ưu tiên) huấn luyện trên 90 câu tiếng Việt (dev.csv + heldout.csv). Hai bản: fp32 (470 MB) và int8 (118 MB; mặc định, chọn theo kích thước, không theo kết quả đo). Tokenizer Unigram viết bằng C# (`Services/SongNguTokenizer.cs`) khớp id của Python **159/159** (gồm ký tự đặc biệt, toàn góc, emoji, chuỗi rỗng, 500 ký tự). Dịch vụ `Services/SongNguClassifier.cs` chạy bằng ONNX Runtime 1.22 trong website, thiếu tệp thì quay về luật từ khóa. Nối vào `PhieuTiepNhan/Create` (lưu `MucDoUuTienDeXuat`, điền phân loại khi trống) và endpoint `GoiYYeuCau`.

**Đối chiếu Python ↔ C#** (chỉ mô hình, fp32): heldout2 VI 83,3% / 63,3%; EN 83,3% / 70,0% — trùng số Python. int8 trong C#: VI 80,0% / 63,3%; EN 83,3% / 63,3%.

**Chính sách kết hợp với luật.** Phiên bản đầu (ghi trước khi đo): phân loại lấy mô hình khi xác suất ≥ 0,5; ưu tiên giữ Cao của luật, còn lại lấy mô hình khi ≥ 0,5. Kết quả int8 trên heldout2: phân loại 80,0%, ưu tiên 60,0% (kém luật đơn lẻ 70,0%). **Sau khi xem kết quả** tôi đổi sang: ưu tiên chỉ lấy mô hình khi luật không có tín hiệu rõ. Kết quả (đã chỉnh theo tập đo nên không còn độc lập): VI 80,0% / 66,7%; EN 76,7% / 53,3%.

**Kiểm thử HTTP:** 43/43 ở mặc định và khi nới `RULE_GAP=100` (thêm 2 kiểm tra gợi ý song ngữ cho câu tiếng Việt và tiếng Anh).

**Giới hạn.** Chỉ 30 câu đo (sai số ±14 điểm), nhãn và câu do cùng tác giả soạn; mô hình học từ 90 câu; mức Cao bị bỏ sót 6/11; chưa so với dữ liệu thật. Kết quả kết hợp (80% / 67%) không vượt rõ rệt luật (73% / 70%) hay mô hình đơn lẻ (80–83% / 63%).

### Kết luận
Mô hình song ngữ đã chạy được trong website bằng ONNX, hiểu cả câu tiếng Việt lẫn tiếng Anh và khớp Python. Phần phân loại tốt hơn luật một chút, phần ưu tiên chưa tốt hơn; đây vẫn chỉ là gợi ý tham khảo cho KTV.

## 29. Hoàn chỉnh mô hình chẩn đoán mã lỗi: mô tả tự do (Việt/Anh) → triệu chứng → Top-3 mã lỗi

**Kiến trúc.** (1) Rule đếm triệu chứng khớp trong TRIEU_CHUNG_LOI, hòa điểm xét theo mã lỗi; (2) AI khởi tạo (ML.NET) chỉ hiển thị, có chốt kích hoạt; (3) **mới**: ô "Mô tả sự cố bằng lời" ở `TroLyChanDoan/Index` gọi `GoiYTrieuChung`, mã hóa câu bằng mô hình đa ngôn ngữ ONNX, so cosine với từ điển TRIEU_CHUNG (tên + mô tả) và tự tích tối đa 4 triệu chứng (ngưỡng 0,45, đặt trước khi đo). KTV xem, bỏ/chọn lại rồi mới chẩn đoán; mã lỗi vẫn do Rule chấm điểm. Không dùng dữ liệu ngoài, không cần huấn luyện.

**Đo** (`tap_mo_ta_chan_doan.csv`: 40 câu VI + bản EN tương ứng, phủ 39/41 mã lỗi, do tác giả soạn, chưa dùng để chỉnh; từ điển sau patch 55 triệu chứng/41 lỗi/111 liên kết). Trần nếu chọn đúng triệu chứng: Top-1 38/40, Top-3 40/40.

| Bản chạy (C#) | Recall triệu chứng | Precision | Mã lỗi Top-1 | Mã lỗi Top-3 |
|---|---:|---:|---:|---:|
| int8 (mặc định) VI | 0,81 | 0,50 | 28/40 | 35/40 (87,5%) |
| int8 EN | 0,82 | 0,62 | 29/40 | 37/40 (92,5%) |
| fp32 VI | 0,82 | 0,50 | 27/40 | 36/40 (90,0%) |
| fp32 EN | 0,82 | 0,58 | 28/40 | 37/40 (92,5%) |

C# fp32 trùng Python (Top-3 36/40 và 37/40). Độ nhạy theo ngưỡng (Python, không dùng để chỉnh): 0,35 → Top-3 90,0/90,0%; 0,55 → 92,5/90,0% (VI/EN). Kết quả ổn định theo ngưỡng.

**Giới hạn.** 40 câu tự soạn, sai số lớn (±10 điểm); câu viết sát cách gọi triệu chứng nên có thể lạc quan hơn câu thật; precision triệu chứng thấp (~0,5) vì hệ thống tích dư, KTV cần bỏ bớt; từ điển sau patch **chưa áp vào database** (cần sao lưu và duyệt), nên trên database hiện tại chỉ khớp được với 44 triệu chứng cũ; chưa kiểm thử với ca thật; giao diện ô mô tả chưa xem bằng mắt trong trình duyệt (HTTP đã xác nhận ô tồn tại, endpoint trả đúng chủ đề cho câu Việt và Anh). Kiểm thử HTTP: 46/46 ở mặc định và `RULE_GAP=100`.

### Kết luận
Mô hình chẩn đoán mã lỗi nay đủ chuỗi: gõ mô tả tiếng Việt hoặc Anh → gợi ý triệu chứng → Rule ra Top-3 mã lỗi (khoảng 88–93% đúng trong Top-3 trên tập tự soạn). Phần AI học máy từ ca thật vẫn chưa có nhãn nên chỉ ở mức khởi tạo. Điều kiện còn lại: áp patch từ điển vào database sau khi sao lưu.

## 30. Áp patch từ điển chẩn đoán vào database phát triển (đã sao lưu)

- Database: `KILLER\SQLEXPRESS` / `QuanLySuCoMayTinh` (cấu hình Development). Không đụng database host ngoài trong `appsettings.json`.
- Sao lưu trước khi áp: `C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\QuanLySuCoMayTinh_truoc_patch_tudien_20261105.bak` (COPY_ONLY, CHECKSUM, đã `RESTORE VERIFYONLY` hợp lệ).
- Áp `Database/Patch_TuDien_DoPhu_20261105_AlignErdV2.sql` (một transaction, NOT EXISTS). Trước → sau: triệu chứng 44 → 55; mã lỗi 36 → 40; liên kết 51 → 109; giải pháp 36 → 40. Mọi mã lỗi đều có triệu chứng và giải pháp.
- Chênh so với kỳ vọng (55/41/111): database thiếu sẵn mã lỗi "Driver máy in bị lỗi hoặc cổng in bị tắc" (có trong file seed nhưng không có trong database), nên 2 liên kết tới lỗi này không được tạo và triệu chứng "Máy in không phản hồi" chưa có liên kết. Chưa tự thêm lỗi này.
- Khôi phục nếu cần: `RESTORE DATABASE ... FROM DISK` với file `.bak` trên.

### Kết luận
Từ điển chẩn đoán trên database phát triển đã đủ 55 triệu chứng; còn 1 triệu chứng và 1 mã lỗi (máy in) cần quyết định bổ sung.

**Bổ sung (sau mục 30).** Theo đồng ý của chủ dự án, thêm mã lỗi "Driver máy in bị lỗi hoặc cổng in bị tắc" + giải pháp (nội dung lấy từ file seed) + 2 liên kết (triệu chứng "Máy in không phản hồi" và "Driver thiết bị bị lỗi") bằng `Database/Patch_MayIn_20261105_AlignErdV2.sql` (một transaction, chạy lặp an toàn; nằm trong phạm vi bản sao lưu đã tạo trước đó). Kết quả database phát triển: triệu chứng 55, mã lỗi 41, liên kết 111, giải pháp 41, không còn triệu chứng chưa liên kết — khớp từ điển sau patch dùng để đo ở mục 29. Chưa chạy website trên database này để kiểm tra lại ô mô tả tự do (tránh các bước khởi tạo dữ liệu mẫu của `Program.cs` ghi vào database thật).

## 31. Cải thiện mô hình phân loại/ưu tiên yêu cầu và kho mô hình riêng

**Dữ liệu.** Soạn thêm `train_bosung.csv` (≈100 câu Việt/Anh/không dấu, có nhãn) → tập huấn luyện đầu phân loại: 249 câu (dev, heldout, heldout2, heldout2_en, train_bosung). Tập giữ riêng mới `heldout3.csv` (40 câu, văn phong đời thường hơn, Việt/Anh) **chỉ đo, không huấn luyện**; siêu tham số giữ nguyên (C=10).

**Kết quả trên heldout3** (một lần đo cho từng cấu hình; do tác giả soạn):

| Cấu hình | Phân loại | Ưu tiên | Ca Cao bắt được |
|---|---:|---:|---:|
| Luật từ khóa | 57,5% | 42,5% | 1/15 |
| Đầu phân loại cũ (90 câu), fp32 | 77,5% | 37,5% | 3/15 |
| Đầu mới (249 câu), fp32 | 80,0% | 55,0% | 6/15 |
| Đầu mới, int8 (bản đang dùng), chỉ mô hình | 77,5% | 47,5% | 5/15 |
| Đầu mới int8 + luật (chính sách hiện tại) | 77,5% | 52,5% | 5/15 |

**Nhận xét trung thực.** Con số ưu tiên ~70% đo trước đó (mục 25–28) trên tập nhỏ hơn là lạc quan: luật được chỉnh theo các tập đó; trên tập mới luật chỉ đúng 42,5% và bỏ sót 14/15 ca Cao. Thêm dữ liệu giúp mô hình (ưu tiên 37,5% → 55%, Cao 3 → 6/15) nhưng ưu tiên vẫn là điểm yếu: chưa phát hiện được phần lớn ca khẩn. Lý do: mức Cao phụ thuộc ngữ cảnh (hạn chót, mất dữ liệu, nhiều người ảnh hưởng, nguy hiểm) mà 249 câu chưa đủ.

**Chính sách ghép** (`SongNguClassifier.KetHop`): phân loại lấy mô hình (đúng hơn luật trên cả heldout2 và heldout3; luật chỉ dự phòng); ưu tiên giữ Cao của luật, mô hình báo Cao (≥ 0,5) thì nâng lên Cao (an toàn), còn lại luật có tín hiệu rõ thì giữ luật, không có tín hiệu thì lấy mô hình khi ≥ 0,5. Lựa chọn phân loại dùng mô hình được quyết định sau khi xem kết quả hai tập, nên số 77,5% trên heldout3 không còn độc lập hoàn toàn.

**Kho mô hình riêng (trả lời đề nghị của chủ dự án).** Thêm `Services/ModelPaths.cs` và khóa cấu hình `AI:ModelRoot`: mọi mô hình (ML.NET `ActiveModel.zip`, `DiagnosticModel_*.zip`, ONNX song ngữ ở `SongNgu\`) được huấn luyện/lưu/nạp từ thư mục đó; không đặt thì dùng `Data\MLModels` như cũ. Đã tạo `D:\Code\Đồ án chuyên nghành\AI-Models\` (kèm README) chứa mô hình ONNX. Kiểm thử HTTP chạy hoàn toàn với `AI__ModelRoot` riêng: 43/43 (mặc định) và 46/46 (nới ngưỡng), thư mục dự án không phát sinh `ActiveModel.zip`. Chưa đặt `AI:ModelRoot` trong `appsettings` của bạn (cần bạn quyết định).

### Kết luận
Phân loại yêu cầu bằng mô hình song ngữ đạt khoảng 77–80% trên tập mới; mức ưu tiên vẫn yếu (~50%) và hay bỏ sót ca khẩn nên chỉ là gợi ý. Đã có kho mô hình riêng trên ổ D, đúng yêu cầu training trước rồi nhúng vào hệ thống.

## 32. Bổ sung dữ liệu công khai vào mô hình (tự tìm, tự tải, tự đo)

Theo ủy quyền của chủ dự án (tự tìm và tự nạp dữ liệu phù hợp, không cần duyệt từng bước), đã tìm qua API Hugging Face (12 từ khóa) và tải các bộ **nhỏ, không cần đăng nhập hoặc biểu mẫu**. Không điền biểu mẫu Open Repair Alliance (cần họ tên/email của chủ dự án) và không vượt rào truy cập (`windows_system_errors_2.7k` bị khóa).

| Bộ | Kích thước | Dùng làm gì | Kết quả đo trên heldout3 (chưa từng huấn luyện) | Quyết định |
|---|---:|---|---|---|
| `marianna13/superuser` (Super User) | 28.506 chủ đề → 13.038 tiêu đề có đúng 1 nhóm theo tag; dùng 2.672 (tối đa 800/nhóm) | Nhãn yếu cho đầu **phân loại** | Phân loại 80,0% → **72,5%** (trên nhãn yếu cùng nguồn 81,1%) | **Không dùng** (khác miền, nhãn tag nhiễu; quy tắc nhận trước: chỉ dùng nếu không làm giảm) |
| `tasksource/help-desk-tickets` (reporter_messages) | 353 tin nhắn phiếu thật có nhãn ưu tiên (Cao 193 / TB 152 / Thấp 8) | Đầu **mức ưu tiên** | Ưu tiên 55,0% → **62,5%**, Cao bắt được 6/15 → 6/15, Thấp báo Cao 1 → 0 | **Dùng** (thỏa quy tắc nhận trước) |
| `tasksource/it-support-tickets` | 2.229 | – | – | Trùng Zenodo, bỏ |

Mô hình mới (đầu phân loại: 249 câu tự soạn; đầu ưu tiên: 249 câu + 353 phiếu thật) đo trong C# (int8, kết hợp với luật, heldout3): phân loại **77,5%**; ưu tiên **65,0%** (chỉ mô hình: 67,5%); bắt Cao 6/15; Thấp báo Cao 0. Mô tả → triệu chứng → Top-3 mã lỗi không đổi (VI 35/40, EN 37/40). Kiểm thử HTTP 46/46 ở cả hai chế độ. Mô hình đã chép vào `D:\Code\Đồ án chuyên nghành\AI-Models\SongNgu`.

**Giới hạn.** Phiếu thật có nhãn ưu tiên thuộc miền phần mềm/ngân hàng (không phải sửa PC), nhãn Thấp chỉ 8; heldout3 là 40 câu tự soạn nên ±10 điểm; việc nhận hay loại từng bộ dựa trên đúng tập này (quy tắc đặt trước nhưng tập đo đã dùng lại), nên số liệu chỉ để định hướng. Tiêu chuẩn ≥ 300.000 bản ghi PC vẫn chưa đạt; không có dữ liệu sửa chữa PC thật có nhãn mã lỗi.

### Kết luận
Dữ liệu công khai đã được bổ sung thật vào mô hình ưu tiên (353 phiếu thật, ưu tiên 55% → 62,5%); Super User không giúp phân loại nên bị loại. Mô hình dùng dữ liệu hàng trăm mẫu, không phải hàng trăm nghìn, và vẫn là gợi ý cho KTV.

## 33. Dữ liệu sửa chữa thật: Open Repair Alliance

**Tải.** Chủ dự án cung cấp họ tên và email và đồng ý để tôi điền biểu mẫu tải của ORA (từ chối cookie không thiết yếu, không đăng ký nhận tin, ghi mục đích học thuật phi thương mại). Tệp `OpenRepairData_v0.3_aggregate_202607.zip` (21 MB) giải nén tại `Datasets Đồ án chuyên nghành\ORA-OpenRepair\` (EEE 416.282 dòng, non-EEE 203.591 dòng; CC BY-SA 4.0, cần ghi nguồn).

**Quy mô thật cho bài toán PC.** Laptop 16.955 + máy tính để bàn 1.846 dòng; còn **11.879 mô tả vấn đề có văn bản** sau lọc và khử trùng (GB 4.391, BE 1.800, DE 1.649, NL 1.504, FR 954...). Đa ngôn ngữ nên hợp với bộ mã hóa đa ngôn ngữ. Nhãn có sẵn chỉ là loại sản phẩm và kết quả sửa (Fixed / End of life / Repairable); **không có nhóm lỗi hay mã lỗi**.

**(1) Độ phủ từ điển trên mô tả thật** (mẫu 3.000 mô tả; không có đáp án đúng nên chỉ đo độ tương đồng): cosine tốt nhất ≥ 0,45 ở 73,7%, ≥ 0,35 ở 91,6%, ≥ 0,55 ở 43,9%. Xem tay 40 mẫu khớp ≥ 0,45: khoảng 20/40 (≈ 50%) là hợp lý; sai điển hình: "adapter defect" → driver, "hard drive faulty" → driver, "hinge fault" → độ phân giải, "power button" → cổng sạc. Mô tả không khớp thường là: quên mật khẩu, chuyển dữ liệu, cài lại/nâng cấp hệ điều hành, bôi trơn quạt, thay ổ cứng, nguồn cháy.

**(2) Thêm vào đầu phân loại** (6.195 mô tả khớp đúng 1 nhóm theo từ khóa đa ngôn ngữ, tối đa 600/nhóm; nhãn yếu): heldout3 phân loại 80,0% → 72,5% → **không dùng** (quy tắc nhận trước), giống kết quả Super User (mục 32).

**Nhận xét trung thực.** Trên dữ liệu thật, từ điển triệu chứng hiện tại phủ được phần lớn nội dung nhưng độ đúng của gợi ý chỉ khoảng một nửa — thấp hơn nhiều so với 87,5–92,5% đo trên 40 câu tự soạn. Câu tự soạn đã lạc quan. Từ điển thiếu: PSU/nút nguồn, thay/hỏng ổ cứng, cài lại hệ điều hành, khôi phục/chuyển dữ liệu, quên mật khẩu, bôi trơn/hỏng quạt. 11.879 mô tả là dữ liệu thật nhưng chưa đạt tiêu chuẩn 300.000 bản ghi; chưa có nhãn nên chưa dùng để huấn luyện có giám sát.

### Kết luận
Đã có dữ liệu sửa chữa PC thật (11.879 mô tả đa ngôn ngữ) và dùng nó để đo độ phủ từ điển: độ đúng thực tế của gợi ý chỉ khoảng 50%, thấp hơn số đo trên tập tự soạn. Dữ liệu này chưa có nhãn nên không cải thiện được mô hình có giám sát; giá trị lớn nhất hiện tại là chỉ ra các triệu chứng/lỗi còn thiếu trong từ điển.

## 34. Bổ sung từ điển theo khoảng trống phát hiện từ dữ liệu thật (ORA)

Theo đồng ý của chủ dự án, thêm vào từ điển **7 triệu chứng, 6 mã lỗi, 14 liên kết, 6 giải pháp** (patch `Database/Patch_TuDien_ORA_20261105_AlignErdV2.sql`, sinh bởi `MoHinh-SongNgu/tao_patch_ora.py`): quên mật khẩu Windows; cần sao lưu/khôi phục/chuyển dữ liệu; cần cài lại hoặc nâng cấp hệ điều hành; nút nguồn không phản ứng (nút nguồn hỏng); ổ cứng/SSD hỏng cần thay; bản lề/vỏ máy gãy nứt; quạt kẹt cần bôi trơn hoặc thay. Nội dung dựa trên hiểu biết sửa chữa PC, **chưa kiểm chứng bằng ca thật**; giải pháp là đề xuất cần KTV duyệt.

Đã sao lưu lại database trước khi áp (`QuanLySuCoMayTinh_truoc_patch_ORA_20261105.bak`, `RESTORE VERIFYONLY` hợp lệ). Database phát triển: triệu chứng 55 → **62**, mã lỗi 41 → **47**, liên kết 111 → **125**, giải pháp 41 → **47**; không còn triệu chứng/lỗi mồ côi hay lỗi thiếu giải pháp.

**Đo lại.** (a) Bộ 40 câu cũ (từ điển lớn hơn): Top-3 tiếng Việt 35/40, tiếng Anh 38/40 (trước đó 35/40 và 37/40) → không hồi quy. (b) 14 câu mới viết cho các lỗi vừa thêm (tự soạn, `tap_mo_ta_moi.csv`): Top-3 12/14 ở cả hai ngôn ngữ (Top-1 8/14). (c) Độ phủ trên 3.000 mô tả ORA thật: cosine ≥ 0,45 từ 73,7% lên **79,4%**; ≥ 0,35 từ 91,6% lên 93,8%. Xem tay 24 mẫu khớp vào triệu chứng mới: khoảng một nửa hợp lý (ví dụ ổ cứng thay SSD, Linux cài mới, quên mật khẩu Windows khớp đúng; "fan broken" bị khớp vào ổ cứng, "hinge" khớp vào quạt/ổ là sai).

**Giới hạn.** Độ phủ tăng không đồng nghĩa độ đúng tăng; không có nhãn đáp án trong ORA nên chưa đo được độ đúng thật. Bộ khớp ngữ nghĩa còn nhầm giữa các triệu chứng phần cứng; cần thêm mẫu thật có nhãn hoặc bước xác nhận của KTV (đã có).

### Kết luận
Từ điển nay có 62 triệu chứng, 47 mã lỗi, 125 liên kết, 47 giải pháp, phủ thêm các tình huống phổ biến trong dữ liệu sửa chữa thật. Độ phủ mô tả thật tăng 73,7% → 79,4%; độ đúng thực tế vẫn chỉ ở mức một nửa khi xem tay.

## 35. Mô hình học máy thật cho nhóm lỗi (không dùng từ điển/luật)

**Yêu cầu của chủ dự án:** "AI" phải giải quyết bài toán học máy — học từ dữ liệu, có train/val/test — không phải nạp tri thức vào từ điển rồi đếm luật như đồ án cơ sở. Phần chẩn đoán mã lỗi hiện tại (Rule trên từ điển) **không** đáp ứng tiêu chí này; chỉ bộ mã hóa đa ngôn ngữ và các đầu phân loại là học máy.

**Thử nghiệm.** `MoHinh-SongNgu/hoc_may_nhom_loi.py`: 5.392 tiêu đề câu hỏi thật (Super User) → 19 nhóm lỗi chi tiết (ổ cứng, SSD, RAM, bàn phím, màn hình, USB, âm thanh, CPU, nguồn/pin, wifi, mạng, router, DNS, VPN, khởi động, Excel, Word, Outlook, trình duyệt); nhãn = tag do cộng đồng gán (nhãn người, **chưa xác minh**; chỉ giữ chủ đề có đúng một nhóm). Chia train/val/test phân tầng 70/10/20 (seed 42); chọn C trên tập val; đặc trưng = vector câu đa ngôn ngữ; bộ phân loại = hồi quy logistic đa lớp.

| Phép đo | Kết quả |
|---|---|
| Test trong miền (1.079 mẫu, 19 nhóm) | Accuracy **78,7%**, Macro-F1 **77,9%**, Top-3 **91,8%** (đoán theo lớp đa số 6,5%, ngẫu nhiên 5,3%) |
| Xuyên ngôn ngữ: câu tiếng Việt/Anh của nhóm (heldout3, 40 câu), gộp 19 nhóm về 5 nhóm hệ thống | **67,5%** (Việt 66,7%, Anh 70,0%) — mô hình **chưa từng thấy câu tiếng Việt** |

**Nhận xét.** Đây là học máy thật trên dữ liệu thật với đánh giá chuẩn. Xuyên ngôn ngữ thấp hơn đầu phân loại huấn luyện bằng câu tiếng Việt (77,5%) vì khác miền (câu hỏi kỹ thuật tiếng Anh với câu khách hàng) và nhãn tag nhiễu; kết quả 40 câu tự soạn, sai số lớn. Nhãn chỉ là *nhóm lỗi*, chưa phải *mã lỗi* trong hệ thống; để dự đoán thẳng `MaLoi` bằng học máy cần ca sửa chữa có nhãn mã lỗi.

### Kết luận
Đã có một mô hình học máy thật cho bài toán "mô tả sự cố → nhóm lỗi chi tiết" (78,7% trong miền, Top-3 91,8%). Từ điển/Rule chỉ nên là danh mục tra cứu giải pháp, không phải bộ ra quyết định.

## 36. Nối lõi học máy vào chẩn đoán

Theo đồng ý của chủ dự án, đầu **nhóm lỗi** (19 nhóm, học máy trên Super User, mục 35) được đưa vào ONNX (đầu ra thứ ba, huấn luyện trên train+val 80%, test 20% giữ nguyên: acc 78,7%) và nối vào hệ thống:
- `Services/ChanDoanAIService.cs`: dự đoán Top-3 nhóm lỗi từ mô tả tự do; xếp hạng mã lỗi = số triệu chứng khớp + 2 × độ tương đồng cosine giữa phân bố nhóm lỗi của mô tả và của tên mã lỗi (cùng bộ phân loại; trọng số 2 đặt trước khi đo). Không có mô tả thì giữ nguyên Rule.
- `TroLyChanDoan/XuLyChanDoan` nhận thêm `moTaTuDo`; trang kết quả hiện nhóm lỗi AI dự đoán và huy hiệu "AI nhóm lỗi" trên từng gợi ý; ô mô tả ở trang chẩn đoán cũng hiện Top-3 nhóm lỗi.
- Mô hình đặt ở kho `AI-Models\SongNgu` (cấu hình `AI:ModelRoot`).

**Đo trong C# (bản int8, từ điển 62/47/125) — Top-3 mã lỗi:**

| Bộ | Rule trên triệu chứng AI gợi ý | Rule + nhóm lỗi học máy (lõi mới) |
|---|---:|---:|
| 40 câu, tiếng Việt | 34/40 | **38/40** |
| 40 câu, tiếng Anh | **38/40** | 35/40 |
| 14 câu mới, tiếng Việt | **13/14** | 12/14 |
| 14 câu mới, tiếng Anh | **12/14** | 11/14 |
| Tổng 108 phép đo | 97 | 96 |

**Nhận xét trung thực.** Thêm tín hiệu nhóm lỗi học máy **không cải thiện độ đúng tổng thể** (96 so với 97/108): tốt hơn ở tiếng Việt 40 câu (+4), kém hơn ở các bộ còn lại. Mô hình nhóm lỗi chỉ phân biệt được các nhóm thô và chuyển sang tiếng Việt còn yếu (67,5%). Lõi học máy giúp hệ thống *không hoàn toàn phụ thuộc vào luật triệu chứng* nhưng chưa chứng minh là chính xác hơn. Mô hình dự đoán nhóm lỗi, không dự đoán trực tiếp mã lỗi (thiếu nhãn ca sửa chữa thật). Kiểm thử HTTP 49/49 ở cả hai chế độ (thêm 3 kiểm tra: nhóm lỗi tiếng Anh/Việt và trang kết quả).

### Kết luận
Lõi học máy đã nối vào chẩn đoán và chạy được, nhưng kết quả ngang với Rule trên tập tự soạn (96/108 so với 97/108). Giá trị chính là có thành phần học từ dữ liệu thật thay vì chỉ luật; muốn vượt luật cần dữ liệu ca sửa chữa có nhãn mã lỗi.

## 37. Mở rộng dữ liệu và cải thiện bộ phân loại nhóm lỗi (thí nghiệm)

**Dữ liệu mới.** Tìm qua API Hugging Face (tìm Stack Exchange khác, dữ liệu tiếng Việt): tải `mteb/AskUbuntuDupQuestions-VN` (CC BY-SA 4.0, 0,2 MB): 4.869 câu hỏi hỗ trợ kỹ thuật **tiếng Việt** (dịch máy từ AskUbuntu, chủ yếu Ubuntu/Linux, không nhãn). Không tải được thêm nhãn tốt hơn: bản Stack Exchange khác trên Hugging Face rất nặng (hàng GB) và không kèm tag; Open Repair Alliance (11.879 mô tả sửa PC thật) không có nhãn nhóm lỗi.

**Thí nghiệm** (`hoc_may_v2.py`, `nhom_loi_v2.py`; cùng cách chia train/val/test 70/10/20; quy tắc nhận trước: không giảm quá 2 điểm Macro-F1 trong miền và xuyên ngôn ngữ ≥ v1):

| Cấu hình | Số nhóm | Trong miền (test 20%) | Xuyên ngôn ngữ heldout3 | Top-3 mã lỗi (Rule + nhóm lỗi, 108 phép đo) |
|---|---:|---|---:|---:|
| v1: tag đơn nhóm | 19 | acc 80,1%, Macro-F1 78,6% | 70,0% | 97/108 (Python) |
| v2: thêm 11 nhóm (card đồ họa, mainboard, chuột/touchpad, driver, sao lưu, chạy chậm, mật khẩu, Explorer, máy in, virus, cài đặt) + gán nhóm hiếm nhất, 9.370 mẫu | 30 | acc 68,1%, Macro-F1 68,1% | 67,5% | – |
| v2 + tự huấn luyện (1.399 nhãn giả từ 4.000 mô tả ORA thật + 3.000 câu hỏi tiếng Việt, ngưỡng 0,6) | 30 | acc 67,9%, Macro-F1 67,8% | 70,0% | 100/108 (Python) |

**Quyết định: giữ v1 (19 nhóm).** v2 + tự huấn luyện hơn v1 ba phép đo trong Python (fp32), nhưng sau khi xuất ONNX int8 và chạy trong C# chỉ còn 97/108 (ngang Rule và v1), độ chính xác trong miền thấp hơn 12 điểm vì nhiều nhóm gần nhau hơn, và kiểm thử HTTP phát hiện lỗi cụ thể: câu "ổ cứng kêu lạch cạch và máy không nhận ổ" bị dự đoán thành nhóm *nguồn/pin*. Lợi ích duy nhất của v2 là chi tiết hơn; chưa đủ để đổi. Mã v2 vẫn giữ trong hồ sơ (`NHOM_LOI=v2` khi xuất ONNX). Mô hình đang chạy là v1, đã đo: test 20% acc 78,7%, Macro-F1 77,9%; kiểm thử HTTP 49/49 ở cả hai chế độ.

**Nhận xét trung thực.** Thêm dữ liệu không nhãn (mô tả sửa chữa thật, câu hỏi tiếng Việt) bằng tự huấn luyện không cải thiện rõ độ đúng; không đủ nhãn thật để làm tốt hơn. Cách duy nhất vượt hẳn là có ca sửa chữa có nhãn mã lỗi. Mô hình còn nhầm trong tiếng Việt do chữ "ổ" (ổ cứng/ổ cắm).

### Kết luận
Đã nạp thêm dữ liệu tiếng Việt và thử mở rộng sang 30 nhóm kèm tự huấn luyện; kết quả không vượt rõ ràng bản 19 nhóm nên giữ nguyên mô hình đang chạy (acc 78,7% trong miền, hệ thống 49/49 kiểm thử). Cải thiện thật sự cần dữ liệu có nhãn mã lỗi.

## 38. Dữ liệu ưu tiên thật bổ sung (Bugzilla, Jira) và kiểm tra đồng bộ ERD – database

**Ưu tiên.** Thử thêm vào đầu mức ưu tiên (cơ sở: 249 câu + 353 phiếu thật; heldout3: acc 62,5%, Cao 6/15, AUC Cao 0,893): (B) Bugzilla Eclipse, mức nghiêm trọng thật của 88.682 báo lỗi (dùng 4.500 mẫu cân bằng, blocker/critical → Cao, major/normal → Trung bình, minor/trivial/enhancement → Thấp): acc **57,5%** (trọng số 0,3) / **50,0%** (trọng số 1), Cao bắt 9/15; (J) Jira "khẩn nhất so với còn lại" (4.000 mẫu): AUC Cao chỉ 0,637, bắt Cao 9/15 nhưng báo nhầm 9/25. Quy tắc nhận trước (acc ≥ cơ sở, Cao ≥ cơ sở, Thấp báo Cao ≤ 1): **không nhận** — dữ liệu phát triển phần mềm khác miền sửa máy tính, làm giảm độ chính xác tổng thể dù bắt được nhiều ca Cao hơn. Giữ mô hình đang chạy.

**Đồng bộ ERD – database (dev DB `QuanLySuCoMayTinh`).** Có 21 bảng = **20 bảng ERD V2** (đủ 17 bảng thực thể + TRIEU_CHUNG_LOI, SU_DUNG_LINH_KIEN, LICH_SU_CHAN_DOAN_TRIEU_CHUNG) **+ `__EFMigrationsHistory`** là bảng kỹ thuật của Entity Framework ghi các migration đã chạy, không thuộc ERD. Mọi thay đổi về AI và từ điển trong phiên này chỉ **thêm dữ liệu (hàng)**, không thêm bảng hay cột; nhóm lỗi AI dự đoán không được lưu vào database. Nếu cần database đúng 20 bảng tuyệt đối, phải triển khai bằng script SQL (không dùng migration) và bỏ `__EFMigrationsHistory`.

### Kết luận
Dữ liệu ưu tiên thật bổ sung không giúp; mô hình giữ nguyên. Database nghiệp vụ khớp ERD 20 bảng (bảng thứ 21 chỉ là bảng kỹ thuật của EF).

## 39. Mô hình nhóm lỗi trên dữ liệu lớn, mô hình kết quả sửa, rà soát dữ liệu

- Super User đầy đủ (435.463 bản ghi có tiêu đề + tag; `flax-sentence-embeddings/stackexchange_title_body_jsonl`, 133 MB) → 128.190 chủ đề có đúng một nhóm lỗi sau khử trùng. Huấn luyện đầu nhóm lỗi trên 37.230 mẫu (tối đa 2.500/nhóm): test chung 8.849 mẫu acc 81,9% (mô hình nhỏ cũ 79,6%), Macro-F1 81,9% (79,5%), Top-3 93,5% (92,2%); xuyên ngôn ngữ heldout3 72,5% (67,5%). Top-3 mã lỗi (Rule + nhóm lỗi, 108 phép đo): 103/108 trong C# (trước 96–97). Đã thay đầu nhóm lỗi trong ONNX; HTTP 49/49 ở cả hai chế độ.
- Mô hình 3 (kết quả sửa; ORA 11.700 mô tả, chia theo nhóm quán sửa 70/10/20): acc 59,8%, Macro-F1 0,551 (TF-IDF ký tự 63,1%/0,576; đa số 56,1%/0,240). Hệ số lưu ở `AI-Models\KetQuaSua`; chưa tích hợp.
- Bảng rà soát đầy đủ: `HoSo_AI_TrinhBay\RA_SOAT_DU_LIEU.md`: bản ghi thật liên quan PC ≈ 284.875–306.536 (95–102% của 300.000; 57–61% của 500.000); dữ liệu có giám sát ≈ 140.000; nhãn mã lỗi xác minh: 0.

### Kết luận
Thêm dữ liệu thật có nhãn (dù là tag) cải thiện rõ: 37k mẫu cho nhóm lỗi tốt hơn 5k mẫu. Về số lượng thô dữ liệu gần đạt 300.000; thiếu hụt thật là dữ liệu có giám sát (~47%) và nhãn mã lỗi xác minh (0).

## 40. Bổ sung dữ liệu: AskUbuntu và phần còn lại của Super User

Tải `askubuntu.com.jsonl.gz` (flax-sentence-embeddings, 108 MB): 347.925 bản ghi có tag, 61.833 có đúng một nhóm lỗi. Tổng dữ liệu thật đã tải: Super User 435.463 + AskUbuntu 347.925 + ORA PC 18.801 = 802.189 bản ghi (liên quan PC theo tag: 346.708 = 116% của 300.000). Thử nâng đầu nhóm lỗi từ 37.230 lên 77.636 (thêm Super User) và 104.074 mẫu (thêm AskUbuntu): acc trong miền 81,9% → 82,3% → 82,0%; heldout3 72,5% → 75,0% → 70,0%; Top-3 mã lỗi 101 → 100 → 100/108 (Python). **Hiệu quả bão hòa**; giữ cấu hình 37.230 mẫu. Chi tiết: `HoSo_AI_TrinhBay\RA_SOAT_DU_LIEU.md` mục 4.

### Kết luận
Về số lượng, dữ liệu thật đã vượt 300.000 bản ghi liên quan PC (116%) và đủ chỉ tiêu 500.000 nếu tính toàn bộ; nhưng thêm dữ liệu cùng loại không còn cải thiện mô hình. Còn thiếu nhãn mã lỗi xác minh.

## 41. Mô hình dự đoán trực tiếp mã lỗi (giám sát yếu) và mô hình kết quả sửa v2

**Yêu cầu của chủ dự án:** AI phải dự đoán được trực tiếp mã lỗi. Dữ liệu công khai không có nhãn mã lỗi, nên đã gán **nhãn yếu**: biểu thức chính quy đặc trưng cho từng mã lỗi (mã dừng BSOD, `bootmgr`, `winsock`, `ransomware`, `thermal paste`, `bad sector`…; `dau_hieu_ma_loi.py`) quét 783.388 bài thật (Super User + AskUbuntu, tiêu đề + thân bài) → 68.382 bài có đúng một mã lỗi. Bộ phân loại văn bản → mã lỗi (vector câu đa ngôn ngữ + hồi quy logistic, 36.896 mẫu, tối đa 1.500/mã lỗi, 47 mã lỗi) **dự đoán thẳng mã lỗi** từ mô tả, không dùng từ điển hay đếm triệu chứng khi dự đoán.

| Phép đo | Kết quả |
|---|---|
| Trong miền (nhãn yếu, 7.380 mẫu test, 47 mã lỗi) | acc 60,1%, Macro-F1 0,490, Top-3 78,2% |
| Độc lập: 108 phép đo (54 câu × Việt/Anh, đáp án mã lỗi do nhóm gán) | Top-1 **65/108 (60,2%)**, Top-3 **97/108 (89,8%)** |
| Chỉ học 39 mã lỗi có ≥ 80 mẫu (bỏ 8 mã lỗi hiếm) | Top-1 55,6%, Top-3 73,1% |

(Tham chiếu: lõi lai triệu chứng + nhóm lỗi đạt Top-3 103/108.) **Giới hạn:** nhãn huấn luyện là luật suy ra từ văn bản (chưa người xác minh); văn bản chứa chính dấu hiệu dùng để gán nhãn nên mô hình có thể học đường tắt (ví dụ mã BSOD); 8 mã lỗi chỉ có 9–40 mẫu; 108 phép đo do nhóm soạn nên có thể lạc quan; chưa đo trên mô tả khách hàng thật. Hệ số lưu tại `AI-Models\MaLoiTrucTiep`, chưa tích hợp.

**Kết quả sửa v2** (thêm hãng, tuổi thiết bị, loại máy; chọn trên val): TF-IDF + vector câu + metadata → test acc 62,2%, Macro-F1 0,572 (v1: 59,8%/0,551; TF-IDF ký tự 63,1%/0,576).

### Kết luận
Hệ thống đã có mô hình học máy dự đoán trực tiếp mã lỗi (Top-3 89,8% trên 108 phép đo độc lập, Top-1 60,2%), học từ gần 37.000 bài thật nhờ nhãn yếu. Độ tin cậy bị giới hạn bởi nhãn yếu; muốn nâng cần nhãn mã lỗi do KTV xác minh.

## 42. Hoàn thiện mô hình 3 (kết quả sửa) và rà bài toán còn lại

- v2 chọn trên val: TF-IDF ký tự + vector câu + hãng/tuổi (acc 62,2%, Macro-F1 0,572 trên 2.599 mẫu test). Cấu hình này dùng TF-IDF ký tự nên chưa xuất sang C#.
- Bản xuất được (vector câu + hãng + tuổi) cho acc 59,7%, Macro-F1 0,551: **không hơn v1**. Lưu `AI-Models\KetQuaSua\ket_qua_sua_v2.json` (script `mh3_v2_xuat.py`); v1 vẫn là bản dùng.
- Bài toán "rào cản không sửa được" (ORA): chỉ 670 mẫu/6 lớp, quá ít, không huấn luyện.
- Chưa tích hợp mô hình nào mới vào website.

### Kết luận
Bốn mô hình đã có bản cuối cùng trong kho AI-Models; mô hình 3 và ưu tiên còn yếu, mã lỗi trực tiếp dùng nhãn yếu. Chờ quyết định bước tiếp theo.

## 43. Làm sạch nhãn yếu của mô hình mã lỗi trực tiếp

Đọc tay 60 bài ngẫu nhiên: nhãn có dấu hiệu chỉ trong thân bài đúng khoảng 35–45%; nhãn có dấu hiệu trong tiêu đề 9/9 (mẫu nhỏ). Thử (A) chỉ giữ 17.675 bài có dấu hiệu trong tiêu đề (46 mã lỗi, 5 mã ≤ 8 mẫu): trong miền acc 92,4% (bị thổi phồng vì tiêu đề chứa chính dấu hiệu) nhưng độc lập Top-1 63/108, Top-3 85/108; (B) che cụm dấu hiệu: trong miền 60,3%, độc lập 60/108 và 79/108. Quy tắc nhận trước (không thấp hơn 65/97): **không nhận**; giữ bản cũ (65/108, 97/108). Script `ra_nhan_yeu.py`, `hoc_ma_loi_sach.py`.

### Kết luận
Lọc nhãn sạch hơn làm mất dữ liệu cho mã lỗi hiếm nên kém hơn trên tập độc lập. Nhãn yếu không khắc phục được bằng lọc; cần nhãn do người xác minh.

## 44. Tích hợp mô hình mã lỗi trực tiếp vào chẩn đoán

Đo lai trước khi nối (108 phép, Python): điểm = số triệu chứng khớp + 2 × tương đồng nhóm lỗi + w × xác suất mô hình trực tiếp. w=0 (đang chạy): Top-1 68, Top-3 101; **w=1 (đặt trước)**: Top-1 **80**, Top-3 **103**; w=2: 79/101; chỉ mô hình trực tiếp: 64/97. Nhận w=1. Lưu ý: 108 phép này cũng là tập từng dùng để phát triển nên có thể lạc quan.
Đã thêm `Services/MaLoiTrucTiepClassifier.cs` (đọc `AI:ModelRoot\MaLoiTrucTiep\ma_loi_truc_tiep.json`, suy luận bằng vector câu), nối vào `ChanDoanAIService.XepHang` (chỉ khi KTV nhập mô tả), hiện thẻ "AI mã lỗi: x%" ở trang kết quả. Không thêm bảng/cột. Kiểm thử HTTP 50/50. KTV vẫn là người chốt mã lỗi.

### Kết luận
Mô hình mã lỗi trực tiếp đã nối vào chẩn đoán. Còn chưa nối: kết quả sửa (mô hình 3).

## 45. Tích hợp mô hình kết quả sửa

Thêm `Services/KetQuaSuaClassifier.cs` (đọc `AI:ModelRoot\KetQuaSua\ket_qua_sua.json`, v1: acc 59,8%, Macro-F1 0,551, ORA ~9.100 mẫu huấn luyện). Trang chi tiết phiếu hiện dòng "AI dự đoán khả năng sửa" (chỉ tham khảo, không lưu database, không thêm bảng/cột). HTTP 51/51. Giới hạn: huấn luyện trên mô tả tiếng Anh, chưa đo riêng với câu tiếng Việt; độ chính xác chỉ khoảng 60%.

### Kết luận
Cả ba mô hình học máy (nhóm lỗi, mã lỗi trực tiếp, kết quả sửa) đã nối vào hệ thống ở mức gợi ý. Nhóm các đầu phân loại hiện là học máy cổ điển trên vector câu của bộ mã hóa transformer; nếu muốn deep learning đầy đủ thì cần huấn luyện/tinh chỉnh mạng nơ-ron (xem đề xuất ở phản hồi).

## 46. Deep learning: MLP và tinh chỉnh bộ mã hóa; kiểm tra trên website thật

**MLP** (384-512-256-C, GELU, dropout 0,3, nhãn mềm; chọn epoch theo Macro-F1 trên val; cùng dữ liệu/cách chia với hồi quy logistic). Quy tắc nhận đặt trước:
| Bài toán | MLP | Hồi quy logistic | Kết luận |
|---|---|---|---|
| Kết quả sửa (2.599 test) | acc 64,8%, F1 0,574 | 59,8%, 0,551 | **Nhận** (đã nối vào hệ thống) |
| Nhóm lỗi | test 82,5%; heldout3 (Việt/Anh) 55,0% | 81,9%; 72,5% | Không nhận (kém xuyên ngôn ngữ) |
| Mã lỗi trực tiếp | trong miền 64,6%; độc lập Top-1 65/108, Top-3 85/108 | 60,1%; 65/108, 97/108 | Không nhận (Top-3 thấp hơn) |

**Tinh chỉnh toàn bộ bộ mã hóa** (MiniLM + đầu 47 mã lỗi, CPU, 15.586 mẫu, 2 epoch): trong miền acc 68,0%, F1 0,557, Top-3 81,9%; độc lập Top-1 68/108, Top-3 91/108. Quy tắc 1 (đứng riêng ≥ 65/97): không đạt Top-3. Quy tắc 2 (đặt trước, nhận vào bản lai nếu Top-1 ≥ 80 và Top-3 ≥ 103): bản lai w=1 cho 76/103 → **không nhận**. Giữ hồi quy logistic cho mã lỗi trực tiếp.

**Kết quả sửa theo ngôn ngữ** (ORA test): không phải tiếng Anh (1.899 mẫu) acc 64,5%, F1 0,558; tiếng Anh ước lượng (700) 65,6%, 0,589. Mô hình ổn định giữa các ngôn ngữ (bằng chứng gián tiếp cho tiếng Việt; chưa có nhãn tiếng Việt).

**Kiểm tra website trên database dev (đã sao lưu trước, `QuanLySuCoMayTinh_truoc_chay_web_20261106.bak`)**: đăng nhập tài khoản seed, tạo phiếu thử có mô tả tiếng Việt → phân loại "Nguồn & Pin", AI đề xuất ưu tiên Cao, "AI dự đoán khả năng sửa: 54%"; trang chẩn đoán gợi ý triệu chứng; kết quả hiện "AI nhóm lỗi 88%", "AI mã lỗi 56%", mã lỗi đứng đầu "Hệ thống tản nhiệt bị nghẹt bụi". Đã xóa phiếu/khách thử (còn 23 phiếu như cũ). Quan sát: gợi ý triệu chứng thêm "virus/malware" (49%) không phù hợp với mô tả; KTV bỏ tích được. HTTP 51/51. Database dev: 20 bảng nghiệp vụ + `__EFMigrationsHistory`, tên cột AI tiếng Việt khớp ERD.

### Kết luận
Deep learning thật được nhận ở bài kết quả sửa (MLP); các bài còn lại hồi quy logistic trên vector câu của transformer vẫn tốt hơn hoặc ngang MLP/tinh chỉnh theo quy tắc nhận trước. Hệ thống chạy được trên database dev.

## 47. Hoàn thiện các mục còn yếu (tự động, không người duyệt)

- **Bộ khớp triệu chứng**: chọn (ngưỡng 0,35, biên 0,10, tối đa 4) theo F1 trên tập 40 câu (dev 0,602→0,653), đo một lần trên tập 14 câu (0,426→0,529, recall giữ 82%). Nhưng khi đưa vào xếp hạng mã lỗi, bản lai tụt: Top-1 80→73, Top-3 103→96/108. **Không nhận**, giữ cấu hình cũ (0,45); đánh đổi: gợi ý triệu chứng thừa nhiều hơn nhưng xếp hạng mã lỗi tốt hơn.
- **Mức ưu tiên**: ngưỡng báo Cao 0,35 chọn bằng kiểm định chéo 5 lần trên dữ liệu huấn luyện (không dùng heldout3): mô hình riêng 62,5%→65,0%, bắt Cao 6→8/15, Thấp báo Cao 1. Lõi kết hợp luật + mô hình trên heldout3 (C#): ưu tiên **75,0%** (30/40), bắt Cao **11/15**, Thấp→Cao 1 (trước: 65,0%, 6/15). MLP cho ưu tiên 57,5%, không nhận. Nhận ngưỡng 0,35.
- **Mức tin cậy** (chọn tự động khi biên điểm top1−top2 lớn): trên 108 phép bản lai: biên ≥ 0,8 phủ 50% (54 phép), Top-1 đúng 90,7%, Top-3 100%; ≥ 0,4 phủ 80%, 76,7%; toàn bộ 74,1%. Trang kết quả hiện "Tin cậy AI: Cao / Trung bình / Thấp". Lưu ý: ngưỡng chọn trên chính tập 108 nên độ đúng có thể thấp hơn trên dữ liệu mới; chưa có tập đo thứ hai.
- HTTP 52/52, build 0 lỗi.

### Kết luận
Ưu tiên cải thiện rõ nhất (75,0%, bắt 11/15 ca Cao). Khớp triệu chứng chặt hơn làm xếp hạng mã lỗi kém nên không nhận. Mức tin cậy cho phép biết lúc nào AI đáng tin, nhưng độ đúng ở mức tin cậy cao (~91%) đo trên tập tự soạn, chưa kiểm chứng ngoài.

## 48. AI tự chốt mã lỗi khi tin cậy Cao

Theo yêu cầu của owner (06/11/2026): khi KTV nhập mô tả, AI xếp hạng mã lỗi và mức tin cậy là **Cao** (biên điểm top1 − top2 ≥ 0,8) và phiếu đang ở "Chờ khám"/"Đang kiểm tra" thì hệ thống **tự chốt** như khi bấm "Chốt & Sửa chữa": lưu mã lỗi, chuyển "Đang sửa chữa", ghi `LICH_SU_TRANG_THAI` với `LoaiSuKien = AI_TU_CHOT` và `DuLieuBoSungJSON` (mã lỗi, biên điểm, mức tin cậy) và `LICH_SU_CHAN_DOAN`. Trang chi tiết phiếu hiện dòng "AI đã tự chốt mã lỗi…". Tắt bằng cấu hình `AI:TuDongChot=false`. Không thêm bảng/cột (dùng cột có sẵn của ERD).
Các ca tin cậy Trung bình/Thấp vẫn hiện trang kết quả để chọn. Nguyên nhân thực tế vẫn chưa có người xác minh (nhãn huấn luyện không lấy từ ca tự chốt: `TrangThaiXacMinh` không đặt là Đã xác minh).
Kiểm thử HTTP 53/53 (cả RULE_GAP mặc định, nơi 3 kiểm tra ML.NET khởi tạo bị bỏ qua vì mô hình bị chặn đúng thiết kế, và RULE_GAP=100). Giới hạn: ngưỡng 0,8 chọn trên chính tập 108 phép; độ đúng Top-1 ~91% ở mức Cao đo trên tập tự soạn (54 phép) nên ngoài thực tế có thể thấp hơn.

### Kết luận
AI nay tự quyết định ở ca tin cậy Cao (khoảng nửa số ca trên tập đo) thay vì chỉ gợi ý. Chưa kiểm chứng trên ca thật; có nhật ký và công tắc tắt.

## 49. Siết điều kiện tự chốt, tập kiểm chứng mới, giám sát tự động

**Tập kiểm chứng mới** `tap_kiem_chung.csv` (40 câu Việt/Anh kiểu khách hàng, soạn sau, chưa dùng để chỉnh ngưỡng). Kết quả hệ thống tự động (khớp triệu chứng → xếp hạng lai): Top-1 **62,5%** (50/80), Top-3 **86,3%** (69/80) — thấp hơn tập phát triển 108 phép (74,1% / 95,4%): xác nhận số liệu cũ lạc quan.

**Điều kiện tự chốt** (chọn trên 108 phép cũ, đo một lần trên tập mới):
| Điều kiện | Dev 108: phủ / đúng | Kiểm chứng 80: phủ / đúng |
|---|---|---|
| A: biên ≥ 0,8 (bản cũ) | 50% / 90,7% | 42% / 85,3% (29/34) |
| B: A + mô hình mã lỗi trực tiếp đồng ý | 37% / 97,5% | 35% / 100% (28/28) |
| C: biên ≥ 1,5 | 28% / 93,3% | 18% / 100% (14/14) |
| **F: biên ≥ 1,0 + trực tiếp đồng ý (nhận)** | 34% / **100%** (37/37) | 32% / **100%** (26/26) |
Nhận F: tin cậy **Cao** = biên ≥ 1,0 và mô hình mã lỗi trực tiếp cùng mã lỗi đứng đầu; chỉ mức Cao mới tự chốt (khoảng 1/3 số ca); Trung bình (biên ≥ 0,4) và Thấp hiện kết quả cho KTV chọn. Cận dưới khoảng tin cậy 95% của 26/26 xấp xỉ 87%.

**Giám sát tự động** (không cần người chấm): `TroLyChanDoanController.TinhThongKeTuChot` đọc nhật ký `AI_TU_CHOT` (100 ca gần nhất); một ca tính là "sửa lại" nếu mã lỗi hiện tại của phiếu hoặc nguyên nhân thực tế đã xác minh khác mã AI chốt; từ 10 ca trở lên mà tỉ lệ > 30% thì **tự tạm dừng tự chốt**. Xem tại `GET /TroLyChanDoan/ThongKeTuChotAI` (Admin KTV). Nhánh tạm dừng đã biên dịch nhưng chưa được kích hoạt trong kiểm thử (cần ≥ 10 ca bị sửa).

HTTP 54/54 (cả RULE_GAP mặc định và 100); build 0 lỗi.

### Kết luận
Điều kiện tự chốt mới chính xác hơn rõ trên tập chưa dùng chỉnh (26/26) và có cơ chế tự tạm dừng, nhưng chỉ phủ khoảng 1/3 ca và vẫn đo trên câu tự soạn; Top-1 chung của hệ thống ngoài mẫu chỉ khoảng 62,5%.

## 50. Quyết định chế độ vận hành
Owner đồng ý (06/11/2026) giữ **AI gợi ý + tự chốt có điều kiện** (chỉ tin cậy Cao tự chốt, còn lại KTV chọn); không tự chốt mọi ca. Không thay đổi mã nguồn thêm.

### Kết luận
Chế độ vận hành đã chốt; các điểm cần nêu khi báo cáo là giới hạn nhãn yếu và tập đo tự soạn.

## 51. Kiểm chứng lớn trên bài thật, hiệu chỉnh mức tin cậy, sửa lỗi ghép mã lỗi

**Kiểm chứng lớn** (`kiem_chung_lon.py`): 1.779 bài Super User/AskUbuntu giữ riêng (phần test của mô hình mã lỗi), nhãn sạch (dấu hiệu nằm trong tiêu đề), loại bài có tiêu đề nằm trong tập huấn luyện đầu nhóm lỗi, **che cụm dấu hiệu** của nhãn khỏi đầu vào, chạy đúng đường ống hệ thống. Kết quả: Top-1 **37,2%** [35,0–39,5], Top-3 55,6% [53,3–57,9]; chỉ mô hình trực tiếp Top-1 48,7%, Top-3 71,6%. Mức Cao cũ (biên ≥ 1,0 và đồng ý) chỉ đúng **72,8%** [68,1–77,0] (phủ 23%) — thấp hơn nhiều so với 26/26 trên 80 câu tự soạn. Đây là bộ bi quan (che dấu hiệu, văn bản diễn đàn Linux); tập 80 câu tự soạn lạc quan; sự thật nằm giữa. Tập 80 câu: tiếng Việt Top-1 23/40, tiếng Anh 27/40.

**Hiệu chỉnh**: hồi quy logistic 8 đặc trưng (biên, đồng ý, xác suất trực tiếp, độ tương đồng nhóm, số triệu chứng khớp...) dự đoán xác suất đúng của mã lỗi đứng đầu, huấn luyện trên nửa A (889 bài), ngưỡng chọn trên A (đúng ≥ 85% với phủ lớn nhất → 0,75). Kiểm trên nửa B (890 bài khác): phủ 18%, đúng **82,8%** (luật cũ cùng bộ: 69,9%); trên 80 câu: phủ 30%, đúng 24/24. Quy tắc nhận trước (B ≥ 80%, 80 câu ≥ 90%, phủ B ≥ 10%) đạt → **nhận**. Cao ≥ 0,75 (tự chốt), Trung bình ≥ 0,5, Thấp còn lại. Hệ số lưu `AI-Models\MaLoiTrucTiep\tin_cay.json`.

**Lỗi nghiêm trọng đã sửa**: kiểm tra trên database dev (không phải harness) cho thấy hầu hết ca "Thấp" và nhiều mã lỗi sai, vì số mã lỗi trong database dev lệch với danh mục lúc huấn luyện (từ số 37 trở đi lệch 1; harness dùng database sạch nên không lộ). Mô hình mã lỗi trực tiếp trước đó ghép lớp theo **số** nên cộng điểm sai mã lỗi. Đã sửa ghép theo **tên mã lỗi** (`ten_loi` trong JSON; 47/47 tên khớp database dev). Sau sửa, 16 câu thử trên database dev: 4 ca tự chốt đều đúng (mật khẩu, webcam, adware, touchpad), 4 ca Trung bình đều đúng, các ca Thấp (8) sai nhiều (ví dụ máy in, tràn ổ C) và để KTV chọn. Top-1 chung 8/16 = 50%. Đã dọn dữ liệu thử (còn 23 phiếu). HTTP 54/54 ở cả hai chế độ.

### Kết luận
Bằng chứng mới cho thấy độ tin cậy trước đó bị đánh giá cao và có một lỗi ghép mã lỗi thật; sau sửa, tự chốt chỉ ở mức Cao với độ đúng ước lượng 83% (bộ bi quan) đến 100% (bộ tự soạn, n nhỏ), phủ 18–30% số ca. Top-1 chung vẫn chỉ khoảng 37–62%, nên các ca còn lại phải do KTV chọn.
