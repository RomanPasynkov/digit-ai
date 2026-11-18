param([string]$PreviewPath)

$ErrorActionPreference = 'Stop'
$projectDirectory = Join-Path $PSScriptRoot '..\PI32_Pasynkov_DigitAI'
$compiler = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
$checkDirectory = Join-Path ([System.IO.Path]::GetTempPath()) ('DigitAI-photo-checks-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $checkDirectory | Out-Null
Copy-Item -LiteralPath (Join-Path $projectDirectory 'train.txt'), (Join-Path $projectDirectory 'test.txt') -Destination $checkDirectory
$checkExe = Join-Path $checkDirectory 'PhotoVersionChecks.exe'
$compileArguments = @('/nologo', '/target:exe', '/optimize+', "/out:$checkExe", '/r:System.Core.dll', '/r:System.Drawing.dll', '/r:System.Windows.Forms.dll')
$compileArguments += (Get-ChildItem -LiteralPath (Join-Path $projectDirectory 'NeuroNet') -Filter '*.cs').FullName
$compileArguments += (Join-Path $projectDirectory 'Form1.cs'), (Join-Path $projectDirectory 'Form1.Designer.cs'), (Join-Path $PSScriptRoot 'PhotoVersionChecks.cs')
& $compiler @compileArguments
if ($LASTEXITCODE -ne 0) { throw 'Photo version check compilation failed.' }
& $checkExe
if ($LASTEXITCODE -ne 0) { throw 'Photo version checks failed.' }
if ($PreviewPath) { Copy-Item -LiteralPath (Join-Path $checkDirectory 'form-preview.png') -Destination $PreviewPath }
