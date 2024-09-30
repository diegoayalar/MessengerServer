using MessengerDomain.Entities;

namespace MessengerPersistency.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Chat> Chats { get; }
        Task<int> CommitAsync();
    }
}
