using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.DTO;
using ProcessDashboard.Model;
using ProcessDashboard.Service.Interface;
using Task = ProcessDashboard.DTO.Task;

//using Plugin.Connectivity;
//using Polly;
namespace ProcessDashboard.Service_Access_Layer
{
    /*
     * 
     * Name: PDashServices.cs
     * 
     * Purpose: This class is a concerete implementation for IPDashServices interface.
     * 
     * Description:
     * This class provides concrete implemntation for getting values either from remote service or from local database.
     * The remote service will inturn make use of a concrete implementation of IApiTypes interface.
     * The local service use of the Database Manager for connecting to SQlite Database.

     */
    
    public class PDashServices : IPDashServices
    {
        // Api Service for making the request using Fusilade
        private readonly IApiTypes _apiService;
        // DB Manager to manage Database operations
        private readonly DBManager _dbm;

        public PDashServices(IApiTypes apiService)
        {
            _apiService = apiService;
            _dbm = DBManager.getInstance();
        }

        /*
         * List of projects 
         * 
         */ 

        public async Task<List<Project>> GetProjectsListLocal(Priority priority, string dataset)
        {
            Debug.WriteLine("ProjectModel Service : " + " Going to get data from DB");

            List<Project> output = null;

            List<ProjectModel> values = _dbm.pw.GetAllRecords();

            if (values == null || values.Count == 0)
            {
                Debug.WriteLine("ProjectModel Service : " + "Nothing in the DB");
                return null;
            }
            // Map from project model to project and return values.
            output = Mapper.GetInstance().toProjectList(values);

            Debug.WriteLine("ProjectModel Service : " + " Done with that");

            return output;
        }

        public async Task<List<Project>> GetProjectsListRemote(Priority priority, string dataset)
        {
            Debug.WriteLine("ProjectModel Service : " + " Going for remote task");

            Task<ProjectsListRoot> getTaskDtoTask;

            Debug.WriteLine("ProjectModel Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    getTaskDtoTask = _apiService.Background.GetProjectsList(dataset);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetProjectsList(dataset);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetProjectsList(dataset);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetProjectsList(dataset);
                    break;
            }

            ProjectsListRoot projects = await getTaskDtoTask;

            //var gitHubApi = RestService.For<IPDashApi>("https://pdes.tuma-solutions.com/api/v1/");
            //ProjectsListRoot projects = await gitHubApi.GetProjectsList("mock");

            Debug.WriteLine("ProjectModel Service : " + "Got the content I guess");
            Debug.WriteLine("ProjectModel Service : " + projects.stat);
            Debug.WriteLine("ProjectModel Service : " + (projects.projects.Count));

            if (!projects.stat.Equals("ok") || projects.projects==null)
            {
                Debug.WriteLine("ProjectModel Service : " + "Null here");
                return null;
            }

            // Convert to model and store in DB
            //List<ProjectModel> output = Mapper.GetInstance().toProjectModelList(projects.projects);
            //_dbm.pw.insertMultipleRecords(output);

            //test(projects);

            /*
            if (CrossConnectivity.Current.IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("TaskModel Service : " + " Setting connection policy");
                task = await Policy
                    .Handle<Exception>()
                    .RetryAsync(retryCount: 5)
                    .ExecuteAsync(async () => await getTaskDtoTask);
            }
            */
            return projects.projects;
        }

        /*
         * Task APIs
         */

        public async Task<List<Task>> GetTasksListLocal(Priority priority, string dataset, string projectId)
        {
            Debug.WriteLine("TaskModel Service : " + " Going to get data from DB");
            List<Task> output = null;

            List<TaskModel> values = _dbm.tw.GetAllRecords();

            if (values == null || values.Count == 0)
            {
                return null;
            }
            // Map from project model to project and return values.
            output = Mapper.GetInstance().toTaskList(values);

            Debug.WriteLine("TaskModel Service : " + " Done with that");

            return output;
        }

