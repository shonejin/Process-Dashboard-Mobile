using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessDashboard.Model
{
    class EditToTaskCompletionDate
    {
        String TaskId { get; set; }
        DateTime NewCompletionDate { get; set; }
        DateTime EditTimestamp { get; set; }
    }
}
