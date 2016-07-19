
using Android.App;
using Android.OS;
using Android.Views;

namespace ProcessDashboard.Droid.Fragments
{
    public class GlobalTimeLogDetail : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;

            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("Global Time Log Detail");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("Global Time Log Detail");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}