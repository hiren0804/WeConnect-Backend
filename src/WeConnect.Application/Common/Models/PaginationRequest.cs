namespace WeConnect.Application.Common.Models;

public class PaginationRequest
{
    private const int MaxPageSize = 100;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public int ValidPage =>
        Page < 1 ? 1 : Page;

    public int ValidPageSize =>
        PageSize > MaxPageSize ? MaxPageSize :
        PageSize < 1 ? 10 :
        PageSize;
}