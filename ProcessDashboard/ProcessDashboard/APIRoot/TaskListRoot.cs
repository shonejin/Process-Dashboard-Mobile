using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.DTO;

namespace ProcessDashboard.APIRoot
{
    public class TaskListRoot
    {
        public List<Task> projectTasks { get; set; }
        public Project forProject { get; set; }
        public string stat { get; set; }

    }
}
