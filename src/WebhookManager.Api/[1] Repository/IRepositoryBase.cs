using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Repository
{
    public interface IRepositoryBase<T> : IRepository
    {
        Task<T> GetByIdAsync(string id);
        Task CreateAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(T obj);
    }

    public interface IRepository { }
}
