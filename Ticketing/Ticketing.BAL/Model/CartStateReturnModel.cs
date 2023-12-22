using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.BAL.Model
{
    public class CartStateReturnModel
    {
        public Guid CartId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
