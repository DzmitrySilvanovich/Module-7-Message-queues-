using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Model;
using Ticketing.BAL.Services;
using Ticketing.DAL.Domain;

namespace Ticketing.BAL.Contracts
{
    public interface IEventService
    {
        public Task<IEnumerable<EventReturnModel>> GetEventsAsync();

        public Task<List<SeatReturnModel>> GetSeatsAsync(int eventId, int sectionId);
    }
}
