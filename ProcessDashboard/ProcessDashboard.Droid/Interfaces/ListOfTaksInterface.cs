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

namespace ProcessDashboard.Droid.Fragments.Interfaces
{
    interface IListOfTaksInterface
    {

        void PassTaskDetailsInfo(string taskId);

    }
}