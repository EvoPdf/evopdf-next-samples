$sourceFontsPath = Join-Path $env:SystemRoot "Fonts"
$targetFontsPath = ".\Fonts"
if (-not (Test-Path $targetFontsPath)) {
    New-Item -ItemType Directory -Path $targetFontsPath | Out-Null
}
# Copy font files
Get-ChildItem $sourceFontsPath -Include *.ttf, *.ttc, *.otf -Recurse |
    Where-Object Name -ne 'lucon.ttf' |
    Copy-Item -Destination $targetFontsPath -Force
