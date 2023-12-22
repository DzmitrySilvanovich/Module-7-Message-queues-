using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Model;

namespace Ticketing.BAL.Contracts
{
    public interface IPaymentService
    {
        Task<PaymentStatusReturnModel> GetPaymentStatusAsync(int paymentId);

        Task CompletePaymentAsync(int paymentId);

        Task FailPaymentAsync(int paymentId);
    }
}
