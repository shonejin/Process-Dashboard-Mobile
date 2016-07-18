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
	public class MainActivity : AppCompatActivity,ListOfProjectsInterface,ListOfTaksInterface
    {
        DrawerLayout drawerLayout;

        public enum fragmentTypes { login, home, settings, listofprojects, listoftasks, taskdetails, tasktimelogdetails, globaltimelog, globaltimelogdetails };
        private Home HomeFragment;
        private Login LoginFragment;
        private SettingsPage SettingsFragment;
        private GlobalTimeLog GlobalTimeLogFragment;
        private GlobalTimeLogDetail GlobalTimeLogDetailFragment;
        private ListOfProjects ListOfProjectFragment;
        private TaskDetails TaskDetailFragment;
        private TaskTimeLogDetail TaskTimeLogDetailFragment;
        private ListProjectTasks ListOfTasksFragment;

	    private TestFragment testFragment;

        private Fragment CurrentFragment;

	    private Android.Support.V7.Widget.Toolbar toolbar;

	    public Controller _ctrl;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            

            // Create UI

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            // Init toolbar
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Process Dashboard";
            SetSupportActionBar(toolbar);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();


            LoginFragment = new Login();
            HomeFragment = new Home();
            SettingsFragment = new SettingsPage();
            GlobalTimeLogFragment = new GlobalTimeLog();
            GlobalTimeLogDetailFragment = new GlobalTimeLogDetail();
            ListOfProjectFragment = new ListOfProjects();
            TaskDetailFragment = new TaskDetails();
            TaskTimeLogDetailFragment = new TaskTimeLogDetail();
            ListOfTasksFragment = new ListProjectTasks("");
            testFragment = new TestFragment();
            


            // if logged in
            CurrentFragment = HomeFragment;
            // else 
            //CurrentFragment = ListOfProjectFragment;

            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            // The fragment will have the ID of Resource.Id.fragment_container.
            fragmentTx.Replace(Resource.Id.fragmentContainer, CurrentFragment);
            // Commit the transaction.
            fragmentTx.Commit();



            var apiService = new ApiTypes(null);
            var service = new PDashServices(apiService);
            _ctrl = new Controller(service);
           // c.testProject();
            //c.testTasks();

		}


        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_home):
                    // React on 'Home' selection
                    ShowFragment(HomeFragment);
                    toolbar.Title = "Process Dasboard";
                    break;
                case (Resource.Id.nav_messages):
                    // React on 'Messages' selection
                    ShowFragment(ListOfProjectFragment);
                    toolbar.Title = "Projects";
                    break;
                case (Resource.Id.nav_discussion):
                    // React on 'Friends' selection
                    ShowFragment(SettingsFragment);
                    toolbar.Title = "Settings";
                    break;
                case (Resource.Id.nav_view):
                    // React on 'Discussion' selection
                    break;
            }

            // Close drawer
            drawerLayout.CloseDrawers();
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
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //case Resource.Id.settings:
                  //  switchToFragment(fragmentTypes.settings);
                    //return true;

                default:
                    return true;
            }

        }
        

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
         //   MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void switchToFragment(fragmentTypes fragmentType)
        {
            switch (fragmentType)
            {
                case fragmentTypes.home:
                    ShowFragment(HomeFragment);
                    toolbar.Title = "Process Dasboard";
                    break;
                case fragmentTypes.login:
                    ShowFragment(LoginFragment);
                    break;
                case fragmentTypes.settings:
                    //ShowFragment(SettingsFragment);
                    break;
                case fragmentTypes.listoftasks:
                    ShowFragment(ListOfTasksFragment);
                    toolbar.Title = "Tasks";
                    break;
                case fragmentTypes.globaltimelog:
                    ShowFragment(GlobalTimeLogFragment);
                    break;
                case fragmentTypes.globaltimelogdetails:
                    ShowFragment(GlobalTimeLogDetailFragment);
                    break;
                case fragmentTypes.listofprojects:
                    ShowFragment(ListOfProjectFragment);
                    toolbar.Title = "Projects";
                    break;
                case fragmentTypes.taskdetails:
                    ShowFragment(TaskDetailFragment); 
                    break;
                case fragmentTypes.tasktimelogdetails:
                    ShowFragment(TaskTimeLogDetailFragment);
                    break;

            }
        }

	    public void listOfProjectsCallback(string projectid)
	    {
            toolbar.Title = "Tasks";
            ListOfTasksFragment.SetId(projectid);
            switchToFragment(fragmentTypes.listoftasks);
            
	    }

	    public void passTaskDetailsInfo(string taskId)
	    {
	        toolbar.Title = "Task Details";
            TaskDetailFragment.setId(taskId);
            switchToFragment(fragmentTypes.taskdetails);
        }
    }
}


