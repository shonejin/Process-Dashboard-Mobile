namespace ProcessDashboard.Droid.Interfaces
{
    internal interface ITimeLogsInterface
    {
        void PassTimeLogInfo(string timelogId, string projectName, string taskName);
    }
}