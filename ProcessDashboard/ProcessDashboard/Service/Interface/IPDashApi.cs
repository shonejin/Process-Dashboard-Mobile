using System.Threading.Tasks;
using ProcessDashboard.APIRoot;
using Refit;

namespace ProcessDashboard.Service.Interface
{
    /*
     * Name: IPDashApi.cs
     * 
     * Purpose: This interface is responsible for establishing the end points to communicate with the server.
     * 
     * Description: It makes use of Refit library for parsing JSON into OO Objects.
     * 
     * Structure of a Refit request:
     *
     *      [GET/POST/PUT/DELETE <realtive url to end point>]
     *      Task<T> function(parameters)
     * Parameters:
     * 
     *      T - Type of the object to be used
     *      parameters - Combination of optional and mandatory parameters.
     *          Madatory parameters - Any replacable block in the url are mandatory
     *          Optional parameters - Optional parameters to the GET/PUT/POST/DELETE request are sent as arguments as well.
    */
    [Headers("Accept: application/json")]
    public interface IPDashApi
    {

        // Get list of projects
        // Sample URL : https://pdes.tuma-solutions.com/api/v1/datasets/mock/projects/
        [Get("/datasets/{dataset}/projects/")]
        Task<ProjectsListRoot> GetProjectsList(string dataset);

        // Get list of tasks
        // Sample URL : https://pdes.tuma-solutions.com/api/v1/datasets/mock/projects/iokdum2d/tasks/
        [Get("/datasets/{dataset}/projects/{projectId}/tasks/")]
        Task<TaskListRoot> GetTasksList(string dataset, string projectId);

        // Get details about a task
        // Sample URL : https://pdes.tuma-solutions.com/api/v1/datasets/mock/tasks/iokdum2d:11401830/
        [Get("/datasets/{dataset}/tasks/{projecttaskId}/")]
        Task<TaskRoot> GetTaskDetails(string dataset, string projecttaskId);

        // Recent tasks
        // Sample URL : https://pdes.tuma-solutions.com/api/v1/mock/datasets/recent-tasks/
        [Get("/datasets/{dataset}/recent-tasks/")]
        Task<RecentTasksRoot> GetRecentTasks(string dataset);

        //Get list of timelogs
        // Sample URL : https://pdes.tuma-solutions.com/api/v1/datasets/mock/time-log/
        // Optional Parameters: maxResults, startDateFrom, startDateTo, taskId, projectId
        [Get("/datasets/{dataset}/time-log/")]
        Task<TimeLogsRoot> GetTimeLogs(string dataset,int maxResults, string startDateFrom, string startDateTo, string taskId, string projectId);
        
    }
}
