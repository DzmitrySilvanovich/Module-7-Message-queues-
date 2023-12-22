using log4net;
using Ticketing.BAL.Contracts;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.BAL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _repositoryOrder;
        private readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        private readonly IRepository<Seat> _repositorySeat;
        private readonly ICacheAdapter _cacheAdapter;
        private readonly ILog _logger;

        private const string keyEvents = "events";

        public OrderService(Repository<Order> repositoryOrder,
            Repository<ShoppingCart> repository,
            Repository<Seat> repositorySeat,
            ICacheAdapter cacheAdapter,
            ILog logger)
        {
            _repositoryOrder = repositoryOrder;
            _repositoryShoppingCart = repository;
            _repositorySeat = repositorySeat;
            _cacheAdapter = cacheAdapter;
            _logger = logger;
        }

        public async Task<bool> ReleaseCartsFromOrderAsync(int orderId)
        {
            _logger.Info("OrderService Start ReleaseCartsFromOrderAsync for orderId {orderId}.");
            var order = await _repositoryOrder.GetByIdAsync(orderId);

            if (order is null)
            {
                _logger.Warn("OrderService ReleaseCartsFromOrderAsync for orderId {orderId} order is null.");
                return false;
            }

            var allCarts = _repositoryShoppingCart.GetAll();
            var carts = allCarts.Where(c => c.CartId == order.CartId).ToList();

            if (!carts.Any())
            {
                _logger.Warn("OrderService ReleaseCartsFromOrderAsync for orderId {orderId} there are no carts.");
                return false;
            }

            var seats = _repositorySeat.GetAll();

            var selectedSeats = carts.Join(seats,
                cart => cart.SeatId,
                seat => seat.Id,
                (cart, seat) => new Seat
                {
                    Id = seat.Id,
                    RowNumber = seat.RowNumber,
                    SeatNumber = seat.SeatNumber,
                    SeatStatusState = seat.SeatStatusState,
                    SectionId = seat.SectionId,
                    Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
                }).ToList();


            foreach (var seat in selectedSeats)
            {
                var currentSeat = await _repositorySeat.GetByIdAsync(seat.Id);

                if (currentSeat == null)
                {
                    return false;
                }

                currentSeat.SeatStatusState = SeatState.Available;
                await _repositorySeat.UpdateAsync(currentSeat);
            }

            foreach (var cart in carts)
            {
                await _repositoryShoppingCart.DeleteAsync(cart);
            }

            await _repositoryOrder.DeleteAsync(order);

            _logger.Info("OrderService ReleaseCartsFromOrderAsync for orderId {orderId} return.");
            return true;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _logger.Info("OrderService Start CreateOrder.");
            _cacheAdapter.Invalidate(keyEvents);
            var newOrder = await _repositoryOrder.CreateAsync(order);

            _logger.Info("OrderService CreateOrder return.");
            return newOrder;
        }

        public async Task UpdateOrder(Order order)
        {
            _logger.Info("OrderService Start UpdateOrder.");

            _cacheAdapter.Invalidate(keyEvents);
            await _repositoryOrder.UpdateAsync(order);

            _logger.Info("OrderService Return UpdateOrder.");
        }
    }
}
