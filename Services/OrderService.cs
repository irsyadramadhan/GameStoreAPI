using GameStoreAPI.Data;
using GameStoreAPI.DTOs.Order;
using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResponseDto> CheckoutAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Game)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart kosong, tidak bisa checkout.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                UserId = userId,
                Status = "Completed",
                TotalPrice = cart.Items.Sum(ci => ci.Game.Price)
            };

            order.Items = cart.Items.Select(ci => new OrderItem
            {
                GameId = ci.GameId,
                PriceAtPurchase = ci.Game.Price
            }).ToList();

            _context.Orders.Add(order);

            // Kosongkan cart setelah checkout
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetOrderByIdAsync(userId, order.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Game)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderedAt)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                OrderedAt = o.OrderedAt,
                Items = o.Items.Select(oi => new OrderItemResponseDto
                {
                    GameId = oi.GameId,
                    GameTitle = oi.Game.Title,
                    PriceAtPurchase = oi.PriceAtPurchase
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(int userId, int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Game)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
            ?? throw new KeyNotFoundException("Order tidak ditemukan.");

        return new OrderResponseDto
        {
            Id = order.Id,
            TotalPrice = order.TotalPrice,
            Status = order.Status,
            OrderedAt = order.OrderedAt,
            Items = order.Items.Select(oi => new OrderItemResponseDto
            {
                GameId = oi.GameId,
                GameTitle = oi.Game.Title,
                PriceAtPurchase = oi.PriceAtPurchase
            }).ToList()
        };
    }
}