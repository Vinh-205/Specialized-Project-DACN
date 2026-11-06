/* Trang Welcome: laptop hoạt hình mở nắp khi vào trang, đóng nắp ở cuối; xoay đối xứng giữa các bước; linh vật CRT di chuyển theo từng bước. */
(function () {
    'use strict';
    var track = document.getElementById('track'), rig = document.getElementById('rig'), railDots = [].slice.call(document.querySelectorAll('#rail i')), glow = document.getElementById('glow');
    var marquee = document.querySelector('.wc-marquee'), mascot = document.getElementById('welcomeMascot'), say = document.getElementById('mascotSay'), caps = document.getElementById('wcCaps');
    var lidEl = document.getElementById('lid'), lcEl = document.getElementById('lc'), up = document.getElementById('stepUp'), down = document.getElementById('stepDown');
    if (!track || !rig || !lidEl) return;
    var copies = [].slice.call(document.querySelectorAll('.copy')), props = [].slice.call(document.querySelectorAll('.prop'));
    var navLinks = [].slice.call(document.querySelectorAll('#publicNav .pn-links a'));
    var still = matchMedia('(prefers-reduced-motion: reduce)').matches;
    var clamp = function (v) { return Math.max(0, Math.min(1, v)); }, smooth = function (x) { x = clamp(x); return x * x * (3 - 2 * x); }, ease = function (x) { x = clamp(x); return 1 - Math.pow(1 - x, 3); };

    /* [tiến độ, tâm x, tâm y (0..1 màn hình), phóng to, xoay Y (độ), xoay X (độ)] */
    var KEYS = [[0, .70, .54, 1, -12, 3], [.08, .70, .54, 1, -12, 3], [.17, .36, .56, 1, 14, 5], [.34, .36, .56, 1, 14, 5], [.48, .64, .56, 1.04, 194, 4], [.64, .64, .56, 1.04, 194, 4], [.78, .34, .56, 1.02, 372, 6], [.93, .34, .56, 1.02, 372, 6], [1, .5, .32, .56, 360, 2]];
    /* linh vật: [tiến độ, tâm x, tâm y] */
    var MKEYS = [[0, .5, .78], [.08, .5, .78], [.17, .84, .86], [.34, .84, .86], [.48, .10, .86], [.64, .10, .86], [.78, .85, .86], [.93, .85, .86], [1, .16, .74]];
    var SAYS = ['Xin chào! Mình là CRT.', 'Phiếu mới? Mình ghi lại ngay.', 'Để DeskAI gợi ý mã lỗi nhé.', 'Linh kiện có sẵn trong kho rồi!', 'Vào làm việc thôi!'];
    var STOPS = [0, .26, .57, .86, 1];

    function interp(keys, p) {
        var i = 0; while (i < keys.length - 2 && p > keys[i + 1][0]) i++;
        var a = keys[i], b = keys[i + 1], k = smooth((p - a[0]) / (b[0] - a[0]));
        return a.map(function (v, j) { return v + (b[j] - v) * k; });
    }
    function win(el, p) { var a = +el.dataset.a, b = +el.dataset.b, f = .035; return smooth((p - a) / f) * (1 - smooth((p - (b - f)) / f)); }
    /* ngưỡng đổi bước khớp với lúc nội dung thật sự đổi trên màn hình (giữa các khoảng chuyển) */
    function stageOf(p) { return p < .185 ? 0 : p < .42 ? 1 : p < .725 ? 2 : p < .92 ? 3 : 4; }
    function prog() { var r = track.getBoundingClientRect(); return clamp(-r.top / Math.max(1, r.height - innerHeight)); }
    function goto(p) { var r = track.getBoundingClientRect(), top = scrollY + r.top + p * (r.height - innerHeight); scrollTo({ top: top, behavior: still ? 'auto' : 'smooth' }); }

    /* nắp laptop: biến đổi affine quanh bản lề (không dùng phối cảnh nên không bao giờ giãn vô hạn) */
    var lastFace = 0, P = { BL: [180, 672], BR: [705, 598], TL: [78, 250], cL: [260, 143] };
    function setLid(deg, w) {
        var s = w / 1024, c = Math.cos(deg * Math.PI / 180), sn = Math.sin(deg * Math.PI / 180), BL = P.BL, BR = P.BR;
        var v0 = [P.TL[0] - BL[0], P.TL[1] - BL[1]], n = [(v0[0] * c + P.cL[0] * sn) * s, (v0[1] * c + P.cL[1] * sn) * s];
        var e1 = [(BR[0] - BL[0]) * s, (BR[1] - BL[1]) * s], e2 = [v0[0] * s, v0[1] * s];
        var cross = e1[0] * n[1] - e1[1] * n[0], face = cross > 0 ? 1 : 0;
        if (face !== lastFace) { lastFace = face; lcEl.style.display = face ? 'block' : 'none'; lidEl.firstElementChild.style.visibility = face ? 'hidden' : 'visible'; }
        if (Math.abs(cross) < 400 * s * s) { lidEl.style.visibility = 'hidden'; return; } lidEl.style.visibility = 'visible';
        var det = e1[0] * e2[1] - e1[1] * e2[0], ia = e2[1] / det, ib = -e2[0] / det, ic = -e1[1] / det, id = e1[0] / det;
        var m00 = e1[0] * ia + n[0] * ic, m01 = e1[0] * ib + n[0] * id, m10 = e1[1] * ia + n[1] * ic, m11 = e1[1] * ib + n[1] * id;
        var bx = BL[0] * s, by = BL[1] * s, tx = bx - (m00 * bx + m01 * by), ty = by - (m10 * bx + m11 * by);
        lidEl.style.transform = 'matrix3d(' + [m00, m10, 0, 0, m01, m11, 0, 0, 0, 0, 1, 0, tx, ty, 1, 1].map(function (x) { return x.toFixed(6); }).join(',') + ')';
    }

    var cur = 0, tgt = 0, t0 = performance.now(), mx = 0, my = 0, sx = 0, sy = 0, lastStage = -1;
    addEventListener('mousemove', function (e) { mx = e.clientX / innerWidth - .5; my = e.clientY / innerHeight - .5; });

    function render(now) {
        var W = innerWidth, H = innerHeight, mob = W < 820, w = Math.min(mob ? W * .95 : Math.min(W * .56, H * 1.0), 760), q = interp(KEYS, cur), el = now - t0;
        sx += (mx - sx) * .07; sy += (my - sy) * .07;
        var idle = still ? 0 : 1, bob = Math.sin(el / 1400) * 6 * idle, sway = Math.sin(el / 2600) * 2.5 * idle;
        var open = still ? 1 : ease((el - 500) / 2600), lid = (1 - open) * 86 + Math.sin(el / 1800) * 1.4 * idle + 86 * smooth((cur - .90) / .07);
        setLid(Math.max(0, Math.min(88, lid)), w);
        var tx = mob ? .5 : q[1], ty = mob ? .27 : q[2], ry = ((q[4] % 360) + 360) % 360, flipped = ry > 90 && ry < 270;
        rig.style.width = rig.style.height = w + 'px';
        rig.style.setProperty('--fl', flipped ? 1 : 0); rig.style.setProperty('--zs', flipped ? -1 : 1);
        rig.style.transform = 'translate3d(' + (tx * W - w / 2).toFixed(1) + 'px,' + (ty * H - w / 2 + bob).toFixed(1) + 'px,0) rotateY(' + (q[4] + sway + sx * 14).toFixed(2) + 'deg) rotateX(' + (q[5] - sy * 8).toFixed(2) + 'deg) scale(' + q[3].toFixed(3) + ')';
        glow.style.setProperty('--gx', (tx * 100) + '%');
        var limit = Math.max(marquee ? marquee.getBoundingClientRect().bottom : 0, document.getElementById('publicNav').getBoundingClientRect().bottom) + 12;   /* minh họa không được chồng lên thanh header hay dòng chào mừng */
        props.forEach(function (e2) {
            var k = win(e2, cur); e2.style.opacity = k.toFixed(3); e2.style.setProperty('--k', k.toFixed(3));
            if (k > .02) { e2.style.setProperty('--dy', '0px'); var top = e2.getBoundingClientRect().top; e2.style.setProperty('--dy', (top < limit ? Math.min(120, limit - top) : 0).toFixed(1) + 'px'); }
        });
        var heroK = 1, dy = 0;
        if (!mob && marquee) {   /* dòng chào mừng nằm giữa thanh header và dòng kicker; nếu màn hình thấp thì đẩy khối chữ xuống một chút */
            var c0 = document.querySelector('.copy.c0'), c0h = c0.offsetHeight, mh = marquee.offsetHeight, navB = 64, top0 = H * .5 - c0h / 2, gap = top0 - navB;
            dy = Math.min(Math.max(0, mh + 24 - gap), Math.max(0, H - 84 - (top0 + c0h)));
            marquee.style.top = (navB + Math.max(0, (gap + dy - mh) / 2)).toFixed(1) + 'px';
        }
        copies.forEach(function (e2) {
            var a = +e2.dataset.a, b = +e2.dataset.b, f = .035, k = win(e2, cur);
            if (e2.classList.contains('c4')) k = smooth((cur - a) / .05);
            if (e2.classList.contains('c0')) { k = 1 - smooth((cur - (b - f)) / f); heroK = k; }
            var y = (1 - k) * 30, t = e2.classList.contains('center') ? 'translate(-50%, calc(-50% + ' + y + 'px))' : (mob ? 'translateY(' + y + 'px)' : 'translateY(calc(-50% + ' + (y + (e2.classList.contains('c0') ? dy : 0)) + 'px))');
            e2.style.opacity = k.toFixed(3); e2.style.transform = t; e2.classList.toggle('on', k > .5);
        });
        caps.style.opacity = heroK.toFixed(3); caps.style.pointerEvents = heroK > .5 ? 'auto' : 'none';

        /* linh vật đi theo từng bước */
        var mw = mob ? 96 : Math.max(140, Math.min(210, W * .17)), c0r = document.querySelector('.copy.c0').getBoundingClientRect().right, heroX = Math.max(.5, (c0r + mw / 2 + 28) / W);   /* ở trang đầu linh vật ở giữa; màn hình hẹp thì dịch sang phải để không đè chữ */
        MKEYS[0][1] = MKEYS[1][1] = heroX;
        var mq = interp(MKEYS, cur), mxp = mob ? .84 : mq[1], myp = mob ? .37 : mq[2], hop = Math.abs(Math.sin(el / 700)) * -4 * idle;
        mascot.style.width = mw + 'px'; mascot.style.left = (mxp * W).toFixed(1) + 'px'; mascot.style.top = (myp * H + hop).toFixed(1) + 'px';
        var side = !mob && cur >= .12 && cur <= .92, right = mxp > .5;
        if (side) { say.style.left = (right ? mxp * W - mw / 2 - 6 : mxp * W + mw / 2 + 6).toFixed(1) + 'px'; say.style.top = (myp * H - mw * .1).toFixed(1) + 'px'; say.style.transform = 'translate(' + (right ? '-100%' : '0') + ', -50%)'; }
        else { say.style.left = Math.max(120, Math.min(W - 120, mxp * W)).toFixed(1) + 'px'; say.style.top = (myp * H - mw / 2 - 18) + 'px'; say.style.transform = 'translate(-50%, -100%)'; }
        var st = stageOf(cur);
        if (st !== lastStage) {
            lastStage = st; say.textContent = SAYS[st]; railDots.forEach(function (d, i) { d.classList.toggle('on', i === st); });
            navLinks.forEach(function (a) { var s = +a.dataset.stage || 0; a.classList.toggle('is-active', s > 0 && s === st); });
        }
        up.disabled = cur < .02; down.disabled = cur > .985;
    }
    function loop(now) { tgt = prog(); cur += (tgt - cur) * .12; if (Math.abs(tgt - cur) < .0004) cur = tgt; render(now); requestAnimationFrame(loop); }
    cur = tgt = prog(); requestAnimationFrame(loop);

    /* điều khiển: hai mũi tên lên/xuống và các mục trên thanh điều hướng */
    function step(dir) {
        var p = prog(), i = 0, best = 9; STOPS.forEach(function (s, k) { var d = Math.abs(s - p); if (d < best) { best = d; i = k; } });
        if (best > .02) { i = dir > 0 ? STOPS.findIndex(function (s) { return s > p + .01; }) : (function () { var j = -1; STOPS.forEach(function (s, k) { if (s < p - .01) j = k; }); return j; })(); }
        else i = i + dir;
        i = Math.max(0, Math.min(STOPS.length - 1, i)); goto(STOPS[i]);
    }
    up.addEventListener('click', function () { step(-1); }); down.addEventListener('click', function () { step(1); });
    navLinks.forEach(function (a) { var s = +a.dataset.stage; if (s) a.addEventListener('click', function (e) { e.preventDefault(); goto(STOPS[s]); }); });
})();
