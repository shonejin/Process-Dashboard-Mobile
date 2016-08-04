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
		public delegate void ControllerExceptionHandler(string message);

		public void RefreshDataset()
		{
			dataset = AccountStorage.DataSet;
		}

		public RobustController(IPDashServices pDash) : base(pDash) {}

		// Get list of projects

		public async Task<List<Project>> GetProjects()
		{
			return await base.GetProjects(dataset);
		}

		// Get list of tasks
		public async Task<List<DTO.Task>> GetTasks(string projectId)
		{
			return await base.GetTasks(dataset, projectId);
		}

		// Get Time Log entries with optional parameters.
		// Optional parameters should be specified as null
		// only dataset is required
		public async Task<List<TimeLogEntry>> GetTimeLogs(int? maxResults, DateTime? startDateFrom, DateTime? startDateTo, string taskId, string projectId)
		{
			return await base.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId);
		}

		// Get the list of recent tasks
		public async Task<List<DTO.Task>> GetRecentTasks()
		{
			return await base.GetRecentTasks(dataset);
		}

		// Get details about a single task
		public async Task<DTO.Task> GetTask(string taskId)
		{
			return await base.GetTask(dataset, taskId);
		}

		// Add a new time log entry
		public async Task<TimeLogEntry> AddATimeLog(string comment, DateTime startDate, string taskId, double loggedTime, double interruptTime, bool open)
		{
			return await base.AddATimeLog(dataset, comment, startDate, taskId, loggedTime, interruptTime, open);
		}

		//Update an existing timelog entry
		public async Task<TimeLogEntry> UpdateTimeLog(string timeLogId, string comment, DateTime? startDate, string taskId, double? loggedTime, double? interruptTime, bool open)
		{
			return await base.UpdateTimeLog(dataset, timeLogId, comment, startDate, taskId, loggedTime, interruptTime, open);
		}

		//Delete a  timelog entry
		public async Task<Service.Interface.DeleteRoot> DeleteTimeLog(string timeLogId)
		{
			return await base.DeleteTimeLog(dataset, timeLogId);
		}

		//Read a timelog entry
		public async Task<TimeLogEntry> GetTimeLog(string timeLogId)
		{
			return await base.GetTimeLog(dataset, timeLogId);
		}

		// Update a particular task
		public async Task<DTO.Task> UpdateATask(string taskId, double? estimatedTime, DateTime? completionDate, bool markTaskIncomplete)
		{
			return await base.UpdateATask(dataset, taskId, estimatedTime, completionDate, markTaskIncomplete);
		}
	}
}

