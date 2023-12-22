using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using log4net;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Venues API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;
        private readonly ILog _logger;

        public VenuesController(IVenueService venueService, ILog logger)
        {
            _venueService = venueService;
            _logger = logger;
        }

        /// <summary>
        /// Get a list with all "<see cref="VenueReturnModel"/>" items.
        /// <returns>Venues</returns>
        /// <response code="200">Return collection of venues</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet]
        [OutputCache(PolicyName = "Expensive")]
        public async Task<IActionResult> Get()
        {
            _logger.Debug("VenuesControler  Start Get venues.");

            var venues = await _venueService.GetVenuesAsync();

            if (venues is null)
            {
                _logger.Error("VenuesControler Get venues Return BadRequest status.");
                return BadRequest(string.Empty);
            }
            else if(!venues.Any())
            {
                _logger.Debug("VenuesControler Get venues Return empty result.");
                return NoContent();
            }

            _logger.Info("VenuesControler  Return venues. Ok");
            return Ok(venues);
        }

        /// <summary>
        /// Get a list with all "<see cref="VenueReturnModel"/>" items.
        /// <param name="venueId">Venue Id</param>
        /// <returns>Collection of section of venues</returns>
        /// <response code="200">Return collection of venues</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet("{venueId}/sections")]
        [OutputCache(PolicyName = "CacheForTenSeconds")]
        public async Task<IActionResult> GetSectionsOfVenue(int venueId)
        {
            _logger.Debug("VenuesControler  Start GetSectionsOfVenue {venueId}.");

            var sections = await _venueService.GetSectionsOfVenueAsync(venueId);

            if (sections is null)
            {
                _logger.Error("VenuesControler GetSectionsOfVenue Return BadRequest status.{venueId}");
                return BadRequest(string.Empty);
            }
            else if (!sections.Any())
            {
                _logger.Debug("VenuesControler Get venues Return empty result.");
                return NoContent();
            }

            _logger.Info("VenuesControler  Return GetSectionsOfVenue .{venueId} Ok");
            return Ok(sections);
        }

        /// <summary>
        /// Delete cache
        /// <param name="cache">IOutputCacheStored</param>
        /// <returns>Collection of section of venues</returns>
        /// <response code="200">Return collection of venues</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpDelete("cache/{tag}")]
        public async Task DeleteCache(IOutputCacheStore cache, string tag) => await cache.EvictByTagAsync(tag, default);
    }
}
