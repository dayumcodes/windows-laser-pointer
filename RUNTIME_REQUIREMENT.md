# Windows App Runtime Requirement

## The Issue

The application requires **Windows App Runtime 1.5** to be installed on the system, even with `WindowsAppSDKSelfContained=true`.

The bootstrapper API (`MddBootstrapInitialize2`) needs the Windows App Runtime framework package to be installed on Windows, regardless of whether the DLLs are bundled with the app.

## Solution: Install Windows App Runtime

### For x64 Systems (Most Common)

1. **Download the installer:**
   - Direct link: https://aka.ms/windowsappsdk/1.5/stable/windowsappruntimeinstall-x64.exe
   - Or visit: https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads

2. **Run the installer:**
   - Right-click the downloaded file
   - Select "Run as Administrator"
   - Follow the installation wizard

3. **Restart the application**

### For x86 Systems

Download: https://aka.ms/windowsappsdk/1.5/stable/windowsappruntimeinstall-x86.exe

### For ARM64 Systems

Download: https://aka.ms/windowsappsdk/1.5/stable/windowsappruntimeinstall-arm64.exe

## Verify Installation

Run this PowerShell command to check if it's installed:

```powershell
Get-AppxPackage -Name Microsoft.WindowsAppRuntime*
```

If you see packages listed, the runtime is installed.

## Alternative: Bundle Runtime Installer

For distribution, you can:
1. Include the runtime installer with your app
2. Create a setup script that installs it automatically
3. Provide clear instructions to users

## Why This Is Required

Unpackaged Windows App SDK apps require the Windows App Runtime framework package to be installed on the system. This is a Windows requirement, not a limitation of the app.

The `WindowsAppSDKSelfContained=true` property bundles the DLLs but doesn't eliminate the need for the runtime package registration.
