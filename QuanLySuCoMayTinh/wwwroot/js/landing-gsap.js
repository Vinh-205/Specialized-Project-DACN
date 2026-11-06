/**
 * landing-gsap.js
 * ────────────────────────────────────────────
 * GSAP 3.12 + ScrollTrigger
 * Scroll-Scrubbing Video  ·  Card Peeling  ·  Parallax
 * ────────────────────────────────────────────
 * Strategy: Hero section has explicit tall height (300vh).
 *   - .hero-pinned uses CSS position:sticky to stay in viewport.
 *   - Video scrubs to scroll progress within hero section.
 *   - Cards peel outward as user scrolls.
 *   - No GSAP pin = no extra spacer = no empty gap.
 */

(function () {
    'use strict';

    if (typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;
    gsap.registerPlugin(ScrollTrigger);

    const video   = document.getElementById('heroVideo');
    const heroSec = document.getElementById('hero');

    if (!heroSec) return;

    /* ═══════════════════════════════════════════
       1. VIDEO — Scroll-Scrub
       ═══════════════════════════════════════════ */
    if (video) {
        video.pause();

        function bindVideoScrub() {
            const dur = video.duration;
            if (!dur || isNaN(dur)) return;

            ScrollTrigger.create({
                trigger: heroSec,
                start: 'top top',
                end: 'bottom bottom',
                scrub: 0.3,
                onUpdate: (self) => {
                    const t = self.progress * dur;
                    if (Math.abs(video.currentTime - t) > 0.03) {
                        video.currentTime = t;
                    }
                }
            });
        }

        if (video.readyState >= 1) bindVideoScrub();
        else video.addEventListener('loadedmetadata', bindVideoScrub);
    }


    /* ═══════════════════════════════════════════
       2. HERO CONTENT — Fade & rise on scroll
       ═══════════════════════════════════════════ */
    gsap.to('#heroCenter', {
        opacity: 0,
        y: -90,
        scale: 0.92,
        ease: 'power1.in',
        scrollTrigger: {
            trigger: heroSec,
            start: 'top top',
            end: '35% top',
            scrub: true,
        }
    });

    gsap.to('#scrollHint', {
        opacity: 0,
        y: 20,
        ease: 'power1.in',
        scrollTrigger: {
            trigger: heroSec,
            start: 'top top',
            end: '12% top',
            scrub: true,
        }
    });


    /* ═══════════════════════════════════════════
       3. BACKGROUND LAYERS — Parallax
       ═══════════════════════════════════════════ */
    gsap.to('#layerBg', {
        y: -80,
        scale: 1.06,
        ease: 'none',
        scrollTrigger: {
            trigger: heroSec,
            start: 'top top',
            end: 'bottom bottom',
            scrub: true,
        }
    });

    gsap.to('#layerGrid', {
        y: -40,
        opacity: 0.12,
        ease: 'none',
        scrollTrigger: {
            trigger: heroSec,
            start: 'top top',
            end: 'bottom bottom',
            scrub: true,
        }
    });


    /* ═══════════════════════════════════════════
       4. CARD PEELING — Explode outward
       ═══════════════════════════════════════════ */
    const cardMoves = [
        { id: '#cardAi',     x: -300, y: -160, rotate: -16, scale: 0.5 },
        { id: '#cardTicket', x:  330, y: -140, rotate:  13, scale: 0.5 },
        { id: '#cardKb',     x: -350, y:  120, rotate:  -9, scale: 0.45 },
        { id: '#cardStats',  x:  380, y:  140, rotate:  15, scale: 0.4 },
        { id: '#cardAuth',   x:  400, y:    5, rotate:  20, scale: 0.35 },
    ];

    cardMoves.forEach((c) => {
        gsap.to(c.id, {
            x: c.x,
            y: c.y,
            rotation: c.rotate,
            scale: c.scale,
            opacity: 0,
            ease: 'power2.in',
            scrollTrigger: {
                trigger: heroSec,
                start: 'top top',
                end: '45% top',
                scrub: 0.6,
            }
        });
    });


    /* ═══════════════════════════════════════════
       5. HERO ENTRANCE — Initial animation
       ═══════════════════════════════════════════ */
    (function heroEntrance() {
        const tl = gsap.timeline({
            defaults: { ease: 'power3.out' },
            delay: 0.15,
        });

        tl.from('#heroBadge', { y: 28, opacity: 0, duration: 0.55 })
          .from('#heroTitle', { y: 40, opacity: 0, duration: 0.65 }, '-=0.30')
          .from('#heroSub',   { y: 28, opacity: 0, duration: 0.55 }, '-=0.30')
          .from('#heroCta',   { y: 22, opacity: 0, duration: 0.45 }, '-=0.25')
          .from('.hero-card', {
              scale: 0.5,
              opacity: 0,
              duration: 0.5,
              stagger: 0.07,
              ease: 'back.out(1.8)',
          }, '-=0.25')
          .from('#scrollHint', { opacity: 0, y: 15, duration: 0.4 }, '-=0.2');
    })();


    /* ═══════════════════════════════════════════
       6. NAVBAR — Glass effect on scroll
       ═══════════════════════════════════════════ */
    (function navGlass() {
        const nav = document.querySelector('.land-nav');
        if (!nav) return;
        ScrollTrigger.create({
            trigger: 'body',
            start: 'top -80px',
            onEnter:     () => nav.classList.add('nav-scrolled'),
            onLeaveBack: () => nav.classList.remove('nav-scrolled'),
        });
    })();


    /* ═══════════════════════════════════════════
       7. SECTION STAGGER HELPER
       ═══════════════════════════════════════════ */
    function stagger(selector, opts = {}) {
        const d = {
            fromY: 50, fromOpacity: 0, fromScale: 1,
            toY: 0,    toOpacity: 1,   toScale: 1,
            duration: 0.7, stagger: 0.12, ease: 'power2.out',
            start: 'top 82%',
        };
        const c = { ...d, ...opts };
        const els = document.querySelectorAll(selector);
        if (!els.length) return;

        gsap.fromTo(els,
            { y: c.fromY, opacity: c.fromOpacity, scale: c.fromScale },
            {
                y: c.toY, opacity: c.toOpacity, scale: c.toScale,
                duration: c.duration,
                stagger: c.stagger,
                ease: c.ease,
                scrollTrigger: {
                    trigger: els[0].closest('section') || els[0].parentElement,
                    start: c.start,
                    toggleActions: 'play none none reverse',
                },
            }
        );
    }


    /* ═══════════════════════════════════════════
       8. FEATURES
       ═══════════════════════════════════════════ */
    stagger('#features .section-badge',  { fromY: 25, stagger: 0 });
    stagger('#features .section-title',  { fromY: 35, stagger: 0 });
    stagger('#features .section-sub',    { fromY: 25, stagger: 0 });
    stagger('#features .feature-card',   {
        fromY: 55, fromScale: 0.95, toScale: 1,
        stagger: 0.09, duration: 0.6, start: 'top 76%',
    });


    /* ═══════════════════════════════════════════
       9. HOW IT WORKS
       ═══════════════════════════════════════════ */
    stagger('#how-it-works .section-badge', { fromY: 25, stagger: 0 });
    stagger('#how-it-works .section-title', { fromY: 35, stagger: 0 });
    stagger('#how-it-works .step-item', {
        fromY: 65, fromScale: 0.92, toScale: 1,
        stagger: 0.13, duration: 0.65,
    });


    /* ═══════════════════════════════════════════
       10. STATS — Counter
       ═══════════════════════════════════════════ */
    (function statsCount() {
        document.querySelectorAll('.stat-number[data-target]').forEach(el => {
            const target = parseFloat(el.dataset.target);
            gsap.fromTo({ v: 0 }, { v: target }, {
                duration: 2,
                ease: 'power1.out',
                onUpdate() {
                    el.textContent = Math.round(this.targets()[0].v);
                },
                scrollTrigger: {
                    trigger: el,
                    start: 'top 85%',
                    toggleActions: 'play none none none',
                },
            });
        });
    })();

    stagger('#stats-section .stat-item', {
        fromY: 40, fromScale: 0.9, toScale: 1,
        stagger: 0.10, duration: 0.55,
    });


    /* ═══════════════════════════════════════════
       11. TECH STACK
       ═══════════════════════════════════════════ */
    (function techAlt() {
        document.querySelectorAll('.tech-card').forEach((card, i) => {
            gsap.fromTo(card,
                { x: i % 2 === 0 ? -45 : 45, opacity: 0 },
                {
                    x: 0, opacity: 1,
                    duration: 0.6,
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: card,
                        start: 'top 88%',
                        toggleActions: 'play none none reverse',
                    },
                    delay: (i % 3) * 0.06,
                }
            );
        });
    })();


    /* ═══════════════════════════════════════════
       12. CTA
       ═══════════════════════════════════════════ */
    (function ctaAnim() {
        const box = document.querySelector('.cta-box');
        if (!box) return;
        gsap.fromTo(box,
            { y: 70, opacity: 0, scale: 0.96 },
            {
                y: 0, opacity: 1, scale: 1,
                duration: 0.85,
                ease: 'back.out(1.3)',
                scrollTrigger: {
                    trigger: '#cta',
                    start: 'top 80%',
                    toggleActions: 'play none none reverse',
                },
            }
        );
    })();


    /* ═══════════════════════════════════════════
       13. SMOOTH ANCHOR SCROLL
       ═══════════════════════════════════════════ */
    document.querySelectorAll('a[href^="#"]').forEach(a => {
        a.addEventListener('click', e => {
            const t = document.querySelector(a.getAttribute('href'));
            if (!t) return;
            e.preventDefault();
            gsap.to(window, {
                scrollTo: { y: t, offsetY: 72 },
                duration: 1,
                ease: 'power3.inOut',
            });
        });
    });


    /* ═══════════════════════════════════════════
       14. CATCH-ALL REVEAL
       ═══════════════════════════════════════════ */
    document.querySelectorAll('.reveal:not(.gsap-done)').forEach(el => {
        el.classList.add('gsap-done');
        gsap.fromTo(el,
            { y: 35, opacity: 0 },
            {
                y: 0, opacity: 1,
                duration: 0.6,
                ease: 'power2.out',
                scrollTrigger: {
                    trigger: el,
                    start: 'top 87%',
                    toggleActions: 'play none none reverse',
                },
            }
        );
    });

})();
