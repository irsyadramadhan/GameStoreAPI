namespace GameStoreAPI.DTOs.Review;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}