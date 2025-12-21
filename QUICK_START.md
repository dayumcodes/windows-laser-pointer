# Quick Start - Release to GitHub

**Simplest way to release Laser Pointer to GitHub as a portable executable.**

## One Command Release

```powershell
.\Scripts\Release-To-GitHub.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
```

This will:
1. ✅ Build executables for all platforms (x64, x86, ARM64)
2. ✅ Create release package in `Releases\v1.0.0\`
3. ✅ Generate release notes

## Then Upload to GitHub

1. Go to your GitHub repository → **Releases** → **Create a new release**
2. **Tag**: `v1.0.0`
3. **Title**: `Laser Pointer v1.0.0`
4. **Description**: Copy from `Releases\v1.0.0\RELEASE_NOTES.md`
5. **Upload files**: Drag all `.exe` files from `Releases\v1.0.0\` folder
6. **Publish**!

## What Users Get

- **Single `.exe` file** - No installation needed
- **Download and run** - Works immediately
- **All dependencies included** - No .NET installation required
- **Works on Windows 10/11**

## File Structure

After running the script:

```
Releases/
└── v1.0.0/
    ├── LaserPointer-1.0.0-x64.exe      (64-bit - Recommended)
    ├── LaserPointer-1.0.0-x86.exe      (32-bit)
    ├── LaserPointer-1.0.0-ARM64.exe    (ARM64)
    └── RELEASE_NOTES.md                 (Copy this to GitHub)
```

## Test Before Uploading

```powershell
# Test the executable
Start-Process ".\Releases\v1.0.0\LaserPointer-1.0.0-x64.exe"

# Verify:
# - Press Ctrl+Shift+L to activate
# - Move mouse to draw
# - Press Ctrl+Shift+L to deactivate
```

## That's It!

Users can now download and use your application directly from GitHub.

For detailed instructions, see `PRODUCTION_GITHUB.md`.
