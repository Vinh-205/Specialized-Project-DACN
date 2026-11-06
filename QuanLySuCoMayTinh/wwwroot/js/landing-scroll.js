(function () {
    'use strict';
    const hero = document.getElementById('hero');
    const video = document.getElementById('heroVideo');
    if (!hero || !video) return;
    const reduced = window.matchMedia('(prefers-reduced-motion: reduce)');
    let target = 0;
    let frame = 0;
    let failed = false;
    let motion = null;
    const chapterButtons = [...document.querySelectorAll('.story-nav button')];

    // Coalesce scroll events and allow one decoded seek at a time.
    function seek() {
        if (failed || video.seeking || video.readyState < 2 || !Number.isFinite(video.duration)) return;
        if (Math.abs(video.currentTime - target) > 0.035) video.currentTime = target;
    }
    function update() {
        frame = 0;
        const range = hero.offsetHeight - document.getElementById('heroPinned').offsetHeight;
        const progress = reduced.matches || !hero.classList.contains('scroll-ready') || range <= 0 ? 0 : Math.max(0, Math.min(1, -hero.getBoundingClientRect().top / range));
        target = Number.isFinite(video.duration) ? progress * Math.max(0, video.duration - 0.05) : 0;
        const chapter = progress < 0.32 ? 0 : progress < 0.72 ? 1 : 2;
        chapterButtons.forEach((button, index) => {
            if (index === chapter) button.setAttribute('aria-current', 'step');
            else button.removeAttribute('aria-current');
        });
        seek();
    }
    function schedule() {
        if (!frame) frame = requestAnimationFrame(update);
    }
    video.pause();
    video.addEventListener('loadeddata', schedule);
    video.addEventListener('seeked', seek);
    video.addEventListener('canplay', schedule);
    function fallback() {
        failed = true;
        motion?.revert();
        hero.classList.add('video-unavailable');
    }
    video.addEventListener('error', fallback);
    video.querySelector('source')?.addEventListener('error', fallback);
    window.addEventListener('scroll', schedule, { passive: true });
    window.addEventListener('resize', schedule);
    window.addEventListener('pageshow', schedule);
    reduced.addEventListener('change', schedule);
    schedule();
    chapterButtons.forEach(button => button.addEventListener('click', () => {
        const range = hero.offsetHeight - document.getElementById('heroPinned').offsetHeight;
        window.scrollTo({ top: window.scrollY + hero.getBoundingClientRect().top + range * Number(button.dataset.progress), behavior: 'smooth' });
    }));

    document.querySelectorAll('a[href^="#"]').forEach(link => {
        const id = link.getAttribute('href').slice(1);
        const destination = id && document.getElementById(id);
        if (!destination) return;
        link.addEventListener('click', event => {
            event.preventDefault();
            destination.scrollIntoView({ behavior: reduced.matches ? 'instant' : 'smooth' });
        });
    });

    if (typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;
    gsap.registerPlugin(ScrollTrigger);
    motion = gsap.matchMedia();
    motion.add('(prefers-reduced-motion: no-preference) and (min-height: 720px)', () => {
        if (failed) return;
        hero.classList.add('scroll-ready', 'has-scroll-motion');
        const timeline = gsap.timeline({ scrollTrigger: {
            trigger: hero, start: 'top top', end: 'bottom bottom', scrub: 0.35,
            invalidateOnRefresh: true,
        }});
        timeline.to('#heroCenter', { autoAlpha: 0, y: -45, duration: 0.18 }, 0.03)
            .to(video, { top: 340, height: () => document.getElementById('heroPinned').offsetHeight - 416, duration: 0.25, ease: 'none' }, 0)
            .fromTo('#chapterDiagnosis', { autoAlpha: 0, y: 25 }, { autoAlpha: 1, y: 0, duration: 0.09 }, 0.26)
            .to('#chapterDiagnosis', { autoAlpha: 0, y: -25, duration: 0.08 }, 0.63)
            .fromTo('#chapterResolution', { autoAlpha: 0, y: 25 }, { autoAlpha: 1, y: 0, duration: 0.09 }, 0.72);
        const moves = [
            ['#cardAi', -1, -1], ['#cardTicket', 1, -1],
            ['#cardKb', -1, 1], ['#cardStats', 1, 1], ['#cardAuth', 1, 0],
        ];
        moves.forEach(([id, x, y], index) => {
            timeline.to(id, {
                x: () => x * Math.min(window.innerWidth * 0.22, 320), y: y * 160,
                rotation: x * 9, scale: 0.9, autoAlpha: 0, duration: 0.23,
                ease: 'power1.in',
            }, 0.025 + index * 0.025);
        });
        // Fixed one-unit timeline keeps chapters aligned with scroll progress.
        timeline.to({}, { duration: 0.01 }, 0.99);
        schedule();
        return () => {
            hero.classList.remove('scroll-ready', 'has-scroll-motion');
            schedule();
        };
    });
})();
