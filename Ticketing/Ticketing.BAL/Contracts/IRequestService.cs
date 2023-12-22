using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Services;
using Ticketing.DAL.Domains;

namespace Ticketing.BAL.Contracts
{
    public interface IRequestService
    {
        public Task<EmailNotification> FillEmailRequestAsync(string customerEmail, string customerName, int orderId);
    }
}