        public async Task<List<Task>> GetTasksListRemote(Priority priority, string dataset, string projectId)
        {
            Debug.WriteLine("Task Service : " + " Going for remote task");

            TaskListRoot tasks = null;
            Task<TaskListRoot> getTaskDtoTask;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    getTaskDtoTask = _apiService.Background.GetTasksList(dataset, projectId);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetTasksList(dataset, projectId);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetTasksList(dataset, projectId);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetTasksList(dataset, projectId);
                    break;
            }

            tasks = await getTaskDtoTask;
            Debug.WriteLine("Task Service : " + "Got the content. STATUS :"+tasks.stat);
            Debug.WriteLine("Task Service : " + "Is null : " + (tasks.projectTasks==null));

            Project p = tasks.forProject;
            Debug.Assert(tasks.projectTasks != null, "tasks.projectTasks != null");
            for (int i = 0; i < tasks.projectTasks.Count; i++)
            {
                Task t = tasks.projectTasks[i];
                t.project = p;
                tasks.projectTasks[i] = t;
            }

            /*
            if (CrossConnectivity.Current.IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("TaskModel Service : " + " Setting connection policy");
                task = await Policy
                    .Handle<Exception>()
                    .RetryAsync(retryCount: 5)
                    .ExecuteAsync(async () => await getTaskDtoTask);
            }
            */
            return tasks.projectTasks;
        }
        
        /**
         * Task Details API
         */
        public async Task<Task> GetTaskDetailsLocal(Priority priority, string dataset, string projecttaskId)
        {
            Debug.WriteLine("TaskModel Service : " + " Going to get data from DB");
            Task output = null;

            TaskModel values = _dbm.tw.getRecord(projecttaskId);

            if (values == null)
            {
                Debug.WriteLine("TaskModel Service : " + " Nothing in DB");
                // DB does not have any values. Get the values from the server.
                output = await GetTaskDetailsRemote(priority, dataset, projecttaskId);

                // Map from task to task model
                values = Mapper.GetInstance().toTaskModel(output);
                // Store in DB
                _dbm.tw.insertRecord(values);

            }
            else
            {
                // Map from project model to project and return values.
                output = Mapper.GetInstance().toTask(values);
            }

            Debug.WriteLine("TaskModel Service : " + " Done with that");

            return output;
        }

        public async Task<Task> GetTaskDetailsRemote(Priority priority, string dataset, string projecttaskId)
        {
            Debug.WriteLine("Task Service : " + " Going for remote task");

            TaskRoot task = null;
            Task<TaskRoot> getTaskDtoTask;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    getTaskDtoTask = _apiService.Background.GetTaskDetails(dataset, projecttaskId);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetTaskDetails(dataset, projecttaskId);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetTaskDetails(dataset, projecttaskId);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetTaskDetails(dataset, projecttaskId);
                    break;
            }

            task = await getTaskDtoTask;
            Debug.WriteLine("Task Service : " + "Got the content I guess");

            // Convert to model and store in DB

            TaskModel output = Mapper.GetInstance().toTaskModel(task.task);
            _dbm.tw.insertRecord(output);

