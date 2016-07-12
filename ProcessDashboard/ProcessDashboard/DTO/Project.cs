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
    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public DateTime creationDate { get; set; }
        public bool isActive { get; set; }

        public override string ToString()
        {
            return this.name;
        }
    }


}
