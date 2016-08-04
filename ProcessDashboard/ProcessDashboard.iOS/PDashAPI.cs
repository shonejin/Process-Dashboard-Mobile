using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessDashboard.DTO;
using ProcessDashboard.Service;
using ProcessDashboard.Service.Interface;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using UIKit;

namespace ProcessDashboard.iOS
{
	// use this wrapper to get different API types, instead of initializing them again and again
	public static class PDashAPI
	{
		public static readonly RobustController Controller;

		static PDashAPI()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller = new RobustController(service);
		}
	}

	// wrapper of Controller
	// hides "dataset"
	// handles exceptions
	public class RobustController : Controller
	{
		// dataset value is refreshed after each login
		private string dataset = AccountStorage.DataSet;

		public void RefreshDataset()
		{
			dataset = AccountStorage.DataSet;
		}

		public RobustController(IPDashServices pDash) : base(pDash) {}

		private UIViewController getTopMostViewController()
		{
			UIViewController topVC = UIApplication.SharedApplication.KeyWindow.RootViewController;
			while (topVC.PresentedViewController != null)
			{
				topVC = topVC.PresentedViewController;
			}
			return topVC;
		}

		private void showAlert(string msg)
		{
			UIViewController vc = getTopMostViewController();

			UIAlertController okAlertController = UIAlertController.Create("Connection Unavailable", msg, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			vc.PresentViewController(okAlertController, true, null);
		}

		// Get list of projects
		public async Task<List<Project>> GetProjects()
		{
			try
			{
				return await base.GetProjects(dataset);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		// Get list of tasks
		public async Task<List<DTO.Task>> GetTasks(string projectId)
		{
			try
			{
				return await base.GetTasks(dataset, projectId);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		// Get Time Log entries with optional parameters.
		// Optional parameters should be specified as null
		// only dataset is required
		public async Task<List<TimeLogEntry>> GetTimeLogs(int? maxResults, DateTime? startDateFrom, DateTime? startDateTo, string taskId, string projectId)
		{
			try
			{
				return await base.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		// Get the list of recent tasks
		public async Task<List<DTO.Task>> GetRecentTasks()
		{
			try
			{
				return await base.GetRecentTasks(dataset);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		// Get details about a single task
		public async Task<DTO.Task> GetTask(string taskId)
		{
			try
			{
				return await base.GetTask(dataset, taskId);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		// Add a new time log entry
		public async Task<TimeLogEntry> AddATimeLog(string comment, DateTime startDate, string taskId, double loggedTime, double interruptTime, bool open)
		{
			try
			{
				return await base.AddATimeLog(dataset, comment, startDate, taskId, loggedTime, interruptTime, open);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		//Update an existing timelog entry
		public async Task<TimeLogEntry> UpdateTimeLog(string timeLogId, string comment, DateTime? startDate, string taskId, double? loggedTime, double? interruptTime, bool open)
		{
			try
			{
				return await base.UpdateTimeLog(dataset, timeLogId, comment, startDate, taskId, loggedTime, interruptTime, open);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		//Delete a  timelog entry
		public async Task<Service.Interface.DeleteRoot> DeleteTimeLog(string timeLogId)
		{
			try
			{
				return await base.DeleteTimeLog(dataset, timeLogId);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		//Read a timelog entry
		public async Task<TimeLogEntry> GetTimeLog(string timeLogId)
		{
			try
			{
				return await base.GetTimeLog(dataset, timeLogId);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}

		// Update a particular task
		public async Task<DTO.Task> UpdateATask(string taskId, double? estimatedTime, DateTime? completionDate, bool markTaskIncomplete)
		{
			try
			{
				return await base.UpdateATask(dataset, taskId, estimatedTime, completionDate, markTaskIncomplete);
			}
			catch (Exception ex)
			{
				showAlert(ex.Message + " Please try again later.");
				return null;
			}
		}
	}
}

