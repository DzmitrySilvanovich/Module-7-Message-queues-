using log4net;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;

namespace Ticketing.BAL.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _repositoryEvent;
        private readonly IRepository<Seat> _repositorySeat;
        private readonly IRepository<SeatStatus> _repositorySeatStatus;
        private readonly IRepository<PriceType> _repositoryPriceType;
        private readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        private readonly ILog _logger;

        private readonly ICacheAdapter _cacheAdapter;
        private const string keyEvents = "events";
        private const string keyEventsSeats = "events-seats";


        public EventService(Repository<Event> repositoryEvent,
                            Repository<Seat> repositorySeat,
                            Repository<SeatStatus> repositorySeatStatus,
                            Repository<PriceType> repositoryPriceType,
                            Repository<ShoppingCart> repositoryShoppingCart,
                            ICacheAdapter cacheAdapter,
                            ILog logger)
        {
            _repositoryEvent = repositoryEvent;
            _repositorySeat = repositorySeat;
            _repositorySeatStatus = repositorySeatStatus;
            _repositoryPriceType = repositoryPriceType;
            _repositoryShoppingCart = repositoryShoppingCart;
            _cacheAdapter = cacheAdapter;
            _logger = logger;
        }

        public async Task<IEnumerable<EventReturnModel>> GetEventsAsync()
        {
            _logger.Info("EventService Start GetEventsAsync.");

            var values = _cacheAdapter.Get<IEnumerable<EventReturnModel>>(keyEvents);

            if (values is not null)
            {
                return values;
            }

            var events = _repositoryEvent.GetAll();

            var evntCollection = events.ProjectToType<EventReturnModel>().ToList();

            _cacheAdapter.Set(keyEvents, evntCollection);

            _logger.Info("EventService  GetEventsAsync return result.");
            return await Task.FromResult<IEnumerable<EventReturnModel>>(evntCollection);
        }

        public async Task<List<SeatReturnModel>> GetSeatsAsync(int eventId, int sectionId)
        {
            _logger.Info("EventService Start GetSeatsAsync Event - {eventId}, Section - {sectionId}.");

            var key = $"{keyEventsSeats}-{eventId} -{sectionId}";

            var values = _cacheAdapter.Get<List<SeatReturnModel>>(key);

            if (values is not null)
            {
                return values;
            }

            var seatStatuses = _repositorySeatStatus.GetAll();
            var priceTypes = _repositoryPriceType.GetAll();

            var shoppingCarts = _repositoryShoppingCart.GetAll();
            var seats = _repositorySeat.GetAll();

            var result = (from seat in seats.Where(s => s.SectionId == sectionId)
                          join shoppingCart in shoppingCarts.Where(sh => sh.EventId == eventId)
                          on seat.Id equals shoppingCart.SeatId
                          join seatStatus in seatStatuses.AsEnumerable()
                          on (int)seat.SeatStatusState equals (int)seatStatus.Id
                          join priceType in priceTypes
                          on shoppingCart.PriceTypeId equals priceType.Id
                          select new
                          {
                              SeatId = seat.Id,
                              seat.SectionId,
                              seat.RowNumber,
                              seat.SeatNumber,
                              seat.SeatStatusState,
                              NameSeatStatus = seatStatus.Name,
                              shoppingCart.PriceTypeId,
                              NamePriceType = priceType.Name
                          }).ProjectToType<SeatReturnModel>().ToList();

            _cacheAdapter.Set(key, result);

            _logger.Info("EventService GetSeatsAsync Event - {eventId}, Section - {sectionId} return.");
            return await Task.FromResult<List<SeatReturnModel>>(result);
        }
    }
}
