param(
    [string]$Version = "1.0.0",
    [string]$ReleaseNotes = "Initial release"
)

$ErrorActionPreference = "Stop"

Write-Host "Creating release package for v$Version..." -ForegroundColor Green

# Create release directory
$releaseDir = ".\Releases\v$Version"
if (Test-Path $releaseDir) {
    Write-Host "Removing existing release directory..." -ForegroundColor Yellow
    Remove-Item $releaseDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $releaseDir | Out-Null

# Build all platforms
$platforms = @("x64", "x86", "ARM64")
foreach ($platform in $platforms) {
    Write-Host "`n=== Building $platform ===" -ForegroundColor Cyan
    & ".\Scripts\Build-Portable.ps1" -Platform $platform -Version $Version
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to build $platform" -ForegroundColor Red
        continue
    }
    
    $runtimeId = switch ($platform) {
        "x64" { "win-x64" }
        "x86" { "win-x86" }
        "ARM64" { "win-arm64" }
    }
    
    $exePath = "LaserPointer\bin\$platform\Release\net8.0-windows10.0.19041.0\$runtimeId\publish\LaserPointer.exe"
    $destPath = Join-Path $releaseDir "LaserPointer-$Version-$platform.exe"
    
    if (Test-Path $exePath) {
        # Copy the entire publish folder to a zip or folder for distribution
        $publishFolder = "LaserPointer\bin\$platform\Release\net8.0-windows10.0.19041.0\$runtimeId\publish"
        $destFolder = Join-Path $releaseDir "LaserPointer-$Version-$platform"
        
        if (Test-Path $publishFolder) {
            # Copy entire folder (needed for Windows App SDK DLLs)
            if (Test-Path $destFolder) {
                Remove-Item $destFolder -Recurse -Force
            }
            Copy-Item $publishFolder $destFolder -Recurse
            
            # Also create a zip file for easier distribution
            $zipPath = "$destFolder.zip"
            if (Test-Path $zipPath) {
                Remove-Item $zipPath -Force
            }
            Compress-Archive -Path $destFolder -DestinationPath $zipPath -Force
            
            $size = [math]::Round((Get-ChildItem $destFolder -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB, 2)
            $zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
            Write-Host "✅ $platform ready (Folder: $size MB, ZIP: $zipSize MB)" -ForegroundColor Green
        } else {
            Write-Host "⚠️  $platform publish folder not found" -ForegroundColor Yellow
        }
    } else {
        Write-Host "⚠️  $platform executable not found" -ForegroundColor Yellow
    }
}

# Create release notes
$notesPath = Join-Path $releaseDir "RELEASE_NOTES.md"
$notesContent = @"
# Laser Pointer v$Version

$ReleaseNotes

## Downloads

### Portable Packages (No Installation Required)
- **[LaserPointer-$Version-x64.zip](LaserPointer-$Version-x64.zip)** - 64-bit Windows (Recommended)
- **[LaserPointer-$Version-x86.zip](LaserPointer-$Version-x86.zip)** - 32-bit Windows
- **[LaserPointer-$Version-ARM64.zip](LaserPointer-$Version-ARM64.zip)** - Windows on ARM

Each ZIP contains the executable and all required DLLs. Extract and run `LaserPointer.exe`.

## Installation

### Portable Version (Easiest)
1. Download the appropriate `.zip` file for your system
2. Extract the ZIP to any folder (e.g., `C:\Program Files\LaserPointer\` or your Desktop)
3. Double-click `LaserPointer.exe` to run - no installation needed!

## System Requirements
- Windows 10 (version 1809 or later)
- Windows 11
- No additional software required (all dependencies included)

## Usage
- Press `Ctrl+Shift+L` to activate/deactivate the laser pointer
- Right-click system tray icon for settings
- Draw by moving your mouse while the overlay is active

## What's New
$ReleaseNotes

## Support
- Report issues: https://github.com/yourusername/windows-laser-pointer/issues
- View source: https://github.com/yourusername/windows-laser-pointer
"@

$notesContent | Out-File -FilePath $notesPath -Encoding UTF8

Write-Host "`n✅ Release package created!" -ForegroundColor Green
Write-Host "`nLocation: $releaseDir" -ForegroundColor Cyan
Write-Host "`nFiles ready for GitHub release:" -ForegroundColor Yellow
Get-ChildItem $releaseDir | ForEach-Object { 
    $size = [math]::Round($_.Length / 1MB, 2)
    Write-Host "  - $($_.Name) ($size MB)" -ForegroundColor Cyan
}

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  1. Test the executables in $releaseDir" -ForegroundColor White
Write-Host "  2. Go to GitHub → Releases → Create a new release" -ForegroundColor White
Write-Host "  3. Upload all files from $releaseDir" -ForegroundColor White
Write-Host "  4. Copy release notes from RELEASE_NOTES.md" -ForegroundColor White
