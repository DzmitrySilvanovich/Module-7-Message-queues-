using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore;
using Moq;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.BAL.Services;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Ticketing.UnitTests.Helpers;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.UnitTests.ServicesTests
{
    public class OrderServiceTests
    {
        static List<Order> orders = new List<Order>();
        static List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
        static List<Seat> seats = new List<Seat>();

        static Mock<Repository<Payment>> mockPaymentRepository = null;
        static Mock<Repository<PaymentStatus>> mockPaymentStatusRepository = null;
        static Mock<Repository<ShoppingCart>> mockShoppingCartsRepository = null;
        static Mock<Repository<Seat>> mockSeatRepository = null;
        static Mock<Repository<Order>> mockOrderRepository = null;

        [Theory]
        [InlineData(1)]
        public async Task ReleaseCartsFromOrderAsync_Success(int Id)
        {
            var service = PrepareDataForSuccess();

            mockOrderRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => orders.FirstOrDefault(p => p.Id == i));
            mockSeatRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => seats.FirstOrDefault(p => p.Id == i));

            var result = await service.ReleaseCartsFromOrderAsync(Id);

            Assert.True(result);
            mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Once, ".Update seat is fail");
            mockShoppingCartsRepository.Verify((p) => p.DeleteAsync(It.IsAny<ShoppingCart>()), Times.Once, "Delete ShoppingCart is fail");
            mockOrderRepository.Verify((p) => p.DeleteAsync(It.IsAny<Order>()), Times.Once, "Delete Order seat is fail");
        }

        [Theory]
        [InlineData(2)]
        public async Task ReleaseCartsFromOrderAsync_Fail(int Id)
        {
            var service = PrepareDataForSuccess();

            var result = await service.ReleaseCartsFromOrderAsync(2);

            Assert.False(result);
            mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Never, ".Update seat is fail");
            mockShoppingCartsRepository.Verify((p) => p.DeleteAsync(It.IsAny<ShoppingCart>()), Times.Never, "Delete ShoppingCart is fail");
            mockOrderRepository.Verify((p) => p.DeleteAsync(It.IsAny<Order>()), Times.Never, "Delete Order seat is fail");
        }

        public static OrderService PrepareDataForSuccess()
        {
            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts = DataHelper.ShoppingCartsInitialization());
            var mockSeatSet = MockDbSet.BuildAsync(seats = DataHelper.SeatsInitialization());
            var mockOrderSet = MockDbSet.BuildAsync(orders = DataHelper.OrdersInitialization());

            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<Microsoft.EntityFrameworkCore.DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);
            mockContext.Setup<DbSet<Order>>(c => c.Orders).Returns(mockOrderSet.Object);

            mockPaymentRepository = new Mock<Repository<Payment>>(mockContext.Object, moqLogObject);
            mockPaymentStatusRepository = new Mock<Repository<PaymentStatus>>(mockContext.Object, moqLogObject);
            mockShoppingCartsRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object, moqLogObject);
            mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object, moqLogObject);
            mockOrderRepository = new Mock<Repository<Order>>(mockContext.Object, moqLogObject);

            Mock<ICacheAdapter> mockCache = new Mock<ICacheAdapter>();

            mockShoppingCartsRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);
            mockOrderRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => orders.ToArray().FirstOrDefault(p => p.Id == i));

            var service = new OrderService(mockOrderRepository.Object, mockShoppingCartsRepository.Object, mockSeatRepository.Object, mockCache.Object, moqLogObject);

            return service;
        }
    }
}

