using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Services;
using Ticketing.DAL.Domain;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Payments API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILog _logger;

        public PaymentsController(IPaymentService paymentService, ILog logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get the status of the payment.
        /// <param name="id">Payment id</param>
        /// <returns>payment status of payment</returns>
        /// <response code="200">Return a status of payment</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet("{id}")]
        [OutputCache(PolicyName = "CacheForTenSeconds")]
        public async Task<IActionResult> GetAsync(int id)
        {
            _logger.Info("PaymentsController  Start GetAsync {id}.");

            var res = await _paymentService.GetPaymentStatusAsync(id);

            if (res is null)
            {
                _logger.Error("PaymentsController GetAsync Return BadRequest status.{id}");
                return BadRequest(string.Empty);
            }

            _logger.Info("PaymentsController  Return GetAsync .{id} Ok");
            return Ok(res);
        }

        /// <summary>
        /// Complete Payment.
        /// <param name="id">Payment id</param>
        /// <returns>payment status of payment</returns>
        /// <response code="204"></response>
        /// </summary>
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> PutCompleteAsync(int id)
        {
            _logger.Info("PaymentsController  Start PutCompleteAsync {id}.");

            await _paymentService.CompletePaymentAsync(id);

            _logger.Info("PaymentsController  Return PutCompleteAsync {id}.");

            return Ok();
        }

        /// <summary>
        /// Fail Payment.
        /// <param name="id">Payment id</param>
        /// <returns>payment status of payment</returns>
        /// <response code="204"></response>
        /// </summary>
        [HttpPut("{id}/failed")]
        public async Task<IActionResult> PutFailedAsync(int id)
        {
            _logger.Info("PaymentsController  Start PutFailedAsync {id}.");

            await _paymentService.FailPaymentAsync(id);

            _logger.Info("PaymentsController  Return PutFailedAsync {id}.");

            return Ok();
        }
    }
}
