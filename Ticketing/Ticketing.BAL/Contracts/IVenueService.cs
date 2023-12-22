using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Model;
using Ticketing.DAL.Domain;

namespace Ticketing.BAL.Contracts
{
    public interface IVenueService
    {
        public Task<IEnumerable<VenueReturnModel>> GetVenuesAsync();

        public Task<IEnumerable<SectionReturnModel>> GetSectionsOfVenueAsync(int venueId);
    }
}
