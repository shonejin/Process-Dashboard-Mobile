using System;
namespace ProcessDashboard.iOS
{
	
	public class HomeNavigationController : UINavigationController
	{
		HomePageViewController _firstController;
		public override void ViewDidLoad()
		{
			_firstController = new HomePageViewController();
			PushViewController(_firstController, true);
			base.ViewDidLoad();
		}
	}
}

