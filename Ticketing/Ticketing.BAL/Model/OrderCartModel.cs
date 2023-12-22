using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domain;

namespace Ticketing.BAL.Model
{
    public class OrderCartModel
    {
        public int EventId { get; set; }
        public int SeatId { get; set; }
        public int PriceTypeId { get; set; }
        public decimal Price { get; set; }
    }
}
