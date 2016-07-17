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
	public partial class TimelogDetailViewController : UIViewController
	{
		TimeLogEntry currentTask { get; set; }
		CustomTimeLogCell currentCell { get; set; }
		public TimeLogPageViewController Delegate { get; set; } // will be used to Save, Delete later
		public TaskTimeLogViewController DelegateforTasktimelog { get; set; } // will be used to Save, Delete later
		public TaskDetailsViewController DelegateforAddingTimelog { get; set;}
		UILabel TaskNameLabel, StartTimeLabel, DeltaLabel, IntLabel, CommentLabel;
		UIButton StartTimeText, IntText;
		UILabel DeltaText;
		UIButton CommentText;
		UIBarButtonItem delete;
		UIBarButtonItem doneButton;
		private DateTime[] _customDates;
		UIPickerView samplePicker;
		string selectedhour, selectedminute;
		UIToolbar toolbar;

		public TimelogDetailViewController(IntPtr handle) : base(handle)
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


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			delete = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (s, e) =>
			{


				UIAlertController actionSheetAlert = UIAlertController.Create(null, "This time log will be deleted.", UIAlertControllerStyle.ActionSheet);

				// Add Actions
				actionSheetAlert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Destructive, (action) => Delegate.DeleteTask(currentTask)));

				actionSheetAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (action) => Console.WriteLine("Cancel button pressed.")));

				UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
				if (presentationPopover != null)
				{
					presentationPopover.SourceView = this.View;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
				}


				// Display the alert
				this.PresentViewController(actionSheetAlert, true, null);
				//delete the task time log
	

			});

			NavigationItem.RightBarButtonItem = delete;

			TaskNameLabel = new UILabel(new CGRect(30, 100, 300, 60))
			{
				Text = currentTask.task.fullName,
				Font = UIFont.SystemFontOfSize(14),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.LightTextColor,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			TaskNameLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			// 
			StartTimeLabel = new UILabel(new CGRect(30, 180, 300, 20))
			{
				Text = "Start Time",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
			};

			StartTimeLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			StartTimeText = new UIButton(UIButtonType.RoundedRect);

			StartTimeText.SetTitle(currentTask.startDate.ToShortDateString()  + " " + currentTask.startDate.ToShortTimeString(), UIControlState.Normal);

			StartTimeText.Frame = new CGRect(30, 220, 300, 20);

			StartTimeText.TitleLabel.SizeToFit();

			StartTimeText.TouchUpInside += (sender, e) =>
			{

				DatePickerButtonTapped(sender, e);
			};

			StartTimeText.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			// 
			DeltaLabel = new UILabel(new CGRect(30, 250, 300, 20))
			{

				Text = "Delta",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),

			};

			DeltaLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			DeltaText = new UILabel(new CGRect(10, 290, 300, 20));

			//int h = 0, m = 0;
			//h = (int)currentTask.loggedTime / 60;
			//m = (int)currentTask.loggedTime % 60;

			DeltaText.Text = currentTask.loggedTime.ToString();

			DeltaText.TextAlignment = UITextAlignment.Center;

			UITapGestureRecognizer tgrLabel = new UITapGestureRecognizer(() =>
			{
				samplePicker.Hidden = false;
				this.DeltaText.Text = this.selectedhour + " : " + this.selectedminute;

			});

			DeltaText.AddGestureRecognizer(tgrLabel);
			DeltaText.UserInteractionEnabled = true;

			DeltaText.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			IntLabel = new UILabel(new CGRect(30, 320, 300, 20))
			{
				Text = "Int",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
			};

			IntLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			IntText = new UIButton(UIButtonType.RoundedRect);
			IntText.Frame = new CGRect(30, 360, 300, 20);
			IntText.SetTitle(currentTask.interruptTime.ToString(), UIControlState.Normal);
			IntText.TitleLabel.SizeToFit();

			IntText.TouchUpInside += (sender, e) =>
			{

				UIAlertView alert = new UIAlertView();
				alert.Title = "Interrupt";
				alert.AddButton("Cancel");
				alert.AddButton("Save");
				alert.Message = "Please enter interrupt time";
				alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
				alert.Clicked += IntButtonClicked;
				alert.Show();
			};

			IntText.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;


			//
			CommentLabel = new UILabel(new CGRect(30, 390, 300, 20))
			{
				Text = "Comment",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),

			};
			CommentLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			//
			CommentText = new UIButton(UIButtonType.RoundedRect);
			CommentText.SetTitle(currentTask.task.taskNote, UIControlState.Normal);
			CommentText.Frame = new CGRect(30, 420, 300, 100);
			CommentText.TitleLabel.SizeToFit();

			CommentText.TouchUpInside += (sender, e) =>
			{

				UIAlertView alert = new UIAlertView();
				alert.Title = "Comment";
				alert.AddButton("Cancel");
				alert.AddButton("Save");
				alert.Message = "Please enter new Comment";
				alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
				alert.Clicked += CommentButtonClicked;
				alert.Show();
			};

			CommentText.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			samplePicker = new UIPickerView(new CoreGraphics.CGRect(10f, this.View.Frame.Height - 250, this.View.Frame.Width - 20, 250f));
			samplePicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			samplePicker.UserInteractionEnabled = true;
			samplePicker.ShowSelectionIndicator = true;

			samplePicker.Select(0, 0, true);

			string[] hours = new string[24];
			string[] minutes = new string[60];

			for (int i = 0; i < hours.Length; i++)
			{
				if (i < 10)
				{
					hours[i] = "0" + i.ToString();
				}
				else
				{
					hours[i] = i.ToString();
				}


			}
			for (int i = 0; i < minutes.Length; i++)
			{
				if (i < 10)
				{
					minutes[i] = "0" + i.ToString();
				}
				else {
					minutes[i] = i.ToString();
				}
			}

			StatusPickerViewModel model = new StatusPickerViewModel(hours, minutes);

			model.NumberSelected += (Object sender,EventArgs e) =>
			{
				this.selectedhour = model.selectedHour;
				this.selectedminute = model.selectedMinute;
				this.DeltaText.Text = this.selectedhour + " : " + this.selectedminute;
			};

			samplePicker.Model = model;
			samplePicker.Hidden = true;

			this.View.AddSubview(samplePicker);


			View.AddSubview(TaskNameLabel);
			View.AddSubview(StartTimeLabel);
			View.AddSubview(StartTimeText);
			View.AddSubview(DeltaLabel);
			View.AddSubview(DeltaText);
			View.AddSubview(IntLabel);
			View.AddSubview(IntText);
			View.AddSubview(CommentLabel);
			View.AddSubview(CommentText);

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			TaskNameLabel.Text = currentTask.task.fullName;
			StartTimeText.TitleLabel.Text = currentTask.startDate.ToLongTimeString();
			DeltaText.Text = currentTask.loggedTime.ToString();

		}

		// this will be called before the view is displayed
		public void SetTask(TimeLogPageViewController d, TimeLogEntry task)
		{
			Delegate = d;
			currentTask = task;
		}

		public void SetTaskforTaskTimelog(TaskTimeLogViewController d, TimeLogEntry task)
		{
			DelegateforTasktimelog = d;
			currentTask = task;
		}

		public void CreateTask(TaskDetailsViewController d, TimeLogEntry task)
		{
			DelegateforAddingTimelog = d;
			currentTask = task;
		}

		public void DeltaButtonClicked(object sender, UIButtonEventArgs e)
		{
			UIAlertView parent_alert = (UIAlertView)sender;

			if (e.ButtonIndex == 1)
			{

				Console.Out.WriteLine("haha: "+ DeltaText.Text);
				DeltaText.Text = parent_alert.GetTextField(0).Text;
				//SaveDeltaTimeChanged(parent_alert.GetTextField(0).Text);
				TimeLogEntry newTask = new TimeLogEntry();
				// TODO: fix the type
				//newTask.Heading = currentTask.Heading;
				//newTask.SubHeading = currentTask.SubHeading;
				//newTask.StartTime = currentTask.StartTime;
				//newTask.Delta = parent_alert.GetTextField(0).Text;
				//newTask.Int = currentTask.Int;
				//newTask.Comment = currentTask.Comment;

				//Delegate.SaveTask(currentTask, newTask);

				Console.Out.WriteLine(parent_alert.GetTextField(0).Text);

			}

		}

		public void IntButtonClicked(object sender, UIButtonEventArgs e)
		{
			UIAlertView parent_alert = (UIAlertView)sender;

			if (e.ButtonIndex == 1)
			{
				IntText.TitleLabel.Text = parent_alert.GetTextField(0).Text;
				// TODO: fix the type
				//TimelogTableItem newTask = new TimelogTableItem();
				//newTask.Heading = currentTask.Heading;
				//newTask.SubHeading = currentTask.SubHeading;
				//newTask.StartTime = currentTask.StartTime;
				//newTask.Delta = currentTask.Delta;
				//newTask.Int = parent_alert.GetTextField(0).Text;
				//newTask.Comment = currentTask.Comment;

				//Delegate.SaveTask(currentTask, newTask);


			}

		}


		public void CommentButtonClicked(object sender, UIButtonEventArgs e)
		{
			UIAlertView parent_alert = (UIAlertView)sender;

			if (e.ButtonIndex == 1)
			{
				CommentText.TitleLabel.Text = parent_alert.GetTextField(0).Text;

				// TODO: fix the type
				//TimelogTableItem newTask = new TimelogTableItem();
				//newTask.Heading = currentTask.Heading;
				//newTask.SubHeading = currentTask.SubHeading;
				//newTask.StartTime = currentTask.StartTime;
				//newTask.Delta = currentTask.Delta;
				//newTask.Int = currentTask.Int;
				//newTask.Comment = parent_alert.GetTextField(0).Text;
				//Delegate.SaveTask(currentTask, newTask);

			}

		}

		async void DatePickerButtonTapped(object sender, EventArgs e)
		{
			var modalPicker = new ModalPickerViewController(ModalPickerType.Date, " ", this)
			{
				HeaderBackgroundColor = UIColor.FromRGB(220, 220, 220),
				HeaderTextColor = UIColor.Black,
				DoneButtonText = "Save", // it will save the time to the DB
				CancelButtonText = "Cancel", // the icon needs to change

				TransitioningDelegate = new ModalPickerTransitionDelegate(),
				ModalPresentationStyle = UIModalPresentationStyle.Custom
			};


			modalPicker.DatePicker.Mode = UIDatePickerMode.DateAndTime;

			modalPicker.OnModalPickerDismissed += (s, ea) =>
			{
				var dateFormatter = new NSDateFormatter()
				{
					DateFormat = "MM/dd/yyyy hh:mm a"
				};

				StartTimeText.TitleLabel.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
				SaveStartTimeChanged(dateFormatter.ToString(modalPicker.DatePicker.Date));

			};

			// When user clicks Cancel and the task has been mark as completed

			await PresentViewControllerAsync(modalPicker, true);
		}

		public void SaveStartTimeChanged(String time)
		{
			// TODO: fix the type
			//TimelogTableItem newTask = new TimelogTableItem();
			//String[] strs = time.Split(' ');
			//newTask.Heading = currentTask.Heading;
			//newTask.SubHeading = strs[0];
			//newTask.StartTime = strs[1] + " " + strs[2];
			//newTask.Delta = DeltaText.TitleLabel.Text;
			//newTask.Int = currentTask.Int;
			//newTask.Comment = currentTask.Comment;

			//Delegate.SaveTask(currentTask, newTask);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			this.View.AddSubview(samplePicker);
		}

	}
}