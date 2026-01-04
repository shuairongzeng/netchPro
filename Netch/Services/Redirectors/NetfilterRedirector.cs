using Netch.Interfaces;
using Netch.Models.Modes;
using Netch.Servers;

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
            return File.Exists(Path.Combine(Global.NetchDir, Constants.NFDriver)) || File.Exists(driverPath);
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
        _initialized = true;
        return Task.FromResult(true);
    }

    public Task<bool> ConfigureAsync(Socks5Server server, Mode mode)
    {
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
