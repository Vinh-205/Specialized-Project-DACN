(() => {
    'use strict';
    const showcase = document.querySelector('.scroll-showcase');
    const home = document.querySelector('.welcome-scroll-track');
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const mobile = matchMedia('(max-width:760px)');
    const header = document.querySelector('.sk-top');
    const stage = showcase?.querySelector('.stage');
    const layers = showcase ? [...showcase.querySelectorAll('.layer')] : [];
    const buttons = showcase ? [...showcase.querySelectorAll('.phase-nav button')] : [];
    const captions = [
        ['Khởi đầu từ kết nối.', 'Thiết bị, yêu cầu và thông tin được tập hợp trong cùng một không gian.'],
        ['Thông tin được bóc tách.', 'Phiếu tiếp nhận giữ rõ thiết bị, triệu chứng và trạng thái xử lý.'],
        ['DeskAI kết nối tri thức.', 'Đối chiếu triệu chứng với mã lỗi và giải pháp để hỗ trợ kỹ thuật viên.'],
        ['Một hành trình liền mạch.', 'Từ dữ liệu đến chẩn đoán, các thành phần phối hợp trong cùng hệ thống.']
    ];
    const clamp = value => Math.max(0, Math.min(1, value));
    const ease = value => value * value * (3 - 2 * value);
    let frame = 0, phase = -1, top = 0;

    function draw() {
        frame = 0;
        if (home) {
            const p = home.classList.contains('scrub-enabled')
                ? clamp(-home.getBoundingClientRect().top / Math.max(1, home.offsetHeight - innerHeight)) : 0;
            home.querySelector('.welcome-copy').style.transform = `translateY(${-p * 45}px)`;
            home.querySelector('.welcome-lede').style.transform = `translateY(${-p * 30}px)`;
            home.querySelector('.welcome-mascot').style.transform = `translate(-50%,${-p * 30}px) scale(${1 - p * .08})`;
            home.querySelectorAll('.welcome-card').forEach((card, i) => {
                card.style.transform = `translate(${(i ? 1 : -1) * p * 25}px,${p * 20}px)`;
            });
        }
        if (!showcase) return;
        const p = showcase.classList.contains('scrub-enabled')
            ? clamp((top - showcase.getBoundingClientRect().top) / Math.max(1, showcase.offsetHeight - stage.offsetHeight)) : 1;
        const spread = ease(clamp((p - .06) / .66));
        // Reserve the final stretch for an unobstructed, level image.
        const reveal = ease(clamp((p - .72) / .18));
        const positions = mobile.matches
            ? [[0,85,-5],[0,35,0],[65,-90,5],[-55,-5,-5],[55,95,5]]
            : [[0,85,-5],[0,10,0],[210,-125,6],[-240,-45,-6],[190,140,5]];
        layers.forEach((layer, i) => {
            const [x,y,r] = positions[i];
            const amount = i === 0 ? spread * (1 - reveal)
                : i === 1 ? spread : ease(clamp((p - (i - 2) * .11) / .55));
            layer.style.transform = `translate(${x * amount}px,${y * amount}px) rotate(${r * amount}deg)`;
            layer.style.opacity = i === 0 ? 1
                : (i === 1 ? 1 : clamp((p - .025 - (i - 2) * .07) * 7)) * (1 - reveal);
            layer.style.visibility = i > 0 && reveal === 1 ? 'hidden' : 'visible';
        });
        showcase.querySelector('.progress span').style.transform = `scaleX(${p})`;
        const next = Math.min(3, Math.floor(p * 4));
        if (next !== phase) {
            phase = next;
            showcase.querySelector('.count').textContent = `0${phase + 1} / 04`;
            showcase.querySelector('.caption h3').textContent = captions[phase][0];
            showcase.querySelector('.caption p').textContent = captions[phase][1];
            buttons.forEach((button, i) => button.setAttribute('aria-current', String(i === phase)));
        }
    }
    function schedule() { if (!frame) frame = requestAnimationFrame(draw); }
    function configure() {
        top = header?.offsetHeight || 0;
        if (showcase) {
            const enabled = !reduced.matches;
            showcase.classList.toggle('scrub-enabled', enabled);
            stage.style.top = enabled ? `${top}px` : '';
            stage.style.height = enabled ? `${innerHeight - top}px` : '';
            if (enabled) {
                const title = showcase.querySelector('.stage-title');
                const caption = showcase.querySelector('.caption');
                const areaTop = title.offsetTop + title.offsetHeight + 20;
                const areaBottom = caption.offsetTop - 20;
                const wide = innerWidth >= 1200;
                const available = wide ? stage.offsetHeight - 110 : areaBottom - areaTop;
                const scale = Math.min(mobile.matches ? .78 : innerWidth <= 1100 ? .72 : 1,
                    Math.max(.2, available / (mobile.matches ? 500 : 650)));
                const scene = showcase.querySelector('.scene');
                scene.style.top = `${wide ? stage.offsetHeight * .49 : (areaTop + areaBottom) / 2}px`;
                scene.style.setProperty('--scene-scale', scale);
            } else {
                showcase.querySelector('.scene').style.removeProperty('top');
                showcase.querySelector('.scene').style.removeProperty('--scene-scale');
            }
        }
        if (home) home.classList.toggle('scrub-enabled', !reduced.matches && !mobile.matches && innerHeight >= 820);
        schedule();
    }
    buttons.forEach((button, i) => button.addEventListener('click', () => {
        const start = scrollY + showcase.getBoundingClientRect().top - top;
        scrollTo({ top:start + i / 3 * (showcase.offsetHeight - stage.offsetHeight), behavior:'instant' });
    }));
    addEventListener('scroll', schedule, { passive:true });
    addEventListener('resize', configure);
    addEventListener('pageshow', configure);
    reduced.addEventListener('change', configure);
    document.fonts?.ready.then(configure);
    configure();
})();
