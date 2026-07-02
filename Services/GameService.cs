using GameStoreAPI.Data;
using GameStoreAPI.DTOs.Game;
using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Services;

public class GameService : IGameService
{
    private readonly AppDbContext _context;

    public GameService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<GameResponseDto>> GetAllAsync(GameQueryDto query)
    {
        var q = _context.Games.Include(g => g.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(g => g.Title.ToLower().Contains(query.Search.ToLower()));

        if (query.CategoryId.HasValue)
            q = q.Where(g => g.CategoryId == query.CategoryId);

        if (query.MinPrice.HasValue)
            q = q.Where(g => g.Price >= query.MinPrice);

        if (query.MaxPrice.HasValue)
            q = q.Where(g => g.Price <= query.MaxPrice);

        var totalItems = await q.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(g => new GameResponseDto
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Price = g.Price,
                ImageUrl = g.ImageUrl,
                Developer = g.Developer,
                ReleasedAt = g.ReleasedAt,
                CategoryName = g.Category.Name
            })
            .ToListAsync();

        return new PagedResultDto<GameResponseDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = query.Page,
            Items = items
        };
    }

    public async Task<GameResponseDto> GetByIdAsync(int id)
    {
        var game = await _context.Games
            .Include(g => g.Category)
            .FirstOrDefaultAsync(g => g.Id == id)
            ?? throw new KeyNotFoundException("Game not found.");

        return new GameResponseDto
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Price = game.Price,
            ImageUrl = game.ImageUrl,
            Developer = game.Developer,
            ReleasedAt = game.ReleasedAt,
            CategoryName = game.Category.Name
        };
    }

    public async Task<GameResponseDto> CreateAsync(GameRequestDto request)
    {
        bool categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new KeyNotFoundException("Category not found.");

        var game = new Game
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            Developer = request.Developer,
            ReleasedAt = request.ReleasedAt,
            CategoryId = request.CategoryId
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(game.Id);
    }

    public async Task<GameResponseDto> UpdateAsync(int id, GameRequestDto request)
    {
        var game = await _context.Games.FindAsync(id)
            ?? throw new KeyNotFoundException("Game not found.");

        bool categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new KeyNotFoundException("Category not found.");

        game.Title = request.Title;
        game.Description = request.Description;
        game.Price = request.Price;
        game.ImageUrl = request.ImageUrl;
        game.Developer = request.Developer;
        game.ReleasedAt = request.ReleasedAt;
        game.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(game.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var game = await _context.Games.FindAsync(id)
            ?? throw new KeyNotFoundException("Game not found.");

        game.IsDeleted = true; // soft delete
        await _context.SaveChangesAsync();
    }
}