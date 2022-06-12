using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Repository.Contracts
{
    public interface IRepository<T> where T:class
    {
        Task<bool> AddAsync(T entity);
        Task<bool> AddAsync(IList<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteAsync(IList<T> entities);
        DbSet<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<bool> UpsertAsync(T entity);
        IQueryable<T> GetAll();
    }
}
