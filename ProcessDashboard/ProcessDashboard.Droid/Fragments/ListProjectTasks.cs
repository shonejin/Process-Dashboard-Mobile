
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
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
                string taskId = p.Id;
                //  ((MainActivity)this.Activity).switchToFragment(MainActivity.fragmentTypes.taskdetails);
                ((MainActivity)this.Activity).PassTaskDetailsInfo(taskId);
                //Project p = listView.GetItemAtPosition(e.Position);

            };
            AddData(((MainActivity)this.Activity).Ctrl, _projectId);


        }

        public void SetId(string projectId)
        {
            this._projectId = projectId;
        }

        
        

        private async void AddData(Controller ctrl,string projectId)
        {
            List<Task> output = await ctrl.GetTasks("mock",projectId);
            TaskAdapter listAdapter = new TaskAdapter(Activity, Android.Resource.Layout.SimpleListItem1,output.ToArray());
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

        public TaskAdapter(Context context, int resource, Task[] objects) : base(context, resource, objects)
        {
            System.Diagnostics.Debug.WriteLine("We are in the right constructor");
            _taskList = objects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = base.GetView(position, convertView, parent);
            if(!_taskList[position].CompletionDate.Equals(DateTime.MinValue))
            {
                string text = _taskList[position].FullName;
                TextView tv = v.FindViewById<TextView>(Android.Resource.Id.Text1);
                SpannableString spannable = new SpannableString(text);
                spannable.SetSpan(new StrikethroughSpan(), 0, text.Length, SpanTypes.InclusiveExclusive);
                tv.TextFormatted = spannable;
                
            }
            return v;
        }

        public Task GetTask(int position)
        {
            return _taskList[position];
        }
    }
}