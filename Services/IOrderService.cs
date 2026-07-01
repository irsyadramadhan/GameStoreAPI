using GameStoreAPI.DTOs.Order;

namespace GameStoreAPI.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CheckoutAsync(int userId);
    Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId);
    Task<OrderResponseDto> GetOrderByIdAsync(int userId, int orderId);
}