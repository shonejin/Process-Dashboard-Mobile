#region
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.Droid.Adapter;
using ProcessDashboard.SyncLogic;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class ListProjectTasks : ListFragment
    {
        private string _projectId;

        public ListProjectTasks(string projectId)
        {
            _projectId = projectId;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity) Activity).SetTitle("List of Tasks");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity) Activity).SetTitle("List of Tasks");
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            SetId(_projectId);
            var listView = ListView;
            listView.ItemClick += (sender, e) =>
            {
                var ta = (TaskAdapter) listView.Adapter;
                var p = ta.GetTask(e.Position);
                var taskId = p.Id;
                //  ((MainActivity)this.Activity).switchToFragment(MainActivity.fragmentTypes.taskdetails);
                ((MainActivity) Activity).PassTaskDetailsInfo(taskId, p.FullName, p.Project.Name, p.CompletionDate,
                    p.EstimatedTime, p.ActualTime);
                //Project p = listView.GetItemAtPosition(e.Position);
            };
            AddData(((MainActivity) Activity).Ctrl, _projectId);
        }

        public void SetId(string projectId)
        {
            _projectId = projectId;
        }

        private async void AddData(Controller ctrl, string projectId)
        {
            var output = await ctrl.GetTasks(Settings.GetInstance().Dataset, projectId);
            var listAdapter = new TaskAdapter(Activity, Android.Resource.Layout.SimpleListItem1, output.ToArray());
            ListView.Adapter = listAdapter;
            SetListShown(true);
        }

        public void LoadDummyData()
        {
            string[] values = {"Sample Task", "Component 1 / Component 2 / Code", "... / head truncation"};
            ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleExpandableListItem1, values);
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //base.OnListItemClick(l, v, position, id);
            //ListView.SetItemChecked(position, true);
            //((MainActivity)Activity).switchToFragment(MainActivity.fragmentTypes.taskdetails);
        }
    }
}