using MessengerService.DTO;

namespace MessengerService.IServices
{
    public interface IAuthService
    {
        Task<(bool Success, string? Token)> RegisterUserAsync(NewUserDTO newUser);
        Task<(bool Success, string? Token)> LoginAsync(LoginUserDTO loginUser);
        void SignOutUser();
        Task<(bool IsValid, string UserId)> ValidateTokenAsync(string token);
        Task<(bool Success, string? Message)> DeleteAccountAsync(LoginUserDTO userToDelete);
        Task SendPasswordResetEmailAsync(string email);
    }
}