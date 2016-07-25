
using System;
using System.Collections;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Text;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid.Fragments
{
    public class TaskTimeLogDetail : ListFragment
    {

        public string Id { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("Task Timelog Detail");
            // Create your fragment here

        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("Task Timelog Detail");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.TaskTimeLogDetail, container, false);
            GetData(v);
            return v;
        }


        public async void GetData(View v)
        {
            Controller ctrl = ((MainActivity)(this.Activity)).Ctrl;
            var timelogs = await ctrl.GetTimeLog(Settings.GetInstance().Dataset, null, null, null, Id, null);
            System.Diagnostics.Debug.WriteLine("Got data for timelogs");
            foreach (var timelog in timelogs)
            {
                System.Diagnostics.Debug.WriteLine(timelog);
            }


            TaskTimeLogAdapter listAdapter = new TaskTimeLogAdapter(Activity, Android.Resource.Layout.SimpleListItem1, timelogs.ToArray(), 1);
            this.ListAdapter = listAdapter;

            TextView pn = v.FindViewById<TextView>(Resource.Id.TaskTimeLog_ProjectName);
            TextView tn = v.FindViewById<TextView>(Resource.Id.TaskTimeLog_TaskName);

            pn.Text = timelogs[0].Task.Project.Name;
            tn.Text = timelogs[0].Task.FullName;

        }
    }


    public class TaskTimeLogAdapter : ArrayAdapter
    {
        readonly TimeLogEntry[] TimeLogEntries;

        public TaskTimeLogAdapter(Context context, int resource, TimeLogEntry[] objects, int flag) : base(context, resource, objects)
        {
            System.Diagnostics.Debug.WriteLine("We are in the right constructor");
            TimeLogEntries = objects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = base.GetView(position, convertView, parent);
           
            return v;
        }

        public TimeLogEntry GetProject(int position)
        {
            return TimeLogEntries[position];
        }
    }
}