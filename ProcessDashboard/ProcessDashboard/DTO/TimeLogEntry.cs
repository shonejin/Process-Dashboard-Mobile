using System;

namespace ProcessDashboard.DTO
{

/*
 * Classes for Parsing JSON to OO Model Objects.
 * Variable names are case-sensitive. Donot change or else parsing will fail
 * 
 */ 
    public class TimeLogEntry
    {
        public int Id { get; set; }
        public Task Task { get; set; }
        public DateTime StartDate { get; set; }
        public double LoggedTime { get; set; }
        public double InterruptTime { get; set; }
        public DateTime EndDate { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            return StartDate.ToLongDateString() + " : "+LoggedTime;
        }
    }
}