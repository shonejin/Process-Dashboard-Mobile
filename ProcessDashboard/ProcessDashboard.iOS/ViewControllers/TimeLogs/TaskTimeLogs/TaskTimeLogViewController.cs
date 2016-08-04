using Foundation;
using System;
using UIKit;
using CoreGraphics;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using ProcessDashboard.DTO;
using System.Collections.Generic;
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
    public partial class TaskTimeLogViewController : UIViewController
    {

		//UILabel ProjectNameLabel, TaskNameLabel, TimelogsLabel;
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
		private async void refreshData()
		{
			await getTimeLogsOfTask();

			TaskTimeLogTable.Source = new TaskTimeLogTableSource(timeLogCache, this);
			TaskTimeLogTable.ReloadData();
		}

		public async System.Threading.Tasks.Task<int> getTimeLogsOfTask()
		{
			List<TimeLogEntry> timeLogEntries = await PDashAPI.Controller.GetTimeLogs(0, null, null,taskId, null);

			timeLogCache = timeLogEntries;

			return 0;
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ProjectNameLabel.Text = task.Project != null ? task.Project.Name : "";
			TaskNameLabel.Text = task.FullName;
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
				AddTaskTimelog(newTimeLog);

			}

		}

		public async void DeleteTask(TimeLogEntry log)
		{

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
				try
				{
					System.Diagnostics.Debug.WriteLine("** Delete the new Time Log entry **");
					System.Diagnostics.Debug.WriteLine("Status :" + tr.Stat);

				}
				catch (System.Exception e)
				{
					System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
				}
			}
			return 0;
		}

		public async void UpdateTaskTimelog(TimeLogEntry log)
		{
			await UpdateATimeLog(log);
		}

		public async System.Threading.Tasks.Task<int> UpdateATimeLog(TimeLogEntry editedTimeLog)
		{
			TimeLogEntry tr = await PDashAPI.Controller.UpdateTimeLog(editedTimeLog.Id.ToString(), editedTimeLog.Comment, editedTimeLog.StartDate, editedTimeLog.Task.Id, editedTimeLog.LoggedTime, editedTimeLog.InterruptTime, true);
			try
			{
				System.Diagnostics.Debug.WriteLine("** Updated the new Time Log entry **");
				System.Diagnostics.Debug.WriteLine("Updated Logged Time :" + tr.LoggedTime);

			}
			catch (System.Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;

		}

		public async void AddTaskTimelog(TimeLogEntry log)
		{
			await TestAddATimeLog(log);
		}

		public async Task<int> TestAddATimeLog(TimeLogEntry log)
		{
			int id;
			try
			{
				var tr = await PDashAPI.Controller.AddATimeLog("No Comment", DateTime.UtcNow, log.Task.Id, log.LoggedTime, log.InterruptTime, true);
				Console.WriteLine("** Added a new Time Log entry **");
				Console.WriteLine(tr.Id);
				id = tr.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine("We are in an error state :" + e);
				id = 0;
			}

			return id;

		}

    }
}