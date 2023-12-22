using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Contracts;

namespace Ticketing.DAL.Repositories
{
    public class PessimisticRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext _db;
        private readonly DbSet<T> _dbSet;

        public PessimisticRepository(ApplicationContext context)
        {
            _db = context;
            _dbSet = _db.Set<T>();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            EntityEntry<T> addedEntity = await _dbSet.AddAsync(entity);

            await _db.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public virtual async ValueTask<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            using var transaction = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try
            {
                _dbSet.Update(entity);
                await _db.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }

        public virtual async Task DeleteAsync(object id)
        {
            if (await _dbSet.FindAsync(id) is T entity)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            using var transaction = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try
            {
                _dbSet.Remove(entity);
                await _db.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }
}
