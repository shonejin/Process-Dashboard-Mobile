using System;
using UIKit;

namespace ProcessDashboard.iOS
{
    public partial class ProjectsTableViewController : UITableViewController
    {
        public ProjectsTableViewController (IntPtr handle) : base (handle)
        {
        }
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			string[] tableItems = new string[] { "Mobile Process Dashboard", "Enterprise Server", "Linux Kernel", "Windows X1 Professional", "Siri for macOS" };
			projectsTableView.Source = new ProjectsTableSource(tableItems, this);
			projectsTableView.ReloadData();

		}
    }
}