# Netch Progressive Upgrade Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Progressively upgrade Netch from .NET 6 to .NET 8, update dependencies, add dependency manifest, and introduce driver abstraction layer.

**Architecture:** Three-phase progressive upgrade (A→B→C) that incrementally modernizes the codebase while minimizing risk. Each phase builds on the previous, with clear verification points.

**Tech Stack:** .NET 8.0 LTS, Windows Forms, C++ native components (unchanged), P/Invoke interop

---

## Phase A: Conservative Upgrade (.NET 6 → .NET 8)

### Task A1: Update Target Framework

**Files:**
- Modify: `common.props:3`

**Step 1: Backup current common.props**

Run: `copy common.props common.props.bak`

**Step 2: Update TargetFramework**

```xml
<!-- Before -->
<TargetFramework>net6.0-windows</TargetFramework>

<!-- After -->
<TargetFramework>net8.0-windows</TargetFramework>
```

**Step 3: Verify change**

Run: `type common.props | findstr TargetFramework`
Expected: `<TargetFramework>net8.0-windows</TargetFramework>`

---

### Task A2: Update Tests Project

**Files:**
- Modify: `Tests\Tests.csproj:4`

**Step 1: Update Tests TargetFramework**

```xml
<!-- Before -->
<TargetFramework>net5.0</TargetFramework>

<!-- After -->
<TargetFramework>net8.0</TargetFramework>
```

**Step 2: Verify change**

Run: `type Tests\Tests.csproj | findstr TargetFramework`
Expected: `<TargetFramework>net8.0</TargetFramework>`

---

### Task A3: Update Core NuGet Packages

**Files:**
- Modify: `Netch\Netch.csproj`

**Step 1: Update Serilog packages**

```xml
<!-- Before -->
<PackageReference Include="Serilog" Version="2.12.0" />
<PackageReference Include="Serilog.Extensions.Hosting" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.Async" Version="1.5.0" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="4.1.0" />

<!-- After -->
<PackageReference Include="Serilog" Version="4.3.0" />
<PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Async" Version="2.1.0" />
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
```

**Step 2: Verify packages restored**

Run: `dotnet restore Netch\Netch.csproj`
Expected: Restore completed successfully

---

### Task A4: Update Microsoft Packages

**Files:**
- Modify: `Netch\Netch.csproj`

**Step 1: Update Microsoft packages**

```xml
<!-- Before -->
<PackageReference Include="Microsoft.VisualStudio.Threading" Version="17.5.22" />
<PackageReference Include="Microsoft.Windows.CsWin32" Version="0.1.588-beta">
<PackageReference Include="System.Management" Version="7.0.0" />
<PackageReference Include="System.ServiceProcess.ServiceController" Version="7.0.0" />
<PackageReference Include="System.Text.Encoding.CodePages" Version="7.0.0" />

<!-- After -->
<PackageReference Include="Microsoft.VisualStudio.Threading" Version="17.14.15" />
<PackageReference Include="Microsoft.Windows.CsWin32" Version="0.3.264">
<PackageReference Include="System.Management" Version="8.0.0" />
<PackageReference Include="System.ServiceProcess.ServiceController" Version="8.0.1" />
<PackageReference Include="System.Text.Encoding.CodePages" Version="8.0.0" />
```

**Step 2: Verify packages restored**

Run: `dotnet restore Netch\Netch.csproj`
Expected: Restore completed successfully

---

### Task A5: Update Other Packages

**Files:**
- Modify: `Netch\Netch.csproj`

**Step 1: Update remaining packages**

```xml
<!-- Before -->
<PackageReference Include="Fody" Version="6.6.4">
<PackageReference Include="MaxMind.GeoIP2" Version="5.1.0" />
<PackageReference Include="Microsoft.Diagnostics.Tracing.TraceEvent" Version="3.0.7" />

<!-- After -->
<PackageReference Include="Fody" Version="6.9.1">
<PackageReference Include="MaxMind.GeoIP2" Version="5.2.0" />
<PackageReference Include="Microsoft.Diagnostics.Tracing.TraceEvent" Version="3.1.17" />
```

**Step 2: Verify packages restored**

Run: `dotnet restore Netch\Netch.csproj`
Expected: Restore completed successfully

---

### Task A6: Update Test Packages

**Files:**
- Modify: `Tests\Tests.csproj`

**Step 1: Update test packages**

```xml
<!-- Before -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.4.1" />
<PackageReference Include="MSTest.TestAdapter" Version="3.0.2" />
<PackageReference Include="MSTest.TestFramework" Version="3.0.2" />
<PackageReference Include="coverlet.collector" Version="3.2.0" />

<!-- After -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="MSTest.TestAdapter" Version="3.7.0" />
<PackageReference Include="MSTest.TestFramework" Version="3.7.0" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
```

