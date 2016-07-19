using System;
using System.Collections;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.App;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid.Fragments
{
    public class Home : ListFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            ((MainActivity)(this.Activity)).setTitle("Process Dasboard");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("Process Dasboard");
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            System.Diagnostics.Debug.WriteLine("We are in the click event 2 ");

            TaskAdapter ta = (TaskAdapter)l.Adapter;
            Task pb = ta.GetTask(position);
            System.Diagnostics.Debug.WriteLine("The id of the task :" + pb.id);
            string taskId = pb.id;
            ((MainActivity)this.Activity).passTaskDetailsInfo(taskId);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.Home, container, false);

            ProgressDialog pd = new ProgressDialog(this.Activity);
            pd.Indeterminate = true;
            pd.SetTitle("Loading");
            

            //ProgressBa pb = v.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            //pb.Indeterminate = true;


            ListView listView = v.FindViewById<ListView>(Android.Resource.Id.List);

            System.Diagnostics.Debug.WriteLine("We are now assigning values to the list view");

            //pb.Visibility = ViewStates.Visible;
            
            TextView recentTask = v.FindViewById<TextView>(Resource.Id.recentTask_Home);
            recentTask.Text = "Project / Mobile App l1 / Iteration 1 / View Skeletons / Create Android Skeletons / Home Screen ";

            pd.Show();
            test(pd, listView, ((MainActivity)this.Activity)._ctrl);
           // loadDummyData(v);


            return v;
        }
        private async void test(ProgressDialog pb, ListView listView, Controller ctrl)
        {
            List<DTO.Task> output = await ctrl.GetRecentTasks("mock");
            pb.Dismiss();
            TaskAdapter listAdapter = new TaskAdapter(Activity, Android.Resource.Layout.SimpleListItem1, output.ToArray(), 1,this.Activity);
            this.ListAdapter = listAdapter;
            //SetListShown(true);

        }

        private void loadDummyData(View v)
        {

            ListView lv = v.FindViewById<ListView>(Android.Resource.Id.List);
            string[] items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
            ArrayAdapter ListAdapter = new ArrayAdapter<String>(this.Activity, Android.Resource.Layout.SimpleListItem1, items);
            this.ListAdapter = ListAdapter;
            
            //lv.ItemClick += Lv_ItemClick;

            // Set the recent task 
            TextView recentTask = v.FindViewById<TextView>(Resource.Id.recentTask_Home);
            recentTask.Text = "Project / Mobile App l1 / Iteration 1 / View Skeletons / Create Android Skeletons / Home Screen ";


        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            // Use the item clicked.

            ((MainActivity)Activity).switchToFragment(MainActivity.fragmentTypes.listoftasks);

        }
    }
}