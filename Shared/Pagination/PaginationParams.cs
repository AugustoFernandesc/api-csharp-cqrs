namespace Shared.Pagination;

public class PaginationParams
{

    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? OrderBy { get; set; }
    public string? OrderByDirection { get; set; } = "asc";
    public List<PagedFilterRequest> Filters { get; set; } = new();



}
