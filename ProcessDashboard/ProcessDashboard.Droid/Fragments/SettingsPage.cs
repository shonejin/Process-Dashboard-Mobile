
using Android.App;
using Android.OS;
using Android.Views;

namespace ProcessDashboard.Droid.Fragments
{
    // Use Preference Fragment
    public class SettingsPage : Android.Preferences.PreferenceFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            
            ((MainActivity)(this.Activity)).setTitle("Settings");
            // Create your fragment here
            AddPreferencesFromResource(Resource.Layout.Settings);
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("Settings");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}