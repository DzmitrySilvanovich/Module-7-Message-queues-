using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.BAL.Options
{
    public class EmailSettings
    {
        public required string ApiKey { get; set; }
        public required string SenderEmail { get; set; }
        public required string SenderName { get; set; }
    }
}
