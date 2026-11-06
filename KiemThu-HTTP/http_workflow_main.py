# Kiểm thử HTTP luồng nghiệp vụ trên SOURCE CHÍNH + database LocalDB tạm (tự tạo, tự xóa). Dựa trên các luồng Codex đã kiểm thử
# (ERD-V2-DEVELOPMENT-20261104/scripts/http_workflow.py), viết lại cho source chính và tên cột sau AlignErdV2.
# Dữ liệu mẫu đều là FIXTURE; không dùng database nghiệp vụ.
import html, http.cookiejar, json, os, re, socket, subprocess, sys, time, urllib.error, urllib.parse, urllib.request
from html.parser import HTMLParser
from pathlib import Path

PROJ = Path(r"D:\Code\Đồ án chuyên nghành\QuanLySuCoMayTinh")
OUT = Path(__file__).resolve().parent
SERVER = r"(localdb)\MSSQLLocalDB"
DB = "ITHelpdesk_MainFlow_20261105"
PASS = "FixtureOnly-2026!"
checks = []


def ck(name, ok, detail=""):
    checks.append({"check": name, "status": "PASS" if ok else "FAIL", "detail": str(detail)[:200]})
    print(("PASS " if ok else "FAIL ") + name + (f"  [{detail}]" if (detail and not ok) else ""), flush=True)


def sql(q, db="__DEFAULT__", rows=False):
    if db == "__DEFAULT__":
        db = DB
    cmd = ["sqlcmd", "-S", SERVER, "-E", "-C", "-b", "-f", "65001", "-W", "-h", "-1", "-s", "|", "-Q", "SET NOCOUNT ON; " + q]
    if db:
        cmd[3:3] = ["-d", db]
    p = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8")
    if p.returncode != 0:
        raise RuntimeError(p.stdout + p.stderr)
    return [l.split("|") for l in p.stdout.splitlines() if l.strip()] if rows else p.stdout.strip()


class Tokens(HTMLParser):
    def __init__(self, text):
        super().__init__(); self.token = None; self.feed(text)

    def handle_starttag(self, tag, attrs):
        a = dict(attrs)
        if tag == "input" and a.get("name") == "__RequestVerificationToken":
            self.token = a.get("value")


class Client:
    def __init__(self, base):
        self.base = base
        self.opener = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
        self.csrf = None

    def request(self, path, data=None, anti=True, json_accept=False, ajax=False):
        data = dict(data) if data is not None else None
        if data is not None and anti and self.csrf:
            data["__RequestVerificationToken"] = self.csrf
        h = {"Accept": "application/json" if json_accept else "text/html"}
        if ajax:
            h["X-Requested-With"] = "XMLHttpRequest"
        req = urllib.request.Request(self.base + path, data=urllib.parse.urlencode(data, doseq=True).encode() if data is not None else None, headers=h)
        try:
            with self.opener.open(req, timeout=60) as r:
                status, raw, url, ct = r.status, r.read(), r.url, r.headers.get("Content-Type", "")
        except urllib.error.HTTPError as e:
            with e:
                status, raw, url, ct = e.code, e.read(), e.url, e.headers.get("Content-Type", "")
        text = raw.decode("utf-8", "replace") if ("zip" not in ct) else ""
        t = Tokens(text).token if text else None
        if t:
            self.csrf = t
        return status, text, url, raw

    def login(self, user):
        self.request("/Auth/Login")
        s, b, _, _ = self.request("/Auth/Login", {"username": user, "password": PASS}, json_accept=True)
        ok = s == 200
        try:
            ok = ok and json.loads(b).get("success") is True
        except Exception:
            ok = False
        ck(f"đăng nhập {user}", ok, f"{s} {b[:80]}")


def create_ticket(admin, name, phone):
    admin.request("/PhieuTiepNhan/Create")  # làm mới mã chống giả mạo (CSRF) từ trang form
    s_, b, url, _ = admin.request("/PhieuTiepNhan/Create", dict(tenKhachHang=name, soDienThoai=phone, tenThietBi=name + " DEVICE", cauHinh="FIXTURE", ngoaiQuan="FIXTURE", submitAction="details", moTaSuCo="FIXTURE", phanLoaiYeuCau="fixture", mucDoUuTien="Thấp"))
    m = re.search(r"/Details/(\d+)|[?&]id=(\d+)", url)
    if m is None:
        raise RuntimeError(f"không tạo được phiếu: status={s_} url={url} body={b[:200]}")
    return int(next(g for g in m.groups() if g))


work_dir = OUT / "work"
TOP_SETS = [[1], [3], [10], [12], [18], [27], [31], [44]]


