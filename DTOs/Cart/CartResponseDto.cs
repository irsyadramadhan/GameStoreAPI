namespace GameStoreAPI.DTOs.Cart;

public class CartResponseDto
{
    public int Id { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}