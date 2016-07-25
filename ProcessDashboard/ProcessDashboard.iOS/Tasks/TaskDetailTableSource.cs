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
		Task TaskItem;
		protected string cellIdentifier = "Cell";
		UIViewController owner;

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

				int newHour = (int)TaskItem.estimatedTime / 60;
				int newMin = (int)TaskItem.estimatedTime % 60;
				string newM = null;

				if (newMin < 10)
				{
					newM = "0" + newMin.ToString();
				}
				else {
					newM = newMin.ToString();
				}

				string newEstimatedTime = newHour + ":" + newM;

				cell.DetailTextLabel.Text = newEstimatedTime;
			}
			else if (indexPath.Row == 1)
			{
				cell.TextLabel.Text = "Actual Time";
				int newHour = (int)TaskItem.actualTime / 60;
				int newMin = (int)TaskItem.actualTime % 60;
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
				if (TaskItem.completionDate.ToShortDateString().Equals("1/1/0001"))
				{
					cell.DetailTextLabel.Text = "--";
				}
				else 
				{
					cell.DetailTextLabel.Text = TaskItem.completionDate.ToShortDateString();
					//Console.WriteLine("task completion date:" + TaskItem.completionDate.ToShortDateString());
				}
			}

			return cell;
		}
	}
}

