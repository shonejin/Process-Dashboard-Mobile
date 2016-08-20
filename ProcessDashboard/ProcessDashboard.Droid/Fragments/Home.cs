#region
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using HockeyApp.Android;
using ProcessDashboard.Droid.Adapter;
using ProcessDashboard.SyncLogic;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class Home : ListFragment
    {
        private MainActivity _mActivity;
        private myBroadCastReceiver onNotice;
        private IntentFilter iff;
        //TODO: Remove this for production
        private AccountStorage _ase;

        private Button play, pause;

        private string _currenttaskid;
     

        public override void OnAttach(Context activity)
        {
            base.OnAttach(activity);
            _mActivity = (MainActivity) activity;

           

        }

        //Disabling compiler warnings
#pragma warning disable 672
        public override void OnAttach(Activity activity)
#pragma warning restore 672
        {
#pragma warning disable 618
            base.OnAttach(activity);
#pragma warning restore 618
            _mActivity = (MainActivity) activity;
            //
        }

        public override void OnDetach()
        {
            base.OnDetach();
            _mActivity = null;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            _mActivity.SetTitle("Process Dashboard");
            _ase = new AccountStorage();
            _ase.SetContext(this.Activity);
        }

        public override void OnPause()
        {
            base.OnPause();

            LocalBroadcastManager.GetInstance(Activity).UnregisterReceiver(onNotice);


        }

        public override void OnResume()
        {
            base.OnResume();
            iff = new IntentFilter("processdashboard.timelogger");
            

            onNotice = new myBroadCastReceiver((MainActivity)this.Activity);

            LocalBroadcastManager.GetInstance(Activity).RegisterReceiver(onNotice,iff);
            

            _mActivity.SetTitle("Process Dashboard");
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            var ta = (TaskAdapter) l.Adapter;
            var pb = ta.GetTask(position);
            _mActivity.PassTaskDetailsInfo(pb.Id, null, null, null, null, null);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            var v = inflater.Inflate(Resource.Layout.Home, container, false);
            LoadData(v, _mActivity.Ctrl);
            Debug.WriteLine("We are in oncreateView of Home screen");

            System.Diagnostics.Debug.WriteLine(_ase.UserId);

            Debug.WriteLine("*******************************");
            return v;
        }

        private async void LoadData(View v, Controller ctrl)
        {
            var pb = new ProgressDialog(_mActivity) {Indeterminate = true};
            pb.SetTitle("Loading");

            play = v.FindViewById<Button>(Resource.Id.Home_Play);

            play.Click += (sender, args) =>
            {
                Debug.WriteLine("Play Clicked");

                //var timerServiceIntent = new Intent("com.tumasolutions.processdashboard.TimerService");

                //var timerServiceConnection = new TimerServiceConnection((MainActivity)this.Activity);

                //Activity.ApplicationContext.BindService(timerServiceIntent, timerServiceConnection, Bind.AutoCreate);
                Intent intent = new Intent(Activity, typeof(TimerService));
                intent.PutExtra("taskId", _currenttaskid);
                Activity.StartService(intent);

            };

             pause = v.FindViewById<Button>(Resource.Id.Home_Pause);

            pause.Click += (sender, args) =>
            {
                Debug.WriteLine("Pause Clicked");
                Activity.StopService(new Intent(Activity, typeof(TimerService)));
                Toast.MakeText(this.Activity, "Time Log Entry Saved", ToastLength.Short).Show();

            };


            var taskComplete = v.FindViewById<CheckBox>(Resource.Id.Home_TaskComplete);
            taskComplete.CheckedChange += (sender, args) =>
            {
                string text = "";
                if (args.IsChecked)
                {
                    // Mark a task as complete
                    DateTime convertedTime = Util.GetInstance().GetServerTime(DateTime.UtcNow);
                    //taskDetail.CompletionDate = convertedTime;

                    try
                    {
                        ((MainActivity)(Activity)).Ctrl.UpdateATask(Settings.GetInstance().Dataset,
                            _currenttaskid, null, convertedTime, false);
                        text = "Task Marked Complete";

                    }
                    catch (CannotReachServerException)
                    {
                        if(pb.IsShowing)
                            pb.Dismiss();
                        Debug.WriteLine("We could not reach the server");
                        taskComplete.Checked = false;
                        text = "Please check your internet connection and try again.";
                        Toast.MakeText(this.Activity,text,ToastLength.Short).Show();
                        
                    }
                    catch (StatusNotOkayException)
                    {
                        pb.Dismiss();
                        taskComplete.Checked = false;
                        //TODO: Should we report this ?
                        text = "An error has occured. Please try again.";
                        Toast.MakeText(this.Activity, text, ToastLength.Short).Show();

                    }
                    catch (Exception e)
                    {
                        // For any other weird exceptions
                        pb.Dismiss();
                        
                        taskComplete.Checked = false;
                        // Sending to HockeyApp
                        ExceptionHandler.SaveException(Java.Lang.Throwable.FromException(e),null, null);
                        text = "Unable to make the change. Please try again.";
                        Toast.MakeText(this.Activity, text, ToastLength.Short).Show();
                    }

                    
                } else {
                    
                    // Unmark the task 
                    
                    try
                    {

                        ((MainActivity)(Activity)).Ctrl.UpdateATask(Settings.GetInstance().Dataset,
                            _currenttaskid, null, null, true);
                        text = "Task Marked Incomplete";

                    }
                    catch (CannotReachServerException)
                    {
                        if (pb.IsShowing)
                            pb.Dismiss();
                        Debug.WriteLine("We could not reach the server");
                        taskComplete.Checked = true;
                        text = "Please check your internet connection and try again.";
                        Toast.MakeText(this.Activity, text, ToastLength.Short).Show();


                    }
                    //TODO: Should we handle this ?
                    catch (StatusNotOkayException)
                    {
                        pb.Dismiss();
                        taskComplete.Checked = true;
                        text = "An error has occured. Please try again.";
                        Toast.MakeText(this.Activity, text, ToastLength.Short).Show();


                    }
                    catch (Exception e)
                    {
                        // For any other weird exceptions
                        taskComplete.Checked = true;
                        ExceptionHandler.SaveException(Java.Lang.Throwable.FromException(e), null, null);
                        text = "Unable to make the change. Please try again.";
                        Toast.MakeText(this.Activity, text, ToastLength.Short).Show();

                    }

                    
                }
                
                Debug.WriteLine("We have changed content ");
                Toast.MakeText(Activity, text, ToastLength.Short).Show();
                
            };

            var recentTask = v.FindViewById<Button>(Resource.Id.Home_RecentTask);
            recentTask.Text = "Loading..";

            var recentProject = v.FindViewById<Button>(Resource.Id.Home_CurrentProject);
            recentProject.Text = "Loading..";
            pb.SetCanceledOnTouchOutside(false);
            pb.Show();

            try
            {
                var output = await ctrl.GetRecentTasks(Settings.GetInstance().Dataset);

                pb.Dismiss();

                var recent = output[0];

                recentTask.Text = recent.FullName;
                recentProject.Text = recent.Project.Name;
                _currenttaskid = recent.Id;

                output.RemoveAt(0);

                if (_mActivity == null)
                {
                    Debug.WriteLine("Activity is null");
                }

                var listAdapter = new TaskAdapter(_mActivity, Android.Resource.Layout.SimpleListItem1, output.ToArray());
                ListAdapter = listAdapter;

                var refresher = v.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

                refresher.Refresh += async delegate
                {
                    try
                    {
                        output = await ctrl.GetRecentTasks(Settings.GetInstance().Dataset);

                        recent = output[0];

                        recentTask.Text = recent.FullName;
                        recentProject.Text = recent.Project.Name;

                        output.RemoveAt(0);
                        listAdapter = new TaskAdapter(_mActivity, Android.Resource.Layout.SimpleListItem1,
                            output.ToArray());
                        ListAdapter = listAdapter;

                        refresher.Refreshing = false;
                    }
                    catch (CannotReachServerException)
                    {
                        AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                        builder.SetTitle("Unable to Connect")
                            .SetMessage("Please check your network connection and try again")
                              .SetNeutralButton("Okay", (sender, args) =>
                              {
                                  builder.Dispose();
                              })
                            .SetCancelable(false);
                        AlertDialog alert = builder.Create();
                        refresher.Refreshing = false;
                        alert.Show();



                    }
                    catch (StatusNotOkayException se)
                    {

                        AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                        builder.SetTitle("An Error has occured")
                            .SetMessage("Error :" + se.GetMessage())
                            .SetNeutralButton("Okay", (sender, args) =>
                            {
                                builder.Dispose();
                            })
                            .SetCancelable(false);
                        AlertDialog alert = builder.Create();
                        refresher.Refreshing = false;
                        alert.Show();


                    }
                    catch (Exception e)
                    {
                        // For any other weird exceptions
                        AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                        builder.SetTitle("An Error has occured")
                              .SetNeutralButton("Okay", (sender, args) =>
                              {
                                  builder.Dispose();
                              })
                            .SetMessage("Error :" + e.Message)
                            .SetCancelable(false);
                        AlertDialog alert = builder.Create();
                        refresher.Refreshing = false;
                        alert.Show();

                    }

                };

                recentProject.Click += (sender, args) =>
                {
                    ((MainActivity) Activity).ListOfProjectsCallback(recent.Project.Id, recent.Project.Name);
                };

                recentTask.Click += (sender, args) =>
                {
                    ((MainActivity) Activity).PassTaskDetailsInfo(recent.Id, recent.FullName, recent.Project.Name,
                        recent.CompletionDate,
                        recent.EstimatedTime, recent.ActualTime);
                };
            }
            catch (CannotReachServerException)
            {
                pb.Dismiss();
                AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                builder.SetTitle("Unable to Connect")
                    .SetMessage("Please check your network connection and try again")
                      .SetNeutralButton("Okay", (sender, args) =>
                      {
                          builder.Dispose();
                      })
                    .SetCancelable(false);
                AlertDialog alert = builder.Create();
                alert.Show();


            }
            catch (StatusNotOkayException se)
            {
                pb.Dismiss();
                AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                builder.SetTitle("An Error has occured")
                    .SetMessage("Error :" + se.GetMessage())
                    .SetNeutralButton("Okay", (sender, args) =>
                    {
                        builder.Dispose();
                    })
                    .SetCancelable(false);
                AlertDialog alert = builder.Create();
                alert.Show();


            }
            catch (Exception e)
            {
                // For any other weird exceptions
                pb.Dismiss();
                AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                builder.SetTitle("An Error has occured")
                      .SetNeutralButton("Okay", (sender, args) =>
                      {
                          builder.Dispose();
                      })
                    .SetMessage("Error :" + e.Message)
                    .SetCancelable(false);
                AlertDialog alert = builder.Create();
                alert.Show();

            }
            /*
                private void LoadDummyData(View v)
                {

                    ListView lv = v.FindViewById<ListView>(Android.Resource.Id.List);
                    var items = new[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
                    ArrayAdapter listAdapter = new ArrayAdapter<String>(_mActivity, Android.Resource.Layout.SimpleListItem1, items);
                    ListAdapter = listAdapter;

                    TextView recentTask = v.FindViewById<TextView>(Resource.Id.Home_RecentTask);
                    recentTask.Text = "Project / Mobile App l1 / Iteration 1 / View Skeletons / Create Android Skeletons / Home Screen ";

                }
        */
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


        [BroadcastReceiver(Enabled = true)]
        [IntentFilter(new[] { "processdashboard.timelogger" })]
        public class myBroadCastReceiver : BroadcastReceiver
        {

            private MainActivity home;

            public myBroadCastReceiver()
            {
                
            }

            public myBroadCastReceiver(MainActivity home)
            {
                this.home = home;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                string value = intent.GetStringExtra("key");
                Debug.WriteLine(value);
                if (value.Equals("Time logging has been started by the server"))
                {
                    home.ModifyPlayPauseState(true);
                }
                else if (value.Equals("Time logging has been cancelled by the server"))
                {
                    home.ModifyPlayPauseState(false);
                }


            }
        }
    }
}