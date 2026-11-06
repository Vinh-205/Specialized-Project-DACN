// Số liệu tổng quan "nhảy chữ số": mỗi chữ số là một cột cuộn từ 0 tới giá trị thật (lấy ý tưởng từ mẫu "Live metric with rolling digits").
// Chỉ đổi cách hiển thị, giá trị và định dạng giữ nguyên (dấu chấm ngăn cách, ký hiệu ₫...). Người dùng bật "giảm chuyển động" thì hiện thẳng.
(function () {
    'use strict';
    var reduce = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    function roll(el) {
        var node = null;
        for (var n = el.firstChild; n; n = n.nextSibling) {
            if (n.nodeType === 3 && /^\s*\d[\d.,]*/.test(n.nodeValue)) { node = n; break; }
        }
        if (!node) return;
        var m = /^(\s*)(\d[\d.,]*)([\s\S]*)$/.exec(node.nodeValue);
        var wrap = document.createElement('span');
        wrap.className = 'rd';
        wrap.setAttribute('aria-label', m[2]);
        var strips = [];
        for (var i = 0; i < m[2].length; i++) {
            var ch = m[2].charAt(i), part = document.createElement('span');
            part.setAttribute('aria-hidden', 'true');
            if (/\d/.test(ch)) {
                part.className = 'rd-col';
                var strip = document.createElement('span');
                strip.className = 'rd-strip';
                strip.style.setProperty('--d', ch);
                for (var k = 0; k < 10; k++) { var s = document.createElement('span'); s.textContent = k; strip.appendChild(s); }
                part.appendChild(strip);
                strips.push(strip);
            } else {
                part.className = 'rd-sep';
                part.textContent = ch;
            }
            wrap.appendChild(part);
        }
        var frag = document.createDocumentFragment();
        if (m[1]) frag.appendChild(document.createTextNode(m[1]));
        frag.appendChild(wrap);
        if (m[3]) frag.appendChild(document.createTextNode(m[3]));
        node.parentNode.replaceChild(frag, node);
        if (reduce) { strips.forEach(function (s) { s.classList.add('on'); }); return; }
        void wrap.offsetWidth; // ép tính kiểu ban đầu (cột đang ở số 0) trước khi cuộn
        strips.forEach(function (s, idx) { setTimeout(function () { s.classList.add('on'); }, 80 + idx * 70); });
    }
    function run() {
        document.querySelectorAll('[data-roll], .dh-hero-num, .dh-status-row strong, .dh-stat-row > strong, .dh-line-num').forEach(roll);
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', run); else run();
})();
