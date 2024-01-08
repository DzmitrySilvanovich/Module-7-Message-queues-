using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Carts API
    /// </summary>
    [Route("api/[controller]/carts")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILog _logger;

        public CartsController(ICartService cartService, ILog logger)
        {
            _cartService = cartService;
            _logger = logger;

        }

        /// <summary>
        /// Get items of the cart.
        /// <param name="id">Cart id</param>
        /// <returns>The collection of items of the cart</returns>
        /// <response code="200">Return a status of request</response>
        /// <response code="400">Bad request</response>
        /// </summary>
        [HttpGet("{id}")]
        [OutputCache(PolicyName = "CacheForTenSeconds")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            _logger.Info("CartsController  Start Get {id}.");
            var shoppingCarts = await _cartService.CartItemsAsync(id);

            if (shoppingCarts is null)
            {
                _logger.Error("CartsController Get {id} Return BadRequest status.");
                return BadRequest(string.Empty);
            }
            else if (!shoppingCarts.Any())
            {
                _logger.Warn("CartsController Get {id} Return empty result.");
                return NoContent();
            }

            _logger.Info("CartsController  Return Get.{id} Ok ststus");
            return Ok(shoppingCarts);
        }

        /// <summary>
        /// Add Seat To Cart.
        /// <param name="id">Cart id</param>
        /// <param name="orderCartModel">Payload for the action</param>
        /// <returns>The collection of items of the cart</returns>
        /// <response code="200">Return a status of request with result</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpPost("{id}")]
         public async Task<IActionResult> Post(Guid id, [FromBody] OrderCartModel orderCartModel)
        {
            _logger.Info("CartsController  Start Post {id}.");
            var result = await _cartService.AddSeatToCartAsync(id, orderCartModel);

            if (result is null)
            {
                _logger.Error("CartsController Post {id} Return BadRequest status.");
                return BadRequest(string.Empty);
            }

            _logger.Info("CartsController Post {id} Return Ok status.");
            return Ok(result);
        }

        /// <summary>
        /// Book Seat To Cart.
        /// <param name="cartId">Cart id</param>
        /// <response code="200">Return a status of request</response>
        /// </summary>
        [HttpPut("{cartId}/book")]
        public async Task<IActionResult> PutAsync(Guid cartId)
        {
            _logger.Info("CartsController Start PutAsync {cartId}.");
            var result = await _cartService.BookSeatToCartAsync(cartId);

            if(result == 0)
            {
                _logger.Error("CartsController PutAsync {id} Return BadRequest status.");
                return BadRequest();
            }

            _logger.Info("CartsController PutAsync {id} Return Ok status.");
            return Ok(result);
        }

        /// <summary>
        /// Delete Seat For Cart.
        /// <param name="cartId">Cart id</param>
        /// <param name="eventId">Event id</param>
        /// <param name="seatId">Seat id</param>
        /// <response code="200">Return a status of request</response>
        /// </summary>
        [HttpDelete("{cartId}/events/{eventId}/seats/{seatId}")]
        public async Task<IActionResult> DeleteAsync(Guid cartId, int eventId, int seatId)
        {
            _logger.Info("CartsController Start DeleteAsync Cart {cartId} Event {eventId} Seat {seatId}.");
            await _cartService.DeleteSeatForCartAsync(cartId, eventId, seatId);

            _logger.Info("CartsController Return DeleteAsync Cart {cartId} Event {eventId} Seat {seatId} Ok ststus.");
            return Ok();
        }
    }
}
