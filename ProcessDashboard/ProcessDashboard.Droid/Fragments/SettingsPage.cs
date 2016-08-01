#region
using Android.OS;
using Android.Preferences;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    // Use Preference Fragment
    public class SettingsPage : PreferenceFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;

            ((MainActivity) Activity).SetTitle("Settings");
            // Create your fragment here
            AddPreferencesFromResource(Resource.Layout.Settings);
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity) Activity).SetTitle("Settings");
        }
    }
}