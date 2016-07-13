using System;
using CoreGraphics;
using Foundation;
using UIKit;


namespace ProcessDashboard.iOS
{
	public class TaskTableSource : UITableViewSource
	{

		string[] TableItems;
		string CellIdentifier = "TableCell";
		HomePageViewController tvcontroller;

		public TaskTableSource(string[] items, HomePageViewController tvcontroller)
		{
			this.tvcontroller = tvcontroller;
			TableItems = items;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{

			return TableItems.Length;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			tvcontroller.PerformSegue("home2taskDetailsSegue", indexPath);
			tableView.DeselectRow(indexPath, true);
		}

		public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
		{
			tvcontroller.PerformSegue("home2taskDetailsSegue", indexPath);
			tableView.DeselectRow(indexPath, true);
		
		}
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{

			var cell = tableView.DequeueReusableCell(CellIdentifier);
			string item = TableItems[indexPath.Row];
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

			//populate the cell with the appropriate data based on the indexPath
			cell.TextLabel.Text = item;
			cell.TextLabel.Font = UIFont.SystemFontOfSize(12);
			cell.TextLabel.Lines = 1;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.TextLabel.TextColor = UIColor.Black;
			cell.TextLabel.LineBreakMode = UILineBreakMode.HeadTruncation;
			return cell;


		}
	}
}


