using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessDashboard.DTO
{
    public class Task
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public Project project { get; set; }
        public DateTime completionDate { get; set; }
        public double estimatedTime { get; set; }
        public double actualTime { get; set; }
        public string taskNote { get; set; }
    }
}
