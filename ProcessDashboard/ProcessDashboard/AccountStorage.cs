using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;

namespace ProcessDashboard
{
	/*
	 * https://developer.xamarin.com/recipes/cross-platform/xamarin-forms/general/store-credentials/
	 */
	public class AccountStorage
	{
		private const String AppName = "Process_Dashboard";

		public static void Set(String userId, String password, String baseUrl, String dataSet)
		{
			ClearStorage();

			if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(baseUrl) || String.IsNullOrEmpty(dataSet))
			{
				throw new FormatException("input parameters contain null or empty strings");
			}
			else
			{
				Account account = new Account
				{
					Username = userId
				};
				account.Properties.Add("Password", password);
				account.Properties.Add("BaseUrl", baseUrl);
				account.Properties.Add("DataSet", dataSet);

				#if __IOS__
				AccountStore.Create().Save(account, AppName);
				#else
				AccountStore.Create(Forms.Context.Save(Account, AppName);
				#endif
			}
		}

		public static void ClearStorage()
		{
			while (true)
			{
				List<Account> accounts = (List<Account>)AccountStore.Create().FindAccountsForService(AppName);
				if (accounts.Count > 0)
				{
					AccountStore.Create().Delete(accounts[0], AppName);
				}
				else
				{
					break;
				}
			}
		}

		public static string AuthHeader
		{
			get
			{
				String _username = AccountStorage.UserId;
				String _password = AccountStorage.Password;
				var authData = $"{_username}:{_password}";
				return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
			}
		}

		public static string UserId
		{
			get
			{
				List<Account> accounts = (List<Account>) AccountStore.Create().FindAccountsForService(AppName);
				return (accounts.Count > 0) ? accounts[0].Username : null;
			}
		}

		public static string Password
		{
			get
			{
				List<Account> accounts = (List<Account>)AccountStore.Create().FindAccountsForService(AppName);
				return (accounts.Count > 0) ? accounts[0].Properties["Password"] : null;
			}
		}

		public static string BaseUrl
		{
			get
			{
				List<Account> accounts = (List<Account>)AccountStore.Create().FindAccountsForService(AppName);
				return (accounts.Count > 0) ? accounts[0].Properties["BaseUrl"] : null;
			}
		}

		public static string DataSet
		{
			get
			{
				List<Account> accounts = (List<Account>)AccountStore.Create().FindAccountsForService(AppName);
				return (accounts.Count > 0) ? accounts[0].Properties["DataSet"] : null;
			}
		}
	}
}

