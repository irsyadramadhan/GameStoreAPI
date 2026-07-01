using GameStoreAPI.DTOs.Review;

namespace GameStoreAPI.Services;

public interface IReviewService
{
    Task<List<ReviewResponseDto>> GetByGameIdAsync(int gameId);
    Task<ReviewResponseDto> CreateAsync(int userId, int gameId, ReviewRequestDto request);
}