using TodoList.Domain.Entities;

namespace TodoList.Application.Common.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokensAsync(User user);
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken, string reason);
    Task RevokeAllUserTokensAsync(string userId);
}

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}