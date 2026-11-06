import { useLayoutEffect, useRef, useState } from 'react';
import { createRoot } from 'react-dom/client';
import { Mascot } from 'page-mascot';

const host = document.getElementById('welcomeMascot');

function WelcomeMascot() {
    const [size, setSize] = useState(host.clientWidth);
    const wrapper = useRef(null);
    useLayoutEffect(() => {
        const observer = new ResizeObserver(() => setSize(host.clientWidth));
        observer.observe(host);
        // The package supplies an English-only accessible label.
        const button = wrapper.current.querySelector('button');
        button.setAttribute('aria-label', 'Chào CRT');
        button.title = 'Chào CRT';
        return () => observer.disconnect();
    }, []);
    return <div ref={wrapper}><Mascot directions={host.dataset.directions}
        reactions={host.dataset.reactions} size={size} label="CRT" /></div>;
}

if (host) {
    // Keep the static sprite visible until both sheets are decoded.
    Promise.all([host.dataset.directions, host.dataset.reactions].map(src => {
        const image = new Image();
        image.src = src;
        return image.decode();
    })).then(() => createRoot(host).render(<WelcomeMascot />)).catch(() => {});
}
