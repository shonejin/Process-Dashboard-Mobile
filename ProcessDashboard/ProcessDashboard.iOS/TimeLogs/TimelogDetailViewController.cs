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
		public TaskTimeLogViewController DelegateforAddingTimelog { get; set;}
		UILabel TaskNameLabel, ProjectNameLabel, StartTimeLabel, DeltaLabel, IntLabel, CommentLabel;
		UIButton StartTimeText;
		UITextField DeltaText, IntText;
		UIButton CommentText;
		UIBarButtonItem delete;
		UIBarButtonItem doneButton;
		private DateTime[] _customDates;
		UIPickerView DeltaPicker;
		UIPickerView IntPicker;
		string deltaSelectedHour, deltaSelectedMinute;
		string intSelectedHour, intSelectedMinute;
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


			ProjectNameLabel = new UILabel(new CGRect(30, 100, 300, 20))
			{
				Text = currentTask.task.project.name,
				Font = UIFont.SystemFontOfSize(16),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
			//	BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			ProjectNameLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			TaskNameLabel = new UILabel(new CGRect(30, 130, 300, 60))
			{
				Text = currentTask.task.fullName,
				Font = UIFont.SystemFontOfSize(14),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
			//	BackgroundColor = UIColor.FromRGB(220, 220, 220),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			TaskNameLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			// 
			StartTimeLabel = new UILabel(new CGRect(30, 210, 300, 20))
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

			StartTimeText.Frame = new CGRect(30, 240, 300, 20);

			StartTimeText.SetTitleColor(UIColor.Blue, UIControlState.Normal);

			StartTimeText.TitleLabel.SizeToFit();

			StartTimeText.TouchUpInside += (sender, e) =>
			{

				DatePickerButtonTapped(sender, e);
			};

			StartTimeText.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			// 
			DeltaLabel = new UILabel(new CGRect(30, 270, 300, 20))
			{

				Text = "Delta",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),

			};

			DeltaLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;


			/// 
			DeltaText = new UITextField(new CGRect(30, 310, 300, 20));

			int newHour =  (int)currentTask.loggedTime / 60;
			int newMin = (int)currentTask.loggedTime % 60;
			string newLoggedTime = newHour + ":" + newMin;

			DeltaText.Text = newLoggedTime;

			DeltaText.TextAlignment = UITextAlignment.Center;
			DeltaText.TextColor = UIColor.Blue;
			DeltaText.Font = UIFont.SystemFontOfSize(14);

			DeltaText.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;


			////
			IntLabel = new UILabel(new CGRect(30, 340, 300, 20))
			{
				Text = "Int",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
			};

			IntLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;


			/////
			IntText = new UITextField(new CGRect(30, 370, 300, 20));

			int newH = (int)currentTask.interruptTime / 60;
			int newM = (int)currentTask.interruptTime % 60;
			string tempH = null, tempM = null;
			if (newH < 10)
			{
				tempH = "0" + newH.ToString();
			}
			if (newM < 10)
			{
				tempM = "0" + newM.ToString();
			}
			else {
				tempH = newH.ToString();
				tempM = newM.ToString();
			}

			string newInt = tempH + ":" + tempM;

			IntText.Text = newInt;

			IntText.TextAlignment = UITextAlignment.Center;
			IntText.TextColor = UIColor.Blue;
			IntText.Font = UIFont.SystemFontOfSize(14);

			IntText.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			/////
			CommentLabel = new UILabel(new CGRect(30, 400, 300, 20))
			{
				Text = "Comment",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),

			};
			CommentLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;


			//////
			CommentText = new UIButton(UIButtonType.RoundedRect);
			CommentText.SetTitle(currentTask.task.taskNote, UIControlState.Normal);
			CommentText.Frame = new CGRect(30, 430, 300, 30);
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


			/////Delta Picker 
			DeltaPicker = new UIPickerView(new CoreGraphics.CGRect(10f, this.View.Frame.Height - 250, this.View.Frame.Width - 20, 200f));
			DeltaPicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			DeltaPicker.UserInteractionEnabled = true;
			DeltaPicker.ShowSelectionIndicator = true;


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

			StatusPickerViewModel deltaModel = new StatusPickerViewModel(hours,minutes);

			int h = (int)currentTask.loggedTime / 60;
			int m = (int)currentTask.loggedTime % 60;

			this.deltaSelectedHour = h.ToString();
			this.deltaSelectedMinute = m.ToString();

			deltaModel.NumberSelected += (Object sender,EventArgs e) =>
			{
				this.deltaSelectedHour = deltaModel.selectedHour;
				this.deltaSelectedMinute = deltaModel.selectedMinute;

			};

			DeltaPicker.Model = deltaModel;

			DeltaPicker.Select(h, 0, true);
			DeltaPicker.Select(m, 1, true);

			 //Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.BlackTranslucent;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done,
			(s, e) =>
			{
				
				this.DeltaText.Text = this.deltaSelectedHour + ":" + this.deltaSelectedMinute;
				this.DeltaText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			this.DeltaText.InputView = DeltaPicker;
			this.DeltaText.InputAccessoryView = toolbar;



			////// Int Picker
			IntPicker = new UIPickerView(new CoreGraphics.CGRect(10f, this.View.Frame.Height - 200, this.View.Frame.Width - 20, 200f));
			IntPicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			IntPicker.UserInteractionEnabled = true;
			IntPicker.ShowSelectionIndicator = true;

			IntPicker.Select(0, 0, true);

			StatusPickerViewModel intModel = new StatusPickerViewModel(hours,minutes);


			int hh = (int)currentTask.interruptTime / 60;
			int mm = (int)currentTask.interruptTime % 60;

			this.intSelectedHour = hh.ToString();
			this.intSelectedMinute = mm.ToString();

			intModel.NumberSelected += (Object sender, EventArgs e) =>
			{
				this.intSelectedHour = intModel.selectedHour;
				this.intSelectedMinute = intModel.selectedMinute;
			};

			IntPicker.Model = intModel;

			IntPicker.Select(hh, 0, true);
			IntPicker.Select(mm, 1, true);

			//Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.BlackTranslucent;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done,
			(s, e) =>
			{
				this.IntText.Text = this.intSelectedHour + ":" + this.intSelectedMinute;
				this.IntText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			this.IntText.InputView = IntPicker;
			this.IntText.InputAccessoryView = toolbar;

			View.AddSubview(ProjectNameLabel);
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
		    int newHour = (int)currentTask.loggedTime / 60;
			int newMin = (int)currentTask.loggedTime % 60;
			string newH = null, newM = null;
			if (newHour < 10)
			{
				newH = "0" + newHour.ToString();
			}
			if (newMin < 10)
			{
				newM = "0" + newMin.ToString();
			}
			else {
				newH = newHour.ToString();
				newM = newMin.ToString();
			}
				
			string newLoggedTime = newH + ":" + newM;

			DeltaText.Text = newLoggedTime;

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

		public void CreateTask(TaskTimeLogViewController d, TimeLogEntry task)
		{
			DelegateforAddingTimelog = d;
			currentTask = task;
		}

		//public void DeltaButtonClicked(object sender, UIButtonEventArgs e)
		//{
		//	UIAlertView parent_alert = (UIAlertView)sender;

		//	if (e.ButtonIndex == 1)
		//	{

		//		Console.Out.WriteLine("haha: "+ DeltaText.Text);
		//		DeltaText.Text = parent_alert.GetTextField(0).Text;
		//		//SaveDeltaTimeChanged(parent_alert.GetTextField(0).Text);
		//		TimeLogEntry newTask = new TimeLogEntry();
		//		// TODO: fix the type
		//		//newTask.Heading = currentTask.Heading;
		//		//newTask.SubHeading = currentTask.SubHeading;
		//		//newTask.StartTime = currentTask.StartTime;
		//		//newTask.Delta = parent_alert.GetTextField(0).Text;
		//		//newTask.Int = currentTask.Int;
		//		//newTask.Comment = currentTask.Comment;

		//		//Delegate.SaveTask(currentTask, newTask);

		//		Console.Out.WriteLine(parent_alert.GetTextField(0).Text);

		//	}

		//}

		//public void IntButtonClicked(object sender, UIButtonEventArgs e)
		//{
		//	UIAlertView parent_alert = (UIAlertView)sender;

		//	if (e.ButtonIndex == 1)
		//	{
		//		IntText.TitleLabel.Text = parent_alert.GetTextField(0).Text;
		//		// TODO: fix the type
		//		//TimelogTableItem newTask = new TimelogTableItem();
		//		//newTask.Heading = currentTask.Heading;
		//		//newTask.SubHeading = currentTask.SubHeading;
		//		//newTask.StartTime = currentTask.StartTime;
		//		//newTask.Delta = currentTask.Delta;
		//		//newTask.Int = parent_alert.GetTextField(0).Text;
		//		//newTask.Comment = currentTask.Comment;

		//		//Delegate.SaveTask(currentTask, newTask);


		//	}

		//}


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

				StartTimeText.SetTitle(dateFormatter.ToString(modalPicker.DatePicker.Date), UIControlState.Normal);
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


	}
}