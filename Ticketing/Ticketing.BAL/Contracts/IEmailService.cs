using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domains;

namespace Ticketing.BAL.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailNotification emailNotification);
    }
}
