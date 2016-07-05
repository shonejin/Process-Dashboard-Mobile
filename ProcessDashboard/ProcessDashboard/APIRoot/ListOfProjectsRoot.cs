using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.DTO;

namespace ProcessDashboard.APIRoot
{
    public class ListOfProjectsRoot
    {
        public List<Project> projects { get; set; }
        public string stat { get; set; }
    }
}
