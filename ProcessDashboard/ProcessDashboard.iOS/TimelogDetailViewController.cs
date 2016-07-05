using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Drawing;

namespace ProcessDashboard.iOS
{
    public partial class TimelogDetailViewController : UIViewController
    {
		TimelogTableItem currentTask { get; set; }
		CustomTimeLogCell currentCell { get; set;}
		public TimeLogPageViewController Delegate { get; set; } // will be used to Save, Delete later
		UILabel TaskNameLabel, StartTimeLabel, DeltaLabel, IntLabel, CommentLabel;
		UITextField StartTimeText, DeltaText, IntText;
		UITextView CommentText;

		public TimelogDetailViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			TaskNameLabel = new UILabel(new CGRect(30, 100, 300, 60))
			{
				Text = "/Project/Mobile App I1/High Level Design Document/View Logic/UI Experiment/Team Walkthrough",
				Font = UIFont.SystemFontOfSize(14),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.LightTextColor,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			// 
			StartTimeLabel = new UILabel(new CGRect(30, 180, 300, 20))
			{
				Text = "Start Time",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
			};

			StartTimeText = new UITextField(new CGRect(30, 220, 300, 20))
			{
				Text = "2016-06-02 10:20 AM",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.LightGray,
				TextAlignment = UITextAlignment.Center,
				//BackgroundColor = UIColor.LightGray,

			};

			// 
			DeltaLabel = new UILabel(new CGRect(30, 250, 300, 20))
			{
				Text = "Delta",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),

			};

			DeltaText = new UITextField(new CGRect(30, 290, 300, 20))
			{
				Text = "1:20",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.LightGray,
				TextAlignment = UITextAlignment.Center,
				//BackgroundColor = UIColor.LightGray,

			};


			// 

			IntLabel = new UILabel(new CGRect(30, 320, 300, 20))
			{
				Text = "Int",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220, 220, 220),
			};


			IntText = new UITextField(new CGRect(30, 360, 300, 20))
			{
				Text = "00:00",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.LightGray,
				TextAlignment = UITextAlignment.Center,
				//BackgroundColor = UIColor.LightGray,

			};

			//
			CommentLabel = new UILabel(new CGRect(30, 390, 300, 20))
			{
				Text = "Comment",
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(220,220,220),

			};

			CommentText = new UITextView(new CGRect(30, 420, 300, 100))
			{
				Text = "The comment just for testing.The comment just for testing.The comment just for testing.The comment just for testing.",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.LightGray,
				TextAlignment = UITextAlignment.Left,
				Editable = true,
				//BackgroundColor = UIColor.LightGray,
	
			};

			this.SetToolbarItems(new UIBarButtonItem[] {
				new UIBarButtonItem(UIBarButtonSystemItem.Trash, (s,e) => {

                UIAlertController actionSheetAlert = UIAlertController.Create(null, "This time log will be deleted.", UIAlertControllerStyle.ActionSheet);

                // Add Actions
				actionSheetAlert.AddAction(UIAlertAction.Create("Delete",UIAlertActionStyle.Destructive, (action) => Delegate.DeleteTask(currentTask)));

				actionSheetAlert.AddAction(UIAlertAction.Create("Cancel",UIAlertActionStyle.Cancel, (action) => Console.WriteLine ("Cancel button pressed.")));

                UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
				if (presentationPopover!=null) {
					presentationPopover.SourceView = this.View;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
				}

			
                // Display the alert
                this.PresentViewController(actionSheetAlert,true,null);
						//delete the task time log
				


				})
				, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 }
				, new UIBarButtonItem(UIBarButtonSystemItem.Save, (s,e) => {
					//Console.WriteLine ("Save changed");
				UIAlertController actionSheetAlert = UIAlertController.Create(null, "All changes will be saved.", UIAlertControllerStyle.ActionSheet);

                // Add Actions
				actionSheetAlert.AddAction(UIAlertAction.Create("Save",UIAlertActionStyle.Default, (action) => Console.WriteLine ("Save button pressed.")));

				actionSheetAlert.AddAction(UIAlertAction.Create("Cancel",UIAlertActionStyle.Cancel, (action) => Console.WriteLine ("Cancel button pressed.")));

				UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
				if (presentationPopover!=null) {
					presentationPopover.SourceView = this.View;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
				}

                // Display the alert
                this.PresentViewController(actionSheetAlert,true,null);
				
				// Save the changes
				TaskNameLabel.Text = currentTask.Heading;
				StartTimeText.Text = currentTask.SubHeading + " "+ currentTask.StartTime;
				DeltaText.Text = currentTask.Delta;
				Delegate.SaveTask(currentTask);

				})
			}, false);

			this.NavigationController.ToolbarHidden = false;

			this.Add(TaskNameLabel);
			this.Add(StartTimeLabel);
			this.Add(StartTimeText);
			this.Add(DeltaLabel);
			this.Add(DeltaText);
			this.Add(IntLabel);
			this.Add(IntText);
			this.Add(CommentLabel);
			this.Add(CommentText);

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			TaskNameLabel.Text = currentTask.Heading;
			StartTimeText.Text = currentTask.SubHeading + " "+ currentTask.StartTime;
			DeltaText.Text = currentTask.Delta;

		}

		// this will be called before the view is displayed
		public void SetTask(TimeLogPageViewController d, TimelogTableItem task)
		{
			Delegate = d;
			currentTask = task;
		}

    }
}