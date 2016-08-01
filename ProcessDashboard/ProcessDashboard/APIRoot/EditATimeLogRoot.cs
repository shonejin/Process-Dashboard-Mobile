#region
using ProcessDashboard.DTO;
#endregion
namespace ProcessDashboard.APIRoot
{
    public class EditATimeLogRoot
    {
        public TimeLogEntry TimeLogEntry { get; set; }
        public string Stat { get; set; }
        public Err Err { get; set; }
    }
}