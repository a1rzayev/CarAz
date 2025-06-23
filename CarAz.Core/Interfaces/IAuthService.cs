using CarAz.Core.DTOs;

namespace CarAz.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
    Task<AuthResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ResendEmailVerificationAsync(string email);
    Task<bool> LogoutAsync(string refreshToken);
} 