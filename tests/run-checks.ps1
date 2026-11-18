param([string]$PreviewPath)

$ErrorActionPreference = 'Stop'
$projectDirectory = Join-Path $PSScriptRoot '..\working-version\PI32_Pasynkov_DigitAI'
$compiler = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
$checkDirectory = Join-Path ([System.IO.Path]::GetTempPath()) ('DigitAI-checks-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $checkDirectory | Out-Null
$checkExe = Join-Path $checkDirectory 'NetworkChecks.exe'
$compileArguments = @('/nologo', '/target:exe', '/optimize+', "/out:$checkExe", '/r:System.Core.dll', '/r:System.Drawing.dll', '/r:System.Windows.Forms.dll')
$compileArguments += (Get-ChildItem -LiteralPath (Join-Path $projectDirectory 'NeuroNet') -Filter '*.cs').FullName
$compileArguments += (Join-Path $projectDirectory 'Form1.cs'), (Join-Path $projectDirectory 'Form1.Designer.cs'), (Join-Path $PSScriptRoot 'NetworkChecks.cs')
& $compiler @compileArguments
if ($LASTEXITCODE -ne 0) { throw 'Check compilation failed.' }
& $checkExe $checkDirectory
if ($LASTEXITCODE -ne 0) { throw 'Network checks failed.' }
if ($PreviewPath) { Copy-Item -LiteralPath (Join-Path $checkDirectory 'form-preview.png') -Destination $PreviewPath }
Write-Output "Check artifacts: $checkDirectory"
