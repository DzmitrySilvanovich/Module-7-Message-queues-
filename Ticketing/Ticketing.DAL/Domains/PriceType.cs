using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domains;

namespace Ticketing.DAL.Domain
{
    public class PriceType : EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
