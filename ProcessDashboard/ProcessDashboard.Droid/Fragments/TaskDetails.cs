#region
using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using ProcessDashboard.Droid.Adapter;
using ProcessDashboard.DTO;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class TaskDetails : Fragment
    {
        private double? _actualTime;
        private DateTime? _completionDate;
        private double? _estimatedTime;
        private string _projectName;
        private string _taskId;
        private string _taskName;
        private Activity _mActivity;
        private Home.myBroadCastReceiver onNotice;
        private IntentFilter iff;

        private Button play, pause;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            _mActivity = (MainActivity)Activity;
            // Create your fragment here
            ((MainActivity)Activity).SetTitle("Task Details");
            // Create your fragment here
        }

        public override void OnPause()
        {
            base.OnPause();
            LocalBroadcastManager.GetInstance(Activity).UnregisterReceiver(onNotice);

        }


        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)Activity).SetTitle("Task Details");
            iff = new IntentFilter("processdashboard.timelogger");
            onNotice = new Home.myBroadCastReceiver((MainActivity)this.Activity);
            LocalBroadcastManager.GetInstance(Activity).RegisterReceiver(onNotice, iff);
        }

        public void SetId(string id, string taskName, string projectName, DateTime? completionDate,
            double? estimatedTime, double? actualTime)
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
            var v = inflater.Inflate(Resource.Layout.TaskDetail, container, false);

            AddData(v);

            return v;
        }

        public void ModifyPlayPauseState(bool isPlaying)
        {
            if (isPlaying)
            {
                play.SetBackgroundResource(Resource.Drawable.play_activated);
                pause.SetBackgroundResource(Resource.Drawable.pause_deactivated);
            }
            else
            {
                play.SetBackgroundResource(Resource.Drawable.play_deactivated);
                pause.SetBackgroundResource(Resource.Drawable.pause_activated);
            }
        }


        private async void AddData(View view)
        {
            var projectName = view.FindViewById<TextView>(Resource.Id.TaskDetails_ProjectName);
            var taskName = view.FindViewById<TextView>(Resource.Id.TaskDetails_TaskName);
            var notes = view.FindViewById<EditText>(Resource.Id.TaskDetails_Notes);
            var timeinfo = view.FindViewById<ListView>(Resource.Id.TaskDetails_TimeInfo);
            play = view.FindViewById<Button>(Resource.Id.TaskDetails_Play);
            pause = view.FindViewById<Button>(Resource.Id.TaskDetails_Pause);



        

            Debug.WriteLine("We are in the begining ");

            var pb = new ProgressDialog(_mActivity) { Indeterminate = true };
            pb.SetTitle("Loading");
            pb.SetCanceledOnTouchOutside(false);
            if (_taskName != null)
                taskName.Text = _taskName;

            if (_projectName != null)
                projectName.Text = _projectName;

            Debug.WriteLine(" 0 ");
            Entry[] output = new Entry[3];

            output[0] = new Entry();
            output[1] = new Entry();
            output[2] = new Entry();

            output[0].name = "Planned Time";

            if (_estimatedTime.HasValue)
            {
                Debug.WriteLine("We have a value in estimated time");
                output[0].value = "" + TimeSpan.FromMinutes(_estimatedTime.Value).ToString(@"hh\:mm");
            }
            else
            {
                Debug.WriteLine("No value in estimated time");
                output[0].value = "";
            }
            Debug.WriteLine(" 1 ");


            output[1].name = "Actual Time";
            if (_actualTime.HasValue)
                output[1].value = "" + TimeSpan.FromMinutes(_actualTime.Value).ToString(@"hh\:mm");
            else
                output[1].value = "";

            output[2].name = "Completion Date";
            Debug.WriteLine(" 2 ");
            if (_completionDate.HasValue)

                output[2].value = Util.GetInstance().GetLocalTime(_completionDate.Value).ToShortDateString();
            else
                output[2].value = "-";

            var listAdapter = new TaskDetailsAdapter(Activity, Resource.Layout.TimeLogEntryListItem,
                   output);


            Debug.WriteLine("We have reached the end ");

            timeinfo.Adapter = listAdapter;

            pb.Show();
            Task taskDetail = null;
            try

            {
                // Get data from server
                taskDetail = await ((MainActivity)Activity).Ctrl.GetTask(Settings.GetInstance().Dataset, _taskId);

                play.Click += (sender, args) =>
                {
                    Debug.WriteLine("Play Clicked");

                    //var timerServiceIntent = new Intent("com.tumasolutions.processdashboard.TimerService");

                    //var timerServiceConnection = new TimerServiceConnection((MainActivity)this.Activity);

                    //Activity.ApplicationContext.BindService(timerServiceIntent, timerServiceConnection, Bind.AutoCreate);
                    Intent intent = new Intent(Activity, typeof(TimerService));
                    intent.PutExtra("taskId", taskDetail.Id);
                    Activity.StartService(intent);

                };



                pause.Click += (sender, args) =>
                {
                    Debug.WriteLine("Pause Clicked");
                    Activity.StopService(new Intent(Activity, typeof(TimerService)));
                    Toast.MakeText(this.Activity, "Time Log Entry Saved", ToastLength.Short).Show();

                };
            }
            catch (CannotReachServerException)
            {
                if (pb.IsShowing)
                    pb.Dismiss();

                Toast.MakeText(Activity, "Please check your internet connection and try again.", ToastLength.Long).Show();
                Debug.WriteLine("We are going to pop backstack");
                ((MainActivity)Activity).FragmentManager.PopBackStack();
            }
            catch (StatusNotOkayException)
            {
                if (pb.IsShowing)
                    pb.Dismiss();

                Toast.MakeText(Activity, "An error occured. Please try again.", ToastLength.Short).Show();
                Debug.WriteLine("We are going to pop backstack");
                ((MainActivity)Activity).FragmentManager.PopBackStack();
            }
            catch (Exception)
            {
                // For any other weird exceptions
                if (pb.IsShowing)
                    pb.Dismiss();
                Toast.MakeText(Activity, "Invalid values. Please try again.", ToastLength.Short).Show();
                Debug.WriteLine("We are going to pop backstack");
                ((MainActivity)Activity).FragmentManager.PopBackStack();
            }
            if (taskDetail == null)
            {
                Debug.WriteLine("T is null");
            }

            if (taskDetail != null)
            {
                projectName.Text = taskDetail.Project.Name;

                timeinfo.ItemClick += (sender, args) =>
                {

                    if (args.Position == 0)
                    {

                        LinearLayout LL = new LinearLayout(Activity);
                        LL.Orientation = (Orientation.Horizontal);

                        NumberPicker aNumberPicker = new NumberPicker(Activity);
                        aNumberPicker.MaxValue = (100);
                        aNumberPicker.MinValue = (0);

                        double temp;

                        temp = taskDetail.EstimatedTime;


                        aNumberPicker.Value = TimeSpan.FromMinutes(temp).Hours;

                        NumberPicker aNumberPickerA = new NumberPicker(Activity)
                        {
                            MaxValue = (59),
                            MinValue = (0),
                            Value = TimeSpan.FromMinutes(temp).Minutes
                        };


                        LinearLayout.LayoutParams parameters = new LinearLayout.LayoutParams(50, 50);
                        parameters.Gravity = GravityFlags.Center;

                        LinearLayout.LayoutParams numPicerParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                        numPicerParams.Weight = 1;

                        LinearLayout.LayoutParams qPicerParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                        qPicerParams.Weight = 1;

                        LL.LayoutParameters = parameters;
                        LL.AddView(aNumberPicker, numPicerParams);
                        LL.AddView(aNumberPickerA, qPicerParams);

                        //((TaskDetailsAdapter)(timeinfo.Adapter)).GetEntry()


                        //var ts = DateTime.ParseExact("", "HH.mm", CultureInfo.InvariantCulture);

                        AlertDialog.Builder np = new AlertDialog.Builder(Activity).SetView(LL);

                        np.SetTitle("Update Planned Time");
                        np.SetNegativeButton("Cancel", (s, a) =>
                        {
                            np.Dispose();
                        });
                        np.SetPositiveButton("Ok", (s, a) =>
                        {

                            //Update Planned Time
                            string number = aNumberPicker.Value.ToString("D2") + ":" + aNumberPickerA.Value.ToString("D2");
                            Debug.WriteLine(number);
                            double val = Convert.ToDouble(TimeSpan.ParseExact(number, @"hh\:mm", CultureInfo.InvariantCulture).TotalMinutes);
                            Debug.WriteLine("The updated val is :" + val);
                            try
                            {
                                ((MainActivity)(Activity)).Ctrl.UpdateATask(Settings.GetInstance().Dataset,
                                    _taskId, val, null, false);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }
                            output[0].value = TimeSpan.FromMinutes(val).ToString(@"hh\:mm");

                            listAdapter = new TaskDetailsAdapter(Activity, Resource.Layout.TimeLogEntryListItem,
                  output);
                            Debug.WriteLine("We have changed content ");
                            timeinfo.Adapter = listAdapter;

                            Toast.MakeText(_mActivity, "Planned Time Updated", ToastLength.Short).Show();
                            np.Dispose();

                        });
                        np.Show();
                        //Planned Time
                    }
                    else if (args.Position == 1)
                    {
                        //Actual Time
                        ((MainActivity)Activity).PassTimeLogInfo(taskDetail.Id, taskDetail.Project.Name,
                               taskDetail.FullName);
                    }
                    else if (args.Position == 2)
                    {
                        // Completion Date


                        DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                        {
                            Debug.WriteLine("The received date is :" + time.ToShortDateString());

                            output[2].value = time.ToShortDateString();

                            listAdapter = new TaskDetailsAdapter(Activity, Resource.Layout.TimeLogEntryListItem,
                  output);
                            Debug.WriteLine("We have changed content ");
                            timeinfo.Adapter = listAdapter;

                            try
                            {
                                ((MainActivity)(Activity)).Ctrl.UpdateATask(Settings.GetInstance().Dataset,
                                    _taskId, null, Util.GetInstance().GetServerTime(time), false);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }


                            Toast.MakeText(_mActivity, "Completion Date Updated", ToastLength.Short).Show();
                        });
                        //frag.StartTime = DateTime.SpecifyKind(DateTime.Parse(""+output[2].value), DateTimeKind.Local);

                        if(taskDetail.CompletionDate.HasValue)
                            frag.StartTime = Util.GetInstance().GetLocalTime(taskDetail.CompletionDate.Value);
                        Debug.WriteLine(frag.StartTime);
                        frag.Show(FragmentManager, DatePickerFragment.TAG);


                    }

                };

                taskName.Text = taskDetail.FullName;
                output[0].value = TimeSpan.FromMinutes(taskDetail.EstimatedTime).ToString(@"hh\:mm");
                output[1].value = TimeSpan.FromMinutes(taskDetail.ActualTime).ToString(@"hh\:mm");
                output[2].value = taskDetail.CompletionDate.HasValue ? Util.GetInstance().GetLocalTime(taskDetail.CompletionDate.Value).ToShortDateString() : "-";
                listAdapter = new TaskDetailsAdapter(Activity, Resource.Layout.TimeLogEntryListItem,
                     output);


                Debug.WriteLine("We have changed content ");

                timeinfo.Adapter = listAdapter;
                if (string.IsNullOrEmpty(taskDetail.Note))
                {
                    notes.Text = "-";
                    notes.Gravity = GravityFlags.Center;

                }
                else
                    notes.Text = taskDetail.Note;

                var timeLogs = view.FindViewById<Button>(Resource.Id.TaskDetails_TimeLogButton);
                timeLogs.Click +=
                    (sender, args) =>
                    {
                        ((MainActivity)Activity).PassTimeLogInfo(taskDetail.Id, taskDetail.Project.Name,
                            taskDetail.FullName);
                    };

                var taskComplete = view.FindViewById<CheckBox>(Resource.Id.TaskDetails_TaskComplete);

                if (taskDetail.CompletionDate.HasValue && taskDetail.CompletionDate.Value != DateTime.MinValue)
                {
                    taskComplete.Checked = true;
                }
                else
                    taskComplete.Checked = false;
               
            }
            if(pb.IsShowing) 
            pb.Dismiss();

            // Dismiss Dialog


        }


    }
}