(() => {
    'use strict';
    const dialog = document.getElementById('welcomeVideoDialog');
    const video = document.getElementById('welcomeVideo');
    document.getElementById('openWelcomeVideo').addEventListener('click', () => {
        dialog.showModal();
        video.play().catch(() => {});
    });
    document.getElementById('closeWelcomeVideo').addEventListener('click', () => dialog.close());
    dialog.addEventListener('close', () => video.pause());
    dialog.addEventListener('click', event => {
        const r = dialog.getBoundingClientRect();
        if (event.clientX < r.left || event.clientX > r.right || event.clientY < r.top || event.clientY > r.bottom) dialog.close();
    });
})();
