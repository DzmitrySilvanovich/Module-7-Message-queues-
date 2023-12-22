using log4net;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static System.Collections.Specialized.BitVector32;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.BAL.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        private readonly IRepository<Seat> _repositorySeat;
        private readonly IRepository<Payment> _repositoryPayment;
        private readonly ILog _logger;

        public CartService(Repository<ShoppingCart> repository, Repository<Seat> repositorySeat, Repository<Payment> repositoryPayment, ILog logger)
        {
            _repositoryShoppingCart = repository;
            _repositorySeat = repositorySeat;
            _repositoryPayment = repositoryPayment;
            _logger = logger;
        }

        public async Task<CartStateReturnModel> AddSeatToCartAsync(Guid cartId, OrderCartModel orderCartModel)
        {
            _logger.Info("CartService Start AddSeatToCartAsync CartId {cartId}.");

            var shoppingCarts = _repositoryShoppingCart.GetAll();

            var item = shoppingCarts.FirstOrDefault(c => c.CartId == cartId && c.EventId == orderCartModel.EventId && c.SeatId == orderCartModel.SeatId);

            var shoppingCartDto = new ShoppingCart
            {
                EventId = orderCartModel.EventId,
                SeatId = orderCartModel.SeatId,
                PriceTypeId = orderCartModel.PriceTypeId,
                Price = orderCartModel.Price,
                CartId = cartId,
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
            };

            await CreateOrUpdateAsync();

            var totalAmount = shoppingCarts.Sum(sc => sc.Price);

            _logger.Info("CartService AddSeatToCartAsync CartId {cartId}  return result.");

            return new CartStateReturnModel
            {
                CartId = cartId,
                TotalAmount = totalAmount
            };

            async Task CreateOrUpdateAsync()
            {
                if (item is null)
                {
                    _logger.Info("CartService AddSeatToCartAsync create shopping cart.");

                    await _repositoryShoppingCart.CreateAsync(shoppingCartDto);
                }
                else
                {
                    _logger.Info("CartService AddSeatToCartAsync update shopping cart.");
                    item.PriceTypeId = orderCartModel.PriceTypeId;
                    item.Price = orderCartModel.Price;
                    await _repositoryShoppingCart.UpdateAsync(shoppingCartDto);
                }
            }
        }

        public async Task<int> BookSeatToCartAsync(Guid cartId)
        {
            _logger.Info("CartService Start BookSeatToCartAsync CartId {cartId}.");

            var shoppingCarts = _repositoryShoppingCart.GetAll();
            var shoppingCartItems = shoppingCarts.Where(c => c.CartId == cartId).ToList();
            var shoppingCartSeats = shoppingCartItems.Select(sh => sh.SeatId).ToList();

            var allSeats = _repositorySeat.GetAll();
            var seats = allSeats.Where(s => shoppingCartSeats.Contains(s.Id));

            decimal totalAmount = shoppingCartItems.Sum(i => i.Price);

            foreach (var seat in seats)
            {
                seat.SeatStatusState = SeatState.Booked;
                await _repositorySeat.UpdateAsync(seat);
            }

            var payment = new Payment
            {
                Amount = totalAmount,
                CartId = cartId,
                PaymentStatusId = PaymentState.NoPayment,
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
            };

            var newPayment = await _repositoryPayment.CreateAsync(payment);

            _logger.Info("CartService Start BookSeatToCartAsync CartId {cartId} return newPayment.Id {newPayment.Id}.");
            return newPayment.Id;
        }

        public async Task DeleteSeatForCartAsync(Guid cartId, int eventId, int seatId)
        {
            _logger.Info("CartService Start DeleteSeatForCartAsync CartId {cartId} Event {eventId}, Seat {seatId}.");
            var shoppingCarts = _repositoryShoppingCart.GetAll();

            var shoppingCartItems = await shoppingCarts.Where(c => c.CartId == cartId && c.EventId == eventId && c.SeatId == seatId).ToListAsync();

            foreach (var item in shoppingCartItems)
            {
                await _repositoryShoppingCart.DeleteAsync(item);
            }

            _logger.Info("CartService DeleteSeatForCartAsync CartId {cartId} Event {eventId}, Seat {seatId} return.");
        }

        public async Task<IEnumerable<ShoppingCartReturnModel>> CartItemsAsync(Guid cartId)
        {
            _logger.Info("CartService Start CartItemsAsync CartId {cartId}.");
            var items = _repositoryShoppingCart.GetAll();

            _logger.Info("CartService CartItemsAsync CartId {cartId} return result.");
            return await Task.FromResult<IEnumerable<ShoppingCartReturnModel>>(items.Where(i => i.CartId == cartId).ProjectToType<ShoppingCartReturnModel>().ToList());
        }
    }
}
