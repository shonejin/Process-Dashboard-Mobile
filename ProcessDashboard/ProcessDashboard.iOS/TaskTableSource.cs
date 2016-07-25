using System;
using System.Collections.Generic;
using Foundation;
using ProcessDashboard.DTO;
using UIKit;

namespace ProcessDashboard.iOS
{
	public class TaskTableSource : UITableViewSource
	{

		List<Task> TableItems;
		string CellIdentifier = "TableCell";
		HomePageViewController tvcontroller;
		public Task selectedTask;

		public TaskTableSource(List<Task> items, HomePageViewController tvcontroller)
		{
			this.tvcontroller = tvcontroller;
			TableItems = items;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{

			return TableItems == null ? 0 : TableItems.Count;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			selectedTask = TableItems[indexPath.Row];
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
			string item = TableItems[indexPath.Row].fullName;
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

			//populate the cell with the appropriate data based on the indexPath
			cell.TextLabel.Text = item;
			cell.TextLabel.Font = UIFont.SystemFontOfSize(12);
			cell.TextLabel.Lines = 1;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.TextLabel.TextColor = UIColor.Black;
			cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			return cell;


		}
	}
}


