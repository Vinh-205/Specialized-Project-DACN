// ==========================================
// PAGE TRANSITION & FADE EFFECTS UTILITY
// ENHANCED WITH GRADIENTS, PARTICLES, PREMIUM EFFECTS
// ==========================================

/**
 * Create premium fade-out overlay with particles and gradient
 * @param {string} message - Optional message to display
 * @param {string} type - Type: 'loading', 'logout', 'redirect', 'success', 'error'
 * @returns {HTMLElement} - The overlay element
 */
function createFadeOverlay(message = '', type = 'loading') {
    const overlay = document.createElement('div');
    overlay.id = 'fadeOverlay';
    
    // Gradient background based on type
    let bgGradient = 'linear-gradient(135deg, rgba(6,13,31,0.98) 0%, rgba(8,50,87,0.95) 100%)';
    let spinnerColor = '#38bdf8';
    let spinnerBg = 'rgba(56,189,248,0.15)';
    let messageText = 'Đang xử lý...';
    let subText = 'Vui lòng chờ một chút';
    let accentColor = '#38bdf8';
    
    if (type === 'logout') {
        bgGradient = 'linear-gradient(135deg, rgba(6,13,31,0.98) 0%, rgba(87,8,50,0.95) 100%)';
        spinnerColor = '#ef4444';
        spinnerBg = 'rgba(239,68,68,0.15)';
        messageText = 'Đang đăng xuất';
        subText = 'Tạm biệt bạn!';
        accentColor = '#ef4444';
    } else if (type === 'success') {
        bgGradient = 'linear-gradient(135deg, rgba(6,13,31,0.98) 0%, rgba(8,87,30,0.95) 100%)';
        spinnerColor = '#22c55e';
        spinnerBg = 'rgba(34,197,94,0.15)';
        messageText = message || 'Thành công!';
        subText = 'Đang vào hệ thống...';
        accentColor = '#22c55e';
    } else if (type === 'error') {
        bgGradient = 'linear-gradient(135deg, rgba(6,13,31,0.98) 0%, rgba(87,20,8,0.95) 100%)';
        spinnerColor = '#f59e0b';
        spinnerBg = 'rgba(245,158,11,0.15)';
        messageText = 'Lỗi';
        subText = message || 'Vui lòng thử lại';
        accentColor = '#f59e0b';
    }
    
    const styles = {
        position: 'fixed',
        inset: '0',
        background: bgGradient,
        backdropFilter: 'blur(0px)',
        WebkitBackdropFilter: 'blur(0px)',
        zIndex: '99999',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        color: '#fff',
        transition: 'all 0.5s cubic-bezier(0.25, 0.46, 0.45, 0.94)',
        opacity: '0',
        fontFamily: "'Be Vietnam Pro', sans-serif"
    };
    
    Object.assign(overlay.style, styles);
    
    // Content with premium styling
    overlay.innerHTML = `
        <div style="text-align: center; position: relative; z-index: 10;">
            <div id="overlaySpinner" style="
                width: 70px; 
                height: 70px; 
                margin-bottom: 28px; 
                border: 3px solid ${spinnerBg}; 
                border-top-color: ${spinnerColor}; 
                border-radius: 50%; 
                animation: spin 1.5s linear infinite;
                box-shadow: 0 0 30px ${spinnerColor}40, inset 0 0 30px ${spinnerColor}20;
            "></div>
            <div id="overlayMessage" style="
                font-size: 22px; 
                font-weight: 700; 
                margin-bottom: 12px; 
                letter-spacing: 0.3px;
                color: #fff;
                text-shadow: 0 2px 8px rgba(0,0,0,0.3);
            ">${messageText}</div>
            <div id="overlaySubtext" style="
                font-size: 14px; 
                color: rgba(255,255,255,0.6);
                letter-spacing: 0.2px;
            ">${subText}</div>
        </div>
        <div id="overlayParticles" style="position: absolute; inset: 0; pointer-events: none;"></div>
    `;
    
    // Add advanced animations
    if (!document.getElementById('advancedAnimStyle')) {
        const style = document.createElement('style');
        style.id = 'advancedAnimStyle';
        style.textContent = `
            @keyframes spin { 
                to { transform: rotate(360deg); } 
            }
            @keyframes float {
                0%, 100% { transform: translateY(0px); }
                50% { transform: translateY(-15px); }
            }
            @keyframes pulse {
                0%, 100% { opacity: 0.4; transform: scale(0.95); }
                50% { opacity: 0.8; transform: scale(1.05); }
            }
            @keyframes particleFloat {
                0% { opacity: 0; transform: translateY(100px) translateX(0) scale(1); }
                10% { opacity: 1; }
                90% { opacity: 1; }
                100% { opacity: 0; transform: translateY(-120px) translateX(var(--tx)) scale(0); }
            }
        `;
        document.head.appendChild(style);
    }
    
    document.body.appendChild(overlay);
    
    // Enhanced fade in with blur
    setTimeout(() => { 
        overlay.style.opacity = '1';
        overlay.style.backdropFilter = 'blur(12px)';
        overlay.style.WebkitBackdropFilter = 'blur(12px)';
    }, 10);
    
    // Add floating particles
    const particleContainer = overlay.querySelector('#overlayParticles');
    if (particleContainer) {
        for (let i = 0; i < 6; i++) {
            const particle = document.createElement('div');
            particle.style.cssText = `
                position: absolute;
                width: ${Math.random() * 8 + 4}px;
                height: ${Math.random() * 8 + 4}px;
                background: ${spinnerColor};
                border-radius: 50%;
                left: ${Math.random() * 100}%;
                bottom: ${Math.random() * 50}%;
                opacity: 0.3;
                filter: blur(1px);
                animation: particleFloat ${Math.random() * 3 + 2}s ease-in infinite;
                --tx: ${(Math.random() - 0.5) * 100}px;
                animation-delay: ${Math.random() * 0.3}s;
            `;
            particleContainer.appendChild(particle);
        }
    }
    
    return overlay;
}

