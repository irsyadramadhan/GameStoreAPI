namespace GameStoreAPI.DTOs.Game;

public class GameResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Developer { get; set; }
    public DateTime ReleasedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}