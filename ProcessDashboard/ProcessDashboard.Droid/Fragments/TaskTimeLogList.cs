#region
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class TaskTimeLogList : ListFragment
    {
        private string _id { get; set; }
        private string ProjectName { get; set; }
        private string TaskName { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity) Activity).SetTitle("Task Timelogs");
            // Create your fragment here
        }

        public void SetData(string id, string projectName, string taskName)
        {
            _id = id;
            ProjectName = projectName;
            TaskName = TaskName;
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity) Activity).SetTitle("Task Timelogs");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var v = inflater.Inflate(Resource.Layout.TaskTimeLogList, container, false);
            GetData(v);
            return v;
        }

        public async void GetData(View v)
        {
            var ctrl = ((MainActivity) Activity).Ctrl;

            var pn = v.FindViewById<TextView>(Resource.Id.TaskTimeLog_ProjectName);
            var tn = v.FindViewById<TextView>(Resource.Id.TaskTimeLog_TaskName);
            var fab = v.FindViewById<FloatingActionButton>(Resource.Id.TaskTimeLog_fab);

            pn.Text = ProjectName;
            tn.Text = TaskName;

            var timelogs = await ctrl.GetTimeLogs(Settings.GetInstance().Dataset, null, null, null, _id, null);

            Debug.WriteLine("Got data for timelogs :" + timelogs.Count);
            foreach (var timelog in timelogs)

            {
                Debug.WriteLine(timelog);
            }

            var listAdapter = new TaskTimeLogAdapter(Activity, Android.Resource.Layout.SimpleListItem1,
                timelogs.ToArray());
            ListAdapter = listAdapter;

            pn.Text = timelogs[0].Task.Project.Name;
            tn.Text = timelogs[0].Task.FullName;

            fab.Click += (sender, args) =>
            {
                if (timelogs.Count > 0)
                {
                    Debug.WriteLine("Project Name :" + timelogs[0].Task.Project.Name);
                    Debug.WriteLine("Task Name :" + timelogs[0].Task.FullName);

                    ((MainActivity) Activity).TimeLogEditCallBack(timelogs[0].Task.Project.Name,
                        timelogs[0].Task.FullName, timelogs[0].Task.Id, null);
                }
                else
                {
                    ((MainActivity) Activity).TimeLogEditCallBack(ProjectName, TaskName, _id, null);
                }
            };
            fab.Show();

            ListView.ItemClick += (sender, args) =>
            {
                var i = args.Position;
                Debug.WriteLine("I position :" + i);
                ((MainActivity) Activity).TimeLogEditCallBack(timelogs[i].Task.Project.Name, timelogs[i].Task.FullName,
                    timelogs[i].Task.Id, timelogs[i]);
            };
        }
    }

    public class TaskTimeLogAdapter : ArrayAdapter
    {
        private readonly TimeLogEntry[] _timeLogEntries;

        public TaskTimeLogAdapter(Context context, int resource, TimeLogEntry[] objects)
            : base(context, resource, objects)
        {
            _timeLogEntries = objects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var v = base.GetView(position, convertView, parent);

            return v;
        }

        public TimeLogEntry GetProject(int position)
        {
            return _timeLogEntries[position];
        }
    }
}