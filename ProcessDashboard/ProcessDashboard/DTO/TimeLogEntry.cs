namespace ProcessDashboard.DTO
{
    public class TimeLogEntry
    {
        public string id { get; set; }
        public Task task { get; set; }
        public string startDate { get; set; }
        public double loggedTime { get; set; }
        public double interruptTime { get; set; }
        public string endDate { get; set; }

    }
}