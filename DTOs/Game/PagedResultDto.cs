namespace GameStoreAPI.DTOs.Game;

public class PagedResultDto<T>
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<T> Items { get; set; } = new();
}