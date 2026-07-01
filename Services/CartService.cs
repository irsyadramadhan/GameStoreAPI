using GameStoreAPI.Data;
using GameStoreAPI.DTOs.Cart;
using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CartResponseDto> GetCartAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return await MapToDto(cart.Id);
    }

    public async Task<CartResponseDto> AddItemAsync(int userId, int gameId)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.Id == gameId);
        if (!gameExists)
            throw new KeyNotFoundException("Game tidak ditemukan.");

        var cart = await GetOrCreateCartAsync(userId);

        bool alreadyInCart = await _context.CartItems
            .AnyAsync(ci => ci.CartId == cart.Id && ci.GameId == gameId);

        if (alreadyInCart)
            throw new InvalidOperationException("Game sudah ada di cart.");

        _context.CartItems.Add(new CartItem { CartId = cart.Id, GameId = gameId });
        await _context.SaveChangesAsync();

        return await MapToDto(cart.Id);
    }

    public async Task RemoveItemAsync(int userId, int cartItemId)
    {
        var cart = await GetOrCreateCartAsync(userId);

        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cart.Id)
            ?? throw new KeyNotFoundException("Item tidak ditemukan di cart kamu.");

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    private async Task<CartResponseDto> MapToDto(int cartId)
    {
        var items = await _context.CartItems
            .Include(ci => ci.Game)
            .Where(ci => ci.CartId == cartId)
            .Select(ci => new CartItemResponseDto
            {
                Id = ci.Id,
                GameId = ci.GameId,
                GameTitle = ci.Game.Title,
                Price = ci.Game.Price
            })
            .ToListAsync();

        return new CartResponseDto
        {
            Id = cartId,
            Items = items,
            TotalPrice = items.Sum(i => i.Price)
        };
    }
}