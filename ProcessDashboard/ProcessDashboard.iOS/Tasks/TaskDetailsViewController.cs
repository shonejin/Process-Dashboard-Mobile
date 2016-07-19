using Foundation;
using System;
using UIKit;
using CoreGraphics;
using ProcessDashboard.DTO;

namespace ProcessDashboard.iOS
{
    public partial class TaskDetailsViewController : UIViewController
    {
		public Task task;
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
				controller.taskId = task.id;
				controller.task = task;
			
			}
			if (segue.Identifier == "AddTimeLogSegue")
			{
				
				var controller = segue.DestinationViewController as TimelogDetailViewController;
				TimeLogEntry newTimeLog = new TimeLogEntry();
				newTimeLog.task = task;
				newTimeLog.startDate = DateTime.Now;
				controller.CreateTask(this, newTimeLog);

			}
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

			string[] tableItems = new string[]{task.estimatedTime.ToString(), task.actualTime.ToString(), task.completionDate.ToString("MM/dd/yyyy") };

			Console.WriteLine("estimated time: " + task.estimatedTime);
			Console.WriteLine("actual time: " + task.actualTime);
			Console.WriteLine("complete time: " + task.completionDate);

			PlanTable.Source = new TaskDetailTableSource(tableItems, this);
			PlanTable.AutoresizingMask = UIViewAutoresizing.All;

			View.AddSubview(PlanTable);

			TdTaskNameLb.Text = task.fullName ?? "";
			tdProjectNameLb.Text = task.project != null ? task.project.name : "";
			TdNotesTf.Text = task.taskNote ?? "";
		}
    }
}