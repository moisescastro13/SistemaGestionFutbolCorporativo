namespace SistemaGestionFutbolCorporativo.Application.Common;


public sealed class IdempotencyRecord
{
    public string Key { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }
}


public interface IIdempotencyStore
{
    Task<IdempotencyRecord?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> TryCreateAsync(string key, string path, CancellationToken cancellationToken = default);
    Task MarkCompletedAsync(string key, int statusCode, string? responseBody, CancellationToken cancellationToken = default);
}