            /*
            if (CrossConnectivity.Current.IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("TaskModel Service : " + " Setting connection policy");
                task = await Policy
                    .Handle<Exception>()
                    .RetryAsync(retryCount: 5)
                    .ExecuteAsync(async () => await getTaskDtoTask);
            }
            */
            return task.task;
        }

        /*
         *Recent Tasks 
         */
        public async Task<List<Task>> GetRecentTasksLocal(Priority priority, string dataset)
        {
            Debug.WriteLine("TaskModel Service : " + " Going to get data from DB");
            List<Task> output = null;

            List<TaskModel> values = _dbm.tw.GetAllRecords();

            values.Sort((val1, val2) => val1.RecentOrdinal.CompareTo(val2.RecentOrdinal));

            if (values == null || values.Count == 0)
            {
                Debug.WriteLine("TaskModel Service : " + " Nothing in DB");
                // DB does not have any values. Get the values from the server.
                output = await GetRecentTasksRemote(priority, dataset);

                // Map from project to project model
                Mapper.GetInstance().toTaskModelList(output);
                
                // Update Recent ordinal
                
                //dbm.tw.insertMultipleRecords(values);

            }
            else
            {
                // Map from project model to project and return values.
                output = Mapper.GetInstance().toTaskList(values);
            }

            Debug.WriteLine("TaskModel Service : " + " Done with that");

            return output;



        }

        public async Task<List<Task>> GetRecentTasksRemote(Priority priority, string dataset)
        {
            Debug.WriteLine("Task Service : " + " Going for remote task");

            RecentTasksRoot task = null;
            Task<RecentTasksRoot> getTaskDtoTask;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    getTaskDtoTask = _apiService.Background.GetRecentTasks(dataset);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetRecentTasks(dataset);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetRecentTasks(dataset);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetRecentTasks(dataset);
                    break;
            }

            task = await getTaskDtoTask;
            Debug.WriteLine("Task Service : " + "Got the content I guess");
            
            // Convert to model and store in DB

            var output = Mapper.GetInstance().toTaskModelList(task.recentTasks);

            // TODO: UPdate Recent Ordinal
            /*
            if (CrossConnectivity.Current.IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("TaskModel Service : " + " Setting connection policy");
                task = await Policy
                    .Handle<Exception>()
                    .RetryAsync(retryCount: 5)
                    .ExecuteAsync(async () => await getTaskDtoTask);
            }
            */
            return task.recentTasks;
        }

        /* 
         * List of Time Logs/ Global Time Log
         */
        public async Task<List<TimeLogEntry>> GetTimeLogsLocal(Priority priority, string dataset)
        {
            Debug.WriteLine("Time log Service : " + " Going to get data from DB");
            List<TimeLogEntry> output = null;

            var values = _dbm.tlw.GetAllRecords();

            if (values == null || values.Count == 0)
            {
                Debug.WriteLine("TaskModel Service : " + " Nothing in DB");
                // DB does not have any values. Get the values from the server.
            //    output = await GetTimeLogsRemote(priority, dataset);

                // Map from project to project model
                values = Mapper.GetInstance().toTimeLogEntryModelList(output);
                // Store in DB
                _dbm.tlw.insertMultipleRecords(values);

            }
            else
            {
                // Map from project model to project and return values.
                output = Mapper.GetInstance().toTimeLogEntryList(values);
            }

            Debug.WriteLine("TaskModel Service : " + " Done with that");

            return output;


        }

        public async Task<List<TimeLogEntry>> GetTimeLogsRemote(Priority priority, string dataset, int maxResults, string startDateFrom, string startDateTo, string taskId, string projectId)
        {
            Debug.WriteLine("Task Service : " + " Going for remote task");
            Task<TimeLogsRoot> getTaskDtoTask;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    getTaskDtoTask = _apiService.Background.GetTimeLogs(dataset,maxResults,  startDateFrom,  startDateTo,  taskId,  projectId);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId);
                    break;
            }

            var task = await getTaskDtoTask;
            Debug.WriteLine("Task Service : " + "Got the content I guess");

            // Convert to model and store in DB

            //List<TimeLogEntryModel> output = Mapper.GetInstance().toTimeLogEntryModelList(task.timeLogEntries);
            //_dbm.tlw.insertMultipleRecords(output);

            /*
            if (CrossConnectivity.Current.IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("TaskModel Service : " + " Setting connection policy");
                task = await Policy
                    .Handle<Exception>()
                    .RetryAsync(retryCount: 5)
                    .ExecuteAsync(async () => await getTaskDtoTask);
            }
            */
            return task.timeLogEntries;
        }


    }




}