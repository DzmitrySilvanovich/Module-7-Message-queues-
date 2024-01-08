using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Model;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;

namespace Ticketing.BAL.Contracts
{
    public interface ICartService
    {
        public Task<CartStateReturnModel> AddSeatToCartAsync(Guid cartId, OrderCartModel orderCartModel);

        public Task<int> BookSeatToCartAsync(Guid cartId);

        public Task DeleteSeatForCartAsync(Guid cartId, int eventId, int seatId);

        public Task<IEnumerable<ShoppingCartReturnModel>> CartItemsAsync(Guid cartId);
    }
}
