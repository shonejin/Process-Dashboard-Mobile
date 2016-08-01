#region
using ProcessDashboard.DTO;
#endregion
namespace ProcessDashboard.APIRoot
{
    public class TimeLogRoot
    {
        public TimeLogEntry TimeLogEntry { get; set; }
        public string Stat { get; set; }
        public Err Err { get; set; }
        public override string ToString()
        {
            if (Stat.Equals("ok"))
                return "TimeLog Entries :" + TimeLogEntry.Id;

            return "Err : " + Err;
        }
    }
}