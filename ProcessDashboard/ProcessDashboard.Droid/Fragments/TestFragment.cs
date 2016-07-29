using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid.Fragments
{
    public class TestFragment : Fragment
    {
        private MainActivity _mActivity;

        /*
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            System.Diagnostics.Debug.WriteLine("On Attach");
            mActivity = (MainActivity) context;
        }
        */
        public override void OnAttach(Activity actvity)
        {
            base.OnAttach(actvity);
            System.Diagnostics.Debug.WriteLine("On Attach 2");
            _mActivity = (MainActivity)actvity;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine("On Create");
            RetainInstance = true;
            Controller controller = ((MainActivity)this.Activity).Ctrl;

           // _controller.testTimeLog();
            //_controller.testTimeLogWithID("i5ixdkxc:15565303");
            controller.TestSingleTask();

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            System.Diagnostics.Debug.WriteLine("On Create View");
            return base.OnCreateView(inflater, container, savedInstanceState);
        }




    }
}