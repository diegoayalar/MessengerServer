using Firebase.Database;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;

namespace MessengerPersistency.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FirebaseClient _firebaseClient;

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Chat> Chats { get;}

        public UnitOfWork(FirebaseClient firebaseClient) { 
            _firebaseClient = firebaseClient;

        }

        public Task<int> CommitAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
