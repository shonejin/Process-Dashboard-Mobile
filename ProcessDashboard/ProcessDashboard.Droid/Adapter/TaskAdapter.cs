#region
using System;
using Android.Content;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;
#endregion
namespace ProcessDashboard.Droid.Adapter
{
    public class TaskAdapter : ArrayAdapter
    {
        private readonly Task[] _taskList;

        public TaskAdapter(Context context, int resource, Task[] objects) : base(context, resource, objects)
        {
            _taskList = objects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var v = base.GetView(position, convertView, parent);
            // If it has a completion date and that date is not the min value
            if (_taskList[position].CompletionDate != null &&
                !_taskList[position].CompletionDate.Equals(DateTime.MinValue))
            {
                // Strike out the text
                var text = _taskList[position].FullName;
                var tv = v.FindViewById<TextView>(Android.Resource.Id.Text1);
                var spannable = new SpannableString(text);
                spannable.SetSpan(new StrikethroughSpan(), 0, text.Length, SpanTypes.InclusiveExclusive);
                tv.TextFormatted = spannable;
            }
            return v;
        }

        public Task GetTask(int position)
        {
            return _taskList[position];
        }
    }
}