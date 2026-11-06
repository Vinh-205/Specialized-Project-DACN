/* Landing (sổ tay): laptop mổ xẻ và hành trình bốn trạm đi theo vị trí cuộn.
   Hình là SVG dựng bởi Tools/LaptopTeardown (kit Anatomy, MIT); script chỉ đẩy các nhóm SVG và đổi chú thích. */
(function () {
    'use strict';
    var clamp = function (v, a, b) { return Math.max(a, Math.min(b, v)); };
    var ease = function (x) { x = clamp(x, 0, 1); return x * x * (3 - 2 * x); };
    var still = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var $ = function (s, r) { return (r || document).querySelector(s); };
    var $$ = function (s, r) { return [].slice.call((r || document).querySelectorAll(s)); };

    /* bộ điều khiển chung: đọc tiến độ cuộn của một khối, làm mượt, gọi apply(progress, dt) khi cần */
    function scroller(section, apply) {
        var v = 0, last = performance.now(), running = false, visible = false;
        function frame(now) {
            var dt = Math.min(.1, (now - last) / 1000); last = now;
            var r = section.getBoundingClientRect(), tg = clamp(-r.top / (r.height - innerHeight), 0, 1);
            v = still ? tg : v + (tg - v) * Math.min(1, dt * 9);
            apply(v);
            if (visible && Math.abs(tg - v) > .0005) requestAnimationFrame(frame); else running = false;
        }
        function kick() { if (!running) { running = true; last = performance.now(); requestAnimationFrame(frame); } }
        new IntersectionObserver(function (e) { visible = e[0].isIntersecting; if (visible) kick(); }, { rootMargin: '100px' }).observe(section);
        addEventListener('scroll', function () { if (visible) kick(); }, { passive: true }); addEventListener('resize', kick);
        apply(0);
    }
    function captions(section, idx) {
        $$('.lf-step', section).forEach(function (el, i) { el.classList.toggle('on', i === idx); });
        $$('.lf-dots i', section).forEach(function (el, i) { el.classList.toggle('on', i === idx); });
    }

    /* ── laptop ── */
    var secA = $('#inside'), figA = $('#lfLaptop');
    if (secA && figA) {
        var RISE = +figA.dataset.rise;
        var layers = $$('.lp-layer', figA).map(function (el) { return { el: el, id: el.dataset.id, stage: +el.dataset.stage, dz: +el.dataset.dz, a: +el.dataset.a, b: +el.dataset.b, base: el.dataset.base }; });
        var byId = {}; layers.forEach(function (l) { byId[l.id] = l; });
        var lidFr = $$('.lp-lidf', figA), lidCur = -1, lidN = 0, FR = Math.max.apply(null, lidFr.map(function (f) { return +f.dataset.f; })) + 1;
        var svgA = figA.firstElementChild;
        var stageOf = function (v) { return v < .1 ? 0 : v < .4 ? 1 : v < .62 ? 2 : v < .84 ? 3 : 4; };
        /* bước đầu: laptop đang mở tự gập lại (đóng nắp); cuộn xuống thì mở nắp trở lại khi lớp mặt phím tách ra */
        var autoLid = still ? 0 : 1, lastP = 0;
        var applyA = function (p) {
            lastP = p;
            var open = Math.max(autoLid, ease((p - .12) / .14)), want = Math.round(clamp(open, 0, 1) * (FR - 1));
            if (want !== lidCur) { lidCur = want; lidFr.forEach(function (f) { f.style.display = (+f.dataset.f === want) ? '' : 'none'; }); }
            layers.forEach(function (l) { l.t = ease((p - l.a) / (l.b - l.a)); });
            var s = stageOf(p);
            layers.forEach(function (l) {
                var off = l.t * l.dz + (l.base ? byId[l.base].t * byId[l.base].dz : 0);
                l.el.setAttribute('transform', 'translate(0 ' + (-off * RISE).toFixed(2) + ')');
                l.el.style.opacity = (s === 0 || l.stage === s) ? 1 : .2;
            });
            // lúc máy còn nguyên vẹn, đưa hình lên giữa khung; máy tách lớp thì trả về vị trí gốc
            var e0 = 1 - ease(p / .45); svgA.style.transform = 'translateY(' + (-30 * e0).toFixed(2) + '%) scale(' + (1 + .4 * e0).toFixed(3) + ')';
            captions(secA, s);
        };
        scroller(secA, applyA);
        if (!still) {
            var t0 = performance.now() + 900;
            (function autoClose(now) {
                var u = clamp((now - t0) / 1600, 0, 1); autoLid = 1 - ease(u); applyA(lastP);
                if (u < 1) requestAnimationFrame(autoClose);
            })(performance.now());
        }
    }

    /* ── bốn trạm ── */
    var secB = $('#journey'), figB = $('#lfJourney');
    if (secB && figB) {
        var DV = figB.dataset.dv.split(',').map(Number);
        var jw = $('#jworld', figB), jd = $('#jdrift', figB), jg = $('#jgrid', figB);
        var sts = $$('.lp-st', figB), gf = $$('.lp-gf', figB);
        var mod = function (a, n) { return ((a % n) + n) % n; };
        scroller(secB, function (q) {
            var u = q * 3, tx = -DV[0] * u, ty = -DV[1] * u;
            var t = 'translate(' + tx.toFixed(1) + ' ' + ty.toFixed(1) + ')';
            jw.setAttribute('transform', t); jg.setAttribute('transform', t);
            jd.setAttribute('transform', 'translate(' + (tx * 1.3).toFixed(1) + ' ' + (ty * 1.3).toFixed(1) + ')');
            sts.forEach(function (el, i) {
                var d = Math.abs(i - u), sc = 1 + .07 * (1 - Math.min(1, d)), sx = +el.dataset.sx, sy = +el.dataset.sy;
                el.style.opacity = clamp(1 - d * .75, .1, 1).toFixed(2);
                el.setAttribute('transform', 'translate(' + sx + ' ' + sy + ') scale(' + sc.toFixed(3) + ') translate(' + (-sx) + ' ' + (-sy) + ')');
            });
            var phase = q * 3 * 360 * 1.2, bi = Math.floor(mod(phase, 30) / 5), si = Math.floor(mod(-phase * 1.5 + 22.5, 45) / 7.5);
            gf.forEach(function (g) { var want = g.dataset.gear === 'big' ? bi : si, on = +g.dataset.f === want; if ((g.style.display !== 'none') !== on) g.style.display = on ? '' : 'none'; });
            captions(secB, clamp(Math.round(u), 0, 3));
        });
    }
})();
