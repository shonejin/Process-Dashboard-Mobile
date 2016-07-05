using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessDashboard.Model
{
    class EditsToEstimatedTime
    {
        String TaskId { get; set; }
        long NewEstimatedTime { get; set; }
        DateTime EditTimeStamp { get; set; }
    }
}
