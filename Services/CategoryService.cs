using GameStoreAPI.Data;
using GameStoreAPI.DTOs.Category;
using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}