using GameStoreAPI.DTOs.Category;

namespace GameStoreAPI.Services;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request);
}