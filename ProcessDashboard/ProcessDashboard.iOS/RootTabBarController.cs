using Foundation;
using System;
using UIKit;

namespace ProcessDashboard.iOS
{
	public partial class RootTabBarController : UITabBarController
	{

		public RootTabBarController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.ViewControllerSelected += (s, e) =>
			{
				var navController = e.ViewController as UINavigationController;
				if (navController != null)
				{
					navController.PopToRootViewController(animated: false);
				}
			};
		}
	}
}
