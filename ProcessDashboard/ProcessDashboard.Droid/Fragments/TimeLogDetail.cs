#region
using System;
using System.Globalization;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class TimeLogDetail : Fragment
    {
        private EditText _interruptTime, _comment, _startTime;
        private string _projectName;
        private string _taskId;
        private string _taskName;
        private TimeLogEntry _timeLog;

        private TextView _tn, _pn;

        public void SetData(string projectName, string taskName, string taskId, TimeLogEntry timeLog)
        {
            _projectName = projectName;
            _taskName = taskName;
            _timeLog = timeLog;
            _taskId = taskId;

            Debug.WriteLine("PN :" + _projectName);
            Debug.WriteLine("TN :" + _taskName);

            Debug.WriteLine("Time log ; " + _timeLog);
            Debug.WriteLine("Task ID :" + _taskId);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Debug.WriteLine("Time Log Detail : " + "We are in On create");
            //  RetainInstance = true;
            SetHasOptionsMenu(true);
            // Create your fragment here
            ((MainActivity) Activity).SetTitle("Time Log Detail");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            Debug.WriteLine("Time Log Detail : " + "We are in On Resume");
            ((MainActivity) Activity).SetTitle("Time Log Detail " + new Random().Next());
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
                    ProcessEntries();
                    return true;
                default:
                    return false;
            }
        }

        public async void ProcessEntries()
        {
            //TODO: Change UTCNow to the time specified in the text field
            if (_timeLog == null)
            {
                Debug.WriteLine("This is a new entry");
                await ((MainActivity) Activity).Ctrl.AddATimeLog(Settings.GetInstance().Dataset, _comment.Text,
                    DateTime.UtcNow, _taskId, 0, double.Parse(_interruptTime.Text), false);
            }
            else
            {
                Debug.WriteLine("This is an update operation");
                //TODO: TimeLog Id is integer??
                await
                    ((MainActivity) Activity).Ctrl.UpdateTimeLog(Settings.GetInstance().Dataset, "" + _timeLog.Id,
                        _comment.Text,
                        DateTime.UtcNow, _taskId, 0, double.Parse(_interruptTime.Text), false);
            }
            Debug.WriteLine("We are going to pop backstack");
            ((MainActivity) Activity).FragmentManager.PopBackStack();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Debug.WriteLine("Time Log Detail : " + "We are in On create view");
            // Use this to return your custom view for this Fragment
            var v = inflater.Inflate(Resource.Layout.TimeLogEntry, container, false);

            _tn = v.FindViewById<TextView>(Resource.Id.TimeLogEdit_TaskName);
            _pn = v.FindViewById<TextView>(Resource.Id.TimeLogEdit_ProjectName);
            _interruptTime = v.FindViewById<EditText>(Resource.Id.TimeLogEdit_InterruptTime);
            _comment = v.FindViewById<EditText>(Resource.Id.TimeLogEdit_Comment);
            _startTime = v.FindViewById<EditText>(Resource.Id.TimeLogEdit_StartTime);

            if (_taskName != null && _projectName != null)
            {
                _tn.Text = _taskName + new Random().Next();
                _pn.Text = _projectName + new Random().Next();
            }

            if (_timeLog == null) return v;
            //TODO: DateTime check
            Debug.WriteLine(_timeLog.StartDate.ToLocalTime().ToString(CultureInfo.CurrentCulture));

            _startTime.SetText("Testing " + new Random().Next(), TextView.BufferType.Editable);

            // startTime.Text = _timeLog.StartDate.ToLocalTime().ToString();

            _interruptTime.Text = "" + _timeLog.InterruptTime + new Random().Next();
            _comment.Text = _timeLog.Comment;
            Debug.WriteLine("Reached end");
            return v;
        }
    }
}