using log4net;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
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
using log4net;


namespace Ticketing.UnitTests.ServicesTests
{
    public class VenueServiceTests
    {
        static private List<Venue> venues = new List<Venue>();
        static private List<Section> sections = new List<Section>();

        [Fact]
        public async Task GetVenuesAsync_Success()
        {
            var service = PrepareDataForSuccess();

            var collection = await service.GetVenuesAsync();
            var array = collection.ToArray();

            Assert.Equal(3, array.Count());
            Assert.Collection(array,
               item => Assert.Equal("Venue1", item.Name),
               item => Assert.Equal("Venue2", item.Name),
               item => Assert.Equal("Venue3", item.Name));
        }

        [Fact]
        public async Task GetSectionsOfVenue_Success()
        {
            var service = PrepareDataForSuccess();
            var collection = await service.GetSectionsOfVenueAsync(1);
            var array = collection.ToArray();

            Assert.Equal(2, array.Count());
            Assert.Collection(array,
               item => Assert.Equal("Section1", item.Name),
               item => Assert.Equal("Section2", item.Name));
        }

        [Fact]
        public async Task GetVenuesAsync_Fail()
        {
            var service = PrepareDataForFail();

            var collection = await service.GetVenuesAsync();

            Assert.Empty(collection);
        }

        [Fact]
        public async Task GetSectionsOfVenue_Fail()
        {
            var service = PrepareDataForFail();
            var collection = await service.GetSectionsOfVenueAsync(1);

            Assert.Empty(collection);
        }

        public static VenueService PrepareDataForSuccess()
        {
            var mockVenueSet = MockDbSet.BuildAsync(venues = DataHelper.VenuesInitialization());
            var mockSectionSet = MockDbSet.BuildAsync(sections = DataHelper.SectionsInitialization());

            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<Venue>>(c => c.Venues).Returns(mockVenueSet.Object);
            mockContext.Setup<DbSet<Section>>(c => c.Sections).Returns(mockSectionSet.Object);

            Mock<Repository<Venue>> mockVenueRepository = new Mock<Repository<Venue>>(mockContext.Object, moqLogObject);
            Mock<Repository<Section>> mockSectionRepository = new Mock<Repository<Section>>(mockContext.Object, moqLogObject);

            mockVenueRepository.Setup(c => c.GetAll()).Returns(mockVenueSet.Object);
            mockSectionRepository.Setup(c => c.GetAll()).Returns(mockSectionSet.Object);

            var service = new VenueService(mockVenueRepository.Object, mockSectionRepository.Object, moqLogObject);

            return service;
        }

        public static VenueService PrepareDataForFail()
        {
            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockVenueSet = MockDbSet.BuildAsync(venues = new List<Venue>());
            var mockSectionSet = MockDbSet.BuildAsync(sections = new List<Section>());

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<Venue>>(c => c.Venues).Returns(mockVenueSet.Object);
            mockContext.Setup<DbSet<Section>>(c => c.Sections).Returns(mockSectionSet.Object);

            Mock<Repository<Venue>> mockVenueRepository = new Mock<Repository<Venue>>(mockContext.Object, moqLogObject);
            Mock<Repository<Section>> mockSectionRepository = new Mock<Repository<Section>>(mockContext.Object, moqLogObject);

            mockVenueRepository.Setup(c => c.GetAll()).Returns(mockVenueSet.Object);
            mockSectionRepository.Setup(c => c.GetAll()).Returns(mockSectionSet.Object);

            var service = new VenueService(mockVenueRepository.Object, mockSectionRepository.Object, moqLogObject);

            return service;
        }
    }


}

