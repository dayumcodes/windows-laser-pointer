# Production Publishing Guide - Laser Pointer

This document outlines all requirements, steps, and considerations for publishing the Laser Pointer application to production.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Build Requirements](#build-requirements)
3. [Platform Targets](#platform-targets)
4. [Code Signing](#code-signing)
5. [MSIX Packaging](#msix-packaging)
6. [Microsoft Store Submission](#microsoft-store-submission)
7. [Alternative Distribution Methods](#alternative-distribution-methods)
8. [Version Management](#version-management)
9. [Testing Checklist](#testing-checklist)
10. [Security & Permissions](#security--permissions)
11. [Performance Optimization](#performance-optimization)
12. [Release Checklist](#release-checklist)

---

## Prerequisites

### Development Environment
- **Visual Studio 2022** (17.8 or later) with:
  - .NET desktop development workload
  - Windows App SDK (WinUI 3) workload
  - Windows 10/11 SDK (10.0.19041.0 or later)
  - MSIX Packaging Tools extension
- **.NET 8.0 SDK** or later
- **Windows 10 version 1809** (build 17763) or later / **Windows 11**

### Required Accounts
- **Microsoft Partner Center Account** (for Store submission)
- **Code Signing Certificate** (for trusted distribution)
- **GitHub/GitLab Account** (for source control and releases)

---

## Build Requirements

### Project Configuration
- **Target Framework**: `net8.0-windows10.0.19041.0`
- **Minimum Platform Version**: `10.0.17763.0`
- **Platforms**: `x86`, `x64`, `ARM64`
- **Runtime Identifiers**: `win-x86`, `win-x64`, `win-arm64`
- **Windows Package Type**: Currently `None` (unpackaged), can be changed to `MSIX` for Store

### NuGet Packages
- `Microsoft.WindowsAppSDK` (Version: 1.5.240627000)
- `Microsoft.Graphics.Win2D` (Version: 1.1.2 or 1.2.0)
- `Microsoft.Windows.SDK.BuildTools` (Version: 10.0.26100.1)

### Build Commands

#### Debug Build
```bash
dotnet build -c Debug -p:Platform=x64
```

#### Release Build (Single Platform)
```bash
dotnet build -c Release -p:Platform=x64
```

#### Release Build (All Platforms)
```bash
dotnet build -c Release -p:Platform=x86
dotnet build -c Release -p:Platform=x64
dotnet build -c Release -p:Platform=ARM64
```

#### Publish Self-Contained (Recommended for Distribution)
```bash
# x64
dotnet publish -c Release -p:Platform=x64 -p:SelfContained=true -p:PublishSingleFile=true -p:RuntimeIdentifier=win-x64

# x86
dotnet publish -c Release -p:Platform=x86 -p:SelfContained=true -p:PublishSingleFile=true -p:RuntimeIdentifier=win-x86

# ARM64
dotnet publish -c Release -p:Platform=ARM64 -p:SelfContained=true -p:PublishSingleFile=true -p:RuntimeIdentifier=win-arm64
```

---

## Platform Targets

### Supported Platforms
1. **x64** (Primary - Recommended)
   - Most common Windows architecture
   - Best performance
   - Primary target for Store submission

2. **x86** (32-bit)
   - Legacy support
   - Lower performance
   - Required for older systems

3. **ARM64** (Windows on ARM)
   - Surface Pro X and similar devices
   - Growing market share
   - Recommended for future-proofing

### Platform-Specific Considerations
- **x64**: Default and recommended platform
- **x86**: May have performance limitations with Win2D
- **ARM64**: Requires ARM64-compatible dependencies

---

## Code Signing

### Why Code Signing is Required
- **User Trust**: Prevents "Unknown Publisher" warnings
- **Windows Defender**: Reduces false positives
- **Enterprise Deployment**: Required for corporate environments
- **Store Submission**: Required for Microsoft Store

### Options for Code Signing

#### Option 1: Commercial Code Signing Certificate (Recommended)
- **Cost**: $200-500/year
- **Providers**: DigiCert, Sectigo, GlobalSign, SSL.com
- **Validity**: 1-3 years
- **Trust Level**: High (trusted by all Windows versions)

#### Option 2: Extended Validation (EV) Certificate
- **Cost**: $300-600/year
- **Benefits**: Immediate trust, no reputation building needed
- **Best for**: Commercial products

#### Option 3: Open Source / Community (Alternative)
- Use **ClickOnce** or **Squirrel** for auto-updates
- Consider **Authenticode** signing with self-signed cert (limited trust)
- For open source: GitHub Actions can automate signing

### Signing Process

#### Using SignTool (Windows SDK)
```bash
# Sign the executable
signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com /d "Laser Pointer" /du "https://yourwebsite.com" LaserPointer.exe

# Verify signature
signtool verify /pa LaserPointer.exe
```

#### Using PowerShell
```powershell
Set-AuthenticodeSignature -FilePath "LaserPointer.exe" -Certificate (Get-PfxData -FilePath "certificate.pfx" -Password $securePassword).EndEntityCertificates[0] -TimestampServer "http://timestamp.digicert.com"
```

---

## MSIX Packaging

### Why MSIX?
- **Modern Windows Packaging**: Recommended by Microsoft
- **Store Compatibility**: Required for Microsoft Store
- **Auto-Updates**: Built-in update mechanism
- **Security**: Sandboxed execution
- **User Experience**: Clean install/uninstall

### Creating MSIX Package

#### Step 1: Update Project File
In `LaserPointer.csproj`, change:
```xml
<WindowsPackageType>None</WindowsPackageType>
```
to:
```xml
<WindowsPackageType>MSIX</WindowsPackageType>
```

#### Step 2: Configure Package.appxmanifest
Ensure `Package.appxmanifest` includes:
- **Identity**: Publisher, Name, Version
- **Capabilities**: 
  - `runFullTrust` (required for global hooks)
  - `allowElevation` (if needed)
- **Declarations**: 
  - `AppExecutionAlias` (optional, for command-line access)
- **Visual Elements**: Icons, splash screen, display name

#### Step 3: Build MSIX
```bash
# Using Visual Studio
Right-click project → Publish → Create App Packages

# Using MSBuild
msbuild LaserPointer.csproj /p:Configuration=Release /p:Platform=x64 /p:AppxBundle=Always /p:AppxBundlePlatforms="x64|x86|ARM64"
```

#### Step 4: Test MSIX
```bash
# Install for testing
Add-AppxPackage -Path LaserPointer_1.0.0.0_x64.msix

# Uninstall
Get-AppxPackage LaserPointer | Remove-AppxPackage
```

### MSIX Bundle Structure
```
LaserPointer_1.0.0.0_x64_x86_ARM64.msixbundle
├── LaserPointer_1.0.0.0_x64.msix
├── LaserPointer_1.0.0.0_x86.msix
└── LaserPointer_1.0.0.0_ARM64.msix
```

---

## Microsoft Store Submission

### Prerequisites
1. **Microsoft Partner Center Account**
   - Sign up at https://partner.microsoft.com/dashboard
   - Pay $19 one-time registration fee (waived for students/educators)
   - Complete business verification

2. **App Identity**
   - Unique app name
   - Publisher display name
   - App description (max 10,000 characters)
   - Category: Productivity / Utilities
   - Age rating: E (Everyone)

3. **Assets Required**
   - **Store Logo**: 300x300px PNG
   - **Screenshots**: 
     - Desktop: 1366x768px minimum (up to 10)
     - Tablet: 1920x1080px (optional)
   - **Promotional Images**: 414x468px (optional)
   - **App Icon**: 44x44px (already in Assets folder)

### Submission Process

#### Step 1: Create App Listing
1. Log into Partner Center
2. Create new app
3. Fill in app information:
   - Name: "Laser Pointer"
   - Category: Productivity
   - Description: [Use content from README.md]
   - Privacy Policy URL: [Required - create privacy policy]

#### Step 2: Upload Package
1. Go to "Packages" section
2. Upload `.msixbundle` file
3. Wait for certification (24-48 hours)

#### Step 3: Age Rating
- Complete age rating questionnaire
- Select appropriate rating (likely E for Everyone)

#### Step 4: Pricing & Availability
- Set price (Free or Paid)
- Select markets (Worldwide recommended)
- Set availability dates

#### Step 5: Store Listing
- Upload screenshots
- Add promotional text
- Set keywords for search

#### Step 6: Submit for Certification
- Review all sections
- Submit for certification
- Wait for approval (typically 1-3 business days)

### Store Requirements Checklist
- [ ] App passes Windows App Certification Kit (WACK)
- [ ] Privacy policy URL provided
- [ ] All required assets uploaded
- [ ] App description complete
- [ ] Screenshots provided (minimum 1, recommended 3-5)
- [ ] Age rating completed
- [ ] Pricing configured
- [ ] Code signing certificate valid
- [ ] App tested on multiple Windows versions
- [ ] No known critical bugs

---

## Alternative Distribution Methods

### Option 1: GitHub Releases (Recommended for Open Source)
**Pros:**
- Free hosting
- Direct download
- Version management
- Release notes

**Steps:**
1. Create GitHub repository
2. Build release artifacts:
   ```bash
   dotnet publish -c Release -p:Platform=x64 -p:SelfContained=true -p:PublishSingleFile=true
   ```
3. Create release on GitHub
4. Upload `.exe` files for each platform
5. Add release notes

### Option 2: Squirrel.Windows (Auto-Updates)
**Pros:**
- Automatic updates
- Delta updates
- Easy installation

**Implementation:**
```bash
# Install Squirrel
dotnet add package Squirrel.Windows

# Create installer
Squirrel.exe --releasify LaserPointer.nupkg
```

### Option 3: ClickOnce
**Pros:**
- Simple deployment
- Auto-updates
- Built into Visual Studio

**Steps:**
1. Right-click project → Properties → Publish
2. Configure publish settings
3. Publish to web server or file share

### Option 4: WiX Toolset (MSI Installer)
**Pros:**
- Traditional Windows installer
- Enterprise-friendly
- Full control

**Considerations:**
- More complex setup
- Requires WiX knowledge

---

## Version Management

### Version Format
Use Semantic Versioning: `MAJOR.MINOR.PATCH.BUILD`

Example: `1.0.0.0`
- **1**: Major version (breaking changes)
- **0**: Minor version (new features)
- **0**: Patch version (bug fixes)
- **0**: Build number (auto-increment)

### Version Locations
Update version in:
1. **Package.appxmanifest**:
   ```xml
   <Identity Name="YourPublisher.LaserPointer" Version="1.0.0.0" />
   ```

2. **AssemblyInfo.cs** (if exists):
   ```csharp
   [assembly: AssemblyVersion("1.0.0.0")]
   [assembly: AssemblyFileVersion("1.0.0.0")]
   ```

3. **Project File** (optional):
   ```xml
   <PropertyGroup>
     <Version>1.0.0.0</Version>
   </PropertyGroup>
   ```

### Versioning Strategy
- **Major**: Breaking API changes, major UI redesign
- **Minor**: New features, new color presets, new settings
- **Patch**: Bug fixes, performance improvements
- **Build**: Automated build number (CI/CD)

---

## Testing Checklist

### Functional Testing
- [ ] Hotkey activation works (Ctrl+Shift+L)
- [ ] Overlay window appears/disappears correctly
- [ ] Laser drawing works on all monitors
- [ ] All color presets work
- [ ] Settings persist after restart
- [ ] Fade duration works correctly
- [ ] Line thickness adjustment works
- [ ] Window background colors work (Transparent, White, DarkGray, etc.)
- [ ] Continuous drawing works when hotkey is active
- [ ] Click-through works (can interact with apps behind overlay)

### Platform Testing
- [ ] Test on Windows 10 (1809, 1909, 20H2, 21H1, 21H2, 22H2)
- [ ] Test on Windows 11 (all versions)
- [ ] Test on x64 architecture
- [ ] Test on x86 architecture (if supported)
- [ ] Test on ARM64 architecture (if supported)

### Performance Testing
- [ ] No memory leaks during extended use
- [ ] Smooth 60fps drawing performance
- [ ] Low CPU usage when idle
- [ ] Low memory footprint (< 50MB)
- [ ] Fast startup time (< 2 seconds)

### Security Testing
- [ ] No elevation prompts required
- [ ] Works with Windows Defender enabled
- [ ] No false positives from antivirus
- [ ] Code signing certificate valid
- [ ] No sensitive data in logs

### UI/UX Testing
- [ ] Settings window opens/closes correctly
- [ ] All UI elements are accessible
- [ ] High DPI scaling works correctly
- [ ] Multi-monitor setup works
- [ ] Window positioning correct on all screens

### Edge Cases
- [ ] Rapid hotkey presses don't cause issues
- [ ] Works when multiple monitors disconnected
- [ ] Handles screen resolution changes
- [ ] Works in fullscreen applications
- [ ] Handles Windows sleep/wake correctly

---

## Security & Permissions

### Required Capabilities

#### runFullTrust (Required)
- **Why**: Needed for global mouse hooks and hotkey registration
- **Impact**: App runs with full trust (not sandboxed)
- **Store Approval**: May require justification in Store submission

#### allowElevation (Optional)
- **Why**: Only if admin privileges needed
- **Current Status**: Not required for current functionality

### Security Considerations
1. **Global Hooks**: 
   - Only capture mouse movement, not keystrokes
   - No data collection or transmission
   - Document in privacy policy

2. **Code Signing**:
   - Required to prevent "Unknown Publisher" warnings
   - Builds user trust
   - Reduces antivirus false positives

3. **Privacy Policy**:
   - Required for Store submission
   - Must state: No data collection, no network access, local-only operation

### Privacy Policy Template
Create `PRIVACY.md`:
```markdown
# Privacy Policy for Laser Pointer

## Data Collection
Laser Pointer does not collect, store, or transmit any personal data.

## Local Storage
Settings are stored locally on your device only.

## Network Access
Laser Pointer does not access the internet or any network resources.

## Permissions
- Global mouse tracking: Required for drawing functionality
- Global hotkey registration: Required for activation
- No other permissions required
```

---

## Performance Optimization

### Build Optimizations
```xml
<PropertyGroup Condition="'$(Configuration)'=='Release'">
  <Optimize>true</Optimize>
  <DebugType>none</DebugType>
  <DebugSymbols>false</DebugSymbols>
  <PublishTrimmed>true</PublishTrimmed>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
</PropertyGroup>
```

### Runtime Optimizations
- ✅ Already using hardware-accelerated Win2D
- ✅ Efficient stroke management (fade-out removes old strokes)
- ✅ Minimal memory allocation in drawing loop
- ✅ Single-threaded UI updates (DispatcherQueue)

### File Size Optimization
- Use `PublishTrimmed=true` to reduce size
- Use `PublishSingleFile=true` for single executable
- Consider removing unused assets

---

## Release Checklist

### Pre-Release
- [ ] All tests pass
- [ ] Version number updated
- [ ] Release notes prepared
- [ ] Code signing certificate valid
- [ ] All dependencies up to date
- [ ] No known critical bugs
- [ ] Documentation updated

### Build
- [ ] Clean solution
- [ ] Build Release configuration for all platforms
- [ ] Run Windows App Certification Kit (WACK)
- [ ] Sign all executables
- [ ] Create MSIX bundle (if Store submission)
- [ ] Test installation on clean Windows VM

### Store Submission
- [ ] Partner Center account active
- [ ] App listing complete
- [ ] All assets uploaded
- [ ] Privacy policy published
- [ ] Age rating completed
- [ ] Pricing configured
- [ ] Package uploaded
- [ ] Submitted for certification

### Post-Release
- [ ] Monitor crash reports (if Store)
- [ ] Monitor user reviews
- [ ] Prepare hotfix if needed
- [ ] Update documentation
- [ ] Announce release (social media, blog, etc.)

---

## Windows App Certification Kit (WACK)

### Running WACK
1. Install Windows SDK
2. Run: `Windows App Certification Kit`
3. Select "Windows App" test
4. Point to your `.msix` or `.appx` file
5. Review results

### Common Issues
- **Missing Privacy Policy**: Add privacy policy URL
- **Capability Justification**: Explain why `runFullTrust` is needed
- **Performance**: Ensure app starts quickly
- **Accessibility**: Ensure keyboard navigation works

---

## Continuous Integration / Continuous Deployment (CI/CD)

### GitHub Actions Example
```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Build
        run: dotnet build -c Release -p:Platform=x64
      - name: Publish
        run: dotnet publish -c Release -p:Platform=x64 -p:SelfContained=true
      - name: Sign
        run: signtool sign /f cert.pfx /p ${{ secrets.CERT_PASSWORD }} LaserPointer.exe
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/LaserPointer.exe
```

---

## Support & Maintenance

### User Support Channels
- GitHub Issues (for open source)
- Email support (for commercial)
- Microsoft Store reviews (respond to reviews)

### Update Strategy
- **Major Updates**: New features, breaking changes
- **Minor Updates**: Bug fixes, small improvements
- **Hotfixes**: Critical security/bug fixes

### Deprecation Policy
- Announce deprecation 6 months in advance
- Provide migration path if applicable
- Maintain last version for 1 year after deprecation

---

## Additional Resources

### Microsoft Documentation
- [Windows App SDK Documentation](https://docs.microsoft.com/windows/apps/windows-app-sdk/)
- [MSIX Packaging](https://docs.microsoft.com/windows/msix/)
- [Partner Center](https://docs.microsoft.com/windows/uwp/publish/)
- [App Certification](https://docs.microsoft.com/windows/uwp/debug-test-perf/windows-app-certification-kit)

### Tools
- [Windows App Certification Kit](https://developer.microsoft.com/windows/downloads/app-certification-kit/)
- [MSIX Packaging Tool](https://www.microsoft.com/store/productId/9N5LW3JBCXKF)
- [SignTool](https://docs.microsoft.com/windows/win32/seccrypto/signtool)

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0.0 | TBD | Initial release |

---

**Last Updated**: [Current Date]
**Maintained By**: [Your Name/Team]
