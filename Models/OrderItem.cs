namespace GameStoreAPI.Models;

public class OrderItem
{
    public int Id { get; set; }
    public decimal PriceAtPurchase { get; set; } // harga snapshot saat beli

    // Foreign keys
    public int OrderId { get; set; }
    public int GameId { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Game Game { get; set; } = null!;
}