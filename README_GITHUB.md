# Laser Pointer - GitHub Distribution

Simple guide to distribute Laser Pointer via GitHub with installers.

## Quick Start

### Option 1: Portable Executable (Easiest - Recommended)

1. **Build the executable:**
   ```powershell
   .\Scripts\Build-Portable.ps1 -Platform x64 -Version "1.0.0"
   ```

2. **Create release package:**
   ```powershell
   .\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
   ```

3. **Upload to GitHub:**
   - Go to your GitHub repository
   - Click "Releases" → "Create a new release"
   - Upload files from `Releases\v1.0.0\` folder
   - Done! Users can download and run directly

### Option 2: With Installer (Professional)

1. **Install Inno Setup:**
   - Download from: https://jrsoftware.org/isdl.php
   - Install Inno Setup Compiler

2. **Build installer:**
   ```powershell
   .\Scripts\Build-Installer.ps1 -Version "1.0.0" -Platform x64
   ```

3. **Upload both portable and installer to GitHub Releases**

## What Users Get

### Portable Version
- Single `.exe` file
- No installation needed
- Just download and run
- Works immediately

### Installer Version
- Professional installer wizard
- Start menu shortcuts
- Desktop shortcut option
- Uninstaller included

## File Structure

```
Releases/
└── v1.0.0/
    ├── LaserPointer-1.0.0-x64.exe      (Portable - 64-bit)
    ├── LaserPointer-1.0.0-x86.exe      (Portable - 32-bit)
    ├── LaserPointer-1.0.0-ARM64.exe    (Portable - ARM64)
    ├── LaserPointer-Setup-1.0.0.exe    (Installer - optional)
    └── RELEASE_NOTES.md                 (Release notes)
```

## GitHub Release Template

When creating a release, use this template:

```markdown
## Laser Pointer v1.0.0

### Download

**Portable (No Installation Required):**
- [LaserPointer-1.0.0-x64.exe](link) - 64-bit Windows (Recommended)
- [LaserPointer-1.0.0-x86.exe](link) - 32-bit Windows
- [LaserPointer-1.0.0-ARM64.exe](link) - Windows on ARM

**Installer (Optional):**
- [LaserPointer-Setup-1.0.0.exe](link) - Full installer with shortcuts

### Installation

1. Download the appropriate file for your system
2. **Portable**: Just run the `.exe` file - no installation needed!
3. **Installer**: Run the setup file and follow the wizard

### Usage

- Press `Ctrl+Shift+L` to activate/deactivate
- Right-click system tray icon for settings

### Requirements

- Windows 10 (1809+) or Windows 11
- No additional software required
```

## Complete Workflow

```powershell
# 1. Build release package
.\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"

# 2. Test the executables
Start-Process ".\Releases\v1.0.0\LaserPointer-1.0.0-x64.exe"

# 3. Commit and tag
git add .
git commit -m "Release v1.0.0"
git tag v1.0.0
git push origin main
git push origin v1.0.0

# 4. Create GitHub release (manual)
# Go to GitHub → Releases → Create new release
# Upload files from Releases\v1.0.0\
```

## Need Help?

See `GITHUB_DISTRIBUTION.md` for detailed instructions on:
- Creating installers (Inno Setup, NSIS, WiX)
- Automated GitHub Actions releases
- Advanced configuration
