// Cảnh chuyển đăng nhập / đăng xuất dùng chung: HDScene.open('login' | 'logout') trả về bộ điều khiển.
// Vòng tròn + icon biến hình (morphicons), nét "loading" (Lottie, tải khi cần từ cdnjs, lỗi thì bỏ qua), chữ viết tay Login/Logout.
(function () {
    'use strict';
    var CFG = window.HDSceneConfig || {};
    var LOTTIE_URL = 'https://cdnjs.cloudflare.com/ajax/libs/lottie-web/5.12.2/lottie_light.min.js';
    var KINDS = {
        login:  { word: 'Login',  icon: 'bi-lock-fill', iconTo: 'bi-check-lg',     sub: 'Đang xác thực tài khoản…', subTo: 'Đang vào không gian làm việc…' },
        logout: { word: 'Logout', icon: 'bi-person',    iconTo: 'bi-arrow-right', sub: 'Đang kết thúc phiên làm việc…', subTo: 'Hẹn gặp lại bạn!', morphAt: 900 }
    };
    var lottiePromise = null;
    function reduced() { return !!(window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches); }
    function loadLottie() {
        if (window.lottie) return Promise.resolve(window.lottie);
        if (!lottiePromise) {
            lottiePromise = new Promise(function (resolve, reject) {
                var s = document.createElement('script');
                s.src = LOTTIE_URL; s.onload = function () { resolve(window.lottie); }; s.onerror = reject;
                document.head.appendChild(s);
            });
        }
        return lottiePromise;
    }
    function open(kind) {
        var k = KINDS[kind] || KINDS.login, t0 = Date.now(), rm = reduced(), timers = [], anim = null, morphed = false, closed = false;
        var el = document.createElement('div');
        el.className = 'sc-overlay';
        el.setAttribute('role', 'status');
        el.setAttribute('aria-live', 'polite');
        el.innerHTML =
            '<div class="sc-stage"><span class="sc-circle"></span><i class="bi ' + k.icon + ' sc-icon"></i></div>' +
            '<div class="sc-load" aria-hidden="true"></div>' +
            '<div class="sc-word">' + k.word + '</div>' +
            '<div class="sc-sub">' + k.sub + '</div>';
        document.body.appendChild(el);
        document.documentElement.classList.add('sc-freeze');            // khóa hiệu ứng CSS phía dưới để lớp mờ nền không phải tính lại mỗi khung hình
        window.dispatchEvent(new CustomEvent('hdscene:open'));
        var iconEl = el.querySelector('.sc-icon'), subEl = el.querySelector('.sc-sub'), loadEl = el.querySelector('.sc-load');
        function at(ms, fn) { timers.push(setTimeout(fn, rm ? 0 : ms)); }
        // Tự thích nghi: máy yếu hoặc không có tăng tốc đồ họa thì làm mờ nền rất tốn, đo khung hình ở giai đoạn đầu rồi chuyển sang chế độ nhẹ
        var lite = false;
        try { lite = sessionStorage.getItem('hdSceneLite') === '1'; } catch (e) { /* bỏ qua */ }
        if (lite) el.classList.add('sc-lite');
        else if (!rm) at(250, function () {
            var n = 0, t0 = 0;
            (function f(t) {
                if (closed) return;
                if (!t0) t0 = t;
                n++;
                if (t - t0 < 600) { requestAnimationFrame(f); return; }
                if (n * 1000 / (t - t0) < 45) {
                    el.classList.add('sc-lite');
                    try { sessionStorage.setItem('hdSceneLite', '1'); } catch (e) { /* bỏ qua */ }
                }
            })(performance.now());
        });
        void el.offsetWidth; // ép trình duyệt tính kiểu ban đầu rồi mới thêm lớp để chạy hiệu ứng mờ dần (không phụ thuộc requestAnimationFrame)
        el.classList.add('on');
        at(250, function () { el.classList.add('circle'); });
        if (CFG.loading) {
            at(300, function () {
                loadLottie().then(function (l) {
                    if (closed) return;
                    anim = l.loadAnimation({ container: loadEl, renderer: 'svg', loop: true, autoplay: !rm, path: CFG.loading,
                        rendererSettings: { preserveAspectRatio: 'xMidYMid slice' } });
                    if (rm) anim.addEventListener('DOMLoaded', function () { anim.goToAndStop(70, true); });
                }).catch(function () { /* không tải được thư viện: bỏ nét loading, cảnh vẫn chạy */ });
            });
        }
        function morph() {
            if (morphed || closed) return;
            morphed = true;
            iconEl.className = 'bi ' + k.iconTo + ' sc-icon';
            subEl.textContent = k.subTo;
        }
        if (k.morphAt) at(k.morphAt, morph);
        return {
            el: el, morph: morph,
            setSub: function (t) { subEl.textContent = t; },
            wait: function (ms) { var left = Math.max(ms - (Date.now() - t0), 0); return new Promise(function (r) { setTimeout(r, rm ? 0 : left); }); },
            close: function () {
                if (closed) return Promise.resolve();
                closed = true;
                timers.forEach(clearTimeout);
                el.classList.remove('on');
                return new Promise(function (r) { setTimeout(function () { if (anim) anim.destroy(); el.remove(); document.documentElement.classList.remove('sc-freeze'); window.dispatchEvent(new CustomEvent('hdscene:close')); r(); }, rm ? 0 : 420); });
            }
        };
    }
    // Đánh dấu để trang đích mở ra bằng lớp mờ tan dần (script nhỏ trong _SystemIcons đọc khóa này)
    function markArrival() { try { sessionStorage.setItem('hdScene', JSON.stringify({ at: Date.now() })); } catch (e) { /* bỏ qua */ } }
    window.HDScene = { open: open, markArrival: markArrival };
})();
