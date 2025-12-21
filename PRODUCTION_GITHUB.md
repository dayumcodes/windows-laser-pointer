# Production Release to GitHub - Portable Executable

Simple, step-by-step guide to release Laser Pointer to GitHub as a portable executable.

## Quick Start (3 Steps)

### Step 1: Build Release Package

Open PowerShell in the project root and run:

```powershell
.\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
```

This will:
- ✅ Build executables for x64, x86, and ARM64
- ✅ Create release package in `Releases\v1.0.0\`
- ✅ Generate release notes

**Output:**
```
Releases/
└── v1.0.0/
    ├── LaserPointer-1.0.0-x64.exe
    ├── LaserPointer-1.0.0-x86.exe
    ├── LaserPointer-1.0.0-ARM64.exe
    └── RELEASE_NOTES.md
```

### Step 2: Test Locally

Test the executable before uploading:

```powershell
# Test the x64 version
Start-Process ".\Releases\v1.0.0\LaserPointer-1.0.0-x64.exe"

# Verify it works:
# - Press Ctrl+Shift+L to activate
# - Move mouse to draw
# - Press Ctrl+Shift+L again to deactivate
```

### Step 3: Upload to GitHub

1. **Go to your GitHub repository**
   - Navigate to: `https://github.com/yourusername/windows-laser-pointer`

2. **Create a new release:**
   - Click "Releases" (right sidebar)
   - Click "Create a new release"

3. **Fill in release details:**
   - **Tag version**: `v1.0.0` (create new tag)
   - **Release title**: `Laser Pointer v1.0.0`
   - **Description**: Copy from `Releases\v1.0.0\RELEASE_NOTES.md`

4. **Upload files:**
   - Drag and drop all 3 `.exe` files from `Releases\v1.0.0\` folder
   - Or click "Attach binaries" and select files

5. **Publish:**
   - Check "Set as the latest release"
   - Click "Publish release"

**Done!** Users can now download and use your application.

---

## Detailed Instructions

### Prerequisites

- ✅ Visual Studio 2022 with .NET 8.0 SDK
- ✅ Windows 10/11
- ✅ PowerShell (comes with Windows)
- ✅ GitHub account and repository

### Build Process Explained

#### What Happens When You Run Create-Release.ps1

1. **Cleans previous builds** - Removes old files
2. **Restores NuGet packages** - Downloads dependencies
3. **Builds for each platform:**
   - x64 (64-bit Windows - most common)
   - x86 (32-bit Windows - legacy)
   - ARM64 (Windows on ARM devices)
4. **Publishes as self-contained:**
   - Includes .NET runtime
   - Single executable file
   - No installation needed
5. **Packages for release:**
   - Copies executables to `Releases\v1.0.0\`
   - Generates release notes

#### Build Output Details

Each executable is:
- **Self-contained**: Includes all dependencies
- **Single file**: One `.exe` file, no DLLs needed
- **Portable**: Can run from any folder
- **Size**: Approximately 50-80 MB (includes .NET runtime)

### Version Management

#### Update Version for New Release

1. **Edit version in code:**
   - `LaserPointer\Package.appxmanifest`: Update `<Identity Version="1.0.1.0" />`
   - `LaserPointer\LaserPointer.csproj`: Update `<Version>1.0.1.0</Version>`

2. **Build new release:**
   ```powershell
   .\Scripts\Create-Release.ps1 -Version "1.0.1" -ReleaseNotes "Bug fixes and improvements"
   ```

3. **Create GitHub release with new version**

### Release Notes Template

When creating release notes, use this format:

```markdown
## Laser Pointer v1.0.0

### Download

**Portable Executables (No Installation Required):**
- [LaserPointer-1.0.0-x64.exe](link) - 64-bit Windows (Recommended)
- [LaserPointer-1.0.0-x86.exe](link) - 32-bit Windows  
- [LaserPointer-1.0.0-ARM64.exe](link) - Windows on ARM

### Installation

1. Download the appropriate file for your system (x64 recommended for most users)
2. Save to any folder (Desktop, Program Files, etc.)
3. Double-click to run - **no installation needed!**

### Usage

- Press `Ctrl+Shift+L` to activate/deactivate the laser pointer
- Move your mouse to draw
- Right-click system tray icon for settings

### What's New

- Initial release
- System-wide laser pointer overlay
- Customizable colors and settings
- Multi-monitor support

### System Requirements

- Windows 10 (version 1809 or later)
- Windows 11
- No additional software required (all dependencies included)

### Support

