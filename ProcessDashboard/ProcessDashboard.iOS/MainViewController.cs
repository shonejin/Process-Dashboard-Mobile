using System;
namespace ProcessDashboard.iOS
{
	public class MainViewController : UITabBarController
	{
		#region Computed Properties
		/// <summary>
		/// Returns the delegate for the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate T
		{
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Constructors
		public MainViewController(IntPtr handle) : base(handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.ViewControllerSelected += (sender, e) =>
			{
			// Take action based on the tab being selected
			switch (TabBar.SelectedItem.Title)
				{
					case "Home":
					case "Lights":
					// Make sure the light information is Up-to-date
					ThisApp.AppState.ResyncLights();
						break;
				}
			};
		}
		#endregion
	}
}
