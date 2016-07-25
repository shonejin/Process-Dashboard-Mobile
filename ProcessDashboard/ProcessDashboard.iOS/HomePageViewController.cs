using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using SharpMobileCode.ModalPicker;



namespace ProcessDashboard.iOS
{
    public partial class HomePageViewController : UIViewController
    {
		UIScrollView scrollView;
		UIButton startButton, stopButton;
		private DateTime[] _customDates;
		UILabel CurrentTaskLabel;
		UIButton ProjectNameBtnLabel, CurrentTaskNameBtnLabel;
		//Boolean isClicked = true;

		public HomePageViewController (IntPtr handle) : base (handle)
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

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			scrollView = new UIScrollView(
			new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
						View.AddSubview(scrollView);

			scrollView.ContentSize = View.Frame.Size;

			ProjectNameBtnLabel = new UIButton();
			ProjectNameBtnLabel.Frame = new CGRect(10, 80, View.Bounds.Width - 20, 40);
			ProjectNameBtnLabel.SetTitle("/ Project / Mobile App I1", UIControlState.Normal);
			ProjectNameBtnLabel.BackgroundColor = UIColor.Gray;            
			ProjectNameBtnLabel.AutoresizingMask = UIViewAutoresizing.All;
			ProjectNameBtnLabel.TouchUpInside += ProjectLabelOnClick;

			CurrentTaskLabel = new UILabel(new CGRect(10, 130, 100, 20))
			{
				Text = "Current Task:",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Left,
				//BackgroundColor = UIColor.LightGray,
			};

			CurrentTaskLabel.AutoresizingMask = UIViewAutoresizing.All;

			CurrentTaskNameBtnLabel = new UIButton();
			CurrentTaskNameBtnLabel.Frame = new CGRect(10, 160, View.Bounds.Width - 20, 60);
			CurrentTaskNameBtnLabel.SetTitle("/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Team Walkthrough", UIControlState.Normal);
			CurrentTaskNameBtnLabel.TitleLabel.Lines = 2;
			CurrentTaskNameBtnLabel.TitleLabel.AdjustsFontSizeToFitWidth = true;
			CurrentTaskNameBtnLabel.TitleLabel.TextAlignment = UITextAlignment.Center;
			CurrentTaskNameBtnLabel.BackgroundColor = UIColor.LightGray;
			CurrentTaskNameBtnLabel.TouchUpInside += TaskLabelOnClick;

			CurrentTaskNameBtnLabel.AutoresizingMask = UIViewAutoresizing.All;

			stopButton = UIButton.FromType(UIButtonType.RoundedRect);
			stopButton.SetImage(UIImage.FromFile("stop.png"), UIControlState.Normal);
			//stopButton.SetImage(UIImage.FromFile("stop.png"), UIControlState.Disabled);
			//stopButton.TintColor = UIColor.Black;
			stopButton.Frame = new CGRect(50, 230, 40, 40);
			stopButton.TouchUpInside += (sender, e) =>
			{
				stopButton.Enabled = false;
				startButton.Enabled = true;

			};

			startButton = UIButton.FromType(UIButtonType.RoundedRect);
			startButton.SetImage(UIImage.FromFile("start.png"), UIControlState.Normal);
			startButton.SetImage(UIImage.FromFile("start.png"), UIControlState.Disabled);
			//startButton.TintColor = UIColor.Black;
			startButton.Frame = new CGRect(120, 230, 40, 40);
			startButton.TouchUpInside += (sender, e) =>
			{
				startButton.Enabled = false;
				stopButton.Enabled = true;

			};
	
			checkButton = UIButton.FromType(UIButtonType.Custom);
			checkButton.SetImage(UIImage.FromFile("Checkbox0.png"), UIControlState.Normal);
			checkButton.SetImage(UIImage.FromFile("Checkbox1.png"), UIControlState.Selected);

			//checkButton.TintColor = UIColor.Black;
			checkButton.Frame = new CGRect(310, 230, 40, 40);

			//checkButton.TouchUpInside += DatePickerButtonTapped;
			checkButton.TouchUpInside += (sender, e) =>
			{
				//isClicked = !isClicked;
			//	checkButton.Selected = !checkButton.Selected;
				Console.WriteLine("status of checkbutton"+ checkButton.Selected); // True means checked. False means uncheck.
			    DatePickerButtonTapped(sender,e);
				//CustomPickerButtonTapped(sender,e);
			};

			// 
			var RecentTasksLabel = new UILabel(new CGRect(10, 280, 100, 20))
			{
				Text = "Recent Tasks:",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Left,

			};

			RecentTasksLabel.AutoresizingMask = UIViewAutoresizing.All;

			string[] tableItems = new string[] { "/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Draft",
				"/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Draft",
				"/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Team Walkthrough",
				"/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / UI experiment/Refine",
				"/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Task2 / Publish", 
				"/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Refine", 
				"/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Draft", 
				"/ Project / Mobile App I1 / High Level Design Document / View Logic / UI experiment / Publish"};

			RecentTaskTable = new UITableView(new CGRect(0, 310, View.Bounds.Width, View.Bounds.Height - 310 ));
			RecentTaskTable.Source = new TaskTableSource(tableItems,this);

			RecentTaskTable.AutoresizingMask = UIViewAutoresizing.All;

			scrollView.AddSubview(RecentTaskTable);
			scrollView.AddSubview(startButton);
			scrollView.AddSubview(stopButton);
			scrollView.AddSubview(checkButton);
			scrollView.AddSubview(ProjectNameBtnLabel);
			scrollView.AddSubview(CurrentTaskLabel);
			scrollView.AddSubview(RecentTaskTable);
			scrollView.AddSubview(RecentTasksLabel);
			scrollView.AddSubview(CurrentTaskNameBtnLabel);

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
			// transitioning to
			if (segue.Identifier == "home2taskDetailsSegue")
			{
				var detailContoller = segue.DestinationViewController as TaskDetailsViewController;
				var indexPath = (NSIndexPath)sender;
			}
			//if (segue.Identifier == "calendarSegue")
			//{
			//	var calendarContoller = segue.DestinationViewController as CalendarViewController;

			//}

		}


    }
}