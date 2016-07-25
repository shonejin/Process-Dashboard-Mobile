using System;
using System.Drawing;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using ProcessDashboard.DTO;

namespace ProcessDashboard.iOS
{
    public partial class TimeLogPageViewController : UIViewController
    {
		UIBarButtonItem delete, done;
		NSIndexPath currentPath;
		//List<TimelogTableItem> veges;
		List<TimeLogEntry> globalTimeLogCache;

        public TimeLogPageViewController (IntPtr handle) : base (handle)
        {
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
		
			TimelogsTable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
	
			Add(TimelogsTable);

			UILabel StaticLabel = new UILabel(new CGRect(0, 60, View.Bounds.Width, 40))
			{
				Text = " Task name\t\t\t\t\t\t Start time \t Delta ",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(225, 225, 225),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			StaticLabel.AutoresizingMask = UIViewAutoresizing.All;

			this.Add(StaticLabel);

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
			refreshData();
		
		}

		//public override void ViewDidDisappear(bool animated)
		//{
		//	base.ViewDidDisappear(animated);
		//	this.NavigationController.PopToRootViewController(true);
		//}


		public void SaveTask(TimeLogEntry oldLog, TimeLogEntry newLog)
		{
			var oldTask = globalTimeLogCache.Find(temp => temp.Task.FullName.Equals(oldLog.Task.FullName));
			globalTimeLogCache.Remove(oldTask);
			globalTimeLogCache.Add(newLog);
		}

		public void DeleteTask(TimeLogEntry log)
		{

			var oldTask = globalTimeLogCache.Find(t => t.Task.FullName.Equals(log.Task.FullName));
			globalTimeLogCache.Remove(oldTask);
			NavigationController.PopViewController(true);
		}

		// This ID is used to fetch the time logs. It is set by the previous view controller

		public async void refreshData()
		{
			await getGlobalTimeLogs();
			TimelogTableSource  source = new TimelogTableSource(globalTimeLogCache, this);
			TimelogsTable.Source = source;
			TimelogsTable.ReloadData();
			TimelogsTable.ScrollToRow(NSIndexPath.FromRowSection(0,source.keys.Length-1), UITableViewScrollPosition.Top, true);
		}

		public async System.Threading.Tasks.Task<int> getGlobalTimeLogs()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			List<TimeLogEntry> timeLogEntries = await c.GetTimeLog(Settings.GetInstance().Dataset, 0, null, null, null, null);

			globalTimeLogCache = timeLogEntries;

			try
			{
				System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
				System.Diagnostics.Debug.WriteLine("Global Length is " + globalTimeLogCache.Count);

				foreach (var proj in timeLogEntries)
				{
					System.Diagnostics.Debug.WriteLine("Task Name : " + proj.Task.FullName);
					System.Diagnostics.Debug.WriteLine("Start Date : " + proj.StartDate);
					System.Diagnostics.Debug.WriteLine("End Date : " + proj.EndDate);
					//  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;
		}

	}

}