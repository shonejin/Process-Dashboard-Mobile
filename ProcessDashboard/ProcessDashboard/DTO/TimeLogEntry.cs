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
        public int id { get; set; }
        public Task task { get; set; }
        public DateTime startDate { get; set; }
        public double loggedTime { get; set; }
        public double interruptTime { get; set; }
        public DateTime endDate { get; set; }

    }
}