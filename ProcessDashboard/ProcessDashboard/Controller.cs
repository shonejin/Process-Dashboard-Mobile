using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.DTO;
using ProcessDashboard.Service;
namespace ProcessDashboard.SyncLogic
{
    public class Controller
    {
  /*
   * 
   * Name: Controller.cs
   * 
   * Purpose: This class is used by the iOS/Android app to get the data.
   * 
   * Description:
   * This class provides methods which can be used by the iOS/Android app. The methods return the list of tasks/projects/timelog entries as well as individual entries.
   * It also abstracts the difference between data obtained from the server and data obtained from the database.

   */

        private readonly IPDashServices _pDashServices;

        private readonly DbManager _dbm;

        public Controller(IPDashServices pDash)
        {
            this._pDashServices = pDash;
            _dbm = DbManager.GetInstance();

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

        // Get list of projects
        public async Task<List<Project>> GetProjects(string dataset)
        {
            //var localprojects = await _pDashServices.GetProjectsListLocal(Priority.UserInitiated, dataset).ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine("We are in the project list task");

            var remoteProjects = await _pDashServices.GetProjectsListRemote(Priority.UserInitiated, dataset)
                .ConfigureAwait(false);

            return remoteProjects;

            /*
            // TODO: Bind the values returned by remoteProjects or use a mechanism that executes after remoteProjects is obtained.

            List<Project> entries;

            if (localprojects == null)
            {
                // We need to wait till we get the remote data.
                entries = await remoteProjects;
                // Use our sync logic to validate the data received
                //TODO: Check Sync logic

                // Convert the remote data into a DB storable form
                List<ProjectModel> models = Mapper.GetInstance().toProjectModelList(entries);
                // Store the remote data into the DB
                _dbm.pw.insertMultipleRecords(models);

            }
            else
            {
                entries = localprojects;
            }

            // Do any UI Operations
            return entries;
            */
        }

        // Get list of tasks
        public async Task<List<DTO.Task>> GetTasks(string dataset, string projectId)
        {
            //var localTasks = await _pDashServices
            //                              .GetTasksListLocal(Priority.UserInitiated, dataset,projectID)
            //                            .ConfigureAwait(false);

            var remoteTasks = await _pDashServices.GetTasksListRemote(Priority.UserInitiated, dataset, projectId)
                .ConfigureAwait(false);
            return remoteTasks;
            /*
            // TODO: Bind the values returned by remoteProjects or use a mechanism that executes after remoteProjects is obtained.

            List<DTO.Task> entries;

            if (localTasks == null)
            {
                // We need to wait till we get the remote data.
                entries = await remoteTasks;
                // Use our sync logic to validate the data received
                //TODO: Check Sync logic

                 
                System.Diagnostics.Debug.WriteLine("Controller : " + "Going for mapping " + (entries == null));
                // Convert the remote data into a DB storable form
                List<TaskModel> models = Mapper.GetInstance().toTaskModelList(entries);
                System.Diagnostics.Debug.WriteLine("Controller : "+"Done with the mapping "+(models==null));
                // Store the remote data into the DB
                _dbm.tw.insertMultipleRecords(models);
                System.Diagnostics.Debug.WriteLine("Controller : " + "Done with the inserting into DB");

            }
            else
            {
                entries = localTasks;
            }

            // Do any UI Operations
            return entries;
            */
        }

        // Get Time Log entries with optional parameters.
        // Optional parameters should be specified as null
        public async Task<List<TimeLogEntry>> GetTimeLog(string dataset, int? maxResults, DateTime? startDateFrom, DateTime? startDateTo, string taskId, string projectId)
        {

            var remoteTasks = await _pDashServices.GetTimeLogsRemote(Priority.UserInitiated, dataset, maxResults, startDateFrom, startDateTo, taskId, projectId)
                .ConfigureAwait(false);
            return remoteTasks;

        }

        // Get the list of recent tasks
        public async Task<List<DTO.Task>> GetRecentTasks(string dataset)
        {
            //var localTasks = await _pDashServices
            //                              .GetTasksListLocal(Priority.UserInitiated, dataset,projectID)
            //                            .ConfigureAwait(false);

            var remoteTasks = await _pDashServices.GetRecentTasksRemote(Priority.UserInitiated, dataset)
                .ConfigureAwait(false);
            return remoteTasks;
            /*
            // TODO: Bind the values returned by remoteProjects or use a mechanism that executes after remoteProjects is obtained.

            List<DTO.Task> entries;

            if (localTasks == null)
            {
                // We need to wait till we get the remote data.
                entries = await remoteTasks;
                // Use our sync logic to validate the data received
                //TODO: Check Sync logic

                 
                System.Diagnostics.Debug.WriteLine("Controller : " + "Going for mapping " + (entries == null));
                // Convert the remote data into a DB storable form
                List<TaskModel> models = Mapper.GetInstance().toTaskModelList(entries);
                System.Diagnostics.Debug.WriteLine("Controller : "+"Done with the mapping "+(models==null));
                // Store the remote data into the DB
                _dbm.tw.insertMultipleRecords(models);
                System.Diagnostics.Debug.WriteLine("Controller : " + "Done with the inserting into DB");

            }
            else
            {
                entries = localTasks;
            }

            // Do any UI Operations
            return entries;
            */

        }

        public async Task<DTO.Task> GetTask(string dataset, string taskId)
        {
            var remoteTasks = await _pDashServices.GetTaskDetailsRemote(Priority.UserInitiated, dataset, taskId)
             .ConfigureAwait(false);
            return remoteTasks;
        }

        public async Task<EditATimeLogRoot> AddATimeLog(string dataset, string comment, string startDate, string taskId, double loggedTime,double interruptTime, bool open)
        {

            EditATimeLogRoot tro = await _pDashServices.AddTimeLog(Priority.UserInitiated, dataset, comment, startDate, taskId, loggedTime,interruptTime,open);

            System.Diagnostics.Debug.WriteLine("Add Stat :" + tro.Stat);
            if (tro.Stat.Equals("ok"))
            {

                System.Diagnostics.Debug.WriteLine("Id:"+tro.TimeLogEntry.Id);

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Id :" + tro.Err.Code);
                System.Diagnostics.Debug.WriteLine("Id :" + tro.Err.Msg);

                if (tro.Err.Equals("stopTimeLogging"))
                {
                    
                    throw new CancelTimeLoggingException(tro.Err.stopTime);

                }

            }
            return tro;

        }

        public async Task<EditATimeLogRoot> UpdateTimeLog(string dataset, string timeLogId, string comment, string startDate, string taskId, double loggedTime,double interruptTime,bool open)
        {


            EditATimeLogRoot tro = await _pDashServices.UpdateTimeLog(Priority.UserInitiated, dataset,timeLogId, comment, startDate, taskId, loggedTime,interruptTime,open);

            System.Diagnostics.Debug.WriteLine("Update Stat :" + tro.Stat);
            if (tro.Stat.Equals("ok"))
            {
                System.Diagnostics.Debug.WriteLine("Count :" + tro.TimeLogEntry.Task);

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Id :" + tro.Err.Code);
                System.Diagnostics.Debug.WriteLine("Id :" + tro.Err.Msg);
                if (tro.Err.Equals("stopTimeLogging"))
                {

                    throw new CancelTimeLoggingException(tro.Err.stopTime);

                }
            }
            return tro;

        }

        public async Task<DeleteRoot> DeleteTimeLog(string dataset, string timeLogId)
        {
            var dro = await _pDashServices.DeleteTimeLog(Priority.UserInitiated, dataset, timeLogId);
            System.Diagnostics.Debug.WriteLine("Delete status :" + dro.Stat);
            return dro;
        }


        /******* TEST CODE ****/

        public async void TestProject()
        {

            List<Project> projectsList = await GetProjects("mock");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF PROJECTS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList.Select(x => x.Name))
                {
                    System.Diagnostics.Debug.WriteLine(proj);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

        }

        public async void TestTasks()
        {

            List<DTO.Task> projectsList = await GetTasks("mock", "iokdum2d");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF TASKS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList)
                {
                    System.Diagnostics.Debug.WriteLine(proj.FullName + " : " + proj.Id);

                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }



        }

