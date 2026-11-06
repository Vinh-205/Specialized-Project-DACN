// Dựng hình "laptop mổ xẻ" kiểu isometric (SVG) bằng bộ kit Anatomy (MIT, xem LICENSE-anatomy.txt).
// Chạy:  node Tools/LaptopTeardown/build.mjs   →  ghi wwwroot/preview/laptop-teardown.html
// Mỗi lớp (vỏ đáy, pin, mainboard, quạt/RAM/SSD/tản nhiệt, mặt phím + màn hình) là một nhóm SVG; script nhỏ trong trang
// đẩy các nhóm lên cao theo vị trí cuộn chuột và làm mờ phần không được nhắc tới.
import { writeFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import * as k from "./iso-kit.mjs";

const HERE = dirname(fileURLToPath(import.meta.url));
const OUT = join(process.env.TEMP || HERE, "laptop-teardown-preview.html");

const WIDTH = 720, HEIGHT = 820;
const X0 = -120, Y0 = -85, W = 240, D = 170, X1 = X0 + W, Y1 = Y0 + D;
const CASE = { x: X0, y: Y0, w: W, d: D, r: 14 };

/* tầng z lúc lắp ráp */
const BOT_H = 14, BAT_Z = 14, BAT_H = 6, PCB_Z = 14, PCB_H = 2.4, TOP_PART = PCB_Z + PCB_H;
const DECK_Z = 28, DECK_H = 4, KEY_Z = DECK_Z + DECK_H;
const SCR_Z = DECK_Z, SCR_H = 150, SCR_T = 5;

/* độ nâng khi tách rời (đơn vị thế giới) */
const LIFT = { battery: 46, board: 100, heatsink: 80, fan: 100, ram: 70, ssd: 84, deck: 250 };

const P = k.fitProjection(
  [...k.boxCorners(CASE, -3, 0), ...k.boxCorners(CASE, DECK_Z + LIFT.deck, SCR_Z + LIFT.deck + SCR_H)],
  WIDTH, HEIGHT, { pad: 34, azimuth: 50 },
);
const RISE = k.screenRise(1, P);

const at = (p) => k.iso(p, P);
const S = (paths, style) => k.solidSvg(paths, style);
const L = (d, style) => k.lineSvg(d, style);
const Dt = (pts, style) => k.dotsSvg(pts, style);
const byDepth = (points) => [...points].sort((a, b) => k.depthOf([a[0], a[1], 0], P) - k.depthOf([b[0], b[1], 0], P));
const planeOf = (y) => k.sideMatrix([0, y], 0, P, "left");
const flat = (x0, z0, x1, z1, r, steps = 6) => k.pathOf(k.roundedPlan({ x: x0, y: z0, w: x1 - x0, d: z1 - z0, r }, steps).map(([u, z]) => [u, -z]), true);
let seed = 11;
const rnd = () => { seed = (seed * 16807) % 2147483647; return seed / 2147483647; };
const group = (attrs, children) => `<g ${attrs}>${children.join("")}</g>`;

/* ───────── vỏ đáy ───────── */
function bottomCase() {
  const out = [`<path class="iso-halo" d="${k.haloOf(CASE, 0, BOT_H, P)}"/>`];
  for (const [x, y] of byDepth(k.corners(CASE, 26))) {
    out.push(S(k.cylinder(x, y, 7, -3, 3, P, 24), { tone: "lo" }));
  }
  out.push(S(k.slabOf(CASE, 0, BOT_H, P, 10, 1.4), { tone: "mid" }));
  out.push(L(k.planOutline(k.insetPlan(CASE, 5), BOT_H, P), { tone: "lo" }));
  // khe tản nhiệt
  const vents = [];
  for (let i = 0; i < 9; i++) vents.push(k.lineOnTop([X1 - 60 + i * 5, Y0 + 14], [X1 - 60 + i * 5, Y0 + 50], BOT_H, P));
  out.push(L(vents.join(""), { tone: "faint" }));
  const screws = byDepth([[X0 + 12, Y0 + 12], [X1 - 12, Y0 + 12], [X0 + 12, Y1 - 12], [X1 - 12, Y1 - 12], [0, Y0 + 12], [0, Y1 - 12]]);
  out.push(L(screws.map(([x, y]) => k.ring(x, y, 2.6, BOT_H, P, 14)).join(""), { tone: "lo" }));
  out.push(Dt(screws.map(([x, y]) => at([x, y, BOT_H])), { size: 0.5 }));
  return out.join("");
}

/* ───────── pin ───────── */
function battery() {
  const B = { x: -100, y: 14, w: 200, d: 62, r: 5 };
  const out = [S(k.slabOf(B, BAT_Z, BAT_H, P, 5, 0.8), { tone: "mid" })];
  const seams = [-50, 0, 50].map((x) => k.lineOnTop([x, B.y + 3], [x, B.y + B.d - 3], BAT_Z + BAT_H, P));
  out.push(L(seams.join(""), { tone: "lo" }));
  out.push(L(k.planOutline(k.insetPlan(B, 4), BAT_Z + BAT_H, P), { tone: "faint" }));
  const label = { x: -80, y: B.y + 22, w: 74, d: 18, r: 2 };
  out.push(S(k.slabOf(label, BAT_Z + BAT_H, 0.6, P, 3), { tone: "lo", crease: "none" }));
  out.push(L([0, 1, 2].map((i) => k.lineOnTop([label.x + 5, label.y + 5 + i * 4], [label.x + 56 - i * 8, label.y + 5 + i * 4], BAT_Z + BAT_H + 0.6, P)).join(""), { tone: "lo", free: true }));
  out.push(S(k.slabOf({ x: -108, y: B.y + 22, w: 9, d: 18, r: 1.5 }, BAT_Z + 1, 4, P, 3, 0.3), { tone: "mid" }));
  out.push(Dt([[-106, B.y + 27], [-106, B.y + 31], [-106, B.y + 35]].map(([x, y]) => at([x, y, BAT_Z + 5])), { size: 0.45 }));
  return out.join("");
}

/* ───────── mainboard (đế PCB, CPU, tụ, khe cắm) ───────── */
function board() {
  const B = { x: -104, y: -78, w: 208, d: 140, r: 6 };
  const out = [S(k.slabOf(B, PCB_Z, PCB_H, P, 6, 0.6), { tone: "mid" })];
  // đường mạch
  const tr = [];
  for (let i = 0; i < 30; i++) {
    let x = X0 + 20 + rnd() * 200, y = Y0 + 10 + rnd() * 130; const pts = [[x, y]];
    for (let j = 0; j < 3; j++) { if (j % 2) y += (rnd() - 0.5) * 60; else x += (rnd() - 0.5) * 70; x = Math.max(X0 + 18, Math.min(X1 - 18, x)); y = Math.max(Y0 + 8, Math.min(Y0 + 140, y)); pts.push([x, y]); }
    tr.push(k.onTop(pts, TOP_PART, P));
  }
  out.push(L(tr.join(""), { tone: "faint" }));
  // gói CPU
  out.push(S(k.slabOf({ x: 4, y: -48, w: 36, d: 32, r: 2 }, TOP_PART, 2.6, P, 3, 0.4), { tone: "mid" }));
  out.push(S(k.slabOf({ x: 12, y: -42, w: 20, d: 20, r: 1 }, TOP_PART + 2.6, 0.8, P, 2), { tone: "lo", crease: "none" }));
  // tụ và chip nhỏ
  const caps = byDepth([[-90, -66], [-80, -66], [-70, -66], [-90, -54], [-80, -54], [-70, -54], [-52, -62], [-44, -62], [-36, -62], [60, 14], [70, 14], [80, 14], [60, 24]]);
  caps.forEach(([x, y]) => out.push(S(k.cylinder(x, y, 2.6, TOP_PART, 4, P, 14), { tone: "mid" })));
  [[-30, -20, 18, 14], [-56, 4, 14, 14], [92, -18, 12, 18], [50, -10, 10, 8]].forEach(([x, y, w, d]) => out.push(S(k.slabOf({ x, y, w, d, r: 1.2 }, TOP_PART, 1.8, P, 2), { tone: "lo" })));
  // khe RAM (2 khe), khe SSD, đầu nối màn hình
  [{ x: -98, y: 10 }, { x: -98, y: 26 }].forEach((s) => out.push(S(k.slabOf({ x: s.x, y: s.y, w: 66, d: 6, r: 1 }, TOP_PART, 3, P, 2), { tone: "lo" })));
  out.push(S(k.slabOf({ x: 26, y: 38, w: 70, d: 7, r: 1 }, TOP_PART, 3, P, 2), { tone: "lo" }));
  out.push(S(k.slabOf({ x: -40, y: -80, w: 40, d: 5, r: 1 }, TOP_PART, 3, P, 2), { tone: "mid" }));
  const screws = byDepth([[-96, -70], [96, -70], [-96, 54], [96, 54], [0, 54]]);
  out.push(L(screws.map(([x, y]) => k.ring(x, y, 2.4, TOP_PART, P, 14)).join(""), { tone: "lo" }));
  out.push(Dt(screws.map(([x, y]) => at([x, y, TOP_PART])), { size: 0.5 }));
  return out.join("");
}

/* ───────── linh kiện rời ───────── */
function heatsink() {
  const z = TOP_PART, H = 8;
  const out = [S(k.slabOf({ x: -2, y: -54, w: 46, d: 42, r: 3 }, z, H, P, 3, 0.8), { tone: "mid" })];
  const fins = [];
  for (let i = 0; i < 10; i++) fins.push(k.lineOnTop([2 + i * 4, -50], [2 + i * 4, -16], z + H, P));
  out.push(L(fins.join(""), { tone: "lo" }));
  // ống dẫn nhiệt tới quạt
  out.push(S(k.slabOf({ x: 42, y: -38, w: 40, d: 8, r: 4 }, z + 2, 4, P, 4, 0.4), { tone: "hi" }));
  out.push(L(k.lineOnTop([44, -34], [80, -34], z + 6, P), { tone: "faint" }));
  return out.join("");
}
function fan() {
  const z = TOP_PART, cx = 82, cy = -52;
  const out = [S(k.slabOf({ x: cx - 26, y: cy - 26, w: 52, d: 52, r: 12 }, z, 8, P, 8, 0.8), { tone: "mid" })];
  out.push(L(k.ring(cx, cy, 21, z + 8, P, 40), { tone: "lo" }));
  let blade = "";
  for (let i = 0; i < 9; i++) {
    const a = (i / 9) * Math.PI * 2, f = (r, da, w) => [Math.cos(a + da) * r, Math.sin(a + da) * r];
    const p0 = f(6, 0), p1 = f(19, 0.55), p2 = f(19, 0.85), p3 = f(7, 0.35);
    blade += `M${p0[0].toFixed(2)} ${p0[1].toFixed(2)}L${p1[0].toFixed(2)} ${p1[1].toFixed(2)}L${p2[0].toFixed(2)} ${p2[1].toFixed(2)}L${p3[0].toFixed(2)} ${p3[1].toFixed(2)}Z`;
  }
  out.push(`<g transform="${k.topMatrix([cx, cy], z + 8, P)}"><g class="lp-fan"><path class="lp-blade" d="${blade}"/><animateTransform attributeName="transform" type="rotate" from="0" to="360" dur="0.9s" repeatCount="indefinite"/></g></g>`);
  out.push(S(k.cylinder(cx, cy, 6, z + 8, 2, P, 20, 0.3), { tone: "hi" }));
  out.push(Dt([at([cx, cy, z + 10])], { size: 0.6 }));
  out.push(S(k.slabOf({ x: cx - 2, y: cy + 24, w: 14, d: 7, r: 1.5 }, z, 6, P, 3), { tone: "lo" }));
  return out.join("");
}
function ramStick(y) {
  const z = TOP_PART, X = -96;
  const out = [S(k.slabOf({ x: X, y, w: 66, d: 12, r: 1.4 }, z + 1, 2.2, P, 2, 0.3), { tone: "mid" })];
  for (let i = 0; i < 4; i++) out.push(S(k.slabOf({ x: X + 6 + i * 14, y: y + 2, w: 11, d: 8, r: 0.8 }, z + 3.2, 1.4, P, 2), { tone: "lo" }));
  out.push(L(k.lineOnTop([X + 2, y + 11], [X + 64, y + 11], z + 3.2, P), { tone: "hi" }));
  return out.join("");
}
function ssd() {
  const z = TOP_PART, X = 28, Y = 20;
  const out = [S(k.slabOf({ x: X, y: Y, w: 62, d: 18, r: 2 }, z + 1, 1.8, P, 3, 0.3), { tone: "mid" })];
  out.push(S(k.slabOf({ x: X + 6, y: Y + 3, w: 24, d: 12, r: 1 }, z + 2.8, 1.2, P, 2), { tone: "lo" }));
  out.push(S(k.slabOf({ x: X + 34, y: Y + 3, w: 22, d: 12, r: 1 }, z + 2.8, 1.2, P, 2), { tone: "lo", crease: "none" }));
  out.push(L(k.ring(X + 59, Y + 9, 1.8, z + 2.8, P, 12), { tone: "hi" }));
  return out.join("");
}


/* ───────── nắp màn hình lật quanh bản lề ─────────
   Nắp là một tấm dày quay quanh trục x ở mép sau. Bộ kit chỉ dựng được khối đứng thẳng, nên mỗi góc mở là một khung hình
   dựng bằng cách chiếu 8 đỉnh của tấm; trang chọn khung theo vị trí cuộn. Góc 0° = nắp úp phẳng lên bàn phím, 105° = mở ngửa ra sau một chút. */
const LID_FRAMES = 16, LID_MAX = 105;
const VIEW = [k.depthOf([1, 0, 0], P) - k.depthOf([0, 0, 0], P), k.depthOf([0, 1, 0], P) - k.depthOf([0, 0, 0], P), k.depthOf([0, 0, 1], P) - k.depthOf([0, 0, 0], P)];
const dotv = (a, c) => a[0] * c[0] + a[1] * c[1] + a[2] * c[2];
function lidFrame(deg, index) {
  const th = (deg * Math.PI) / 180, c = Math.cos(th), sn = Math.sin(th);
  const H = [0, Y0 + 7, KEY_Z + 4], T = SCR_T, L = SCR_H, hw = W / 2;
  const pt = (x, v, o) => [H[0] + x, H[1] + v * c - o * sn, H[2] + v * sn + o * c];
  const sc = [0, sn, -c];                                  // pháp tuyến mặt màn hình
  const faces = [
    { n: sc, q: [[-hw, 0, 0], [hw, 0, 0], [hw, L, 0], [-hw, L, 0]], screen: true },
    { n: [0, -sn, c], q: [[-hw, 0, T], [hw, 0, T], [hw, L, T], [-hw, L, T]], outer: true },
    { n: [0, -c, -sn], q: [[-hw, 0, 0], [hw, 0, 0], [hw, 0, T], [-hw, 0, T]] },
    { n: [0, c, sn], q: [[-hw, L, 0], [hw, L, 0], [hw, L, T], [-hw, L, T]] },
    { n: [-1, 0, 0], q: [[-hw, 0, 0], [-hw, L, 0], [-hw, L, T], [-hw, 0, T]] },
    { n: [1, 0, 0], q: [[hw, 0, 0], [hw, L, 0], [hw, L, T], [hw, 0, T]] },
  ];
  const out = [];
  const vis = faces.filter((f) => dotv(f.n, VIEW) > 0.001).sort((a, b) => a.q.reduce((t, p) => t + k.depthOf(pt(...p), P), 0) - b.q.reduce((t, p) => t + k.depthOf(pt(...p), P), 0));
  const basis = (o) => { const P0 = k.iso(pt(0, 0, o), P), Pu = k.iso(pt(1, 0, o), P), Pv = k.iso(pt(0, 1, o), P); return `matrix(${(Pu[0] - P0[0]).toFixed(4)} ${(Pu[1] - P0[1]).toFixed(4)} ${(Pv[0] - P0[0]).toFixed(4)} ${(Pv[1] - P0[1]).toFixed(4)} ${P0[0].toFixed(2)} ${P0[1].toFixed(2)})`; };
  const rect = (x0, v0, x1, v1, r) => k.pathOf(k.roundedPlan({ x: x0, y: v0, w: x1 - x0, d: v1 - v0, r }, 5), true);
  for (const f of vis) {
    const d = k.pathOf(f.q.map((p) => k.iso(pt(...p), P)), true);
    const bucket = Math.max(0, Math.min(3, Math.floor(((f.n[0] * 0.5 + f.n[1] * 0.5 + f.n[2] * 0.3) + 1) / 2 * 4)));
    out.push(f.n[2] > 0.6 ? `<path class="iso-top" d="${d}"/>` : `<path class="iso-shade" data-shade="${bucket}" d="${d}"/>`);
    out.push(`<path class="iso-line" data-tone="lo" d="${d}"/>`);
    if (f.screen) {
      const gx0 = -hw + 8, gx1 = hw - 8, g0 = 9, g1 = L - 8;
      const g = [`<g transform="${basis(0)}">`, `<path class="lp-glass" d="${rect(gx0, g0, gx1, g1, 4)}"/>`, `<path class="lp-bar" d="${rect(gx0, g1 - 14, gx1, g1, 4)}"/>`,
        `<path class="lp-side" d="${rect(gx0 + 4, g0 + 4, gx0 + 40, g1 - 18, 2)}"/>`];
      [[0, "lp-t1"], [1, "lp-t2"], [2, "lp-t3"]].forEach(([i, cl]) => g.push(`<path class="${cl}" d="${rect(gx0 + 48 + i * 52, g1 - 56, gx0 + 48 + i * 52 + 46, g1 - 22, 3)}"/>`));
      for (let r = 0; r < 6; r++) {
        g.push(`<path class="lp-row${r % 2 ? " lp-alt" : ""}" d="${rect(gx0 + 48, g1 - 70 - r * 12, gx1 - 6, g1 - 62 - r * 12, 1.6)}"/>`);
        g.push(`<path class="lp-ink" d="${rect(gx0 + 54, g1 - 68 - r * 12, gx0 + 54 + 30 + (r * 17) % 40, g1 - 64 - r * 12, 1)}"/>`);
      }
      g.push(`<path class="iso-line" data-tone="lo" d="${rect(gx0, g0, gx1, g1, 4)}"/>`, `<circle class="lp-cam" cx="0" cy="${L - 4}" r="1.4"/>`, `</g>`);
      out.push(g.join(""));
    }
    if (f.outer) out.push(`<g transform="${basis(T)}"><circle class="lp-logo" cx="0" cy="${L / 2}" r="15"/></g>`);
  }
  return `<g class="lp-lidf" data-f="${index}" data-deg="${deg.toFixed(1)}"${index === LID_FRAMES - 1 ? "" : ' style="display:none"'}>${out.join("")}</g>`;
}
const lidFrames = Array.from({ length: LID_FRAMES }, (_, i) => ({ deg: (i / (LID_FRAMES - 1)) * LID_MAX, i }));
const lidBack = lidFrames.filter((f) => f.deg >= 80).map((f) => lidFrame(f.deg, f.i)).join("");   // nắp dựng đứng: nằm sau bàn phím
const lidFront = lidFrames.filter((f) => f.deg < 80).map((f) => lidFrame(f.deg, f.i)).join("");   // nắp đang úp xuống: nằm trên bàn phím

/* ───────── mặt phím + màn hình ───────── */
const ROWS = [
  Array(14).fill(1), Array(14).fill(1), Array(14).fill(1), Array(14).fill(1), [1.5, 1.5, 1.5, 5, 1.5, 1.5, 1.5],
];
function deck() {
  const out = [`<path class="iso-halo" d="${k.haloOf(CASE, DECK_Z, DECK_H, P)}"/>`];
  // thân bàn phím
  out.push(S(k.slabOf(CASE, DECK_Z, DECK_H, P, 10, 1.4), { tone: "mid" }));
  out.push(L(k.planOutline(k.insetPlan(CASE, 5), KEY_Z, P), { tone: "faint" }));
  const KB = { x: -108, y: -66, w: 216, d: 74, r: 4 };
  out.push(S(k.slabOf(KB, KEY_Z, 0.8, P, 3), { tone: "lo", crease: "none" }));
  out.push(lidBack);
  const keys = [];
  const U = 15, KY = -62, RH = 14;
  ROWS.forEach((row, r) => { let x = KB.x + 4; row.forEach((w) => { keys.push({ x, y: KY + r * RH, w: w * U - 1.6 }); x += w * U; }); });
  for (const key of keys.sort((a, b) => k.depthOf([a.x, a.y, 0], P) - k.depthOf([b.x, b.y, 0], P))) {
    out.push(S(k.slabOf({ x: key.x, y: key.y, w: key.w, d: 12, r: 1.6 }, KEY_Z + 0.8, 2.4, P, 2, 0.3), { tone: "mid", crease: "faint" }));
  }
  const TP = { x: -36, y: 24, w: 72, d: 46, r: 4 };
  out.push(S(k.slabOf(TP, KEY_Z, 0.9, P, 4, 0.3), { tone: "lo" }));
  out.push(L(k.planOutline(k.insetPlan(TP, 2.5), KEY_Z + 0.9, P), { tone: "faint" }));
  [-1, 1].forEach((s) => out.push(Dt(k.dotGrid(s > 0 ? 78 : -100, 30, 5, 5, 4, KEY_Z, P), { size: 0.4 })));
  out.push(S(k.cylinder(X1 - 14, Y0 + 12, 4, KEY_Z, 1.6, P, 18, 0.3), { tone: "hi" }));
  out.push(Dt([at([X1 - 14, Y0 + 12, KEY_Z + 1.6])], { size: 0.7, pulse: false }));
  [[X0 + 24], [X1 - 24]].forEach(([x]) => out.push(S(k.slabOf({ x: x - 8, y: Y0 + 5, w: 16, d: 9, r: 1.5 }, KEY_Z, 6, P, 3, 0.3), { tone: "lo" })));
  out.push(lidFront);
  return out.join("");
}

/* ───────── lắp bản vẽ ───────── */
const layers = [
  { id: "case", cls: "", stage: 4, dz: 0, a: 0, b: 1, html: bottomCase() },
  { id: "battery", stage: 4, dz: LIFT.battery, a: 0.62, b: 1.0, html: battery() },
  { id: "board", stage: 3, dz: LIFT.board, a: 0.46, b: 0.82, html: board() },
  ...[
    ["ram1", ramStick(10), LIFT.ram], ["ram2", ramStick(26), LIFT.ram], ["ssd", ssd(), LIFT.ssd], ["heatsink", heatsink(), LIFT.heatsink], ["fan", fan(), LIFT.fan],
  ].map(([id, html, dz], i) => ({ id, stage: 2, dz, base: "board", a: 0.26 + i * 0.03, b: 0.6 + i * 0.01, html })),
  { id: "deck", stage: 1, dz: LIFT.deck, a: 0.14, b: 0.45, html: deck() },
];
// thứ tự vẽ: lớp thấp trước, lớp cao sau; các chi tiết rời vẽ sau mainboard
const svgBody = layers.map((l) => `<g class="lp-layer" data-id="${l.id}" data-stage="${l.stage}" data-dz="${l.dz}" data-a="${l.a}" data-b="${l.b}"${l.base ? ` data-base="${l.base}"` : ""}>${l.html}</g>`).join("");
const figure = k.figureSvg({ width: WIDTH, height: HEIGHT, label: "Laptop được tách thành các lớp: vỏ đáy, pin, mainboard, quạt, RAM, SSD, mặt phím và màn hình", body: svgBody, className: "lp-svg" });


/* ───────── cảnh bay qua 4 trạm ───────── */
const G = 230;
const P2 = k.fitProjection([...k.boxCorners({ x: -80, y: -80, w: 160, d: 160, r: 10 }, -8, 110)], 720, 600, { pad: 40, azimuth: 50 });
const at2 = (p) => k.iso(p, P2);
const DV = (() => { const a = at2([G, 0, 0]), b = at2([0, 0, 0]); return [a[0] - b[0], a[1] - b[1]]; })();
const plane2 = (y) => k.sideMatrix([0, y], 0, P2, "left");
const S2 = (paths, style) => k.solidSvg(paths, style);
function platform(cx) {
  return [`<path class="iso-halo" d="${k.haloOf({ x: cx - 62, y: -62, w: 124, d: 124, r: 62 }, -6, 6, P2)}"/>`,
    S2(k.cylinder(cx, 0, 62, -6, 6, P2, 60, 1), { tone: "mid" }),
    L(k.ring(cx, 0, 54, 0, P2, 56), { tone: "lit" }), L(k.ring(cx, 0, 46, 0, P2, 56), { tone: "faint" })].join("");
}
function stClipboard(cx) {
  const out = [S2(k.slabOf({ x: cx - 30, y: -6, w: 60, d: 7, r: 2 }, 0, 80, P2, 3, 0.8), { tone: "mid" })];
  const g = [`<g transform="${plane2(1)}">`, `<path class="lp-paper" d="${flat(cx - 24, 8, cx + 24, 68, 2)}"/>`];
  for (let i = 0; i < 4; i++) {
    g.push(`<path class="${i < 2 ? "lp-boxok" : "lp-box"}" d="${flat(cx - 19, 52 - i * 13, cx - 12, 59 - i * 13, 1)}"/>`);
    g.push(`<path class="lp-ink" d="${flat(cx - 7, 54 - i * 13, cx + 19 - (i % 2) * 6, 57 - i * 13, 0.8)}"/>`);
  }
  g.push("</g>");
  out.push(g.join(""));
  out.push(S2(k.slabOf({ x: cx - 11, y: -8, w: 22, d: 11, r: 2 }, 72, 10, P2, 3, 0.4), { tone: "hi" }));
  return out.join("");
}
function stMagnifier(cx) {
  const out = [S2(k.slabOf({ x: cx - 18, y: -8, w: 36, d: 16, r: 4 }, 0, 7, P2, 4, 0.6), { tone: "mid" }),
    S2(k.slabOf({ x: cx - 3, y: -3, w: 6, d: 6, r: 1.5 }, 7, 22, P2, 3), { tone: "lo" })];
  const lz = 62;
  out.push(`<g transform="${plane2(3)}"><circle class="lp-lens" cx="${cx}" cy="${-lz}" r="27"/>
    <path class="lp-handle" d="M${cx + 19} ${-(lz - 19)}L${cx + 40} ${-(lz - 40)}"/>
    <path class="lp-glint" d="M${cx - 18} ${-(lz + 8)}A20 20 0 0 1 ${cx - 6} ${-(lz + 19)}"/>
    <path class="lp-ink" d="${flat(cx - 14, lz - 3, cx + 12, lz - 1, 0.8)}${flat(cx - 14, lz + 5, cx + 4, lz + 7, 0.8)}${flat(cx - 14, lz - 11, cx + 8, lz - 9, 0.8)}"/></g>`);
  return out.join("");
}
function gearRing(cx, cy, n, ro, ri, rot) {
  const pts = [], st = (Math.PI * 2) / n;
  for (let i = 0; i < n; i++) for (const [r, da] of [[ri, 0], [ro, 0.18], [ro, 0.42], [ri, 0.6]]) { const a = i * st + da * st + rot; pts.push([cx + Math.cos(a) * r, cy + Math.sin(a) * r]); }
  return pts;
}
function stGears(cx) {
  const bc = [cx - 20, 8], sc = [cx + 30, -6], out = [];
  const frames = (name, c, n, ro, ri, h) => {
    for (let f = 0; f < 6; f++) {
      const rot = ((Math.PI * 2) / n) * (f / 6);
      out.push(`<g class="lp-gf" data-gear="${name}" data-f="${f}"${f ? ' style="display:none"' : ""}>${S2(k.extrude(gearRing(c[0], c[1], n, ro, ri, rot), 0, h, P2, { convex: false, bevel: 0.6 }), { tone: name === "big" ? "mid" : "hi" })}</g>`);
    }
  };
  frames("big", bc, 12, 38, 31, 14); frames("small", sc, 8, 21, 15.5, 14);
  out.push(S2(k.cylinder(bc[0], bc[1], 9, 14, 3, P2, 24, 0.4), { tone: "hi" }), L(k.ring(bc[0], bc[1], 22, 14, P2, 40), { tone: "lo" }));
  out.push(S2(k.cylinder(sc[0], sc[1], 5, 14, 3, P2, 18, 0.4), { tone: "mid" }));
  return out.join("");
}
function stCoin(cx) {
  const out = [S2(k.cylinder(cx, 0, 44, 0, 10, P2, 56, 1), { tone: "mid" }), L(k.ring(cx, 0, 36, 10, P2, 48), { tone: "lo" })];
  out.push(L(k.onTop([[cx - 20, 2], [cx - 6, 16], [cx + 22, -16]], 10, P2), { tone: "green", className: "lp-check" }));
  out.push(Dt([[cx + 52, -30], [cx - 54, 20], [cx + 40, 44], [cx - 40, -42]].map(([x, y]) => at2([x, y, 14])), { size: 0.9, tone: "hi" }));
  return out.join("");
}
const STATIONS = [stClipboard, stMagnifier, stGears, stCoin];
let worldGrid = [];
for (let x = -300; x <= G * 3 + 300; x += 46) worldGrid.push(k.lineOnTop([x, -150], [x, 150], -6, P2));
for (let y = -150; y <= 150; y += 46) worldGrid.push(k.lineOnTop([-300, y], [G * 3 + 300, y], -6, P2));
let drift = [];
for (let i = 0; i < 70; i++) drift.push(at2([-200 + rnd() * (G * 3 + 400), -140 + rnd() * 280, 10 + rnd() * 110]));
const journeyBody =
  `<g id="jgrid">${L(worldGrid.join(""), { tone: "faint" })}</g>` +
  `<g id="jworld">${STATIONS.map((fn, i) => { const c = at2([i * G, 0, 40]); return `<g class="lp-st" data-i="${i}" data-sx="${c[0].toFixed(1)}" data-sy="${c[1].toFixed(1)}">${platform(i * G)}${fn(i * G)}</g>`; }).join("")}</g>` +
  `<g id="jdrift">${Dt(drift, { size: 0.55, tone: "mid" })}</g>`;
const journeyFigure = k.figureSvg({ width: 720, height: 600, label: "Bốn trạm của một phiếu sửa chữa: chờ khám, đang kiểm tra, đang sửa chữa, hoàn thành", body: journeyBody, className: "lp-svg" });

const css = `
.lp .iso, .lp .iso[data-theme="light"], .lp .iso[data-theme="dark"] { background: none; }
.lp .iso[data-theme="light"] { --anatomy-paper:#f4f9fb; --anatomy-face:#ffffff; --anatomy-top:#ffffff; --anatomy-shade-0:#c9dbe4; --anatomy-shade-1:#d5e4eb; --anatomy-shade-2:#e0ecf1; --anatomy-shade-3:#ebf3f6;
  --anatomy-hi:#173b50; --anatomy-mid:#5d8195; --anatomy-lo:#9fbac8; --anatomy-faint:#c9dbe4; --anatomy-lit:#007eaf; --anatomy-halo:#6f93a6; --anatomy-halo-opacity:.28; --anatomy-dot:#173b50; }
.lp .iso[data-theme="dark"] { --anatomy-paper:#1a2829; --anatomy-face:#223133; --anatomy-top:#2a3c3f; --anatomy-shade-0:#16211f; --anatomy-shade-1:#1a2829; --anatomy-shade-2:#1f2f31; --anatomy-shade-3:#243537;
  --anatomy-hi:#d7ecf2; --anatomy-mid:#7fa6b6; --anatomy-lo:#4a6a76; --anatomy-faint:#2f4a52; --anatomy-lit:#5bbbe3; --anatomy-halo:#050909; --anatomy-halo-opacity:.8; --anatomy-dot:#d7ecf2; }
.lp-layer { transition: opacity .45s ease; will-change: transform; }
.lp-glass { fill: var(--anatomy-shade-0); }
.lp-bar { fill: var(--anatomy-lit); opacity: .9; }
.lp-side { fill: var(--anatomy-faint); }
.lp-t1 { fill: var(--anatomy-lit); opacity: .75; } .lp-t2 { fill: var(--anatomy-green); opacity: .8; } .lp-t3 { fill: #d99a2b; opacity: .85; }
.lp-row { fill: var(--anatomy-shade-2); } .lp-alt { fill: var(--anatomy-shade-1); } .lp-ink { fill: var(--anatomy-lit); opacity: .55; }
.lp-cam { fill: var(--anatomy-mid); }
.lp-blade { fill: var(--anatomy-mid); stroke: var(--anatomy-hi); stroke-width: .5px; stroke-linejoin: round; opacity: .9; }
.lp-lidf .lp-logo { fill: var(--anatomy-lit); }
.lp-paper { fill: var(--anatomy-face); } .lp-box { fill: var(--anatomy-faint); stroke: var(--anatomy-lo); stroke-width: .5px; } .lp-boxok { fill: var(--anatomy-green); opacity: .9; }
.lp-lens { fill: var(--anatomy-lit); fill-opacity: .12; stroke: var(--anatomy-lit); stroke-width: 5px; }
.lp-handle { stroke: var(--anatomy-hi); stroke-width: 7px; stroke-linecap: round; fill: none; }
.lp-glint { stroke: var(--anatomy-top); stroke-width: 2px; stroke-linecap: round; fill: none; opacity: .9; }
.lp-check { stroke-width: 7px !important; stroke-linejoin: round; }
.lp-st { transition: opacity .3s linear; }
#jworld, #jdrift, #jgrid { will-change: transform; }
`;
const html = `<!doctype html>
<html lang="vi" data-theme="light"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Laptop mổ xẻ (isometric) | IT Helpdesk</title>
<link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet">
<style>
:root { --bg:#e9f0f3; --surface:#ffffff; --ink:#173b50; --muted:#456b80; --primary:#007eaf; --tint:#e1f1f8; --border:#dfe9ee; }
html[data-theme="dark"] { --bg:#131c1d; --surface:#1e2a2b; --ink:#e7f2f2; --muted:#a7bec1; --primary:#5bbbe3; --tint:#1f4350; --border:#2c3e41; }
* { box-sizing:border-box; } body { margin:0; background:var(--bg); color:var(--ink); font-family:'Be Vietnam Pro', system-ui, sans-serif; }
.top { position:fixed; z-index:20; top:12px; left:12px; right:12px; height:52px; border-radius:999px; background:var(--surface); display:flex; align-items:center; justify-content:space-between; padding:0 12px 0 20px; }
.top span { color:var(--muted); font-size:12px; margin-left:10px; }
.btn { border:0; cursor:pointer; font:inherit; font-weight:600; font-size:13px; padding:9px 16px; border-radius:999px; background:var(--primary); color:#fff; } html[data-theme="dark"] .btn { color:#06222d; }
.intro { min-height:100vh; display:flex; flex-direction:column; justify-content:center; padding:120px 8vw 60px; max-width:900px; }
.intro h1 { font-size:clamp(34px,6vw,62px); line-height:1.08; margin:0 0 18px; letter-spacing:-.02em; } .intro p { font-size:17px; line-height:1.6; color:var(--muted); max-width:560px; }
.label { display:inline-block; font-size:12px; font-weight:700; letter-spacing:.08em; text-transform:uppercase; color:var(--primary); background:var(--tint); padding:6px 12px; border-radius:999px; margin-bottom:16px; width:fit-content; }
.pin { position:relative; height:520vh; } .stick { position:sticky; top:0; height:100vh; overflow:hidden; display:flex; align-items:center; justify-content:flex-end; padding:70px 4vw 20px 0; }
.stage.wide { aspect-ratio:720/600; max-width:70vw; }
.stage { height:calc(100vh - 100px); aspect-ratio:${WIDTH}/${HEIGHT}; max-width:62vw; } .stage .iso { height:100%; } .stage svg { width:100%; height:100%; display:block; }
.copy { position:absolute; left:8vw; bottom:12vh; width:min(400px,38vw); height:200px; pointer-events:none; }
.copy .step { position:absolute; left:0; bottom:0; opacity:0; transform:translateY(24px); transition:opacity .45s ease, transform .45s ease; }
.copy .step.on { opacity:1; transform:none; } .copy .no { font-size:13px; font-weight:700; color:var(--primary); margin-bottom:8px; }
.copy h2 { font-size:clamp(26px,3.4vw,40px); line-height:1.1; margin:0 0 10px; } .copy p { margin:0; font-size:16px; line-height:1.55; color:var(--muted); }
.bar { position:absolute; right:14px; top:50%; transform:translateY(-50%); display:flex; flex-direction:column; gap:10px; z-index:3; } .bar i { width:8px; height:8px; border-radius:50%; background:var(--border); transition:all .3s; } .bar i.on { background:var(--primary); height:26px; border-radius:6px; }
.outro { min-height:50vh; display:flex; flex-direction:column; align-items:center; justify-content:center; text-align:center; padding:60px 8vw; } .outro p { color:var(--muted); max-width:560px; line-height:1.6; }
@media (max-width:760px) { .stick { justify-content:center; padding:70px 0 0; align-items:flex-start; } .stage { max-width:96vw; height:auto; } .copy { left:16px; width:calc(100vw - 56px); bottom:5vh; } .copy .step { background:color-mix(in srgb, var(--bg) 86%, transparent); border-radius:16px; padding:12px 14px; } }
@media (prefers-reduced-motion: reduce) { .lp-layer, .copy .step { transition:none; } }
${k.ISO_CSS}
${css}
</style></head><body class="lp">
<header class="top"><div><b>IT Helpdesk</b><span>Bản xem trước · laptop mổ xẻ (isometric)</span></div><button class="btn" id="themeBtn">Sáng / Tối</button></header>
<section class="intro"><span class="label">Xem trước · tranh kỹ thuật isometric</span><h1>Bên trong một chiếc laptop, cuộn để tách từng lớp.</h1><p>Dựng bằng SVG theo phương pháp của bộ kit Anatomy (MIT). Mỗi lớp là một nhóm chi tiết; cuộn chuột để nâng từng lớp lên và xem từng linh kiện.</p></section>
<section class="pin" id="pin"><div class="stick">
  <div class="stage"><div class="iso" data-theme="light" id="iso">${figure}</div></div>
  <div class="copy" id="copy">
    <div class="step"><div class="no">01 · NGUYÊN VẸN</div><h2>Một chiếc laptop mang đến sửa</h2><p>Khách mô tả sự cố; kỹ thuật viên cần biết bên trong có gì để chẩn đoán đúng.</p></div>
    <div class="step"><div class="no">02 · MẶT PHÍM VÀ MÀN HÌNH</div><h2>Bàn phím, touchpad, màn hình</h2><p>Lỗi hiển thị, liệt phím hay không lên nguồn thường bắt đầu từ lớp này.</p></div>
    <div class="step"><div class="no">03 · QUẠT, RAM, SSD</div><h2>Linh kiện dễ thay</h2><p>Máy nóng, treo hoặc báo thiếu ổ đĩa: kiểm tra quạt tản nhiệt, thanh RAM và ổ SSD.</p></div>
    <div class="step"><div class="no">04 · MAINBOARD</div><h2>Bo mạch chủ</h2><p>Chip xử lý, tụ, khe cắm: nơi chẩn đoán lỗi phần cứng sâu nhất.</p></div>
    <div class="step"><div class="no">05 · PIN VÀ VỎ ĐÁY</div><h2>Pin và vỏ đáy</h2><p>Mọi linh kiện xuất kho đều được ghi lại cùng đơn giá để tính chi phí sửa chữa.</p></div>
  </div>
  <div class="bar" id="bar"><i></i><i></i><i></i><i></i><i></i></div>
</div></section>
<section class="pin" id="pin2" style="height:460vh"><div class="stick">
  <div class="stage wide"><div class="iso" data-theme="light" id="iso2">${journeyFigure}</div></div>
  <div class="copy" id="copy2">
    <div class="step"><div class="no">TRẠM 1 · CHỜ KHÁM</div><h2>Tiếp nhận phiếu</h2><p>Tạo phiếu, gắn thiết bị và khách hàng, ghi mô tả sự cố cùng mức ưu tiên.</p></div>
    <div class="step"><div class="no">TRẠM 2 · ĐANG KIỂM TRA</div><h2>Chẩn đoán</h2><p>KTV nhận ca, chọn triệu chứng; luật và học máy cùng xếp hạng mã lỗi gợi ý.</p></div>
    <div class="step"><div class="no">TRẠM 3 · ĐANG SỬA CHỮA</div><h2>Chốt lỗi và sửa</h2><p>Áp dụng giải pháp, xuất linh kiện có kiểm tra tồn kho và ghi đơn giá.</p></div>
    <div class="step"><div class="no">TRẠM 4 · HOÀN THÀNH</div><h2>Chốt phí, bàn giao</h2><p>Khách nhận máy và đánh giá dịch vụ qua liên kết công khai.</p></div>
  </div>
  <div class="bar" id="bar2"><i></i><i></i><i></i><i></i></div>
</div></section>
<section class="outro"><h2>Bạn thấy kiểu này thế nào?</h2><p>Đây là tranh minh họa dựng bằng mã nên có thể đổi màu theo hệ thống, nhẹ và sắc nét ở mọi kích thước màn hình.</p></section>
<script>
(function () {
  var root = document.documentElement, isos = [document.getElementById('iso'), document.getElementById('iso2')];
  try { var t = localStorage.getItem('hd_theme'); if (t) root.setAttribute('data-theme', t); } catch (e) {}
  function sync() { var th = root.getAttribute('data-theme') === 'dark' ? 'dark' : 'light'; isos.forEach(function (i) { i.setAttribute('data-theme', th); }); } sync();
  document.getElementById('themeBtn').onclick = function () { var n = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark'; root.setAttribute('data-theme', n); try { localStorage.setItem('hd_theme', n); } catch (e) {} sync(); };
  var RISE = ${RISE.toFixed(4)};
  var clamp = function (v, a, b) { return Math.max(a, Math.min(b, v)); }, ease = function (x) { x = clamp(x, 0, 1); return x * x * (3 - 2 * x); };
  var layers = [].slice.call(document.querySelectorAll('.lp-layer')).map(function (el) { return { el: el, id: el.dataset.id, stage: +el.dataset.stage, dz: +el.dataset.dz, a: +el.dataset.a, b: +el.dataset.b, base: el.dataset.base }; });
  var byId = {}; layers.forEach(function (l) { byId[l.id] = l; });
  var steps = [].slice.call(document.querySelectorAll('#copy .step')), dots = [].slice.call(document.querySelectorAll('#bar i'));
  var pin = document.getElementById('pin'), p = 0, target = 0, last = performance.now(), running = false, visible = false;
  var still = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  function stageOf(v) { return v < .1 ? 0 : v < .4 ? 1 : v < .62 ? 2 : v < .84 ? 3 : 4; }
  var lid = document.querySelector('.lp-lid'), hy = +lid.dataset.hinge;
  function apply() {
    var ls = .05 + .95 * ease(p / .1); lid.setAttribute('transform', 'translate(0 ' + hy + ') scale(1 ' + ls.toFixed(3) + ') translate(0 ' + (-hy) + ')');
    layers.forEach(function (l) { l.t = ease((p - l.a) / (l.b - l.a)); });
    var s = stageOf(p);
    layers.forEach(function (l) {
      var off = l.t * l.dz + (l.base ? byId[l.base].t * byId[l.base].dz : 0);
      l.el.setAttribute('transform', 'translate(0 ' + (-off * RISE).toFixed(2) + ')');
      l.el.style.opacity = (s === 0 || l.stage === s) ? 1 : .22;
    });
    steps.forEach(function (el, i) { el.classList.toggle('on', i === s); }); dots.forEach(function (el, i) { el.classList.toggle('on', i === s); });
  }
  function frame(now) {
    var dt = Math.min(.1, (now - last) / 1000); last = now;
    var r = pin.getBoundingClientRect(); target = clamp(-r.top / (r.height - innerHeight), 0, 1);
    p = still ? target : p + (target - p) * Math.min(1, dt * 7);
    apply();
    if (visible && Math.abs(target - p) > .0005) requestAnimationFrame(frame); else running = false;
  }
  function kick() { if (!running) { running = true; last = performance.now(); requestAnimationFrame(frame); } }
  new IntersectionObserver(function (e) { visible = e[0].isIntersecting; if (visible) kick(); }, { rootMargin: '100px' }).observe(pin);
  addEventListener('scroll', function () { if (visible) kick(); }, { passive: true }); addEventListener('resize', kick);
  apply();

  /* ── phần 2: bay qua bốn trạm theo cuộn chuột ── */
  var DV = [${DV[0].toFixed(2)}, ${DV[1].toFixed(2)}];
  var pin2 = document.getElementById('pin2'), jw = document.getElementById('jworld'), jd = document.getElementById('jdrift'), jg = document.getElementById('jgrid');
  var sts = [].slice.call(document.querySelectorAll('.lp-st')), gf = [].slice.call(document.querySelectorAll('.lp-gf'));
  var steps2 = [].slice.call(document.querySelectorAll('#copy2 .step')), dots2 = [].slice.call(document.querySelectorAll('#bar2 i'));
  var q = 0, qt = 0, last2 = performance.now(), run2 = false, vis2 = false;
  function apply2() {
    var u = q * 3, tx = -DV[0] * u, ty = -DV[1] * u, mod = function (a, n) { return ((a % n) + n) % n; };
    jw.setAttribute('transform', 'translate(' + tx.toFixed(1) + ' ' + ty.toFixed(1) + ')'); jg.setAttribute('transform', 'translate(' + tx.toFixed(1) + ' ' + ty.toFixed(1) + ')');
    jd.setAttribute('transform', 'translate(' + (tx * 1.3).toFixed(1) + ' ' + (ty * 1.3).toFixed(1) + ')');
    sts.forEach(function (el, i) {
      var d = Math.abs(i - u), sc = 1 + .07 * (1 - Math.min(1, d)), sx = +el.dataset.sx, sy = +el.dataset.sy;
      el.style.opacity = clamp(1 - d * .75, .1, 1).toFixed(2);
      el.setAttribute('transform', 'translate(' + sx + ' ' + sy + ') scale(' + sc.toFixed(3) + ') translate(' + (-sx) + ' ' + (-sy) + ')');
    });
    var phase = q * 3 * 360 * 1.2, bi = Math.floor(mod(phase, 30) / 5), si = Math.floor(mod(-phase * 1.5 + 22.5, 45) / 7.5);
    gf.forEach(function (g) { var want = g.dataset.gear === 'big' ? bi : si, on = +g.dataset.f === want; if ((g.style.display !== 'none') !== on) g.style.display = on ? '' : 'none'; });
    var s2 = clamp(Math.round(u), 0, 3);
    steps2.forEach(function (el, i) { el.classList.toggle('on', i === s2); }); dots2.forEach(function (el, i) { el.classList.toggle('on', i === s2); });
  }
  function frame2(now) {
    var dt = Math.min(.1, (now - last2) / 1000); last2 = now;
    var r = pin2.getBoundingClientRect(); qt = clamp(-r.top / (r.height - innerHeight), 0, 1);
    q = still ? qt : q + (qt - q) * Math.min(1, dt * 7);
    apply2();
    if (vis2 && Math.abs(qt - q) > .0005) requestAnimationFrame(frame2); else run2 = false;
  }
  function kick2() { if (!run2) { run2 = true; last2 = performance.now(); requestAnimationFrame(frame2); } }
  new IntersectionObserver(function (e) { vis2 = e[0].isIntersecting; if (vis2) kick2(); }, { rootMargin: '100px' }).observe(pin2);
  addEventListener('scroll', function () { if (vis2) kick2(); }, { passive: true }); addEventListener('resize', kick2);
  apply2();
})();
</script></body></html>`;
writeFileSync(OUT, html);
console.log("đã ghi", OUT, `(${(html.length / 1024).toFixed(0)} KB)`);

const MANA_THEMES = `
.iso[data-theme="paper"] { --anatomy-paper:#f4f9fb; --anatomy-face:#ffffff; --anatomy-top:#ffffff; --anatomy-shade-0:#c9dbe4; --anatomy-shade-1:#d5e4eb; --anatomy-shade-2:#e0ecf1; --anatomy-shade-3:#ebf3f6;
  --anatomy-hi:#173b50; --anatomy-mid:#5d8195; --anatomy-lo:#9fbac8; --anatomy-faint:#c9dbe4; --anatomy-lit:#0978c5; --anatomy-halo:#6f93a6; --anatomy-halo-opacity:.28; --anatomy-dot:#173b50; --anatomy-green:#299475; }
`;

/* ───────── xuất cho trang Landing (Razor partial + CSS) ───────── */
const ROOT = join(HERE, "..", "..");
const figCss = `/* Sinh bởi Tools/LaptopTeardown/build.mjs: kit Anatomy (MIT) + kiểu riêng của hình laptop và hành trình. Không sửa tay. */\n${k.ISO_CSS}\n${css.replace(/\.lp \.iso/g, '.iso')}\n${MANA_THEMES}`;
writeFileSync(join(ROOT, "Views", "Shared", "_LaptopFigure.cshtml"),
  `<div class="iso lf-fig" data-theme="paper" id="lfLaptop" data-rise="${RISE.toFixed(4)}">${figure}</div>\n`);
writeFileSync(join(ROOT, "Views", "Shared", "_JourneyFigure.cshtml"),
  `<div class="iso lf-fig" data-theme="paper" id="lfJourney" data-dv="${DV[0].toFixed(2)},${DV[1].toFixed(2)}">${journeyFigure}</div>\n`);
writeFileSync(join(ROOT, "wwwroot", "css", "iso-figures.css"), figCss);
console.log("đã ghi partial và iso-figures.css");
