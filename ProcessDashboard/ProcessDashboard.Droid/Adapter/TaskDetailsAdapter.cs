using Android.App;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

namespace ProcessDashboard.Droid.Adapter
{
    public class TaskDetailsAdapter : BaseAdapter
    {
        private readonly Entry[] _values;
        private readonly Activity context;
        private readonly int resource;
        public TaskDetailsAdapter(Activity context, int resource, Entry[] objects) 
        {
            _values = objects;
            this.context = context;
            this.resource = resource;
        }

        public override bool IsEnabled(int position)
        {
            if (position == 2)
            {
                if (_values[2].value.Equals("-"))
                {
                    return false;
                }
            }
            return true;
        }

        public override Object GetItem(int position)
        {
            return "";
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var v = convertView ?? context.LayoutInflater.Inflate(resource, null);

           
            var name = _values[position].name;
            var value = _values[position].value;

            var tv = v.FindViewById<TextView>(Resource.Id.Entry_name);
            var sv = v.FindViewById<TextView>(Resource.Id.Entry_value);
            
            tv.Text = name;
            sv.Text = ""+value;

            return v;
        }
        public override int Count
        {
            get { return _values.Length; }
        }

        public Entry GetEntry(int position)
        {
            return _values[position];
        }
    }

    public class Entry
    {
        public string name { get; set; }
        public object value { get; set; }

    }
}