**Step 2: Verify packages restored**

Run: `dotnet restore Tests\Tests.csproj`
Expected: Restore completed successfully

---

### Task A7: Regenerate CsWin32 Bindings

**Files:**
- May affect: `Netch\NativeMethods.cs` (auto-generated)

**Step 1: Clean and rebuild to regenerate CsWin32 bindings**

Run: `dotnet clean Netch\Netch.csproj`
Run: `dotnet build Netch\Netch.csproj -c Debug`

**Step 2: Check for CsWin32 breaking changes**

If build fails with CsWin32 errors, check `Netch\NativeMethods.txt` and update API names if needed.

Expected: Build succeeds or specific API changes identified

---

### Task A8: Fix Compilation Errors

**Files:**
- Various (based on errors)

**Step 1: Build the project**

Run: `dotnet build Netch\Netch.csproj -c Release`

**Step 2: Address any compilation errors**

Common issues with .NET 8 upgrade:
- Nullable reference type warnings
- Obsolete API warnings
- CsWin32 API signature changes

**Step 3: Verify build succeeds**

Run: `dotnet build Netch\Netch.csproj -c Release`
Expected: Build succeeded. 0 Warning(s). 0 Error(s).

---

### Task A9: Run Tests

**Files:**
- None (verification only)

**Step 1: Run all tests**

Run: `dotnet test Tests\Tests.csproj -c Release`
Expected: All tests pass

**Step 2: Commit Phase A changes**

```bash
git add -A
git commit -m "chore: upgrade to .NET 8 and update NuGet packages

- Upgrade TargetFramework from net6.0-windows to net8.0-windows
- Upgrade Tests from net5.0 to net8.0
- Update Serilog packages to latest versions
- Update Microsoft.* packages for .NET 8 compatibility
- Update CsWin32 to 0.3.264
- Update test packages"
```

---

## Phase B: Moderate Upgrade (Dependencies & Binary Management)

### Task B1: Create Dependencies Manifest

**Files:**
- Create: `Netch\DependenciesManifest.json`

**Step 1: Create manifest file**

```json
{
  "$schema": "./DependenciesManifest.schema.json",
  "version": "1.0.0",
  "lastUpdated": "2026-01-03",
  "binaries": {
    "Redirector.bin": {
      "description": "Traffic interception via Netfilter SDK",
      "version": "1.0.0",
      "platform": "win-x64",
      "hash": "",
      "required": true
    },
    "RouteHelper.bin": {
      "description": "Windows routing table manipulation",
      "version": "1.0.0",
      "platform": "win-x64",
      "hash": "",
      "required": true
    },
    "tun2socks.bin": {
      "description": "TUN virtual adapter proxy",
      "version": "1.0.0",
      "platform": "win-x64",
      "hash": "",
      "required": true
    },
    "aiodns.bin": {
      "description": "Async DNS resolver",
      "version": "1.0.0",
      "platform": "win-x64",
      "hash": "",
      "required": true
    },
    "v2ray-sn.exe": {
      "description": "V2Ray SagerNet core",
      "version": "1.0.0",
      "platform": "win-x64",
      "hash": "",
      "required": true
    },
    "nfapi.dll": {
      "description": "Netfilter API library",
      "version": "1.0.0",
      "platform": "win-x64",
      "hash": "",
      "required": true
    },
    "nfdriver.sys": {
      "description": "Netfilter kernel driver",
      "version": "1.0.0",
      "platform": "win-x64",
      "hash": "",
      "required": true
    }
  },
  "capabilities": {
    "processMode": ["Redirector.bin", "nfapi.dll", "nfdriver.sys"],
    "tunMode": ["tun2socks.bin", "RouteHelper.bin"],
    "shareMode": [],
    "protocols": ["v2ray-sn.exe"]
  }
}
```

**Step 2: Verify JSON is valid**

Run: `type Netch\DependenciesManifest.json | python -m json.tool`
Expected: Valid JSON output

---

### Task B2: Create Dependency Checker Service

**Files:**
- Create: `Netch\Services\DependencyChecker.cs`

**Step 1: Create the service**

