using Foundation;
using System;
using UIKit;
using CoreGraphics;
using ProcessDashboard.DTO;
using ProcessDashboard.Model;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Drawing;
using SharpMobileCode.ModalPicker;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using ProcessDashboard.DTO;

namespace ProcessDashboard.iOS
{
    public partial class TaskDetailsViewController : UIViewController
    {
		public Task task;
		public Task taskDetail;

		/*
        public string fullName { get; set; }
        public Project project { get; set; }
        public DateTime completionDate { get; set; }
        public double estimatedTime { get; set; }
        public double actualTime { get; set; }
        public string taskNote { get; set; }

		*/

		public TaskDetailsViewController (IntPtr handle) : base (handle)
        {
        }

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);
			if (segue.Identifier == "TaskTimeLogsSegue")
			{
				var controller = segue.DestinationViewController as TaskTimeLogViewController;
				controller.taskId = task.Id;
				controller.task = task;
			
			}
			if (segue.Identifier == "showListofTasks")
			{
				TasksTableViewController controller = (TasksTableViewController)segue.DestinationViewController;
				controller.projectId = task.Project.Id;
				controller.projectName = task.Project.Name;
			}

		}


		public async System.Threading.Tasks.Task<int> GetTaskithID(string taskID)
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);
			DTO.Task taskItem = await c.GetTask("mock", taskID);
			taskDetail = taskItem;

			try
			{
				System.Diagnostics.Debug.WriteLine("** TASK ENTRY **");
				System.Diagnostics.Debug.WriteLine(taskItem.FullName + " : " + taskItem.Id);
				System.Diagnostics.Debug.WriteLine(taskItem.EstimatedTime + " & " + taskItem.ActualTime);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;


		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			refreshData();
		}

		//public override void ViewDidDisappear(bool animated)
		//{
		//	base.ViewDidDisappear(animated);
		//	try
		//	{
		//		this.NavigationController.PopToRootViewController(true);
		//	}
		//	catch (Exception e)
		//	{
				
		//	}
		//}

		public async void refreshData()
		{
			await GetTaskithID(task.Id);
			PlanTable.Source = new TaskDetailTableSource(taskDetail, this);
			PlanTable.ReloadData();
		}

		public override void ViewDidLoad()
		{
			if (task == null)
			{
				UIAlertController okAlertController = UIAlertController.Create("Oops", "Task is NULL. Service Access Layer not implemented", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(okAlertController, true, null);
				return;
			}

			base.ViewDidLoad();

			var estimated = TimeSpan.FromMinutes(task.EstimatedTime);
			var actual = TimeSpan.FromMinutes(task.ActualTime);
			string[] tableItems = new string[]{estimated.ToString("hh\\:mm"), actual.ToString("hh\\:mm"), task.CompletionDate.ToString("MM/dd/yyyy") };

			Console.WriteLine("estimated time: " + task.EstimatedTime);
			Console.WriteLine("actual time: " + task.ActualTime);
			Console.WriteLine("complete time: " + task.CompletionDate);
			refreshData();

			PlanTable.AutoresizingMask = UIViewAutoresizing.All;

			View.AddSubview(PlanTable);

			TdTaskNameLb.Text = task.FullName ?? "";
			tdProjectNameBtn.SetTitle(task.Project != null ? task.Project.Name : "",UIControlState.Normal);
			tdProjectNameBtn.SetTitleColor(UIColor.Black, UIControlState.Normal);

			TdNotesTf.Text = task.TaskNote ?? "";
		}
    }
}