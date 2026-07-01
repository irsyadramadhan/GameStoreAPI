using GameStoreAPI.Data;
using GameStoreAPI.DTOs.Auth;
using GameStoreAPI.Helpers;
using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDbContext context, JwtTokenGenerator tokenGenerator, JwtSettings jwtSettings)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        bool emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
            throw new InvalidOperationException("Email sudah terdaftar.");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Buat cart otomatis untuk user baru
        var cart = new Cart { UserId = user.Id };
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email atau password salah.");

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token tidak valid atau sudah expired.");

        return await GenerateAuthResponse(user);
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(User user)
    {
        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}