- Report issues: [GitHub Issues](https://github.com/yourusername/windows-laser-pointer/issues)
- View source: [GitHub Repository](https://github.com/yourusername/windows-laser-pointer)
```

---

## Automated Release (Optional)

### GitHub Actions Workflow

Create `.github\workflows\release.yml`:

```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'

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
            --no-build
      
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: LaserPointer-${{ matrix.platform }}
          path: LaserPointer/bin/Release/net8.0-windows10.0.19041.0/win-${{ matrix.platform == 'x64' && 'x64' || matrix.platform == 'x86' && 'x86' || 'arm64' }}/publish/LaserPointer.exe
  
  release:
    needs: build
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Download artifacts
        uses: actions/download-artifact@v4
        with:
          path: artifacts
      
      - name: Rename files
        run: |
          mv artifacts/LaserPointer-x64/LaserPointer.exe LaserPointer-${GITHUB_REF#refs/tags/}-x64.exe
          mv artifacts/LaserPointer-x86/LaserPointer.exe LaserPointer-${GITHUB_REF#refs/tags/}-x86.exe
          mv artifacts/LaserPointer-ARM64/LaserPointer.exe LaserPointer-${GITHUB_REF#refs/tags/}-ARM64.exe
        shell: bash
      
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          tag_name: ${{ github.ref }}
          name: Laser Pointer ${{ github.ref_name }}
          body: |
            ## Laser Pointer ${{ github.ref_name }}
            
            ### Download
            
            **Portable Executables (No Installation Required):**
            - [LaserPointer-${{ github.ref_name }}-x64.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-${{ github.ref_name }}-x64.exe) - 64-bit Windows (Recommended)
            - [LaserPointer-${{ github.ref_name }}-x86.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-${{ github.ref_name }}-x86.exe) - 32-bit Windows
            - [LaserPointer-${{ github.ref_name }}-ARM64.exe](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/LaserPointer-${{ github.ref_name }}-ARM64.exe) - Windows on ARM
            
            ### Installation
            
            1. Download the appropriate file for your system
            2. Run the executable - no installation needed!
            3. Press `Ctrl+Shift+L` to activate
            
            ### System Requirements
            - Windows 10 (1809+) or Windows 11
            - No additional software required
          files: |
            LaserPointer-*.exe
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

**To use automated releases:**

1. Create the workflow file above
2. Commit and push:
   ```bash
   git add .github/workflows/release.yml
   git commit -m "Add automated release workflow"
   git push
   ```

3. Create a tag to trigger release:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

4. GitHub Actions will automatically:
   - Build all platforms
   - Create release
   - Upload files
   - Publish release

---

## Troubleshooting

### Build Fails

**Error**: `Platform not found`
```powershell
# Solution: Specify platform correctly
dotnet build -c Release -p:Platform=x64
# NOT: dotnet build -c Release --platform x64
```

**Error**: `.NET SDK not found`
```powershell
# Solution: Install .NET 8.0 SDK
# Download from: https://dotnet.microsoft.com/download
```

### Executable Doesn't Run

**Issue**: Windows Defender blocks it
- **Solution**: This is normal for unsigned executables
- Users need to click "More info" → "Run anyway"
- Consider code signing for production (see GITHUB_DISTRIBUTION.md)

**Issue**: "Application failed to start"
- **Solution**: Ensure Windows 10 version 1809 or later
- Check Windows Event Viewer for detailed error

### GitHub Upload Issues

**Issue**: Files too large
- **Solution**: GitHub allows up to 2GB per file
- Your executables should be ~50-80 MB (well under limit)

**Issue**: Release not showing
- **Solution**: Check "Set as the latest release" checkbox
- Verify tag format: `v1.0.0` (with 'v' prefix)

---

## Production Checklist

Before releasing:

- [ ] Test executable on clean Windows VM
- [ ] Verify all features work (hotkey, drawing, settings)
- [ ] Check file sizes are reasonable (< 100 MB each)
- [ ] Update version numbers in code
- [ ] Write release notes
- [ ] Test on Windows 10 and Windows 11
- [ ] Verify multi-monitor support works
- [ ] Check system tray icon appears
- [ ] Test uninstall (just delete the .exe)

---

## User Instructions (For README)

Add this to your GitHub README:

```markdown
## Download

Download the latest release from the [Releases](https://github.com/yourusername/windows-laser-pointer/releases) page.

### Quick Start

1. Download `LaserPointer-x64.exe` (or appropriate version for your system)
2. Run the executable - no installation needed!
3. Press `Ctrl+Shift+L` to activate the laser pointer

### System Requirements

- Windows 10 (version 1809 or later)
- Windows 11
- No additional software required

### Installation

**No installation required!** Just download and run the `.exe` file.

You can:
- Run it from anywhere (Desktop, Downloads, Program Files, etc.)
- Create a shortcut if desired
- Delete the file to "uninstall"
```

---

## Summary

**Option 1 (Portable Executable) is the simplest approach:**

✅ **Pros:**
- No installer needed
- Users download and run immediately
- Works on any Windows 10/11 system
- Easy to update (just replace the .exe)
- No admin rights required

⚠️ **Considerations:**
- Users may see "Unknown Publisher" warning (normal for unsigned apps)
- No Start Menu shortcuts (users can create manually)
- No automatic updates (users download new version manually)

**This is perfect for:**
- Open source projects
- Quick distribution
- Users who prefer portable apps
- Simple deployment

---

**Ready to release?** Run:

```powershell
.\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
```

Then upload to GitHub!
