using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Mediators.Tests.Mocks;

/// <summary>
/// Mock IConfiguration for testing purposes
/// </summary>
public class MockConfiguration(Dictionary<string, string> data) : IConfiguration
{
    private readonly Dictionary<string, string> _data = data;

    public static MockConfiguration Default =>
        new(
            new Dictionary<string, string>
            {
                ["ConnectionStrings:ChatDatabase"] =
                    $"Server=TestServer_{Guid.NewGuid()};Database=ChatDb;Trusted_Connection=True;TrustServerCertificate=True",
                ["ConnectionStrings:UserDatabase"] =
                    $"Server=TestServer_{Guid.NewGuid()};Database=UserDb;Trusted_Connection=True;TrustServerCertificate=True",
                ["UseInMemoryDatabase"] = true.ToString(),
            }
        );

    public string? this[string key]
    {
        get => _data.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value != null)
            {
                _data[key] = value;
            }
            else
            {
                _data.Remove(key);
            }
        }
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return Enumerable.Empty<IConfigurationSection>();
    }

    public IChangeToken GetReloadToken()
    {
        return new MockChangeToken();
    }

    public IConfigurationSection GetSection(string key)
    {
        return new MockConfigurationSection(this, key);
    }

    private class MockChangeToken : IChangeToken
    {
        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => false;

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state) =>
            new EmptyDisposable();
    }

    private class EmptyDisposable : IDisposable
    {
        public void Dispose() { }
    }

    private class MockConfigurationSection : IConfigurationSection
    {
        private readonly IConfiguration _configuration;
        private readonly string _path;

        public MockConfigurationSection(IConfiguration configuration, string path)
        {
            _configuration = configuration;
            _path = path;
        }

        public string? this[string key]
        {
            get => _configuration[$"{_path}:{key}"];
            set => _configuration[$"{_path}:{key}"] = value;
        }

        public string Key => _path.Split(':').Last();
        public string Path => _path;
        public string? Value
        {
            get => _configuration[_path];
            set => _configuration[_path] = value;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return Enumerable.Empty<IConfigurationSection>();
        }

        public IChangeToken GetReloadToken()
        {
            return new MockChangeToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return new MockConfigurationSection(_configuration, $"{_path}:{key}");
        }
    }
}
