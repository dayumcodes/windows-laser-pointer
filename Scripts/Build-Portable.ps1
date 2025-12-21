param(
    [string]$Platform = "x64",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

Write-Host "Building portable executable for $Platform..." -ForegroundColor Green

# Get project file path
$projectFile = "LaserPointer\LaserPointer.csproj"
if (-not (Test-Path $projectFile)) {
    Write-Host "❌ Project file not found: $projectFile" -ForegroundColor Red
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

# Clean and build
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean $projectFile -c Release -p:Platform=$Platform

Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore $projectFile

# Publish as self-contained single file (this will build and publish in one step)
$runtimeId = switch ($Platform) {
    "x64" { "win-x64" }
    "x86" { "win-x86" }
    "ARM64" { "win-arm64" }
}

Write-Host "Publishing as self-contained (folder deployment)..." -ForegroundColor Yellow
Write-Host "Note: Single-file doesn't work with Windows App SDK bootstrapper, using folder deployment instead" -ForegroundColor Yellow
dotnet publish $projectFile -c Release -p:Platform=$Platform `
    -p:SelfContained=true `
    -p:PublishSingleFile=false `
    -p:RuntimeIdentifier=$runtimeId `
    -p:PublishTrimmed=true `
    -p:WindowsAppSDKSelfContained=true `
    -p:PublishProfile= `
    --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}

# For folder deployment, the EXE is in the publish folder
$outputPath = "LaserPointer\bin\$Platform\Release\net8.0-windows10.0.19041.0\$runtimeId\publish\LaserPointer.exe"

if (Test-Path $outputPath) {
    $fileSize = [math]::Round((Get-Item $outputPath).Length / 1MB, 2)
    Write-Host "`n✅ Build complete!" -ForegroundColor Green
    Write-Host "   Location: $outputPath" -ForegroundColor Cyan
    Write-Host "   Size: $fileSize MB" -ForegroundColor Cyan
} else {
    Write-Host "❌ Output file not found!" -ForegroundColor Red
    exit 1
}