def top_for(admin, symptoms, idx):
    """Gợi ý đứng đầu (hidden MaLoi trên màn hình) và mã lỗi chẩn đoán được lưu vào phiếu."""
    t = create_ticket(admin, f"FIXTURE TOP {idx}", f"0000009{300 + idx}")
    admin.request("/PhieuTiepNhan/NhanCa", dict(id=t))
    s_, b, _, _ = admin.request("/TroLyChanDoan/XuLyChanDoan", dict(maPhieu=t, selectedTrieuChungs=symptoms))
    m = re.search(r'name="MaLoi" value="(\d+)"', b)
    cd = sql(f"SELECT ISNULL(CONVERT(varchar,MaLoi),'NULL') FROM PHIEU_TIEP_NHAN WHERE MaPhieu={t}")
    return (m.group(1) if m else None, cd)


def state(ticket):
    t = sql(f"SELECT TrangThai, ISNULL(MoTaSuCo,N'') FROM PHIEU_TIEP_NHAN WHERE MaPhieu={ticket}", rows=True)[0]
    hist = sql(f"SELECT MaLichSuChanDoan, ISNULL(CONVERT(varchar,MaLoiThucTe),'NULL'), TrangThaiXacMinh, NguonDuLieu FROM LICH_SU_CHAN_DOAN WHERE MaPhieu={ticket} ORDER BY MaLichSuChanDoan", rows=True)
    rep = sql(f"SELECT TOP 1 MaChiTietSuaChua FROM CHI_TIET_SUA_CHUA WHERE MaPhieu={ticket} ORDER BY MaChiTietSuaChua DESC", rows=True)
    usage = sql(f"SELECT u.SoLuong, u.DonGiaXuat FROM SU_DUNG_LINH_KIEN u JOIN CHI_TIET_SUA_CHUA c ON c.MaChiTietSuaChua=u.MaChiTietSuaChua WHERE c.MaPhieu={ticket}", rows=True)
    trainable = sql(f"SELECT COUNT(*) FROM LICH_SU_CHAN_DOAN WHERE MaPhieu={ticket} AND MaLoiThucTe IS NOT NULL AND NguonDuLieu=1 AND TrangThaiXacMinh=N'Đã xác minh' AND NgayXacMinh IS NOT NULL AND MaNguoiDungXacMinh IS NOT NULL")
    rv = sql(f"SELECT COUNT(*), ISNULL(MAX(SoSao),0) FROM DANH_GIA_DICH_VU WHERE MaPhieu={ticket}", rows=True)[0]
    stock = sql("SELECT SoLuongTon FROM LINH_KIEN WHERE TenLinhKien=N'FIXTURE PART'")
    return {"TrangThai": t[0], "MoTaSuCo": t[1], "hist": hist, "repair": int(rep[0][0]) if rep else None, "usage": usage,
            "trainable": int(trainable), "reviewCount": int(rv[0]), "reviewStars": int(rv[1]), "stock": int(stock)}