        public async void TestProjectTask()
        {

            List<DTO.Task> projectsList = await GetTasks("mock", "iokdum2d");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF TASKS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList.Select(x => x.Project))
                {
                    System.Diagnostics.Debug.WriteLine(proj.Name);

                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }



        }

        public async void TestTimeLog()
        {
            List<TimeLogEntry> timeLogEntries = await GetTimeLog("mock", 0, null, null, null, null);
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
                System.Diagnostics.Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var proj in timeLogEntries)
                {
                    System.Diagnostics.Debug.WriteLine("Task Name : " + proj.Task.FullName);
                    System.Diagnostics.Debug.WriteLine("Start Date : " + proj.StartDate);
                    System.Diagnostics.Debug.WriteLine("End Date : " + proj.EndDate);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

        }

        public async void TestTimeLogWithId(string taskId)
        {
            List<TimeLogEntry> timeLogEntries = await GetTimeLog("mock", 0, null, null, taskId, null);
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
                System.Diagnostics.Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var proj in timeLogEntries)
                {
                    System.Diagnostics.Debug.WriteLine("Task Name : " + proj.Task.FullName);
                    System.Diagnostics.Debug.WriteLine("Start Date : " + proj.StartDate);
                    System.Diagnostics.Debug.WriteLine("End Date : " + proj.EndDate);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestRecentTasks()
        {
            List<DTO.Task> projectsList = await GetRecentTasks("mock");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF RECENT TASKS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList.Select(x => x.FullName))
                {
                    System.Diagnostics.Debug.WriteLine(proj);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

        }

        public async void TestSingleTask()
        {
            DTO.Task taskItem = await GetTask("mock", "iokdum2d:11401830");
            try
            {
                System.Diagnostics.Debug.WriteLine("** TASK ENTRY **");

                System.Diagnostics.Debug.WriteLine(taskItem.FullName + " : " + taskItem.Id);
                System.Diagnostics.Debug.WriteLine(taskItem.EstimatedTime + " & " + taskItem.ActualTime);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }


        }

        public async Task<int> TestAddATimeLog()
        {
            EditATimeLogRoot tr = await AddATimeLog("INST-szewf0", "Testing a comment", DateTime.Now.ToString(), "305", 2,0,true);
            try
            {
                System.Diagnostics.Debug.WriteLine("** Added a new Time Log entry **");

                System.Diagnostics.Debug.WriteLine(tr.TimeLogEntry.Id);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

            return tr.TimeLogEntry.Id;

        }

        public async void TestUpdateATimeLog()
        {

            string timeLogId = ""+await TestAddATimeLog();

            EditATimeLogRoot tr = await UpdateTimeLog("INST-szewf0",timeLogId, "Testing a comment", DateTime.Now.ToString(Settings.GetInstance().DateTimePattern), "305", 24,2,true);
            try
            {
                System.Diagnostics.Debug.WriteLine("** Updated the new Time Log entry **");
                System.Diagnostics.Debug.WriteLine("Updated Logged Time :"+tr.TimeLogEntry.LoggedTime);
                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

        }

        public async void TestDeleteATimeLog()
        {
            string timeLogId = "" + await TestAddATimeLog();

            DeleteRoot tr = await DeleteTimeLog("INST-szewf0", timeLogId);
            try
            {
                System.Diagnostics.Debug.WriteLine("** Delete the new Time Log entry **");
                System.Diagnostics.Debug.WriteLine("Status :" + tr.Stat);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }


        }
}
    }
