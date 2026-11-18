$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$samples = Get-Content -Raw -LiteralPath (Join-Path $PSScriptRoot 'digits_3x5_10x11.json') | ConvertFrom-Json
$pageWidth = 2480
$pageHeight = 3508
$cellSize = 39
$gridColumns = 53
$gridRows = 68
$originX = [int](($pageWidth - $gridColumns * $cellSize) / 2)
$originY = [int](($pageHeight - $gridRows * $cellSize) / 2)

$bitmap = [System.Drawing.Bitmap]::new($pageWidth, $pageHeight)
$bitmap.SetResolution(300, 300)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::White)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None

$gridPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(168, 175, 182), 2)
$originalPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(86, 130, 163), 3)
$separatorPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(211, 72, 72), 3)
$ink = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(22, 22, 22))

try {
    for ($x = 0; $x -le $gridColumns; $x++) {
        $position = $originX + $x * $cellSize
        $graphics.DrawLine($gridPen, $position, $originY, $position, $originY + $gridRows * $cellSize)
    }
    for ($y = 0; $y -le $gridRows; $y++) {
        $position = $originY + $y * $cellSize
        $graphics.DrawLine($gridPen, $originX, $position, $originX + $gridColumns * $cellSize, $position)
    }

    for ($digit = 0; $digit -lt 10; $digit++) {
        for ($example = 0; $example -lt 11; $example++) {
            $pattern = [string]$samples[$digit][$example]
            if ($pattern.Length -ne 15) { throw "Wrong pattern length at digit $digit, example $example" }
            for ($row = 0; $row -lt 5; $row++) {
                for ($col = 0; $col -lt 3; $col++) {
                    if ($pattern[$row * 3 + $col] -ne '#') { continue }
                    $x = $originX + ($example * 5 + $col) * $cellSize
                    $y = $originY + ($digit * 7 + $row) * $cellSize
                    $graphics.FillRectangle($ink, $x, $y, $cellSize, $cellSize)
                }
            }
        }
    }

    $originalSeparatorX = $originX + 4 * $cellSize
    $graphics.DrawLine($originalPen, $originalSeparatorX, $originY, $originalSeparatorX, $originY + $gridRows * $cellSize)
    $testSeparatorX = $originX + 49 * $cellSize
    $graphics.DrawLine($separatorPen, $testSeparatorX, $originY, $testSeparatorX, $originY + $gridRows * $cellSize)
    $bitmap.Save((Join-Path $PSScriptRoot 'digits_print_a4.png'), [System.Drawing.Imaging.ImageFormat]::Png)
}
finally {
    $gridPen.Dispose()
    $originalPen.Dispose()
    $separatorPen.Dispose()
    $ink.Dispose()
    $graphics.Dispose()
    $bitmap.Dispose()
}
