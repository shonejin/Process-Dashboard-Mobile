
using Android.App;
using Android.OS;
using Android.Views;

namespace ProcessDashboard.Droid.Fragments
{
    public class TaskDetails : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
             return inflater.Inflate(Resource.Layout.TaskDetail, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}