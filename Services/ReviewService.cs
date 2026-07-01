using GameStoreAPI.Data;
using GameStoreAPI.DTOs.Review;
using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;

    public ReviewService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReviewResponseDto>> GetByGameIdAsync(int gameId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.GameId == gameId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                Username = r.User.Username,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ReviewResponseDto> CreateAsync(int userId, int gameId, ReviewRequestDto request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating harus antara 1 sampai 5.");

        var gameExists = await _context.Games.AnyAsync(g => g.Id == gameId);
        if (!gameExists)
            throw new KeyNotFoundException("Game tidak ditemukan.");

        // Business rule: harus pernah beli game ini
        bool hasPurchased = await _context.OrderItems
            .Include(oi => oi.Order)
            .AnyAsync(oi => oi.GameId == gameId && oi.Order.UserId == userId);

        if (!hasPurchased)
            throw new InvalidOperationException("Kamu harus membeli game ini dulu sebelum bisa review.");

        bool alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.GameId == gameId && r.UserId == userId);

        if (alreadyReviewed)
            throw new InvalidOperationException("Kamu sudah pernah review game ini.");

        var review = new Review
        {
            UserId = userId,
            GameId = gameId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);

        return new ReviewResponseDto
        {
            Id = review.Id,
            Username = user!.Username,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}