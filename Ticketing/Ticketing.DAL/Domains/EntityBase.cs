using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.DAL.Domains
{
    public class EntityBase
    {
        [ConcurrencyCheck]
        public required byte[] Version { get; set; }
    }
}
