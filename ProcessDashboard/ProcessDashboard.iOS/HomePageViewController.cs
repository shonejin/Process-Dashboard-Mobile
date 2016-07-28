using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using SharpMobileCode.ModalPicker;
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

		public void CompleteBtnOnClick(object sender, EventArgs ea)
		{
			DatePickerButtonTapped(sender, ea);
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
			activityView.Center = View.Center;

			pauseBtn.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
			pauseBtn.Enabled = true;
			pauseBtn.TouchUpInside += PauseBtnOnClick;

			playBtn.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Normal);
			playBtn.Enabled = false;
			playBtn.TouchUpInside += PlayBtnOnClick;

			completeBtn.TouchUpInside += CompleteBtnOnClick;

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
				completeBtn.SetImage(UIImage.FromBundle("checkbox-empty"), UIControlState.Normal);
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
					completeBtn.SetImage(UIImage.FromBundle("checkbox-empty"), UIControlState.Normal);
				}
				completeBtn.Enabled = true;

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

		async void DatePickerButtonTapped(object sender, EventArgs e)
		{
			var modalPicker = new ModalPickerViewController(ModalPickerType.Date, " ", this)
			{
				HeaderBackgroundColor = UIColor.FromRGB(220, 220, 220),
				HeaderTextColor = UIColor.Black,

				TransitioningDelegate = new ModalPickerTransitionDelegate(),
				ModalPresentationStyle = UIModalPresentationStyle.Custom
			};

			if (completeBtn.Selected == true)
			{
				modalPicker.DoneButtonText = "Mark Complete";  // it will save the time to the DB
				modalPicker.CancelButtonText = "Mark Incomplete"; // the icon needs to change
			}
			else 
			{
				modalPicker.DoneButtonText = "Mark Complete";
				modalPicker.CancelButtonText = "Cancel";
			}

			modalPicker.DatePicker.Mode = UIDatePickerMode.DateAndTime;

			modalPicker.OnModalPickerDismissed += (s, ea) =>
			{
				var dateFormatter = new NSDateFormatter()
				{
					DateFormat = "MMMM dd, HH mm"
				};

				if (completeBtn.Selected == false)
				{
				}
			};

			// When user clicks Cancel and the task has been mark as completed
			if (completeBtn.Selected == true)
			{
				completeBtn.Selected = !completeBtn.Selected;
			}
				
			await PresentViewControllerAsync(modalPicker, true);
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