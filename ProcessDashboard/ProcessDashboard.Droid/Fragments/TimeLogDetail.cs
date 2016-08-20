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
        private EditText _interruptTime, _comment, _startTime, _deltaTime;
        private string _projectName;
        private string _taskId;
        private string _taskName;
        private TimeLogEntry _timeLog;

        private double _oldLoggedTime;
        private double _oldInterruptTime;

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
            ((MainActivity)Activity).SetTitle("Time Log Detail");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            Debug.WriteLine("Time Log Detail : " + "We are in On Resume");
            ((MainActivity)Activity).SetTitle("Time Log Detail ");
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
                case Resource.Id.TimeLog_Save:
                    ProcessEntries();
                    return true;
                case Resource.Id.TimeLog_Delete:
                    DeleteCurrentEntry();
                    return true;
                default:
                    return false;
            }
        }

        public async void DeleteCurrentEntry()
        {
            ProgressDialog pd = new ProgressDialog(Activity);
            pd.Indeterminate = true;
            pd.SetCancelable(false);
            pd.SetMessage("Deleting");
            try
            {
              
                await ((MainActivity)Activity).Ctrl.DeleteTimeLog(Settings.GetInstance().Dataset, "" + _timeLog.Id);
                pd.Dismiss();
                Toast.MakeText(Activity, "Deleted the timelog entry", ToastLength.Short).Show();
                Debug.WriteLine("We are going to pop backstack");
                ((MainActivity)Activity).FragmentManager.PopBackStack();
            }
            catch (CannotReachServerException)
            {
                if (pd.IsShowing)
                    pd.Dismiss();
                Toast.MakeText(Activity, "Please check your internet connection and try again.", ToastLength.Long).Show();
                Debug.WriteLine("We are going to pop backstack");
                ((MainActivity)Activity).FragmentManager.PopBackStack();
            }
            catch (StatusNotOkayException)
            {
                if (pd.IsShowing)
                    pd.Dismiss();
                Toast.MakeText(Activity, "An error occured. Please try again.", ToastLength.Short).Show();
                Debug.WriteLine("We are going to pop backstack");
                ((MainActivity)Activity).FragmentManager.PopBackStack();
            }
            catch (Exception)
            {
                if (pd.IsShowing)
                    pd.Dismiss();
                // For any other weird exceptions
                Toast.MakeText(Activity, "Invalid operation. Please try again.", ToastLength.Short).Show();
                Debug.WriteLine("We are going to pop backstack");
                ((MainActivity)Activity).FragmentManager.PopBackStack();
            }
        }

        public async void ProcessEntries()
        {
            ProgressDialog pd = new ProgressDialog(Activity);
            pd.SetMessage("Saving..");
            pd.Indeterminate = true;

            if (_timeLog == null)
            {
                Debug.WriteLine("This is a new entry");
                try
                {
                    var newDate = DateTime.SpecifyKind(DateTime.Parse(_startTime.Text), DateTimeKind.Local);


                    double val =
                        Convert.ToDouble(
                            TimeSpan.ParseExact(_deltaTime.Text, @"hh\:mm", CultureInfo.InvariantCulture).TotalMinutes);
                    double val2 =
                        Convert.ToDouble(
                            TimeSpan.ParseExact(_interruptTime.Text, @"hh\:mm", CultureInfo.InvariantCulture)
                                .TotalMinutes);
                    pd.Show();
                    await
                        ((MainActivity)Activity).Ctrl.AddATimeLog(Settings.GetInstance().Dataset, _comment.Text,
                            newDate
                            , _taskId, val, val2, false);
                    pd.Dismiss();
                    Toast.MakeText(Activity, "Added new Timelog Entry", ToastLength.Short).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }
                catch (CannotReachServerException)
                {
                    if (pd.IsShowing)
                        pd.Dismiss();
                    Toast.MakeText(Activity, "Please check your internet connection and try again.", ToastLength.Long).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }
                catch (StatusNotOkayException)
                {
                    if (pd.IsShowing)
                        pd.Dismiss();
                    Toast.MakeText(Activity, "An error occured. Please try again.", ToastLength.Short).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }
                catch (Exception)
                {
                    if (pd.IsShowing)
                        pd.Dismiss();
                    // For any other weird exceptions
                    Toast.MakeText(Activity, "Invalid values. Please try again.", ToastLength.Short).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }

            }
            else
            {
                Debug.WriteLine("This is an update operation");
                try
                {

                    var newDate = DateTime.SpecifyKind(DateTime.Parse(_startTime.Text), DateTimeKind.Local);
                    double val = Convert.ToDouble(TimeSpan.ParseExact(_deltaTime.Text, @"hh\:mm", CultureInfo.InvariantCulture).TotalMinutes);
                    var updatedLoggedTime = val - _oldLoggedTime;
                    val = Convert.ToDouble(TimeSpan.ParseExact(_interruptTime.Text, @"hh\:mm", CultureInfo.InvariantCulture).TotalMinutes);
                    var updatedInterruptTime = val - _oldInterruptTime;

                    pd.Show();
                    await
                        ((MainActivity)Activity).Ctrl.UpdateTimeLog(Settings.GetInstance().Dataset,
                            "" + _timeLog.Id,
                            _comment.Text,
                            newDate, _taskId, updatedLoggedTime, updatedInterruptTime, false);


                    pd.Dismiss();
                    Toast.MakeText(Activity, "Updated the Timelog Entry", ToastLength.Short).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }
                catch (CannotReachServerException)
                {
                    if(pd.IsShowing)
                        pd.Dismiss();
                    Toast.MakeText(Activity, "Please check your internet connection and try again.", ToastLength.Long).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }
                catch (StatusNotOkayException)
                {
                    if (pd.IsShowing)
                        pd.Dismiss();
                    Toast.MakeText(Activity, "An error occured. Please try again.", ToastLength.Short).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }
                catch (Exception)
                {
                    if (pd.IsShowing)
                        pd.Dismiss();
                    // For any other weird exceptions
                    Toast.MakeText(Activity, "Invalid values. Please try again.", ToastLength.Short).Show();
                    Debug.WriteLine("We are going to pop backstack");
                    ((MainActivity)Activity).FragmentManager.PopBackStack();
                }
            }

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
            _deltaTime = v.FindViewById<EditText>(Resource.Id.TimeLogEdit_DeltaTime);


            _startTime.Click += (sender, args) =>
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    //TODO: DEBUG: Check whether this is causing any issues.
                    _startTime.Text = time.ToString(CultureInfo.InvariantCulture);
                    Toast.MakeText(Activity, "Start Date Updated", ToastLength.Short).Show();
                    TimePickerFragment frag2 = TimePickerFragment.NewInstance(delegate (int hour, int min)
                    {
                        Toast.MakeText(Activity, "Start Time Updated", ToastLength.Short).Show();
                        DateTime output = new DateTime(time.Year, time.Month, time.Day, hour, min, 0);
                        _startTime.Text = output.ToString("g");

                    });

                    frag2.chosenDate = time;
                    if (_timeLog != null)
                    {
                        frag2.StartHour = _timeLog.StartDate.Hour;
                        frag2.StartMinute = _timeLog.StartDate.Minute;
                    }
                    else
                    {
                        frag2.StartHour = time.Hour;
                        frag2.StartMinute = time.Minute;
                    }

                    frag2.Show(FragmentManager, TimePickerFragment.TAG);

                });
                //frag.StartTime = DateTime.SpecifyKind(DateTime.Parse(""+output[2].value), DateTimeKind.Local);

                frag.StartTime = _timeLog != null ? _timeLog.StartDate : DateTime.Now;

                Debug.WriteLine(frag.StartTime);
                frag.Show(FragmentManager, DatePickerFragment.TAG);


            };


            if (_timeLog != null)
            {
                _oldLoggedTime = _timeLog.LoggedTime;
                _oldInterruptTime = _timeLog.InterruptTime;
            }
            else
            {
                _oldLoggedTime = 0;
                _oldInterruptTime = 0;
            }

            if (_taskName != null && _projectName != null)
            {
                _tn.Text = _taskName;
                _pn.Text = _projectName;
            }

            if (_timeLog == null) return v;

            string c = Util.GetInstance().GetLocalTime(_timeLog.StartDate).ToString("g");
            _startTime.Text = (c);
            _interruptTime.Text = "" + TimeSpan.FromMinutes(_timeLog.InterruptTime).ToString(@"hh\:mm");
            _comment.Text = _timeLog.Comment ?? "-";
            _deltaTime.Text = "" + TimeSpan.FromMinutes(_timeLog.LoggedTime).ToString(@"hh\:mm");
            return v;
        }
    }
}