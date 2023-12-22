using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domain;

namespace Ticketing.DAL.Contracts
{
     public interface IRepository<T>
    {
        public Task<T> CreateAsync(T entity);

        public IQueryable<T> GetAll();

        public ValueTask<T?> GetByIdAsync(object id);

        public Task UpdateAsync(T entity);

        public Task DeleteAsync(object id);

        public Task DeleteAsync(T entity);
    }
}
