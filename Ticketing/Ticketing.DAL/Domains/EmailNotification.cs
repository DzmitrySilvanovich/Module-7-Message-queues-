using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.DAL.Domains
{
    public class EmailNotification : EntityBase
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public DateTime EmailTimeStamp { get; set; }
        public int OrderId { get; set; }
        public int OrderAmount { get; set; }
        public decimal OrderSummary { get; set; }
        public EmailRequestStatus EmailRequestStatus { get; set; }
    }
}
