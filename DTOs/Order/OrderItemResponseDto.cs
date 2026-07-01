namespace GameStoreAPI.DTOs.Order;

public class OrderItemResponseDto
{
    public int GameId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public decimal PriceAtPurchase { get; set; }
}