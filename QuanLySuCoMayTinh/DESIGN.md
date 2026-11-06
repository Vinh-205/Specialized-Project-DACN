---
version: alpha
name: IT Helpdesk — Lavender Dark
description: Design system for the IT Helpdesk (QuanLySuCoMayTinh) project. A glassmorphism-heavy, deep-space dark theme with a Lavender accent palette.
colors:
  primary: "#5B5BD6"
  primary-light: "#9898E8"
  primary-base: "#D3D3FF"
  primary-dark: "#3D3DAE"
  primary-darkest: "#1A1A48"
  bg-page: "#0D0D2B"
  border: "rgba(211,211,255,0.15)"
  text-primary: "#FFFFFF"
  text-secondary: "rgba(255,255,255,0.75)"
  text-muted: "rgba(255,255,255,0.5)"
  topbar-start: "#FF7A45"
  topbar-mid: "#FFA53E"
  topbar-end: "#FFC15E"
typography:
  hero:
    fontFamily: Inter
    fontSize: 64px
    fontWeight: 800
    lineHeight: 1.1
  heading-xl:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: 700
    lineHeight: 1.2
  heading-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: 700
    lineHeight: 1.3
  heading-md:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: 600
    lineHeight: 1.3
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: 400
    lineHeight: 1.7
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: 400
    lineHeight: 1.6
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: 400
    lineHeight: 1.5
  label:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: 600
    lineHeight: 1.4
spacing:
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 32px
  2xl: 48px
  3xl: 64px
  4xl: 96px
rounded:
  sm: 8px
  md: 12px
  lg: 16px
  xl: 24px
  full: 9999px
components:
  feature-card:
    backgroundColor: "{colors.primary-darkest}"
    rounded: "{rounded.lg}"
    textColor: "{colors.text-primary}"
  stat-item:
    backgroundColor: "{colors.primary-darkest}"
    textColor: "{colors.text-primary}"
  step-item:
    backgroundColor: "{colors.primary-darkest}"
    textColor: "{colors.text-secondary}"
  nav:
    backgroundColor: "{colors.bg-page}"
    textColor: "{colors.text-primary}"
    height: 64px
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.text-primary}"
    rounded: "{rounded.full}"
  button-cta:
    backgroundColor: "{colors.primary-light}"
    textColor: "{colors.text-primary}"
    rounded: "{rounded.full}"
  topbar:
    backgroundColor: "{colors.topbar-start}"
    textColor: "{colors.text-primary}"

---

# IT Helpdesk — Lavender Dark Design System

## Overview

IT Helpdesk là nền tảng quản lý sự cố máy tính dành cho kỹ thuật viên IT, trưởng phòng và nhân viên báo cáo sự cố. Giao diện mang phong cách **công nghệ cao, hiện đại** — không gian đêm sâu (deep space) kết hợp với hiệu ứng **Glassmorphism** và dải màu **Lavender** làm điểm nhấn thương hiệu.

**Cảm xúc mục tiêu:** Chuyên nghiệp, đáng tin cậy, hiện đại — như một hệ thống điều hành nhiệm vụ của tương lai.

**Nguyên tắc cốt lõi:**
- Ưu tiên khả năng đọc: Văn bản trắng nổi bật trên nền kính tối.
- Ít màu, đậm sắc: Chỉ dùng dải Lavender và trắng — tránh màu nguyên sắc.
- Chuyển động có chủ đích: Mọi animation phải phục vụ trải nghiệm, không trang trí thừa.
- Glass-over-space: Các thẻ/card luôn là lớp kính mờ nổi trên nền không gian tối.

## Colors

