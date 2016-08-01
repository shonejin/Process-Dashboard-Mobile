#region
using System;
#endregion
namespace ProcessDashboard.DTO
{
    /*
    * Classes for Parsing JSON to OO Model Objects.
    * Variable names are case-sensitive. Donot change or else parsing will fail
    * 
    */

    public class Task
    {
        //TODO: Remove ToString
        public string Id { get; set; }
        public string FullName { get; set; }
        public Project Project { get; set; }
        public DateTime? CompletionDate { get; set; }
        public double EstimatedTime { get; set; }
        public double ActualTime { get; set; }
        public string TaskNote { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}