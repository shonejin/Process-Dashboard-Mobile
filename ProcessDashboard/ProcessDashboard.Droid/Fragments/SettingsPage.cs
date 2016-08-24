#region
using Android.App;
using Android.OS;
using Android.Preferences;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    // Use Preference Fragment
    public class SettingsPage : PreferenceFragment
    {
        private AccountStorage _accountStorage;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            ((MainActivity)Activity).SetTitle("Settings");
            _accountStorage = new AccountStorage();
            _accountStorage.SetContext(this.Activity);
            // Create your fragment here
            AddPreferencesFromResource(Resource.Layout.Settings);

            Preference pf = FindPreference("login_preference");
            pf.Summary = "Logged in as : " + _accountStorage.UserId;

            pf.PreferenceClick += (sender, args) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("We are about to logout");
                    _accountStorage.ClearStorage(); 
                    System.Diagnostics.Debug.WriteLine("Main Activity is :" + Activity == null);
                    System.Diagnostics.Debug.WriteLine("Items in the backstack :" + Activity.FragmentManager.BackStackEntryCount);
                    System.Diagnostics.Debug.WriteLine("Main Activity is :" + Activity == null);
                    Activity.FragmentManager.PopBackStack(null, PopBackStackFlags.Inclusive);
                    System.Diagnostics.Debug.WriteLine("Items in the backstack 2 :" + Activity.FragmentManager.BackStackEntryCount);
                    ((MainActivity) (Activity)).SetDrawerState(false);
                    ((MainActivity) (Activity)).SwitchToFragment(MainActivity.FragmentTypes.Login);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("We encountered an error :" + e.Message);
                }

            };



        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity) Activity).SetTitle("Settings");
        }
    }
}