
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;


namespace ProcessDashboard.Droid.Fragments
{
    public class GlobalTimeLog : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
           // View v = inflater.Inflate(Resource.Layout.GlobalTimeLog, container, false);
            
            //var listView = v.FindViewById<ExpandableListView>(Resource.Id.myExpandableListview);
            //listView.SetAdapter(new ExpandableDataAdapter(this, Data.SampleData()));


            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}