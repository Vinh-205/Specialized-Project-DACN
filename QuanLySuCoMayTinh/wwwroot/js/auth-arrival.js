(() => {
    let arrival;
    try {
        arrival = JSON.parse(sessionStorage.getItem('authArrival') || 'null');
        sessionStorage.removeItem('authArrival');
    } catch { return; }
    if (!arrival || Date.now() - arrival.at > 15000 || arrival.url !== location.pathname + location.search) return;
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    if (reduced.matches) return;
    const animation = document.body.animate([
        { opacity:.4, transform:'translateX(32px)' },
        { opacity:1, transform:'translateX(0)' }
    ], { duration:380, easing:'cubic-bezier(.2,.7,.2,1)' });
    reduced.addEventListener('change', () => { if (reduced.matches) animation.cancel(); }, { once:true });
})();
