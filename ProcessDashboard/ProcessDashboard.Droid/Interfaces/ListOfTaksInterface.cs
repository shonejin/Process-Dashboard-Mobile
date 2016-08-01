#region
using System;
#endregion
namespace ProcessDashboard.Droid.Interfaces
{
    internal interface IListOfTaksInterface
    {
        void PassTaskDetailsInfo(string id, string taskName, string projectName, DateTime? completionDate,
            double? estimatedTime, double? actualTime);
    }
}