# GitHub Distribution Guide - Laser Pointer

This guide focuses on distributing Laser Pointer via GitHub with installers. No Microsoft Store submission required.

## Table of Contents
1. [Installer Options](#installer-options)
2. [Quick Start - Simple Setup](#quick-start---simple-setup)
3. [Option 1: Portable Executable (Easiest)](#option-1-portable-executable-easiest)
4. [Option 2: Inno Setup Installer (Recommended)](#option-2-inno-setup-installer-recommended)
5. [Option 3: NSIS Installer](#option-3-nsis-installer)
6. [Option 4: WiX Toolset (MSI)](#option-4-wix-toolset-msi)
7. [GitHub Releases Setup](#github-releases-setup)
8. [Automated Release Process](#automated-release-process)

---

## Installer Options

### Comparison

| Option | Difficulty | Features | Best For |
|--------|-----------|----------|----------|
| **Portable EXE** | ⭐ Easy | No installer needed | Quick distribution |
| **Inno Setup** | ⭐⭐ Medium | Professional installer, auto-updates | Recommended |
| **NSIS** | ⭐⭐ Medium | Free, customizable | Open source projects |
| **WiX (MSI)** | ⭐⭐⭐ Hard | Enterprise standard | Corporate environments |

**Recommendation**: Start with **Portable EXE** for simplicity, then move to **Inno Setup** for a professional installer.

---

## Quick Start - Simple Setup

### Step 1: Build Portable Executable

Create `Scripts\Build-Portable.ps1`:
```powershell
param(
    [string]$Platform = "x64",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

Write-Host "Building portable executable for $Platform..." -ForegroundColor Green

# Clean and build
dotnet clean -c Release -p:Platform=$Platform
dotnet restore
dotnet build -c Release -p:Platform=$Platform --no-restore

# Publish as self-contained single file
$runtimeId = switch ($Platform) {
    "x64" { "win-x64" }
    "x86" { "win-x86" }
    "ARM64" { "win-arm64" }
}

dotnet publish -c Release -p:Platform=$Platform `
    -p:SelfContained=true `
    -p:PublishSingleFile=true `
    -p:RuntimeIdentifier=$runtimeId `
    -p:PublishTrimmed=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    --no-build

$outputPath = "LaserPointer\bin\Release\net8.0-windows10.0.19041.0\$runtimeId\publish\LaserPointer.exe"

Write-Host "✅ Build complete: $outputPath" -ForegroundColor Green
Write-Host "`nFile size: $([math]::Round((Get-Item $outputPath).Length / 1MB, 2)) MB" -ForegroundColor Cyan
```

**Usage:**
```powershell
.\Scripts\Build-Portable.ps1 -Platform x64 -Version "1.0.0"
```

### Step 2: Create GitHub Release

1. Build the executable:
   ```powershell
   .\Scripts\Build-Portable.ps1 -Platform x64
   ```

2. Go to GitHub → Releases → Create a new release
3. Upload `LaserPointer.exe` from the publish folder
4. Users download and run directly - no installation needed!

---

## Option 1: Portable Executable (Easiest)

### Advantages
- ✅ No installer needed
- ✅ Users just download and run
- ✅ Works immediately
- ✅ No admin rights required

### Implementation

#### Create Build Script
The script above (`Build-Portable.ps1`) creates a single executable that includes everything.

#### Create README for Users
Create `INSTALL.md`:
```markdown
# Installation Instructions

## Portable Version (Recommended)

1. Download `LaserPointer-x64.exe` from the [Releases](https://github.com/yourusername/windows-laser-pointer/releases) page
2. Save the file to any folder (e.g., `C:\Program Files\LaserPointer\` or `C:\Users\YourName\Desktop\`)
3. Double-click `LaserPointer-x64.exe` to run
4. No installation required!

## First Run

- The application will start in the system tray
- Press `Ctrl+Shift+L` to activate the laser pointer
- Right-click the system tray icon to open settings

## Uninstallation

Simply delete the `LaserPointer-x64.exe` file. No uninstaller needed.

## System Requirements

- Windows 10 version 1809 or later
- Windows 11
- No additional software required (all dependencies included)
```

#### GitHub Release Template
When creating a release, use this template:

```markdown
## Laser Pointer v1.0.0

### Download

- **[LaserPointer-x64.exe](https://github.com/yourusername/windows-laser-pointer/releases/download/v1.0.0/LaserPointer-x64.exe)** (Recommended - 64-bit Windows)
- **[LaserPointer-x86.exe](https://github.com/yourusername/windows-laser-pointer/releases/download/v1.0.0/LaserPointer-x86.exe)** (32-bit Windows)
- **[LaserPointer-ARM64.exe](https://github.com/yourusername/windows-laser-pointer/releases/download/v1.0.0/LaserPointer-ARM64.exe)** (Windows on ARM)

### Installation

1. Download the appropriate file for your system
2. Run the executable - no installation needed!
3. Press `Ctrl+Shift+L` to activate

### What's New

- Initial release
- System-wide laser pointer overlay
- Customizable colors and settings
- Multi-monitor support

### System Requirements

- Windows 10 (1809+) or Windows 11
- No additional software required
```

---

## Option 2: Inno Setup Installer (Recommended)

### Advantages
- ✅ Professional installer with wizard
- ✅ Start menu shortcuts
- ✅ Uninstaller included
- ✅ Optional auto-updates
- ✅ Free and open source

### Step 1: Install Inno Setup

1. Download from: https://jrsoftware.org/isdl.php
2. Install Inno Setup Compiler
3. (Optional) Install Inno Script Studio for GUI editor

### Step 2: Create Installer Script

Create `Installer\LaserPointer.iss`:
```iss
#define MyAppName "Laser Pointer"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Your Name"
#define MyAppURL "https://github.com/yourusername/windows-laser-pointer"
#define MyAppExeName "LaserPointer.exe"

[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=LICENSE
OutputDir=.\Output
OutputBaseFilename=LaserPointer-Setup-{#MyAppVersion}
SetupIconFile=..\LaserPointer\Assets\Square44x44Logo.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64 x86 arm64
ArchitecturesInstall64=x64 arm64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1

[Files]
Source: "..\LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: IsX64
Source: "..\LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-x86\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: IsX86
Source: "..\LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-arm64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: IsARM64

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function IsX64: Boolean;
begin
  Result := (ProcessorArchitecture = paX64);
end;

function IsX86: Boolean;
begin
  Result := (ProcessorArchitecture = paX86);
end;

function IsARM64: Boolean;
begin
  Result := (ProcessorArchitecture = paARM64);
end;
```

### Step 3: Build Installer Script

Create `Scripts\Build-Installer.ps1`:
```powershell
param(
    [string]$Version = "1.0.0",
    [string]$Platform = "x64"
)

$ErrorActionPreference = "Stop"

Write-Host "Building installer for version $Version..." -ForegroundColor Green

# Build the application first
Write-Host "Building application..." -ForegroundColor Yellow
.\Scripts\Build-Portable.ps1 -Platform $Platform -Version $Version

# Update version in Inno Setup script
$issPath = ".\Installer\LaserPointer.iss"
$issContent = Get-Content $issPath -Raw
$issContent = $issContent -replace 'MyAppVersion "[\d.]+"', "MyAppVersion `"$Version`""
Set-Content -Path $issPath -Value $issContent

# Find Inno Setup compiler
$innoCompiler = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
if (-not (Test-Path $innoCompiler)) {
    $innoCompiler = "${env:ProgramFiles}\Inno Setup 6\ISCC.exe"
}

if (-not (Test-Path $innoCompiler)) {
    Write-Host "❌ Inno Setup Compiler not found!" -ForegroundColor Red
    Write-Host "Please install Inno Setup from: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    exit 1
}

# Compile installer
Write-Host "Compiling installer..." -ForegroundColor Yellow
& $innoCompiler $issPath

if ($LASTEXITCODE -eq 0) {
    $installerPath = ".\Installer\Output\LaserPointer-Setup-$Version.exe"
    if (Test-Path $installerPath) {
        Write-Host "✅ Installer created: $installerPath" -ForegroundColor Green
        Write-Host "File size: $([math]::Round((Get-Item $installerPath).Length / 1MB, 2)) MB" -ForegroundColor Cyan
    } else {
        Write-Host "⚠️  Installer compilation completed but file not found" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ Installer compilation failed!" -ForegroundColor Red
    exit 1
}
```

**Usage:**
```powershell
.\Scripts\Build-Installer.ps1 -Version "1.0.0" -Platform x64
```

### Step 4: Test Installer

1. Run the installer: `Installer\Output\LaserPointer-Setup-1.0.0.exe`
2. Install to default location
3. Verify it works
4. Test uninstaller from Control Panel

---

## Option 3: NSIS Installer

### Advantages
- ✅ Free and open source
- ✅ Highly customizable
- ✅ Small installer size
- ✅ Good for open source projects

### Step 1: Install NSIS

1. Download from: https://nsis.sourceforge.io/Download
2. Install NSIS

### Step 2: Create NSIS Script

Create `Installer\LaserPointer.nsi`:
```nsis
!define APP_NAME "Laser Pointer"
!define APP_VERSION "1.0.0"
!define APP_PUBLISHER "Your Name"
!define APP_URL "https://github.com/yourusername/windows-laser-pointer"
!define APP_EXE "LaserPointer.exe"

Name "${APP_NAME}"
OutFile "LaserPointer-Setup-${APP_VERSION}.exe"
InstallDir "$PROGRAMFILES64\${APP_NAME}"
RequestExecutionLevel admin
Unicode True

!include "MUI2.nsh"

!define MUI_ICON "..\LaserPointer\Assets\Square44x44Logo.ico"
!define MUI_UNICON "..\LaserPointer\Assets\Square44x44Logo.ico"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

Section "MainSection" SEC01
    SetOutPath "$INSTDIR"
    File /r "..\LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*"
    
    CreateShortcut "$SMPROGRAMS\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
    CreateShortcut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
SectionEnd

Section -AdditionalIcons
    CreateShortcut "$SMPROGRAMS\${APP_NAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
SectionEnd

Section Uninstall
    Delete "$INSTDIR\uninstall.exe"
    Delete "$SMPROGRAMS\${APP_NAME}\Uninstall.lnk"
    RMDir /r "$INSTDIR"
    RMDir "$SMPROGRAMS\${APP_NAME}"
    Delete "$DESKTOP\${APP_NAME}.lnk"
    Delete "$SMPROGRAMS\${APP_NAME}.lnk"
SectionEnd
```

### Step 3: Build NSIS Installer

Create `Scripts\Build-NSIS.ps1`:
```powershell
param(
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

# Build application first
.\Scripts\Build-Portable.ps1 -Platform x64 -Version $Version

# Find NSIS compiler
$nsisCompiler = "${env:ProgramFiles(x86)}\NSIS\makensis.exe"
if (-not (Test-Path $nsisCompiler)) {
    $nsisCompiler = "${env:ProgramFiles}\NSIS\makensis.exe"
}

if (-not (Test-Path $nsisCompiler)) {
    Write-Host "❌ NSIS not found! Install from: https://nsis.sourceforge.io/" -ForegroundColor Red
    exit 1
}

# Update version in script
$nsiPath = ".\Installer\LaserPointer.nsi"
$nsiContent = Get-Content $nsiPath -Raw
$nsiContent = $nsiContent -replace 'APP_VERSION "[\d.]+"', "APP_VERSION `"$Version`""
Set-Content -Path $nsiPath -Value $nsiContent

# Compile
& $nsisCompiler $nsiPath

Write-Host "✅ NSIS installer created" -ForegroundColor Green
```

---

## Option 4: WiX Toolset (MSI)

### Advantages
- ✅ Enterprise standard (MSI format)
- ✅ Group Policy compatible
- ✅ Professional for corporate use

### Step 1: Install WiX Toolset

1. Download from: https://wixtoolset.org/releases/
2. Install WiX Toolset

### Step 2: Create WiX Project

Create `Installer\LaserPointer.wxs`:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*" Name="Laser Pointer" Language="1033" Version="1.0.0" 
           Manufacturer="Your Name" UpgradeCode="A1B2C3D4-E5F6-7890-ABCD-EF1234567890">
    <Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine" />
    
    <MajorUpgrade DowngradeErrorMessage="A newer version is already installed." />
    <MediaTemplate />
    
    <Feature Id="ProductFeature" Title="Laser Pointer" Level="1">
      <ComponentGroupRef Id="ProductComponents" />
    </Feature>
  </Product>
  
  <Fragment>
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFilesFolder">
        <Directory Id="INSTALLFOLDER" Name="Laser Pointer" />
      </Directory>
    </Directory>
  </Fragment>
  
  <Fragment>
    <ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">
      <Component Id="ProductComponent">
        <File Source="..\LaserPointer\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\LaserPointer.exe" />
      </Component>
    </ComponentGroup>
  </Fragment>
</Wix>
```

### Step 3: Build MSI

```powershell
# Build application
.\Scripts\Build-Portable.ps1 -Platform x64

# Compile WiX
candle.exe Installer\LaserPointer.wxs
light.exe LaserPointer.wixobj -out LaserPointer-Setup-1.0.0.msi
```

---

## GitHub Releases Setup

### Step 1: Create Release Package Script

Create `Scripts\Create-Release.ps1`:
```powershell
param(
    [string]$Version = "1.0.0",
    [string]$ReleaseNotes = "Initial release"
)

$ErrorActionPreference = "Stop"

Write-Host "Creating release package for v$Version..." -ForegroundColor Green

# Create release directory
$releaseDir = ".\Releases\v$Version"
New-Item -ItemType Directory -Force -Path $releaseDir | Out-Null

# Build all platforms
$platforms = @("x64", "x86", "ARM64")
foreach ($platform in $platforms) {
    Write-Host "`nBuilding $platform..." -ForegroundColor Yellow
    .\Scripts\Build-Portable.ps1 -Platform $platform -Version $Version
    
    $runtimeId = switch ($platform) {
        "x64" { "win-x64" }
        "x86" { "win-x86" }
        "ARM64" { "win-arm64" }
    }
    
    $exePath = "LaserPointer\bin\Release\net8.0-windows10.0.19041.0\$runtimeId\publish\LaserPointer.exe"
    $destPath = Join-Path $releaseDir "LaserPointer-$Version-$platform.exe"
    Copy-Item $exePath $destPath
    Write-Host "✅ $platform ready" -ForegroundColor Green
}

# Build installer (if Inno Setup is installed)
if (Test-Path "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe") {
    Write-Host "`nBuilding installer..." -ForegroundColor Yellow
    .\Scripts\Build-Installer.ps1 -Version $Version -Platform x64
    $installerPath = ".\Installer\Output\LaserPointer-Setup-$Version.exe"
    if (Test-Path $installerPath) {
        Copy-Item $installerPath (Join-Path $releaseDir "LaserPointer-Setup-$Version.exe")
        Write-Host "✅ Installer ready" -ForegroundColor Green
    }
}

# Create release notes
$notesPath = Join-Path $releaseDir "RELEASE_NOTES.md"
@"
# Laser Pointer v$Version

$ReleaseNotes

## Downloads

### Portable Executables (No Installation Required)
- **[LaserPointer-$Version-x64.exe](LaserPointer-$Version-x64.exe)** - 64-bit Windows (Recommended)
- **[LaserPointer-$Version-x86.exe](LaserPointer-$Version-x86.exe)** - 32-bit Windows
- **[LaserPointer-$Version-ARM64.exe](LaserPointer-$Version-ARM64.exe)** - Windows on ARM

### Installer (Optional)
- **[LaserPointer-Setup-$Version.exe](LaserPointer-Setup-$Version.exe)** - Full installer with shortcuts

## Installation

### Portable Version (Easiest)
1. Download the appropriate `.exe` file for your system
2. Save to any folder
3. Double-click to run - no installation needed!

### Installer Version
1. Download `LaserPointer-Setup-$Version.exe`
2. Run the installer
3. Follow the installation wizard
4. Launch from Start Menu or Desktop shortcut

## System Requirements
- Windows 10 (version 1809 or later)
- Windows 11
- No additional software required

## Usage
- Press `Ctrl+Shift+L` to activate/deactivate the laser pointer
- Right-click system tray icon for settings
- Draw by moving your mouse while the overlay is active

## Support
- Report issues: https://github.com/yourusername/windows-laser-pointer/issues
- View source: https://github.com/yourusername/windows-laser-pointer
"@ | Out-File -FilePath $notesPath -Encoding UTF8

Write-Host "`n✅ Release package created at: $releaseDir" -ForegroundColor Green
Write-Host "`nFiles ready for GitHub release:" -ForegroundColor Cyan
Get-ChildItem $releaseDir | ForEach-Object { 
    $size = [math]::Round($_.Length / 1MB, 2)
    Write-Host "  - $($_.Name) ($size MB)" -ForegroundColor Cyan
}
```

### Step 2: Create GitHub Release

#### Manual Process

1. **Prepare Release:**
   ```powershell
   .\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
   ```

2. **Go to GitHub:**
   - Navigate to your repository
   - Click "Releases" → "Create a new release"

3. **Fill Release Form:**
   - **Tag**: `v1.0.0` (create new tag)
   - **Title**: `Laser Pointer v1.0.0`
   - **Description**: Copy from `Releases\v1.0.0\RELEASE_NOTES.md`

4. **Upload Files:**
   - Drag and drop all files from `Releases\v1.0.0\` folder
   - Or click "Attach binaries" and select files

5. **Publish:**
   - Check "Set as the latest release"
   - Click "Publish release"

#### Automated Process (GitHub Actions)

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
        description: 'Version (e.g., 1.0.0)'
        required: true

jobs:
  build:
    runs-on: windows-latest
    strategy:
      matrix:
        platform: [x64, x86, ARM64]
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Build
        run: |
          dotnet build -c Release -p:Platform=${{ matrix.platform }}
          $runtimeId = switch ("${{ matrix.platform }}") {
            "x64" { "win-x64" }
            "x86" { "win-x86" }
            "ARM64" { "win-arm64" }
          }
          dotnet publish -c Release -p:Platform=${{ matrix.platform }} `
            -p:SelfContained=true `
            -p:PublishSingleFile=true `
            -p:RuntimeIdentifier=$runtimeId `
            -p:PublishTrimmed=true
      
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: LaserPointer-${{ matrix.platform }}
          path: LaserPointer/bin/Release/net8.0-windows10.0.19041.0/win-${{ matrix.platform == 'x64' && 'x64' || matrix.platform == 'x86' && 'x86' || 'arm64' }}/publish/LaserPointer.exe
  
  release:
    needs: build
    runs-on: ubuntu-latest
    if: startsWith(github.ref, 'refs/tags/v')
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Download artifacts
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
            
            **Portable Executables (No Installation Required)**
            - [LaserPointer-x64.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-x64.exe) - 64-bit Windows (Recommended)
            - [LaserPointer-x86.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-x86.exe) - 32-bit Windows
            - [LaserPointer-ARM64.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-ARM64.exe) - Windows on ARM
            
            ### Installation
            
            1. Download the appropriate file for your system
            2. Run the executable - no installation needed!
            3. Press `Ctrl+Shift+L` to activate
            
            ### System Requirements
            - Windows 10 (1809+) or Windows 11
            - No additional software required
          files: |
            artifacts/LaserPointer-x64/LaserPointer.exe
            artifacts/LaserPointer-x86/LaserPointer.exe
            artifacts/LaserPointer-ARM64/LaserPointer.exe
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

**To trigger release:**
```bash
git tag v1.0.0
git push origin v1.0.0
```

---

## Automated Release Process

### Complete Workflow

1. **Update Version:**
   ```powershell
   # Update version in code
   # Edit Package.appxmanifest and LaserPointer.csproj
   ```

2. **Create Release Package:**
   ```powershell
   .\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
   ```

3. **Test Locally:**
   ```powershell
   # Test the executable
   Start-Process ".\Releases\v1.0.0\LaserPointer-1.0.0-x64.exe"
   ```

4. **Commit and Tag:**
   ```bash
   git add .
   git commit -m "Release v1.0.0"
   git tag v1.0.0
   git push origin main
   git push origin v1.0.0
   ```

5. **Create GitHub Release:**
   - Go to GitHub → Releases
   - Click "Draft a new release"
   - Select tag `v1.0.0`
   - Upload files from `Releases\v1.0.0\`
   - Publish

---

## Quick Reference

### Build Commands
```powershell
# Build portable executable
.\Scripts\Build-Portable.ps1 -Platform x64 -Version "1.0.0"

# Build installer (Inno Setup)
.\Scripts\Build-Installer.ps1 -Version "1.0.0" -Platform x64

# Create complete release package
.\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Bug fixes"
```

### GitHub Release Template
```markdown
## Laser Pointer v1.0.0

### Download

**Portable (No Installation):**
- [LaserPointer-1.0.0-x64.exe](link) - 64-bit (Recommended)
- [LaserPointer-1.0.0-x86.exe](link) - 32-bit
- [LaserPointer-1.0.0-ARM64.exe](link) - ARM64

**Installer:**
- [LaserPointer-Setup-1.0.0.exe](link) - Full installer

### Installation
1. Download the appropriate file
2. Run the executable
3. Press `Ctrl+Shift+L` to activate

### Requirements
- Windows 10 (1809+) or Windows 11
```

---

## Recommended Approach

**For Quick Start:**
1. Use **Portable EXE** (Option 1)
2. Build with `Build-Portable.ps1`
3. Upload to GitHub Releases
4. Done! Users download and run

**For Professional Distribution:**
1. Use **Inno Setup** (Option 2)
2. Build installer with `Build-Installer.ps1`
3. Provide both portable and installer options
4. Users choose their preference

---

**Last Updated**: [Current Date]