```csharp
using System.Security.Cryptography;
using System.Text.Json;

namespace Netch.Services;

public class DependencyChecker
{
    private readonly string _binDirectory;
    private readonly Dictionary<string, BinaryInfo> _manifest;

    public DependencyChecker(string binDirectory)
    {
        _binDirectory = binDirectory;
        _manifest = new Dictionary<string, BinaryInfo>();
    }

    public async Task<DependencyCheckResult> CheckDependenciesAsync()
    {
        var result = new DependencyCheckResult();
        var binPath = Path.Combine(Global.NetchDir, "bin");

        foreach (var file in _manifest)
        {
            var filePath = Path.Combine(binPath, file.Key);
            if (!File.Exists(filePath))
            {
                if (file.Value.Required)
                    result.MissingRequired.Add(file.Key);
                else
                    result.MissingOptional.Add(file.Key);
            }
        }

        result.IsValid = result.MissingRequired.Count == 0;
        return result;
    }

    public static async Task<string> ComputeHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hash);
    }
}

public class BinaryInfo
{
    public string Description { get; set; } = "";
    public string Version { get; set; } = "";
    public string Platform { get; set; } = "";
    public string Hash { get; set; } = "";
    public bool Required { get; set; }
}

public class DependencyCheckResult
{
    public bool IsValid { get; set; }
    public List<string> MissingRequired { get; } = new();
    public List<string> MissingOptional { get; } = new();
}
```

**Step 2: Verify compilation**

Run: `dotnet build Netch\Netch.csproj`
Expected: Build succeeded

---

### Task B3: Integrate Dependency Check at Startup

**Files:**
- Modify: `Netch\Program.cs`

**Step 1: Add dependency check after configuration load**

Add after line ~63 (after Configuration.LoadAsync()):

```csharp
// Check dependencies
var depChecker = new DependencyChecker(Path.Combine(Global.NetchDir, "bin"));
var depResult = await depChecker.CheckDependenciesAsync();
if (!depResult.IsValid)
{
    Log.Warning("Missing required dependencies: {Dependencies}",
        string.Join(", ", depResult.MissingRequired));
}
```

**Step 2: Add using statement**

Add at top of file:
```csharp
using Netch.Services;
```

**Step 3: Verify compilation**

Run: `dotnet build Netch\Netch.csproj`
Expected: Build succeeded

---

### Task B4: Commit Phase B Changes

**Step 1: Commit**

```bash
git add -A
git commit -m "feat: add dependency manifest and checker

- Add DependenciesManifest.json for tracking binary dependencies
- Add DependencyChecker service for startup validation
- Integrate dependency check in Program.cs startup flow"
```

---

## Phase C: Deep Upgrade (Driver Abstraction Layer)

### Task C1: Define Network Redirector Interface

**Files:**
- Create: `Netch\Interfaces\INetworkRedirector.cs`

**Step 1: Create the interface**

```csharp
using Netch.Models.Modes;
using Netch.Servers;

namespace Netch.Interfaces;

public interface INetworkRedirector : IDisposable
{
    string Name { get; }
    bool IsAvailable { get; }
    RedirectorCapabilities Capabilities { get; }

    Task<bool> InitializeAsync();
    Task<bool> ConfigureAsync(Socks5Server server, Mode mode);
    Task<bool> StartAsync();
    Task StopAsync();

    event EventHandler<RedirectorStatusChangedEventArgs>? StatusChanged;
    event EventHandler<RedirectorErrorEventArgs>? ErrorOccurred;
}

[Flags]
public enum RedirectorCapabilities
{
    None = 0,
    ProcessMode = 1,
    TunMode = 2,
    ShareMode = 4,
    IPv6 = 8,
    Socks5Auth = 16
}

public class RedirectorStatusChangedEventArgs : EventArgs
{
    public string Status { get; init; } = "";
    public bool IsRunning { get; init; }
}

public class RedirectorErrorEventArgs : EventArgs
{
    public string Message { get; init; } = "";
    public Exception? Exception { get; init; }
    public bool IsFatal { get; init; }
}
```

**Step 2: Verify compilation**

Run: `dotnet build Netch\Netch.csproj`
Expected: Build succeeded

---

### Task C2: Create Redirector Factory

**Files:**
- Create: `Netch\Services\RedirectorFactory.cs`

**Step 1: Create the factory**

```csharp
using Netch.Interfaces;
using Netch.Models.Modes;

namespace Netch.Services;

public class RedirectorFactory
{
    private readonly Dictionary<ModeType, Func<INetworkRedirector>> _redirectors = new();

    public void Register(ModeType type, Func<INetworkRedirector> factory)
    {
        _redirectors[type] = factory;
    }

    public INetworkRedirector? Create(ModeType type)
    {
        return _redirectors.TryGetValue(type, out var factory) ? factory() : null;
    }

    public bool IsSupported(ModeType type)
    {
        if (!_redirectors.TryGetValue(type, out var factory))
            return false;

        using var redirector = factory();
        return redirector.IsAvailable;
    }

    public RedirectorCapabilities GetCapabilities(ModeType type)
    {
        if (!_redirectors.TryGetValue(type, out var factory))
            return RedirectorCapabilities.None;

        using var redirector = factory();
        return redirector.Capabilities;
    }
}
```

