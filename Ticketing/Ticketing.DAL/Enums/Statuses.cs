using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.DAL.Enums
{
    public class Statuses
    {
        public enum PaymentState { NoPayment = 1, PartPayment = 2, FullPayment = 3, PaymentFailed = 4 };
        public enum SeatState { Available = 1, Booked = 2, Sold = 3 };

        public enum EmailRequestStatus { None = 0, InProgress = 1, Success = 2, Fail = 3 }
    }
}
