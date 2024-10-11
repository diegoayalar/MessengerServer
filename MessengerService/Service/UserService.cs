using Firebase.Database;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;

namespace MessengerService.Service
{
    public class UserService
    {
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<FirebaseObject<User>> InsertUserAsync(User user)
        {
            return await _userRepository.InsertAsync(user);
        }

        public async Task UpdateUserAsync(User user) => await _userRepository.UpdateAsync(user);
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();
        public async Task<User> GetUserByIdAsync(string id) => await _userRepository.GetByIdAsync(id);
        public async Task<User> GetUserByEmailAsync(string email) => await _userRepository.GetByFieldAsync("Email", email);
        public async Task UpdateUserIsActiveAsync(string id, bool isActive)
        {
            var user = await GetUserByIdAsync(id) ?? throw new Exception("User not found.");
            user.IsActive = isActive;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserDataAsync(User user)
        {
            DeleteCredentials(user);

            if (user.Profile != null)
            {
                user.Profile.Description = null;
                user.Profile.ProfilePic = null;
                user.Profile.Status = null;
            }

            await UpdateUserAsync(user);
        }

        private void DeleteCredentials(User user)
        {
            user.Email = null;
            user.Password = null;
            user.Phone = null;
        }

        public async Task DeleteUserAsync(string email) => await _userRepository.DeleteByFieldAsync("Email", email);
    }
}