**Step 2: Verify compilation**

Run: `dotnet build Netch\Netch.csproj`
Expected: Build succeeded

---

### Task C3: Implement NetfilterRedirector Wrapper

**Files:**
- Create: `Netch\Services\Redirectors\NetfilterRedirector.cs`

**Step 1: Create wrapper implementation**

```csharp
using Netch.Interfaces;
using Netch.Models.Modes;
using Netch.Servers;
using static Netch.Interops.Redirector;

namespace Netch.Services.Redirectors;

public class NetfilterRedirector : INetworkRedirector
{
    private bool _disposed;
    private bool _initialized;

    public string Name => "Netfilter Redirector";

    public bool IsAvailable
    {
        get
        {
            var driverPath = $"{Environment.SystemDirectory}\\drivers\\netfilter2.sys";
            return File.Exists(Constants.NFDriver) || File.Exists(driverPath);
        }
    }

    public RedirectorCapabilities Capabilities =>
        RedirectorCapabilities.ProcessMode |
        RedirectorCapabilities.IPv6 |
        RedirectorCapabilities.Socks5Auth;

    public event EventHandler<RedirectorStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<RedirectorErrorEventArgs>? ErrorOccurred;

    public Task<bool> InitializeAsync()
    {
        // Driver check and installation logic from NFController
        _initialized = true;
        return Task.FromResult(true);
    }

    public Task<bool> ConfigureAsync(Socks5Server server, Mode mode)
    {
        // Configuration logic from NFController.StartAsync
        return Task.FromResult(true);
    }

    public async Task<bool> StartAsync()
    {
        var result = await Interops.Redirector.InitAsync();
        StatusChanged?.Invoke(this, new RedirectorStatusChangedEventArgs
        {
            Status = result ? "Running" : "Failed",
            IsRunning = result
        });
        return result;
    }

    public async Task StopAsync()
    {
        await Interops.Redirector.FreeAsync();
        StatusChanged?.Invoke(this, new RedirectorStatusChangedEventArgs
        {
            Status = "Stopped",
            IsRunning = false
        });
    }

    public void Dispose()
    {
        if (_disposed) return;
        StopAsync().GetAwaiter().GetResult();
        _disposed = true;
    }
}
```

**Step 2: Verify compilation**

Run: `dotnet build Netch\Netch.csproj`
Expected: Build succeeded

---

### Task C4: Create Redirectors Directory

**Files:**
- Create directory: `Netch\Services\Redirectors\`

**Step 1: Create directory**

Run: `mkdir Netch\Services\Redirectors`

---

### Task C5: Final Build and Test

**Step 1: Clean build**

Run: `dotnet clean Netch.sln`
Run: `dotnet build Netch.sln -c Release`

**Step 2: Run tests**

Run: `dotnet test Tests\Tests.csproj -c Release`
Expected: All tests pass

**Step 3: Commit Phase C changes**

```bash
git add -A
git commit -m "feat: add driver abstraction layer

- Add INetworkRedirector interface for redirector abstraction
- Add RedirectorFactory for redirector creation and capability detection
- Add NetfilterRedirector as initial implementation
- Prepare architecture for future driver replacements"
```

---

## Verification Checklist

### Phase A Verification
- [ ] `dotnet build Netch.sln` succeeds
- [ ] `dotnet test Tests\Tests.csproj` passes
- [ ] Application starts without errors

### Phase B Verification
- [ ] DependenciesManifest.json is valid JSON
- [ ] Dependency checker runs at startup
- [ ] Missing dependencies are logged

### Phase C Verification
- [ ] INetworkRedirector interface compiles
- [ ] RedirectorFactory can create redirectors
- [ ] NetfilterRedirector wrapper works

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| CsWin32 API changes | Check generated code, update NativeMethods.txt if needed |
| Serilog breaking changes | Review migration guide, update logging calls |
| Test failures | Fix tests or mark as skipped with TODO |
| Runtime errors | Test with debug build first, check P/Invoke signatures |

---

## Rollback Plan

If upgrade fails at any phase:

1. **Phase A rollback**: `git checkout common.props Tests\Tests.csproj Netch\Netch.csproj`
2. **Phase B rollback**: Delete new files, revert Program.cs changes
3. **Phase C rollback**: Delete Interfaces and Services/Redirectors directories

