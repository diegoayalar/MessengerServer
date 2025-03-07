using Firebase.Database;
using MessengerDomain.Entities;
using MessengerService.DTO;

namespace MessengerService.IServices
{
    public interface IAuthService
    {
        Task<(bool Success, string? Message)> RegisterUserAsync(NewUserDTO newUser);
        Task<(bool Success, string? Token)> LoginAsync(LoginUserDTO loginUser);
        void SignOutUser();
        Task<(bool Success, string? Message)> RefreshTokenAsync(string refreshToken);
        Task<(bool Success, string? Message)> SaveRefreshTokenAsync(string email, RefreshToken refreshToken);
        Task<(bool Success, string Message)> DeleteRefreshTokenAsync(string refreshToken);
        Task<(bool IsValid, string? Message)> ValidateTokenAsync(string token);
        Task<(bool Success, string? Message)> DeleteAccountAsync(LoginUserDTO userToDelete);
        Task SendPasswordResetEmailAsync(string email);
    }
}