using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessDashboard.DTO
{

    /*
    * Classes for Parsing JSON to OO Model Objects.
    * Variable names are case-sensitive. Donot change or else parsing will fail
    * 
    */

    public class Task
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public Project project { get; set; }
        public DateTime completionDate { get; set; }
        public double estimatedTime { get; set; }
        public double actualTime { get; set; }
        public string taskNote { get; set; }

        public override string ToString()
        {
            return fullName;
            
        }
    }
}
