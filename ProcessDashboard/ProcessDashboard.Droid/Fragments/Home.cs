using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Content;
using Android.Media;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid.Fragments
{
    public class Home : ListFragment
    {
        private MainActivity _mActivity;
        
        public override void OnAttach(Context activity)
        {
            base.OnAttach(activity);
            System.Diagnostics.Debug.WriteLine("On Attach 1");
            _mActivity = (MainActivity)activity;
           // mActivity.setTitle("Process Dasboard");
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            System.Diagnostics.Debug.WriteLine("On Attach 2");
            _mActivity = (MainActivity)activity;
            //
        }

        public override void OnDetach()
        {
            base.OnDetach();
            _mActivity = null;
            System.Diagnostics.Debug.WriteLine("On Detach");
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine("On Create");
            RetainInstance = true;
            _mActivity.setTitle("Process Dasboard");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            _mActivity.setTitle("Process Dasboard");
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            System.Diagnostics.Debug.WriteLine("We are in the click event 2 ");

            TaskAdapter ta = (TaskAdapter)l.Adapter;
            Task pb = ta.GetTask(position);
            System.Diagnostics.Debug.WriteLine("The id of the task :" + pb.Id);
            string taskId = pb.Id;
            _mActivity.PassTaskDetailsInfo(taskId,null,null,null,null,null);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            System.Diagnostics.Debug.WriteLine("On Create View");
            View v = inflater.Inflate(Resource.Layout.Home, container, false);

            ProgressDialog pd = new ProgressDialog(_mActivity);
            pd.Indeterminate = true;
            pd.SetTitle("Loading");
            
            //ProgressBa pb = v.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            //pb.Indeterminate = true;

            ListView listView = v.FindViewById<ListView>(Android.Resource.Id.List);
            System.Diagnostics.Debug.WriteLine("We are now assigning values to the list view");
            //pb.Visibility = ViewStates.Visible;
            TextView recentTask = v.FindViewById<TextView>(Resource.Id.Home_RecentTask);
            recentTask.Text = "Project / Mobile App l1 / Iteration 1 / View Skeletons / Create Android Skeletons / Home Screen ";
            pd.Show();
            LoadData(pd, v, _mActivity.Ctrl);
           // loadDummyData(v);
            return v;
        }

        private async void LoadData(ProgressDialog pb, View v, Controller ctrl)
        {
            ListView listView = v.FindViewById<ListView>(Android.Resource.Id.List);
            List<DTO.Task> output = await ctrl.GetRecentTasks(Settings.GetInstance().Dataset);
            System.Diagnostics.Debug.WriteLine("Got the data back");
            pb.Dismiss();
            System.Diagnostics.Debug.WriteLine("Dialog has been dismissed");
            Task recent = output[0];
            System.Diagnostics.Debug.WriteLine("BFE : 1");
            TextView rt = v.FindViewById<TextView>(Resource.Id.Home_RecentTask);
            System.Diagnostics.Debug.WriteLine("BFE : 2");
            TextView cp = v.FindViewById<TextView>(Resource.Id.Home_CurrentProject);
            System.Diagnostics.Debug.WriteLine("BFE : 3");

            rt.Text = recent.FullName;
            System.Diagnostics.Debug.WriteLine("BFE : 4");
            cp.Text = recent.Project.Name;
            System.Diagnostics.Debug.WriteLine("BFE : 5");

            output.RemoveAt(0);

            System.Diagnostics.Debug.WriteLine("BFE : 6");

            if (_mActivity == null)
            {
                System.Diagnostics.Debug.WriteLine("Activity is null");
            }
           
            TaskAdapter listAdapter = new TaskAdapter(_mActivity, Android.Resource.Layout.SimpleListItem1, output.ToArray());
            System.Diagnostics.Debug.WriteLine("BFE : 7");
            this.ListAdapter = listAdapter;
            System.Diagnostics.Debug.WriteLine("BFE : 8");
            //SetListShown(true);
        }

        private void LoadDummyData(View v)
        {

            ListView lv = v.FindViewById<ListView>(Android.Resource.Id.List);
            string[] items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
            ArrayAdapter listAdapter = new ArrayAdapter<String>(_mActivity, Android.Resource.Layout.SimpleListItem1, items);
            this.ListAdapter = listAdapter;
            //lv.ItemClick += Lv_ItemClick;
            // Set the recent task 
            TextView recentTask = v.FindViewById<TextView>(Resource.Id.Home_RecentTask);
            recentTask.Text = "Project / Mobile App l1 / Iteration 1 / View Skeletons / Create Android Skeletons / Home Screen ";
            
        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Use the item clicked.
            _mActivity.SwitchToFragment(MainActivity.FragmentTypes.Listoftasks);

        }
    }
}