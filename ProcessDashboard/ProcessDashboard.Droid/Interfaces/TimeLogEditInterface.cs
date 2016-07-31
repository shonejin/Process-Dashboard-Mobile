#region
using ProcessDashboard.DTO;
#endregion
namespace ProcessDashboard.Droid.Interfaces
{
    internal interface ITimeLogEditInterface
    {
        void TimeLogEditCallBack(string projectname, string taskName, string taskId, TimeLogEntry timelog);
    }
}