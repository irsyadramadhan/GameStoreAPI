using GameStoreAPI.DTOs.Review;
using GameStoreAPI.Helpers;
using GameStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreAPI.Controllers;

[ApiController]
[Route("api/games/{gameId}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByGame(int gameId)
    {
        var result = await _reviewService.GetByGameIdAsync(gameId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(int gameId, ReviewRequestDto request)
    {
        try
        {
            var result = await _reviewService.CreateAsync(User.GetUserId(), gameId, request);
            return CreatedAtAction(nameof(GetByGame), new { gameId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}