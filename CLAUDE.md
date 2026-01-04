# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Netch is a Windows proxy client that supports multiple proxy protocols (Socks5, Shadowsocks, ShadowsocksR, WireGuard, Trojan, VMess, VLESS) and operating modes (ProcessMode, ShareMode, TunMode). It's a .NET 6.0 Windows Forms application with native C++ components.

## Build Commands

**Full build (PowerShell):**
```powershell
.\build.ps1 -Configuration Release -OutputPath release
```

**Build main C# project only:**
```powershell
dotnet publish -c Release -r win-x64 -p:Platform=x64 -o .\Netch\bin\Release .\Netch\Netch.csproj
```

**Build native components (requires MSBuild):**
```powershell
msbuild -property:Configuration=Release -property:Platform=x64 .\Redirector\Redirector.vcxproj
msbuild -property:Configuration=Release -property:Platform=x64 .\RouteHelper\RouteHelper.vcxproj
```

**Run tests:**
```powershell
dotnet test .\Tests\Tests.csproj
```

## Architecture

### Solution Structure

- **Netch/** - Main C# WinForms application (.NET 6.0-windows)
- **Redirector/** - C++ DLL for traffic interception using Netfilter SDK (outputs `Redirector.bin`, `nfapi.dll`)
- **RouteHelper/** - C++ DLL for Windows routing table manipulation (outputs `RouteHelper.bin`)
- **RedirectorTester/** - Test harness for the Redirector component
- **Tests/** - MSTest unit tests
- **Other/** - External dependencies (aiodns, pcap2socks, v2ray-sn, wintun) built via `Other\build.ps1`

### Core Application Flow

1. **Program.cs** - Entry point: handles single instance, logging (Serilog), i18n loading, and launches MainForm
2. **MainController** - Orchestrates server and mode controllers to establish proxy connections
3. **IServerController** - Interface for protocol-specific controllers that create local Socks5 servers
4. **IModeController** - Interface for mode controllers that route traffic through the Socks5 server

### Mode Types (Netch/Models/Modes/ModeType.cs)

| Mode | Controller | Description |
|------|------------|-------------|
| ProcessMode | NFController | Uses Netfilter driver to intercept process traffic |
| ShareMode | PcapController | Network sharing via WinPcap/Npcap |
| TunMode | TUNController | Virtual adapter via WinTUN driver |

### Server Protocol Implementation Pattern

Each protocol in `Netch/Servers/{Protocol}/` follows the same structure:
- `{Protocol}Server.cs` - Server model
- `{Protocol}Controller.cs` - Implements IServerController
- `{Protocol}Form.cs` - Server edit form
- `{Protocol}Util.cs` - Implements IServerUtil for parsing share links

Supported protocols: Shadowsocks, ShadowsocksR, Socks5, SSH, Trojan, VMess, VLESS, WireGuard

### Native Component Interop

**Redirector.bin** - Traffic interception via P/Invoke:
- `aio_register/unregister` - Register/unregister process names
- `aio_dial` - Configure filtering options (DNS, TCP, UDP, ICMP)
- `aio_init/free` - Initialize/cleanup driver

**RouteHelper.bin** - Routing table via P/Invoke:
- `CreateRoute/DeleteRoute` - Manage IPv4/IPv6 routes
- `CreateIPv4/CreateUnicastIP` - Configure interface addresses

### Key Directories

- `Netch/Controllers/` - Mode and utility controllers (DNS, NF, TUN, Pcap)
- `Netch/Services/` - ModeService (mode loading/management), Updater
- `Netch/Utils/` - Configuration, DnsUtils, Firewall, i18N, PortHelper, etc.
- `Netch/Models/` - Server, Mode, Settings models
- `Storage/` - Runtime files (modes, i18n, drivers)

## Development Notes

- x64 only - no x86 or ARM support
- Requires Windows 10 build 17763+ (October 2018 Update)
- Uses CsWin32 for P/Invoke code generation (NativeMethods.txt)
- ConfigureAwait.Fody handles ConfigureAwait(false) automatically
- Code analysis is enforced with warnings as errors
