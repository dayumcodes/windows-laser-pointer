# Complete Release Workflow for GitHub
# This script builds and prepares everything for GitHub release

param(
    [string]$Version = "1.0.0",
    [string]$ReleaseNotes = "Initial release"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Laser Pointer - GitHub Release" -ForegroundColor Cyan
Write-Host "  Version: $Version" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Step 1: Create release package
Write-Host "Step 1: Building release package..." -ForegroundColor Yellow
& ".\Scripts\Create-Release.ps1" -Version $Version -ReleaseNotes $ReleaseNotes

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n❌ Release package creation failed!" -ForegroundColor Red
    exit 1
}

$releaseDir = ".\Releases\v$Version"

# Step 2: Show summary
Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  ✅ Release Package Ready!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green

Write-Host "Files created in: $releaseDir`n" -ForegroundColor Cyan

$files = Get-ChildItem $releaseDir -File
foreach ($file in $files) {
    $size = [math]::Round($file.Length / 1MB, 2)
    Write-Host "  📦 $($file.Name) ($size MB)" -ForegroundColor White
}

# Step 3: Instructions
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "  Next Steps:" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Yellow

Write-Host "1. TEST the executables:" -ForegroundColor Cyan
Write-Host "   Start-Process `"$releaseDir\LaserPointer-$Version-x64.exe`"`n" -ForegroundColor White

Write-Host "2. CREATE GITHUB RELEASE:" -ForegroundColor Cyan
Write-Host "   - Go to: https://github.com/yourusername/windows-laser-pointer/releases" -ForegroundColor White
Write-Host "   - Click 'Create a new release'" -ForegroundColor White
Write-Host "   - Tag: v$Version" -ForegroundColor White
Write-Host "   - Title: Laser Pointer v$Version" -ForegroundColor White
Write-Host "   - Description: Copy from $releaseDir\RELEASE_NOTES.md" -ForegroundColor White
Write-Host "   - Upload files from: $releaseDir" -ForegroundColor White
Write-Host "   - Check 'Set as the latest release'" -ForegroundColor White
Write-Host "   - Click 'Publish release'`n" -ForegroundColor White

Write-Host "3. (Optional) CREATE GIT TAG:" -ForegroundColor Cyan
Write-Host "   git tag v$Version" -ForegroundColor White
Write-Host "   git push origin v$Version`n" -ForegroundColor White

Write-Host "========================================" -ForegroundColor Green
Write-Host "  Ready to publish!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green
