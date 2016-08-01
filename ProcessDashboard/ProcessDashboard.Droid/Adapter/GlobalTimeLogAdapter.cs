#region
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ProcessDashboard.DTO;
#endregion
namespace ProcessDashboard.Droid.Adapter
{
    internal class GlobalTimeLogAdapter : BaseExpandableListAdapter
    {
        private Activity _activity;
        private Dictionary<string, List<TimeLogEntry>> _dictGroup;
        private List<string> _lstGroupId;

        public GlobalTimeLogAdapter(Activity activity, Dictionary<string, List<TimeLogEntry>> dictGroup)
        {
            _dictGroup = dictGroup;
            _activity = activity;
            _lstGroupId = dictGroup.Keys.ToList();
        }
        #region implemented abstract members of BaseExpandableListAdapter
        public override Object GetChild(int groupPosition, int childPosition)
        {
            var myObj = _dictGroup[_lstGroupId[groupPosition]][childPosition];
            return new JavaObjectWrapper<TimeLogEntry> {Obj = myObj};
        }
        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }
        public override int GetChildrenCount(int groupPosition)
        {
            return _dictGroup[_lstGroupId[groupPosition]].Count;
        }
        public override View GetChildView(int groupPosition,
            int childPosition,
            bool isLastChild,
            View convertView,
            ViewGroup parent)
        {
            var item = _dictGroup[_lstGroupId[groupPosition]][childPosition];

            if (convertView == null)
                convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ListControl_ChildItem, null);

            var taskName = convertView.FindViewById<TextView>(Resource.Id.gt_taskName);
            var delta = convertView.FindViewById<TextView>(Resource.Id.gt_delta);
            var startTime = convertView.FindViewById<TextView>(Resource.Id.gt_startTime);

            taskName.Text = item.Task.FullName;
            delta.Text = "" + item.LoggedTime;
            startTime.Text = "" + item.StartDate.TimeOfDay;

            return convertView;
        }

        public override Object GetGroup(int groupPosition)
        {
            return _lstGroupId[groupPosition];
        }
        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }
        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            var item = _lstGroupId[groupPosition];

            if (convertView == null)
                convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ListControl_Group, null);

            var textBox = convertView.FindViewById<TextView>(Resource.Id.txtLarge);
            textBox.SetText(item, TextView.BufferType.Normal);

            return convertView;
        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
        public override int GroupCount => _dictGroup.Count;
        public override bool HasStableIds => true;
        #endregion
    }

    public class JavaObjectWrapper<T> : Object
    {
        public T Obj { get; set; }
    }
}