using log4net;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Model;
using Ticketing.BAL.Services;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Ticketing.UnitTests.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.UnitTests.ServicesTests
{
    public class PaymentServiceTests
    {
        static List<Payment> payments = new List<Payment>();
        static List<PaymentStatus> paymentStatuses = new List<PaymentStatus>();
        static List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
        static List<Seat> seats = new List<Seat>();

        static Mock<Repository<Payment>>? _mockPaymentRepository = null;
        static Mock<Repository<PaymentStatus>>? _mockPaymentStatusRepository = null;
        static Mock<Repository<ShoppingCart>>? _mockShoppingCartRepository = null;
        static Mock<Repository<Seat>>? _mockSeatRepository = null;

        [Theory]
        [InlineData(1)]
        public async Task GetPaymentStatusAsync_Success(int Id)
        {
            var service = PrepareDataForSuccess();

            var expected = payments.Join(
                paymentStatuses,
                payment => payment.PaymentStatusId,
                state => state.Id,
                (payment, state) => new PaymentStatusReturnModel { Id = (int)state.Id, Name = state.Name }
            ).First();

            var paymentState = paymentStatuses.First().Id;

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.ToArray().Single(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.ToArray().Single(p => p.Id == pmntState));

            var result = await service.GetPaymentStatusAsync(Id);

            Assert.NotNull(result);
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Name, result.Name);
        }

        [Fact]
        public async Task CompletePaymentAsync_Success()
        {
            var service = PrepareDataForSuccess();
            await service.CompletePaymentAsync(1);

            _mockPaymentRepository.Verify((p) => p.UpdateAsync(It.IsAny<Payment>()), Times.Once, ".Update payment is fail");
            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Once, ".Update seat is fail");
        }

        [Fact]
        public async Task FailPaymentAsync_Success()
        {
            var service = PrepareDataForSuccess();
            await service.FailPaymentAsync(1);

            _mockPaymentRepository.Verify((p) => p.UpdateAsync(It.IsAny<Payment>()), Times.Once, ".Update payment is fail");
            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Once, ".Update seat is fail");
        }

        [Theory]
        [InlineData(10)]
        public async Task GetPaymentStatusAsync_Fail(int Id)
        {
            var service = PrepareDataForSuccess();

            var expected = payments.Join(
                paymentStatuses,
                payment => payment.PaymentStatusId,
                state => state.Id,
                (payment, state) => new PaymentStatusReturnModel { Id = (int)state.Id, Name = state.Name }
            ).First();

            var paymentState = paymentStatuses.First().Id;

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.FirstOrDefault(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.FirstOrDefault(p => p.Id == pmntState));

            var result = await service.GetPaymentStatusAsync(Id);

            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Equal("", result.Name);
        }


        [Theory]
        [InlineData(10)]
        public async Task CompletePaymentAsync_Fail(int Id)
        {
            var service = PrepareDataForSuccess();
            await service.CompletePaymentAsync(Id);

            _mockPaymentRepository.Verify((p) => p.UpdateAsync(It.IsAny<Payment>()), Times.Never, ".Update payment is fail");
            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Never, ".Update seat is fail");
        }

        [Theory]
        [InlineData(10)]
        public async Task FailPaymentAsync_Fail(int Id)
        {
            var service = PrepareDataForSuccess();
            await service.FailPaymentAsync(Id);

            _mockPaymentRepository.Verify((p) => p.UpdateAsync(It.IsAny<Payment>()), Times.Never, ".Update payment is fail");
            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Never, ".Update seat is fail");
        }

        public static PaymentService PrepareDataForSuccess()
        {
            var mockPaymentSet = MockDbSet.BuildAsync(payments = DataHelper.PaymentsInitialization());
            var mockPaymentStatusSet = MockDbSet.BuildAsync(paymentStatuses = DataHelper.PaymentStatusesInitialization());
            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts = DataHelper.ShoppingCartsInitialization());
            var mockSeatSet = MockDbSet.BuildAsync(seats = DataHelper.SeatsInitialization());

            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            mockPaymentSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                       .Returns((object[] r) => new ValueTask<Payment>(payments.FirstOrDefault(b => b.Id == (int)r[0])));
            mockPaymentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] r) => payments.FirstOrDefault(p => p.Id == (int)r[0]));

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<Payment>>(c => c.Payments).Returns(mockPaymentSet.Object);

            mockContext.Setup<DbSet<PaymentStatus>>(c => c.PaymentStatuses).Returns(mockPaymentStatusSet.Object);
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);

            _mockPaymentRepository = new Mock<Repository<Payment>>(mockContext.Object, moqLogObject);
            _mockPaymentStatusRepository = new Mock<Repository<PaymentStatus>>(mockContext.Object, moqLogObject);
            _mockShoppingCartRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object, moqLogObject);
            _mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object, moqLogObject);

            _mockPaymentRepository.Setup(c => c.GetAll()).Returns(mockPaymentSet.Object);
            _mockPaymentStatusRepository.Setup(c => c.GetAll()).Returns(mockPaymentStatusSet.Object);
            _mockShoppingCartRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.FirstOrDefault(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.FirstOrDefault(p => p.Id == pmntState));

            var service = new PaymentService(_mockPaymentRepository.Object, _mockPaymentStatusRepository.Object, _mockShoppingCartRepository.Object, _mockSeatRepository.Object, moqLogObject);

            return service;
        }

        public static PaymentService PrepareDataForFail()
        {
            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockPaymentSet = MockDbSet.BuildAsync(payments = new List<Payment>());
            var mockPaymentStatusSet = MockDbSet.BuildAsync(paymentStatuses = new List<PaymentStatus>());
            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts = new List<ShoppingCart>());
            var mockSeatSet = MockDbSet.BuildAsync(seats = new List<Seat>());

            mockPaymentSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                       .Returns((object[] r) => new ValueTask<Payment>(payments.FirstOrDefault(b => b.Id == (int)r[0])));
            mockPaymentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] r) => payments.FirstOrDefault(p => p.Id == (int)r[0]));

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<Payment>>(c => c.Payments).Returns(mockPaymentSet.Object);

            mockContext.Setup<DbSet<PaymentStatus>>(c => c.PaymentStatuses).Returns(mockPaymentStatusSet.Object);
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);

            _mockPaymentRepository = new Mock<Repository<Payment>>(mockContext.Object, moqLogObject);
            _mockPaymentStatusRepository = new Mock<Repository<PaymentStatus>>(mockContext.Object, moqLogObject);
            _mockShoppingCartRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object, moqLogObject);
            _mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object);

            _mockPaymentRepository.Setup(c => c.GetAll()).Returns(mockPaymentSet.Object);
            _mockPaymentStatusRepository.Setup(c => c.GetAll()).Returns(mockPaymentStatusSet.Object);
            _mockShoppingCartRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.Single(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.Single(p => p.Id == pmntState));

            var service = new PaymentService(_mockPaymentRepository.Object, _mockPaymentStatusRepository.Object, _mockShoppingCartRepository.Object, _mockSeatRepository.Object, moqLogObject);

            return service;
        }
    }
}
