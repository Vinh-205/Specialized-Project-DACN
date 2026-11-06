import { readdir, readFile } from 'node:fs/promises';
import { join } from 'node:path';
import assert from 'node:assert/strict';
import { iconGroups } from '../Client/icon-map.mjs';

const aliases = Object.values(iconGroups).flatMap(value => value.split(' '));
assert.equal(new Set(aliases).size, aliases.length, 'Duplicate icon aliases');
const known = new Set(aliases.map(name => `bi-${name}`));
const missing = new Set();
async function scan(directory) {
    for (const entry of await readdir(directory, { withFileTypes:true })) {
        const path = join(directory, entry.name);
        if (entry.isDirectory()) { await scan(path); continue; }
        if (!/\.(cshtml|js)$/.test(path) || path.endsWith('.bundle.js')) continue;
        const source = await readFile(path, 'utf8');
        for (const [name] of source.matchAll(/\bbi-[a-z0-9]+(?:-[a-z0-9]+)*/g)) {
            if (!known.has(name)) missing.add(`${path}: ${name}`);
        }
    }
}
await scan('Views');
await scan('wwwroot/js');
assert.deepEqual([...missing], [], 'Unmapped legacy icons');
console.log(`PASS: ${known.size} aliases; all literal view/script icons mapped.`);
