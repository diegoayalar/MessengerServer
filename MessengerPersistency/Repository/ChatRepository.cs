using Firebase.Database;
using Firebase.Database.Query;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;

namespace MessengerPersistency.Repository
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        private readonly FirebaseClient _firebaseClient;

        public ChatRepository(FirebaseClient firebaseClient): base(firebaseClient, "chats")
        {
            _firebaseClient = firebaseClient;
        }

        public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(string userId)
        {
            var chats = await _firebaseClient
                .Child("chats")
                .OrderBy("UserId")
                .EqualTo(userId)
                .OnceAsync<Chat>();

            return chats.Select(item => item.Object);
        }
    }
}
