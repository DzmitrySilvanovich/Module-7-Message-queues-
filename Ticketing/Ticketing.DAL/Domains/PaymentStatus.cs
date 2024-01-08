using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domains;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.DAL.Domain
{
    public class PaymentStatus : EntityBase
    {
        public PaymentState Id { get; set; }
        public required string Name { get; set; }
    }
}
