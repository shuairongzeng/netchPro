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
