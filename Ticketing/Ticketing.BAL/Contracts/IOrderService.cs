using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domain;

namespace Ticketing.BAL.Contracts
{
    public interface IOrderService
    {
        public Task<bool> ReleaseCartsFromOrderAsync(int orderId);

        public Task<Order> CreateOrder(Order order);

        public Task UpdateOrder(Order order);
    }
}
