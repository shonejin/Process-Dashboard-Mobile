using Android.App;
using Android.Widget;
using Android.OS;
using ProcessDashboard.Model;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard.Droid
{
	[Activity (Label = "ProcessDashboard.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);

            var apiService = new ApiTypes(null);
            var service = new PDashServices(apiService);
            Controller c = new Controller(service);
           // c.testProject();
            c.testTasks();

		}
	}
}


