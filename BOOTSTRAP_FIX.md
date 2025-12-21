# Bootstrap Initialization Error - FIXED

## The Problem

When using `PublishSingleFile=true` with Windows App SDK unpackaged apps, the bootstrapper DLL (`Microsoft.WindowsAppRuntime.Bootstrap.dll`) cannot be loaded because it's bundled inside the single EXE file. This causes an `SEHException` during bootstrap initialization.

## The Solution

Changed from **single-file deployment** to **folder deployment**:
- `PublishSingleFile=false` - Creates a folder with EXE + all DLLs
- Package the folder as a ZIP for distribution
- Users extract the ZIP and run `LaserPointer.exe` from the folder

## What Changed

1. **Build-Portable.ps1**: Changed `PublishSingleFile=true` to `PublishSingleFile=false`
2. **Create-Release.ps1**: Now packages the entire publish folder as a ZIP file
3. **Release Notes**: Updated to reflect ZIP distribution instead of single EXE

## How to Test

1. **Rebuild the release:**
   ```powershell
   .\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
   ```

2. **Extract and test:**
   - Extract `Releases\v1.0.0\LaserPointer-1.0.0-x64.zip`
   - Run `LaserPointer.exe` from the extracted folder
   - It should work without the bootstrap error!

## Why This Works

- The bootstrapper DLL is now a separate file in the same folder as the EXE
- Windows can find and load the DLL properly
- All dependencies are in the same folder, making it portable

## Distribution

Users will:
1. Download the ZIP file (e.g., `LaserPointer-1.0.0-x64.zip`)
2. Extract it to any folder
3. Run `LaserPointer.exe` from that folder

This is still portable - users can put it anywhere and it will work!
