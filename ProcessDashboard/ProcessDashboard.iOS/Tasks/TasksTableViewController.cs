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
using System.Drawing;
using CoreGraphics;

namespace ProcessDashboard.iOS
{
    public partial class TasksTableViewController : UITableViewController
    {
		public string projectId;
		public string projectName;
		List<Task> tasksCache;
		UILabel StaticLabel;

		public TasksTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			StaticLabel = new UILabel(new CGRect(0, 0, View.Bounds.Width, 40))
			{
				Text = " ",
				Font = UIFont.SystemFontOfSize(12),
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.FromRGB(225, 225, 225),
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
			};

			StaticLabel.AutoresizingMask = UIViewAutoresizing.All;

			tasksTableView.TableHeaderView = new UIView(new CGRect(0, 0, View.Bounds.Width, 40));
			tasksTableView.TableHeaderView.AddSubview(StaticLabel);

			this.RefreshControl = new UIRefreshControl();
			this.RefreshControl.ValueChanged += (sender, e) => { refreshData(); };
			refreshData();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			//refreshData();
		}

		//public override void ViewWillDisappear( bool animated)
		//{
		//	base.ViewWillDisappear(animated);
		//	this.NavigationController.PopToRootViewController(true);
		//}

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

			TasksTableSource source = new TasksTableSource(tasksCache, this);
			tasksTableView.Source = source;
			NavigationItem.Title = "Tasks";
			StaticLabel.Text = projectName;

			int pos = 0;
			for (int i = 0; i < tasksCache.Count; i++)
			{
				if (tasksCache[i].CompletionDate.ToShortDateString().Equals("1/1/0001"))
				{
					pos = i;
					break;
				}
					
			}
			String refreshTime = DateTime.Now.ToString("g");
			String subTitle = "Last refresh: " + refreshTime;
			this.RefreshControl.AttributedTitle = new Foundation.NSAttributedString(subTitle);

			tasksTableView.ReloadData();
			// The scroll bar should be scrolled so that the first incomplete task is the first task in the screen.
			if (tasksCache.Count != 0)
			{
				tasksTableView.ScrollToRow(NSIndexPath.FromRowSection(pos, 0), UITableViewScrollPosition.Top, true);
			}
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

				foreach (var task in tasksList)  //.Select(x => x.estimatedTime)
				{
					System.Diagnostics.Debug.WriteLine(task.FullName);
		
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