
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid.Fragments
{
    public class ListProjectTasks : ListFragment
    {
        private string projectID;

        public ListProjectTasks (string projectID)
        {
            this.projectID = projectID;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            setID(projectID);
            ListView listView = this.ListView;
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                TaskAdapter ta = (TaskAdapter)listView.Adapter;
                Task p = ta.GetTask(e.Position);
                string taskID = p.id;
                ((MainActivity)this.Activity).switchToFragment(MainActivity.fragmentTypes.taskdetails);

                //Project p = listView.GetItemAtPosition(e.Position);

            };
            test(((MainActivity)this.Activity)._ctrl, projectID);


        }

        public void setID(string projectID)
        {
            this.projectID = projectID;
        }

        
        

        private async void test(Controller ctrl,string projectID)
        {
            List<Task> output = await ctrl.GetTasks("mock",projectID);
            TaskAdapter ListAdapter = new TaskAdapter(Activity, Android.Resource.Layout.SimpleListItem1,output.ToArray(),1);
            this.ListView.Adapter = ListAdapter;
            SetListShown(true);
        }


        public void loadDummyData()
        {
            string[] values = new[] { "Sample Task", "Component 1 / Component 2 / Code", "... / head truncation" };
            this.ListAdapter = new Android.Widget.ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleExpandableListItem1, values);
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //base.OnListItemClick(l, v, position, id);
            //ListView.SetItemChecked(position, true);
            //((MainActivity)Activity).switchToFragment(MainActivity.fragmentTypes.taskdetails);
            
        }

      
    }

    public class TaskAdapter : ArrayAdapter
    {
        Task[] _taskList;

        Activity _activity;

        public TaskAdapter(Context context, int resource, Task[] objects, int flag) : base(context, resource, objects)
        {
            System.Diagnostics.Debug.WriteLine("We are in the right constructor");
            this._taskList = objects;
        }

        public TaskAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public TaskAdapter(Context context, int resource) : base(context, resource)
        {
        }

        public TaskAdapter(Context context, int resource, int textViewResourceId)
            : base(context, resource, textViewResourceId)
        {
        }

        public TaskAdapter(Context context, int resource, int textViewResourceId, IList objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }

        public TaskAdapter(Context context, int resource, int textViewResourceId, Object[] objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }

        public TaskAdapter(Context context, int resource, IList objects) : base(context, resource, objects)
        {
        }

        public TaskAdapter(Context context, int resource, Object[] objects) : base(context, resource, objects)
        {
        }


        public Task GetTask(int position)
        {
            return _taskList[position];
        }
    }
}