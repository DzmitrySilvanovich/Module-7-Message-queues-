using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.UnitTests.Helpers
{
    public static class MockDbSet
    {
        public static Mock<DbSet<TEntity>> BuildAsync<TEntity>(List<TEntity> data) where TEntity : class
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<TEntity>>();
            mockSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TEntity>(queryable.GetEnumerator()));

            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<TEntity>(queryable.Provider));

            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            mockSet.Setup(m => m.Add(It.IsAny<TEntity>())).Callback<TEntity>(data.Add);
            mockSet.Setup(m => m.AddAsync(It.IsAny<TEntity>(), default)).Callback<TEntity, CancellationToken>((s, token) =>
            {
                data.Add(s);
            });
            mockSet.Setup(set => set.Remove(It.IsAny<TEntity>())).Callback<TEntity>(t => data.Remove(t));

            return mockSet;
        }
    }
}
