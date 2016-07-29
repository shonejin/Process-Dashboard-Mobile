
using System;
using System.Collections;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using ProcessDashboard.DTO;
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
                string projectId = p.GetProject(e.Position).Id;
                //  ((MainActivity)this.Activity).switchToFragment(MainActivity.fragmentTypes.listoftasks);
                ((MainActivity)this.Activity).ListOfProjectsCallback(projectId);

            };
            System.Diagnostics.Debug.WriteLine("We are now assigning values to the list view");
            GetData(listView, ((MainActivity)this.Activity).Ctrl);


        }
   

        private async void GetData(ListView listView, Controller ctrl)
        {
            List<Project> output = await ctrl.GetProjects(Settings.GetInstance().Dataset);
            ArrayList al = new ArrayList();
            ProjectsAdapter listAdapter = new ProjectsAdapter(Activity, Android.Resource.Layout.SimpleListItem1, output.ToArray(),1);
         
            listView.Adapter = listAdapter;
            SetListShown(true);
        }
        
		private void LoadDummyData(ListView listView)
		{
			string[] items = new string[] { "Sample Project", "Linux Kernel", "Windows 11 Ultimate", "Mobile Process Dashboard"};
			//ArrayAdapter ListAdapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleListItem1, items);
			//listView.Adapter = ListAdapter;
		}
    }



    public class ProjectsAdapter : ArrayAdapter
    {
        Project[] _projectList;

        public ProjectsAdapter(Context context, int resource, Project[] objects, int flag) : base(context, resource, objects)
        {
            System.Diagnostics.Debug.WriteLine("We are in the right constructor");
            this._projectList = objects ;
        }

      
        public Project GetProject(int position)
        {
            return _projectList[position];
        }
    }
}