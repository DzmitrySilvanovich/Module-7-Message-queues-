using AutoFixture;
using FluentAssertions;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.UI.Controllers;

namespace Ticketing.UnitTests.ControllersTests
{
    public class EventsControllerTests
    {
        private readonly IFixture _fixture;

        public EventsControllerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAsync_Succes()
        {
            var collection = _fixture.Build<EventReturnModel>()
             .CreateMany(1)
             .ToList();

            Mock<IEventService> service = new Mock<IEventService>();

            service.Setup(s => s.GetEventsAsync()).ReturnsAsync(collection);

            var controller = new EventsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.Get();

            service.Verify(u => u.GetEventsAsync(), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAsync_Fail()
        {
            var collection = _fixture.Build<EventReturnModel>()
             .CreateMany(0)
             .ToList();

            Mock<IEventService> service = new Mock<IEventService>();

            service.Setup(s => s.GetEventsAsync()).ReturnsAsync(collection);

            var controller = new EventsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.Get();

            service.Verify(u => u.GetEventsAsync(), Times.Once, "fail");
            Assert.IsType<NoContentResult>(result);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetSeatsAsync_Success()
        {
            var collection = _fixture.Build<SeatReturnModel>()
             .CreateMany(1)
             .ToList();

            Mock<IEventService> service = new Mock<IEventService>();

            service.Setup(s => s.GetSeatsAsync(1, 1)).Returns(Task.FromResult(collection));

            var controller = new EventsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.GetSeatsAsync(1,1);

            service.Verify(u => u.GetSeatsAsync(1, 1), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetSeatsAsync_Fail()
        {
            var collection = _fixture.Build<SeatReturnModel>()
             .CreateMany(0)
             .ToList();

            Mock<IEventService> service = new Mock<IEventService>();

            service.Setup(s => s.GetSeatsAsync(1, 1)).Returns(Task.FromResult(collection));

            var controller = new EventsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.GetSeatsAsync(1, 1);

            service.Verify(u => u.GetSeatsAsync(1, 1), Times.Once, "fail");
            Assert.IsType<NoContentResult>(result);
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
