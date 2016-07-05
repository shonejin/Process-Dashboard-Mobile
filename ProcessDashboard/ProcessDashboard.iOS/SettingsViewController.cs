using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ProcessDashboard.iOS
{
    public partial class SettingsViewController : UITableViewController
    {
        public SettingsViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//view.BringSubviewToFront = this;

			string[] tableItems = new string[] { "My Account",
				"Security",
				"Clear Cache",
				"Help & Feedback",
				"About TSP",
				"Log out Current Account",};

			SettingTable = new UITableView(new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height));
			SettingTable.Source = new ProjectsTableSource(tableItems, this);

			Add(SettingTable);

			// Perform any additional setup after loading the view, typically from a nib.
		}

    }
}