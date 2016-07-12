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
		UILabel ProjectNameLabel, TaskNameLabel;
		string[] tableItems;

		// This ID is used to fetch the time logs. It is set by the previous view controller
		public string taskId;

		public TaskTimeLogViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			refreshData();
		}

		private void refreshData()
		{
			getTimeLogsOfTask();

			TaskTimeLogTable.Source = new TaskTimeLogTableSource(tableItems, this);
			TaskTimeLogTable.ReloadData();
		}

		public async void getTimeLogsOfTask()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			List<TimeLogEntry> timeLogEntries = await c.GetTimeLog("mock", 0, null, null,taskId, null);

			try
			{
				System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
				System.Diagnostics.Debug.WriteLine("Length is " + timeLogEntries.Count);

				foreach (var proj in timeLogEntries)
				{
					System.Diagnostics.Debug.WriteLine("Task Name : " + proj.task.fullName);
					System.Diagnostics.Debug.WriteLine("Start Date : " + proj.startDate);
					System.Diagnostics.Debug.WriteLine("End Date : " + proj.endDate);
					//  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
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