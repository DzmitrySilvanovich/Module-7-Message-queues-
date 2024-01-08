using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domains;
using static System.Collections.Specialized.BitVector32;

namespace Ticketing.DAL.Domain
{
    public class Venue : EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<Section> Sections { get; set; } = new List<Section>();
    }
}
