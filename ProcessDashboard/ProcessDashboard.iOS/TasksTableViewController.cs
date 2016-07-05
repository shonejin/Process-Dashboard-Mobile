using Foundation;
using System;
using UIKit;

namespace ProcessDashboard.iOS
{
    public partial class TasksTableViewController : UITableViewController
    {
        public TasksTableViewController (IntPtr handle) : base (handle)
        {
        }
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			string[] tableItems = new string[] { "T1 / C2 / D3 / E4 / Plan", "T1 / C2 / D3 / E4 / Code"};
			tasksTableView.Source = new TasksTableSource(tableItems, this);
			tasksTableView.ReloadData();

		}
    }
}