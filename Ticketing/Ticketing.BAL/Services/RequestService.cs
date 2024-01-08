using log4net;
using Ticketing.BAL.Contracts;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.BAL.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRepository<Order> _repositoryOrder;
        private readonly IRepository<Cart> _repositoryCart;
        private readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        private readonly ILog _logger;

        public RequestService(Repository<Order> repositoryOrder,
                              Repository<Cart> repositoryCart,
                              Repository<ShoppingCart> repositoryShoppingCart,
                              ILog logger)
        {
            _repositoryOrder = repositoryOrder;
            _repositoryCart = repositoryCart;
            _repositoryShoppingCart = repositoryShoppingCart;
            _logger = logger;
        }
        public async Task<EmailNotification> FillEmailRequestAsync(string customerEmail, string customerName, int orderId)
        {
            _logger.Debug($"Start FillEmailRequestAsync Customer Email - {customerEmail}, Customer Name - {customerName}, Order Id {orderId}.");

            int orderAmount = 0;
            decimal orderSummary = 0;

            var emailNotification = new EmailNotification
            {
                Email = customerEmail,
                Name = customerName,
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond),
                EmailRequestStatus = EmailRequestStatus.None,
                OrderAmount = orderAmount,
                OrderSummary = orderSummary,
                EmailTimeStamp = DateTime.Now,
                OrderId = orderId
            };

            var order = await _repositoryOrder.GetByIdAsync(orderId);

            if (order is not null)
            {
                var cart = await _repositoryCart.GetByIdAsync(order.CartId);

                if (cart is not null)
                {
                    var shoppingCarts = _repositoryShoppingCart.GetAll().Where(sh => sh.CartId == cart.Id).ToList();

                    foreach (var item in shoppingCarts)
                    {
                        ++orderAmount;
                        orderSummary += item.Price;
                    }

                    emailNotification.OrderAmount = orderAmount;
                    emailNotification.OrderSummary = orderSummary;
                }
            }

            _logger.Debug("Finish FillEmailRequestAsync");

            return emailNotification; 
        }
    }
}
