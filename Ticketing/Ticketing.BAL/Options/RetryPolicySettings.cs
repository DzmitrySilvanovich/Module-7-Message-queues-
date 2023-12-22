using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.BAL.Options
{
    public class RetryPolicySettings
    {
        public int RetryCount { get; set; }
        public int SleepDurationSec { get; set; }
    }
}