Bảng màu xoay quanh một dải **Lavender** duy nhất từ nhạt đến đậm, trên nền không gian tối (#0D0D2B).

- **Primary (#5B5BD6):** Vivid Lavender — dùng cho nút CTA, icon nổi bật, và các điểm nhấn tương tác chính.
- **Primary Light (#9898E8):** Mid Lavender — hover states, link colors, các element phụ.
- **Primary Base (#D3D3FF):** Pale Lavender — màu text gradient, badge, và viền nhẹ.
- **Primary Dark (#3D3DAE):** Deep Lavender — gradient kết thúc, trạng thái active.
- **Primary Darkest (#1A1A48):** Space Lavender — nền thẻ glassmorphism (kết hợp opacity 45%).
- **Background (#0D0D2B):** Deep Space — nền trang chủ, tạo độ sâu tối đa.
- **Text Primary (#FFFFFF):** Trắng thuần — tiêu đề, nội dung chính.
- **Text Secondary (rgba(255,255,255,0.75)):** Trắng mờ — mô tả, phụ đề.
- **Topbar Gradient:** Cam-vàng ấm (#FF7A45 → #FFA53E → #FFC15E) — chỉ dùng cho thanh topbar trong màn dashboard (sau đăng nhập).

**Liquid Color Blobs:** 5 quả cầu màu với `filter: blur(120px)` được ghim cố định (position: fixed; z-index: -1) tạo hiệu ứng hòa sắc sống động trong suốt quá trình cuộn trang.

## Typography

Font chữ duy nhất: **Inter** (Google Fonts). Thân thiện, hiện đại, dễ đọc ở mọi cỡ.

Tiêu đề hero dùng font-weight 800 với letter-spacing âm nhẹ để tạo cảm giác mạnh mẽ. Body text dùng line-height cao (1.6–1.7) để thoáng và dễ đọc dài.

Gradient text áp dụng cho các tiêu đề nổi bật: `background: linear-gradient(90deg, #D3D3FF, #9898E8); -webkit-background-clip: text`.

## Layout

- Max content width: **1280px**, căn giữa, padding ngang 32px.
- Dùng CSS Grid cho cards, Flexbox cho navigation và hero.
- **Responsive breakpoints:** sm (480px), md (768px), lg (1024px), xl (1280px).
- Feature grid: `repeat(auto-fill, minmax(280px, 1fr))`.
- Stats grid: 4 cột trên desktop, 2 cột trên tablet, 1 cột trên mobile.

## Elevation & Depth

Hiệu ứng độ sâu đạt được qua **Glassmorphism** thay vì shadow truyền thống:

- **Lớp 0 (Nền):** Deep space background + liquid blobs.
- **Lớp 1 (Nội dung):** Cards kính mờ (`backdrop-filter: blur(24px)`).
- **Lớp 2 (Floating):** Tooltips, dropdowns — tăng blur và shadow.
- **Lớp 3 (Modal/Overlay):** Overlay đen mờ, modal glassmorphism tăng cường.

Shadow glow được dùng cho hover: `box-shadow: 0 0 24px rgba(91,91,214,0.4)`.

## Shapes

Toàn bộ UI dùng bo góc cong (rounded corners) — không có góc vuông sắc:
- Buttons: `border-radius: 9999px` (pill).
- Cards: `border-radius: 16px`.
- Icons/Avatars: `border-radius: 50%`.
- Input fields: `border-radius: 12px`.

## Components

### Navigation Bar (`.land-nav`)
Sticky navbar (64px cao), nền kính mờ. Khi cuộn xuống thêm class `.nav-scrolled` để tăng blur. Logo dùng màu `primary-base`. Nav links dùng `text-secondary` và chuyển sang `text-primary` khi hover.

### Feature Card (`.feature-card`)
Card glassmorphism chuẩn. Icon 48x48px hình tròn với gradient accent. Hover: trượt lên 6px và xuất hiện glow. Stagger animation vào khi scroll.

### Step Item (`.step-item`)
Layout ngang (icon + text). Step badge hình tròn 32px gradient. Đường nối giữa các step dùng nét đứt lavender mờ.

### Stat Item (`.stat-item`)
Căn giữa. Con số lớn dùng gradient text và count-up animation (GSAP, 2s). Label text-secondary.

### Button Primary (`.btn-primary`)
Gradient accent, pill shape, hover: translateY(-2px) + glow shadow. Active: scale(0.97). Không dùng màu nguyên sắc đơn giản.

### Live Badge (`.hero-live-badge`)
Pill mờ với dot màu xanh lá nhấp nháy (pulse animation). Cho thấy hệ thống đang hoạt động real-time.

## Do's and Don'ts

**Do:**
- Luôn đặt text trên nền tối đủ tương phản (WCAG AA: 4.5:1 tối thiểu).
- Dùng glassmorphism cho mọi card và surface.
- Animation có easing tự nhiên (power2.out, back.out).
- Kết hợp liquid blob với glassmorphism để tạo chiều sâu thị giác.

**Don't:**
- Không dùng màu nguyên sắc (đỏ, xanh lá, vàng) trừ trạng thái semantic (error, success).
- Không dùng nền trắng hoặc nền sáng trong các section landing page.
- Không thêm border dày hoặc shadow cứng — luôn dùng border mỏng bán trong suốt.
- Không animate quá nhiều thứ cùng lúc — tối đa 3 element stagger cùng thời điểm.
- Không quên `prefers-reduced-motion` media query cho người dùng nhạy cảm với chuyển động.
