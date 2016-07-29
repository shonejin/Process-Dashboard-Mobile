using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using HockeyApp.Android;
using ProcessDashboard.Droid.Fragments;
using ProcessDashboard.Droid.Fragments.Interfaces;
using ProcessDashboard.Droid.Interfaces;
using ProcessDashboard.DTO;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Fragment = Android.App.Fragment;
using FragmentTransaction = Android.App.FragmentTransaction;

namespace ProcessDashboard.Droid
{
	[Activity (Label = "ProcessDashboard.Droid", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class MainActivity : AppCompatActivity,IListOfProjectsInterface,IListOfTaksInterface,ITimeLogsInterface,ITimeLogEditInterface
        //,IOnBackStackChangedListener
    {
        DrawerLayout _drawerLayout;

	    private string APP_ID = "168ed05dd48a4d32b8bbcc25fec3d3a9";

        public enum FragmentTypes { Login, Home, Settings, Listofprojects, Listoftasks, Taskdetails, Tasktimelogdetails, Globaltimelog, Globaltimelogdetails };
        private Home _homeFragment;
        private Login _loginFragment;
        private SettingsPage _settingsFragment;
        private GlobalTimeLogList _globalTimeLogFragment;
        private TimeLogDetail _TimeLogDetailFragment;
        private ListOfProjects _listOfProjectFragment;
        private TaskDetails _taskDetailFragment;
        private TaskTimeLogList _taskTimeLogDetailFragment;
        private ListProjectTasks _listOfTasksFragment;

	    private TestFragment _testFragment;

        private Fragment _currentFragment;

	    private Android.Support.V7.Widget.Toolbar _toolbar;

        private static bool _CrashHandlerRegistered = false;

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
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            //_toolbar.

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetCheckedItem(0);
            navigationView.Menu.GetItem(0).SetChecked(true);

            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout, _toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            _drawerLayout.SetDrawerListener(drawerToggle);
          //  drawerToggle.SetHomeAsUpIndicator(null);
            drawerToggle.SyncState();


            _loginFragment = new Login();
            _homeFragment = new Home();
            _settingsFragment = new SettingsPage();
            _globalTimeLogFragment = new GlobalTimeLogList();
            _TimeLogDetailFragment = new TimeLogDetail();
            _listOfProjectFragment = new ListOfProjects();
            _taskDetailFragment = new TaskDetails();
            _taskTimeLogDetailFragment = new TaskTimeLogList();
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

            // ...
            checkForCrashes();
            //  checkForUpdates();


            // FragmentManager.AddOnBackStackChangedListener(this);
            // shouldDisplayHomeUp();


            // c.testProject();
            //_ctrl.testTasks();
            //_ctrl.testSingleTask();
            //_ctrl.testAddATimeLog();
            Ctrl.TestUpdateATimeLog();
            Ctrl.TestDeleteATimeLog(-50);
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
                    ShowFragment(_TimeLogDetailFragment);
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

	    protected override void OnResume()
	    {
	        base.OnResume();
            checkForCrashes();
            if (!_CrashHandlerRegistered)
            {
                /*
                _CrashHandlerRegistered = true;
                CrashManager.Register(this, PlatformUtils.HockeyApplicationId);
                //copy build properties into c# land so that the handler won't crash accessing java
                TraceWriter.InitializeConstants();
                //TraceWriter.Initialize(listener); // instead of InitializeConstants: if you want things like user/contact/description

                AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
                {
                    TraceWriter.WriteTrace(args.Exception);
                    args.Handled = true;
                };
                AppDomain.CurrentDomain.UnhandledException +=
                    (sender, args) => TraceWriter.WriteTrace(args.ExceptionObject);
                TaskScheduler.UnobservedTaskException += (sender, args) => TraceWriter.WriteTrace(args.Exception);
                ExceptionSupport.UncaughtTaskExceptionHandler = TraceWriter.WriteTrace;
                */
            }
        }

	    public void ListOfProjectsCallback(string projectid)
	    {
            
            _listOfTasksFragment.SetId(projectid);
            SwitchToFragment(FragmentTypes.Listoftasks);
            
	    }

	    public void PassTaskDetailsInfo(string id, string taskName, string projectName, DateTime? completionDate, double? estimatedTime, double? actualTime)
        {
	        _taskDetailFragment.SetId(id,taskName,projectName,completionDate,estimatedTime,actualTime);
            SwitchToFragment(FragmentTypes.Taskdetails);
        }

	    public void PassTimeLogInfo(string timelogId,string projectName,string taskName)
	    {
            
            _taskTimeLogDetailFragment.SetData(timelogId,projectName,taskName);
            SwitchToFragment(FragmentTypes.Tasktimelogdetails);

	    }
        
	    public void TimeLogEditCallBack(string projectname, string taskName,string taskId, TimeLogEntry timelogId)
	    {
            //  throw new NotImplementedException();
            _TimeLogDetailFragment = new TimeLogDetail();
            _TimeLogDetailFragment.SetData(projectname,taskName,taskId,timelogId);
            SwitchToFragment(FragmentTypes.Globaltimelogdetails);


        }

        /*
	    public void OnBackStackChanged()
	    {
	        shouldDisplayHomeUp();
	    }

        public  void shouldDisplayHomeUp()
        {
            //Enable Up button only  if there are entries in the back stack
            bool canback = FragmentManager.BackStackEntryCount > 0;
            this.SupportActionBar?.SetDisplayHomeAsUpEnabled(canback);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                // Respond to the action bar's Up/Home button
                case Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        public override bool OnSupportNavigateUp()
        {
            //This method is called when the up button is pressed. Just the pop back stack.
            FragmentManager.PopBackStack();
            return true;
        }
        */

        protected override void OnDestroy()
        {
            base.OnDestroy();
            unregisterManagers();
        }

        protected override void OnPause()
        {
            base.OnPause();
            unregisterManagers();
        }

        //HockeyApp


        private void checkForCrashes()
        {
            CrashManager.Register(this, APP_ID);
        }

        private void checkForUpdates()
        {
            // Remove this for store builds!
            UpdateManager.Register(this, APP_ID);
        }

        private void unregisterManagers()
        {
            UpdateManager.Unregister();
            
            // unregister other managers if necessary...
        }

    }
}


