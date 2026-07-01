namespace GameStoreAPI.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}