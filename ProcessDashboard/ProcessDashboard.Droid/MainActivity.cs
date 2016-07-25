using System;
using Android.App;
using Android.Widget;
using Android.OS;

using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Views;
using ProcessDashboard.Droid.Fragments;
using ProcessDashboard.Droid.Fragments.Interfaces;
using ProcessDashboard.Model;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid
{
	[Activity (Label = "ProcessDashboard.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : AppCompatActivity,IListOfProjectsInterface,IListOfTaksInterface,ITimeLogsInterface
    {
        DrawerLayout _drawerLayout;

        public enum FragmentTypes { Login, Home, Settings, Listofprojects, Listoftasks, Taskdetails, Tasktimelogdetails, Globaltimelog, Globaltimelogdetails };
        private Home _homeFragment;
        private Login _loginFragment;
        private SettingsPage _settingsFragment;
        private GlobalTimeLog _globalTimeLogFragment;
        private GlobalTimeLogDetail _globalTimeLogDetailFragment;
        private ListOfProjects _listOfProjectFragment;
        private TaskDetails _taskDetailFragment;
        private TaskTimeLogDetail _taskTimeLogDetailFragment;
        private ListProjectTasks _listOfTasksFragment;

	    private TestFragment _testFragment;

        private Fragment _currentFragment;

	    private Android.Support.V7.Widget.Toolbar _toolbar;

	    public Controller Ctrl;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            

            // Create UI

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            // Init toolbar
            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            _toolbar.Title = "Process Dashboard";
            SetSupportActionBar(_toolbar);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetCheckedItem(0);
            navigationView.Menu.GetItem(0).SetChecked(true);

            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout, _toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            _drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();


            _loginFragment = new Login();
            _homeFragment = new Home();
            _settingsFragment = new SettingsPage();
            _globalTimeLogFragment = new GlobalTimeLog();
            _globalTimeLogDetailFragment = new GlobalTimeLogDetail();
            _listOfProjectFragment = new ListOfProjects();
            _taskDetailFragment = new TaskDetails();
            _taskTimeLogDetailFragment = new TaskTimeLogDetail();
            _listOfTasksFragment = new ListProjectTasks("");
            _testFragment = new TestFragment();
            


            // if logged in
            _currentFragment = _homeFragment;
            // else 
            //CurrentFragment = ListOfProjectFragment;

            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            // The fragment will have the ID of Resource.Id.fragment_container.
            fragmentTx.Replace(Resource.Id.fragmentContainer, _currentFragment);
            // Commit the transaction.
            fragmentTx.Commit();



            var apiService = new ApiTypes(null);
            var service = new PDashServices(apiService);
            Ctrl = new Controller(service);
            // c.testProject();
            //_ctrl.testTasks();
            //_ctrl.testSingleTask();
            //_ctrl.testAddATimeLog();
            //_ctrl.testUpdateATimeLog();
           // _ctrl.testDeleteATimeLog();
		}

	    public void setTitle(string title)
	    {
	        _toolbar.Title = title;
	    }

        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_home):
                    // React on 'Home' selection
                    ShowFragment(_homeFragment);
                    
                    break;
                case (Resource.Id.nav_projects):
                    // React on 'Projects' selection
                    ShowFragment(_listOfProjectFragment);
                    
                    break;
                case (Resource.Id.nav_timelogs):
                    // React on 'Time Logs' selection
                    ShowFragment(_globalTimeLogFragment);
                    break;
                case (Resource.Id.nav_settings):
                    // React on 'Settings' selection
                    ShowFragment(_settingsFragment);
                    break;
            }

            // Close drawer
            _drawerLayout.CloseDrawers();
        }


        private void ShowFragment(Fragment fragment)
        {
            if (fragment.IsVisible)
            {
                return;
            }
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            // The fragment will have the ID of Resource.Id.fragment_container.
            fragmentTx.Replace(Resource.Id.fragmentContainer, fragment);
            // Commit the transaction.
            fragmentTx.AddToBackStack("hello"+new Random().Next());
            
            fragmentTx.Commit();
        }
        
        public override void OnBackPressed()
        {

            if (FragmentManager.BackStackEntryCount > 0)
            {
                FragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed();
            }
        }
        
        
        public void SwitchToFragment(FragmentTypes fragmentType)
        {
            switch (fragmentType)
            {
                case FragmentTypes.Home:
                    ShowFragment(_homeFragment);
                    
                    break;
                case FragmentTypes.Login:
                    ShowFragment(_loginFragment);
                    break;
                case FragmentTypes.Settings:
                    ShowFragment(_settingsFragment);
                    break;
                case FragmentTypes.Listoftasks:
                    ShowFragment(_listOfTasksFragment);
                    
                    break;
                case FragmentTypes.Globaltimelog:
                    ShowFragment(_globalTimeLogFragment);
                    break;
                case FragmentTypes.Globaltimelogdetails:
                    ShowFragment(_globalTimeLogDetailFragment);
                    break;
                case FragmentTypes.Listofprojects:
                    ShowFragment(_listOfProjectFragment);
                    
                    break;
                case FragmentTypes.Taskdetails:
                    ShowFragment(_taskDetailFragment); 
                    break;
                case FragmentTypes.Tasktimelogdetails:
                    ShowFragment(_taskTimeLogDetailFragment);
                    break;

            }
        }

	    public void ListOfProjectsCallback(string projectid)
	    {
            
            _listOfTasksFragment.SetId(projectid);
            SwitchToFragment(FragmentTypes.Listoftasks);
            
	    }

	    public void PassTaskDetailsInfo(string taskId)
	    {
	        
            _taskDetailFragment.SetId(taskId);
            SwitchToFragment(FragmentTypes.Taskdetails);
        }

	    public void PassTimeLogInfo(string timelogId)
	    {
	        _taskTimeLogDetailFragment.Id = (timelogId);
            SwitchToFragment(FragmentTypes.Tasktimelogdetails);

	    }
    }
}


