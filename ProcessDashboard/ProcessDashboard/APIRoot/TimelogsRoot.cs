﻿using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.DTO;

/*
 * Classes for Parsing JSON to OO Model Objects.
 * Variable names are case-sensitive. Donot change or else parsing will fail
 * 
 */
namespace ProcessDashboard.APIRoot
{
    public class TimeLogsRoot
    {
        public List<TimeLogEntry> timeLogEntries { get; set; }
        public string stat { get; set; }

    }
}
