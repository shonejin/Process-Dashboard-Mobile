
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid.Fragments
{
    public class TaskTimeLogList : ListFragment
    {

        private string _id { get; set; }
        private string _projectName { get; set; }
        private string _taskName { get; set; }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("Task Timelogs");
            // Create your fragment here

        }

        public void SetData(string id,string projectName, string taskName)
        {
            _id = id;
            _projectName = projectName;
            _taskName = _taskName;


        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("Task Timelogs");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.TaskTimeLogList, container, false);
            GetData(v);
            return v;
        }


        public async void GetData(View v)
        {
            Controller ctrl = ((MainActivity)(this.Activity)).Ctrl;

            TextView pn = v.FindViewById<TextView>(Resource.Id.TaskTimeLog_ProjectName);
            TextView tn = v.FindViewById<TextView>(Resource.Id.TaskTimeLog_TaskName);
            FloatingActionButton fab = v.FindViewById<FloatingActionButton>(Resource.Id.TaskTimeLog_fab);

            pn.Text = _projectName;
            tn.Text = _taskName;

            var timelogs = await ctrl.GetTimeLog(Settings.GetInstance().Dataset, null, null, null, _id, null);

            System.Diagnostics.Debug.WriteLine("Got data for timelogs :"+timelogs.Count);
            foreach (var timelog in timelogs)

            {
                System.Diagnostics.Debug.WriteLine(timelog);
            }


            TaskTimeLogAdapter listAdapter = new TaskTimeLogAdapter(Activity, Android.Resource.Layout.SimpleListItem1, timelogs.ToArray(), 1);
            ListAdapter = listAdapter;
            
            pn.Text = timelogs[0].Task.Project.Name;
            tn.Text = timelogs[0].Task.FullName;
            
            fab.Click += (sender, args) =>
            {
                if (timelogs.Count>0)
                {
                    System.Diagnostics.Debug.WriteLine("Project Name :"+timelogs[0].Task.Project.Name);
                    System.Diagnostics.Debug.WriteLine("Task Name :" + timelogs[0].Task.FullName);

                    ((MainActivity) this.Activity).TimeLogEditCallBack(timelogs[0].Task.Project.Name,timelogs[0].Task.FullName,timelogs[0].Task.Id, null);
                }
                else
                {
                    ((MainActivity) this.Activity).TimeLogEditCallBack(_projectName,_taskName,_id, null);
                }
            }; 
            fab.Show();
            
    
            this.ListView.ItemClick += (sender, args) =>
            {
                int i = args.Position;
                System.Diagnostics.Debug.WriteLine("I position :"+i);
                ((MainActivity)this.Activity).TimeLogEditCallBack(timelogs[i].Task.Project.Name, timelogs[i].Task.FullName,timelogs[i].Task.Id,timelogs[i]);

            };


        }

       
    }


    public class TaskTimeLogAdapter : ArrayAdapter
    {
        readonly TimeLogEntry[] _timeLogEntries;

        public TaskTimeLogAdapter(Context context, int resource, TimeLogEntry[] objects, int flag) : base(context, resource, objects)
        {
            System.Diagnostics.Debug.WriteLine("We are in the right constructor");
            _timeLogEntries = objects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = base.GetView(position, convertView, parent);
           
            return v;
        }

        public TimeLogEntry GetProject(int position)
        {
            return _timeLogEntries[position];
        }
    }
}