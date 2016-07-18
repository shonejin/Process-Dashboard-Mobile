using System;
using System.Collections.Generic;
using Foundation;
using ProcessDashboard.DTO;
using UIKit;

namespace ProcessDashboard.iOS
{
	public class TaskDetailTableSource : UITableViewSource
	{
		public TaskDetailTableSource()
		{
		}
		string[] TableItems;
		protected string cellIdentifier = "Cell";
		UIViewController owner;

		public TaskDetailTableSource(string[] items, UIViewController owner)
		{
			TableItems = items;
			this.owner = owner;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			// TODO: handling NULL
			return TableItems.Length;
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
				cell.DetailTextLabel.Text = TableItems[0];
			}
			else if (indexPath.Row == 1)
			{
				cell.TextLabel.Text = "Actual Time";
				cell.DetailTextLabel.Text = TableItems[1];
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			else {

				cell.TextLabel.Text = "Completed Date";
				cell.DetailTextLabel.Text = TableItems[2];
			}

			return cell;
		}
	}
}

