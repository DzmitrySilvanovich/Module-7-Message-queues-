using AutoFixture;
using Azure;
using FluentAssertions;
using log4net;
using MapsterMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.UI.Controllers;

namespace Ticketing.UnitTests.ControllersTests
{
    public class VenuesControllerTests
    {
        private readonly IFixture _fixture;

        public VenuesControllerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAsync_Succes()
        {
            var collection = _fixture.Build<VenueReturnModel>()
             .CreateMany(1)
             .ToList();

            Mock<IVenueService> service = new Mock<IVenueService>();

            service.Setup(s => s.GetVenuesAsync()).ReturnsAsync(collection);

            var controller = new VenuesController(service.Object, new Mock<ILog>().Object);
            var result = await controller.Get();

            service.Verify(u => u.GetVenuesAsync(), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAsync_Fail()
        {
            var collection = _fixture.Build<VenueReturnModel>()
             .CreateMany(0)
             .ToList();

            Mock<IVenueService> service = new Mock<IVenueService>();

            service.Setup(s => s.GetVenuesAsync()).ReturnsAsync(collection);

            var controller = new VenuesController(service.Object, new Mock<ILog>().Object);
            var result = await controller.Get();

            service.Verify(u => u.GetVenuesAsync(), Times.Once, "fail");
            Assert.IsType<NoContentResult>(result);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetSectionsOfVenue_Succes()
        {
            var collection = _fixture.Build<SectionReturnModel>()
             .CreateMany(1)
             .ToList();

            Mock<IVenueService> service = new Mock<IVenueService>();

            service.Setup(s => s.GetSectionsOfVenueAsync(1)).ReturnsAsync(collection);

            var controller = new VenuesController(service.Object, new Mock<ILog>().Object);
            var result = await controller.GetSectionsOfVenue(1);

            service.Verify(u => u.GetSectionsOfVenueAsync(1), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetSectionsOfVenue_Fail()
        {
            var collection = _fixture.Build<SectionReturnModel>()
             .CreateMany(0)
             .ToList();

            Mock<IVenueService> service = new Mock<IVenueService>();

            service.Setup(s => s.GetSectionsOfVenueAsync(1)).ReturnsAsync(collection);

            var controller = new VenuesController(service.Object, new Mock<ILog>().Object);
            var result = await controller.GetSectionsOfVenue(1);

            service.Verify(u => u.GetSectionsOfVenueAsync(1), Times.Once, "fail");
            Assert.IsType<NoContentResult>(result);
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
