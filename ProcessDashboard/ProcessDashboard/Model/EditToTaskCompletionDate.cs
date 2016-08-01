#region
using System;
#endregion
// ReSharper disable UnusedMember.Local
namespace ProcessDashboard.Model
{
    internal class EditToTaskCompletionDate
    {
        private string TaskId { get; set; }

        private DateTime NewCompletionDate { get; set; }
        private DateTime EditTimestamp { get; set; }
    }
}