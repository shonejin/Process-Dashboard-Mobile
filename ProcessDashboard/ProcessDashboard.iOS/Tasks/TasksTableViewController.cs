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
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.DTO;

namespace ProcessDashboard.iOS
{
    public partial class TasksTableViewController : UITableViewController
    {
		public string projectId;
		public string projectName;
		List<Task> tasksCache;

		public TasksTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.RefreshControl = new UIRefreshControl();
			this.RefreshControl.ValueChanged += (sender, e) => { refreshData(); };
			refreshData();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			//refreshData();
		}

		public override void PrepareForSegue(UIKit.UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			base.PrepareForSegue(segue, sender);
			if (segue.Identifier.Equals("task2TaskDetail"))
			{
				TaskDetailsViewController controller = (TaskDetailsViewController)segue.DestinationViewController;
				controller.task = ((TasksTableSource)tasksTableView.Source).selectedTask;
				//controller.project = ((TasksTableSource)tasksTableView.Source).project;
			}
		}

		public async void refreshData()
		{
			if (!this.RefreshControl.Refreshing)
			{
				this.RefreshControl.BeginRefreshing();
			}

			await getDataOfTask();

			tasksTableView.Source = new TasksTableSource(tasksCache, this);

			String refreshTime = DateTime.Now.ToString("g");
			String subTitle = "Last refresh: " + refreshTime;
			this.RefreshControl.AttributedTitle = new Foundation.NSAttributedString(subTitle);

			tasksTableView.ReloadData();

			if (this.RefreshControl.Refreshing)
			{
				this.RefreshControl.EndRefreshing();
			}
		}



		public async System.Threading.Tasks.Task<int> getDataOfTask()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);
			List<Task> tasksList = await c.GetTasks("mock", projectId);
			tasksCache = tasksList;


			try
			{
				System.Diagnostics.Debug.WriteLine("** GET TASKS **");
				System.Diagnostics.Debug.WriteLine("Length is " + tasksList.Count);

				foreach (var task in tasksList.Select(x => x.fullName))
				{
					System.Diagnostics.Debug.WriteLine(task);
				}

			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}

			return 0;
		}
    }
}