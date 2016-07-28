using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;

namespace ProcessDashboard.Droid.Interfaces
{
    interface ITimeLogEditInterface
    {

        void TimeLogEditCallBack(string projectname, string taskName,string taskId, TimeLogEntry timelog);
      

    }
}