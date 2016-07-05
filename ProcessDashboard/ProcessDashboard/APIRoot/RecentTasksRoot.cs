using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.DTO;

namespace ProcessDashboard.APIRoot
{
    public class RecentTasksRoot
    {
        public List<Task> recentTasks { get; set; }
        public string stat { get; set; }
    }
}
