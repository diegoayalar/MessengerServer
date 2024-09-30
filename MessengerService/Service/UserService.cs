using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerPersistency.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.Service
{
    public class UserService
    {
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task InsertUserAsync(User user) => await _userRepository.InsertAsync(user);
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();
        public async Task<User> GetUserByIdAsync(string id) => await _userRepository.GetByIdAsync(id);
        public async Task UpdateUserAsync(User user) => await _userRepository.UpdateAsync(user);
        public async Task DeleteUserAsync(string id) => await _userRepository.DeleteAsync(id);
    }
}
