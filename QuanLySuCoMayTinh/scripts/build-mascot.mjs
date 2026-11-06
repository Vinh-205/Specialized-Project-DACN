import { build } from 'esbuild';

await build({
    entryPoints: ['Client/welcome-mascot.jsx'],
    outfile: 'wwwroot/js/welcome-mascot.bundle.js',
    bundle: true,
    minify: true,
    jsx: 'automatic',
    define: { 'process.env.NODE_ENV': '"production"' },
    legalComments: 'eof',
    target: ['es2020']
});
