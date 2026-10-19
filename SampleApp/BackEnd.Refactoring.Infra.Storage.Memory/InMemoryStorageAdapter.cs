namespace BackEnd.Refactoring.Infra.Storage.Memory;

/// <summary>
/// In-memory implementation of IReportStorage for use in unit tests.
///
/// Because ReportService depends on IReportStorage (not on Azure SDK types),
/// tests can use this fake without any Azure infrastructure, SDK references,
/// or network access. This is the payoff of the ACL pattern.
/// </summary>
public sealed class InMemoryStorageAdapter : IReportStorage
{
    private readonly Dictionary<string, byte[]> _store = new();

    public Task UploadAsync(string path, Stream content, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        content.CopyTo(ms);
        _store[path] = ms.ToArray();
        return Task.CompletedTask;
    }

    public Task<Stream> DownloadAsync(string path, CancellationToken ct = default)
    {
        if (!_store.TryGetValue(path, out var data))
            throw new FileNotFoundException($"File not found at '{path}'.");

        Stream stream = new MemoryStream(data);
        return Task.FromResult(stream);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken ct = default)
        => Task.FromResult(_store.ContainsKey(path));

    public Task DeleteAsync(string path, CancellationToken ct = default)
    {
        _store.Remove(path);
        return Task.CompletedTask;
    }
}
