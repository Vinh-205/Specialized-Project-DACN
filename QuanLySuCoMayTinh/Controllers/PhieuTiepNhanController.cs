using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;
using System.Security.Claims;
using System;

namespace QuanLySuCoMayTinh.Controllers
{
    [Authorize]
    public class PhieuTiepNhanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.SongNguClassifier _songNgu;

        private readonly Services.KetQuaSuaClassifier _ketQuaSua;

        public PhieuTiepNhanController(ApplicationDbContext context, Services.SongNguClassifier songNgu, Services.KetQuaSuaClassifier ketQuaSua)
        {
            _ketQuaSua = ketQuaSua;
            _context = context;
            _songNgu = songNgu;
        }

        private Services.YeuCauGoiY GoiYYeuCauKetHop(string? moTa) =>
            Services.SongNguClassifier.KetHop(Services.YeuCauSuggester.Suggest(moTa), _songNgu.Predict(moTa));

        // GET: Danh sách phiếu tiếp nhận
        public async Task<IActionResult> Index(string searchString, string trangThai)
        {
            var query = _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .Include(p => p.User_KTV)
                .AsQueryable();

            // Lọc theo từ khóa (Tìm Mã phiếu, Tên thiết bị hoặc Tên khách hàng)
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.MaPhieu.ToString().Contains(searchString) ||
                                         (p.ThietBi != null && p.ThietBi.TenTB.Contains(searchString)) ||
                                         (p.ThietBi != null && p.ThietBi.KhachHang != null && p.ThietBi.KhachHang.HoTen.Contains(searchString)));
            }

            // Lọc theo trạng thái thả xuống
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả trạng thái")
            {
                query = query.Where(p => p.TrangThai == trangThai);
            }

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentFilter"] = string.IsNullOrEmpty(trangThai) ? "Tất cả trạng thái" : trangThai;

            var danhSachPhieu = await query.OrderByDescending(p => p.NgayNhan).ToListAsync();
            return View(danhSachPhieu);
        }

        // GET: Hiển thị form lập phiếu
        public IActionResult Create()
        {
            return View();
        }

        // GET: gợi ý phân loại/ưu tiên từ mô tả sự cố (luật từ khóa, chỉ để KTV tham khảo)
        [HttpGet]
        public IActionResult GoiYYeuCau(string? moTa)
        {
            if ((moTa?.Length ?? 0) > 4000) return BadRequest();
            var g = GoiYYeuCauKetHop(moTa);
            return Json(new { phanLoai = g.PhanLoai, mucDo = g.MucDoUuTien, lyDo = g.LyDo, nguon = g.Nguon });
        }

        // POST: Nhận dữ liệu từ form và lưu vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string tenKhachHang, string soDienThoai, string tenThietBi, string? cauHinh, string? ngoaiQuan, string submitAction, string? moTaSuCo = null, string? phanLoaiYeuCau = null, string? mucDoUuTien = null)
        {
            if (string.IsNullOrWhiteSpace(tenKhachHang) || string.IsNullOrWhiteSpace(soDienThoai) || string.IsNullOrWhiteSpace(tenThietBi))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ thông tin Khách hàng và Thiết bị.");
                return View();
            }

            if ((moTaSuCo?.Length ?? 0) > 4000 || (phanLoaiYeuCau?.Length ?? 0) > 100 || (mucDoUuTien?.Length ?? 0) > 30) {
                ModelState.AddModelError(string.Empty, "Thông tin sự cố vượt giới hạn cho phép."); return View();
            }

            // 1. Tìm hoặc tạo mới Khách Hàng theo Số điện thoại
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.SDT == soDienThoai);
            if (khachHang == null)
            {
                khachHang = new KhachHang { HoTen = tenKhachHang, SDT = soDienThoai };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
            }

            // 2. Tạo mới Thiết Bị cho Khách hàng này (bao gồm cấu hình + ngoại quan)
            var thietBi = new ThietBi
            {
                TenTB = tenThietBi,
                MaKH = khachHang.MaKH,
                CauHinh = string.IsNullOrWhiteSpace(cauHinh) ? null : cauHinh.Trim(),
                NgoaiQuan = string.IsNullOrWhiteSpace(ngoaiQuan) ? null : ngoaiQuan.Trim()
            };
            _context.ThietBis.Add(thietBi);
            await _context.SaveChangesAsync();

            // Gợi ý (luật từ khóa): chỉ lưu vào MucDoUuTienDeXuat; phân loại chỉ điền khi KTV để trống. Không ghi đè lựa chọn của KTV.
            var goiY = string.IsNullOrWhiteSpace(moTaSuCo) ? null : GoiYYeuCauKetHop(moTaSuCo);
            if (string.IsNullOrWhiteSpace(phanLoaiYeuCau) && goiY != null && goiY.PhanLoai != "Chưa rõ") phanLoaiYeuCau = goiY.PhanLoai;

            // 3. Lập Phiếu tiếp nhận - trạng thái "Chờ khám" (Bước 1)
            var phieu = new PhieuTiepNhan
            {
                MaTB = thietBi.MaTB,
                NgayNhan = DateTime.Now,
                TrangThai = "Chờ khám",
                MoTaSuCo = moTaSuCo,
                PhanLoaiYeuCau = phanLoaiYeuCau,
                MucDoUuTien = mucDoUuTien,
                MucDoUuTienDeXuat = goiY?.MucDoUuTien,
                MaUser_KTV = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            };

            _context.Add(phieu);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã lập phiếu tiếp nhận #{phieu.MaPhieu.ToString("D4")} thành công!";
            TempData["PlayTicketSound"] = true;

            if (submitAction == "diagnose")
            {
                return RedirectToAction("Index", "TroLyChanDoan", new { maPhieu = phieu.MaPhieu });
            }

            return RedirectToAction("Details", new { id = phieu.MaPhieu });
        }

        // GET: Xem chi tiết phiếu tiếp nhận
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .Include(p => p.User_KTV)
                .Include(p => p.MaLoiNavigation)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.SuDungLinhKiens)
                        .ThenInclude(s => s.LinhKien)
                .Include(p => p.HinhAnhSuCos)        // ← Ảnh chứng cứ sự cố
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            ViewBag.KhaNangSua = _ketQuaSua.DuDoan(phieu.MoTaSuCo);   // gợi ý học máy, không lưu vào database
            ViewBag.MaLoiXacMinh = await _context.MaLois.Where(x => x.Status).OrderBy(x => x.TenLoi).ToListAsync();
            ViewBag.LichSuChanDoan = await _context.LichSuChanDoans.Where(x => x.MaPhieu == id).OrderByDescending(x => x.ThoiGian).ToListAsync();
            ViewBag.KhoLinhKien = await _context.LinhKiens
                .Where(lk => lk.CoTrangThai && lk.SoLuongTon > 0)
                .OrderBy(lk => lk.TenLK)
                .ToListAsync();

            // ── Gợi ý linh kiện dựa trên chẩn đoán AI ──
            // Mapping MaLoiID → danh sách LoaiLK phù hợp
            var loiToLoai = new Dictionary<int, string[]>
            {
                // Phần cứng - GPU/Màn hình
                { 1, new[] { "Card đồ họa", "Cáp tín hiệu" } },
                { 2, new[] { "Cáp tín hiệu" } },
                { 4, new[] { "Màn hình laptop", "Màn hình", "Cáp tín hiệu" } },
                { 5, new[] { "Cáp tín hiệu", "Màn hình laptop" } },
                // Nguồn & Pin
                { 6, new[] { "Nguồn" } },
                { 7, new[] { "Pin laptop" } },
                { 8, new[] { "Linh kiện mainboard", "Pin laptop" } },
                { 9, new[] { "Cổng kết nối", "Sạc laptop" } },
                // Tản nhiệt
                { 10, new[] { "Tản nhiệt", "Vật tư" } },
                { 11, new[] { "Vật tư", "Tản nhiệt" } },
                // RAM & Ổ cứng
                { 12, new[] { "RAM" } },
                { 13, new[] { "Ổ cứng" } },
                { 14, new[] { "Ổ cứng" } },
                // Hệ điều hành
                { 15, new[] { "Ổ cứng", "Phần mềm & Công cụ" } },
                { 16, new[] { "Phần mềm & Công cụ", "Ổ cứng" } },
                { 17, new[] { "Phần mềm & Công cụ" } },
                { 18, new[] { "Phần mềm & Công cụ" } },
                { 19, new[] { "Phần mềm & Công cụ" } },
                // Phần mềm & Bảo mật
                { 20, new[] { "Phần mềm & Công cụ" } },
                { 21, new[] { "Phần mềm & Công cụ" } },
                { 22, new[] { "Phần mềm & Công cụ" } },
                // BSOD
                { 23, new[] { "RAM", "Ổ cứng", "Phần mềm & Công cụ" } },
                { 24, new[] { "Phần mềm & Công cụ", "Card mạng" } },
                { 25, new[] { "Phần mềm & Công cụ", "Ổ cứng" } },
                { 26, new[] { "RAM" } },
                { 27, new[] { "RAM", "Ổ cứng" } },
                // Mạng
                { 28, new[] { "Card mạng" } },
                { 29, new[] { "Card mạng" } },
                { 30, new[] { "Phần mềm & Công cụ", "Card mạng" } },
                { 31, new[] { "Phần mềm & Công cụ", "Card mạng" } },
                { 32, new[] { "Card mạng" } },
                // Bàn phím & Touchpad
                { 33, new[] { "Bàn phím" } },
                { 34, new[] { "Touchpad", "Phần mềm & Công cụ" } },
                // Âm thanh & USB
                { 35, new[] { "Card âm thanh", "Phần mềm & Công cụ" } },
                { 36, new[] { "Card mở rộng", "Cổng kết nối" } },
            };

            List<int> goiYIds = new();
            if (phieu.MaLoi_CD.HasValue && loiToLoai.TryGetValue(phieu.MaLoi_CD.Value, out var loais))
            {
                var allParts = (List<LinhKien>)ViewBag.KhoLinhKien;
                goiYIds = allParts
                    .Where(lk => loais.Contains(lk.LoaiLK))
                    .Select(lk => lk.MaLK)
                    .ToList();
            }
            ViewBag.GoiYLinhKienIds = goiYIds;

            // Load lịch sử chuyển trạng thái để hiển thị timeline
            ViewBag.LichSuTrangThai = await _context.LichSuTrangThais
                .Include(ls => ls.KTV)
                .Where(ls => ls.MaPhieu == id)
                .OrderByDescending(ls => ls.ThoiGian)
                .ToListAsync();

            // Lấy IP LAN của máy chủ để điện thoại quét QR có thể kết nối được
            string localIp = "localhost";
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(ip))
                    {
                        localIp = ip.ToString();
                        break;
                    }
                }
            }
            catch { }
            ViewBag.LocalIp = localIp;
            ViewBag.Port = Request.Host.Port;
            ViewBag.Scheme = Request.Scheme;

            return View(phieu);
        }

        // POST: PhieuTiepNhan/UpdateGhiChuNoiBo
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin KTV,Admin,KTV")]
        public async Task<IActionResult> UpdateGhiChuNoiBo(int id, string ghiChu)
        {
            var phieu = await _context.PhieuTiepNhans.FindAsync(id);
            if (phieu == null) return NotFound();

            phieu.GhiChuNoiBo = ghiChu;
            _context.Update(phieu);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật ghi chú nội bộ thành công.";
            return RedirectToAction(nameof(Details), new { id = phieu.MaPhieu });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Print(int? id, string type = "all")
        {
            if (id == null) return NotFound();

            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .Include(p => p.User_KTV)
                .Include(p => p.MaLoiNavigation)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.SuDungLinhKiens)
                        .ThenInclude(s => s.LinhKien)
                .Include(p => p.HinhAnhSuCos)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            string localIp = "localhost";
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(ip))
                    {
                        localIp = ip.ToString();
                        break;
                    }
                }
            }
            catch { }
            ViewBag.LocalIp = localIp;
            ViewBag.Port = Request.Host.Port;
            ViewBag.Scheme = Request.Scheme;

            ViewBag.PrintType = type;
            return View(phieu);
        }

        // GET: Hiển thị form chỉnh sửa phiếu (chặn nếu đã Hoàn thành)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.User_KTV)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);
                
            if (phieu == null) return NotFound();

            // Khóa cứng: Không cho sửa phiếu đã Hoàn thành (Bước 5 - Bảo toàn kế toán)
            if (phieu.TrangThai == "Hoàn thành")
            {
                TempData["ErrorMsg"] = "Đơn này đã được thanh toán và khóa cứng. Không được phép chỉnh sửa.";
                return RedirectToAction("Details", new { id });
            }

            var thietBiList = _context.ThietBis
                .Include(t => t.KhachHang)
                .Select(t => new { MaTB = t.MaTB, HienThi = t.KhachHang.HoTen + " - " + t.TenTB }).ToList();

            ViewData["MaTB"] = new SelectList(thietBiList, "MaTB", "HienThi", phieu.MaTB);
            return View(phieu);
        }

        // POST: Cập nhật thông tin phiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPhieu,MaTB,NgayNhan,TrangThai,MaUser_KTV")] PhieuTiepNhan phieu)
        {
            if (id != phieu.MaPhieu) return NotFound();

            // Bỏ qua kiểm tra các property liên kết (navigation properties)
            ModelState.Remove("ThietBi");
            ModelState.Remove("User_KTV");
            ModelState.Remove("MaLoiNavigation");
            ModelState.Remove("ChiTietSuaChuas");

            if (ModelState.IsValid)
            {
                var existingPhieu = await _context.PhieuTiepNhans.AsNoTracking().FirstOrDefaultAsync(p => p.MaPhieu == id);
                if (existingPhieu == null) return NotFound();

                // 1. Không cho sửa phiếu đã Hoàn thành
                if (existingPhieu.TrangThai == "Hoàn thành")
                {
                    TempData["ErrorMsg"] = "Phiếu đã hoàn thành, không thể chỉnh sửa.";
                    return RedirectToAction("Details", new { id });
                }

                // 2. Không được chọn Hoàn thành thủ công (phải qua form Chốt Phí)
                if (phieu.TrangThai == "Hoàn thành")
                {
                    ModelState.AddModelError("TrangThai", "Vui lòng sử dụng nút [Chốt Phí] trong chi tiết phiếu để Hoàn thành.");
                }
                // 3. Logic: Chờ khám không nhảy cóc sang Đang sửa chữa
                else if (existingPhieu.TrangThai == "Chờ khám" && phieu.TrangThai == "Đang sửa chữa")
                {
                    ModelState.AddModelError("TrangThai", "Phải qua bước 'Đang kiểm tra' trước khi 'Đang sửa chữa'.");
                }
                // 4. Logic: Không lùi từ Đang sửa chữa về Chờ khám
                else if (existingPhieu.TrangThai == "Đang sửa chữa" && phieu.TrangThai == "Chờ khám")
                {
                    ModelState.AddModelError("TrangThai", "Không thể lùi từ 'Đang sửa chữa' về 'Chờ khám'.");
                }

                if (ModelState.IsValid)
                {
                    _context.Update(phieu);

                    // 5. Tự động ghi Log Lịch sử nếu có thay đổi trạng thái
                    if (existingPhieu.TrangThai != phieu.TrangThai)
                    {
                        var ktvId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        _context.LichSuTrangThais.Add(new LichSuTrangThai
                        {
                            MaPhieu = id,
                            TrangThaiCu = existingPhieu.TrangThai,
                            TrangThaiMoi = phieu.TrangThai,
                            ThoiGian = DateTime.Now,
                            Ma_KTV = string.IsNullOrEmpty(ktvId) ? null : int.Parse(ktvId)
                        });
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã cập nhật thông tin phiếu #{phieu.MaPhieu.ToString("D4")} thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            var thietBiList = _context.ThietBis.Include(t => t.KhachHang).Select(t => new { MaTB = t.MaTB, HienThi = t.KhachHang.HoTen + " - " + t.TenTB }).ToList();
            ViewData["MaTB"] = new SelectList(thietBiList, "MaTB", "HienThi", phieu.MaTB);
            return View(phieu);
        }

        // POST: Xoá phiếu tiếp nhận (chặn nếu đã Hoàn thành)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phieu = await _context.PhieuTiepNhans.FindAsync(id);
            if (phieu != null)
            {
                // Khóa cứng: Không cho xóa phiếu đã Hoàn thành
                if (phieu.TrangThai == "Hoàn thành")
                {
                    TempData["ErrorMsg"] = "Không thể xóa phiếu đã thanh toán và khóa cứng.";
                    return RedirectToAction("Details", new { id });
                }

                _context.PhieuTiepNhans.Remove(phieu);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa phiếu #{id.ToString("D4")} thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Hiển thị form chốt phí và hoàn thành phiếu
        public async Task<IActionResult> ChotPhi(int? id)
        {
            if (id == null) return NotFound();

            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.SuDungLinhKiens)
                        .ThenInclude(s => s.LinhKien)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            // Tính tổng chi phí linh kiện đã xuất kho
            var tongLinhKien = phieu.ChiTietSuaChuas?.Sum(ct => ct.ChiPhi) ?? 0;
            ViewBag.TongLinhKien = tongLinhKien;

            return View(phieu);
        }

        // POST: Cập nhật phí sửa chữa và đổi trạng thái
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChotPhi(int id, string? tenCongViec, decimal tienCong)
        {
            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.SuDungLinhKiens)
                        .ThenInclude(s => s.LinhKien)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            // Cập nhật trạng thái Hoàn thành (Bước 5)
            var trangThaiCu = phieu.TrangThai;
            phieu.TrangThai = "Hoàn thành";
            _context.Update(phieu);

            // Chốt phí không xác minh nguyên nhân hoặc tự nâng nguồn nhãn.

            // Ghi nhật ký chuyển trạng thái
            var ktvId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.LichSuTrangThais.Add(new LichSuTrangThai
            {
                MaPhieu = id,
                TrangThaiCu = trangThaiCu,
                TrangThaiMoi = "Hoàn thành",
                ThoiGian = DateTime.Now,
                Ma_KTV = string.IsNullOrEmpty(ktvId) ? null : int.Parse(ktvId)
            });

            // Chỉ thêm dòng tiền công nếu có giá trị > 0
            if (tienCong > 0)
            {
                var hangMucCong = string.IsNullOrWhiteSpace(tenCongViec)
                    ? "Tiền công sửa chữa"
                    : tenCongViec.Trim();

                var ct = new ChiTietSuaChua {
                    MaPhieu   = id,
                    TenHangMuc = hangMucCong,
                    ChiPhi    = tienCong
                };
                _context.Add(ct);
            }

            await _context.SaveChangesAsync();

            // Tính tổng để hiện thị thông báo
            var tongPhi = await _context.ChiTietSuaChuas
                .Where(ct => ct.MaPhieu == id)
                .SumAsync(ct => ct.ChiPhi);

            TempData["Success"] = $"Đã chốt phí {tongPhi:N0}đ và hoàn thành phiếu #{id.ToString("D4")}!";
            TempData["PlayCompletedSound"] = true;
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Xuất danh sách phiếu ra file Excel (CSV)
        public async Task<IActionResult> ExportExcel(string searchString, string trangThai)
        {
            var query = _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .Include(p => p.User_KTV)
                .AsQueryable();

            // Lọc dữ liệu y hệt như trên màn hình Index để xuất ra Excel cho chính xác
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.MaPhieu.ToString().Contains(searchString) ||
                                         (p.ThietBi != null && p.ThietBi.TenTB.Contains(searchString)) ||
                                         (p.ThietBi != null && p.ThietBi.KhachHang != null && p.ThietBi.KhachHang.HoTen.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả trạng thái")
            {
                query = query.Where(p => p.TrangThai == trangThai);
            }

            var danhSachPhieu = await query.OrderByDescending(p => p.NgayNhan).ToListAsync();

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Mã phiếu,Khách hàng,Số điện thoại,Thiết bị,KTV Tiếp nhận,Ngày nhận,Trạng thái");

            foreach (var item in danhSachPhieu)
            {
                var maPhieu = $"HD-{item.MaPhieu:D4}";
                var tenKhachHang = item.ThietBi?.KhachHang?.HoTen ?? "Khách lẻ";
                var sdt = item.ThietBi?.KhachHang?.SDT ?? "";
                var thietBi = item.ThietBi?.TenTB ?? "";
                var ktv = item.User_KTV?.FullName ?? "N/A";
                var ngayNhan = item.NgayNhan.ToString("dd/MM/yyyy HH:mm");
                var trangThaiPhieu = item.TrangThai;

                // Bọc trong dấu ngoặc kép để tránh lỗi tách cột khi dữ liệu chứa dấu phẩy
                builder.AppendLine($"\"{maPhieu}\",\"{tenKhachHang}\",\"{sdt}\",\"{thietBi}\",\"{ktv}\",\"{ngayNhan}\",\"{trangThaiPhieu}\"");
            }

            // Đính kèm BOM (Byte Order Mark) để Excel hiểu đây là file UTF-8 (Chống lỗi font tiếng Việt)
            var data = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(builder.ToString())).ToArray();

            return File(data, "text/csv", $"DanhSachPhieu_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        // ═══════════════════════════════════════════════════════════════════════
        // BƯỚC 2: NHẬN CA KIỂM TRA (KTV bấm nhận ca → khóa định danh KTV)
        // ═══════════════════════════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NhanCa(int id)
        {
            var phieu = await _context.PhieuTiepNhans.FindAsync(id);
            if (phieu == null) return NotFound();

            // Chỉ cho nhận ca khi phiếu đang "Chờ khám"
            if (phieu.TrangThai != "Chờ khám")
            {
                TempData["ErrorMsg"] = "Phiếu này không ở trạng thái \"Chờ khám\", không thể nhận ca.";
                return RedirectToAction("Details", new { id });
            }

            var ktvId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Chuyển trạng thái và khóa KTV vào phiếu
            phieu.TrangThai = "Đang kiểm tra";
            phieu.MaUser_KTV = int.Parse(ktvId!);
            _context.Update(phieu);

            // Ghi nhật ký chuyển trạng thái (Bước 2)
            _context.LichSuTrangThais.Add(new LichSuTrangThai
            {
                MaPhieu = id,
                TrangThaiCu = "Chờ khám",
                TrangThaiMoi = "Đang kiểm tra",
                ThoiGian = DateTime.Now,
                Ma_KTV = int.Parse(ktvId!)
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Bạn đã nhận ca kiểm tra phiếu #{id.ToString("D4")} thành công!";
            return RedirectToAction("Index", "TroLyChanDoan", new { maPhieu = id });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // BƯỚC 5: ĐÁNH GIÁ DỊCH VỤ (Khách hàng/KTV chấm sao + bình luận)
        // ═══════════════════════════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGia(int id, int soSao, string? binhLuan)
        {
            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.DanhGiaDichVu)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            // Chỉ cho đánh giá khi phiếu đã "Hoàn thành"
            if (phieu.TrangThai != "Hoàn thành")
            {
                TempData["ErrorMsg"] = "Chỉ có thể đánh giá phiếu đã hoàn thành.";
                return RedirectToAction("Details", new { id });
            }

            // Ràng buộc 1:1: Mỗi phiếu chỉ được đánh giá 1 lần
            if (phieu.DanhGiaDichVu != null)
            {
                TempData["ErrorMsg"] = "Phiếu này đã được đánh giá trước đó.";
                return RedirectToAction("Details", new { id });
            }

            // Validate số sao
            if (soSao < 1 || soSao > 5)
            {
                TempData["ErrorMsg"] = "Số sao phải từ 1 đến 5.";
                return RedirectToAction("Details", new { id });
            }

            var danhGia = new DanhGiaDichVu
            {
                MaPhieu = id,
                SoSao = soSao,
                BinhLuan = string.IsNullOrWhiteSpace(binhLuan) ? null : binhLuan.Trim(),
                NgayDG = DateTime.Now
            };

            _context.DanhGiaDichVus.Add(danhGia);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cảm ơn bạn đã đánh giá {soSao} sao cho phiếu #{id.ToString("D4")}!";
            return RedirectToAction("Details", new { id });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ĐÁNH GIÁ CÔNG KHAI (Khách hàng quét QR trên hóa đơn, không cần đăng nhập)
        // ═══════════════════════════════════════════════════════════════════════
        [AllowAnonymous]
        public async Task<IActionResult> DanhGiaPublic(int? id)
        {
            if (id == null) return NotFound();

            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .Include(p => p.User_KTV)
                .Include(p => p.DanhGiaDichVu)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            return View(phieu);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGiaPublic(int id, int soSao, string? binhLuan)
        {
            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.DanhGiaDichVu)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            if (phieu.TrangThai != "Hoàn thành")
            {
                ViewBag.ErrorMsg = "Phiếu chưa hoàn thành, không thể đánh giá.";
                return View(phieu);
            }

            if (phieu.DanhGiaDichVu != null)
            {
                ViewBag.AlreadyRated = true;
                return View(phieu);
            }

            if (soSao < 1 || soSao > 5) soSao = 5;

            var danhGia = new DanhGiaDichVu
            {
                MaPhieu = id,
                SoSao = soSao,
                BinhLuan = string.IsNullOrWhiteSpace(binhLuan) ? null : binhLuan.Trim(),
                NgayDG = DateTime.Now
            };

            _context.DanhGiaDichVus.Add(danhGia);
            await _context.SaveChangesAsync();

            ViewBag.ThankYou = true;
            // Reload to get the review attached
            phieu = await _context.PhieuTiepNhans
                .Include(p => p.DanhGiaDichVu)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            return View(phieu);
        }
    }
}
