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

			string[] tableItems = new string[] { "Logged in as",
				"Connect only over WiFi",
				"Max continuous interrupt time",
				"Forgotten timer threshold"};

			SettingTable = new UITableView(new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height));
			SettingTable.Source = new SettingTableSource(tableItems, this);

			Add(SettingTable);

			// Perform any additional setup after loading the view, typically from a nib.
		}

    }
}