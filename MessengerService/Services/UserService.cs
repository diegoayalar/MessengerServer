using Firebase.Database;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerPersistency.Repository;

namespace MessengerService.Services
{
    public class UserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly FirebaseStorageService _firebaseStorageService;

        public UserService(IGenericRepository<User> userRepository, FirebaseStorageService firebaseStorageService)
        {
            _userRepository = userRepository;
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task<FirebaseObject<User>> InsertUserAsync(User user)
        {
            return await _userRepository.InsertAsync(user);
        }

        public async Task UpdateUserAsync(User user) => await _userRepository.UpdateAsync(user);
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();
        public async Task<User?> GetUserByIdAsync(string id) => await _userRepository.GetByIdAsync(id);
        public async Task<User?> GetUserByEmailAsync(string email) => await _userRepository.GetByFieldAsync("Email", email);

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserFieldAsync(string id, Action<User> updateAction)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return (false, "User not found.");
            }

            updateAction(user);
            await UpdateUserAsync(user);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserProfilePicAsync(string id, Stream profilePicStream)
        {
            if (profilePicStream == null || profilePicStream.Length == 0)
            {
                return (false, "Invalid profile picture stream.");
            }

            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return (false, "User not found.");
            }

            string newFileName = Guid.NewGuid().ToString();

            using (profilePicStream)
            {
                await _firebaseStorageService.UploadFileAsync(profilePicStream, newFileName);
            }

            user.Profile.ProfilePic = newFileName;
            await UpdateUserAsync(user);

            return (true, null);
        }

        public async Task DeleteUserDataAsync(User user)
        {
            DeleteUserCredentials(user);

            if (user.Profile != null)
            {
                user.Profile.Description = null;
                user.Profile.ProfilePic = null;
                user.Profile.Status = null;
            }

            await UpdateUserAsync(user);
        }

        private void DeleteUserCredentials(User user)
        {
            user.Email = null;
            user.Password = null;
            user.Phone = null;
        }
    }
}
