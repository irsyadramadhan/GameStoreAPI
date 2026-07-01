namespace GameStoreAPI.DTOs.Cart;

public class CartItemResponseDto
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
}