using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.DTO;

namespace ProcessDashboard.APIRoot
{
    public class TimeLogRoot
    {

        public TimeLogEntry timeLogEntry { get; set; }
        public string Stat { get; set; }
        public Err Err { get; set; }
        public override string ToString()
        {
            if (Stat.Equals("ok"))
                return "TimeLog Entries :" + timeLogEntry.Id;
              
            else
                return "Err : " + Err;
        }
    }
}
