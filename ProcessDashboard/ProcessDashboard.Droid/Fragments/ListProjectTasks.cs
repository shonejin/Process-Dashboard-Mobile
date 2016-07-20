
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
        private string _projectId;

        public ListProjectTasks (string projectId)
        {
            this._projectId = projectId;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("List of Tasks");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("List of Tasks");
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            SetId(_projectId);
            ListView listView = this.ListView;
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                TaskAdapter ta = (TaskAdapter)listView.Adapter;
                Task p = ta.GetTask(e.Position);
                string taskId = p.id;
                //  ((MainActivity)this.Activity).switchToFragment(MainActivity.fragmentTypes.taskdetails);
                ((MainActivity)this.Activity).passTaskDetailsInfo(taskId);
                //Project p = listView.GetItemAtPosition(e.Position);

            };
            AddData(((MainActivity)this.Activity)._ctrl, _projectId);


        }

        public void SetId(string projectId)
        {
            this._projectId = projectId;
        }

        
        

        private async void AddData(Controller ctrl,string projectId)
        {
            List<Task> output = await ctrl.GetTasks("mock",projectId);
            TaskAdapter listAdapter = new TaskAdapter(Activity, Android.Resource.Layout.SimpleListItem1,output.ToArray(),1,Activity);
            ListView.Adapter = listAdapter;
            SetListShown(true);
        }


        public void LoadDummyData()
        {
            string[] values = { "Sample Task", "Component 1 / Component 2 / Code", "... / head truncation" };
            this.ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleExpandableListItem1, values);
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
        readonly Task[] _taskList;

        public Activity Activity1 { get; }

        public TaskAdapter(Context context, int resource, Task[] objects, int flag, Activity activity) : base(context, resource, objects)
        {
            System.Diagnostics.Debug.WriteLine("We are in the right constructor");
            _taskList = objects;
            Activity1 = activity;
        }

        public TaskAdapter(IntPtr javaReference, JniHandleOwnership transfer, Activity activity) : base(javaReference, transfer)
        {
            Activity1 = activity;
        }

        public TaskAdapter(Context context, int resource, Activity activity) : base(context, resource)
        {
            Activity1 = activity;
        }

        public TaskAdapter(Context context, int resource, int textViewResourceId, Activity activity)
            : base(context, resource, textViewResourceId)
        {
            Activity1 = activity;
        }

        public TaskAdapter(Context context, int resource, int textViewResourceId, IList objects, Activity activity)
            : base(context, resource, textViewResourceId, objects)
        {
            Activity1 = activity;
        }

        public TaskAdapter(Context context, int resource, int textViewResourceId, Object[] objects, Activity activity)
            : base(context, resource, textViewResourceId, objects)
        {
            Activity1 = activity;
        }

        public TaskAdapter(Context context, int resource, IList objects, Activity activity) : base(context, resource, objects)
        {
            Activity1 = activity;
        }

        public TaskAdapter(Context context, int resource, Object[] objects, Activity activity) : base(context, resource, objects)
        {
            Activity1 = activity;
        }


        public Task GetTask(int position)
        {
            return _taskList[position];
        }
    }
}