using System.Text.Json;

namespace TasApp.Infrastructure.Persistence;

internal sealed class JsonFileStore<T>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _filePath;

    public JsonFileStore(string filePath) => _filePath = filePath;

    public async Task<T?> LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_filePath))
            return default;

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<T>(stream, Options, ct);
    }

    public async Task SaveAsync(T value, CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, value, Options, ct);
    }
}