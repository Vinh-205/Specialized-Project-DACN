import { createMorph } from 'morphicons/dom';
import { icons } from './system-icon-data.generated.js';

const instances = new Map();
const ns = 'http://www.w3.org/2000/svg';
function update(element) {
    const key = [...element.classList].find(name => icons[name]);
    if (!key) return;
    let entry = instances.get(element);
    if (entry && !element.contains(entry.svg)) {
        entry.morph.destroy();
        instances.delete(element);
        entry = null;
    }
    if (!entry) {
        const svg = document.createElementNS(ns, 'svg');
        for (const [name, value] of Object.entries({viewBox:'0 0 24 24', fill:'none', stroke:'currentColor', 'stroke-width':'1.8', 'stroke-linecap':'round', 'stroke-linejoin':'round', 'aria-hidden':'true', focusable:'false'})) svg.setAttribute(name, value);
        const path = document.createElementNS(ns, 'path');
        svg.append(path);
        element.append(svg);
        const morph = createMorph(path, icons[key], { reducedMotion: 'user' });
        entry = { svg, morph, key };
        instances.set(element, entry);
        element.classList.add('morph-ready');
    } else if (key !== entry.key) {
        entry.morph.morphTo(icons[key], 'snappy');
        entry.key = key;
    }
    // Filled ratings must remain distinguishable from empty ratings.
    entry.svg.style.fill = key === 'bi-star-fill' || key === 'bi-circle-fill' ? 'currentColor' : 'none';
    const number = /^bi-([12])-circle$/.exec(key)?.[1];
    let text = entry.svg.querySelector('text');
    if (number && !text) {
        text = document.createElementNS(ns, 'text');
        for (const [name, value] of Object.entries({x:'12', y:'16', 'text-anchor':'middle', 'font-size':'12', 'font-style':'normal', fill:'currentColor', stroke:'none'})) text.setAttribute(name, value);
        entry.svg.append(text);
    }
    if (number && text.textContent !== number) text.textContent = number;
    if (!number) text?.remove();
    if (!element.classList.contains('morph-ready')) element.classList.add('morph-ready');
}
function scan(root) {
    if (!(root instanceof Element)) return;
    if (root.matches('.bi')) update(root);
    root.querySelectorAll('.bi').forEach(update);
}
scan(document.body);
new MutationObserver(records => {
    for (const record of records) {
        if (record.type === 'attributes') {
            if (record.target.matches('.bi')) update(record.target);
        } else {
            if (record.target instanceof Element && record.target.matches('.bi')) update(record.target);
            record.addedNodes.forEach(scan);
        }
    }
    for (const [element, entry] of instances) {
        if (!element.isConnected) { entry.morph.destroy(); instances.delete(element); }
    }
}).observe(document.body, { subtree:true, childList:true, attributes:true, attributeFilter:['class'] });

// Directional CTA icons morph without changing the meaning of the action.
const hoverPairs = { 'bi-arrow-right':'bi-arrow-up-right', 'bi-arrow-up-right':'bi-arrow-right' };
function emphasize(event, active) {
    const control = event.target.closest('a,button');
    if (!control || (event.relatedTarget instanceof Node && control.contains(event.relatedTarget))) return;
    control.querySelectorAll('.bi').forEach(element => {
        const entry = instances.get(element);
        if (entry && hoverPairs[entry.key]) entry.morph.morphTo(icons[active ? hoverPairs[entry.key] : entry.key], 'snappy');
    });
}
document.addEventListener('pointerover', event => emphasize(event, true));
document.addEventListener('pointerout', event => emphasize(event, false));
document.addEventListener('focusin', event => emphasize(event, true));
document.addEventListener('focusout', event => emphasize(event, false));
