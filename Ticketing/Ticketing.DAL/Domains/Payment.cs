using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Enums;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.DAL.Domain
{
    public class Payment : EntityBase
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public required PaymentState PaymentStatusId { get; set; }
        public Guid CartId { get; set; }
    }
}
