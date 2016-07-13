using System;

using UIKit;

namespace ProcessDashboard.iOS
{
	partial class LoginPageViewController : UIViewController
	{
		//Create an event when a authentication is successful
		public event EventHandler OnLoginSuccess;

		public LoginPageViewController(IntPtr handle) : base(handle)
		{
		}

		partial void LoginButton_TouchUpInside(UIButton sender)
		{
			//Validate our Username & Password.
			//This is usually a web service call.
			if (IsUserNameValid() && IsPasswordValid()&& IsServerLinkValid())
			{
				//We have successfully authenticated a the user,
				//Now fire our OnLoginSuccess Event.
				if (OnLoginSuccess != null)
				{
					OnLoginSuccess(sender, new EventArgs());
				}
			}
			else
			{
				new UIAlertView("Login Error", "Bad user name or password", null, "OK", null).Show();
			}
		}

		private bool IsUserNameValid()
		{
			return !String.IsNullOrEmpty(UserNameTextView.Text.Trim());
		}

		private bool IsPasswordValid()
		{
			return !String.IsNullOrEmpty(PasswordTextView.Text.Trim());
		}

		private bool IsServerLinkValid()
		{
			return !String.IsNullOrEmpty(ServerLinkTextView.Text.Trim());
		}

		//public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		//{
		//	// we're passed to orientation that it will rotate to. in our case, we could
		//	// just return true, but this switch illustrates how you can test for the
		//	// different cases
		//	switch (toInterfaceOrientation)
		//	{
		//		case UIInterfaceOrientation.LandscapeLeft:
		//		case UIInterfaceOrientation.LandscapeRight:
		//		case UIInterfaceOrientation.Portrait:
		//		case UIInterfaceOrientation.PortraitUpsideDown:
		//		default:
		//			return true;
		//	}
		//}

		//public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
		//{
		//	base.WillAnimateRotation(toInterfaceOrientation, duration);

		//	// call our helper method to position the controls
		//	//PositionControls(toInterfaceOrientation);
		//}

		//protected void PositionControls(UIInterfaceOrientation toInterfaceOrientation)
		//{
		//	// depending one what orientation we start in, we want to position our controls
		//	// appropriately
		//	switch (toInterfaceOrientation)
		//	{
		//		// if we're switching to landscape
		//		case UIInterfaceOrientation.LandscapeLeft:
		//		case UIInterfaceOrientation.LandscapeRight:

		//			// reposition the buttons
		//			this.AppNameLabel.Frame = new CoreGraphics.CGRect(300, 50, 300, 33);
		//			this.PasswordTextView.Frame = new CoreGraphics.CGRect(300, 100, 300, 33);
		//			this.UserNameTextView.Frame = new CoreGraphics.CGRect(300, 150, 300, 33);
		//			this.ServerLinkTextView.Frame = new CoreGraphics.CGRect(300, 200, 300, 33);
		//			this.LoginButton.Frame = new CoreGraphics.CGRect(400, 250, 300, 33);

		//			break;

		//		// we're switching back to portrait
		//		case UIInterfaceOrientation.Portrait:
		//		case UIInterfaceOrientation.PortraitUpsideDown:

		//			//reposition the buttons
		//			//this.AppNameLabel.Frame = new CoreGraphics.CGRect(200, 50, 300, 33);
		//			//this.PasswordTextView.Frame = new CoreGraphics.CGRect(200, 100, 300, 33);
		//			//this.UserNameTextView.Frame = new CoreGraphics.CGRect(200, 150, 300, 33);
		//			//this.ServerLinkTextView.Frame = new CoreGraphics.CGRect(200, 200, 300, 33);
		//			//this.LoginButton.Frame = new CoreGraphics.CGRect(200, 250, 300, 33);

		//			break;
		//	}
		//}
	}
}
