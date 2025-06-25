using CarAz.Core.Entities;

namespace CarAz.Core.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string refreshToken);
    Task<RefreshToken> CreateRefreshTokenAsync(User user);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token, string reason = "Used");
    Task RevokeAllRefreshTokensForUserAsync(Guid userId, string reason = "Security");
} 