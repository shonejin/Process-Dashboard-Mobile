using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.DTO;

namespace ProcessDashboard.APIRoot
{
    public class EditATimeLogRoot
    {

        public TimeLogEntry timeLogEntry { get; set; }
        public string stat { get; set; }
        public Err err { get; set; }

    }
}
