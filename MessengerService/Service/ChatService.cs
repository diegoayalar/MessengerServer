using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.Service
{
    public class ChatService
    {
        private readonly IGenericRepository<Chat> _chatRepository;

        public ChatService(IGenericRepository<Chat> Repository)
        {
            _chatRepository = Repository;
        }

        public async Task InsertUserAsync(Chat user) => await _chatRepository.InsertAsync(user);
        public async Task<IEnumerable<Chat>> GetAllUsersAsync() => await _chatRepository.GetAllAsync();
        public async Task<Chat> GetUserByIdAsync(string id) => await _chatRepository.GetByIdAsync(id);
        public async Task UpdateUserAsync(Chat chat) => await _chatRepository.UpdateAsync(chat);
        public async Task DeleteUserAsync(string id) => await _chatRepository.DeleteAsync(id);
    }
}