def main():
    sql(f"IF DB_ID('{DB}') IS NOT NULL BEGIN ALTER DATABASE {DB} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {DB} END; CREATE DATABASE {DB}", db=None)
    s = socket.socket(); s.bind(("127.0.0.1", 0)); port = s.getsockname()[1]; s.close(); base = f"http://127.0.0.1:{port}"
    env = dict(os.environ, ASPNETCORE_ENVIRONMENT="Production", ALLOW_ALIGN_ERDV2="1",
               ConnectionStrings__DefaultConnection=f"Server={SERVER};Database={DB};Trusted_Connection=True;TrustServerCertificate=True",
               AI__BootstrapMaxRuleGap=os.environ.get("RULE_GAP", "10"),
               AI__ModelRoot=str(OUT / "models"))

    def start(extra_env=None):
        log = open(OUT / "server.log", "a", encoding="utf-8")
        work = OUT / "work"; work.mkdir(exist_ok=True)
        import shutil; shutil.rmtree(work / "Data" / "MLModels", ignore_errors=True)
        shutil.rmtree(OUT / "models", ignore_errors=True); shutil.copytree(PROJ / "Data" / "MLModels" / "SongNgu", OUT / "models" / "SongNgu", ignore=shutil.ignore_patterns("song_ngu_fp32.onnx"))   # kho mô hình riêng cho lần chạy thử   # tránh mô hình sót từ lần chạy trước làm sai đường cơ sở
        shutil.copytree(Path("D:/Code/Đồ án chuyên nghành/AI-Models/MaLoiTrucTiep"), OUT / "models" / "MaLoiTrucTiep")
        shutil.copytree(Path("D:/Code/Đồ án chuyên nghành/AI-Models/KetQuaSua"), OUT / "models" / "KetQuaSua")
        p = subprocess.Popen(["dotnet", str(PROJ / "bin" / "Debug" / "net8.0" / "QuanLySuCoMayTinh.dll"), "--urls", base], cwd=work, env=extra_env or env, stdout=log, stderr=subprocess.STDOUT)
        for _ in range(120):
            if p.poll() is not None:
                raise RuntimeError("website không khởi động được; xem server.log")
            try:
                urllib.request.urlopen(base + "/Auth/Login", timeout=3); return p, log
            except Exception:
                time.sleep(1)
        raise RuntimeError("website khởi động quá thời gian")

    def stop(p, log):
        p.terminate()
        try:
            p.wait(timeout=20)
        except Exception:
            p.kill()
        log.close()

    result = {}
    p = log = None
    try:
        p, log = start(); stop(p, log)  # lần 1: áp dụng migration
        sql("""INSERT INTO NGUOI_DUNG(TenDangNhap,MatKhauBam,HoTen,VaiTro,TrangThaiHoatDong) VALUES (N'main_admin_fixture',N'%s',N'MAIN ADMIN FIXTURE',N'Admin KTV',1),(N'main_ktv_fixture',N'%s',N'MAIN KTV FIXTURE',N'KTV',1);
INSERT INTO LINH_KIEN(TenLinhKien,LoaiLinhKien,SoLuongTon,DonGia,TrangThaiSuDung) VALUES (N'FIXTURE PART',N'FIXTURE',10,999,1);
INSERT INTO KHACH_HANG(HoTen,SoDienThoai,DiaChi) VALUES (N'SEED FIXTURE',N'00000009000',N'FIXTURE');
INSERT INTO THIET_BI(TenThietBi,CauHinh,MaKhachHang,NgoaiQuan) SELECT N'SEED DEVICE',N'x',MaKhachHang,N'' FROM KHACH_HANG;
INSERT INTO PHIEU_TIEP_NHAN(MaThietBi,MaNguoiDung,NgayNhan,TrangThai) SELECT MaThietBi,(SELECT MIN(MaNguoiDung) FROM NGUOI_DUNG),SYSUTCDATETIME(),N'Hoàn thành' FROM THIET_BI;
INSERT INTO ML_MODEL_INFO(PhienBan,DoChinhXacMicro,DoChinhXacMacro,LogLoss,LogLossReduction,SoBanGhiHuanLuyen,NgayTao,DangSuDung,DoChinhXacAI,SoBanGhiThucTe,DoChinhXacRule,SoBanGhiTongHop,SoBanGhiLichSuNguoiDung,LoaiBaiToan) VALUES (N'FIXTURE_V1',0.9,0.9,0.1,0.1,50,SYSUTCDATETIME(),0,90,5,80,45,0,N'chan_doan_loi');""" % (PASS, PASS))
        p, log = start()  # lần 2: nạp dữ liệu mồi vì đã có phiếu
        ck("website khởi động trên database tạm sau migration", True)
        anon = Client(base); admin = Client(base)
        admin.login("main_admin_fixture")
        stock0 = state(1)["stock"]
        part = int(sql("SELECT MaLinhKien FROM LINH_KIEN WHERE TenLinhKien=N'FIXTURE PART'"))
        s_, b, _, _ = admin.request("/PhieuTiepNhan/Create")
        ck("form tiếp nhận hiển thị trường mới", s_ == 200 and all(x in b for x in ["moTaSuCo", "phanLoaiYeuCau", "mucDoUuTien"]))
        desc = "FIXTURE: máy tính khởi động lỗi, không có dữ liệu khách hàng thật."
        s_, b, url, _ = admin.request("/PhieuTiepNhan/Create", dict(tenKhachHang="FIXTURE HTTP KH", soDienThoai="00000009100", tenThietBi="FIXTURE HTTP LAPTOP", cauHinh="FIXTURE", ngoaiQuan="FIXTURE", submitAction="details", moTaSuCo=desc, phanLoaiYeuCau="Phần mềm - fixture", mucDoUuTien="Cao"))
        m = re.search(r"/Details/(\d+)|[?&]id=(\d+)", url)
        ck("tạo phiếu và mở chi tiết", s_ == 200 and m is not None, f"{s_} {url}")
        ticket = int(next(g for g in m.groups() if g))
        st = state(ticket)
        ck("trường mới được lưu (MoTaSuCo) và trạng thái Chờ khám", st["MoTaSuCo"] == desc and st["TrangThai"] == "Chờ khám", st["TrangThai"])
        s_, b, _, _ = admin.request("/PhieuTiepNhan/NhanCa", dict(id=ticket))
        ck("KTV nhận ca → Đang kiểm tra", s_ == 200 and state(ticket)["TrangThai"] == "Đang kiểm tra", state(ticket)["TrangThai"])
        s_, b, _, _ = admin.request("/TroLyChanDoan/XuLyChanDoan", dict(maPhieu=ticket, selectedTrieuChungs=[1]))
        ck("chẩn đoán chạy và hiển thị gợi ý", s_ == 200 and "Exception" not in b and 'name="MaLoi"' in b, s_)
        s_, b, _, _ = admin.request("/TroLyChanDoan/ChotDonSuaChua", dict(MaPhieu=ticket, MaLoi=1, TenLoi="FIXTURE KTV DIAGNOSIS", TrieuChungIds=[1], TrieuChungNames=["Màn hình không lên sau khi bật máy"]))
        st = state(ticket)
        ck("chốt đơn → Đang sửa chữa, chọn mã lỗi KHÔNG tự thành nhãn thực", s_ == 200 and st["TrangThai"] == "Đang sửa chữa" and st["trainable"] == 0 and all(h[1] == "NULL" for h in st["hist"]), f"{st['TrangThai']} {st['hist']}")
        s_, b, _, _ = admin.request("/SuDungLinhKien/XuatKho", dict(maPhieu=ticket, maLK=part, soLuong=2, ghiChu="FIXTURE PART USE"))
        st = state(ticket)
        ck("xuất linh kiện trừ tồn kho và lưu đơn giá xuất", s_ == 200 and st["stock"] == stock0 - 2 and len(st["usage"]) == 1 and float(st["usage"][0][1]) == 999.0, f"stock={st['stock']} usage={st['usage']}")
        admin.request("/SuDungLinhKien/XuatKho", dict(maPhieu=ticket, maLK=part, soLuong=999))
        st = state(ticket)
        ck("thiếu tồn kho không làm đổi dữ liệu", st["stock"] == stock0 - 2 and len(st["usage"]) == 1, f"stock={st['stock']}")
        hist = int(st["hist"][-1][0]); repair = st["repair"]
        s_, b, _, _ = admin.request("/TroLyChanDoan/XacMinhNguyenNhan", dict(maLichSuChanDoan=hist, maLoiThucTe=1))
        ck("xác minh nguyên nhân bị chặn khi chưa có bằng chứng kiểm tra lại", s_ == 400 and state(ticket)["trainable"] == 0, s_)
        s_, b, _, _ = admin.request("/PhieuTiepNhan/ChotPhi", dict(id=ticket, tenCongViec="FIXTURE LABOR", tienCong=50))
        st = state(ticket)
        ck("chốt phí → Hoàn thành, không tự nâng thành RealRepair", s_ == 200 and st["TrangThai"] == "Hoàn thành" and st["trainable"] == 0, st["TrangThai"])
        s_, b, _, _ = admin.request("/PhieuTiepNhan/Print/" + str(ticket))
        ck("in phiếu hiển thị đúng", s_ == 200 and "FIXTURE HTTP LAPTOP" in html.unescape(b), s_)
        s_, b, _, _ = anon.request("/PhieuTiepNhan/DanhGiaPublic/" + str(ticket))
        ck("trang đánh giá công khai mở được khi chưa đăng nhập", s_ == 200 and anon.csrf is not None, s_)
        anon.request("/PhieuTiepNhan/DanhGiaPublic", dict(id=ticket, soSao=4, binhLuan="FIXTURE PUBLIC RATING"))
        st = state(ticket)
        ck("đánh giá công khai lưu một-một (4 sao)", st["reviewStars"] == 4 and st["reviewCount"] == 1, f"{st['reviewStars']}/{st['reviewCount']}")
        anon.request("/PhieuTiepNhan/DanhGiaPublic", dict(id=ticket, soSao=5, binhLuan="FIXTURE DUPLICATE"))
        st = state(ticket)
        ck("đánh giá trùng bị từ chối, không ghi đè", st["reviewStars"] == 4 and st["reviewCount"] == 1, f"{st['reviewStars']}/{st['reviewCount']}")
        admin.request("/PhieuTiepNhan/Details/" + str(ticket))
        s_, b, _, _ = admin.request("/ChiTietSuaChua/CapNhatKiemTraLai", dict(maCT=repair, trangThai="Đạt", bangChung="FIXTURE ONLY: local retest reference"))
        ck("lưu bằng chứng kiểm tra lại", s_ == 200, s_)
        ktv = Client(base); ktv.login("main_ktv_fixture"); ktv.request("/PhieuTiepNhan/Create")
        s_, b, _, _ = ktv.request("/TroLyChanDoan/XacMinhNguyenNhan", dict(maLichSuChanDoan=hist, maLoiThucTe=1), ajax=True)
        ck("KTV không được giao ca không xác minh được", s_ in (400, 403) and state(ticket)["trainable"] == 0, s_)
        s_, b, _, _ = admin.request("/TroLyChanDoan/XacMinhNguyenNhan", dict(maLichSuChanDoan=hist, maLoiThucTe=1))
        ck("xác minh nguyên nhân có người thực hiện → đủ điều kiện huấn luyện", s_ == 200 and state(ticket)["trainable"] == 1, f"{s_} trainable={state(ticket)['trainable']}")
        admin.request("/PhieuTiepNhan/Details/" + str(ticket))
        admin.request("/ChiTietSuaChua/CapNhatKiemTraLai", dict(maCT=repair, trangThai="Không đạt", bangChung="FIXTURE ONLY: evidence changed"))
        ck("đổi kết quả kiểm tra lại thu hồi điều kiện huấn luyện", state(ticket)["trainable"] == 0, state(ticket)["trainable"])
        s_, b, _, _ = anon.request("/TroLyChanDoan/HuanLuyen", dict(), anti=False)
        ck("người ẩn danh không kích hoạt được huấn luyện", s_ in (400, 401, 403) or "/Auth/Login" in b, s_)
        s_, b, _, _ = anon.request("/Admin/XuatDuLieuHuanLuyen")
        ck("người ẩn danh không xuất được dữ liệu huấn luyện", s_ in (401, 403) or "/Auth/Login" in b or s_ == 200 and "Đăng nhập" in b, s_)
        base_tops = [top_for(admin, sy, i) for i, sy in enumerate(TOP_SETS)]
        ck("đường cơ sở: chẩn đoán chỉ dùng Rule (chưa có mô hình) cho mọi bộ triệu chứng thử", all(t[0] is not None and t[0] == t[1] for t in base_tops), base_tops)
        # huấn luyện KHỞI TẠO: chưa có nhãn thực đã xác minh → huấn luyện từ dữ liệu tổng hợp, gắn nhãn rõ, không báo là đã kiểm chứng
        admin.request("/Admin/MLModels")
        before = int(sql("SELECT COUNT(*) FROM ML_MODEL_INFO"))
        s_, b, _, _ = admin.request("/TroLyChanDoan/HuanLuyen", dict(), ajax=True, json_accept=True)
        ck("admin gửi yêu cầu huấn luyện được chấp nhận", s_ in (200, 302), f"{s_} {b[:100]}")
        final = None
        for _ in range(180):
            time.sleep(2)
            rows = sql("SELECT TOP 1 TrangThai, ISNULL(ThongBaoLoi,N'') FROM AI_TRAINING_JOB ORDER BY MaTacVu DESC", rows=True)
            if rows and rows[0][0] not in ("Pending", "Running"):
                final = rows[0]; break
        ck("tác vụ huấn luyện kết thúc", final is not None, final)
        ck("huấn luyện khởi tạo hoàn thành và ghi cảnh báo chưa kiểm chứng bằng ca thật", final is not None and final[0] == "Completed" and "KHỞI TẠO" in final[1], final)
        mrow = sql("SELECT TOP 1 MaMoHinh, PhienBanBoDuLieu, SoBanGhiThucTe, SoBanGhiTongHop, DoChinhXacAI, DoChinhXacRule, DoChinhXacMicro, DoChinhXacMacro FROM ML_MODEL_INFO ORDER BY MaMoHinh DESC", rows=True)
        ck("có mô hình mới gắn nhãn KHOI_TAO_TONG_HOP, không có bản ghi thực", int(sql("SELECT COUNT(*) FROM ML_MODEL_INFO")) == before + 1 and mrow[0][1].startswith("KHOI_TAO_TONG_HOP") and int(mrow[0][2]) == 0, mrow)
        print("   chỉ số mô hình khởi tạo (AI%, Rule%, Micro, Macro):", mrow[0][4:], flush=True)
        model_id = int(mrow[0][0])
        s_, b, _, _ = admin.request("/Admin/MLModels")
        ck("trang quản lý hiển thị nhãn 'Khởi tạo (tổng hợp)'", s_ == 200 and "Khởi tạo (tổng hợp)" in b, s_)
        s_, b, _, _ = admin.request("/Admin/ActivateModel", dict(id=model_id), json_accept=True)
        try:
            j = json.loads(b)
        except Exception:
            j = {}
        gate_msg = j.get("message", "")
        activated = s_ == 200 and j.get("success") is True
        blocked_ok = s_ == 200 and j.get("success") is False and "thấp hơn Rule Engine" in gate_msg
        ck("kích hoạt mô hình khởi tạo: được kích hoạt kèm cảnh báo, hoặc bị chặn đúng chốt 'AI không kém Rule quá 10 điểm'",
           (activated and "khởi tạo" in gate_msg) or blocked_ok, f"{s_} {gate_msg[:160]}")
        print("   kết cục kích hoạt:", "ĐÃ KÍCH HOẠT" if activated else "BỊ CHẶN (AI kém Rule quá 10 điểm)", flush=True)
        active_zip = OUT / "models" / "ActiveModel.zip"
        if activated:
            ck("ActiveModel.zip được tạo trong kho mô hình riêng AI:ModelRoot (không phải thư mục dự án)", active_zip.exists() and active_zip.stat().st_size > 1000, active_zip)
        # chốt an toàn khác vẫn hoạt động: mô hình fixture chỉ có 5 bản ghi thực, không phải mô hình khởi tạo → vẫn bị chặn
        s_, b, _, _ = admin.request("/Admin/ActivateModel", dict(id=int(sql("SELECT MIN(MaMoHinh) FROM ML_MODEL_INFO WHERE PhienBan=N'FIXTURE_V1'"))), json_accept=True)
        try:
            j = json.loads(b)
        except Exception:
            j = {}
        ck("mô hình thường thiếu bản ghi thực vẫn bị chặn (chốt ≥100 bản ghi giữ nguyên)", s_ == 200 and j.get("success") is False and "100" in j.get("message", ""), f"{s_} {b[:120]}")
        restarted = False
        if activated:
            # chẩn đoán dùng mô hình: có thể cần khởi động lại để nạp ActiveModel.zip mới
            def diag_has_ai():
                t2 = create_ticket(admin, "FIXTURE AI DIAG", "00000009200")
                admin.request("/PhieuTiepNhan/NhanCa", dict(id=t2))
                s2, b2, _, _ = admin.request("/TroLyChanDoan/XuLyChanDoan", dict(maPhieu=t2, selectedTrieuChungs=[1]))
                return s2 == 200, ("Hybrid AI" in b2), ("Mô hình khởi tạo" in b2)
            ok, has_ai, has_note = diag_has_ai()
            if ok and not has_ai:
                stop(p, log); p, log = start(); admin = Client(base); admin.login("main_admin_fixture"); restarted = True
                ok, has_ai, has_note = diag_has_ai()
            ck("chẩn đoán dùng mô hình khởi tạo và hiển thị cảnh báo trên màn hình kết quả" + (" (sau khi khởi động lại)" if restarted else " (không cần khởi động lại)"), ok and has_ai and has_note, f"ok={ok} ai={has_ai} note={has_note} restarted={restarted}")
            after_tops = [top_for(admin, sy, 20 + i) for i, sy in enumerate(TOP_SETS)]
            ck("AI khởi tạo chỉ hiển thị: gợi ý đứng đầu và mã lỗi lưu vào phiếu GIỐNG HỆT khi chỉ dùng Rule (không đổi thứ hạng)", after_tops == base_tops, f"before={base_tops} after={after_tops}")
        result_extra = {"can_khoi_dong_lai_de_nap_model_moi": restarted}
        for path in ["/Admin/MLModels", "/Admin/Dashboard", "/Admin/ValidationSuite", "/PhieuTiepNhan/Index", "/Auth/Profile"]:
            s_, b, _, _ = admin.request(path)
            ck("trang " + path, s_ == 200 and "Unhandled exception" not in b, s_)
        from urllib.parse import quote
        s_, b, _, _ = admin.request("/PhieuTiepNhan/GoiYYeuCau?moTa=" + quote("Pin bị phồng, cần gấp"))
        ck("endpoint gợi ý yêu cầu trả Cao + Nguồn & Pin", s_ == 200 and '"Cao"' in b and "Ngu" in b, f"{s_} {b[:160]}")
        for nm, txt, ky_vong in (("tiếng Việt", "Laptop bị vào nước, bật không lên, khách cần gấp", "Cao"), ("tiếng Anh", "The wifi keeps dropping and I cannot connect to the network", "Mạng")):
            s_, b, _, _ = admin.request("/PhieuTiepNhan/GoiYYeuCau?moTa=" + quote(txt))
            import json as _j
            jj = _j.loads(b) if s_ == 200 else {}
            ck(f"gợi ý song ngữ ONNX hoạt động với câu {nm} (nguồn có mô hình, kết quả hợp lý)", s_ == 200 and "song ngữ" in jj.get("nguon", "") and ky_vong in (jj.get("phanLoai"), jj.get("mucDo")), f"{s_} {b[:300]}")
        for nm, txt, tu_khoa in (("tiếng Việt", "máy không kết nối được wifi, sóng yếu hay bị ngắt", "wifi"), ("tiếng Anh", "my laptop screen is flickering and has stripes", "màn hình")):
            s_, b, _, _ = admin.request("/TroLyChanDoan/GoiYTrieuChung?moTa=" + quote(txt))
            jj = _j.loads(b) if s_ == 200 else {}
            kq = jj.get("ketQua", [])
            ck(f"gợi ý triệu chứng từ mô tả {nm}: có kết quả đúng chủ đề", s_ == 200 and jj.get("sanSang") and any(tu_khoa in k["ten"].lower() for k in kq), f"{s_} {b[:300]}")
        s_, b, _, _ = admin.request("/TroLyChanDoan/GoiYTrieuChung?moTa=" + quote("my wifi keeps dropping and the router is unstable"))
        jj = _j.loads(b) if s_ == 200 else {}
        ck("mô hình học máy dự đoán nhóm lỗi: câu tiếng Anh về wifi ra nhóm mạng", s_ == 200 and any(n["ma"] in ("wifi", "mang", "router") for n in jj.get("nhomLoi", [])[:2]), f"{s_} {b[:300]}")
        s_, b, _, _ = admin.request("/TroLyChanDoan/GoiYTrieuChung?moTa=" + quote("ổ cứng kêu lạch cạch và máy không nhận ổ"))
        jj = _j.loads(b) if s_ == 200 else {}
        ck("mô hình học máy dự đoán nhóm lỗi: câu tiếng Việt về ổ cứng ra nhóm lưu trữ", s_ == 200 and any(n["ma"] in ("o_cung", "ssd") for n in jj.get("nhomLoi", [])[:2]), f"{s_} {b[:300]}")
        t_ai = create_ticket(admin, "FIXTURE AI CORE", "0000009777"); admin.request("/PhieuTiepNhan/NhanCa", dict(id=t_ai))
        s_, b, _, _ = admin.request("/TroLyChanDoan/XuLyChanDoan", dict(maPhieu=t_ai, selectedTrieuChungs=[3], moTaTuDo="màn hình bị sọc ngang và nhấp nháy"))
        ck("chẩn đoán có lõi học máy: trang kết quả hiện nhóm lỗi do AI dự đoán", s_ == 200 and "Mô hình học máy dự đoán nhóm lỗi" in b and "AI nhóm lỗi" in b, f"{s_} {len(b)}")
        ck("mô hình dự đoán trực tiếp mã lỗi: thẻ AI mã lỗi hiện trong kết quả", s_ == 200 and "AI mã lỗi" in b, f"{s_} {len(b)}")
        ck("kết quả chẩn đoán hiện mức tin cậy AI theo biên điểm", s_ == 200 and "Tin cậy AI" in b, f"{s_} {len(b)}")
        # AI tự chốt: mô tả rõ + triệu chứng khớp → tin cậy Cao thì phiếu tự sang "Đang sửa chữa" và có nhật ký AI_TU_CHOT; nếu tin cậy không Cao thì vẫn ở trang kết quả
        tu_chot = False; ket_qua_th = False; s_ = 0; url_ = ""; tried = 0
        for md_ in ("laptop tự tắt, quạt kêu to, rất nóng, có mùi khét khi chạy lâu", "Quên mất mật khẩu đăng nhập Windows, vào không được", "Máy in Canon báo offline, in không ra", "Ổ C: báo đầy 100%, cài app nào cũng không được", "Bấm nút nguồn mà máy không có phản ứng gì, không đèn", "Bản lề laptop bị gãy, màn hình bị lỏng lẻo"):
            s_, b, _, _ = admin.request("/TroLyChanDoan/GoiYTrieuChung?moTa=" + quote(md_)); ids_ = [k["ma"] for k in _j.loads(b)["ketQua"]]
            t_auto = create_ticket(admin, "FIXTURE AI AUTO", "00000097%02d" % (78 + tried)); tried += 1
            s_, b, url_, _ = admin.request("/TroLyChanDoan/XuLyChanDoan", dict(maPhieu=t_auto, selectedTrieuChungs=ids_, moTaTuDo=md_))
            tu_chot = "/PhieuTiepNhan/Details" in url_ and "AI đã tự chốt mã lỗi" in b
            ket_qua_th = s_ == 200 and "Tin cậy AI" in b and "/PhieuTiepNhan/Details" not in url_
            if tu_chot: break
        ck("AI tự chốt khi tin cậy Cao hoặc hiện kết quả kèm thẻ tin cậy khi không Cao (từ điển thử của harness ít hơn DB dev nên có thể không có ca Cao)", s_ == 200 and (tu_chot or ket_qua_th), f"{s_} {url_} tự chốt={tu_chot} số câu thử={tried}")
        print("   [thông tin] AI tự chốt xảy ra:", tu_chot)
        s_, b, _, _ = admin.request("/TroLyChanDoan/ThongKeTuChotAI"); jt = _j.loads(b) if s_ == 200 else {}
        ck("giám sát AI tự chốt: có thống kê và chưa tạm dừng", s_ == 200 and "soCaTuChot" in jt and jt.get("tamDung") is False and (jt["soCaTuChot"] >= 1 if tu_chot else True), f"{s_} {b[:200]}")
        # Nhánh TỰ TẠM DỪNG: chèn 12 ca tự chốt giả mà mã lỗi hiện tại của phiếu khác mã AI chốt (tỉ lệ sửa lại 100% > 30%, ≥ 10 ca) → phải tạm dừng; xóa đi thì hoạt động lại
        ml_ = sql("SELECT TOP 1 MaLoi FROM MA_LOI ORDER BY MaLoi").strip()
        sql(f"UPDATE PHIEU_TIEP_NHAN SET MaLoi={ml_} WHERE MaPhieu={t_ai}")
        sql("INSERT INTO LICH_SU_TRANG_THAI (MaPhieu,TrangThaiCu,TrangThaiMoi,ThoiGian,LoaiSuKien,DuLieuBoSungJSON) VALUES " + ",".join(f"({t_ai},N'Đang kiểm tra',N'Đang sửa chữa',GETDATE(),N'AI_TU_CHOT_GIA',REPLACE(N'{{~MaLoi~:-1}}',N'~',NCHAR(34)))" for _ in range(12)))
        sql("UPDATE LICH_SU_TRANG_THAI SET LoaiSuKien=N'AI_TU_CHOT' WHERE LoaiSuKien=N'AI_TU_CHOT_GIA'")
        s_, b, _, _ = admin.request("/TroLyChanDoan/ThongKeTuChotAI"); jp = _j.loads(b) if s_ == 200 else {}
        ck("giám sát AI tự chốt: tự tạm dừng khi ≥ 10 ca và tỉ lệ sửa lại > 30%", s_ == 200 and jp.get("tamDung") is True and jp.get("tiLeSuaLai", 0) > 0.3, f"{s_} {b[:250]}")
        sql(f"DELETE FROM LICH_SU_TRANG_THAI WHERE MaPhieu={t_ai} AND LoaiSuKien=N'AI_TU_CHOT' AND DuLieuBoSungJSON=REPLACE(N'{{~MaLoi~:-1}}',N'~',NCHAR(34))")
        sql(f"UPDATE PHIEU_TIEP_NHAN SET MaLoi=NULL WHERE MaPhieu={t_ai}")
        s_, b, _, _ = admin.request("/TroLyChanDoan/ThongKeTuChotAI"); jq = _j.loads(b) if s_ == 200 else {}
        ck("giám sát AI tự chốt: bỏ ca giả thì hoạt động lại", s_ == 200 and jq.get("tamDung") is False, f"{s_} {b[:200]}")
        s_, b, _, _ = admin.request(f"/PhieuTiepNhan/Details/{t_ai}")
        ck("chi tiết phiếu hiện dự đoán khả năng sửa của mô hình học máy", s_ == 200 and "AI dự đoán khả năng sửa" in b, f"{s_} {len(b)}")
        s_, b, _, _ = admin.request("/TroLyChanDoan/Index")
        ck("trang chẩn đoán có ô mô tả tự do", s_ == 200 and 'id="moTaTuDo"' in b, s_)
        s_, b, _, raw = admin.request("/Admin/XuatDuLieuHuanLuyen")
        ck("xuất dữ liệu huấn luyện trả ZIP", s_ == 200 and raw[:2] == b"PK", f"{s_} {len(raw)}")
        result = {"status": "PASS" if all(c["status"] == "PASS" for c in checks) else "FAIL"}; result.update(result_extra)
    except Exception as e:
        ck("ngoại lệ trong lúc chạy", False, repr(e))
        result = {"status": "FAIL"}
    finally:
        if p is not None and p.poll() is None:
            stop(p, log)
        try:
            if os.environ.get('KEEP_DB') != '1':
                sql(f"ALTER DATABASE {DB} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {DB}", db=None)
        except Exception:
            pass
        result.update(checks=checks, check_count=len(checks), database="LocalDB tạm, đã xóa", fixture_only=True)
        (OUT / "ket_qua_http_source_chinh.json").write_text(json.dumps(result, ensure_ascii=False, indent=2), encoding="utf-8")
        print(json.dumps({"status": result["status"], "check_count": len(checks), "fail": [c["check"] for c in checks if c["status"] == "FAIL"]}, ensure_ascii=False))


if __name__ == "__main__":
    main()
