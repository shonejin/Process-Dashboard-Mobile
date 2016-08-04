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
using System.Drawing;



namespace ProcessDashboard.iOS
{
    public partial class TaskDetailsViewController : UIViewController
    {
		public Task task;
		TimeLoggingController timeLoggingController;
		DateTime completeTimeSelectedDate;
		UIToolbar toolbar;
		UIBarButtonItem saveButton, cancelButton;
		UIDatePicker CompleteTimePicker;
		String saveButtonLabel = "Save";

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

		public async void refreshData()
		{
			try
			{
				task = await PDashAPI.Controller.GetTask(task.Id);
				PlanTable.Source = new TaskDetailTableSource(task, this);
				PlanTable.ReloadData();
				refreshControlButtons();
			}
			catch (Exception e)
			{
				;
			}

		}

		public void MarkComplete(DateTime date)
		{ 
			TdCheckboxBtn.SetImage(UIImage.FromBundle("checkbox-checked"), UIControlState.Normal);
			PDashAPI.Controller.UpdateATask(task.Id, task.EstimatedTime, date, false);
		}

		public void MarkIncomplete()
		{ 
			TdCheckboxBtn.SetImage(UIImage.FromBundle("checkbox-unchecked"), UIControlState.Normal);
			PDashAPI.Controller.UpdateATask(task.Id, task.EstimatedTime, null, true);
		}

		public void ChangeCompleteDate(DateTime date)
		{ 
			PDashAPI.Controller.UpdateATask(task.Id, task.EstimatedTime, date, false);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (task == null)
			{
				return;
			}

			timeLoggingController = TimeLoggingController.GetInstance();
			timeLoggingController.TimeLoggingStateChanged += new StateChangedEventHandler(timeLoggingStateChanged);

			Console.WriteLine(NavigationController.NavigationBar.TopItem.Title);
			if (NavigationController.NavigationBar.TopItem.Title.Equals("Process Dashboard"))
			{
				NavigationController.NavigationBar.TopItem.Title = "Back";
			}

			refreshControlButtons();

			TdCheckboxBtn.TouchUpInside += delegate
			{
				((TaskDetailTableSource)PlanTable.Source).completeDateText.BecomeFirstResponder();
			};

			tdProjectNameBtn.SetTitle(task.Project != null ? task.Project.Name : "", UIControlState.Normal);
			tdProjectNameBtn.SetTitleColor(UIColor.Black, UIControlState.Normal);
			TdTaskNameLb.Text = task.FullName ?? "";

			TdNotesTf.Layer.BorderColor = UIColor.LightGray.CGColor;
			TdNotesTf.Layer.BorderWidth = 2.0f;
			TdNotesTf.Layer.CornerRadius = 10.0f;
			TdNotesTf.Text = task.Note ?? "";

			TimeSpan estimated = TimeSpan.FromMinutes(task.EstimatedTime);
			TimeSpan actual = TimeSpan.FromMinutes(task.ActualTime);
			string[] tableItems = new string[]{estimated.ToString("hh\\:mm"), 
				                                        actual.ToString("hh\\:mm"), 
				                                        task.CompletionDate == null ? "-:-" : task.CompletionDate.Value.ToString("MM/dd/yyyy") };
			PlanTable.Source = new TaskDetailTableSource(task, this);
			refreshData();

			View.AddSubview(PlanTable);
		}

		private void refreshControlButtons()
		{
			if (task == null)
			{
				TdPauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
				TdPlayBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
				TdCheckboxBtn.SetImage(UIImage.FromBundle("checkbox-unchecked"), UIControlState.Normal);
				TdPauseBtn.Enabled = false;
				TdPlayBtn.Enabled = false;
				TdCheckboxBtn.Enabled = false;
			}
			else
			{
				if (task.CompletionDate == null)
				{
					TdCheckboxBtn.SetImage(UIImage.FromBundle("checkbox-unchecked"), UIControlState.Normal);
				}
				else
				{
					TdCheckboxBtn.SetImage(UIImage.FromBundle("checkbox-checked"), UIControlState.Normal);
				}
				TdCheckboxBtn.Enabled = true;

				if (timeLoggingController.IsTimerRunning() && timeLoggingController.GetTimingTaskId() == task.Id)
				{
					TdPauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
					TdPlayBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
					TdPauseBtn.Enabled = true;
					TdPlayBtn.Enabled = false;
				}
				else
				{
					TdPauseBtn.SetImage(UIImage.FromBundle("pause-activated"), UIControlState.Normal);
					TdPlayBtn.SetImage(UIImage.FromBundle("play-deactivated"), UIControlState.Normal);
					TdPauseBtn.Enabled = false;
					TdPlayBtn.Enabled = true;
				}
			}
		}

