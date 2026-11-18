// Generates a 10 x 11 sheet of distinct 3 x 5 digit samples.
// Run with: node digit_sheet/generate.js
const fs = require('fs');
const path = require('path');
const zlib = require('zlib');

const models = [
  { digit: 0, rows: ['.#.', '#.#', '#.#', '#.#', '.#.'], change: [0, 2, 12, 14], original: '###/#.#/#.#/#.#/###' },
  { digit: 1, rows: ['.#.', '##.', '.#.', '.#.', '###'], change: [0, 2, 12, 14, 5, 8] },
  { digit: 2, rows: ['###', '..#', '###', '#..', '###'], change: [0, 2, 12, 14, 4, 10] },
  { digit: 3, rows: ['###', '..#', '###', '..#', '###'], change: [0, 2, 12, 14, 10] },
  { digit: 4, rows: ['#.#', '#.#', '###', '..#', '..#'], change: [0, 2, 12, 13, 10] },
  { digit: 5, rows: ['###', '#..', '###', '..#', '###'], change: [0, 2, 12, 14, 4, 10] },
  { digit: 6, rows: ['###', '#..', '###', '#.#', '###'], change: [0, 2, 12, 14, 4, 10] },
  {
    digit: 7,
    rows: ['###', '..#', '.##', '#..', '#..'],
    examples: [
      '###/..#/.##/#../#..',
      '.##/..#/.##/#../#..',
      '##./..#/.##/#../#..',
      '###/.##/.##/#../#..',
      '###/..#/.##/##./#..',
      '###/..#/.##/#../##.',
      '.##/.##/.##/#../#..',
      '##./.##/.##/#../#..',
      '.##/..#/.##/##./#..',
      '##./..#/.##/##./#..',
      '###/.##/.##/##./#..',
    ],
  },
  { digit: 8, rows: ['.#.', '#.#', '###', '#.#', '.#.'], change: [0, 2, 12, 14], original: '###/#.#/###/#.#/###' },
  { digit: 9, rows: ['###', '#.#', '###', '..#', '###'], change: [0, 2, 12, 14, 10] },
];

function flip(pattern, positions) {
  const cells = pattern.split('');
  for (const position of positions) cells[position] = cells[position] === '#' ? '.' : '#';
  return cells.join('');
}

function coversFrame(pattern) {
  return pattern.slice(0, 3).includes('#') &&
    pattern.slice(12, 15).includes('#') &&
    [0, 3, 6, 9, 12].some(p => pattern[p] === '#') &&
    [2, 5, 8, 11, 14].some(p => pattern[p] === '#');
}

function hamming(a, b) {
  let distance = 0;
  for (let i = 0; i < 15; i++) if (a[i] !== b[i]) distance++;
  return distance;
}

