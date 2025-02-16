using Firebase.Database;
using MessengerDomain.Entities;

namespace MessengerPersistency.IRepository
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<IEnumerable<Chat>> GetChatsByUserIdAsync(string userId);
    }
}
