using GameStoreAPI.DTOs.Cart;

namespace GameStoreAPI.Services;

public interface ICartService
{
    Task<CartResponseDto> GetCartAsync(int userId);
    Task<CartResponseDto> AddItemAsync(int userId, int gameId);
    Task RemoveItemAsync(int userId, int cartItemId);
}