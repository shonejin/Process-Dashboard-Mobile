using System;
using System.Collections.Generic;
using Foundation;
using ProcessDashboard.DTO;
using UIKit;
using CoreGraphics;

namespace ProcessDashboard.iOS
{
	public class SettingTableSource : UITableViewSource
	{
		public SettingTableSource()
		{
		}
		string[] Item;
		protected string cellIdentifier = "Cell";
		UIViewController owner;
		UITextField InterruptTimeText, ForgottenTimerText;
		UIPickerView InterruptTimePicker, ForgottenTimePicker;
		UIToolbar toolbar;
		UIBarButtonItem saveButton, cancelButton;
		String saveButtonLabel = "Save";
		string planSelectedHour, planSelectedMinute;

		public SettingTableSource(string[] items, UIViewController owner)
		{
			Item = items;
			this.owner = owner;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			// TODO: handling NULL
			return 4;
		}

		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//tableView.DeselectRow(indexPath, true);

			//if (indexPath.Row == 1)
			//{
			//	owner.PerformSegue("TaskTimeLogsSegue", owner);
			//}
			//tableView.DeselectRow(indexPath, true);
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

			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Value1, cellIdentifier);
			cell.TextLabel.Font = UIFont.SystemFontOfSize(13);

			if (indexPath.Row == 0)
			{
				cell.TextLabel.Text = "Logged in as";
				UIToolbar logoutToolbar = new UIToolbar(new CGRect(0, 0, 65, 30));
				logoutToolbar.BarStyle = UIBarStyle.Default;
				logoutToolbar.BackgroundColor = UIColor.White;
				logoutToolbar.Translucent = true;

				UIBarButtonItem item = new UIBarButtonItem("Logout", UIBarButtonItemStyle.Plain,
				(s, e) =>
				{
					UIAlertController actionSheetAlert = UIAlertController.Create(null, "Are you sure you want to log out?", UIAlertControllerStyle.ActionSheet);

					// TODO: logout this user
					actionSheetAlert.AddAction(UIAlertAction.Create("Log out", UIAlertActionStyle.Destructive, (action) => Console.WriteLine("Logout"))); 

					actionSheetAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (action) => Console.WriteLine("Cancel button pressed.")));

					UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
					if (presentationPopover != null)
					{
						presentationPopover.SourceView = this.owner.View;
						presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
					}

					// Display the alert
					this.owner.PresentViewController(actionSheetAlert, true, null);

				});

				logoutToolbar.SetItems(new UIBarButtonItem[] { item }, true);
				cell.AccessoryView = logoutToolbar;

			}
			else if (indexPath.Row == 1)
			{
				cell.TextLabel.Text = "Connect only over WiFi";
				cell.AccessoryView = new UISwitch(new CGRect(0, 0, 100, 25));

			}
			else if (indexPath.Row == 2)
			{

				cell.TextLabel.Text = "Max continuous interrupt time";
				InterruptTimeText = new UITextField(new CGRect(0, 400, 150, 30));
				cell.AccessoryView = InterruptTimeText;
				InterruptTimeText.TextColor = UIColor.LightGray;
				InterruptTimeText.TextAlignment = UITextAlignment.Right;
				newInterruptTimePicker();

			}
			else if (indexPath.Row == 3){
				
				cell.TextLabel.Text = "Forgotten timer threshold";
				ForgottenTimerText = new UITextField(new CGRect(0, 500, 150, 30));
				cell.AccessoryView = ForgottenTimerText;
				ForgottenTimerText.TextColor = UIColor.LightGray;
				ForgottenTimerText.TextAlignment = UITextAlignment.Right;
				newForgottenTimePicker();

			}
			return cell;


		}

		public void newInterruptTimePicker()
		{
			InterruptTimePicker = new UIPickerView(new CoreGraphics.CGRect(0, 300, 400, 200f));
			InterruptTimePicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			InterruptTimePicker.UserInteractionEnabled = true;
			InterruptTimePicker.ShowSelectionIndicator = true;

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

			planModel.NumberSelected += (Object sender, EventArgs e) =>
			{
				this.planSelectedHour = planModel.selectedHour;
				this.planSelectedMinute = planModel.selectedMinute;

			};

			InterruptTimePicker.Model = planModel;
			InterruptTimePicker.BackgroundColor = UIColor.White;
			this.planSelectedHour = "0";
			this.planSelectedMinute = "00";
			this.InterruptTimeText.Text = this.planSelectedHour + ":" + this.planSelectedMinute;
			InterruptTimePicker.Select(0, 0, true);
			InterruptTimePicker.Select(0, 1, true);

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

				this.InterruptTimeText.Text = this.planSelectedHour + ":" + this.planSelectedMinute;
				this.InterruptTimeText.ResignFirstResponder();
			});

			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				this.InterruptTimeText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

			this.InterruptTimeText.InputView = InterruptTimePicker;
			this.InterruptTimeText.InputAccessoryView = toolbar;
		}

	public void newForgottenTimePicker()
		{
			ForgottenTimePicker = new UIPickerView(new CoreGraphics.CGRect(0, 300, 400, 200f));
			ForgottenTimePicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			ForgottenTimePicker.UserInteractionEnabled = true;
			ForgottenTimePicker.ShowSelectionIndicator = true;

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

			planModel.NumberSelected += (Object sender, EventArgs e) =>
			{
				this.planSelectedHour = planModel.selectedHour;
				this.planSelectedMinute = planModel.selectedMinute;

			};

			ForgottenTimePicker.Model = planModel;
			ForgottenTimePicker.BackgroundColor = UIColor.White;
			this.planSelectedHour = "0";
			this.planSelectedMinute = "00";
			this.ForgottenTimerText.Text = this.planSelectedHour + ":" + this.planSelectedMinute;
			ForgottenTimePicker.Select(0, 0, true);
			ForgottenTimePicker.Select(0, 1, true);

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

				this.ForgottenTimerText.Text = this.planSelectedHour + ":" + this.planSelectedMinute;
				this.ForgottenTimerText.ResignFirstResponder();
			});

			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				this.ForgottenTimerText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

			this.ForgottenTimerText.InputView = ForgottenTimePicker;
			this.ForgottenTimerText.InputAccessoryView = toolbar;
		}

	}
}

