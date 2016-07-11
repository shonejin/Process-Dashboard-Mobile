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
			if (segue.Identifier.Equals("taskDetails2TimeLogs"))
			{
				TaskTimeLogViewController controller = (TaskTimeLogViewController)segue.DestinationViewController;
				controller.taskId = task.id;
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
			tdProjectNameLb.Text = task.project.name;
			TdTaskNameLb.Text = task.fullName;
			TdCompleteLb.Text = task.completionDate.ToString();
			TdPlannedLb.Text = task.estimatedTime.ToString();
			TdActualLb.Text = task.actualTime.ToString();
			TdNotesTf.Text = task.taskNote;
		}
    }
}