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
        LoadManifest();
    }

    private void LoadManifest()
    {
        var manifestPath = Path.Combine(Global.NetchDir, "DependenciesManifest.json");
        if (!File.Exists(manifestPath))
            return;

        try
        {
            var json = File.ReadAllText(manifestPath);
            var doc = JsonDocument.Parse(json);
            var binaries = doc.RootElement.GetProperty("binaries");

            foreach (var prop in binaries.EnumerateObject())
            {
                _manifest[prop.Name] = new BinaryInfo
                {
                    Description = prop.Value.GetProperty("description").GetString() ?? "",
                    Version = prop.Value.GetProperty("version").GetString() ?? "",
                    Platform = prop.Value.GetProperty("platform").GetString() ?? "",
                    Hash = prop.Value.GetProperty("hash").GetString() ?? "",
                    Required = prop.Value.GetProperty("required").GetBoolean()
                };
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to load dependencies manifest");
        }
    }

    public Task<DependencyCheckResult> CheckDependenciesAsync()
    {
        var result = new DependencyCheckResult();

        foreach (var file in _manifest)
        {
            var filePath = Path.Combine(_binDirectory, file.Key);
            if (!File.Exists(filePath))
            {
                if (file.Value.Required)
                    result.MissingRequired.Add(file.Key);
                else
                    result.MissingOptional.Add(file.Key);
            }
        }

        result.IsValid = result.MissingRequired.Count == 0;
        return Task.FromResult(result);
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
