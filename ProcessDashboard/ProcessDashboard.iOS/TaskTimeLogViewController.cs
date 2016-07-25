using Foundation;
using System;
using UIKit;
using CoreGraphics;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using ProcessDashboard.DTO;
using System.Collections.Generic;

namespace ProcessDashboard.iOS
{
    public partial class TaskTimeLogViewController : UIViewController
    {

		UILabel ProjectNameLabel, TaskNameLabel, TimelogsLabel;
		List<TimeLogEntry> timeLogCache;
		//UITableView TaskTimeLogTable;

		// This ID is used to fetch the time logs. It is set by the previous view controller
		public string taskId;
		public Task task;

		public TaskTimeLogViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			refreshData();
		}

		//public override void ViewDidDisappear(bool animated)
		//{
		//	base.ViewDidDisappear(animated);
		//	try
		//	{
		//		this.NavigationController.PopToRootViewController(true);
		//	}
		//	catch (Exception e){}
		//}

		private async void refreshData()
		{
			await getTimeLogsOfTask();

			TaskTimeLogTable.Source = new TaskTimeLogTableSource(timeLogCache, this);
			TaskTimeLogTable.ReloadData();
		}

		public async System.Threading.Tasks.Task<int> getTimeLogsOfTask()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			List<TimeLogEntry> timeLogEntries = await c.GetTimeLog("mock", 0, null, null,taskId, null);

			timeLogCache = timeLogEntries;

			try
			{
				System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
				System.Diagnostics.Debug.WriteLine("Length is " + timeLogEntries.Count);

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


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ProjectNameLabel = new UILabel(new CGRect(30, 100, 300, 40))
			{
				Text = task.Project != null ? task.Project.Name : "",
				Font = UIFont.SystemFontOfSize(16),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			TaskNameLabel = new UILabel(new CGRect(30, 160, 300, 60))
			{
				Text = task.FullName.ToString(),
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			TimelogsLabel = new UILabel(new CGRect(10, 240, 300, 20))
			{
				Text = "Time Logs:",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Left,
				//BackgroundColor = UIColor.FromRGB(220, 220, 220),
				//Lines = 0,
				//LineBreakMode = UILineBreakMode.WordWrap,
			};

			//TaskTimeLogTable = new UITableView(new CGRect(0, 200, View.Bounds.Width, View.Bounds.Height));

			//Add(TaskTimeLogTable);
			this.Add(ProjectNameLabel);
			this.Add(TaskNameLabel);
			this.Add(TimelogsLabel);


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

					navctlr.SetTaskforTaskTimelog(this, timeLogCache[rowPath.Row]); // to be defined on the TaskDetailViewController
				}
			}
			if (segue.Identifier == "AddTimeLogSegue")
			{

				var controller = segue.DestinationViewController as TimelogDetailViewController;
				TimeLogEntry newTimeLog = new TimeLogEntry();
				newTimeLog.Task = task;
				newTimeLog.StartDate = DateTime.Now;
				controller.CreateTask(this, newTimeLog);

			}

		}
    }
}