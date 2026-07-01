using GameStoreAPI.DTOs.Cart;
using GameStoreAPI.Helpers;
using GameStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreAPI.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cartService.GetCartAsync(User.GetUserId());
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddCartItemRequestDto request)
    {
        try
        {
            var result = await _cartService.AddItemAsync(User.GetUserId(), request.GameId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        try
        {
            await _cartService.RemoveItemAsync(User.GetUserId(), id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}