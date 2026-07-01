using GameStoreAPI.DTOs.Game;

namespace GameStoreAPI.Services;

public interface IGameService
{
    Task<PagedResultDto<GameResponseDto>> GetAllAsync(GameQueryDto query);
    Task<GameResponseDto> GetByIdAsync(int id);
    Task<GameResponseDto> CreateAsync(GameRequestDto request);
    Task<GameResponseDto> UpdateAsync(int id, GameRequestDto request);
    Task DeleteAsync(int id);
}