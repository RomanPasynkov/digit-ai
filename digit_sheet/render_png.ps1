$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$samples = Get-Content -Raw -LiteralPath (Join-Path $PSScriptRoot 'digits_3x5_10x11.json') | ConvertFrom-Json
$cellSize = 22
$sampleWidth = 3 * $cellSize
$sampleHeight = 5 * $cellSize
$columnStride = 92
$rowStride = 150
$leftMargin = 82
$topMargin = 122
$imageWidth = $leftMargin + 11 * $columnStride + 42
$imageHeight = $topMargin + 10 * $rowStride + 44

$bitmap = [System.Drawing.Bitmap]::new($imageWidth, $imageHeight)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::White)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit

$ink = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(24, 35, 51))
$white = [System.Drawing.Brushes]::White
$gridPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(174, 184, 194), 1)
$framePen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(105, 119, 135), 2)
$testPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(207, 58, 58), 2)
$titleFont = [System.Drawing.Font]::new('Arial', 20, [System.Drawing.FontStyle]::Bold)
$labelFont = [System.Drawing.Font]::new('Arial', 15, [System.Drawing.FontStyle]::Bold)
$smallFont = [System.Drawing.Font]::new('Arial', 11)

try {
    $graphics.DrawString('Цифры 3×5 — оригинал, 9 обучающих, 1 тестовый', $titleFont, $ink, 36, 12)
    $graphics.DrawString('Чёрная клетка = закрасить; белая = оставить пустой.', $smallFont, $ink, 36, 50)
    $graphics.DrawString('ОРИГИНАЛ', $smallFont, $ink, 75, 78)
    $graphics.DrawString('ОБУЧЕНИЕ', $smallFont, $ink, 460, 78)
    $graphics.DrawString('ТЕСТ', $smallFont, $ink, 1015, 78)

    $dividerX = $leftMargin + 10 * $columnStride - 13
    $graphics.DrawLine($testPen, $dividerX, 88, $dividerX, $imageHeight - 37)

    for ($column = 0; $column -lt 11; $column++) {
        $labelX = $leftMargin + $column * $columnStride + 23
        $graphics.DrawString(($column + 1).ToString(), $smallFont, $ink, $labelX, 95)
    }

    for ($digit = 0; $digit -lt 10; $digit++) {
        $topY = $topMargin + $digit * $rowStride
        $graphics.DrawString($digit.ToString(), $labelFont, $ink, 36, $topY + 43)
        for ($column = 0; $column -lt 11; $column++) {
            $leftX = $leftMargin + $column * $columnStride
            $pattern = [string]$samples[$digit][$column]
            $pen = if ($column -eq 10) { $testPen } else { $framePen }
            $graphics.DrawRectangle($pen, $leftX - 2, $topY - 2, $sampleWidth + 3, $sampleHeight + 3)

            for ($row = 0; $row -lt 5; $row++) {
                for ($col = 0; $col -lt 3; $col++) {
                    $x = $leftX + $col * $cellSize
                    $y = $topY + $row * $cellSize
                    $brush = if ($pattern[$row * 3 + $col] -eq '#') { $ink } else { $white }
                    $graphics.FillRectangle($brush, $x, $y, $cellSize, $cellSize)
                    $graphics.DrawRectangle($gridPen, $x, $y, $cellSize, $cellSize)
                }
            }
        }
    }

    $bitmap.Save((Join-Path $PSScriptRoot 'digits_3x5_10x11.png'), [System.Drawing.Imaging.ImageFormat]::Png)
}
finally {
    $titleFont.Dispose()
    $labelFont.Dispose()
    $smallFont.Dispose()
    $gridPen.Dispose()
    $framePen.Dispose()
    $testPen.Dispose()
    $ink.Dispose()
    $graphics.Dispose()
    $bitmap.Dispose()
}
