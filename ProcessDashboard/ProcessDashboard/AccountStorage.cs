using System;
using Xamarin.Auth;

namespace ProcessDashboard
{
	/*
	 * https://developer.xamarin.com/recipes/cross-platform/xamarin-forms/general/store-credentials/
	 */
	public class AccountStorage
	{
		private const String AppName = "Process_Dashboard";

		public void Set(String userId, String password, String baseUrl, String dataSet)
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

		public void ClearStorage()
		{
			while (true)
			{
				Account account = (Account)AccountStore.Create().FindAccountsForService(AppName);
				if (account != null)
				{
					AccountStore.Create().Delete(account, AppName);
				}
				else
				{
					break;
				}
			}
		}

		public string UserId
		{
			get
			{
				Account account = (Account) AccountStore.Create().FindAccountsForService(AppName);
				return (account != null) ? account.Username : null;
			}
		}

		public string Password
		{
			get
			{
				Account account = (Account) AccountStore.Create().FindAccountsForService(AppName);
				return (account != null) ? account.Properties["Password"] : null;
			}
		}

		public string BaseUrl
		{
			get
			{
				Account account = (Account)AccountStore.Create().FindAccountsForService(AppName);
				return (account != null) ? account.Properties["BaseUrl"] : null;
			}
		}

		public string DataSet
		{
			get
			{
				Account account = (Account)AccountStore.Create().FindAccountsForService(AppName);
				return (account != null) ? account.Properties["DataSet"] : null;
			}
		}
	}
}

