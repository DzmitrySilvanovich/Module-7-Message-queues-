using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Model;
using Ticketing.BAL.Services;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Ticketing.UnitTests.Helpers;
using Ticketing.BAL.Contracts;
using log4net;

namespace Ticketing.UnitTests.ServicesTests
{
    public class EventsServiceTests
    {
        private static List<Event> events;
        private static List<SeatStatus> seatStatuses;
        private static List<PriceType> priceType;
        private static List<ShoppingCart> shoppingCarts;
        private static List<Seat> seats;

        [Fact]
        public async Task GetEventsAsync_Success()
        {
            var service = PrepareDataForSuccess();

            var eventCollection = await service.GetEventsAsync();

            var eventArray = eventCollection.ToArray();

            Assert.Equal(3, eventArray.Length);
            Assert.Collection(eventArray,
               item => Assert.Equal("Event1", item.Name),
               item => Assert.Equal("Event2", item.Name),
               item => Assert.Equal("Event3", item.Name));
        }

        [Fact]
        public async Task GetEventsAsync_Fail()
        {
            var service = PrepareDataForFail();

            var eventCollection = await service.GetEventsAsync();

            Assert.Empty(eventCollection);
        }

        [Fact]
        public async Task GetSeatsAsync_Success()
        {
            var service = PrepareDataForSuccess();
            var result = await service.GetSeatsAsync(1, 1);

            Assert.Single(result);
            Assert.Collection(result,
                item => { Assert.Equal(1, item.SeatId);
                    Assert.Equal(1, item.RowNumber);
                    Assert.Equal(1, item.SectionId);
                    Assert.Equal("Available", item.NameSeatStatus);
                    Assert.Equal("Adult", item.NamePriceType);
                }
            );
        }

        [Fact]
        public async Task GetSeatsAsync_Fail()
        {
            var service = PrepareDataForFail();
            var result = await service.GetSeatsAsync(1, 1);

            Assert.Empty(result);
        }

        public static EventService PrepareDataForSuccess()
        {
            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts = DataHelper.ShoppingCartsInitialization());
            var mockSeatSet = MockDbSet.BuildAsync(seats = DataHelper.SeatsInitialization());
            var mockEventSet = MockDbSet.BuildAsync(events = DataHelper.EventsInitialization());
            var mockSeatStatusSet = MockDbSet.BuildAsync(seatStatuses = DataHelper.SeatStatusesInitialization());
            var mockPriceTypeSet = MockDbSet.BuildAsync(priceType = DataHelper.PriceTypesInitialization());

            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);
            mockContext.Setup<DbSet<Event>>(c => c.Events).Returns(mockEventSet.Object);
            mockContext.Setup<DbSet<SeatStatus>>(c => c.SeatStatuses).Returns(mockSeatStatusSet.Object);
            mockContext.Setup<DbSet<PriceType>>(c => c.PriceTypes).Returns(mockPriceTypeSet.Object);

            Mock<Repository<Event>> mockEventRepository = new Mock<Repository<Event>>(mockContext.Object, moqLogObject);
            Mock<Repository<ShoppingCart>> mockShoppingCartsRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object, moqLogObject);
            Mock<Repository<Seat>> mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object, moqLogObject);
            Mock<Repository<PriceType>> mockPriceTypeRepository = new Mock<Repository<PriceType>>(mockContext.Object, moqLogObject);
            Mock<Repository<SeatStatus>> mockSeatStatusRepository = new Mock<Repository<SeatStatus>>(mockContext.Object, moqLogObject);

            Mock<ICacheAdapter> mockCache = new Mock<ICacheAdapter>();

            mockShoppingCartsRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            mockEventRepository.Setup(c => c.GetAll()).Returns(mockEventSet.Object);
            mockSeatRepository.Setup(c => c.GetAll()).Returns(mockSeatSet.Object);
            mockPriceTypeRepository.Setup(c => c.GetAll()).Returns(mockPriceTypeSet.Object);
            mockSeatStatusRepository.Setup(c => c.GetAll()).Returns(mockSeatStatusSet.Object);

            mockCache.Setup(c => c.Get<IEnumerable<EventReturnModel>>("events")).Returns(events.Adapt<IEnumerable<EventReturnModel>>);

            var service = new EventService(mockEventRepository.Object,
                mockSeatRepository.Object,
                mockSeatStatusRepository.Object,

                mockPriceTypeRepository.Object,
                mockShoppingCartsRepository.Object,
                mockCache.Object,
                moqLogObject);

            return service;
        }

        public static EventService PrepareDataForFail()
        {
            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts = DataHelper.ShoppingCartsInitialization());
            var mockSeatSet = MockDbSet.BuildAsync(seats = DataHelper.SeatsInitialization());
            var mockEventSet = MockDbSet.BuildAsync(events = DataHelper.EventsInitialization());
            var mockSeatStatusSet = MockDbSet.BuildAsync(seatStatuses = DataHelper.SeatStatusesInitialization());
            var mockPriceTypeSet = MockDbSet.BuildAsync(priceType = DataHelper.PriceTypesInitialization());

            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);
            mockContext.Setup<DbSet<Event>>(c => c.Events).Returns(mockEventSet.Object);
            mockContext.Setup<DbSet<SeatStatus>>(c => c.SeatStatuses).Returns(mockSeatStatusSet.Object);
            mockContext.Setup<DbSet<PriceType>>(c => c.PriceTypes).Returns(mockPriceTypeSet.Object);

            Mock<Repository<Event>> mockEventRepository = new Mock<Repository<Event>>(mockContext.Object, moqLogObject);
            Mock<Repository<ShoppingCart>> mockShoppingCartsRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object, moqLogObject);
            Mock<Repository<Seat>> mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object, moqLogObject);
            Mock<Repository<PriceType>> mockPriceTypeRepository = new Mock<Repository<PriceType>>(mockContext.Object, moqLogObject);
            Mock<Repository<SeatStatus>> mockSeatStatusRepository = new Mock<Repository<SeatStatus>>(mockContext.Object, moqLogObject);

            Mock<ICacheAdapter> mockCache = new Mock<ICacheAdapter>();

            mockShoppingCartsRepository.Setup(c => c.GetAll()).Returns(mockShoppingCartSet.Object);
            mockPriceTypeRepository.Setup(c => c.GetAll()).Returns(mockPriceTypeSet.Object);
            mockSeatStatusRepository.Setup(c => c.GetAll()).Returns(mockSeatStatusSet.Object);

            var service = new EventService(mockEventRepository.Object,
                mockSeatRepository.Object,
                mockSeatStatusRepository.Object,
                mockPriceTypeRepository.Object,
                mockShoppingCartsRepository.Object,
                mockCache.Object,
                moqLogObject);

            return service;
        }
    }
}
