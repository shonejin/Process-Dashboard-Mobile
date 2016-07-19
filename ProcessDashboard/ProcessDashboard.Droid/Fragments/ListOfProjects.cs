
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.View.Menu;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DTO;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using Object = Java.Lang.Object;

namespace ProcessDashboard.Droid.Fragments
{
    public class ListOfProjects : ListFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("List of Projects");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("List of projects");
        }


        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            ListView listView = this.ListView;
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {


                // var p = e.Parent.SelectedItem.Class;

                ProjectsAdapter p = (ProjectsAdapter)listView.Adapter;
                System.Diagnostics.Debug.WriteLine("Type:" + p.GetProject(e.Position));

                string projectID = p.GetProject(e.Position).id;
                //  ((MainActivity)this.Activity).switchToFragment(MainActivity.fragmentTypes.listoftasks);

                ((MainActivity)this.Activity).listOfProjectsCallback(projectID);

            };
            System.Diagnostics.Debug.WriteLine("We are now assigning values to the list view");
            test(listView, ((MainActivity)this.Activity)._ctrl);


        }
   

        private async void test(ListView listView, Controller ctrl)
        {
            List<Project> output = await ctrl.GetProjects("mock");
            ArrayList al = new ArrayList();
            ProjectsAdapter ListAdapter = new ProjectsAdapter(Activity, Android.Resource.Layout.SimpleListItem1, output.ToArray(),1);
         
            listView.Adapter = ListAdapter;
            SetListShown(true);
        }
        
		private void loadDummyData(ListView listView)
		{
			string[] items = new string[] { "Sample Project", "Linux Kernel", "Windows 11 Ultimate", "Mobile Process Dashboard"};
			//ArrayAdapter ListAdapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleListItem1, items);
			//listView.Adapter = ListAdapter;
		}
    }



    public class ProjectsAdapter : ArrayAdapter
    {
        Project[] _projectList;

        Activity _activity;

        public ProjectsAdapter(Context context, int resource, Project[] objects, int flag) : base(context, resource, objects)
        {
            System.Diagnostics.Debug.WriteLine("We are in the right constructor");
            this._projectList = objects ;
        }

        public ProjectsAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public ProjectsAdapter(Context context, int resource) : base(context, resource)
        {
        }

        public ProjectsAdapter(Context context, int resource, int textViewResourceId)
            : base(context, resource, textViewResourceId)
        {
        }

        public ProjectsAdapter(Context context, int resource, int textViewResourceId, IList objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }

        public ProjectsAdapter(Context context, int resource, int textViewResourceId, Object[] objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }

        public ProjectsAdapter(Context context, int resource, IList objects) : base(context, resource, objects)
        {
        }

        public ProjectsAdapter(Context context, int resource, Object[] objects) : base(context, resource, objects)
        {
        }


        public Project GetProject(int position)
        {
            return _projectList[position];
        }
    }
}