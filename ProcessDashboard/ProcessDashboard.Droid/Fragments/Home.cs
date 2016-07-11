using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.App;

namespace ProcessDashboard.Droid.Fragments
{
    public class Home : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.Home, container, false);
            
            loadDummyData(v);
            return v;
        }

        private void loadDummyData(View v)
        {

            ListView lv = v.FindViewById<ListView>(Resource.Id.recentTaskList_Home);
            string[] items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
            ArrayAdapter ListAdapter = new ArrayAdapter<String>(this.Activity, Android.Resource.Layout.SimpleListItem1, items);
            lv.Adapter = ListAdapter;

            lv.ItemClick += Lv_ItemClick;

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