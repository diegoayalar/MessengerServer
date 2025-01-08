using Firebase.Database;
using Firebase.Database.Query;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;

namespace MessengerPersistency.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly string _collectionName;

        public GenericRepository(FirebaseClient firebaseClient, string collectionName)
        {
            _firebaseClient = firebaseClient;
            _collectionName = collectionName;
        }

        public async Task<FirebaseObject<T>> InsertAsync(T entity)
        {
            return await _firebaseClient.Child(_collectionName).PostAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _firebaseClient.Child(_collectionName).OnceAsync<T>()
                .ContinueWith(task => task.Result.Select(item => item.Object));
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _firebaseClient.Child(_collectionName).Child(id).OnceSingleAsync<T>();
        }

        public async Task<T> GetByFieldAsync(string fieldName, string fieldValue)
        {
            var result = await _firebaseClient
                .Child(_collectionName)
                .OrderBy(fieldName)
                .EqualTo(fieldValue)
                .OnceAsync<T>();

            return result.FirstOrDefault()?.Object;
        }

        public async Task<T> GetChildItem<T>(string parentID, string childCollection, string childID) { 
            
            var result = await _firebaseClient
                .Child(_collectionName)
                .Child(parentID)
                .Child(childCollection)
                .Child(childID)
                .OnceSingleAsync<T>();

            return result;
        }

        public async Task<IEnumerable<T>> getFiltredItems<T>(string parentID, string childCollection, int size) {

            var listItems = await _firebaseClient
                .Child(_collectionName)
                .Child(parentID)
                .Child(childCollection)
                .OrderBy("DateSent")
                .LimitToLast(size)
                .OnceAsync<T>();

            
            return listItems.Select(item => item.Object);
        }
        public async Task UpdateOrAddChildItem(string parentID, string childCollection, string childID, Object entity) {

            await _firebaseClient
                .Child(_collectionName)
                .Child(parentID)
                .Child(childCollection)
                .Child(childID)
                .PatchAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var id = (entity.GetType().GetProperty("Id")?.GetValue(entity, null) as string);
            if (id != null)
            {
                await _firebaseClient.Child(_collectionName).Child(id).PutAsync(entity);
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _firebaseClient.Child(_collectionName).Child(id).DeleteAsync();
        }

        public async Task DeleteByFieldAsync(string fieldName, string fieldValue)
        {
            var result = await _firebaseClient
                .Child(_collectionName)
                .OrderBy(fieldName)
                .EqualTo(fieldValue)
                .OnceAsync<T>();

            var entityToDelete = result.FirstOrDefault();

            if (entityToDelete != null)
            {
                await _firebaseClient.Child(_collectionName).Child(entityToDelete.Key).DeleteAsync();
            }
        }

    }
}
