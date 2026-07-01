namespace GameStoreAPI.Models;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Developer { get; set; }
    public DateTime ReleasedAt { get; set; }
    public bool IsDeleted { get; set; } = false; // soft delete
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int CategoryId { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}