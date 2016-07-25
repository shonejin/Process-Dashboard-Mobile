using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.DTO;

namespace ProcessDashboard.APIRoot
{
    public class EditATimeLogRoot
    {

        public TimeLogEntry TimeLogEntry { get; set; }
        public string Stat { get; set; }
        public Err Err { get; set; }

    }
}
