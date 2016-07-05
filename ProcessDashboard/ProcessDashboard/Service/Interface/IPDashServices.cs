using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DTO;

namespace ProcessDashboard.Service.Interface
{
    interface IPDashServices
    {
        // These tasks gets the values from the Database
        Task<List<Project>> GetProjectsListLocal(Priority priority, string dataset);
        Task<List<DTO.Task>> GetTasksListLocal(Priority priority, string dataset, string projectID);
        Task<DTO.Task> GetTaskDetailsLocal(Priority priority, string dataset, string projecttaskID);
        Task<List<DTO.Task>> GetRecentTasksLocal(Priority priority, string dataset);
        Task<List<TimeLogEntry>> GetTimeLogsLocal(Priority priority, string dataset);

        // These tasks gets the value from the server
        Task<List<Project>> GetProjectsListRemote(Priority priority, string dataset);
        Task<List<DTO.Task>> GetTasksListRemote(Priority priority, string dataset, string projectID);
        Task<DTO.Task> GetTaskDetailsRemote(Priority priority, string dataset, string projecttaskID);
        Task<List<DTO.Task>> GetRecentTasksRemote(Priority priority, string dataset);
        Task<List<TimeLogEntry>> GetTimeLogsRemote(Priority priority, string dataset);

    }
}
