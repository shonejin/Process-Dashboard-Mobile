using System;
using Foundation;
using UIKit;

namespace ProcessDashboard.iOS
{
	public class TaskTimeLogTableSource : UITableViewSource
	{
		protected string[] tableItems;
		protected string cellIdentifier = "projectCell";
		UIViewController owner;

		public TaskTimeLogTableSource(string[] items, UIViewController owner)
		{
			tableItems = items;
			this.owner = owner;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			// Display view
			owner.PerformSegue("eachTimeLogSegue", indexPath);

			tableView.DeselectRow(indexPath, true);
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);

			cell.TextLabel.Text = tableItems[indexPath.Row];
			cell.TextLabel.Font = UIFont.SystemFontOfSize(12);
			cell.TextLabel.Lines = 1;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.TextLabel.TextColor = UIColor.Black;
			cell.TextLabel.LineBreakMode = UILineBreakMode.HeadTruncation;
			return cell;
		}
	}
}
