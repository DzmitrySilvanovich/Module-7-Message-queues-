using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.BAL.Model
{
    public class PaymentStatusReturnModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
