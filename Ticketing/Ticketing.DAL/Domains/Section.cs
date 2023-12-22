using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domains;

namespace Ticketing.DAL.Domain
{
    public class Section : EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int VenueId { get; set; }
        public int PriceTypeId { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
