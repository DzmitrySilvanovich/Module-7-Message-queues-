using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.BAL.Options
{
    public class RabbitMqSettings
    {
        public required string HostName { get; set; }
        public required string QueueName { get; set; }
    }
}
