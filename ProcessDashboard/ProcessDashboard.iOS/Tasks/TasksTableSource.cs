using System;
using System.Collections.Generic;
using Foundation;
using ProcessDashboard.DTO;
using UIKit;

namespace ProcessDashboard.iOS
{
	public class TasksTableSource : UITableViewSource
	{
		public TasksTableSource()
		{
		}
		public List<Task> tableItems;
		protected string cellIdentifier = "taskCell";
		public Task selectedTask;
		UIViewController owner;
		//public int firstIncompleteTaskPos = 1 ;
		//bool isFirst = true;

		public TasksTableSource(List<Task> items, UIViewController owner)
		{
			tableItems = items;
			this.owner = owner;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			// TODO: handling NULL
			if (tableItems.Count == 0)
			{
				UIAlertController okAlertController = UIAlertController.Create("No Tasks Found", "", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				owner.PresentViewController(okAlertController, true, null);
			}
			return tableItems == null ? 0 : tableItems.Count;
		}

		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			
			selectedTask = tableItems[indexPath.Row];
			tableView.DeselectRow(indexPath, true);
			owner.PerformSegue("task2TaskDetail", owner);

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

/*
			if (true)
			{
				var attriStr = new NSAttributedString(tableItems[indexPath.Row].fullName, strikethroughStyle: NSUnderlineStyle.Single);
				cell.TextLabel.AttributedText = attriStr;
			}
			else {
				cell.TextLabel.Text = tableItems[indexPath.Row].fullName;
			}
*/
			//if (isFirst && tableItems[indexPath.Row].completionDate.ToShortDateString().Equals("1/1/0001"))
			//{
			//	firstIncompleteTaskPos = indexPath.Row;
			//	Console.WriteLine("Line:" + indexPath.Row);
			//	isFirst = false;
			//}
			//Console.WriteLine(isFirst + "Outside Line:" + indexPath.Row);
			cell.TextLabel.Text = tableItems[indexPath.Row].fullName;
			cell.TextLabel.Font = UIFont.SystemFontOfSize(13);
			cell.TextLabel.Lines = 0;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.TextLabel.TextColor = UIColor.Black;
			cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;


			return cell;
		}
	}
}

