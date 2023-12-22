using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domains;

namespace Ticketing.DAL.Domain
{
    public class Order : EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Guid CartId { get; set; }
    }
}
