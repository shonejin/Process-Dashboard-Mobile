#region
using System;
#endregion
namespace ProcessDashboard.APIRoot
{
    public class Err
    {
        public string Code { get; set; }
        public string Msg { get; set; }

        public DateTime StopTime { get; set; }

        public override string ToString()
        {
            return "Code : " + Code + " Msg :" + Msg;
        }
    }
}