// Creates a printable A4 graph-paper sheet from the curated 3 x 5 samples.
// Run after generate.js: node digit_sheet/generate_print.js
const fs = require('fs');
const path = require('path');

const samples = JSON.parse(fs.readFileSync(path.join(__dirname, 'digits_3x5_10x11.json'), 'utf8'));
const pageWidth = 210;
const pageHeight = 297;
const cell = 3.3;
const columns = 53; // 11 glyphs * 3 cells + 10 gaps * 2 cells
const rows = 68;    // 10 glyphs * 5 cells + 9 gaps * 2 cells
const originX = (pageWidth - columns * cell) / 2;
const originY = (pageHeight - rows * cell) / 2;

if (samples.length !== 10 || samples.some(row => row.length !== 11)) {
  throw new Error('Expected 10 rows with 11 digit samples each');
}

const svg = [
  `<svg xmlns="http://www.w3.org/2000/svg" width="210mm" height="297mm" viewBox="0 0 210 297">`,
  '<rect width="210" height="297" fill="white"/>',
];

// One continuous sheet of squared paper, including the empty cells between glyphs.
for (let x = 0; x <= columns; x++) {
  const position = originX + x * cell;
  svg.push(`<line x1="${position.toFixed(3)}" y1="${originY.toFixed(3)}" x2="${position.toFixed(3)}" y2="${(originY + rows * cell).toFixed(3)}" stroke="#a8afb6" stroke-width="0.15"/>`);
}
for (let y = 0; y <= rows; y++) {
  const position = originY + y * cell;
  svg.push(`<line x1="${originX.toFixed(3)}" y1="${position.toFixed(3)}" x2="${(originX + columns * cell).toFixed(3)}" y2="${position.toFixed(3)}" stroke="#a8afb6" stroke-width="0.15"/>`);
}

for (let digit = 0; digit < 10; digit++) {
  for (let example = 0; example < 11; example++) {
    const pattern = samples[digit][example];
    if (pattern.length !== 15) throw new Error(`Wrong size at ${digit}/${example + 1}`);
    for (let row = 0; row < 5; row++) {
      for (let col = 0; col < 3; col++) {
        if (pattern[row * 3 + col] !== '#') continue;
        const x = originX + (example * 5 + col) * cell;
        const y = originY + (digit * 7 + row) * cell;
        svg.push(`<rect x="${x.toFixed(3)}" y="${y.toFixed(3)}" width="${cell}" height="${cell}" fill="#161616"/>`);
      }
    }
  }
}

const originalSeparatorX = originX + 4 * cell;
svg.push(`<line x1="${originalSeparatorX.toFixed(3)}" y1="${originY.toFixed(3)}" x2="${originalSeparatorX.toFixed(3)}" y2="${(originY + rows * cell).toFixed(3)}" stroke="#5682a3" stroke-width="0.24"/>`);
const testSeparatorX = originX + 49 * cell;
svg.push(`<line x1="${testSeparatorX.toFixed(3)}" y1="${originY.toFixed(3)}" x2="${testSeparatorX.toFixed(3)}" y2="${(originY + rows * cell).toFixed(3)}" stroke="#d34848" stroke-width="0.28"/>`);
svg.push('</svg>');

fs.writeFileSync(path.join(__dirname, 'digits_print_a4.svg'), svg.join('\n'), 'utf8');
console.log('Created A4 SVG: 110 digits on one 53 x 68 graph-paper grid.');
