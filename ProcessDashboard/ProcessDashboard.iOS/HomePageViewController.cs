using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using ProcessDashboard.DTO;
using ProcessDashboard.Model;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using System.Linq;
using System.Text;
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;


namespace ProcessDashboard.iOS
{
    public partial class HomePageViewController : UIViewController
    {
		Project currentProject;
		DTO.Task currentTask;
		List<DTO.Task> RecentTaskItems;
		TimeLoggingController timeLoggingController;
		Controller c;
		UIActivityIndicatorView activityView;
		DateTime completeTimeSelectedDate ;
		UIToolbar toolbar;
		UIBarButtonItem saveButton, cancelButton;
		UIDatePicker CompleteTimePicker;
		UITextField completedDateText;
		String saveButtonLabel = "Save";

		public HomePageViewController(IntPtr handle) : base(handle)
		{}

		public void ProjectNameBtnOnClick(object sender, EventArgs ea)
		{
			if (currentProject != null)
			{
				PerformSegue("home2TasksSegue", this);
			}
		}

		public void TaskNameBtnOnClick(object sender, EventArgs ea)
		{
			if (currentTask != null)
			{
				PerformSegue("homeTask2TaskDetails", this);
			}
		}

		public void PauseBtnOnClick(object sender, EventArgs ea)
		{
			timeLoggingController.stopTiming();
			pauseBtn.SetImage(UIImage.FromBundle("pause-activated"), UIControlState.Normal);
			pauseBtn.Enabled = false;
			playBtn.SetImage(UIImage.FromBundle("play-deactivated"), UIControlState.Normal);
			playBtn.Enabled = true;
		}

