using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using ProcessDashboard.DTO;

namespace ProcessDashboard.Droid.Adapter
{
    public class ProjectsAdapter : ArrayAdapter
    {
        private Project[] _projectList;

        public ProjectsAdapter(Context context, int resource, Project[] objects) : base(context, resource, objects)
        {
            _projectList = objects;
        }

        public Project GetProject(int position)
        {
            return _projectList[position];
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var v = base.GetView(position, convertView, parent);
            // If it has a completion date and that date is not the min value

            var text = _projectList[position].Name;
            var tv = v.FindViewById<TextView>(Android.Resource.Id.Text1);
            var spannable = new SpannableString(text);
            spannable.SetSpan(new LeadingMarginSpanStandard(0, 15), 0, text.Length, 0);
            
            tv.TextFormatted = spannable;
            return v;
        }
    }
}