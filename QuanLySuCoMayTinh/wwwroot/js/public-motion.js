(() => {
    'use strict';
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const animations = new Set();
    const seen = new WeakSet();
    function reveal(element, delay = 0) {
        if (seen.has(element)) return;
        seen.add(element);
        if (reduced.matches) return;
        const animation = element.animate([
            { opacity:0, translate:'0 18px' },
            { opacity:1, translate:'0 0' }
        ], { duration:620, delay, easing:'cubic-bezier(.2,.75,.25,1)', fill:'backwards' });
        animations.add(animation);
        animation.finished.catch(() => {}).finally(() => animations.delete(animation));
    }
    const selector = '.welcome-copy > *, .welcome-lede, .welcome-mascot, .welcome-card, .welcome-next > *, .sk-intro > *, .sb-stage, .sk-about > *, .sk-plates > *, .plate';
    const observer = new IntersectionObserver(entries => {
        let order = 0;
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;
            reveal(entry.target, Math.min(order++ * 65, 260));
            observer.unobserve(entry.target);
        });
    }, { threshold:.08 });
    document.querySelectorAll(selector).forEach(element => observer.observe(element));
    reduced.addEventListener('change', () => {
        if (reduced.matches) animations.forEach(animation => animation.cancel());
    });
})();