		public void PlayBtnOnClick(object sender, EventArgs ea)
		{
			if (timeLoggingController.wasNetworkAvailable)
			{
				timeLoggingController.startTiming(currentTask.Id);
				pauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
				pauseBtn.Enabled = true;
				playBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
				playBtn.Enabled = false;
			}
			else
			{
				UIAlertController okAlertController = UIAlertController.Create("Oops", "The previous time log is not yet updated to the server. Please try again later.", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(okAlertController, true, null);
				return;
			}
		}


		private void timeLoggingStateChanged(object sender, StateChangedEventArgs ea)
		{
			if (ea.NewState.Equals(TimeLoggingControllerStates.TimeLogCanceled))
			{
				refreshControlButtons();
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			timeLoggingController = TimeLoggingController.GetInstance();
			timeLoggingController.TimeLoggingStateChanged += new StateChangedEventHandler(timeLoggingStateChanged);

			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			c = new Controller(service);

			activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
			activityView.Frame = View.Frame;
			activityView.Center = View.Center;

			pauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
			pauseBtn.Enabled = true;
			pauseBtn.TouchUpInside += PauseBtnOnClick;

			playBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
			playBtn.Enabled = false;
			playBtn.TouchUpInside += PlayBtnOnClick;

			completeBtn.TouchUpInside += delegate
			{
				completedDateText.BecomeFirstResponder();
			};

			projectNameBtn.TitleLabel.TextAlignment = UITextAlignment.Center;
			projectNameBtn.TitleLabel.Lines = 2;
			projectNameBtn.TitleLabel.AdjustsFontSizeToFitWidth = true;

			taskNameBtn.TitleLabel.TextAlignment = UITextAlignment.Center;
			taskNameBtn.TitleLabel.Lines = 3;
			taskNameBtn.TitleLabel.AdjustsFontSizeToFitWidth = true;

			projectNameBtn.TouchUpInside += ProjectNameBtnOnClick;
			taskNameBtn.TouchUpInside += TaskNameBtnOnClick;

			refreshData();
		}

		public void newCompleteDatePicker()
		{
			completedDateText = new UITextField(new CGRect(0, 0, 0, 0));
			View.Add(completedDateText);

			CompleteTimePicker = new UIDatePicker(new CoreGraphics.CGRect(0, this.View.Frame.Height - 250, this.View.Frame.Width, 200f));
			CompleteTimePicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			CompleteTimePicker.UserInteractionEnabled = true;
			CompleteTimePicker.Mode = UIDatePickerMode.DateAndTime;

			//Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.BackgroundColor = UIColor.FromRGB(220, 220, 220);
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			saveButton = new UIBarButtonItem(saveButtonLabel, UIBarButtonItemStyle.Bordered, null);

			saveButton.Clicked += (s, e) =>
			{
				Console.WriteLine(saveButton.Title.ToString());
				if (saveButton.Title.ToString().Equals("Mark Complete"))
				{
					completeBtn.SetImage(UIImage.FromBundle("checkbox-checked"), UIControlState.Normal);
					// TODO: Save the completed Date to datebase
				}
				else if (saveButton.Title.ToString().Equals("Mark InComplete"))
				{
					completeBtn.SetImage(UIImage.FromBundle("checkbox-unchecked"), UIControlState.Normal);
					// TODO: Set the current task complete date as "1/1/0001" and save to database
				}
				else { // saveButton.Title.ToString().Equals("Change Completion Date")
					
					// TODO: Save the new completed Date to datebase
				}
				this.completedDateText.ResignFirstResponder();
			};

			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				Console.WriteLine("Cancel");
				this.completedDateText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

		    completeTimeSelectedDate = currentTask.CompletionDate;

			if (currentTask.CompletionDate.ToShortDateString().Equals("1/1/0001"))
			{
				saveButton.Title = "Mark Complete";
				CompleteTimePicker.SetDate(ConvertDateTimeToNSDate(DateTime.Now), true);
			}
			else if (!currentTask.CompletionDate.ToShortDateString().Equals("1/1/0001"))
			{
				saveButton.Title = "Mark InComplete";
				CompleteTimePicker.SetDate(ConvertDateTimeToNSDate(currentTask.CompletionDate), true);
			}

			CompleteTimePicker.ValueChanged += (Object s, EventArgs e) =>
			{
				if (!currentTask.CompletionDate.ToShortDateString().Equals("1/1/0001"))
				{
					saveButton.Title = "Change Completion Date";

				}
				completeTimeSelectedDate = ConvertNSDateToDateTime((s as UIDatePicker).Date);
			};

			CompleteTimePicker.BackgroundColor = UIColor.White;

			this.completedDateText.InputView = CompleteTimePicker;
			this.completedDateText.InputAccessoryView = toolbar;

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			refreshData();
		}

		private void refreshControlButtons()
		{
			if (currentTask == null)
			{
				pauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
				playBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
				completeBtn.SetImage(UIImage.FromBundle("checkbox-unchecked"), UIControlState.Normal);
				pauseBtn.Enabled = false;
				playBtn.Enabled = false;
				completeBtn.Enabled = false;
			}
			else
			{
				if (currentTask.CompletionDate.Ticks > 0)
				{
					completeBtn.SetImage(UIImage.FromBundle("checkbox-checked"), UIControlState.Normal);
				}
				else
				{
					completeBtn.SetImage(UIImage.FromBundle("checkbox-unchecked"), UIControlState.Normal);
				}
				completeBtn.Enabled = true;

				// set up start time customized UIpicker
				newCompleteDatePicker();

				if (timeLoggingController.isTimerRunning())
				{
					pauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
					playBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
					pauseBtn.Enabled = true;
					playBtn.Enabled = false;
				}
				else
				{ 
					pauseBtn.SetImage(UIImage.FromBundle("pause-activated"), UIControlState.Normal);
					playBtn.SetImage(UIImage.FromBundle("play-deactivated"), UIControlState.Normal);
					pauseBtn.Enabled = false;
					playBtn.Enabled = true;
				}
			}
		}

		public async void refreshData()
		{
			activityView.StartAnimating();
			activityView.Hidden = false;

			projectNameBtn.SetTitle("loading current project ...", UIControlState.Normal);
			taskNameBtn.SetTitle("loading current task ...", UIControlState.Normal);

			try
			{
				await GetRecentTasksData();

				currentProject = RecentTaskItems[0].Project;
				currentTask = RecentTaskItems[0];

				projectNameBtn.SetTitle(currentProject.Name, UIControlState.Normal);
				taskNameBtn.SetTitle(currentTask.FullName, UIControlState.Normal);

				refreshControlButtons();

				recentTaskTableView.Source = new TaskTableSource(RecentTaskItems.GetRange(1, RecentTaskItems.Count - 1), this);
				recentTaskTableView.ReloadData();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			finally
			{
				activityView.StopAnimating();
				activityView.Hidden = true;
			}
		}

		public async System.Threading.Tasks.Task<int> GetRecentTasksData()
		{
			List<DTO.Task> projectsList = await c.GetRecentTasks(Settings.GetInstance().Dataset);
			RecentTaskItems = projectsList;
			return 0;
		}

		public static DateTime ConvertNSDateToDateTime(NSDate date)
		{
			DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0);
			DateTime currentDate = reference.AddSeconds(date.SecondsSinceReferenceDate);
			DateTime localDate = currentDate.ToLocalTime();
			return localDate;
		}

		public static NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - newDate).TotalSeconds);
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);
			if (segue.Identifier == "home2taskDetailsSegue")
			{
				TaskDetailsViewController controller = (TaskDetailsViewController)segue.DestinationViewController;
				controller.task = ((TaskTableSource)recentTaskTableView.Source).selectedTask;
			}
			else if (segue.Identifier.Equals("home2TasksSegue"))
			{
				TasksTableViewController controller = (TasksTableViewController)segue.DestinationViewController;
				controller.projectId = currentProject.Id;
				controller.projectName = currentProject.Name;
			}
			else if (segue.Identifier.Equals("homeTask2TaskDetails"))
			{ 
				TaskDetailsViewController controller = (TaskDetailsViewController)segue.DestinationViewController;
				controller.task = currentTask;
			}
		}
    }
}