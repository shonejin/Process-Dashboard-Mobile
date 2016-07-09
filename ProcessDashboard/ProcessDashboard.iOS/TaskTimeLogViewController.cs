using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace ProcessDashboard.iOS
{
    public partial class TaskTimeLogViewController : UIViewController
    {
		UILabel ProjectNameLabel, TaskNameLabel;
		string[] tableItems;

		public TaskTimeLogViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ProjectNameLabel = new UILabel(new CGRect(30, 100, 300, 40))
			{
				Text = "/ Project / Mobile App I1",
				Font = UIFont.SystemFontOfSize(16),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			TaskNameLabel = new UILabel(new CGRect(30, 180, 300, 60))
			{
				Text = "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Team Walkthrough",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};


			tableItems = new string[] { "2016-06-02 11:00 AM",
				"2016-06-02 2:10 PM",
				"2016-06-02 3:30 PM",
				"2016-06-03 11:00 AM",
				"2016-06-04 1:00 AM",
				"2016-06-05 12:00 PM",
				"2016-06-06 4:00 PM",
				"2016-06-07 11:00 PM" };

			TaskTimeLogTable = new UITableView(new CGRect(0, 250, View.Bounds.Width, View.Bounds.Height));
			TaskTimeLogTable.Source = new TaskTimeLogTableSource(tableItems, this);

			Add(TaskTimeLogTable);

			this.Add(ProjectNameLabel);
			this.Add(TaskNameLabel);


		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "eachTimeLogSegue")
			{
				var navctlr = segue.DestinationViewController as TimelogDetailViewController;
				if (navctlr != null)
				{
					var source = TaskTimeLogTable.Source as TaskTimeLogTableSource;
					var rowPath = TaskTimeLogTable.IndexPathForSelectedRow;
					var item = tableItems[rowPath.Row];
					TimelogTableItem t= new TimelogTableItem();
					String[] strs = item.Split(' ');

					t.Heading = TaskNameLabel.Text;
					t.SubHeading = strs[0];
					t.StartTime = strs[1] + " " + strs[2];
					t.Delta = "00:00";
					t.Int = "00:00";
					t.Comment = "test!";
					navctlr.SetTaskforTaskTimelog(this, t); // to be defined on the TaskDetailViewController
				}
			}

		}
    }
}