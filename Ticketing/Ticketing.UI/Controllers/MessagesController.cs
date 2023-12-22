using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ticketing.BAL.Contracts;

namespace Ticketing.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IRabbitMqService _mqService;
        private readonly IRequestService _requestService;
        private readonly ILog _logger;

        public MessagesController(IRabbitMqService mqService, IRequestService requestService, ILog logger)
        {
            _mqService = mqService;
            _requestService = requestService;
            _logger = logger;
        }

        /// <summary>
        /// Send Emaul Notification
        /// <returns>Collection of set from section for event</returns>
        /// <param name="customerEmail">Customer Email</param>
        /// <param name="customerName">Customer Name</param>
        /// <param name="orderId">Order Id</param>
        /// <response code="200">Return collection of seats</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>
        /// </summary>
        [Route("[action]/{customerEmail}/{customerName}/{orderId}")]
        [HttpPost]
        public async Task<IActionResult> SendRequestAsync(string customerEmail, string customerName, int orderId)
        {
            _logger.Debug($"Send Emaul Notification Customer Email - {customerEmail}, Customer Name - {customerName}, Order Id {orderId}.");

            var emailNotification = await _requestService.FillEmailRequestAsync(customerEmail, customerName, orderId);

            _mqService.SendMessage(emailNotification);

            _logger.Debug("Send Emaul Notification OK.");

            return Ok();
        }
    }
}
