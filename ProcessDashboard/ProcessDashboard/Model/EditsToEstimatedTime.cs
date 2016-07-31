#region
using System;
#endregion
// ReSharper disable UnusedMember.Local
namespace ProcessDashboard.Model
{
    internal class EditsToEstimatedTime
    {
        private string TaskId { get; set; }
        private long NewEstimatedTime { get; set; }
        private DateTime EditTimeStamp { get; set; }
    }
}