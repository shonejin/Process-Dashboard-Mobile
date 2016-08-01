using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ProcessDashboard.iOS
{
	partial class LoginPageViewController : UIViewController
	{
		//Create an event when a authentication is successful
		public event EventHandler OnLoginSuccess;
		private DataSetLocationResolver _resolver;

		public LoginPageViewController(IntPtr handle) : base(handle)
		{
			_resolver = new DataSetLocationResolver();
		}

		partial void LoginButton_TouchUpInside(UIButton sender)
		{
			String userId = UserIDTextView.Text.Trim();
			String password = PasswordTextView.Text;
			String dataToken = DataTokenTextView.Text.Trim();

			Boolean dataReady = userId.Length * password.Length * dataToken.Length != 0;

			if (!dataReady)
			{
				new UIAlertView("Invalid Data", "Please make sure all blanks are filled before continue", null, "OK", null).Show();
				return;
			}

			String baseUrl;
			String dataSet;
			try
			{
				_resolver.ResolveFromToken(dataToken, out baseUrl, out dataSet);
			}
			catch (ArgumentException e)
			{
				new UIAlertView("Invalid Data", "DataToken not recognized", null, "OK", null).Show();
				return;
			}

			System.Net.HttpWebResponse resp;
			try
			{
				var req = System.Net.WebRequest.CreateHttp(baseUrl + "api/v1/datasets/" + dataSet + "/");
				req.Method = "GET";
				req.AllowAutoRedirect = false;
				String credential = userId + ":" + password;
				req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credential)));
				resp = (System.Net.HttpWebResponse)req.GetResponse();
			}
			catch (Exception e)
			{
				new UIAlertView("Invalid Data", "DataToken or credentials not recognized. " + e.Message, null, "OK", null).Show();
				return;
			}


			if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				new UIAlertView("Wrong Credentials", "Your user ID or password is not correct", null, "OK", null).Show();
				return;
			}
			else if (resp.StatusCode == System.Net.HttpStatusCode.OK)
			{
				if (resp.GetResponseStream().CanRead)
				{
					byte[] buffer = new byte[10001];
					resp.GetResponseStream().Read(buffer, 0, 10000);
					String responseStr = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
					if (responseStr.Contains("auth-required"))
					{
						new UIAlertView("Wrong Credentials", "Your user ID or password is not correct", null, "OK", null).Show();
						return;
					}
					else if (responseStr.Contains("permission-denied"))
					{
						new UIAlertView("Permission Denied", "You may not have the permission to connect to the given dashboard", null, "OK", null).Show();
						return;
					}
					else if (responseStr.Contains("dataset"))
					{
						AccountStorage.Set(userId, password, baseUrl, dataSet);
						if (OnLoginSuccess != null)
						{
							OnLoginSuccess(sender, new EventArgs());
						}
					}
				}
			}
			else
			{
				new UIAlertView("Invalid Data", "Server is not recognized. Please check the DataToken", null, "OK", null).Show();
				return;
			}
		}

		partial void InfoBtnOnClicked(UIButton sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://www.processdash.com/mobile-login"));
		}
	}
}