/**
 * Fade out page and redirect
 * @param {string} url - Target URL to redirect to
 * @param {string} message - Optional message
 * @param {string} type - Type of fade (logout, redirect)
 */
function fadeOutAndRedirect(url, message = '', type = 'redirect') {
    const overlay = createFadeOverlay(message, type);
    
    setTimeout(() => {
        window.location.href = url;
    }, 1800);
}

/**
 * Add fade out effect on page unload (for all links)
 */
function setupPageTransitions() {
    // Add fade effect to regular links (except external, target=_blank, anchor links)
    document.addEventListener('click', function(e) {
        const link = e.target.closest('a');
        
        if (!link) return;
        
        const href = link.getAttribute('href');
        const target = link.getAttribute('target');
        const onclick = link.getAttribute('onclick');
        
        // Skip: external links, new tabs, anchor links, onclick handlers, logout links
        if (!href || target === '_blank' || href.startsWith('#') || onclick || link.id === 'logoutLink' || link.classList.contains('no-fade')) {
            return;
        }
        
        // Only apply fade to internal navigation links
        if (href.startsWith('http') && !href.includes(window.location.host)) {
            return;
        }
        
        // For logout, don't prevent default here - let it handle separately
        if (href.includes('Logout')) {
            return;
        }
        
        e.preventDefault();
        
        const overlay = createFadeOverlay('Đang tải trang...', 'redirect');
        
        setTimeout(() => {
            window.location.href = href;
        }, 1200);
    });
}

/**
 * Add fade effect to logout action
 */
function setupLogoutFade() {
    const logoutElements = document.querySelectorAll('[href*="Logout"], [href*="logout"]');
    
    logoutElements.forEach(el => {
        el.addEventListener('click', function(e) {
            e.preventDefault();
            
            const href = this.getAttribute('href');
            const overlay = createFadeOverlay('Đang đăng xuất...', 'logout');
            
            setTimeout(() => {
                window.location.href = href;
            }, 1600);
        });
    });
}

/**
 * Initialize all transition effects
 */
function initializeTransitionEffects() {
    // Only setup in non-login pages
    if (!window.location.href.includes('/Auth/Login')) {
        setupPageTransitions();
        setupLogoutFade();
    }
}

// Initialize on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeTransitionEffects);
} else {
    initializeTransitionEffects();
}
