using System.Threading.Tasks;
using ProcessDashboard.APIRoot;
using Refit;

namespace ProcessDashboard.Service.Interface
{
    /*
        This has the list of all endpoints
    */
    [Headers("Accept: application/json")]
    public interface IPDashApi
    {

        // Get list of projects
        // https://pdes.tuma-solutions.com/api/v1/datasets/mock/projects/
        [Get("/datasets/{dataset}/projects/")]
        Task<ListOfProjectsRoot> GetProjectsList(string dataset);

        // Get list of tasks
        // https://pdes.tuma-solutions.com/api/v1/datasets/mock/projects/iokdum2d/tasks/
        [Get("/datasets/{dataset}/projects/{projectID}/tasks/")]
        Task<TaskListRoot> GetTasksList(string dataset, string projectID);

        // Get details about a task
        // https://pdes.tuma-solutions.com/api/v1/datasets/mock/tasks/iokdum2d:11401830/
        [Get("/datasets/{dataset}/tasks/{projecttaskID}/")]
        Task<TaskRoot> GetTaskDetails(string dataset, string projecttaskID);

        // Recent tasks
        // https://pdes.tuma-solutions.com/api/v1/mock/datasets/recent-tasks/
        [Get("/datasets/{dataset}/recent-tasks/")]
        Task<RecentTasksRoot> GetRecentTasks(string dataset);

        //Get list of timelogs
        // https://pdes.tuma-solutions.com/api/v1/datasets/mock/time-log/
        [Get("/datasets/{dataset}/time-log/")]
        Task<TimeLogsRoot> GetTimeLogs(string dataset);
        
    }
}
