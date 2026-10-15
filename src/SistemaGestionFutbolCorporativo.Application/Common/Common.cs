namespace SistemaGestionFutbolCorporativo.Application.Common;


public readonly record struct PagedList<T>(
    IReadOnlyList<T> Items, 
    int Page, 
    int PageSize, 
    int TotalCount)
{
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
}
