using Foundation;
using System;
using UIKit;
using ProcessDashboard.Model;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProcessDashboard.iOS
{
    public partial class TasksTableViewController : UITableViewController
    {
		string[] tableItems;
		string test;

        public TasksTableViewController (IntPtr handle) : base (handle)
        {
        }
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			getDataOfTask();
			Console.WriteLine("hehe:" + test);

			tableItems = new string[] {test, "T1 / C2 / D3 / E4 / Code"};
			tasksTableView.Source = new TasksTableSource(tableItems, this);
			tasksTableView.ReloadData();

		}

		public async void getDataOfTask()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);
			List<DTO.Task> tasksList = await c.GetTasks("mock", "iokdum2d");

			try
			{
				System.Diagnostics.Debug.WriteLine("** GET TASKS **");
				System.Diagnostics.Debug.WriteLine("Length is " + tasksList.Count);

				//test = new String[tasksList.Count];
				//int i = 0;
				//System.Diagnostics.Debug.WriteLine("haha:" + tableItems[0].GetType());
			
				foreach (var task in tasksList.Select(x => x.fullName))
				{
					System.Diagnostics.Debug.WriteLine(task);
					test = task;
					System.Diagnostics.Debug.WriteLine("hei:" + test);

				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}

		}
    }
}