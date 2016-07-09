using System;
using System.Drawing;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;

namespace ProcessDashboard.iOS
{
    public partial class TimeLogPageViewController : UIViewController
    {
		UIBarButtonItem delete, done;
		NSIndexPath currentPath;
		List<TimelogTableItem> veges;

        public TimeLogPageViewController (IntPtr handle) : base (handle)
        {
        }

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			TimelogsTable = new UITableView(new CGRect(0,100,View.Bounds.Width,View.Bounds.Height-100), UITableViewStyle.Grouped);

			done = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) =>
			{
				TimelogsTable.SetEditing(false, true);
				NavigationItem.RightBarButtonItem = delete;
			//	TimelogTableSource.DidFinishTableEditing(table);
			});
			delete = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (s, e) =>
			{
				if (TimelogsTable.Editing)
					TimelogsTable.SetEditing(false, true); // if we've half-swiped a row
				//tableSource.WillBeginTableEditing(table);
				TimelogsTable.SetEditing(true, true);
				NavigationItem.LeftBarButtonItem = null;
				NavigationItem.RightBarButtonItem = done;
			});

			NavigationItem.RightBarButtonItem = delete;
			//NavigationItem.LeftBarButtonItem = edit;

			//TimelogsTable.AutoresizingMask = UIViewAutoresizing.All;
			CreateTableItems();
			Add(TimelogsTable);

			StaticLabel = new UILabel(new CGRect(0, 60, View.Bounds.Width, 40))
			{
				Text = " Task name\t\t\t\t\t\t Start time \t Delta ",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(225, 225, 225),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};
			this.Add(StaticLabel);

		}

		protected void CreateTableItems()
		{
			veges = new List<TimelogTableItem>();

		    var vege1 = new string[] { "2016-06-01","/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Personal Review","11:00 AM", "3:01"};
			veges.Add(new TimelogTableItem(vege1[1]) { SubHeading = vege1[0], StartTime = vege1[2], Delta = vege1[3], Int = "00:00", Comment = "Test" });

			var vege2 = new string[] { "2016-06-01", "/ Project / Mobile App I1 / High Level Design Document / View Logic/ UI experiment / Team Review","3:00 PM", "1:20" };
			veges.Add(new TimelogTableItem(vege2[1]) { SubHeading = vege2[0], StartTime = vege2[2], Delta = vege2[3], Int = "00:00", Comment = "Test" });

			var vege3 = new string[] { "2016-06-01", "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Refine Document","7:00 PM", "2:31" };
			veges.Add(new TimelogTableItem(vege3[1]) { SubHeading = vege3[0],StartTime = vege3[2], Delta = vege3[3], Int = "00:00", Comment = "Test" });

			var vege4 = new string[] { "2016-06-02", " / Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Personal Review","11:20 AM", "2:32" };
			veges.Add(new TimelogTableItem(vege4[1]) { SubHeading = vege4[0],StartTime = vege4[2], Delta = vege4[3], Int = "00:00", Comment = "Test" });

			var vege5 = new string[] { "2016-06-02", "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Team Review","3:00 PM", "1:00" };
			veges.Add(new TimelogTableItem(vege5[1]) { SubHeading = vege5[0],StartTime = vege5[2], Delta = vege5[3], Int = "00:00", Comment = "Test" });

			var vege6 = new string[] { "2016-06-03", "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Refine Document","3:24 PM", "1:21" };
			veges.Add(new TimelogTableItem(vege6[1]) { SubHeading = vege6[0],StartTime = vege6[2], Delta = vege6[3], Int = "00:00", Comment = "Test" });

			var vege7 = new string[] { "2016-06-04", "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Refine Document", "1:30 PM", "0:28" };
			veges.Add(new TimelogTableItem(vege6[1]) { SubHeading = vege7[0], StartTime = vege7[2], Delta = vege7[3], Int = "00:00", Comment = "Test" });

			var vege8 = new string[] { "2016-06-04", "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Refine Document", "11:10 PM", "0:31" };
			veges.Add(new TimelogTableItem(vege6[1]) { SubHeading = vege8[0], StartTime = vege8[2], Delta = vege8[3], Int = "00:00", Comment = "Test" });

			var vege9 = new string[] { "2016-06-05", "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Refine Document", "12:11 PM", "0:21" };
			veges.Add(new TimelogTableItem(vege6[1]) { SubHeading = vege9[0], StartTime = vege9[2], Delta = vege9[3], Int = "00:00", Comment = "Test" });

			var vege10 = new string[] { "2016-06-06", "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Refine Document", "5:11 PM", "0:49" };
			veges.Add(new TimelogTableItem(vege6[1]) { SubHeading = vege10[0], StartTime = vege10[2], Delta = vege10[3], Int = "00:00", Comment = "Test" });

			TimelogsTable.Source = new TimelogTableSource(veges, this);

		}


		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			// set the View Controller that’s powering the screen we’re
			// transitioning to
			if (segue.Identifier == "detailSegue")
			{

				var navctlr = segue.DestinationViewController as TimelogDetailViewController;
				if (navctlr != null)
				{
					var source = TimelogsTable.Source as TimelogTableSource;
					var rowPath = TimelogsTable.IndexPathForSelectedRow;
					//currentPath = [keys[indexPath.Section]][indexPath.Row];
					var item = source.GetItem(rowPath);
					navctlr.SetTask(this, item); // to be defined on the TaskDetailViewController
				}
			}

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			TimelogsTable.Source = new TimelogTableSource(veges, this);
			//TimelogsTable.ReloadData();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
		}

		public void SaveTask(TimelogTableItem oldLog, TimelogTableItem newLog)
		{
			var oldTask = veges.Find(temp => temp.Heading.Equals(oldLog.Heading));
			veges.Remove(oldTask);
			veges.Add(newLog);
		}

		public void DeleteTask(TimelogTableItem log)
		{

			var oldTask = veges.Find(t => t.Heading.Equals(log.Heading));
			veges.Remove(oldTask);
			NavigationController.PopViewController(true);
		}


	}

}