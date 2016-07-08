using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using ProcessDashboard.Model;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid
{
	[Activity (Label = "ProcessDashboard.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            // Create UI
            
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            // Init toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button> (Resource.Id.myButton);

            var apiService = new ApiTypes(null);
            var service = new PDashServices(apiService);
            Controller c = new Controller(service);
           // c.testProject();
            c.testTasks();

		}


        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_home):
                    // React on 'Home' selection
                    break;
                case (Resource.Id.nav_messages):
                    // React on 'Messages' selection
                    break;
                case (Resource.Id.nav_friends):
                    // React on 'Friends' selection
                    break;
                case (Resource.Id.nav_discussion):
                    // React on 'Discussion' selection
                    break;
            }

            // Close drawer
            drawerLayout.CloseDrawers();
        }
    }
}


