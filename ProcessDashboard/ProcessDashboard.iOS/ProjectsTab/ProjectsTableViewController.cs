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
    public partial class ProjectsTableViewController : UITableViewController
    {
		List<Project> projectsCache;

        public ProjectsTableViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			refreshData();
		}

		public override void PrepareForSegue(UIKit.UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			base.PrepareForSegue(segue, sender);
			if (segue.Identifier.Equals("project2Tasks"))
			{
				TasksTableViewController controller = (TasksTableViewController)segue.DestinationViewController;
				controller.projectId = ((ProjectsTableSource)projectsTableView.Source).selectedProjectId;
			
			}
		}

		public async void refreshData()
		{
			await getDataOfProject();
			//Console.WriteLine("HAHAH Length is " + projectsCache.Count);
			projectsTableView.Source = new ProjectsTableSource(projectsCache, this);
			projectsTableView.ReloadData();
		}

		public async System.Threading.Tasks.Task<int> getDataOfProject()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);

			// TODO: make controller a global singelton for better performance
			Controller c = new Controller(service);

			// TODO: should this line be wrapped in try-catch?
			List<Project> projectsList = await c.GetProjects("mock");

			// TODO: add exception handling logic
			projectsCache = projectsList;

			Console.WriteLine("HAHAH Length is " + projectsCache.Count);

			try
			{
				System.Diagnostics.Debug.WriteLine("** GET PROJECTS **");
				System.Diagnostics.Debug.WriteLine("Length is " + projectsCache.Count);

				foreach (var proj in projectsList.Select(x => x.name))
				{
					System.Diagnostics.Debug.WriteLine(proj);
				}
				projectsTableView.ReloadData();
				System.Diagnostics.Debug.WriteLine("Projects Tab: data reloaded");
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;
		}
    }
}