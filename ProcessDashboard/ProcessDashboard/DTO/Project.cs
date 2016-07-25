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
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }


}
