using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class CartServiceTests
    {
        static List<ShoppingCart> _shoppingCarts = new List<ShoppingCart>();
        static List<Payment> _payments = new List<Payment>();
        static List<Seat> _seats = new List<Seat>();

        static Mock<Repository<Payment>> _mockPaymentRepository = null;
        static Mock<Repository<ShoppingCart>> _mockShoppingCartsRepository = null;
        static Mock<Repository<Seat>> _mockSeatRepository = null;

        [Fact]
        public async Task AddSeatToCart_Succes()
        {
            var service = PrepareDataForSuccess();

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => _payments.FirstOrDefault(p => p.Id == i));

            _mockShoppingCartsRepository.Setup(c => c.CreateAsync(It.IsAny<ShoppingCart>())).Callback(() => _shoppingCarts.Add(new ShoppingCart
            {
                Id = 1,
                EventId = 1,
                SeatId = 1,
                PriceTypeId = 1,
                Price = 2,
                CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"),
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
            }));

            var orderCart = new OrderCartModel
            {
                EventId = 1,
                SeatId = 2,
                PriceTypeId = 1,
                Price = 15
            };

            var result = await service.AddSeatToCartAsync(new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), orderCart);

            _mockShoppingCartsRepository.Verify((p) => p.CreateAsync(It.IsAny<ShoppingCart>()), Times.Once, ".Update ShoppingCart is fail");

            Assert.NotNull(result);
            Assert.Equal(new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), result.CartId);
            Assert.Equal(3, result.TotalAmount);
        }

        [Fact]
        public async Task BookSeatToCartAsync_Succes()
        {
            var service = PrepareDataForSuccess();

            _mockShoppingCartsRepository.Setup(c => c.CreateAsync(It.IsAny<ShoppingCart>())).Callback(() => _shoppingCarts.Add(new ShoppingCart
            {
                Id = 1,
                EventId = 1,
                SeatId = 1,
                PriceTypeId = 1,
                Price = 2,
                CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"),
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
            }));

            var payment = new Payment
            {
                Id = 15,
                Amount = 15,
                CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"),
                PaymentStatusId = PaymentState.NoPayment,
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
            };

            _mockPaymentRepository.Setup(c => c.CreateAsync(It.IsAny<Payment>())).Callback(() => _payments.Add(payment)).Returns(Task.FromResult(payment));

            var result = await service.BookSeatToCartAsync(new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"));

            Assert.Equal(15, result);

            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Once, ".Update Seat is fail");
        }


        [Fact]
        public async Task DeleteSeatForCartAsync_Succes()
        {
            var service = PrepareDataForSuccess();

            await service.DeleteSeatForCartAsync(new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), 1, 1);

            _mockShoppingCartsRepository.Verify((p) => p.DeleteAsync(It.IsAny<ShoppingCart>()), Times.Once, ".Delete ShoppingCart is fail");
        }

        [Fact]
        public async Task CartItemsAsync_Succes()
        {
            var service = PrepareDataForSuccess();

            var collection = await service.CartItemsAsync(new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"));

            Assert.Single(collection);
            Assert.Collection(collection,
                item => {
                    Assert.Equal(1, item.SeatId);
                    Assert.Equal(1, item.EventId);
                }
            );
        }

        [Fact]
        public async Task BookSeatToCartAsync_Fail()
        {
            var service = PrepareDataForSuccess();

            _mockShoppingCartsRepository.Setup(c => c.CreateAsync(It.IsAny<ShoppingCart>())).Callback(() => _shoppingCarts.Add(new ShoppingCart
            {
                Id = 1,
                EventId = 1,
                SeatId = 1,
                PriceTypeId = 1,
                Price = 2,
                CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"),
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
            }));

            var payment = new Payment
            {
                Id = 15,
                Amount = 15,
                CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"),
                PaymentStatusId = PaymentState.NoPayment,
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
            };

            _mockPaymentRepository.Setup(c => c.CreateAsync(It.IsAny<Payment>())).Callback(() => _payments.Add(payment)).Returns(Task.FromResult(payment));

            var result = await service.BookSeatToCartAsync(Guid.NewGuid());

            Assert.Equal(15, result);

            _mockShoppingCartsRepository.Verify((p) => p.DeleteAsync(It.IsAny<ShoppingCart>()), Times.Never, ".Delete ShoppingCart is fail");
        }

        [Fact]
        public async Task DeleteSeatForCartAsync_Fail()
        {
            var service = PrepareDataForSuccess();

            await service.DeleteSeatForCartAsync(Guid.NewGuid(), 1, 1);

            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Never, ".Delete ShoppingCart is fail");
        }

        [Fact]
        public async Task CartItemsAsync_Fail()
        {
            var service = PrepareDataForSuccess();

            var collection = await service.CartItemsAsync(Guid.NewGuid());

            Assert.Empty(collection);
        }

        public static CartService PrepareDataForSuccess()
        {
            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockPaymentSet = MockDbSet.BuildAsync(_payments = DataHelper.PaymentsInitialization());
            var mockShoppingCartSet = MockDbSet.BuildAsync(_shoppingCarts = DataHelper.ShoppingCartsInitialization());
            var mockSeatSet = MockDbSet.BuildAsync(_seats = DataHelper.SeatsInitialization());

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<Microsoft.EntityFrameworkCore.DbSet<Payment>>(c => c.Payments).Returns(mockPaymentSet.Object);
            mockContext.Setup<Microsoft.EntityFrameworkCore.DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);

            _mockPaymentRepository = new Mock<Repository<Payment>>(mockContext.Object, moqLogObject);
            _mockShoppingCartsRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object, moqLogObject);
            _mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object, moqLogObject);

            _mockShoppingCartsRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);
            _mockShoppingCartsRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);

            var service = new CartService(_mockShoppingCartsRepository.Object, _mockSeatRepository.Object, _mockPaymentRepository.Object, moqLogObject);

            return service;
        }

        public static CartService PrepareDataForFail()
        {
            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockPaymentSet = MockDbSet.BuildAsync(_payments = new List<Payment>());
            var mockShoppingCartSet = MockDbSet.BuildAsync(_shoppingCarts = new List<ShoppingCart>());
            var mockSeatSet = MockDbSet.BuildAsync(_seats = new List<Seat>());

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<Microsoft.EntityFrameworkCore.DbSet<Payment>>(c => c.Payments).Returns(mockPaymentSet.Object);
            mockContext.Setup<Microsoft.EntityFrameworkCore.DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);

            _mockPaymentRepository = new Mock<Repository<Payment>>(mockContext.Object, moqLogObject);
            _mockShoppingCartsRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object, moqLogObject);
            _mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object, moqLogObject);

            _mockShoppingCartsRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);
            _mockShoppingCartsRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);

            var service = new CartService(_mockShoppingCartsRepository.Object, _mockSeatRepository.Object, _mockPaymentRepository.Object, moqLogObject);

            return service;
        }
    }
}
