using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerPersistency.IRepository
{
    public interface IGenericRepository <T> where T : class
    {
        Task InsertAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}
