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
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			List<TimeLogEntry> timeLogEntries = await c.GetTimeLog(Settings.GetInstance().Dataset, 0, null, null,taskId, null);

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
			ProjectNameLabel.Text = task.Project != null ? task.Project.Name : "";
			TaskNameLabel.Text = task.FullName.ToString();

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
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			string timeLogId;
			if (!val.HasValue)
			{
				timeLogId = "" + await c.TestAddATimeLog();
			}
			else
				timeLogId = "" + val.Value;

			DeleteRoot tr = await c.DeleteTimeLog("INST-szewf0", timeLogId);
			try
			{
				System.Diagnostics.Debug.WriteLine("** Delete the new Time Log entry **");
				System.Diagnostics.Debug.WriteLine("Status :" + tr.Stat);

			}
			catch (System.Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;

		}

		public async void UpdateTaskTimelog(TimeLogEntry log)
		{
			await UpdateATimeLog(log);
		}

		public async System.Threading.Tasks.Task<int> UpdateATimeLog(TimeLogEntry editedTimeLog)
		{

			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			EditATimeLogRoot tr = await c.UpdateTimeLog("INST-szewf0", editedTimeLog.Id.ToString(), editedTimeLog.Comment, editedTimeLog.StartDate.ToString(Settings.GetInstance().DateTimePattern), editedTimeLog.Task.Id.ToString(), editedTimeLog.InterruptTime, editedTimeLog.LoggedTime, true);
			try
			{
				System.Diagnostics.Debug.WriteLine("** Updated the new Time Log entry **");
				System.Diagnostics.Debug.WriteLine("Updated Logged Time :" + tr.TimeLogEntry.LoggedTime);

			}
			catch (System.Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;

		}

		public async void AddTaskTimelog(TimeLogEntry log)
		{
			await AddATimeLog(log);
		}

		public async System.Threading.Tasks.Task<int> AddATimeLog(TimeLogEntry log)
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			EditATimeLogRoot tr = await c.AddATimeLog("INST-szewf0", "No Comment", DateTime.Now.ToString(), log.Task.Id, log.InterruptTime, log.LoggedTime, true);
			try
			{
				System.Diagnostics.Debug.WriteLine("** Added a new Time Log entry **");

				System.Diagnostics.Debug.WriteLine(tr.TimeLogEntry.Id);
			}
			catch (System.Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}

			return tr.TimeLogEntry.Id;

		}
    }
}