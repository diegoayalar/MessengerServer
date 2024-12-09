using Firebase.Database;
using MessengerDomain.Entities;

namespace MessengerService.IServices
{
    public interface IUserService
    {
        Task<FirebaseObject<User>> InsertUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<(bool Success, string? ErrorMessage)> UpdateUserFieldAsync(string id, Action<User> updateAction);
        Task<(bool Success, string? ErrorMessage)> UpdateUserProfilePicAsync(string id, Stream profilePicStream);
        Task DeleteUserDataAsync(User user);
    }
}