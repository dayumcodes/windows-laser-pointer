# How to Run Release Scripts

## Important: Where to Run the Scripts

**You must run the scripts from the PROJECT ROOT directory**, not from inside the `LaserPointer` folder.

### Correct Directory Structure

```
laser pointer/                    ← RUN SCRIPTS FROM HERE (project root)
├── Scripts/
│   ├── Build-Portable.ps1
│   ├── Create-Release.ps1
│   └── Release-To-GitHub.ps1
├── LaserPointer/                 ← NOT from here!
│   ├── LaserPointer.csproj
│   └── ...
└── Releases/
```

## Step-by-Step Instructions

### Step 1: Open PowerShell

1. Press `Win + X` and select "Windows PowerShell" or "Terminal"
2. Or press `Win + R`, type `powershell`, press Enter

### Step 2: Navigate to Project Root

```powershell
# Navigate to the project root
cd "C:\Users\mfuza\Downloads\laser pointer"
```

**Verify you're in the right place:**
```powershell
# Check current directory
pwd
# Should show: C:\Users\mfuza\Downloads\laser pointer

# Verify Scripts folder exists
Test-Path ".\Scripts\Create-Release.ps1"
# Should return: True
```

### Step 3: Run the Release Script

```powershell
# Run from project root
.\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
```

**OR use the complete workflow script:**
```powershell
.\Scripts\Release-To-GitHub.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"
```

## Common Mistakes

### ❌ Wrong: Running from LaserPointer folder
```powershell
PS C:\Users\mfuza\Downloads\laser pointer\LaserPointer> .\Scripts\Create-Release.ps1
# ERROR: Script not found
```

### ✅ Correct: Running from project root
```powershell
PS C:\Users\mfuza\Downloads\laser pointer> .\Scripts\Create-Release.ps1 -Version "1.0.0"
# SUCCESS: Script runs correctly
```

## Quick Check Commands

Before running scripts, verify your location:

```powershell
# Check current directory
Get-Location

# Should show:
# Path
# ----
# C:\Users\mfuza\Downloads\laser pointer

# Verify Scripts folder
ls Scripts

# Should show:
# Build-Portable.ps1
# Create-Release.ps1
# Release-To-GitHub.ps1
```

## Full Example

```powershell
# 1. Open PowerShell
# 2. Navigate to project root
cd "C:\Users\mfuza\Downloads\laser pointer"

# 3. Verify location
pwd
# Output: C:\Users\mfuza\Downloads\laser pointer

# 4. Run the script
.\Scripts\Create-Release.ps1 -Version "1.0.0" -ReleaseNotes "Initial release"

# 5. Wait for build to complete
# 6. Files will be in: Releases\v1.0.0\
```

## If You Get "Script Not Found" Error

1. **Check your current directory:**
   ```powershell
   pwd
   ```

2. **If you're in LaserPointer folder, go up one level:**
   ```powershell
   cd ..
   ```

3. **Verify Scripts folder exists:**
   ```powershell
   Test-Path ".\Scripts\Create-Release.ps1"
   ```

4. **If still not found, check the folder structure:**
   ```powershell
   Get-ChildItem -Directory
   # Should show: Scripts, LaserPointer, Releases, etc.
   ```

## Troubleshooting

### Error: "Cannot be loaded because running scripts is disabled"

**Solution:** Enable script execution
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Error: "Platform not found"

**Solution:** Make sure you're using the correct parameter format
```powershell
# Correct
.\Scripts\Build-Portable.ps1 -Platform x64

# Wrong
.\Scripts\Build-Portable.ps1 --platform x64
```

### Error: "dotnet command not found"

**Solution:** Install .NET 8.0 SDK
- Download from: https://dotnet.microsoft.com/download
- Or install via Visual Studio Installer

---

## Summary

**Always run scripts from:**
```
C:\Users\mfuza\Downloads\laser pointer
```

**NOT from:**
```
C:\Users\mfuza\Downloads\laser pointer\LaserPointer
```
