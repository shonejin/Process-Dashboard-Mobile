#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Fusillade;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.DTO;
using ProcessDashboard.Service.Interface;
using Task = ProcessDashboard.DTO.Task;
#endregion
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

        public Controller(IPDashServices pDash)
        {
            _pDashServices = pDash;
            DbManager.GetInstance();
        }

        public Controller()
        {
        }
        // Get list of projects
        public async Task<List<Project>> GetProjects(string dataset)
        {
            var remoteProjects = await _pDashServices.GetProjectsListRemote(Priority.UserInitiated, dataset)
                .ConfigureAwait(false);

            return remoteProjects;
        }

        // Get list of tasks
        public async Task<List<Task>> GetTasks(string dataset, string projectId)
        {
            var remoteTasks = await _pDashServices.GetTasksListRemote(Priority.UserInitiated, dataset, projectId)
                .ConfigureAwait(false);
            return remoteTasks;
        }

        // Get Time Log entries with optional parameters.
        // Optional parameters should be specified as null
        // only dataset is required
        public async Task<List<TimeLogEntry>> GetTimeLogs(string dataset, int? maxResults, DateTime? startDateFrom,
            DateTime? startDateTo, string taskId, string projectId)
        {
            var remoteTimelogs =
                await
                    _pDashServices.GetTimeLogsRemote(Priority.UserInitiated, dataset, maxResults, startDateFrom,
                        startDateTo, taskId, projectId)
                        .ConfigureAwait(false);
            return remoteTimelogs;
        }

        // Get the list of recent tasks
        public async Task<List<Task>> GetRecentTasks(string dataset)
        {
            var remoteTasks = await _pDashServices.GetRecentTasksRemote(Priority.UserInitiated, dataset)
                .ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine("We are under remote tasks :"+remoteTasks.Count);
            return remoteTasks;
        }

        // Get details about a single task
        public async Task<Task> GetTask(string dataset, string taskId)
        {
            var remoteTask = await _pDashServices.GetTaskDetailsRemote(Priority.UserInitiated, dataset, taskId)
                .ConfigureAwait(false);
            return remoteTask;
        }

        // Add a new time log entry
        public async Task<TimeLogEntry> AddATimeLog(string dataset, string comment, DateTime startDate, string taskId,
            double loggedTime, double interruptTime, bool open)

        {
            try
            {
                var tro =
                    await
                        _pDashServices.AddTimeLog(Priority.UserInitiated, dataset, comment, startDate, taskId,
                            loggedTime, interruptTime, open);
                Debug.WriteLine("Id:" + tro.Id);
                return tro;
            }
            catch (CannotReachServerException)
            {
                Debug.WriteLine("Unable to connect to network");
            }
            catch (CancelTimeLoggingException)
            {
                // Add this when needed
                Debug.WriteLine("Cancel Time Logging Right now");
            }
            catch (StatusNotOkayException)
            {
                Debug.WriteLine("Status Not Okay!!");
            }
            catch (Exception)
            {
                // For any other weird exceptions
            }

            //if (tro.Err.Equals("stopTimeLogging"))
            //{

            //    throw new CancelTimeLoggingException(tro.Err.StopTime);

            //}

            return null;
        }

        //Update an existing timelog entry
        public async Task<TimeLogEntry> UpdateTimeLog(string dataset, string timeLogId, string comment,
            DateTime? startDate, string taskId, double? loggedTime, double? interruptTime, bool open)
        {
            try
            {
                var tro =
                    await
                        _pDashServices.UpdateTimeLog(Priority.UserInitiated, dataset, timeLogId, comment, startDate,
                            taskId, loggedTime, interruptTime, open);
                return tro;
            }
            catch (CannotReachServerException)
            {
                Debug.WriteLine("Unable to connect to network");
            }
            catch (CancelTimeLoggingException)
            {
                // Add this when needed
                Debug.WriteLine("Cancel Time Logging Right now");
            }
            catch (StatusNotOkayException)
            {
                Debug.WriteLine("Status Not Okay!! We are in the outer thingy");
            }
            catch (Exception)
            {
                // For any other weird exceptions
            }

            return null;
        }

        //Delete a  timelog entry
        public async Task<DeleteRoot> DeleteTimeLog(string dataset, string timeLogId)
        {
            var dro = await _pDashServices.DeleteTimeLog(Priority.UserInitiated, dataset, timeLogId);
            Debug.WriteLine("Delete status :" + dro.Stat);
            return dro;
        }

        //Read a timelog entry
        public async Task<TimeLogEntry> GetTimeLog(string dataset, string timeLogId)
        {
            var dro = await _pDashServices.GetATimeLogRemote(Priority.UserInitiated, dataset, timeLogId);
            Debug.WriteLine("Get status :" + dro.Id);
            return dro;
        }

        // Update a particular task
        public async Task<Task> UpdateATask(string dataset, string taskId, double? estimatedTime,
            DateTime? completionDate,bool markTaskIncomplete)
        {
            var dro =
                await
                    _pDashServices.UpdateTaskDetails(Priority.UserInitiated, dataset, taskId, estimatedTime,
                        completionDate,markTaskIncomplete);
            Debug.WriteLine("Get status :" + dro);
            return dro;
        }

        /******* TEST CODE ****/
    }
}