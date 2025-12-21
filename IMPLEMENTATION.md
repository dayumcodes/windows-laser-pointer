# Production Implementation Guide - Laser Pointer

This is a detailed, step-by-step guide for implementing the Laser Pointer application in production. Follow these instructions in order to successfully publish your application.

## Table of Contents
1. [Pre-Implementation Setup](#pre-implementation-setup)
2. [Code Preparation](#code-preparation)
3. [Version Configuration](#version-configuration)
4. [Build Configuration](#build-configuration)
5. [Code Signing Setup](#code-signing-setup)
6. [MSIX Packaging Implementation](#msix-packaging-implementation)
7. [Testing Implementation](#testing-implementation)
8. [Microsoft Store Implementation](#microsoft-store-implementation)
9. [GitHub Releases Implementation](#github-releases-implementation)
10. [CI/CD Implementation](#cicd-implementation)
11. [Post-Deployment](#post-deployment)

---

## Pre-Implementation Setup

### Step 1: Verify Development Environment

#### Check Visual Studio Installation
```powershell
# Open PowerShell and run:
Get-Command devenv

# Verify .NET SDK version
dotnet --version
# Should show: 8.0.x or higher

# Verify Windows SDK
Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\Include" | Select-Object Name
# Should show: 10.0.19041.0 or higher
```

#### Install Missing Components
1. Open Visual Studio Installer
2. Click "Modify" on Visual Studio 2022
3. Ensure these workloads are installed:
   - ✅ .NET desktop development
   - ✅ Windows App SDK (WinUI 3) development
   - ✅ MSIX Packaging Tools (under Individual components)

### Step 2: Create Required Accounts

#### Microsoft Partner Center Account
1. Go to https://partner.microsoft.com/dashboard
2. Sign in with Microsoft account
3. Complete registration:
   - Pay $19 one-time fee (or use student/educator waiver)
   - Complete business verification
   - Wait for approval (1-2 business days)

#### Code Signing Certificate (Choose One)

**Option A: Commercial Certificate (Recommended)**
1. Purchase from:
   - DigiCert: https://www.digicert.com/code-signing/
   - Sectigo: https://sectigo.com/ssl-certificates-tls/code-signing
   - GlobalSign: https://www.globalsign.com/en/code-signing-certificate
2. Complete validation process (1-3 business days)
3. Download certificate as `.pfx` file
4. Store password securely

**Option B: Self-Signed (Testing Only)**
```powershell
# Create self-signed certificate (NOT for production)
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=LaserPointer" -KeyExportPolicy Exportable -CertStoreLocation Cert:\CurrentUser\My
$pwd = ConvertTo-SecureString -String "YourPassword" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath ".\LaserPointer.pfx" -Password $pwd
```

#### GitHub Account (If Using GitHub Releases)
1. Create account at https://github.com
2. Create new repository: `windows-laser-pointer`
3. Initialize repository:
```bash
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/yourusername/windows-laser-pointer.git
git push -u origin main
```

---

## Code Preparation

### Step 1: Clean Up Debug Code

#### Remove Debug Logging
1. Open `OverlayWindow.xaml.cs`
2. Search for `#region agent log` and `#endregion`
3. Remove all debug logging blocks (or comment them out)
4. Repeat for:
   - `LaserCanvas.xaml.cs`
   - `WindowHelper.cs`
   - `MainWindow.xaml.cs`

**Example:**
```csharp
// REMOVE OR COMMENT OUT:
// #region agent log
// try { File.AppendAllText(...) } catch { }
// #endregion
```

#### Remove Debug Log File
```powershell
# Delete debug log file
Remove-Item "c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log" -ErrorAction SilentlyContinue
```

### Step 2: Update Package.appxmanifest

#### Edit Publisher Identity
1. Open `LaserPointer\Package.appxmanifest`
2. Update the Identity section:

```xml
<Identity
  Name="YourPublisherName.LaserPointer"
  Publisher="CN=YourPublisherName"
  Version="1.0.0.0" />
```

**Important:**
- `Name`: Must be unique, format: `PublisherName.AppName`
- `Publisher`: Must match your code signing certificate
- `Version`: Start with `1.0.0.0`

#### Update Display Information
```xml
<Properties>
  <DisplayName>Laser Pointer</DisplayName>
  <PublisherDisplayName>Your Company Name</PublisherDisplayName>
  <Logo>Assets\StoreLogo.png</Logo>
</Properties>
```

#### Verify Capabilities
Ensure `runFullTrust` is present:
```xml
<Capabilities>
  <rescap:Capability Name="runFullTrust" />
</Capabilities>
```

### Step 3: Update Project File for Production

#### Edit LaserPointer.csproj
1. Open `LaserPointer\LaserPointer.csproj`
2. Add production build properties:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Existing properties -->
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>
    
    <!-- ADD THESE FOR PRODUCTION -->
    <Version>1.0.0.0</Version>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    <Company>Your Company Name</Company>
    <Product>Laser Pointer</Product>
    <Copyright>Copyright © 2024</Copyright>
    <Description>System-wide laser pointer overlay for Windows</Description>
    
    <!-- Release optimizations -->
    <PropertyGroup Condition="'$(Configuration)'=='Release'">
      <Optimize>true</Optimize>
      <DebugType>none</DebugType>
      <DebugSymbols>false</DebugSymbols>
      <PublishTrimmed>true</PublishTrimmed>
      <PublishReadyToRun>true</PublishReadyToRun>
      <PublishSingleFile>true</PublishSingleFile>
      <SelfContained>true</SelfContained>
    </PropertyGroup>
  </PropertyGroup>
  
  <!-- Rest of project file -->
</Project>
```

---

## Version Configuration

### Step 1: Set Initial Version

#### Update Package.appxmanifest
```xml
<Identity Version="1.0.0.0" />
```

#### Update Project File
```xml
<Version>1.0.0.0</Version>
<AssemblyVersion>1.0.0.0</AssemblyVersion>
<FileVersion>1.0.0.0</FileVersion>
```

#### Create Version.ps1 Script (Optional)
Create `Scripts\Version.ps1`:
```powershell
param(
    [string]$Version = "1.0.0.0"
)

$manifestPath = "LaserPointer\Package.appxmanifest"
$csprojPath = "LaserPointer\LaserPointer.csproj"

# Update manifest
$manifest = Get-Content $manifestPath -Raw
$manifest = $manifest -replace 'Version="[^"]*"', "Version=`"$Version`""
Set-Content -Path $manifestPath -Value $manifest

# Update csproj
$csproj = Get-Content $csprojPath -Raw
$csproj = $csproj -replace '<Version>[^<]*</Version>', "<Version>$Version</Version>"
$csproj = $csproj -replace '<AssemblyVersion>[^<]*</AssemblyVersion>', "<AssemblyVersion>$Version</AssemblyVersion>"
$csproj = $csproj -replace '<FileVersion>[^<]*</FileVersion>', "<FileVersion>$Version</FileVersion>"
Set-Content -Path $csprojPath -Value $csproj

Write-Host "Version updated to $Version"
```

**Usage:**
```powershell
.\Scripts\Version.ps1 -Version "1.0.1.0"
```

---

## Build Configuration

### Step 1: Create Build Scripts

#### Create Build-Release.ps1
Create `Scripts\Build-Release.ps1`:
```powershell
param(
    [string]$Platform = "x64",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "Building Laser Pointer for $Platform ($Configuration)..." -ForegroundColor Green

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean -c $Configuration -p:Platform=$Platform

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

# Build
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build -c $Configuration -p:Platform=$Platform --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Publish
Write-Host "Publishing application..." -ForegroundColor Yellow
$runtimeId = switch ($Platform) {
    "x64" { "win-x64" }
    "x86" { "win-x86" }
    "ARM64" { "win-arm64" }
}

dotnet publish -c $Configuration -p:Platform=$Platform `
    -p:SelfContained=true `
    -p:PublishSingleFile=true `
    -p:RuntimeIdentifier=$runtimeId `
    -p:PublishTrimmed=true `
    -p:PublishReadyToRun=true `
    --no-build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

$outputPath = "LaserPointer\bin\$Configuration\net8.0-windows10.0.19041.0\$runtimeId\publish"
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Output: $outputPath" -ForegroundColor Cyan
```

**Usage:**
```powershell
# Build x64
.\Scripts\Build-Release.ps1 -Platform x64

# Build all platforms
.\Scripts\Build-Release.ps1 -Platform x64
.\Scripts\Build-Release.ps1 -Platform x86
.\Scripts\Build-Release.ps1 -Platform ARM64
```

### Step 2: Test Build Locally

```powershell
# Navigate to project root
cd "c:\Users\mfuza\Downloads\laser pointer"

# Build x64 release
.\Scripts\Build-Release.ps1 -Platform x64

# Test the executable
$exePath = "LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\LaserPointer.exe"
Start-Process $exePath
```

---

## Code Signing Setup

### Step 1: Prepare Certificate

#### Store Certificate Securely
```powershell
# Create secure certificate storage
$certPath = ".\Certificates\LaserPointer.pfx"
$certPassword = Read-Host "Enter certificate password" -AsSecureString

# Verify certificate
$cert = Get-PfxData -FilePath $certPath -Password $certPassword
Write-Host "Certificate Subject: $($cert.EndEntityCertificates[0].Subject)"
Write-Host "Certificate Valid Until: $($cert.EndEntityCertificates[0].NotAfter)"
```

### Step 2: Create Signing Script

#### Create Sign-Executable.ps1
Create `Scripts\Sign-Executable.ps1`:
```powershell
param(
    [string]$FilePath,
    [string]$CertPath = ".\Certificates\LaserPointer.pfx",
    [string]$CertPassword,
    [string]$Description = "Laser Pointer",
    [string]$Website = "https://yourwebsite.com"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $FilePath)) {
    Write-Host "File not found: $FilePath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $CertPath)) {
    Write-Host "Certificate not found: $CertPath" -ForegroundColor Red
    exit 1
}

Write-Host "Signing $FilePath..." -ForegroundColor Yellow

# Get certificate password
if (-not $CertPassword) {
    $securePassword = Read-Host "Enter certificate password" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $CertPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# Sign using signtool
$signtool = "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe"

if (-not (Test-Path $signtool)) {
    # Try alternative path
    $signtool = Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "signtool.exe" | Select-Object -First 1 -ExpandProperty FullName
}

if (-not $signtool) {
    Write-Host "signtool.exe not found. Please install Windows SDK." -ForegroundColor Red
    exit 1
}

$timestampServer = "http://timestamp.digicert.com"

& $signtool sign /f $CertPath /p $CertPassword /t $timestampServer /d $Description /du $Website $FilePath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Signing failed!" -ForegroundColor Red
    exit 1
}

# Verify signature
Write-Host "Verifying signature..." -ForegroundColor Yellow
& $signtool verify /pa $FilePath

if ($LASTEXITCODE -eq 0) {
    Write-Host "Signature verified successfully!" -ForegroundColor Green
} else {
    Write-Host "Signature verification failed!" -ForegroundColor Red
    exit 1
}
```

**Usage:**
```powershell
$exePath = "LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\LaserPointer.exe"
.\Scripts\Sign-Executable.ps1 -FilePath $exePath
```

---

## MSIX Packaging Implementation

### Step 1: Update Project for MSIX

#### Modify LaserPointer.csproj
Change:
```xml
<WindowsPackageType>None</WindowsPackageType>
```
To:
```xml
<WindowsPackageType>MSIX</WindowsPackageType>
```

### Step 2: Configure Package.appxmanifest

#### Update Publisher (Critical)
The Publisher must match your code signing certificate:

```xml
<Identity
  Name="YourPublisherName.LaserPointer"
  Publisher="CN=YourPublisherName, O=YourOrganization, L=City, S=State, C=US"
  Version="1.0.0.0" />
```

**To get your certificate publisher:**
```powershell
$cert = Get-PfxData -FilePath ".\Certificates\LaserPointer.pfx" -Password $securePassword
$cert.EndEntityCertificates[0].Subject
```

### Step 3: Create MSIX Build Script

#### Create Build-MSIX.ps1
Create `Scripts\Build-MSIX.ps1`:
```powershell
param(
    [string]$Platform = "x64",
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0.0"
)

$ErrorActionPreference = "Stop"

Write-Host "Building MSIX package for $Platform..." -ForegroundColor Green

# Update version
.\Scripts\Version.ps1 -Version $Version

# Clean
dotnet clean -c $Configuration -p:Platform=$Platform

# Restore
dotnet restore

# Build
dotnet build -c $Configuration -p:Platform=$Platform --no-restore

# Create MSIX package
$msbuild = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if (-not (Test-Path $msbuild)) {
    $msbuild = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
}
if (-not (Test-Path $msbuild)) {
    $msbuild = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
}

& $msbuild "LaserPointer\LaserPointer.csproj" `
    /p:Configuration=$Configuration `
    /p:Platform=$Platform `
    /p:AppxBundle=Always `
    /p:AppxBundlePlatforms="$Platform" `
    /p:UapAppxPackageBuildMode=StoreUpload `
    /p:AppxPackageSigningEnabled=true

$outputPath = "LaserPointer\bin\$Configuration\$Platform\AppPackages"
Write-Host "MSIX package created at: $outputPath" -ForegroundColor Green
```

### Step 4: Test MSIX Package

```powershell
# Build MSIX
.\Scripts\Build-MSIX.ps1 -Platform x64 -Version "1.0.0.0"

# Install for testing
$msixPath = Get-ChildItem "LaserPointer\bin\Release\x64\AppPackages" -Recurse -Filter "*.msix" | Select-Object -First 1
Add-AppxPackage -Path $msixPath.FullName

# Test the app
Start-Process "shell:AppsFolder\YourPublisherName.LaserPointer_*"

# Uninstall when done
Get-AppxPackage "*LaserPointer*" | Remove-AppxPackage
```

---

## Testing Implementation

### Step 1: Create Test Script

#### Create Test-Application.ps1
Create `Scripts\Test-Application.ps1`:
```powershell
param(
    [string]$ExePath
)

$ErrorActionPreference = "Stop"

Write-Host "Testing Laser Pointer Application..." -ForegroundColor Green

# Test 1: File exists
if (-not (Test-Path $ExePath)) {
    Write-Host "❌ Executable not found: $ExePath" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Executable exists" -ForegroundColor Green

# Test 2: File is signed
$signtool = Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "signtool.exe" | Select-Object -First 1
if ($signtool) {
    $verify = & $signtool.Verify /pa $ExePath 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Executable is signed" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Executable is not signed" -ForegroundColor Yellow
    }
}

# Test 3: File size (should be reasonable)
$fileSize = (Get-Item $ExePath).Length / 1MB
Write-Host "✅ File size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Green

# Test 4: Start application
Write-Host "Starting application for manual testing..." -ForegroundColor Yellow
$process = Start-Process -FilePath $ExePath -PassThru
Start-Sleep -Seconds 5

if ($process.HasExited) {
    Write-Host "❌ Application exited immediately (exit code: $($process.ExitCode))" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Application is running" -ForegroundColor Green

# Stop application
Stop-Process -Id $process.Id -Force
Write-Host "✅ Application stopped successfully" -ForegroundColor Green

Write-Host "`nAll automated tests passed!" -ForegroundColor Green
Write-Host "Please perform manual testing:" -ForegroundColor Yellow
Write-Host "  1. Test hotkey activation (Ctrl+Shift+L)" -ForegroundColor Cyan
Write-Host "  2. Test drawing functionality" -ForegroundColor Cyan
Write-Host "  3. Test settings persistence" -ForegroundColor Cyan
Write-Host "  4. Test multi-monitor support" -ForegroundColor Cyan
```

**Usage:**
```powershell
$exePath = "LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\LaserPointer.exe"
.\Scripts\Test-Application.ps1 -ExePath $exePath
```

### Step 2: Run Windows App Certification Kit

```powershell
# Install WACK (if not installed)
# Download from: https://developer.microsoft.com/windows/downloads/app-certification-kit/

# Run WACK
$wackPath = "${env:ProgramFiles(x86)}\Windows Kits\10\App Certification Kit\appcert.exe"
$msixPath = "LaserPointer\bin\Release\x64\AppPackages\LaserPointer_1.0.0.0_x64.msix"

& $wackPath test -apptype windowsstore -packagepath $msixPath -reportoutputpath ".\WACK-Report.xml"
```

---

## Microsoft Store Implementation

### Step 1: Prepare Store Assets

#### Create Required Images
1. **Store Logo** (300x300px PNG):
   - Use existing `Assets\StoreLogo.png` or create new
   - Must be square, high quality

2. **Screenshots** (1366x768px minimum):
   - Take 3-5 screenshots showing:
     - Settings window
     - Laser pointer in action
     - Multi-monitor support
   - Save as PNG format

3. **Promotional Images** (414x468px, optional):
   - Create promotional banner
   - Highlight key features

#### Create Privacy Policy
Create `PRIVACY.md`:
```markdown
# Privacy Policy for Laser Pointer

**Last Updated:** [Date]

## Introduction
Laser Pointer ("we", "our", "us") is committed to protecting your privacy. This Privacy Policy explains how we handle information when you use our application.

## Data Collection
Laser Pointer does **NOT** collect, store, or transmit any personal data, usage statistics, or any information about you or your device.

## Local Storage
The application stores settings locally on your device only, including:
- Color preferences
- Fade duration settings
- Line thickness settings
- Window background color preference

This data is stored in your local application data folder and is never transmitted outside your device.

## Network Access
Laser Pointer does **NOT** access the internet or any network resources. The application operates entirely offline.

## Permissions
Laser Pointer requires the following permissions:
- **Global mouse tracking**: Required to enable the laser pointer drawing functionality
- **Global hotkey registration**: Required to activate/deactivate the overlay with keyboard shortcuts

These permissions are used solely for the application's core functionality and are not used to collect or transmit any data.

## Third-Party Services
Laser Pointer does not use any third-party analytics, advertising, or tracking services.

## Children's Privacy
Our application is suitable for users of all ages and does not collect any information from anyone, including children.

## Changes to This Policy
We may update this Privacy Policy from time to time. We will notify you of any changes by posting the new Privacy Policy on this page and updating the "Last Updated" date.

## Contact Us
If you have any questions about this Privacy Policy, please contact us at: [your-email@example.com]
```

**Publish Privacy Policy:**
1. Host on GitHub Pages, or
2. Host on your website, or
3. Use GitHub raw content URL: `https://raw.githubusercontent.com/yourusername/windows-laser-pointer/main/PRIVACY.md`

### Step 2: Create Store Listing

#### Step-by-Step Store Submission

1. **Log into Partner Center**
   - Go to https://partner.microsoft.com/dashboard
   - Sign in with your Microsoft account

2. **Create New App**
   - Click "Create new app"
   - Enter app name: "Laser Pointer"
   - Select "Productivity" category
   - Click "Create"

3. **App Identity**
   - **Reserved name**: Laser Pointer (or your chosen name)
   - **Package/Identity name**: Auto-generated (e.g., `YourPublisher.LaserPointer`)
   - **Publisher display name**: Your company/name

4. **Pricing and Availability**
   - Set price: Free (or your price)
   - Markets: Select "Worldwide" or specific markets
   - Visibility: "Make this app available to the public"

5. **Properties**
   - **Category**: Productivity
   - **Subcategory**: Utilities
   - **Age rating**: Complete questionnaire (likely E for Everyone)

6. **Store Listing**
   - **Description**: Copy from README.md, max 10,000 characters
   - **Keywords**: "laser pointer, presentation, drawing, overlay, screen annotation"
   - **Copyright**: "Copyright © 2024 [Your Name]"
   - **Support contact**: Your email
   - **Privacy policy URL**: Link to your privacy policy

7. **Upload Assets**
   - **Store logo**: Upload 300x300px PNG
   - **Screenshots**: Upload 3-5 screenshots (1366x768px minimum)
   - **Promotional images**: Optional

8. **Packages**
   - Click "New submission"
   - Upload your `.msixbundle` file
   - Wait for package processing

9. **Submit for Certification**
   - Review all sections
   - Click "Submit to the Store"
   - Wait for certification (1-3 business days)

### Step 3: Monitor Submission

```powershell
# Check submission status (manual process)
# Log into Partner Center and check "Submissions" section
```

---

## GitHub Releases Implementation

### Step 1: Prepare Release Assets

#### Create Release-Package.ps1
Create `Scripts\Release-Package.ps1`:
```powershell
param(
    [string]$Version = "1.0.0.0",
    [string]$ReleaseNotes = "Initial release"
)

$ErrorActionPreference = "Stop"

Write-Host "Preparing release package for version $Version..." -ForegroundColor Green

# Create release directory
$releaseDir = ".\Releases\$Version"
New-Item -ItemType Directory -Force -Path $releaseDir | Out-Null

# Build all platforms
$platforms = @("x64", "x86", "ARM64")
foreach ($platform in $platforms) {
    Write-Host "Building $platform..." -ForegroundColor Yellow
    .\Scripts\Build-Release.ps1 -Platform $platform
    
    # Sign executable
    $runtimeId = switch ($platform) {
        "x64" { "win-x64" }
        "x86" { "win-x86" }
        "ARM64" { "win-arm64" }
    }
    
    $exePath = "LaserPointer\bin\Release\net8.0-windows10.0.19041.0\$runtimeId\publish\LaserPointer.exe"
    
    if (Test-Path ".\Certificates\LaserPointer.pfx") {
        Write-Host "Signing $platform executable..." -ForegroundColor Yellow
        .\Scripts\Sign-Executable.ps1 -FilePath $exePath
    }
    
    # Copy to release directory
    $destPath = Join-Path $releaseDir "LaserPointer-$Version-$platform.exe"
    Copy-Item $exePath $destPath
    Write-Host "✅ $platform build complete" -ForegroundColor Green
}

# Create release notes file
$releaseNotesPath = Join-Path $releaseDir "RELEASE_NOTES.md"
@"
# Laser Pointer $Version

## Release Notes
$ReleaseNotes

## Installation
1. Download the appropriate executable for your system:
   - **x64**: LaserPointer-$Version-x64.exe (Recommended)
   - **x86**: LaserPointer-$Version-x86.exe (32-bit systems)
   - **ARM64**: LaserPointer-$Version-ARM64.exe (Windows on ARM)

2. Run the executable
3. Press Ctrl+Shift+L to activate the laser pointer

## System Requirements
- Windows 10 version 1809 or later
- Windows 11
- .NET 8.0 Runtime (included in self-contained build)

## Changes
- Initial release

## Support
For issues or feature requests, please visit: https://github.com/yourusername/windows-laser-pointer/issues
"@ | Out-File -FilePath $releaseNotesPath -Encoding UTF8

Write-Host "`nRelease package prepared at: $releaseDir" -ForegroundColor Green
Write-Host "Files:" -ForegroundColor Cyan
Get-ChildItem $releaseDir | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor Cyan }
```

### Step 2: Create GitHub Release

#### Manual Process
1. Go to your GitHub repository
2. Click "Releases" → "Create a new release"
3. **Tag version**: `v1.0.0`
4. **Release title**: `Laser Pointer v1.0.0`
5. **Description**: Copy from `RELEASE_NOTES.md`
6. **Upload files**: Upload all `.exe` files from `Releases\1.0.0.0\`
7. Check "Set as the latest release"
8. Click "Publish release"

#### Automated Process (GitHub Actions)
See [CI/CD Implementation](#cicd-implementation) section below.

---

## CI/CD Implementation

### Step 1: Create GitHub Actions Workflow

#### Create .github/workflows/release.yml
Create `.github\workflows\release.yml`:
```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'
  workflow_dispatch:
    inputs:
      version:
        description: 'Version number (e.g., 1.0.0)'
        required: true

jobs:
  build:
    runs-on: windows-latest
    strategy:
      matrix:
        platform: [x64, x86, ARM64]
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build -c Release -p:Platform=${{ matrix.platform }} --no-restore
      
      - name: Publish
        run: |
          $runtimeId = switch ("${{ matrix.platform }}") {
            "x64" { "win-x64" }
            "x86" { "win-x86" }
            "ARM64" { "win-arm64" }
          }
          dotnet publish -c Release -p:Platform=${{ matrix.platform }} `
            -p:SelfContained=true `
            -p:PublishSingleFile=true `
            -p:RuntimeIdentifier=$runtimeId `
            -p:PublishTrimmed=true `
            -p:PublishReadyToRun=true `
            --no-build
      
      - name: Sign executable (if certificate available)
        if: ${{ secrets.CODE_SIGNING_CERT != '' }}
        run: |
          # Decode certificate
          $certBytes = [Convert]::FromBase64String("${{ secrets.CODE_SIGNING_CERT }}")
          [System.IO.File]::WriteAllBytes("cert.pfx", $certBytes)
          
          # Sign
          $runtimeId = switch ("${{ matrix.platform }}") {
            "x64" { "win-x64" }
            "x86" { "win-x86" }
            "ARM64" { "win-arm64" }
          }
          $exePath = "LaserPointer\bin\Release\net8.0-windows10.0.19041.0\$runtimeId\publish\LaserPointer.exe"
          
          $signtool = Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "signtool.exe" | Select-Object -First 1
          & $signtool sign /f cert.pfx /p "${{ secrets.CODE_SIGNING_PASSWORD }}" /t http://timestamp.digicert.com /d "Laser Pointer" $exePath
        shell: powershell
      
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: LaserPointer-${{ matrix.platform }}
          path: LaserPointer/bin/Release/net8.0-windows10.0.19041.0/win-${{ matrix.platform == 'x64' && 'x64' || matrix.platform == 'x86' && 'x86' || 'arm64' }}/publish/LaserPointer.exe
          retention-days: 30
  
  release:
    needs: build
    runs-on: ubuntu-latest
    if: startsWith(github.ref, 'refs/tags/v')
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Download all artifacts
        uses: actions/download-artifact@v4
        with:
          path: artifacts
      
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          tag_name: ${{ github.ref }}
          name: Laser Pointer ${{ github.ref_name }}
          body: |
            ## Laser Pointer ${{ github.ref_name }}
            
            ### Downloads
            - [LaserPointer-x64.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-x64.exe)
            - [LaserPointer-x86.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-x86.exe)
            - [LaserPointer-ARM64.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-ARM64.exe)
            
            See [CHANGELOG.md](CHANGELOG.md) for details.
          files: |
            artifacts/LaserPointer-x64/LaserPointer.exe
            artifacts/LaserPointer-x86/LaserPointer.exe
            artifacts/LaserPointer-ARM64/LaserPointer.exe
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

### Step 2: Configure GitHub Secrets

1. Go to your GitHub repository
2. Settings → Secrets and variables → Actions
3. Add secrets:
   - `CODE_SIGNING_CERT`: Base64-encoded `.pfx` file
   - `CODE_SIGNING_PASSWORD`: Certificate password

**To encode certificate:**
```powershell
$certBytes = [System.IO.File]::ReadAllBytes(".\Certificates\LaserPointer.pfx")
$base64 = [Convert]::ToBase64String($certBytes)
$base64 | Out-File "cert-base64.txt"
# Copy contents to GitHub secret
```

### Step 3: Trigger Release

```bash
# Create and push tag
git tag v1.0.0
git push origin v1.0.0

# GitHub Actions will automatically build and create release
```

---

## Post-Deployment

### Step 1: Monitor Application

#### Set Up Monitoring
1. **Microsoft Store**:
   - Monitor crash reports in Partner Center
   - Respond to user reviews
   - Check analytics for usage patterns

2. **GitHub Releases**:
   - Monitor Issues for bug reports
   - Track download statistics
   - Review user feedback

### Step 2: Create Update Process

#### Version Update Workflow
1. **Update Version**:
   ```powershell
   .\Scripts\Version.ps1 -Version "1.0.1.0"
   ```

2. **Build and Test**:
   ```powershell
   .\Scripts\Build-Release.ps1 -Platform x64
   .\Scripts\Test-Application.ps1 -ExePath "LaserPointer\bin\Release\..."
   ```

3. **Create Release**:
   ```powershell
   .\Scripts\Release-Package.ps1 -Version "1.0.1.0" -ReleaseNotes "Bug fixes and improvements"
   ```

4. **Publish**:
   - Microsoft Store: Upload new package in Partner Center
   - GitHub: Create new release with tag `v1.0.1`

### Step 3: Maintenance Checklist

#### Weekly
- [ ] Check for crash reports
- [ ] Review user feedback
- [ ] Monitor GitHub issues
- [ ] Check certificate expiration (if applicable)

#### Monthly
- [ ] Review analytics
- [ ] Plan feature updates
- [ ] Update dependencies
- [ ] Security audit

#### Quarterly
- [ ] Major version planning
- [ ] Performance optimization review
- [ ] Documentation updates

---

## Troubleshooting

### Common Issues

#### Issue: Build Fails with Platform Error
**Solution:**
```powershell
# Ensure platform is specified correctly
dotnet build -c Release -p:Platform=x64
# NOT: dotnet build -c Release --platform x64
```

#### Issue: MSIX Package Fails to Install
**Solution:**
- Check Publisher name matches certificate
- Verify `runFullTrust` capability is present
- Ensure version number is incremented

#### Issue: Code Signing Fails
**Solution:**
```powershell
# Verify certificate is valid
$cert = Get-PfxData -FilePath ".\Certificates\LaserPointer.pfx" -Password $securePassword
$cert.EndEntityCertificates[0].NotAfter  # Check expiration date

# Verify signtool path
Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "signtool.exe"
```

#### Issue: Application Doesn't Start
**Solution:**
- Check Windows Event Viewer for errors
- Verify .NET 8.0 runtime is installed (if not self-contained)
- Check Windows Defender isn't blocking

---

## Quick Reference

### Build Commands
```powershell
# Debug build
dotnet build -c Debug -p:Platform=x64

# Release build
.\Scripts\Build-Release.ps1 -Platform x64

# Sign executable
.\Scripts\Sign-Executable.ps1 -FilePath "path\to\LaserPointer.exe"

# Create MSIX
.\Scripts\Build-MSIX.ps1 -Platform x64 -Version "1.0.0.0"

# Test application
.\Scripts\Test-Application.ps1 -ExePath "path\to\LaserPointer.exe"
```

### Version Management
```powershell
# Update version
.\Scripts\Version.ps1 -Version "1.0.1.0"

# Create release package
.\Scripts\Release-Package.ps1 -Version "1.0.1.0" -ReleaseNotes "Bug fixes"
```

### GitHub Release
```bash
# Create tag and trigger CI/CD
git tag v1.0.0
git push origin v1.0.0
```

---

**Last Updated**: [Current Date]
**Next Review**: [Date + 3 months]
