using Firebase.Database;

namespace MessengerPersistency.IRepository
{
    public interface IGenericRepository <T> where T : class
    {
        Task<FirebaseObject<T>> InsertAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> GetByFieldAsync(string fieldName, string fieldValue);
        Task<T> GetChildItem<T>(string parentID, string childCollection, string childID);
        Task UpdateOrAddChildItem(string parentID, string childCollection, string childID, Object entity);
        Task<IEnumerable<T>> getFiltredItems<T>(string parentID, string childCollection, int size);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
        Task DeleteByFieldAsync(string fieldName, string fieldValue);
    }
}
