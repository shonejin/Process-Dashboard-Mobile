using System;
using System.Collections.Generic;
using Foundation;
using ProcessDashboard.DTO;
using UIKit;
using CoreGraphics;

namespace ProcessDashboard.iOS
{
	public class TaskDetailTableSource : UITableViewSource
	{
		public TaskDetailTableSource()
		{
		}
		Task TaskItem;
		protected string cellIdentifier = "Cell";
		UIViewController owner;
		public UITextField completeDateText, planTimeText;
		DateTime completeTimeSelectedDate;
		UIToolbar toolbar;
		UIBarButtonItem saveButton, cancelButton;
		UIDatePicker CompleteTimePicker;
		UIPickerView PlanTimePicker;
		String saveButtonLabel = "Save";
		string planSelectedHour, planSelectedMinute;
		TaskDetailsViewController controller;

		public TaskDetailTableSource(Task items, UIViewController owner)
		{
			TaskItem = items;
			this.owner = owner;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			// TODO: handling NULL
			return 3;
		}

		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow(indexPath, true);

			if (indexPath.Row == 1)
			{
				owner.PerformSegue("TaskTimeLogsSegue", owner);
			}
			tableView.DeselectRow(indexPath, true);
			/*
			UIAlertController okAlertController = UIAlertController.Create("Row Selected", tableItems[indexPath.Row], UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			owner.PresentViewController(okAlertController, true, null);

			
			*/
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
			// if there are no cells to reuse, create a new one
		//	string item = TableItems[indexPath.Row];

			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Value1, cellIdentifier);

			if (indexPath.Row == 0)
			{
				cell.TextLabel.Text = "Planned Time";
				planTimeText = new UITextField(new CGRect(0, 300, 150, 30));
				cell.AccessoryView = planTimeText;
				planTimeText.TextColor = UIColor.LightGray;
				planTimeText.TextAlignment = UITextAlignment.Right;
				newPlanTimePicker();

				int newHour = (int)TaskItem.EstimatedTime / 60;
				int newMin = (int)TaskItem.EstimatedTime % 60;
				string newM = null;

				if (newMin < 10)
				{
					newM = "0" + newMin.ToString();
				}
				else {
					newM = newMin.ToString();
				}

				string newEstimatedTime = newHour + ":" + newM;

				planTimeText.Text = newEstimatedTime;

			}
			else if (indexPath.Row == 1)
			{
				cell.TextLabel.Text = "Actual Time";
				int newHour = (int)TaskItem.ActualTime / 60;
				int newMin = (int)TaskItem.ActualTime % 60;
				string newM = null;
				if (newMin < 10)
				{
					newM = "0" + newMin.ToString();
				}
				else {
					newM = newMin.ToString();
				}
				string newActualTime = newHour + ":" + newM;
				cell.DetailTextLabel.Text = newActualTime;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			else {

				cell.TextLabel.Text = "Completed Date";
				completeDateText = new UITextField(new CGRect(0, 400, 150, 30));
				cell.AccessoryView = completeDateText;
				completeDateText.TextColor = UIColor.LightGray;
				completeDateText.TextAlignment = UITextAlignment.Right;
				newCompleteDatePicker();

				if (!TaskItem.CompletionDate.HasValue)
				{
					completeDateText.Text = "--";
				}
				else 
				{
					completeDateText.Text = TaskItem.CompletionDate.Value.ToShortDateString();

				}
			}

			return cell;
		}

		public void newPlanTimePicker()
		{
			PlanTimePicker = new UIPickerView(new CoreGraphics.CGRect(0, 300, 400, 200f));
			PlanTimePicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			PlanTimePicker.UserInteractionEnabled = true;
			PlanTimePicker.ShowSelectionIndicator = true;

			string[] hours = new string[24];
			string[] minutes = new string[60];

			for (int i = 0; i < hours.Length; i++)
			{
				hours[i] = i.ToString();

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

			StatusPickerViewModel planModel = new StatusPickerViewModel(hours, minutes);

			int h = (int)TaskItem.EstimatedTime / 60;
			int m = (int)TaskItem.EstimatedTime % 60;
			this.planSelectedHour = h.ToString();

			if (m < 10)
			{
				this.planSelectedMinute = "0" + m.ToString();
			}
			else {
				this.planSelectedMinute = m.ToString();
			}

			planModel.NumberSelected += (Object sender, EventArgs e) =>
			{
				this.planSelectedHour = planModel.selectedHour;
				this.planSelectedMinute = planModel.selectedMinute;

			};

			PlanTimePicker.Model = planModel;
			PlanTimePicker.BackgroundColor = UIColor.White;
			PlanTimePicker.Select(h, 0, true);
			PlanTimePicker.Select(m, 1, true);

			//Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.BackgroundColor = UIColor.FromRGB(220, 220, 220);
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar

			saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{

				this.planTimeText.Text = this.planSelectedHour + ":" + this.planSelectedMinute;
				this.planTimeText.ResignFirstResponder();
			});

			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				this.planTimeText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

			this.planTimeText.InputView = PlanTimePicker;
			this.planTimeText.InputAccessoryView = toolbar;
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
			saveButton = new UIBarButtonItem(saveButtonLabel, UIBarButtonItemStyle.Bordered, null);

			saveButton.Clicked += (s, e) =>
			{
				Console.WriteLine(saveButton.Title.ToString());
				if (saveButton.Title.ToString().Equals("Mark Complete"))
				{
					//controller.changeCheckBoxImage("checkbox-checked");
					// TODO: Save the completed Date to datebase
				}
				else if (saveButton.Title.ToString().Equals("Mark InComplete"))
				{
					//controller.changeCheckBoxImage("checkbox-unchecked");
					// TODO: Set the current task complete date as "1/1/0001" and save to database
				}
				else { // saveButton.Title.ToString().Equals("Change Completion Date")
					
					// TODO: Save the new completed Date to datebase
				}
				this.completeDateText.Text = completeTimeSelectedDate.ToShortDateString();
				this.completeDateText.ResignFirstResponder();

			};
			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				this.completeDateText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

			if (TaskItem.CompletionDate == null)
			{
				saveButton.Title = "Mark Complete";
				CompleteTimePicker.SetDate(ConvertDateTimeToNSDate(DateTime.Now), true);
			}
			else
			{
				saveButton.Title = "Mark InComplete";
				CompleteTimePicker.SetDate(ConvertDateTimeToNSDate(TaskItem.CompletionDate.Value), true);
			}

			CompleteTimePicker.ValueChanged += (Object s, EventArgs e) =>
			{
				if (TaskItem.CompletionDate != null)
				{
					saveButton.Title = "Change Completion Date";

				}
				completeTimeSelectedDate = ConvertNSDateToDateTime((s as UIDatePicker).Date);
			};

			CompleteTimePicker.BackgroundColor = UIColor.White;

			this.completeDateText.InputView = CompleteTimePicker;
			this.completeDateText.InputAccessoryView = toolbar;

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
	}
}

