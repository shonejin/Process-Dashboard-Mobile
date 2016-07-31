#region
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.Droid.Adapter;
using ProcessDashboard.SyncLogic;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class Home : ListFragment
    {
        private MainActivity _mActivity;

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
            _mActivity.SetTitle("Process Dasboard");
        }

        public override void OnResume()
        {
            base.OnResume();
            _mActivity.SetTitle("Process Dasboard");
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

            var pd = new ProgressDialog(_mActivity) {Indeterminate = true};
            pd.SetTitle("Loading");

            var recentTask = v.FindViewById<TextView>(Resource.Id.Home_RecentTask);
            recentTask.Text = "Loading..";

            var recentProject = v.FindViewById<TextView>(Resource.Id.Home_CurrentProject);
            recentProject.Text = "Loading..";

            pd.Show();
            LoadData(pd, v, _mActivity.Ctrl);

            return v;
        }

        private async void LoadData(ProgressDialog pb, View v, Controller ctrl)
        {
            var output = await ctrl.GetRecentTasks(Settings.GetInstance().Dataset);

            pb.Dismiss();

            var recent = output[0];

            var rt = v.FindViewById<TextView>(Resource.Id.Home_RecentTask);
            var cp = v.FindViewById<TextView>(Resource.Id.Home_CurrentProject);

            rt.Text = recent.FullName;
            cp.Text = recent.Project.Name;

            output.RemoveAt(0);

            if (_mActivity == null)
            {
                Debug.WriteLine("Activity is null");
            }

            var listAdapter = new TaskAdapter(_mActivity, Android.Resource.Layout.SimpleListItem1, output.ToArray());
            ListAdapter = listAdapter;
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
}