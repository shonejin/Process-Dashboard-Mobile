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
		UIScrollView scrollView;
		UIButton playButton, pauseButton;
		UIButton ProjectNameBtn, CurrentTaskNameBtn;
		UILabel CurrentTaskLabel;
		List<DTO.Task> RecentTaskItems;
		TimeLoggingController timeLoggingController;


		private DateTime[] _customDates;

		public HomePageViewController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		private void Initialize()
		{
			_customDates = new DateTime[]
			{
				DateTime.Now, DateTime.Now.AddDays(7), DateTime.Now.AddDays(7*2),
				DateTime.Now.AddDays(7*3), DateTime.Now.AddDays(7*4), DateTime.Now.AddDays(7*5),
				DateTime.Now.AddDays(7*6), DateTime.Now.AddDays(7*7), DateTime.Now.AddDays(7*8),
				DateTime.Now.AddDays(7*9), DateTime.Now.AddDays(7*10), DateTime.Now.AddDays(7*11),
				DateTime.Now.AddDays(7*12), DateTime.Now.AddDays(7*13), DateTime.Now.AddDays(7*14)
			};
		}


		public void ProjectLabelOnClick(object sender, EventArgs ea)
		{
			UIAlertController okAlertController = UIAlertController.Create("Oops", "not implemented", UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			PresentViewController(okAlertController, true, null);
			return;
		}

		public void TaskLabelOnClick(object sender, EventArgs ea)
		{ 
			UIAlertController okAlertController = UIAlertController.Create("Oops", "not implemented", UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			PresentViewController(okAlertController, true, null);
			return;
		}

		public void PauseBtnOnClick(object sender, EventArgs ea)
		{
			try
			{
				timeLoggingController.stopTiming();
				pauseButton.Enabled = false;
				playButton.Enabled = true;
			}

			// TODO: we can't received this exception here
			// all exceptions are handled before here
			// if we pass back exceptions, we can't guarentee that we can handle all of them, because some are not called by UI
			// the controller automatically retry saving to the server. If so, what do we show in the UI?
			catch (CannotReachServerException e)
			{
				UIAlertController okAlertController = UIAlertController.Create("Cannot Reach Server", "Please check network connection and server availability and try again", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(okAlertController, true, null);
			}

			return;
		}

		public void PlayBtnOnClick(object sender, EventArgs ea)
		{
			try
			{
				timeLoggingController.startTiming(RecentTaskItems[0].Id);
				playButton.Enabled = false;
				pauseButton.Enabled = true;
			}
			catch (CannotReachServerException e)
			{
				UIAlertController okAlertController = UIAlertController.Create("Cannot Reach Server", "Please check network connection and server availability and try again", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(okAlertController, true, null);
			}

			return;
		}

		public void CheckboxBtnOnClick(object sender, EventArgs ea)
		{
			
			DatePickerButtonTapped(sender, ea);
		}

		public void CheckBtnShowIcon(bool completed)
		{
			if (completed)
			{
				checkButton.SetImage(UIImage.FromBundle("checkbox-checked"), UIControlState.Selected);
			}
			else
			{
				checkButton.SetImage(UIImage.FromBundle("checkbox-empty"), UIControlState.Normal);
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			timeLoggingController = new TimeLoggingController();

			scrollView = new UIScrollView(
			new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
						View.AddSubview(scrollView);

			scrollView.ContentSize = View.Frame.Size;


			ProjectNameBtn = new UIButton();
			ProjectNameBtn.Frame = new CGRect(10, 80, View.Bounds.Width - 20, 40);
			ProjectNameBtn.SetTitle("/ Project / Mobile App I1", UIControlState.Normal);
			ProjectNameBtn.BackgroundColor = UIColor.Gray;            
			ProjectNameBtn.AutoresizingMask = UIViewAutoresizing.All;
			ProjectNameBtn.TouchUpInside += ProjectLabelOnClick;

			CurrentTaskLabel = new UILabel(new CGRect(10, 130, 100, 20))
			{
				Text = "Current Task:",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Left,
				//BackgroundColor = UIColor.LightGray,
			};
			CurrentTaskLabel.AutoresizingMask = UIViewAutoresizing.All;

			CurrentTaskNameBtn = new UIButton();
			CurrentTaskNameBtn.Frame = new CGRect(10, 160, View.Bounds.Width - 20, 60);
			CurrentTaskNameBtn.SetTitle("/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Team Walkthrough", UIControlState.Normal);
			CurrentTaskNameBtn.TitleLabel.Lines = 2;
			CurrentTaskNameBtn.TitleLabel.AdjustsFontSizeToFitWidth = true;
			CurrentTaskNameBtn.TitleLabel.TextAlignment = UITextAlignment.Center;
			CurrentTaskNameBtn.BackgroundColor = UIColor.LightGray;
			CurrentTaskNameBtn.TouchUpInside += TaskLabelOnClick;

			CurrentTaskNameBtn.AutoresizingMask = UIViewAutoresizing.All;

			pauseButton = UIButton.FromType(UIButtonType.RoundedRect);
			pauseButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			pauseButton.SetImage(UIImage.FromBundle("pause-deactivated"), UIControlState.Normal);
			pauseButton.SetImage(UIImage.FromBundle("pause-activated"), UIControlState.Disabled);
			pauseButton.Enabled = false; // init state: not started. pause is activated
			pauseButton.Frame = new CGRect(50, 230, 60, 60);

			pauseButton.TouchUpInside += PauseBtnOnClick;

			playButton = UIButton.FromType(UIButtonType.RoundedRect);
			playButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			playButton.SetImage(UIImage.FromBundle("play-deactivated"), UIControlState.Normal);
			playButton.SetImage(UIImage.FromBundle("play-activated"), UIControlState.Disabled);
			playButton.Enabled = true;	// init state: not started. play can be clicked
			playButton.Frame = new CGRect(120, 230, 60, 60);
			playButton.TouchUpInside += PlayBtnOnClick;
	
			checkButton = UIButton.FromType(UIButtonType.Custom);
			checkButton.Frame = new CGRect(310, 230, 60, 60);
			checkButton.TouchUpInside += CheckboxBtnOnClick;

			var RecentTasksLabel = new UILabel(new CGRect(10, 280, 100, 20))
			{
				Text = "Recent Tasks:",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Left
			};

			RecentTasksLabel.AutoresizingMask = UIViewAutoresizing.All;

			RecentTaskTable = new UITableView(new CGRect(0, 310, View.Bounds.Width, View.Bounds.Height - 310 ));
			refreshData();
			RecentTaskTable.AutoresizingMask = UIViewAutoresizing.All;

			scrollView.AddSubview(RecentTaskTable);
			scrollView.AddSubview(playButton);
			scrollView.AddSubview(pauseButton);
			scrollView.AddSubview(checkButton);
			scrollView.AddSubview(ProjectNameBtn);
			scrollView.AddSubview(CurrentTaskLabel);
			scrollView.AddSubview(RecentTaskTable);
			scrollView.AddSubview(RecentTasksLabel);
			scrollView.AddSubview(CurrentTaskNameBtn);

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			refreshData();
		}

		public async void refreshData()
		{
			await GetRecentTasksData();

			ProjectNameBtn.TitleLabel.Text = RecentTaskItems[0].Project.Name;
			CurrentTaskNameBtn.TitleLabel.Text = RecentTaskItems[0].FullName;

			RecentTaskTable.Source = new TaskTableSource(RecentTaskItems.GetRange(1,RecentTaskItems.Count-1), this);
			RecentTaskTable.ReloadData();
		}

		public async System.Threading.Tasks.Task<int> GetRecentTasksData()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);
			List<DTO.Task> projectsList = await c.GetRecentTasks(Settings.GetInstance().Dataset);
			RecentTaskItems = projectsList;

			try
			{
				System.Diagnostics.Debug.WriteLine("** LIST OF RECENT TASKS **");
				System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

				foreach (var proj in projectsList.Select(x => x.FullName))
				{
					System.Diagnostics.Debug.WriteLine(proj);

				}

			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
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

			if (checkButton.Selected == true)
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

				if (checkButton.Selected == false)
				{
					checkButton.Selected = !checkButton.Selected;
					var apiService = new ApiTypes(null);
					var service = new PDashServices(apiService);
					Controller c = new Controller(service);
					//List<DTO.Task> projectsList = await //GetRecentTasks(Settings.GetInstance().Dataset);
				}
				//Console.WriteLine("after" + checkBtnStatus);
				CurrentTaskLabel.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);



			};

			// When user clicks Cancel and the task has been mark as completed
			if (checkButton.Selected == true)
			{
				checkButton.Selected = !checkButton.Selected;
			}
				
			await PresentViewControllerAsync(modalPicker, true);
		}


		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			// set the View Controller that’s powering the screen we’re
			// transitioning to task detail screen
			if (segue.Identifier == "home2taskDetailsSegue")
			{
				TaskDetailsViewController controller = (TaskDetailsViewController)segue.DestinationViewController;
				controller.task = ((TaskTableSource)RecentTaskTable.Source).selectedTask;
			}

		}


    }
}