# Chuyển màu gõ cứng của giao diện tối cũ sang biến màu theo theme (dashboard-shell.css): --surface, --surface2, --text, --text-muted, --text-light, --border.
# Giữ chữ trắng khi cùng khối CSS/style có nền màu đậm (nút, huy hiệu, icon). Có sao lưu trước. Dùng: python migrate_colors.py <file> [--apply]
import re, shutil, sys
from pathlib import Path

WHITE_A = r"rgba\(\s*255\s*,\s*255\s*,\s*255\s*,\s*(0?\.\d+|1(?:\.0+)?)\s*\)"
STRONG_BG = re.compile(r"background(?:-color)?\s*:[^;{}]*(gradient|#[0-9a-fA-F]{3,8}\b|var\(--(?:lav|green|danger|orange|cyan|accent|status)|rgb\(|rgba\((?!\s*255\s*,\s*255\s*,\s*255))", re.I)


def alpha_text(a):
    a = float(a)
    return "var(--text-muted)" if a >= 0.35 else "var(--text-light)"


def fix_decls(block):
    """block: nội dung giữa { } hoặc trong style="..." — trả về (chuỗi mới, số thay đổi)."""
    n = 0
    strong = bool(STRONG_BG.search(block)) and not re.search(r"background(?:-color)?\s*:\s*#fff\b|background(?:-color)?\s*:\s*white", block, re.I)

    def color_alpha(m):
        nonlocal n
        n += 1
        return "color:" + alpha_text(m.group(1)) if not strong else m.group(0)
    block = re.sub(r"(?<![\w-])color\s*:\s*" + WHITE_A, color_alpha, block)

    def color_white(m):
        nonlocal n
        if strong:
            return m.group(0)
        n += 1
        return "color:var(--text)"
    block = re.sub(r"(?<![\w-])color\s*:\s*(?:#fff(?:fff)?|white)\b", color_white, block, flags=re.I)

    def bg_white(m):
        nonlocal n
        n += 1
        a = float(m.group(2))
        if re.search(r"(?<![\w-])(width|height)\s*:\s*1px", block):   # đường kẻ mảnh: dùng màu viền, không dùng màu nền thẻ
            return m.group(1) + ":var(--border)"
        return m.group(1) + (":var(--surface)" if a < 0.07 else ":var(--surface2)")
    block = re.sub(r"(background(?:-color)?)\s*:\s*" + WHITE_A.replace("(0?\\.\\d+|1(?:\\.0+)?)", "(0?\\.\\d+)"), lambda m: bg_white(re.match(r"(.*?)\s*:.*?,\s*(0?\.\d+)\s*\)", m.group(0), re.S)) if re.match(r"(.*?)\s*:.*?,\s*(0?\.\d+)\s*\)", m.group(0), re.S) else m.group(0), block)

    def border_white(m):
        nonlocal n
        n += 1
        return m.group(1) + ":" + (m.group(2) or "") + "var(--border)"
    NEON = {"#4ade80": "var(--c-ok)", "#22c55e": "var(--c-ok)", "#34d399": "var(--c-ok)", "#10b981": "var(--c-ok)",
            "#f87171": "var(--c-err)", "#ef4444": "var(--c-err)", "#fb7185": "var(--c-err)",
            "#fbbf24": "var(--c-warn)", "#f59e0b": "var(--c-warn)", "#fcd34d": "var(--c-warn)", "#ffb84e": "var(--c-warn)",
            "#38bdf8": "var(--c-info)", "#22d3ee": "var(--c-info)", "#60a5fa": "var(--c-info)", "#7dd3fc": "var(--c-info)", "#67e8f9": "var(--c-info)",
            "#a78bfa": "var(--c-purple)", "#c4b5fd": "var(--c-purple)", "#8b5cf6": "var(--c-purple)",
            "#fca5a5": "var(--c-err)", "#fecaca": "var(--c-err)", "#fda4af": "var(--c-err)", "#dc2626": "var(--c-err)",
            "#fde047": "var(--c-warn)", "#fef08a": "var(--c-warn)", "#eab308": "var(--c-warn)",
            "#86efac": "var(--c-ok)", "#bbf7d0": "var(--c-ok)", "#6ee7b7": "var(--c-ok)", "#5eead4": "var(--c-info)", "#99f6e4": "var(--c-info)",
            "#d8b4fe": "var(--c-purple)", "#e9d5ff": "var(--c-purple)", "#c084fc": "var(--c-purple)", "#bae6fd": "var(--c-info)"}
    def neon(m):
        nonlocal n
        v = NEON.get(m.group(1).lower())
        if not v:
            return m.group(0)
        n += 1
        return "color:" + v
    block = re.sub(r"(?<![\w-])color\s*:\s*(#[0-9a-fA-F]{6})", neon, block)
    block = re.sub(r"(border(?:-[a-z]+)?)\s*:\s*((?:\d+(?:\.\d+)?px\s+solid\s+)?)" + WHITE_A, border_white, block)
    block = re.sub(r"(border(?:-[a-z]+)?-color)\s*:\s*" + WHITE_A, lambda m: (m.group(1) + ":var(--border)"), block)
    return block, n


