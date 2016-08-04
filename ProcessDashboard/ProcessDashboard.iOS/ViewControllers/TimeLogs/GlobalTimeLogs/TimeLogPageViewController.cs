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
using System.Linq;
using System.Threading.Tasks;
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using Exception = System.Exception;
using Task = ProcessDashboard.DTO.Task;

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
				TimelogsTable.SetEditing(true, true);
				NavigationItem.LeftBarButtonItem = null;
				NavigationItem.RightBarButtonItem = done;
			});

			NavigationItem.RightBarButtonItem = delete;
		
			TimelogsTable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
	
			Add(TimelogsTable);

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

		public async void DeleteTask(TimeLogEntry log)
		{

			//var oldTask = globalTimeLogCache.Find(t => t.Task.FullName.Equals(log.Task.FullName));
			await DeleteATimeLog(log.Id);
			NavigationController.PopViewController(true);
		}

		public async System.Threading.Tasks.Task<int> DeleteATimeLog(int? val)
		{
			string timeLogId;
			if (val.HasValue)
			{
				timeLogId = "" + val.Value;
				DeleteRoot tr = await PDashAPI.Controller.DeleteTimeLog(timeLogId);
			}
			return 0;
		}

		public async void UpdateTaskTimelog(TimeLogEntry log)
		{
			await UpdateATimeLog(log);
			//NavigationController.PopViewController(true);
		}

		public async System.Threading.Tasks.Task<int> UpdateATimeLog(TimeLogEntry editedTimeLog)
		{
			TimeLogEntry tr = await PDashAPI.Controller.UpdateTimeLog(editedTimeLog.Id.ToString(), editedTimeLog.Comment, editedTimeLog.StartDate, editedTimeLog.Task.Id.ToString(),  editedTimeLog.InterruptTime, editedTimeLog.LoggedTime, true);
			return 0;

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
			List<TimeLogEntry> timeLogEntries = await PDashAPI.Controller.GetTimeLogs(0, null, null, null, null);

			globalTimeLogCache = timeLogEntries;

			return 0;
		}

	}

}