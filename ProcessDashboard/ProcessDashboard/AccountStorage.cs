using System;
using System.Linq;
using Xamarin.Auth;
#if __ANDROID__
using Android.Content;
#endif

namespace ProcessDashboard
{
	/*
	 * https://developer.xamarin.com/recipes/cross-platform/xamarin-forms/general/store-credentials/
	 */
	public class AccountStorage
	{
		private const string AppName = "Process_Dashboard";

#if __ANDROID__
	    private static Context _context;

	    public static void SetContext(Context context)
	    {
	        _context = context;
	    }

#endif


        public static void Set(string userId, string password, string baseUrl, string dataSet)
		{
			ClearStorage();

			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(dataSet))
			{
				throw new FormatException("input parameters contain null or empty strings");
			}
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
            AccountStore.Create(_context).Save(account, AppName);
#endif
		}

		public static void ClearStorage()
		{
			while (true)
			{
#if __ANDROID__
			    var account = AccountStore.Create(_context).FindAccountsForService(AppName);
#else
                Account account = (Account)AccountStore.Create().FindAccountsForService(AppName);
#endif
                if (account.Count() > 0)
				{
#if __ANDROID__
                    AccountStore.Create(_context).Delete(account.First(), AppName);
#else
                    AccountStore.Create().Delete(account, AppName);
#endif
                }
                else
				{
					break;
				}
			}
		}

		public static string UserId
		{
			get
			{
#if __IOS__
                Account account = (Account) AccountStore.Create().FindAccountsForService(AppName);
#else
                Account account =  AccountStore.Create(_context).FindAccountsForService(AppName).ElementAtOrDefault(0);
#endif
                return account?.Username;
			}
		}

		public static string Password
		{
			get
			{
#if __IOS__
                Account account = (Account) AccountStore.Create().FindAccountsForService(AppName);
#else
                Account account = (Account)AccountStore.Create(_context).FindAccountsForService(AppName).ElementAtOrDefault(0);
#endif
                return account?.Properties["Password"];
			}
		}

		public static string BaseUrl
		{
			get
			{
#if __IOS__
                Account account = (Account) AccountStore.Create().FindAccountsForService(AppName);
#else
                Account account = (Account)AccountStore.Create(_context).FindAccountsForService(AppName).ElementAtOrDefault(0);
#endif
                System.Diagnostics.Debug.WriteLine("Base url in storage is :"+ account?.Properties["BaseUrl"]);
                return account?.Properties["BaseUrl"];
			}
		}

		public static string DataSet
		{
			get
			{
                #if __IOS__
				List<Account> accounts = (List<Account>)AccountStore.Create().FindAccountsForService(AppName);
				if (accounts.Count > 0 && accounts[0].Properties.ContainsKey("DataToken"))
				{
					return accounts[0].Properties["DataToken"];
				}
				return null;
#else
                Account account = (Account)AccountStore.Create(_context).FindAccountsForService(AppName).ElementAtOrDefault(0);
#endif
                return account?.Properties["DataSet"];
			}
		}
	}
}

