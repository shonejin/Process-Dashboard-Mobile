
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;
namespace ProcessDashboard.Droid.Fragments
{
    public class TaskDetails : Fragment
    {
        public string taskId;

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

        public void setId(string id)
        {
            taskId = id;
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

            Task t = await (((MainActivity)(Activity))._ctrl).GetTask("mock", taskId);

            if (t == null)
            {
                System.Diagnostics.Debug.WriteLine("T is null");
            }

            TextView projectName = view.FindViewById<TextView>(Resource.Id.TdProjectNameTextView);
            projectName.Text = t.project.name;
            System.Diagnostics.Debug.WriteLine("1");

            TextView taskName = view.FindViewById<TextView>(Resource.Id.TdTaskNameTextView);
            taskName.Text = t.fullName;
            System.Diagnostics.Debug.WriteLine("2");
            TextView tdCompleteTextView = view.FindViewById<TextView>(Resource.Id.TdCompleteTextView);
            tdCompleteTextView.Text = "Completed on : " + t.completionDate;

            System.Diagnostics.Debug.WriteLine("3");
            TextView tdPlannedTimeTextView = view.FindViewById<TextView>(Resource.Id.TdPlannedTimeTextView);
            tdPlannedTimeTextView.Text = "" + t.estimatedTime;
            System.Diagnostics.Debug.WriteLine("4");
            TextView tdActualTimeTextView = view.FindViewById<TextView>(Resource.Id.TdActualTimeTextView);
            tdActualTimeTextView.Text = "" + t.actualTime;
            System.Diagnostics.Debug.WriteLine("5");
            EditText a = view.FindViewById<EditText>(Resource.Id.TdNotesEditText);
            if (t.taskNote == null || t.taskNote.Length == 0)
            {
                a.Text = "-";
            }else
            a.Text = t.taskNote;
            System.Diagnostics.Debug.WriteLine("6");

        }
    }
}