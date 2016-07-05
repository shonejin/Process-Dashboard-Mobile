using System;

namespace ProcessDashboard.DTO
{
    public class TimeLogEntry
    {
        public int id { get; set; }
        public Task task { get; set; }
        public DateTime startDate { get; set; }
        public double loggedTime { get; set; }
        public double interruptTime { get; set; }
        public DateTime endDate { get; set; }

    }
}