using BCrypt.Net;
using CarAz.Core.DTOs.Requests;
using CarAz.Core.DTOs.Responses;
using CarAz.Core.Entities;
using CarAz.Core.Interfaces;
using CarAz.Core.Settings;
using CarAz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CarAz.Core.DTOs;

namespace CarAz.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ApplicationDbContext context, IJwtService jwtService, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = await _jwtService.CreateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenExpiresAt = refreshToken.ExpiryDate,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User with this email already exists"
            };
        }

        // Create new user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = await _jwtService.CreateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenExpiresAt = refreshToken.ExpiryDate,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            // Validate the expired access token to get user info
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, // We want to accept expired tokens for refresh
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(request.AccessToken, tokenValidationParameters, out var validatedToken);

            // Get user ID from the token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid access token"
                };
            }

            // Get the refresh token from database
            var refreshToken = await _jwtService.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.UserId != userId)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid refresh token"
                };
            }

            // Get user
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "User not found or inactive"
                };
            }

            // Revoke the old refresh token
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, "Refreshed");

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = await _jwtService.CreateRefreshTokenAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                RefreshTokenExpiresAt = newRefreshToken.ExpiryDate,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role
                }
            };
        }
        catch (Exception)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid token"
            };
        }
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            await _jwtService.RevokeRefreshTokenAsync(refreshToken, "Revoked by user");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RevokeAllRefreshTokensAsync(Guid userId)
    {
        try
        {
            await _jwtService.RevokeAllRefreshTokensForUserAsync(userId, "Security measure");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null)
            return false;

        // Generate password reset token
        var resetToken = Guid.NewGuid().ToString();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(24);

        await _context.SaveChangesAsync();

        // TODO: Send email with reset token
        // For now, just return true
        return true;
    }

    public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token && 
                                     u.PasswordResetTokenExpiry > DateTime.UtcNow);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid or expired reset token"
            };
        }

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await _context.SaveChangesAsync();

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = await _jwtService.CreateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Password reset successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenExpiresAt = refreshToken.ExpiryDate,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }

    public async Task<bool> ResendEmailVerificationAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null || user.IsEmailVerified)
            return false;

        // Generate verification token
        var verificationToken = Guid.NewGuid().ToString();
        user.EmailVerificationToken = verificationToken;
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

        await _context.SaveChangesAsync();

        // TODO: Send verification email
        return true;
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        return await RevokeRefreshTokenAsync(refreshToken);
    }

    public async Task<AuthResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User not found or inactive"
            };
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Current password is incorrect"
            };
        }

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        // Revoke all refresh tokens for security
        await _jwtService.RevokeAllRefreshTokensForUserAsync(userId, "Password changed");

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = await _jwtService.CreateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Password changed successfully",
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenExpiresAt = refreshToken.ExpiryDate,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token && 
                                     u.EmailVerificationTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return false;

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;

        await _context.SaveChangesAsync();
        return true;
    }
} 