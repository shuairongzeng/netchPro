# Netch Remaining Issues

> Document created: 2026-01-04
> Last updated: 2026-01-04

## Overview

This document tracks remaining issues and pending updates after the .NET 8 upgrade and dependency updates.

---

## 1. Netfilter SDK Driver Update

### Current Status
| File | Current Version | Latest Version | Status |
|------|-----------------|----------------|--------|
| nfdriver.sys | Unknown (2022) | 1.7.4.0 (2025-09) | Pending |
| nfapi.dll | Unknown (2022) | 1.7.4.0 (2025-09) | Pending |

### Issue Description
The Netfilter SDK components (nfdriver.sys and nfapi.dll) are from 2022. The latest version 1.7.4.0 includes:
- Fixed TCP handling issues (v1.7.2.1)
- Performance improvements
- Bug fixes

### Update Instructions
1. Visit https://netfiltersdk.com/download.html
2. Download "Demo version of NetFilter SDK for Windows" (build 1.7.4.0)
3. Extract the archive
4. Copy files:
   - `bin\x64\nfapi.dll` → `Redirector\static\nfapi.dll`
   - `bin\x64\nfdriver.sys` → `Storage\nfdriver.sys`
5. Rebuild Redirector:
   ```powershell
   msbuild Redirector\Redirector.vcxproj /p:Configuration=Release /p:Platform=x64
   ```
6. Copy updated files to release:
   ```powershell
   Copy-Item Redirector\bin\Release\nfapi.dll release\bin\
   Copy-Item Storage\nfdriver.sys release\bin\
   ```

### Priority
**Low** - Current version works fine. Update only if experiencing TCP connection issues.

---

## 2. pcap2socks Update

### Current Status
| File | Current Version | Latest Version | Status |
|------|-----------------|----------------|--------|
| pcap2socks.exe | v0.6.2 (2021) | v0.6.2 (2021) | No Update Available |

### Issue Description
The pcap2socks project (https://github.com/zhxie/pcap2socks) has not been updated since 2022. The last release is v0.6.2.

### Impact
- ShareMode functionality depends on this component
- No security patches or improvements available
- Works with current functionality

### Recommendation
- Consider alternative solutions if ShareMode issues arise
- Monitor the project for any future updates

---

## 3. Code Quality Warnings

### Nullable Reference Warnings
The following files have nullable reference warnings that should be addressed in future updates:

- `Netch\Servers\V2ray\V2rayConfig.cs` - CS8632
- `Netch\Forms\MainForm.cs` - CS8600, CS8602, CS8604
- `Netch\Forms\BindingForm.cs` - CS8600, CS8602
- `Netch\Forms\LogForm.cs` - CS8604
- `Netch\Utils\Utils.cs` - CS8602
- `Netch\Utils\i18N.cs` - CS8604

### Obsolete API Warnings
- `Netch\Utils\WebUtil.cs:21` - SYSLIB0014: WebRequest.Create is obsolete, use HttpClient
- `Netch\Utils\StringExtension.cs:88` - SYSLIB0021: SHA1CryptoServiceProvider is obsolete

### Threading Warnings
- `Netch\Controllers\TUNController.cs:137-138` - VSTHRD002: Synchronous wait may cause deadlocks
- `Netch\Services\Redirectors\NetfilterRedirector.cs:66` - VSTHRD002: Synchronous wait

---

## 4. Build Environment Requirements

### Required Tools
For full source build, the following tools are required:

| Tool | Purpose | Installation |
|------|---------|--------------|
| Visual Studio 2022 Build Tools | C++ compilation | Installed |
| .NET 8 SDK | C# compilation | Installed |
| Go 1.23+ | aiodns, v2ray compilation | Installed |
| Rust | pcap2socks compilation | Installed |
| MinGW-w64 (GCC) | CGO support for Go | Installed at C:\msys64 |

### Build Commands
```powershell
# Full build
.\build.ps1 -Configuration Release -OutputPath release

# C# only
dotnet publish -c Release -r win-x64 -p:Platform=x64 -o .\release .\Netch\Netch.csproj

# C++ components
msbuild Redirector\Redirector.vcxproj /p:Configuration=Release /p:Platform=x64
msbuild RouteHelper\RouteHelper.vcxproj /p:Configuration=Release /p:Platform=x64
```

---

## 5. Future Improvements

### Recommended Updates
1. **Replace WebRequest with HttpClient** - Modern async HTTP handling
2. **Fix nullable reference warnings** - Improve null safety
3. **Update async patterns** - Replace synchronous waits with proper async/await
4. **Consider SHA256 for hashing** - Replace SHA1CryptoServiceProvider

### Architecture Improvements
1. Complete the INetworkRedirector abstraction layer
2. Add TUN and Pcap redirector implementations
3. Implement dependency injection for better testability

---

## Changelog

### 2026-01-04
- Upgraded from .NET 6 to .NET 8
- Updated all NuGet packages to latest versions
- Updated v2ray-core to v5.43.0
- Updated wintun to 0.14.1
- Updated tun2socks to v2.5.2
- Rebuilt Redirector.bin and RouteHelper.bin
- Rebuilt aiodns.bin with latest Go
- Updated GeoLite2-Country.mmdb
- Added driver abstraction layer (INetworkRedirector)
- Added dependency manifest and checker
