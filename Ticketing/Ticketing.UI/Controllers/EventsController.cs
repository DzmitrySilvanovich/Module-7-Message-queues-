using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Events API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILog _logger;

        public EventsController(IEventService eventService, ILog logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        /// <summary>
        /// Get a list with all "<see cref="EventReturnModel"/>" items.
        /// <returns>Events</returns>
        /// <response code="200">Return collection of events</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet]
        [OutputCache(PolicyName = "CacheForTenSeconds")]
        public async Task<IActionResult> Get()
        {
            _logger.Info("EventsController  Start Get.");
            var events = await _eventService.GetEventsAsync();

            if (events is null)
            {
                _logger.Error("EventsController Get venues Return BadRequest status.");
                return BadRequest(string.Empty);
            }
            else if (!events.Any())
            {
                _logger.Warn("EventsController Get venues Return empty result.");
                return NoContent();
            }

            _logger.Info("EventsController  Return venues. Ok");
            return Ok(events);
        }

        /// <summary>
        /// Get a list with all "<see cref="SeatReturnModel"/>" items.
        /// <returns>Collection of set from section for event</returns>
        /// <param name="eventId">Event id</param>
        /// <param name="sectionId">Section id</param>
        /// <response code="200">Return collection of seats</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet("{eventId}/sections/{sectionId}/seats")]
        [OutputCache(PolicyName = "CacheForTenSeconds")]
        public async Task<IActionResult> GetSeatsAsync(int eventId, int sectionId)
        {
            _logger.Info($"EventsController  Start GetSeatsAsync  event {eventId}  section {sectionId} ");
            var seats = await _eventService.GetSeatsAsync( eventId, sectionId);

            if (seats is null)
            {
                _logger.Info($"EventsController  Return GetSeatsAsync  event {eventId}  section {sectionId} BadRequest status.");
                return BadRequest(string.Empty);
            }
            else if (!seats.Any())
            {
                _logger.Info("EventsController  Return GetSeatsAsync  event {eventId}  section {sectionId} NoContent status.");
                return NoContent();
            }

            _logger.Info($"EventsController  Return GetSeatsAsync  event {eventId}  section {sectionId} Ok status.");
            return Ok(seats);
        }
    }
}
