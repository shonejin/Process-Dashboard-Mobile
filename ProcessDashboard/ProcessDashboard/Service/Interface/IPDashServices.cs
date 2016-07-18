using System.Collections.Generic;
using System.Threading.Tasks;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DTO;

namespace ProcessDashboard.Service.Interface
{
    /*
     * Name: IPDashServices.cs
     * 
     * Purpose: This interface is responsible for establishing the methods that will be returning the values either from the server or from the database.
     * 
     * Description: 
     *
     * It has all the methods that will be getting the data from either a local environment or from the database.
     * Database tasks are named as 
     *      Get<item>Local
     * Remote tasks that retrieve data from the server are named as
     *      Get<item>Remote
     *      
     * Parameters:
     * 
     * The parameters which are mostly common for the different functions are explained below:
     *    priority - This is the priority with which the given task should run.
     *    dataset - The dataset value
     *    projectId - The Id of the project for which the data is required.
     *    projecttaskId - The Id of the task within the project
     *    
     * ** Optional Parameters **
     *  TimeLogs accepts optional parameters. They are
     *      maxResults
     *      startDateFrom
     *      startDateTo
     *      taskId
     *      projectId    
    */
    public interface IPDashServices
    {
        // These tasks gets the values from the Database
        Task<List<Project>> GetProjectsListLocal(Priority priority, string dataset);
        Task<List<DTO.Task>> GetTasksListLocal(Priority priority, string dataset, string projectId);
        Task<DTO.Task> GetTaskDetailsLocal(Priority priority, string dataset, string projecttaskId);
        Task<List<DTO.Task>> GetRecentTasksLocal(Priority priority, string dataset);
        Task<List<TimeLogEntry>> GetTimeLogsLocal(Priority priority, string dataset);

        // These tasks gets the value from the server
        Task<List<Project>> GetProjectsListRemote(Priority priority, string dataset);
        Task<List<DTO.Task>> GetTasksListRemote(Priority priority, string dataset, string projectId);
        Task<DTO.Task> GetTaskDetailsRemote(Priority priority, string dataset, string projecttaskId);
        Task<List<DTO.Task>> GetRecentTasksRemote(Priority priority, string dataset);
        Task<List<TimeLogEntry>> GetTimeLogsRemote(Priority priority, string dataset, int maxResults, string startDateFrom, string startDateTo, string taskId, string projectId);

        Task<TimeLogsRoot> AddTimeLog(Priority priority, string dataset, string comment, string startDate, string taskId, string loggedTime);

        Task<TimeLogsRoot> UpdateTimeLog(Priority priority, string dataset, string timeLogId, string comment, string startDate, string taskId, string loggedTime);
        Task<DeleteRoot> DeleteTimeLog(Priority priority, string dataset, string timelogId);
    }
}
