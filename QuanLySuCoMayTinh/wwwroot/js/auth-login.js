(() => {
    'use strict';
    const form = document.getElementById('loginForm');
    const password = document.getElementById('password');
    const toggle = document.getElementById('pwdToggle');
    const submit = document.getElementById('submitBtn');
    const username = document.getElementById('username');
    const mascot = document.querySelector('.auth-mascot');
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    let activeField = 'username';
    let reaction;
    // ── Hiệu ứng Lottie (cdnjs lottie-web): ổ khóa PIN phía trên phản ứng theo số ký tự mật khẩu, nét "loading" phía dưới chạy khi xác thực.
    // Không tải được thư viện thì giữ nguyên mascot CRT và vòng quay trong nút (không ảnh hưởng đăng nhập).
    const section = document.querySelector('.auth-form');
    const pinBox = document.getElementById('authPinAnim');
    const PIN_FRAMES = [0, 28, 36, 46, 53];   // 0–4 ký tự: số dấu * hiện ra trên thanh PIN
    const PIN_UNLOCK = 130;                   // khóa mở + dấu tích trên khiên
    let pin = null, pinFrame = 0, scene = null;
    function movePin(to, speed) {
        if (!pin || to === pinFrame) return;
        if (reduced.matches) { pin.goToAndStop(to, true); pinFrame = to; return; }
        const from = pinFrame;
        pin.setSpeed(speed || 1.5);
        pin.setDirection(to > from ? 1 : -1);
        pin.playSegments([from, to], true);
        pinFrame = to;
    }
    // Cảnh chuyển mở: dừng khóa PIN (hoạt ảnh SVG nặng) để lớp mờ nền không bị tính lại mỗi khung hình
    window.addEventListener('hdscene:open', () => { if (pin) pin.pause(); });
    function resetPin() { movePin(PIN_FRAMES[Math.min(password.value.length, 4)] || 0, 2); }
    function startLottie() {
        if (!window.lottie || !pinBox) return;
        pin = lottie.loadAnimation({ container: pinBox, renderer: 'svg', loop: false, autoplay: false, path: pinBox.dataset.src,
            rendererSettings: { preserveAspectRatio: 'xMidYMid meet' } });
        pin.addEventListener('DOMLoaded', () => { section.classList.add('has-lottie'); pin.goToAndStop(0, true); });
    }
    if (document.readyState === 'complete') startLottie(); else addEventListener('load', startLottie);
    password.addEventListener('input', () => movePin(PIN_FRAMES[Math.min(password.value.length, 4)]));
    function syncMascot() {
        const closed = activeField === 'password' && password.type === 'password';
        if (mascot.classList.contains('is-eyes-closed') === closed) return;
        mascot.classList.toggle('is-eyes-closed', closed);
        mascot.setAttribute('aria-label', closed ? 'CRT nhắm mắt khi mật khẩu được ẩn' : 'CRT mở mắt chào bạn');
        reaction?.cancel();
        if (!reduced.matches) {
            reaction = mascot.animate([
                { transform:'translateY(0) scale(1)' },
                { transform:'translateY(2px) scale(1.03,.96)', offset:.4 },
                { transform:'translateY(0) scale(1)' }
            ], { duration:240, easing:'ease-out' });
        }
    }
    for (const event of ['focus', 'input', 'change']) {
        username.addEventListener(event, () => { activeField = 'username'; syncMascot(); });
        password.addEventListener(event, () => { activeField = 'password'; syncMascot(); });
    }
    reduced.addEventListener('change', () => { if (reduced.matches) reaction?.cancel(); });
    toggle.addEventListener('click', () => {
        const visible = password.type === 'password';
        password.type = visible ? 'text' : 'password';
        toggle.setAttribute('aria-pressed', String(visible));
        toggle.setAttribute('aria-label', visible ? 'Ẩn mật khẩu' : 'Hiện mật khẩu');
        toggle.title = toggle.getAttribute('aria-label');
        document.getElementById('pwdIcon').className = visible ? 'bi bi-eye-slash-fill' : 'bi bi-eye';
        activeField = 'password';
        syncMascot();
    });
    // Opt-in JSON keeps native POST working when JavaScript is unavailable.
    form.addEventListener('submit', async event => {
        event.preventDefault();
        if (submit.disabled) return;
        submit.disabled = true;
        submit.classList.add('is-loading');
        scene = window.HDScene ? HDScene.open('login') : null;
        form.setAttribute('aria-busy', 'true');
        const status = document.getElementById('loginStatus');
        status.classList.remove('is-error');
        document.getElementById('submitText').textContent = 'Đang xác thực...';
        status.textContent = 'Đang xác thực tài khoản.';
        const abort = new AbortController();
        const timeout = setTimeout(() => abort.abort(), 20000);
        try {
            const response = await fetch(form.action, {
                method:'POST', body:new FormData(form), credentials:'same-origin',
                headers:{ Accept:'application/json' }, signal:abort.signal
            });
            const data = response.headers.get('content-type')?.includes('application/json')
                ? await response.json() : null;
            if (!response.ok || data?.success !== true) {
                throw new Error(data?.message || (response.status === 400
                    ? 'Phiên đăng nhập đã hết hạn. Vui lòng tải lại trang.'
                    : 'Không thể đăng nhập lúc này. Vui lòng thử lại.'));
            }
            if (typeof data.redirectUrl !== 'string') throw new Error('Không xác định được trang làm việc.');
            const destination = new URL(data.redirectUrl, location.href);
            if (destination.origin !== location.origin) throw new Error('Đường dẫn chuyển tiếp không hợp lệ.');
            submit.classList.remove('is-loading');
            submit.classList.add('is-success');
            document.getElementById('submitIcon').className = 'bi bi-check-circle-fill';
            document.getElementById('submitText').textContent = 'Đăng nhập thành công';
            status.textContent = 'Đang vào không gian làm việc...';
            if (!scene) movePin(PIN_UNLOCK, 1.8);
            if (scene) {
                await scene.wait(2200);          // cảnh chuyển hiện tối thiểu ~2,2 giây để chữ Login kịp viết xong
                scene.morph();                   // khóa → dấu tích, dòng trạng thái đổi sang "Đang vào không gian làm việc…"
                await scene.wait(3000);
            } else {
                await new Promise(resolve => setTimeout(resolve, reduced.matches ? 0 : 450));
            }
            window.HDScene?.markArrival();       // trang đích mở ra bằng lớp mờ tan dần
            location.assign(destination.href);
        } catch (error) {
            submit.disabled = false;
            submit.classList.remove('is-loading', 'is-success');
            scene?.close(); scene = null;
            resetPin();
            document.getElementById('submitIcon').className = 'bi bi-arrow-right';
            document.getElementById('submitText').textContent = 'Đăng nhập';
            status.classList.add('is-error');
            status.textContent = error.name === 'AbortError' || error instanceof TypeError
                ? 'Kết nối bị gián đoạn. Vui lòng thử lại.' : error.message;
        } finally {
            clearTimeout(timeout);
            form.removeAttribute('aria-busy');
        }
    });
    addEventListener('pageshow', () => {
        submit.classList.remove('is-loading', 'is-success');
        document.getElementById('submitIcon').className = 'bi bi-arrow-right';
        document.querySelector('.auth-main').getAnimations().forEach(animation => animation.cancel());
        scene?.close(); scene = null;
        resetPin();
        submit.disabled = false;
        form.removeAttribute('aria-busy');
        document.getElementById('submitText').textContent = 'Đăng nhập';
        document.getElementById('loginStatus').textContent = '';
        activeField = document.activeElement === username ? 'username'
            : document.activeElement === password || password.value ? 'password' : 'username';
        syncMascot();
    });
})();
