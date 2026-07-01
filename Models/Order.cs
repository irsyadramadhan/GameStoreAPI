namespace GameStoreAPI.Models;

public class Order
{
    public int Id { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Completed";
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}