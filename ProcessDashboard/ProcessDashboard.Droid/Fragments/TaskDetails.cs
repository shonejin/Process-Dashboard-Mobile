
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;
namespace ProcessDashboard.Droid.Fragments
{
    public class TaskDetails : Fragment
    {
        public string TaskId;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("Task Details");
            // Create your fragment here
            
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("Task Details");
        }

        public void SetId(string id)
        {
            TaskId = id;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.TaskDetail, container, false);

            AddData(v);


            return v;
        }

        private async void AddData(View view)
        {

            Task t = await (((MainActivity)(Activity)).Ctrl).GetTask("mock", TaskId);

            if (t == null)
            {
                System.Diagnostics.Debug.WriteLine("T is null");
            }

            TextView projectName = view.FindViewById<TextView>(Resource.Id.TaskDetails_ProjectName);
            projectName.Text = t.Project.Name;


            TextView taskName = view.FindViewById<TextView>(Resource.Id.TaskDetails_TaskName);
            taskName.Text = t.FullName;

            TextView tdCompleteTextView = view.FindViewById<TextView>(Resource.Id.TaskDetails_CompletionDate);
            tdCompleteTextView.Text =  ""+t.CompletionDate;


            TextView tdPlannedTimeTextView = view.FindViewById<TextView>(Resource.Id.TaskDetails_PlannedTime);


            tdPlannedTimeTextView.Text = "" + TimeSpan.FromMinutes(t.EstimatedTime).ToString(@"hh\:mm");

            TextView tdActualTimeTextView = view.FindViewById<TextView>(Resource.Id.TaskDetails_ActualTime);
            tdActualTimeTextView.Text = "" + TimeSpan.FromMinutes(t.ActualTime).ToString(@"hh\:mm");


            EditText a = view.FindViewById<EditText>(Resource.Id.TaskDetails_Notes);
            if (t.TaskNote == null || t.TaskNote.Length == 0)
            {
                a.Text = "-";
            }
            else
                a.Text = t.TaskNote;


            Button timeLogs = view.FindViewById<Button>(Resource.Id.TaskDetails_TimeLogButton);
            timeLogs.Click += (sender, args) =>
            {
               ((MainActivity)(this.Activity)).PassTimeLogInfo(t.Id);
            };


        }

       
    }
}