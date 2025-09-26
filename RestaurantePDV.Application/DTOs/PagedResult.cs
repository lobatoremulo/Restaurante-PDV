namespace RestaurantePDV.Application.DTOs;

public class PagedResult<T>
{
    public required IEnumerable<T> Items { get; init; } = Array.Empty<T>();
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;
}