		private void timeLoggingStateChanged(object sender, StateChangedEventArgs ea)
		{
			if (ea.NewState.Equals(TimeLoggingControllerStates.TimeLogCanceled))
			{
				refreshControlButtons();
			}
		}

		public void PauseBtnOnClick(object sender, EventArgs ea)
		{
			timeLoggingController.StopTiming();
			TdPauseBtn.SetImage(UIImage.FromBundle("pause-activated"), UIControlState.Normal);
			TdPauseBtn.Enabled = false;
			TdPlayBtn.SetImage(UIImage.FromBundle("play-deactivated"), UIControlState.Normal);
			TdPlayBtn.Enabled = true;
		}

		public void PlayBtnOnClick(object sender, EventArgs ea)
		{
			if (timeLoggingController.WasNetworkAvailable)
			{
				timeLoggingController.StartTiming(task.Id);
				TdPauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
				TdPauseBtn.Enabled = true;
				TdPlayBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
				TdPlayBtn.Enabled = false;
			}
			else
			{
				UIAlertController okAlertController = UIAlertController.Create("Oops", "The previous time log is not yet updated to the server. Please try again later.", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(okAlertController, true, null);
				return;
			}
		}


		public void newCompleteDatePicker()
		{

			CompleteTimePicker = new UIDatePicker(new CoreGraphics.CGRect(0, 300, 400, 200f));
			CompleteTimePicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			CompleteTimePicker.UserInteractionEnabled = true;
			CompleteTimePicker.Mode = UIDatePickerMode.DateAndTime;
			CompleteTimePicker.MaximumDate = ConvertDateTimeToNSDate(DateTime.UtcNow.ToLocalTime());

			//Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.BackgroundColor = UIColor.FromRGB(220, 220, 220);
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			saveButton = new UIBarButtonItem(saveButtonLabel, UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				((TaskDetailTableSource)PlanTable.Source).completeDateText.Text = completeTimeSelectedDate.ToShortDateString();
				((TaskDetailTableSource)PlanTable.Source).completeDateText.ResignFirstResponder();
			});


			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				((TaskDetailTableSource)PlanTable.Source).completeDateText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

			completeTimeSelectedDate = task.CompletionDate.Value;

			if (task.CompletionDate == null)
			{
				saveButton.Title = "Mark Complete";
				Console.WriteLine(DateTime.Now.ToString());
				CompleteTimePicker.SetDate(ConvertDateTimeToNSDate(DateTime.Now), true);
			}
			else
			{
				saveButton.Title = "Mark Incomplete";
				CompleteTimePicker.SetDate(ConvertDateTimeToNSDate(task.CompletionDate.Value), true);
			}

			CompleteTimePicker.ValueChanged += (Object s, EventArgs e) =>
			{
				if (task.CompletionDate != null)
				{
					saveButton.Title = "Change Completion Date";

				}
				completeTimeSelectedDate = ConvertNSDateToDateTime((s as UIDatePicker).Date);
			};

			CompleteTimePicker.BackgroundColor = UIColor.White;

			((TaskDetailTableSource)PlanTable.Source).completeDateText.InputView = CompleteTimePicker;
			((TaskDetailTableSource)PlanTable.Source).completeDateText.InputAccessoryView = toolbar;

		}

		public async void DeleteTask(string id, double newTime)
		{

			//var oldTask = globalTimeLogCache.Find(t => t.Task.FullName.Equals(log.Task.FullName));
			await TaskUpdateEstimatedTime(id, newTime);
			//NavigationController.PopViewController(true);
		}

		public async System.Threading.Tasks.Task<int> TaskUpdateEstimatedTime(string taskId, double estimatedTime)
		{
			var tr =await PDashAPI.Controller.UpdateATask(taskId, estimatedTime, null, false);

			return 0;

		}


		public static DateTime ConvertNSDateToDateTime(NSDate date)
		{
			DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Local);
			DateTime currentDate = reference.AddSeconds(date.SecondsSinceReferenceDate);
			return currentDate;
		}

		public static NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Local);
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - newDate).TotalSeconds);
		}

		public void changeCheckBoxImage(String imageName)
		{
			//Console.WriteLine("hahaha");
			this.TdCheckboxBtn.SetImage(UIImage.FromBundle(imageName), UIControlState.Normal);
		}
			
    }
}