
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProcessDashboard.Droid.Adapter;
using ProcessDashboard.DTO;
using ProcessDashboard.Service;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid.Fragments
{
    public class GlobalTimeLog : Fragment
    {
        Dictionary<string, List<TimeLogEntry>> _headings = new Dictionary<string, List<TimeLogEntry>>();
        List<string> _timelogs = new List<string>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity)(this.Activity)).setTitle("Global Time Log");

            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity)(this.Activity)).setTitle("Global Time Log");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.GlobalTimeLog, container, false);

            CreateExpendableListData(v);
            System.Diagnostics.Debug.WriteLine("We are proceeding");
            
            //var listView = v.FindViewById<ExpandableListView>(Resource.Id.myExpandableListview);
            //listView.SetAdapter(new ExpandableDataAdapter(this, Data.SampleData()));


            return v;
        }


        private async Task<int>  CreateExpendableListData(View v)
        {
            Controller ctrl = ((MainActivity)(this.Activity)).Ctrl;

            var timelogEntries = await ctrl.GetTimeLog("mock", 0,null,null, null, null);

            System.Diagnostics.Debug.WriteLine("Got the values : "+timelogEntries.Count);
            int count = 0;
            foreach (TimeLogEntry te in timelogEntries)
            {

                System.Diagnostics.Debug.WriteLine(te);

            }



            foreach (TimeLogEntry te in timelogEntries)
            {
                
                try
                {
                    bool present = true;
                    List<TimeLogEntry> children;
                    _headings.TryGetValue(te.StartDate.ToShortDateString(), out children);
                    if (children == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Children is null");
                        children = new List<TimeLogEntry>();
                        count++;
                        present = false;
                    }
                    System.Diagnostics.Debug.WriteLine("Going to add children");
                    children.Add(te);
                    
                    if (present)
                    {
                        System.Diagnostics.Debug.WriteLine("Going to remove");
                        _headings.Remove(te.StartDate.Date.ToShortDateString());
                    }
                    System.Diagnostics.Debug.WriteLine("Going to add to _headings");
                    _headings.Add(te.StartDate.Date.ToShortDateString(), children);

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message); 
                }
              

            }

            System.Diagnostics.Debug.WriteLine("Count :"+count);


            _timelogs = new List<string>(_headings.Keys);
            var ctlExListBox = v.FindViewById<ExpandableListView>(Resource.Id.myExpandableListview);
            ctlExListBox.SetAdapter(new GlobalTimeLogAdapter(this.Activity, _headings));

            ctlExListBox.ChildClick += delegate (object sender, ExpandableListView.ChildClickEventArgs e) {
                var itmGroup = _timelogs[e.GroupPosition];
                var itmChild = _headings[itmGroup][e.ChildPosition];

                Toast.MakeText(this.Activity, string.Format("You Click on Group {0} with child {1}", itmGroup, itmChild),
                                ToastLength.Long).Show();
            };
            return 0;
        }
    }
}