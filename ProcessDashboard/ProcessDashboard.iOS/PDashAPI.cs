using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessDashboard.DTO;
using ProcessDashboard.Service;
using ProcessDashboard.Service.Interface;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using UIKit;
using CoreFoundation;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;

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
		private bool wifiAvailable = false;
		public delegate void ControllerExceptionHandler(string message);

		public RobustController()
		{
			refreshWifiAvailability();
		}

		public void refreshWifiAvailability()
		{
			var connections = CrossConnectivity.Current.ConnectionTypes;
			wifiAvailable = false;
			foreach (ConnectionType type in connections)
			{
				if (type.Equals(ConnectionType.WiFi))
				{
					wifiAvailable = true;
					break;
				}
			}
		}

		private bool shouldProceed()
		{
			refreshWifiAvailability();
			return !SettingsData.WiFiOnly || wifiAvailable;
		}

		public void RefreshDataset()
		{
			dataset = AccountStorage.DataSet;
		}

		public RobustController(IPDashServices pDash) : base(pDash) {}

		// Get list of projects

		public async Task<List<Project>> GetProjects()
		{
			if (shouldProceed())
			{
				return await base.GetProjects(dataset);
			}
			else 
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		// Get list of tasks
		public async Task<List<DTO.Task>> GetTasks(string projectId)
		{
			if (shouldProceed())
			{
				return await base.GetTasks(dataset, projectId);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		// Get Time Log entries with optional parameters.
		// Optional parameters should be specified as null
		// only dataset is required
		public async Task<List<TimeLogEntry>> GetTimeLogs(int? maxResults, DateTime? startDateFrom, DateTime? startDateTo, string taskId, string projectId)
		{
			if (shouldProceed())
			{
				return await base.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		// Get the list of recent tasks
		public async Task<List<DTO.Task>> GetRecentTasks()
		{
			if (shouldProceed())
			{
				return await base.GetRecentTasks(dataset);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		// Get details about a single task
		public async Task<DTO.Task> GetTask(string taskId)
		{
			if (shouldProceed())
			{
				return await base.GetTask(dataset, taskId);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		// Add a new time log entry
		public async Task<TimeLogEntry> AddATimeLog(string comment, DateTime startDate, string taskId, double loggedTime, double interruptTime, bool open)
		{
			if (shouldProceed())
			{
				return await base.AddATimeLog(dataset, comment, startDate, taskId, loggedTime, interruptTime, open);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		//Update an existing timelog entry
		public async Task<TimeLogEntry> UpdateTimeLog(string timeLogId, string comment, DateTime? startDate, string taskId, double? loggedTime, double? interruptTime, bool open)
		{
			if (shouldProceed())
			{
				return await base.UpdateTimeLog(dataset, timeLogId, comment, startDate, taskId, loggedTime, interruptTime, open);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		//Delete a  timelog entry
		public async Task<Service.Interface.DeleteRoot> DeleteTimeLog(string timeLogId)
		{
			if (shouldProceed())
			{
				return await base.DeleteTimeLog(dataset, timeLogId);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		//Read a timelog entry
		public async Task<TimeLogEntry> GetTimeLog(string timeLogId)
		{
			if (shouldProceed())
			{
				return await base.GetTimeLog(dataset, timeLogId);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}

		// Update a particular task
		public async Task<DTO.Task> UpdateATask(string taskId, double? estimatedTime, DateTime? completionDate, bool markTaskIncomplete)
		{
			if (shouldProceed())
			{
				return await base.UpdateATask(dataset, taskId, estimatedTime, completionDate, markTaskIncomplete);
			}
			else
			{
				throw new Exception("App running in WiFi-Only mode but WiFi is not available.");
			}
		}
	}
}

