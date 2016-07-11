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
		string[] tableItems;

        public ProjectsTableViewController (IntPtr handle) : base (handle)
        {
        }
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			getDataOfProject();

			tableItems = new string[] { "Mobile Process Dashboard", "Enterprise Server", "Linux Kernel", "Windows X1 Professional", "Siri for macOS" };
			projectsTableView.Source = new ProjectsTableSource(tableItems, this);
			projectsTableView.ReloadData();

		}

		public async void getDataOfProject()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);
			List<Project> projectsList = await c.GetProjects("mock");

			try
			{
				System.Diagnostics.Debug.WriteLine("** GET PROJECTS **");
				System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

				tableItems = new string[projectsList.Count];
				int i = 0;
				foreach (var proj in projectsList.Select(x => x.name))
				{
					System.Diagnostics.Debug.WriteLine(proj);
					tableItems[i++] = proj;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}


		}
	
    }
}