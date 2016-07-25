using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.DTO;
using ProcessDashboard.Model;
using ProcessDashboard.Service.Interface;
using Refit;
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
        private readonly DbManager _dbm;
        private Settings _settings;

        private string _pattern;

        public PDashServices(IApiTypes apiService)
        {
            _apiService = apiService;
            this._settings = Settings.GetInstance();
            _pattern = _settings.DateTimePattern;
            _dbm = DbManager.GetInstance();
        }

        /*
         * List of projects 
         * 
         */ 

        public async Task<List<Project>> GetProjectsListLocal(Priority priority, string dataset)
        {
            Debug.WriteLine("ProjectModel Service : " + " Going to get data from DB");

            List<Project> output = null;

            List<ProjectModel> values = _dbm.Pw.GetAllRecords();

            if (values == null || values.Count == 0)
            {
                Debug.WriteLine("ProjectModel Service : " + "Nothing in the DB");
                return null;
            }
            // Map from project model to project and return values.
            output = Mapper.GetInstance().ToProjectList(values);

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
                    getTaskDtoTask = _apiService.Background.GetProjectsList(dataset,_settings.AuthHeader);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetProjectsList(dataset, _settings.AuthHeader);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetProjectsList(dataset, _settings.AuthHeader);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetProjectsList(dataset, _settings.AuthHeader);
                    break;
            }

            ProjectsListRoot projects = await getTaskDtoTask;

            //var gitHubApi = RestService.For<IPDashApi>("https://pdes.tuma-solutions.com/api/v1/");
            //ProjectsListRoot projects = await gitHubApi.GetProjectsList("mock");

            Debug.WriteLine("ProjectModel Service : " + "Got the content I guess");
            Debug.WriteLine("ProjectModel Service : " + projects.Stat);
            Debug.WriteLine("ProjectModel Service : " + (projects.Projects.Count));

            if (!projects.Stat.Equals("ok") || projects.Projects==null)
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
            return projects.Projects;
        }

        /*
         * Task APIs
         */

        public async Task<List<Task>> GetTasksListLocal(Priority priority, string dataset, string projectId)
        {
            Debug.WriteLine("TaskModel Service : " + " Going to get data from DB");
            List<Task> output = null;

            List<TaskModel> values = _dbm.Tw.GetAllRecords();

            if (values == null || values.Count == 0)
            {
                return null;
            }
            // Map from project model to project and return values.
            output = Mapper.GetInstance().ToTaskList(values);

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
                    getTaskDtoTask = _apiService.Background.GetTasksList(dataset, projectId, _settings.AuthHeader);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetTasksList(dataset, projectId, _settings.AuthHeader);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetTasksList(dataset, projectId, _settings.AuthHeader);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetTasksList(dataset, projectId, _settings.AuthHeader);
                    break;
            }

            tasks = await getTaskDtoTask;
            Debug.WriteLine("Task Service : " + "Got the content. STATUS :"+tasks.Stat);
            Debug.WriteLine("Task Service : " + "Is null : " + (tasks.ProjectTasks==null));

            Project p = tasks.ForProject;
            Debug.Assert(tasks.ProjectTasks != null, "tasks.projectTasks != null");
            for (int i = 0; i < tasks.ProjectTasks.Count; i++)
            {
                Task t = tasks.ProjectTasks[i];
                t.Project = p;
                tasks.ProjectTasks[i] = t;
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
            return tasks.ProjectTasks;
        }
        
        /**
         * Task Details API
         */
        public async Task<Task> GetTaskDetailsLocal(Priority priority, string dataset, string projecttaskId)
        {
            Debug.WriteLine("TaskModel Service : " + " Going to get data from DB");
            Task output = null;

            TaskModel values = _dbm.Tw.GetRecord(projecttaskId);

            if (values == null)
            {
                Debug.WriteLine("TaskModel Service : " + " Nothing in DB");
                // DB does not have any values. Get the values from the server.
                output = await GetTaskDetailsRemote(priority, dataset, projecttaskId);

                // Map from task to task model
                values = Mapper.GetInstance().ToTaskModel(output);
                // Store in DB
                _dbm.Tw.InsertRecord(values);

            }
            else
            {
                // Map from project model to project and return values.
                output = Mapper.GetInstance().ToTask(values);
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
                    getTaskDtoTask = _apiService.Background.GetTaskDetails(dataset, projecttaskId, _settings.AuthHeader);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetTaskDetails(dataset, projecttaskId, _settings.AuthHeader);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetTaskDetails(dataset, projecttaskId, _settings.AuthHeader);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetTaskDetails(dataset, projecttaskId, _settings.AuthHeader);
                    break;
            }

            task = await getTaskDtoTask;
            Debug.WriteLine("Task Service : " + "Got the content I guess");

            // Convert to model and store in DB

            TaskModel output = Mapper.GetInstance().ToTaskModel(task.Task);
            _dbm.Tw.InsertRecord(output);

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
            return task.Task;
        }

        /*
         *Recent Tasks 
         */
        public async Task<List<Task>> GetRecentTasksLocal(Priority priority, string dataset)
        {
            Debug.WriteLine("TaskModel Service : " + " Going to get data from DB");
            List<Task> output = null;

            List<TaskModel> values = _dbm.Tw.GetAllRecords();

            values.Sort((val1, val2) => val1.RecentOrdinal.CompareTo(val2.RecentOrdinal));

            if (values == null || values.Count == 0)
            {
                Debug.WriteLine("TaskModel Service : " + " Nothing in DB");
                // DB does not have any values. Get the values from the server.
                output = await GetRecentTasksRemote(priority, dataset);

                // Map from project to project model
                Mapper.GetInstance().ToTaskModelList(output);
                
                // Update Recent ordinal
                
                //dbm.tw.insertMultipleRecords(values);

            }
            else
            {
                // Map from project model to project and return values.
                output = Mapper.GetInstance().ToTaskList(values);
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
                    getTaskDtoTask = _apiService.Background.GetRecentTasks(dataset, _settings.AuthHeader);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetRecentTasks(dataset, _settings.AuthHeader);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetRecentTasks(dataset, _settings.AuthHeader);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetRecentTasks(dataset, _settings.AuthHeader);
                    break;
            }

            task = await getTaskDtoTask;
            Debug.WriteLine("Task Service : " + "Got the content I guess");
            
            // Convert to model and store in DB

            var output = Mapper.GetInstance().ToTaskModelList(task.RecentTasks);

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
            return task.RecentTasks;
        }

        /* 
         * List of Time Logs/ Global Time Log
         */
        public async Task<List<TimeLogEntry>> GetTimeLogsLocal(Priority priority, string dataset)
        {
            Debug.WriteLine("Time log Service : " + " Going to get data from DB");
            List<TimeLogEntry> output = null;

            var values = _dbm.Tlw.GetAllRecords();

            if (values == null || values.Count == 0)
            {
                Debug.WriteLine("TaskModel Service : " + " Nothing in DB");
                // DB does not have any values. Get the values from the server.
            //    output = await GetTimeLogsRemote(priority, dataset);

                // Map from project to project model
                values = Mapper.GetInstance().ToTimeLogEntryModelList(output);
                // Store in DB
                _dbm.Tlw.InsertMultipleRecords(values);

            }
            else
            {
                // Map from project model to project and return values.
                output = Mapper.GetInstance().ToTimeLogEntryList(values);
            }

            Debug.WriteLine("TaskModel Service : " + " Done with that");

            return output;


        }

        public async Task<List<TimeLogEntry>> GetTimeLogsRemote(Priority priority, string dataset, int? maxResults, DateTime? startDateFrom, DateTime? startDateTo, string taskId, string projectId)
        {
            Debug.WriteLine("Task Service : " + " Going for remote task");
            Task<TimeLogsRoot> getTaskDtoTask;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    getTaskDtoTask = _apiService.Background.GetTimeLogs(dataset,maxResults,  startDateFrom,  startDateTo,  taskId,  projectId, _settings.AuthHeader);
                    break;
                case Priority.UserInitiated:
                    getTaskDtoTask = _apiService.UserInitiated.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId, _settings.AuthHeader);
                    break;
                case Priority.Speculative:
                    getTaskDtoTask = _apiService.Speculative.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId, _settings.AuthHeader);
                    break;
                default:
                    getTaskDtoTask = _apiService.UserInitiated.GetTimeLogs(dataset, maxResults, startDateFrom, startDateTo, taskId, projectId, _settings.AuthHeader);
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
            return task.TimeLogEntries;
        }

        /*
         * Adding/Updating/Deleting Time Log entries
         */ 
        
        public async Task<EditATimeLogRoot> AddTimeLog(Priority priority, string dataset, string comment,string startDate, string taskId,double loggedTime)
        {
            

            Dictionary<string, object> value = new Dictionary<string, object>();
            value.Add("comment", comment);
            value.Add("startDate", DateTime.Parse(startDate).ToString(_pattern));
            value.Add("taskId", taskId);
            value.Add("loggedTime", loggedTime);
            value.Add("editTimestamp", DateTime.Now.ToString(_pattern));

            EditATimeLogRoot task = null;
            Task<EditATimeLogRoot> addTimeLog;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    addTimeLog = _apiService.Background.AddTimeLog(_settings.AuthHeader, dataset, value);
                    break;
                case Priority.UserInitiated:
                    addTimeLog = _apiService.UserInitiated.AddTimeLog(_settings.AuthHeader, dataset, value);
                    break;
                case Priority.Speculative:
                    addTimeLog = _apiService.Speculative.AddTimeLog(_settings.AuthHeader, dataset, value);
                    break;
                default:
                    addTimeLog = _apiService.UserInitiated.AddTimeLog(_settings.AuthHeader, dataset, value);
                    break;
            }
            var timelogged = await addTimeLog;
            return timelogged;
        }

        public async Task<EditATimeLogRoot> UpdateTimeLog(Priority priority,string dataset, string timeLogId, string comment, string startDate, string taskId, double loggedTime)
        {           
            

            Dictionary<string, object> value = new Dictionary<string, object>();
            value.Add("comment", comment);
            value.Add("startDate", startDate);
            value.Add("taskId", taskId);
            value.Add("loggedTime", loggedTime);
            value.Add("editTimestamp", DateTime.Now.ToString(_pattern));
            System.Diagnostics.Debug.WriteLine("Data set is :"+dataset+" Time Log is :"+timeLogId+" EditTime stamp:"+value["editTimestamp"]);
            EditATimeLogRoot task = null;
            Task<EditATimeLogRoot> updateTimeLog;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    updateTimeLog = _apiService.Background.UpdateTimeLog(_settings.AuthHeader, dataset, timeLogId, value);
                    break;
                case Priority.UserInitiated:
                    updateTimeLog = _apiService.UserInitiated.UpdateTimeLog(_settings.AuthHeader, dataset, timeLogId, value);
                    break;
                case Priority.Speculative:
                    updateTimeLog = _apiService.Speculative.UpdateTimeLog(_settings.AuthHeader, dataset, timeLogId, value);
                    break;
                default:
                    updateTimeLog = _apiService.UserInitiated.UpdateTimeLog(_settings.AuthHeader, dataset, timeLogId, value);
                    break;
            }


            var timelogged = await updateTimeLog;

            return timelogged;

        }

        public async Task<DeleteRoot> DeleteTimeLog(Priority priority,string dataset, string timelogId)
        {

            
            string value = DateTime.Now.ToString(_pattern);

            DeleteRoot task = null;
            Task<DeleteRoot> deleteTimeLog;
            Debug.WriteLine("Task Service : " + " Setting priority");
            switch (priority)
            {
                case Priority.Background:
                    deleteTimeLog = _apiService.Background.DeleteTimeLog(_settings.AuthHeader, dataset,timelogId, value);
                    break;
                case Priority.UserInitiated:
                    deleteTimeLog = _apiService.UserInitiated.DeleteTimeLog(_settings.AuthHeader, dataset, timelogId, value);
                    break;
                case Priority.Speculative:
                    deleteTimeLog = _apiService.Speculative.DeleteTimeLog(_settings.AuthHeader, dataset, timelogId, value);
                    break;
                default:
                    deleteTimeLog = _apiService.UserInitiated.DeleteTimeLog(_settings.AuthHeader, dataset, timelogId, value);
                    break;
            }


            var timelogged = await deleteTimeLog;

            return timelogged;


        }

     
    }




}