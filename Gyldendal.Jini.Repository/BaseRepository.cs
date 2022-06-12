using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Jini.Repository.Contracts;
using Gyldendal.Jini.Services.Data;

namespace Gyldendal.Jini.Repository
{
    public class BaseRepository<T> : IRepository<T> where T :class
    {
        protected readonly Jini_Entities Context;
        private DbSet<T> _entities;

        protected virtual DbSet<T> Entities
        {
            get { return _entities ?? (_entities = Context.Set<T>()); }
        }
        public DbSet<T> Table => Entities;
        public IQueryable<T> TableNoTracking => Entities.AsNoTracking();


        public BaseRepository(Jini_Entities context)
        {
            Context = context;
        }
        public async Task<bool> AddAsync(T entity)
        {
            Entities.Add(entity);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddAsync(IList<T> entities)
        {
            Entities.AddRange(entities);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            Entities.Remove(entity);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(IList<T> entities)
        {
            Entities.RemoveRange(entities);
            return await Context.SaveChangesAsync() > 0;
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await Entities.FindAsync(id);
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await Entities.ToListAsync();
        }
        public async Task<bool> UpsertAsync(T entity)
        { 
            Entities.AddOrUpdate(entity);
         return await Context.SaveChangesAsync() > 0;
        }

        public  IQueryable<T> GetAll()
        {
            return Entities.AsQueryable();
        }
    }
}
