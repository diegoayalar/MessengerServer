﻿namespace MessengerPersistency.IRepository
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
