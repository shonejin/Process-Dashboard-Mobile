
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;
using System;
using System.Threading.Tasks;

namespace ProcessDashboard.Droid.Fragments
{
    public class TimeLogDetail : Fragment
    {
        private string _projectName;
        private string _taskName;
        private string _taskId;
        private TimeLogEntry _timeLog;

        private TextView tn, pn;
        private EditText interruptTime, comment, startTime;

        public void SetData(string projectName, string taskName, string taskId,TimeLogEntry timeLog)
        {
            this._projectName = projectName;
            this._taskName = taskName;
            this._timeLog = timeLog;
            this._taskId = taskId;

            System.Diagnostics.Debug.WriteLine("PN :"+_projectName);
            System.Diagnostics.Debug.WriteLine("TN :" + _taskName);
            System.Diagnostics.Debug.WriteLine("timeLog :" + _timeLog==null);
            System.Diagnostics.Debug.WriteLine("Time log ; "+_timeLog);
            System.Diagnostics.Debug.WriteLine("Task ID :" + _taskId);

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine("Time Log Detail : " + "We are in On create");
          //  RetainInstance = true;
            SetHasOptionsMenu(true);
            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("Time Log Detail");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine("Time Log Detail : " + "We are in On Resume");
            ((MainActivity)(this.Activity)).setTitle("Time Log Detail "+new Random().Next());
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            inflater.Inflate(Resource.Menu.TimelogMenu, menu);


        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {

                case Resource.Id.TimeLogSave:

                    // Do Activity menu item stuff here

                    //TODO: Start Date is a string\
                    processEntries();
                    return true;
                default:
                    return false;
                    break;
            }

            return false;


        }


        public async void processEntries()
        {
            if (_timeLog == null)
            {
                System.Diagnostics.Debug.WriteLine("This is a new entry");
                await ((MainActivity) Activity).Ctrl.AddATimeLog(Settings.GetInstance().Dataset, comment.Text,
                    startTime.Text, _taskId, 0, Double.Parse(interruptTime.Text), false);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("This is an update operation");
                //TODO: TimeLog Id is integer??
                await ((MainActivity)Activity).Ctrl.UpdateTimeLog(Settings.GetInstance().Dataset, ""+_timeLog.Id,comment.Text,
                    startTime.Text, _taskId, 0, Double.Parse(interruptTime.Text), false);
            }
            System.Diagnostics.Debug.WriteLine("We are going to pop backstack");
            ((MainActivity)Activity).FragmentManager.PopBackStack();

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("Time Log Detail : "+"We are in On create view");
            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.TimeLogEntry, container, false);

            tn = v.FindViewById<TextView>(Resource.Id.TimeLogEdit_TaskName);
            pn = v.FindViewById<TextView>(Resource.Id.TimeLogEdit_ProjectName);
            interruptTime = v.FindViewById<EditText>(Resource.Id.TimeLogEdit_InterruptTime);
            comment = v.FindViewById<EditText>(Resource.Id.TimeLogEdit_Comment);
            startTime = v.FindViewById<EditText>(Resource.Id.TimeLogEdit_StartTime);

            if (_taskName != null && _projectName != null)
            {
                tn.Text = _taskName + new Random().Next();
                pn.Text = _projectName + new Random().Next();
            }

            if (_timeLog == null) return v;
            //TODO: DateTime check
            System.Diagnostics.Debug.WriteLine("Reached here");
            System.Diagnostics.Debug.WriteLine(_timeLog.StartDate.ToLocalTime().ToString());


            startTime.SetText("Testing " + new Random().Next(),TextView.BufferType.Editable);
            ///startTime.Text = 
          //  startTime.Text = _timeLog.StartDate.ToLocalTime().ToString();
            
            interruptTime.Text = "" + _timeLog.InterruptTime+new Random().Next();
            comment.Text = _timeLog.Comment;
            System.Diagnostics.Debug.WriteLine("Reached end");
            return v;
        }


    }
}