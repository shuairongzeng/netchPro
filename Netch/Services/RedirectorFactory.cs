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
