namespace DalleTelegramBot.Common.Pagination;

internal class PaginatedList<T> : List<T>
{
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex < TotalPages - 1;

    private PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        AddRange(source);
    }

    public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var totalCount = source.Count();
        var items = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, pageIndex, pageSize, totalCount);
    }
}
