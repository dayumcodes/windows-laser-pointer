# Where is the EXE File Built?

## Build Locations

The executable files are built in the following locations:

### Direct Build Output (Before Release Script)
- **x64**: `LaserPointer\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\publish\LaserPointer.exe`
- **x86**: `LaserPointer\bin\x86\Release\net8.0-windows10.0.19041.0\win-x86\publish\LaserPointer.exe`
- **ARM64**: `LaserPointer\bin\ARM64\Release\net8.0-windows10.0.19041.0\win-arm64\publish\LaserPointer.exe`

### Release Package (After Running Create-Release.ps1)
- **Location**: `Releases\v1.0.0\`
- **Files**:
  - `LaserPointer-1.0.0-x64.exe`
  - `LaserPointer-1.0.0-x86.exe`
  - `LaserPointer-1.0.0-ARM64.exe`

## How to Run the EXE

### Option 1: Run from Build Output (Development/Testing)

1. **Navigate to the publish folder:**
   ```powershell
   cd "C:\Users\mfuza\Downloads\laser pointer\LaserPointer\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\publish"
   ```

2. **Double-click `LaserPointer.exe`** or run from command line:
   ```powershell
   .\LaserPointer.exe
   ```

### Option 2: Run from Release Package (Distribution)

1. **Navigate to the release folder:**
   ```powershell
   cd "C:\Users\mfuza\Downloads\laser pointer\Releases\v1.0.0"
   ```

2. **Double-click the appropriate EXE:**
   - `LaserPointer-1.0.0-x64.exe` (for 64-bit Windows - most common)
   - `LaserPointer-1.0.0-x86.exe` (for 32-bit Windows)
   - `LaserPointer-1.0.0-ARM64.exe` (for Windows on ARM devices)

### Option 3: Run from Anywhere (After Copying)

1. **Copy the EXE to any folder** (e.g., Desktop, Program Files, etc.)
2. **Double-click to run** - no installation needed!

## Quick Test

To quickly test if the EXE works:

```powershell
# Navigate to project root
cd "C:\Users\mfuza\Downloads\laser pointer"

# Run the x64 version (most common)
.\LaserPointer\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\publish\LaserPointer.exe
```

## What to Expect

When you run the EXE:
1. **System Tray Icon**: A laser pointer icon appears in your system tray (bottom-right)
2. **No Window Opens**: The app runs in the background
3. **Activation**: Press `Ctrl+Shift+L` (default hotkey) to activate the laser pointer overlay
4. **Settings**: Right-click the system tray icon to access settings

## Troubleshooting

### "Windows protected your PC" Warning
- This is normal for unsigned executables
- Click "More info" → "Run anyway"
- To avoid this, you need to code-sign the EXE (see `PRODUCTION_GITHUB.md`)

### "Missing DLL" or "Application Error"
- Make sure you're running the EXE from the `publish` folder (it contains all dependencies)
- Or use the single-file executable from the `Releases` folder

### App Doesn't Start
- Check Windows Event Viewer for errors
- Make sure Windows App SDK runtime is installed (usually auto-installed)
- Try running as Administrator

## File Sizes

Expected file sizes:
- **x64**: ~50-80 MB (single-file, self-contained)
- **x86**: ~45-70 MB
- **ARM64**: ~50-80 MB

These are large because they include the entire .NET runtime and all dependencies.
