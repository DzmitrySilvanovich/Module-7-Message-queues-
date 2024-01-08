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
using Ticketing.DAL.Domain;
using Ticketing.UI.Controllers;

namespace Ticketing.UnitTests.ControllersTests
{
    public class CartsControllerTests
    {
        private readonly IFixture _fixture;

        private readonly Guid guid = Guid.NewGuid();

        public CartsControllerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAsync_Succes()
        {
            var collection = _fixture.Build<ShoppingCartReturnModel>()
             .CreateMany(1)
             .ToList();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.CartItemsAsync(guid)).ReturnsAsync(collection);

            var controller = new CartsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.GetAsync(guid);

            service.Verify(u => u.CartItemsAsync(guid), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAsync_fail()
        {
            var collection = _fixture.Build<ShoppingCartReturnModel>()
             .CreateMany(0)
             .ToList();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.CartItemsAsync(guid)).ReturnsAsync(collection);

            var controller = new CartsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.GetAsync(guid);

            service.Verify(u => u.CartItemsAsync(guid), Times.Once, "fail");
            Assert.IsType<NoContentResult>(result);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Post_Succesc()
        {
            var model = _fixture.Build<CartStateReturnModel>().Create();
            var orderCartModel = _fixture.Build<OrderCartModel>().Create();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.AddSeatToCartAsync(guid, orderCartModel)).Returns(Task.FromResult(model));

            var controller = new CartsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.Post(guid, orderCartModel);

            service.Verify(u => u.AddSeatToCartAsync(guid, orderCartModel), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutAsync_Succesc()
        {
            var model = _fixture.Create<int>();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.BookSeatToCartAsync(guid)).Returns(Task.FromResult(model));

            var controller = new CartsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.PutAsync(guid);

            service.Verify(u => u.BookSeatToCartAsync(guid), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutAsync_Fail()
        {
            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.BookSeatToCartAsync(guid)).Returns(Task.FromResult(0));

            var controller = new CartsController(service.Object, new Mock<ILog>().Object);
            var result = await controller.PutAsync(guid);

            service.Verify(u => u.BookSeatToCartAsync(guid), Times.Once, "fail");
            Assert.IsType<BadRequestResult>(result);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteAsync_Success()
        {
            Mock<ICartService> service = new Mock<ICartService>();
            service.Setup(s => s.DeleteSeatForCartAsync(guid, 1, 1)).Returns(Task.CompletedTask);
            var controller = new CartsController(service.Object, new Mock<ILog>().Object);

            var result = await controller.DeleteAsync(guid, 1, 1);

            service.Verify(u => u.DeleteSeatForCartAsync(guid, 1, 1), Times.Once, "fail");
            Assert.IsType<OkResult>(result);
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteAsync_Fail()
        {
            Mock<ICartService> service = new Mock<ICartService>();
            service.Setup(s => s.DeleteSeatForCartAsync(guid, 1, 1)).Returns(Task.CompletedTask);
            var controller = new CartsController(service.Object, new Mock<ILog>().Object);

            var result = await controller.DeleteAsync(guid, 1, 1);

            service.Verify(u => u.DeleteSeatForCartAsync(guid, 1, 1), Times.Once, "fail");
            Assert.IsType<OkResult> (result);
            result.Should().BeOfType<OkResult>();
        }

    }
}
