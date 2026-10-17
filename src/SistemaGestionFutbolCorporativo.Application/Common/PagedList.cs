using System.Text.Json.Serialization;

namespace SistemaGestionFutbolCorporativo.Application.Common;

public sealed record PagedResponse<T>
{
    [JsonPropertyName("data")]
    public IReadOnlyList<T> Data { get; init; }

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("totalRecords")]
    public int TotalRecords { get; init; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalRecords / PageSize) : 0;

    public PagedResponse(IReadOnlyList<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
    }

    public static PagedResponse<T> Create(IReadOnlyList<T> data, int pageNumber, int pageSize, int totalRecords) =>
        new(data, pageNumber, pageSize, totalRecords);
}

public record PaginationParams
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;

    private int _pageNumber = 1;
    private int _pageSize = DefaultPageSize;
    private string _sortDirection = "asc";

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value < 1 ? DefaultPageSize : (value > MaxPageSize ? MaxPageSize : value);
    }

    public string? SortBy { get; init; }

    public string SortDirection
    {
        get => _sortDirection;
        init => _sortDirection = string.Equals(value, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
    }

    public int Offset => (PageNumber - 1) * PageSize;
}
