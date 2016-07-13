using Foundation;
using System;
using UIKit;
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
				Console.WriteLine("in the task detail.");
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
			TdActualLb.Text = task.actualTime.ToString();
			TdTaskNameLb.Text = task.fullName ?? "";
			TdCompleteLb.Text = task.completionDate.ToString("MM/dd/yyyy");
			TdPlannedLb.Text = task.estimatedTime.ToString();
			TdActualLb.Text = task.actualTime.ToString();

			// TODO: project name always return null

			tdProjectNameLb.Text = task.project != null ? task.project.name : "";
			TdNotesTf.Text = task.taskNote ?? "";
		}
    }
}