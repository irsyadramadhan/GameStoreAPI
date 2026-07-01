namespace GameStoreAPI.Models;

public class CartItem
{
    public int Id { get; set; }

    // Foreign keys
    public int CartId { get; set; }
    public int GameId { get; set; }

    // Navigation properties
    public Cart Cart { get; set; } = null!;
    public Game Game { get; set; } = null!;
}