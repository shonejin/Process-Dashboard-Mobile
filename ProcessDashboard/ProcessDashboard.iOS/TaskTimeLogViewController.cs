using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace ProcessDashboard.iOS
{
    public partial class TaskTimeLogViewController : UIViewController
    {
        public TaskTimeLogViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var ProjectNameLabel = new UILabel(new CGRect(30, 100, 300, 40))
			{
				Text = "/ Project / Mobile App I1",
				Font = UIFont.SystemFontOfSize(16),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			var TaskNameLabel = new UILabel(new CGRect(30, 180, 300, 60))
			{
				Text = "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Team Walkthrough",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};


			string[] tableItems = new string[] { "2016-06-02 11:00 AM",
				"2016-06-02 2:10 PM",
				"2016-06-02 3:30 PM",
				"2016-06-03 11:00 AM",
				"2016-06-04 1:00 AM",
				"2016-06-05 12:00 PM",
				"2016-06-06 4:00 PM",
				"2016-06-07 11:00 PM" };

			TaskTimeLogTable = new UITableView(new CGRect(0, 250, View.Bounds.Width, View.Bounds.Height));
			TaskTimeLogTable.Source = new ProjectsTableSource(tableItems, this);

			Add(TaskTimeLogTable);


			this.Add(ProjectNameLabel);
			this.Add(TaskNameLabel);


		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			// set the View Controller that’s powering the screen we’re
			// transitioning to
			if (segue.Identifier == "home2taskDetailsSegue")
			{
				var detailContoller = segue.DestinationViewController as TaskDetailsViewController;
				var indexPath = (NSIndexPath)sender;
			}

		}
    }
}