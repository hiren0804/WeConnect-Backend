namespace WeConnect.Application.Common.Models;

public sealed class PagedList<T>
{
    public IReadOnlyList<T> Items       { get; }
    public int Page         { get; }
    public int PageSize     { get; }
    public int TotalCount   { get; }
    public int TotalPages   => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNext     => Page < TotalPages;
    public bool HasPrevious => Page > 1;

    public PagedList(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items; Page = page; PageSize = pageSize; TotalCount = totalCount;
    }

    public PaginationMeta ToMeta() =>
        new(Page, PageSize, TotalCount, TotalPages, HasNext, HasPrevious);
}