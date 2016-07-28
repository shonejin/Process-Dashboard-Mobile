using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;

namespace ProcessDashboard.Droid.Fragments
{
    public class TaskDetails : Fragment
    {
        private string _taskId;
        private string _taskName;
        private string _projectName;
        private DateTime? _completionDate;
        private double? _estimatedTime;
        private double? _actualTime;

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

        public void SetId(string id, string taskName, string projectName, DateTime? completionDate, double? estimatedTime, double? actualTime)
        {
            _taskId = id;
            _taskName = taskName;
            _projectName = projectName;
            _completionDate = completionDate;
            _estimatedTime = estimatedTime;
            _actualTime = actualTime;


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

            TextView projectName = view.FindViewById<TextView>(Resource.Id.TaskDetails_ProjectName);
            TextView taskName = view.FindViewById<TextView>(Resource.Id.TaskDetails_TaskName);
            TextView tdCompleteTextView = view.FindViewById<TextView>(Resource.Id.TaskDetails_CompletionDate);
            TextView tdPlannedTimeTextView = view.FindViewById<TextView>(Resource.Id.TaskDetails_PlannedTime);
            TextView tdActualTimeTextView = view.FindViewById<TextView>(Resource.Id.TaskDetails_ActualTime);
            EditText notes = view.FindViewById<EditText>(Resource.Id.TaskDetails_Notes);


            if (_taskName != null)
                taskName.Text = _taskName;

            if (_projectName != null)
                projectName.Text = _projectName;
            if (_completionDate.HasValue)
                //TODO: Check if shortDateString is appropriate in this situation
                tdCompleteTextView.Text = _completionDate.Value.ToLocalTime().ToShortDateString();

            if (_estimatedTime.HasValue)
                tdPlannedTimeTextView.Text = ""+_estimatedTime.Value;

            if (_actualTime.HasValue)
                tdActualTimeTextView.Text = "" + _actualTime.Value;
            // Get data from server
            Task taskDetail = await (((MainActivity)(Activity)).Ctrl).GetTask(Settings.GetInstance().Dataset, _taskId);

            if (taskDetail == null)
            {
                System.Diagnostics.Debug.WriteLine("T is null");
            }
            projectName.Text = taskDetail.Project.Name;

            taskName.Text = taskDetail.FullName;

            tdCompleteTextView.Text =  ""+taskDetail.CompletionDate;

            tdPlannedTimeTextView.Text = "" + TimeSpan.FromMinutes(taskDetail.EstimatedTime).ToString(@"hh\:mm");

            tdActualTimeTextView.Text = "" + TimeSpan.FromMinutes(taskDetail.ActualTime).ToString(@"hh\:mm");

            if (taskDetail.TaskNote == null || taskDetail.TaskNote.Length == 0)
            {
                notes.Text = "-";
                notes.TextAlignment = TextAlignment.Center;
                
            }
            else
                notes.Text = taskDetail.TaskNote;


            Button timeLogs = view.FindViewById<Button>(Resource.Id.TaskDetails_TimeLogButton);
            timeLogs.Click += (sender, args) =>
            {
               ((MainActivity)(this.Activity)).PassTimeLogInfo(taskDetail.Id,taskDetail.Project.Name,taskDetail.FullName);
            };


            CheckBox taskComplete = view.FindViewById<CheckBox>(Resource.Id.TaskDetails_TaskComplete);

            if (taskDetail.CompletionDate!=null && taskDetail.CompletionDate != DateTime.MinValue)
            {
                taskComplete.Checked = true;
            }
            taskComplete.CheckedChange += (sender, args) =>
            {
                string text = "";
                if (args.IsChecked)
                {
                    // Mark a task as complete
                    taskDetail.CompletionDate = DateTime.Now;
                    text = "Task Marked Complete";
                }
                else
                {
                    // Unmark the task 
                    taskDetail.CompletionDate = DateTime.MinValue;
                    text = "Task Marked Incomplete";
                }
                Toast.MakeText(this.Activity, text, ToastLength.Short).Show();
                // await (((MainActivity)(Activity)).Ctrl).UpdateTimeLog(Settings.GetInstance().Dataset,)

            };

        }

       
    }
}