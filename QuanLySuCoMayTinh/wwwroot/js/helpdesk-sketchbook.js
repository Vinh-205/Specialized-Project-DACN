/* Sketchbook navigation, tools and index adapted to the Helpdesk chapters. */
(() => {
    'use strict';
    const book = document.getElementById('helpdeskBook');
    if (!book) return;
    const hero = document.getElementById('sketchbook');
    const desk = hero.querySelector('.sk-desk');
    const stage = hero.querySelector('.sb-stage');
    const pages = [...book.querySelectorAll('.sb-spread')];
    const entries = [...document.querySelectorAll('[data-goto]')];
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const desktop = matchMedia('(min-width:900px) and (min-height:760px)');
    const caption = document.getElementById('bookCaption');
    const zoomRead = document.getElementById('zoomRead');
    const out = document.getElementById('zoomOut');
    const into = document.getElementById('zoomIn');
    const loupe = document.getElementById('loupeToggle');
    let index = 0, zoom = 1, magnify = false, frame = 0, animation = null;
    let chapterAnimations = [];

    function measurePages() {
        // Reserve the tallest spread so controls never jump between chapters.
        pages.forEach(page => { page.style.minHeight = ''; });
        let height = 0;
        pages.forEach(page => {
            const hidden = page.hidden;
            page.hidden = false;
            height = Math.max(height, page.offsetHeight);
            page.hidden = hidden;
        });
        pages.forEach(page => { page.style.minHeight = `${height}px`; });
    }

    function render(next, animate = true) {
        next = Math.max(0, Math.min(pages.length - 1, next));
        const changed = index !== next;
        animation?.cancel();
        chapterAnimations.forEach(item => item.cancel());
        chapterAnimations = [];
        index = next;
        pages.forEach((page, i) => { page.hidden = i !== index; });
        entries.forEach((entry, i) => entry.setAttribute('aria-current', String(i === index)));
        document.querySelector('[data-step="-1"]').disabled = index === 0;
        document.querySelector('[data-step="1"]').disabled = index === pages.length - 1;
        caption.textContent = `${String(index + 1).padStart(2, '0')} / 04 · ${pages[index].querySelector('h2').textContent}`;
        if (changed && animate && !reduced.matches) {
            // A flat paper reveal preserves readable DOM content on touch devices.
            animation = pages[index].animate([
                { opacity: .35, clipPath: 'inset(0 0 0 15%)', transform: 'translateX(10px)' },
                { opacity: 1, clipPath: 'inset(0)', transform: 'translateX(0)' }
            ], { duration: 360, easing: 'cubic-bezier(.2,.7,.2,1)' });
            pages[index].querySelectorAll('.sb-number,h2,.sb-tag,.sb-description,.sb-picture,.sb-art-note').forEach((element, i) => {
                chapterAnimations.push(element.animate([
                    { opacity:0, translate:`${i < 4 ? -12 : 12}px 8px` },
                    { opacity:1, translate:'0 0' }
                ], { duration:450, delay:i * 45, fill:'backwards', easing:'cubic-bezier(.2,.7,.2,1)' }));
            });
        }
        placeLens();
    }
    function scrollRange() { return Math.max(0, hero.offsetHeight - desk.offsetHeight); }
    function goTo(next) {
        next = Math.max(0, Math.min(pages.length - 1, next));
        render(next);
        if (document.body.classList.contains('scroll-book')) {
            const y = window.scrollY + hero.getBoundingClientRect().top - 100;
            // Keep button navigation and scroll-driven chapter selection in sync.
            window.scrollTo({ top: y + next / (pages.length - 1) * scrollRange(), behavior: 'instant' });
        }
    }
    function onScroll() {
        frame = 0;
        if (!document.body.classList.contains('scroll-book')) return;
        const range = scrollRange();
        const progress = range ? Math.max(0, Math.min(1, (100 - hero.getBoundingClientRect().top) / range)) : 0;
        const next = Math.min(pages.length - 1, Math.floor(progress * pages.length));
        if (next !== index) render(next);
    }
    function schedule() { if (!frame) frame = requestAnimationFrame(onScroll); }
    function configure() {
        document.body.classList.toggle('scroll-book', desktop.matches && !reduced.matches);
        if (reduced.matches) {
            animation?.cancel();
            chapterAnimations.forEach(item => item.cancel());
        }
        schedule();
    }
    document.querySelectorAll('[data-step]').forEach(button => button.addEventListener('click', () => goTo(index + Number(button.dataset.step))));
    entries.forEach(entry => entry.addEventListener('click', event => {
        event.preventDefault();
        goTo(Number(entry.dataset.goto));
        if (!document.body.classList.contains('scroll-book')) hero.scrollIntoView({ behavior: reduced.matches ? 'instant' : 'smooth' });
        stage.focus({ preventScroll: true });
    }));
    stage.addEventListener('keydown', event => {
        if (event.altKey || event.ctrlKey || event.metaKey) return;
        if (event.key === 'ArrowRight' || event.key === 'ArrowLeft') {
            event.preventDefault(); goTo(index + (event.key === 'ArrowRight' ? 1 : -1));
        }
    });
    let touch = null;
    stage.addEventListener('pointerdown', event => {
        if (event.pointerType !== 'touch' || event.target.closest('button')) return;
        touch = { x: event.clientX, y: event.clientY };
    });
    stage.addEventListener('pointerup', event => {
        if (!touch) return;
        const dx = event.clientX - touch.x, dy = event.clientY - touch.y;
        touch = null;
        if (Math.abs(dx) > 45 && Math.abs(dx) > Math.abs(dy) * 1.5) goTo(index + (dx < 0 ? 1 : -1));
    });
    stage.addEventListener('pointercancel', () => { touch = null; });

    function setZoom(value) {
        zoom = Math.max(1, Math.min(1.6, Math.round(value * 10) / 10));
        book.style.setProperty('--book-zoom', zoom);
        zoomRead.textContent = `${Math.round(zoom * 100)}%`;
        out.disabled = zoom <= 1;
        into.disabled = zoom >= 1.6;
        placeLens();
    }
    function placeLens(event) {
        const picture = pages[index].querySelector('.sb-picture');
        const image = picture.querySelector('img');
        const lens = picture.querySelector('.sb-lens');
        picture.classList.toggle('loupe-active', magnify);
        if (!magnify) return;
        const bounds = picture.getBoundingClientRect();
        const radius = lens.offsetWidth / 2;
        const x = event ? Math.max(radius, Math.min(bounds.width - radius, event.clientX - bounds.left)) : bounds.width / 2;
        const y = event ? Math.max(radius, Math.min(bounds.height - radius, event.clientY - bounds.top)) : bounds.height / 2;
        const px = (x - bounds.width / 2) / zoom + bounds.width / 2;
        const py = (y - bounds.height / 2) / zoom + bounds.height / 2;
        const factor = zoom * 2.3;
        lens.style.left = `${x - radius}px`; lens.style.top = `${y - radius}px`;
        lens.style.backgroundImage = `url("${image.currentSrc || image.src}")`;
        lens.style.backgroundSize = `${bounds.width * factor}px ${bounds.height * factor}px`;
        lens.style.backgroundPosition = `${radius - px * factor}px ${radius - py * factor}px`;
    }
    into.addEventListener('click', () => setZoom(zoom + .2));
    out.addEventListener('click', () => setZoom(zoom - .2));
    document.getElementById('zoomReset').addEventListener('click', () => setZoom(1));
    loupe.addEventListener('click', () => {
        magnify = !magnify;
        loupe.setAttribute('aria-pressed', String(magnify));
        pages.forEach(page => page.querySelector('.sb-picture').classList.remove('loupe-active'));
        placeLens();
    });
    pages.forEach(page => page.querySelector('.sb-picture').addEventListener('pointermove', placeLens));
    pages.forEach(page => {
        const img = page.querySelector('img');
        img.addEventListener('error', () => { img.src = pages[0].querySelector('img').src; }, { once: true });
    });
    window.addEventListener('scroll', schedule, { passive: true });
    window.addEventListener('resize', () => { measurePages(); schedule(); placeLens(); });
    window.addEventListener('pageshow', schedule);
    reduced.addEventListener('change', configure);
    desktop.addEventListener('change', configure);
    document.body.classList.add('sk-ready');
    render(0, false); setZoom(1); configure();
    measurePages();
    document.fonts?.ready.then(() => { measurePages(); schedule(); });
})();
