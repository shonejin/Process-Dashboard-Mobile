using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessDashboard.DTO
{
    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public DateTime creationDate { get; set; }
        public bool isActive { get; set; }

        
    }
}
