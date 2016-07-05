using System;
using System.Collections.Generic;
using System.Text;

using ProcessDashboard.DTO;

namespace ProcessDashboard.APIRoot
{
    public class TimeLogsRoot
    {
        public List<TimeLogEntry> timeLogEntries { get; set; }
        public string stat { get; set; }

    }
}