const bases = models.map(model => model.rows.join(''));
const samples = models.map((model, digit) => {
  if (model.examples) return model.examples.map(example => example.replace(/\//g, ''));

  const base = bases[digit];
  const masks = [[]];
  for (const p of model.change) masks.push([p]);
  for (let a = 0; a < model.change.length; a++) {
    for (let b = a + 1; b < model.change.length; b++) {
      masks.push([model.change[a], model.change[b]]);
    }
  }

  const variants = [];
  for (const mask of masks) {
    const pattern = flip(base, mask);
    if (!coversFrame(pattern)) continue;
    if (variants.includes(pattern)) continue;
    // Keep each drawing closer to its own digit than to any other base glyph.
    const own = hamming(pattern, base);
    const nearestOther = Math.min(...bases.filter((_, i) => i !== digit)
      .map(other => hamming(pattern, other)));
    if (own >= nearestOther) continue;
    variants.push(pattern);
    if (variants.length === 11) break;
  }
  if (variants.length !== 11) {
    throw new Error(`Digit ${digit}: only ${variants.length} suitable variants`);
  }
  if (model.original) {
    return [model.original.replace(/\//g, ''), ...variants.slice(0, 10)];
  }
  return variants;
});

const seen = new Map();
for (let digit = 0; digit < 10; digit++) {
  for (let column = 0; column < 11; column++) {
    const pattern = samples[digit][column];
    if (pattern.length !== 15) throw new Error(`Digit ${digit}, sample ${column + 1} is not 3 x 5`);
    if (!coversFrame(pattern)) throw new Error(`Digit ${digit}, sample ${column + 1} does not fill 3 x 5`);
    if (seen.has(pattern)) throw new Error(`Duplicate ${digit}/${column + 1} and ${seen.get(pattern)}`);
    seen.set(pattern, `${digit}/${column + 1}`);
  }
}
if (samples[3].some(pattern => pattern.slice(6, 9) !== '###')) {
  throw new Error('Every variant of three must retain the full middle row');
}
if (samples[7].some(pattern => pattern.slice(6, 9) !== '.##')) {
  throw new Error('All training and test variants of seven must keep the middle bar');
}
if (samples[7].some(pattern => pattern[9] !== '#' || pattern[12] !== '#')) {
  throw new Error('Every variant of seven must keep the left lower stem');
}

const cell = 22;
const sampleWidth = 3 * cell;
const sampleHeight = 5 * cell;
const columnStride = 92;
const rowStride = 150;
const left = 82;
const top = 122;
const width = left + 11 * columnStride + 42;
const height = top + 10 * rowStride + 44;

const svg = [];
svg.push(`<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">`);
svg.push('<rect width="100%" height="100%" fill="white"/>');
svg.push('<style>text{font-family:Arial,sans-serif;fill:#182333}.title{font-size:24px;font-weight:700}.label{font-size:18px;font-weight:700}.small{font-size:14px}</style>');
svg.push('<text x="36" y="37" class="title">Цифры 3×5 — оригинал, 9 обучающих, 1 тестовый</text>');
svg.push('<text x="36" y="65" class="small">Чёрная клетка = закрасить; белая = оставить пустой. Строка слева указывает правильную цифру.</text>');
svg.push(`<text x="${left + sampleWidth / 2}" y="83" text-anchor="middle" class="small">ОРИГИНАЛ</text>`);
svg.push(`<text x="${left + 5 * columnStride}" y="83" text-anchor="middle" class="small">ОБУЧЕНИЕ</text>`);
svg.push(`<text x="${left + 10 * columnStride + sampleWidth / 2}" y="83" text-anchor="middle" class="small" fill="#bb3131">ТЕСТ</text>`);
svg.push(`<line x1="${left + 10 * columnStride - 13}" y1="88" x2="${left + 10 * columnStride - 13}" y2="${height - 37}" stroke="#cf3a3a" stroke-width="3"/>`);
for (let column = 0; column < 11; column++) {
  const x = left + column * columnStride + sampleWidth / 2;
  svg.push(`<text x="${x}" y="109" class="small" text-anchor="middle">${column + 1}</text>`);
}

for (let digit = 0; digit < 10; digit++) {
  const y = top + digit * rowStride;
  svg.push(`<text x="42" y="${y + 67}" class="label" text-anchor="middle">${digit}</text>`);
  for (let column = 0; column < 11; column++) {
    const x = left + column * columnStride;
    const pattern = samples[digit][column];
    svg.push(`<rect x="${x - 2}" y="${y - 2}" width="${sampleWidth + 4}" height="${sampleHeight + 4}" fill="white" stroke="${column === 10 ? '#cf3a3a' : '#697787'}" stroke-width="1.5"/>`);
    for (let row = 0; row < 5; row++) {
      for (let col = 0; col < 3; col++) {
        const px = x + col * cell;
        const py = y + row * cell;
        const black = pattern[row * 3 + col] === '#';
        svg.push(`<rect x="${px}" y="${py}" width="${cell}" height="${cell}" fill="${black ? '#182333' : '#ffffff'}" stroke="#aeb8c2" stroke-width="1"/>`);
      }
    }
  }
}
svg.push('</svg>');

const outputDir = __dirname;
fs.writeFileSync(path.join(outputDir, 'digits_3x5_10x11.svg'), svg.join('\n'), 'utf8');
fs.writeFileSync(path.join(outputDir, 'digits_3x5_10x11.json'), JSON.stringify(samples, null, 2) + '\n');
console.log('Created 110 unique samples; all touch top, bottom, left, and right.');
for (let digit = 0; digit < 10; digit++) {
  console.log(`${digit}: ${samples[digit].map(p => p.match(/.{3}/g).join('/')).join('  ')}`);
}
