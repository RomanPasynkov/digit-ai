$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$samples = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'photo_samples_2026-11-17.json') -Raw | ConvertFrom-Json
$bitmap = [System.Drawing.Bitmap]::new(780, 1010)
$g = [System.Drawing.Graphics]::FromImage($bitmap)
$font = [System.Drawing.Font]::new('Arial', 10)
$digitFont = [System.Drawing.Font]::new('Arial', 14, [System.Drawing.FontStyle]::Bold)
$gridPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(150, 150, 150), 1)
$testPen = [System.Drawing.Pen]::new([System.Drawing.Color]::Firebrick, 2)
try {
    $g.Clear([System.Drawing.Color]::White)
    $g.DrawString('1-10: train.txt     11: test.txt', $digitFont, [System.Drawing.Brushes]::Black, 35, 10)
    for ($example = 0; $example -lt 11; $example++) {
        $g.DrawString([string]($example + 1), $font, [System.Drawing.Brushes]::Black, 62 + $example * 64, 45)
    }
    for ($digit = 0; $digit -lt 10; $digit++) {
        $y = 75 + $digit * 92
        $g.DrawString([string]$digit, $digitFont, [System.Drawing.Brushes]::Black, 15, $y + 18)
        for ($example = 0; $example -lt 11; $example++) {
            $rows = ([string]$samples[$digit][$example]).Split('/')
            $x = 50 + $example * 64
            for ($row = 0; $row -lt 5; $row++) {
                for ($col = 0; $col -lt 3; $col++) {
                    $rect = [System.Drawing.Rectangle]::new($x + $col * 12, $y + $row * 12, 12, 12)
                    if ($rows[$row][$col] -eq '#') { $g.FillRectangle([System.Drawing.Brushes]::Black, $rect) }
                    $g.DrawRectangle($gridPen, $rect)
                }
            }
            if ($example -eq 10) { $g.DrawRectangle($testPen, $x - 3, $y - 3, 42, 66) }
        }
    }
    $bitmap.Save((Join-Path $PSScriptRoot 'photo_samples_check.png'))
}
finally { $testPen.Dispose(); $gridPen.Dispose(); $digitFont.Dispose(); $font.Dispose(); $g.Dispose(); $bitmap.Dispose() }
