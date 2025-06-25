using CarAz.Core.DTOs.Requests;
using CarAz.Core.DTOs.Responses;

namespace CarAz.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
    Task<AuthResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ResendEmailVerificationAsync(string email);
    Task<bool> LogoutAsync(string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeAllRefreshTokensAsync(Guid userId);
} 