NEON_MAP = {"#4ade80": "var(--c-ok)", "#22c55e": "var(--c-ok)", "#34d399": "var(--c-ok)", "#10b981": "var(--c-ok)",
            "#f87171": "var(--c-err)", "#ef4444": "var(--c-err)", "#fb7185": "var(--c-err)",
            "#fbbf24": "var(--c-warn)", "#f59e0b": "var(--c-warn)", "#fcd34d": "var(--c-warn)", "#ffb84e": "var(--c-warn)",
            "#38bdf8": "var(--c-info)", "#22d3ee": "var(--c-info)", "#60a5fa": "var(--c-info)", "#7dd3fc": "var(--c-info)", "#67e8f9": "var(--c-info)",
            "#a78bfa": "var(--c-purple)", "#c4b5fd": "var(--c-purple)", "#8b5cf6": "var(--c-purple)"}
NEON_RE = re.compile(r"(?<![\w-])color\s*:\s*(#[0-9a-fA-F]{6})")


def neon_text(text):
    k = [0]

    def sub(m):
        v = NEON_MAP.get(m.group(1).lower())
        if not v:
            return m.group(0)
        k[0] += 1
        return "color:" + v
    return NEON_RE.sub(sub, text), k[0]



def process(text):
    total = 0

    def css_block(m):
        nonlocal total
        new, n = fix_decls(m.group(2)); total += n
        return m.group(1) + new + m.group(3)
    # khối CSS trong <style>...</style>
    def style_tag(m):
        inner = re.sub(r"(\{)([^{}]*)(\})", css_block, m.group(2))
        return m.group(1) + inner + m.group(3)
    text = re.sub(r"(<style[^>]*>)(.*?)(</style>)", style_tag, text, flags=re.S)

    def inline(m):
        nonlocal total
        new, n = fix_decls(m.group(2)); total += n
        return m.group(1) + new + m.group(3)
    text = re.sub(r'(style\s*=\s*")([^"]*)(")', inline, text)
    text, k = neon_text(text)
    return text, total + k


if __name__ == "__main__":
    f = Path(sys.argv[1]); apply = "--apply" in sys.argv
    src = f.read_text(encoding="utf-8")
    new, n = process(src)
    print(f"{f.name}: {n} thay đổi" + (" (ÁP DỤNG)" if apply else " (chạy thử, chưa ghi)"))
    if apply and n:
        bak = Path(r"C:\Users\tuanv\AppData\Local\Temp\claude\2ce6a041c07f62d5\5a2a645e-f429-4cd9-beac-82f1c35af514\scratchpad\view_backups")
        bak.mkdir(parents=True, exist_ok=True)
        shutil.copy2(f, bak / (f.parent.name + "__" + f.name + ".bak"))
        f.write_text(new, encoding="utf-8")
