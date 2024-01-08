using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using log4net;

namespace Ticketing.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext _db;
        private readonly DbSet<T> _dbSet;
        private readonly ILog _logger;

        public Repository(ApplicationContext context, ILog logger)
        {
            _db = context;
            _dbSet = _db.Set<T>();
            _logger = logger;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            EntityEntry<T> addedEntity = await _dbSet.AddAsync(entity);

            await _db.SaveChangesAsync();

            _logger.Info($"Repository {typeof(T)} CreateAsync() {addedEntity.Entity}");
            return addedEntity.Entity;
        }

        public virtual IQueryable<T> GetAll()
        {
            _logger.Info($"Repository {typeof(T)} GetAll()");
            return _dbSet;
        }

        public virtual async ValueTask<T?> GetByIdAsync(object id)
        {
            _logger.Info($"Repository {typeof(T)} GetByIdAsync() {id}");
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _logger.Info($"Repository {typeof(T)} UpdateAsync() {entity} start");
            _dbSet.Update(entity);

            try
            {
                await _db.SaveChangesAsync();
                _logger.Info($"Repository {typeof(T)} UpdateAsync() {entity} save");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
                var entry = ex.Entries.Single();
                var proposedValues = entry.CurrentValues;
                var databaseValues = entry.GetDatabaseValues();
                entry.OriginalValues.SetValues(proposedValues);

                _logger.Info($"Repository {typeof(T)} UpdateAsync() save exception {databaseValues} {proposedValues}");

                await _db.SaveChangesAsync();
                _logger.Info($"Repository {typeof(T)} UpdateAsync() save succesfull");
            }
        }

        public virtual async Task DeleteAsync(object id)
        {
            if (await _dbSet.FindAsync(id) is T entity)
            {
                await DeleteAsync(entity);

                _logger.Info($"Repository {typeof(T)} DeleteAsync() {id}");
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);

            try
            {
                await _db.SaveChangesAsync();
                _logger.Info($"Repository {typeof(T)} DeleteAsync() {entity}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
                var entry = ex.Entries.Single();
                var clientValues = entry.Entity;
                var databaseValues = entry.GetDatabaseValues();
                entry.OriginalValues.SetValues(databaseValues);
                entry.CurrentValues.SetValues(databaseValues);

                _logger.Info($"Repository {typeof(T)} DeleteAsync() delete exception {databaseValues}");
                await _db.SaveChangesAsync();
                _logger.Info($"Repository {typeof(T)} DeleteAsync() save succesfull");
            }
        }
    }
}
