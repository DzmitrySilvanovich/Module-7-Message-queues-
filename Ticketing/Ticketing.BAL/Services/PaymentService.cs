using log4net;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.BAL.Services
{
    public class PaymentService : IPaymentService
    {
        readonly IRepository<Payment> _repositoryPayment;
        readonly IRepository<PaymentStatus> _repositoryPaymentStatus;
        readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        readonly IRepository<Seat> _repositorySeat;
        private readonly ILog _logger;

        public PaymentService(Repository<Payment> repositoryPayment,
             Repository<PaymentStatus> repositoryPaymentStatus,
             Repository<ShoppingCart> repositoryShoppingCart,
             Repository<Seat> repositorySeat,
             ILog logger)
        {
            _repositoryPayment = repositoryPayment;
            _repositoryPaymentStatus = repositoryPaymentStatus;
            _repositoryShoppingCart = repositoryShoppingCart;
            _repositorySeat = repositorySeat;
             _logger = logger;
        }

        public async Task<PaymentStatusReturnModel> GetPaymentStatusAsync(int paymentId)
        {
            _logger.Info("PaymentService  Start GetPaymentStatusAsync for paymentId {paymentId}.");
            var returnPaymentStatus = new PaymentStatusReturnModel { Id = 0, Name = string.Empty };

            var payment = await _repositoryPayment.GetByIdAsync(paymentId);

            if (payment is null)
            {
                _logger.Warn("PaymentService GetPaymentStatusAsync for paymentId {paymentId} payment is null.");
                return returnPaymentStatus;
            }

            var paymentStatus = await _repositoryPaymentStatus.GetByIdAsync(payment.PaymentStatusId);

            if (paymentStatus is null)
            {
                _logger.Warn("PaymentService GetPaymentStatusAsync for paymentId {paymentId} returnPaymentStatus is null.");
                return returnPaymentStatus;
            }

            _logger.Info("PaymentService GetPaymentStatusAsync for paymentId {paymentId} returnPaymentStatus.");
            return paymentStatus.Adapt(returnPaymentStatus);
        }

        public async Task CompletePaymentAsync(int paymentId)
        {
            _logger.Info("PaymentService  Start CompletePaymentAsync for paymentId {paymentId}.");
            var payment = await _repositoryPayment.GetByIdAsync(paymentId);

            if (payment is null)
            {
                _logger.Warn("PaymentService CompletePaymentAsync for paymentId {paymentId} payment is null.");
                return;
            }

            payment.PaymentStatusId = PaymentState.FullPayment;
            await _repositoryPayment.UpdateAsync(payment);

            var shoppingCarts = _repositoryShoppingCart.GetAll();

            var shoppingCartItems = shoppingCarts.Where(c => c.CartId == payment.CartId).ToList();

            var shoppingCartSeats = shoppingCartItems.Select(sh => sh.SeatId).ToList();
                
            var allSeats = _repositorySeat.GetAll();

            var seats = allSeats.Where(s => shoppingCartSeats.Contains(s.Id));

            foreach (var seat in seats)
            {
                seat.SeatStatusState = SeatState.Sold;
                await _repositorySeat.UpdateAsync(seat);
            }

            _logger.Info("PaymentService CompletePaymentAsync for paymentId {paymentId} return.");
        }

        public async Task FailPaymentAsync(int paymentId)
        {
            _logger.Info("PaymentService  Start FailPaymentAsync for paymentId {paymentId}.");
            var payment = await _repositoryPayment.GetByIdAsync(paymentId);

            if (payment is null)
            {
                _logger.Warn("PaymentService FailPaymentAsync for paymentId {paymentId} payment is null.");
                return;
            }

            payment.PaymentStatusId = PaymentState.PaymentFailed;
            await _repositoryPayment.UpdateAsync(payment);

            var shoppingCarts =  _repositoryShoppingCart.GetAll();

            var shoppingCartItems = shoppingCarts.Where(c => c.CartId == payment.CartId).ToList();

            var shoppingCartSeats = shoppingCartItems.Select(sh => sh.SeatId).ToList();

            var allSeats = _repositorySeat.GetAll();

            var seats = allSeats.Where(s => shoppingCartSeats.Contains(s.Id));

            foreach (var seat in seats)
            {
                seat.SeatStatusState = SeatState.Available;
                await _repositorySeat.UpdateAsync(seat);
            }

            _logger.Info("PaymentService FailPaymentAsync for paymentId {paymentId} return.");
        }
    }
}
