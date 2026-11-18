param([string]$PhotoPath)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$outputDirectory = Join-Path $PSScriptRoot 'photo-reading'
New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
$source = [System.Drawing.Bitmap]::new($PhotoPath)
$starts = @(
    @(137,202,267,332,396,459,521,583,646,707,767),
    @(134,199,265,330,394,457,521,584,646,709,770),
    @(129,196,263,328,392,456,520,584,647,710,772),
    @(125,192,258,325,391,455,519,583,648,711,773),
    @(119,187,255,322,389,455,519,584,648,712,775),
    @(114,183,251,319,387,453,519,585,650,713,776),
    @(108,178,247,316,385,452,519,586,651,715,779),
    @(103,174,243,313,382,450,519,586,653,716,781),
    @(98,169,239,308,379,448,518,586,654,718,782),
    @(94,165,236,306,376,446,515,584,653,718,782)
)
$topRows = @(167,262,360,459,560,704,809,916,1026,1137)
$slopes = @(3.1,2.9,2.4,2.0,1.7,1.1,0.6,0.1,-0.5,-0.8)
$heights = @(71,71,72,73,74,76,77,79,80,81)
function Get-Light([double]$x, [double]$y) {
    $sum = 0.0
    for ($dy = -2; $dy -le 2; $dy++) {
        for ($dx = -2; $dx -le 2; $dx++) {
            $color = $source.GetPixel([int][Math]::Round($x + $dx), [int][Math]::Round($y + $dy))
            $sum += ($color.R + $color.G + $color.B) / 3.0
        }
    }
    return $sum / 25.0
}
try {
    $focus = [System.Drawing.Bitmap]::new(1920, 600)
    $focusGraphics = [System.Drawing.Graphics]::FromImage($focus)
    $focusGraphics.Clear([System.Drawing.Color]::White)
    $focusFont = [System.Drawing.Font]::new('Consolas', 12)
    $focusCases = @(@(4,10), @(5,7), @(7,7), @(7,10), @(8,2), @(8,7), @(8,8))
    try {
        for ($index = 0; $index -lt $focusCases.Count; $index++) {
            $digit = $focusCases[$index][0]
            $example = $focusCases[$index][1]
            $x = $starts[$digit][$example]
            $y = $topRows[$digit] + $slopes[$digit] * $example
            $width = 40.0 + $digit * 0.1 - $example * 0.35
            $height = $heights[$digit] - 0.45 * $example
            $left = $index * 270
            $focusGraphics.DrawString("$digit : $($example + 1)", $focusFont, [System.Drawing.Brushes]::Black, $left + 15, 0)
            $focusGraphics.DrawImage($source, [System.Drawing.Rectangle]::new($left + 5, 35, 245, 510), [single]($x - 6), [single]($y - 6), [single]($width + 12), [single]($height + 12), [System.Drawing.GraphicsUnit]::Pixel)
            for ($row = 0; $row -lt 5; $row++) {
                for ($col = 0; $col -lt 3; $col++) {
                    $cx = $left + 5 + (6 + ($col + 0.5) * $width / 3 - $row * 0.15) / ($width + 12) * 245
                    $cy = 35 + (6 + ($row + 0.5) * $height / 5) / ($height + 12) * 510
                    $focusGraphics.DrawEllipse([System.Drawing.Pens]::Red, [single]($cx - 4), [single]($cy - 4), 8, 8)
                }
            }
        }
        $focus.Save((Join-Path $outputDirectory 'focus.png'))
    }
    finally { $focusFont.Dispose(); $focusGraphics.Dispose(); $focus.Dispose() }
    for ($digit = 0; $digit -lt 10; $digit++) {
        $sheet = [System.Drawing.Bitmap]::new(1980, 480)
        $g = [System.Drawing.Graphics]::FromImage($sheet)
        $g.Clear([System.Drawing.Color]::White)
        $font = [System.Drawing.Font]::new('Consolas', 12)
        try {
            for ($example = 0; $example -lt 11; $example++) {
                $x = $starts[$digit][$example]
                $y = $topRows[$digit] + $slopes[$digit] * $example
                $width = 40.0 + $digit * 0.1 - $example * 0.35
                $height = $heights[$digit] - 0.45 * $example
                $paper = ((Get-Light ($x - 6) ($y + $height / 2)) + (Get-Light ($x + $width + 6) ($y + $height / 2))) / 2
                $pattern = ''
                $values = @()
                for ($row = 0; $row -lt 5; $row++) {
                    for ($col = 0; $col -lt 3; $col++) {
                        $light = Get-Light ($x + ($col + 0.5) * $width / 3 - $row * 0.15) ($y + ($row + 0.5) * $height / 5)
                        $values += [int][Math]::Round($light)
                        $pattern += $(if ($light -lt $paper * 0.83) { '#' } else { '.' })
                    }
                }
                $rows = for ($row = 0; $row -lt 5; $row++) { $pattern.Substring($row * 3, 3) }
                "$digit $($example + 1): $($rows -join '/') (paper=$([int]$paper)) values=$($values -join ',')"
                $left = $example * 180
                $g.DrawString("$digit : $($example + 1)", $font, [System.Drawing.Brushes]::Black, $left + 10, 0)
                $dest = [System.Drawing.Rectangle]::new($left + 10, 30, 150, 275)
                $g.DrawImage($source, $dest, [single]($x - 2), [single]($y - 2), [single]($width + 4), [single]($height + 4), [System.Drawing.GraphicsUnit]::Pixel)
                for ($row = 0; $row -lt 5; $row++) {
                    for ($col = 0; $col -lt 3; $col++) {
                        $rect = [System.Drawing.Rectangle]::new($left + 45 + $col * 22, 320 + $row * 22, 22, 22)
                        if ($pattern[$row * 3 + $col] -eq '#') { $g.FillRectangle([System.Drawing.Brushes]::Black, $rect) }
                        $g.DrawRectangle([System.Drawing.Pens]::Gray, $rect)
                    }
                }
            }
            $sheet.Save((Join-Path $outputDirectory "digit-$digit.png"))
        }
        finally { $font.Dispose(); $g.Dispose(); $sheet.Dispose() }
    }
}
finally { $source.Dispose() }
