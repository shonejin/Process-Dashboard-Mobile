#region
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ProcessDashboard.SyncLogic;
#endregion
namespace ProcessDashboard
{
    internal class ControllerTest
    {
        private Controller _ctrl;

        public ControllerTest(Controller ctrl)
        {
            _ctrl = ctrl;
        }
        public async void TestListOfProjects()
        {
            var projectsList = await _ctrl.GetProjects("mock");
            try
            {
                Debug.WriteLine("** LIST OF PROJECTS **");

                Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList)
                {
                    Debug.WriteLine(proj.Id);
                    Debug.WriteLine(proj.CreationDate + " : " + proj.CreationDate.Kind);

                    Debug.WriteLine(proj.IsActive);
                    Debug.WriteLine(proj.Name);

                    Debug.WriteLine("***************");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestRecentTasks()
        {
            var projectsList = await _ctrl.GetRecentTasks("mock");
            try
            {
                Debug.WriteLine("** LIST OF RECENT TASKS **");
                Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList)
                {
                    Debug.WriteLine(proj.Project);
                    Debug.WriteLine(proj.ActualTime);
                    Debug.WriteLine(proj.CompletionDate);
                    Debug.WriteLine(proj.EstimatedTime);
                    Debug.WriteLine(proj.FullName);
                    Debug.WriteLine(proj.Id);
                    Debug.WriteLine(proj.Note);
                    Debug.WriteLine("********************");
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestSingleTask()
        {
            var taskItem = await _ctrl.GetTask("mock", "iokdum2d:11401830");
            Debug.WriteLine("** TASK ENTRY **");
            try
            {
                Debug.WriteLine(taskItem.Project);
                Debug.WriteLine(taskItem.FullName + " : " + taskItem.Id);
                Debug.WriteLine(taskItem.EstimatedTime + " & " + taskItem.ActualTime);
                Debug.WriteLine(taskItem.CompletionDate);
                Debug.WriteLine(taskItem.Note);
                Debug.WriteLine("*********");
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestListOfTasks()
        {
            var tasksList = await _ctrl.GetTasks("mock", "iokdum2d");
            try
            {
                Debug.WriteLine("** LIST OF TASKS **");
                Debug.WriteLine("Length is " + tasksList.Count);

                foreach (var task in tasksList)
                {
                    Debug.WriteLine(task.FullName);
                    try
                    {
                        Debug.WriteLine(task.CompletionDate + " : " +
                                        task.CompletionDate.GetValueOrDefault().Kind);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    Debug.WriteLine(task.ActualTime);
                    Debug.WriteLine(task.EstimatedTime);
                    Debug.WriteLine(task.Id);
                    Debug.WriteLine(task.Note);
                    Debug.WriteLine(task.Project);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestListOfTimeLogs()
        {
            try
            {
                var timeLogEntries = await _ctrl.GetTimeLogs("mock", null, null, null, null, null);

                Debug.WriteLine("** LIST OF Timelog **");
                Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var entry in timeLogEntries)
                {
                    Debug.WriteLine("Task Name : " + entry.Task.FullName);
                    Debug.WriteLine("Start Date : " + entry.StartDate);
                    Debug.WriteLine("End Date : " + entry.EndDate);

                    Debug.WriteLine("Interrupt Time" + entry.InterruptTime);
                    Debug.WriteLine("Logged Time " + entry.LoggedTime);
                    Debug.WriteLine("ID " + entry.Id);
                    Debug.WriteLine("**********************");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestListOfTimeLogsWithMaxResults()
        {
            try
            {
                var timeLogEntries = await _ctrl.GetTimeLogs("mock", 10, null, null, null, null);

                Debug.WriteLine("** LIST OF TestListOfTimeLogsWithMaxResults **");
                Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var entry in timeLogEntries)
                {
                    Debug.WriteLine("Task Name : " + entry.Task.FullName);
                    Debug.WriteLine("Start Date : " + entry.StartDate);
                    Debug.WriteLine("End Date : " + entry.EndDate);

                    Debug.WriteLine("Interrupt Time" + entry.InterruptTime);
                    Debug.WriteLine("Logged Time " + entry.LoggedTime);
                    Debug.WriteLine("ID " + entry.Id);
                    Debug.WriteLine("**********************");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestListOfTimeLogsWithstartDateFrom()
        {
            try
            {
                var dt = new DateTime(2014, 3, 2).ToUniversalTime();
                var timeLogEntries = await _ctrl.GetTimeLogs("mock", null, dt, null, null, null);
                Debug.WriteLine("** LIST OF TestListOfTimeLogsWithstartDateFrom **");
                Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var entry in timeLogEntries)
                {
                    Debug.WriteLine("Task Name : " + entry.Task.FullName);
                    Debug.WriteLine("Start Date : " + entry.StartDate);
                    Debug.WriteLine("End Date : " + entry.EndDate);

                    Debug.WriteLine("Interrupt Time" + entry.InterruptTime);
                    Debug.WriteLine("Logged Time " + entry.LoggedTime);
                    Debug.WriteLine("ID " + entry.Id);
                    Debug.WriteLine("**********************");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestListOfTimeLogsWithstartDateTo()
        {
            try
            {
                var dt = new DateTime(2014, 3, 2).ToUniversalTime();
                var timeLogEntries = await _ctrl.GetTimeLogs("mock", null, null, dt, null, null);
                Debug.WriteLine("** LIST OF TestListOfTimeLogsWithstartDateTo **");
                Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var entry in timeLogEntries)
                {
                    Debug.WriteLine("Task Name : " + entry.Task.FullName);
                    Debug.WriteLine("Start Date : " + entry.StartDate);
                    Debug.WriteLine("End Date : " + entry.EndDate);

                    Debug.WriteLine("Interrupt Time" + entry.InterruptTime);
                    Debug.WriteLine("Logged Time " + entry.LoggedTime);
                    Debug.WriteLine("ID " + entry.Id);
                    Debug.WriteLine("**********************");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestListOfTimeLogsWithtaskId()
        {
            try
            {
                var timeLogEntries = await _ctrl.GetTimeLogs("mock", null, null, null, "i5ixdkxc:15565303", null);

                Debug.WriteLine("** LIST OF TestListOfTimeLogsWithtaskId **");
                Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var entry in timeLogEntries)
                {
                    Debug.WriteLine("Task Name : " + entry.Task.FullName);
                    Debug.WriteLine("Start Date : " + entry.StartDate);
                    Debug.WriteLine("End Date : " + entry.EndDate);
                    Debug.WriteLine("Start Date Kind :"+entry.StartDate.Kind);
                    Debug.WriteLine("End Date Kind :" + entry.EndDate.Kind);
                    Debug.WriteLine("Start Date Universal : " + entry.StartDate.ToUniversalTime());
                    Debug.WriteLine("End Date Universal: " + entry.EndDate.ToUniversalTime());
                    Debug.WriteLine("Interrupt Time" + entry.InterruptTime);
                    Debug.WriteLine("Logged Time " + entry.LoggedTime);
                    Debug.WriteLine("ID " + entry.Id);
                    Debug.WriteLine("**********************");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestListOfTimeLogsWithprojectId()
        {
            try
            {
                var timeLogEntries = await _ctrl.GetTimeLogs("mock", null, null, null, null, "i5ixdkxc");
                Debug.WriteLine("** LIST OF TestListOfTimeLogsWithprojectId **");
                Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var entry in timeLogEntries)
                {
                    Debug.WriteLine("Task Name : " + entry.Task.FullName);
                    Debug.WriteLine("Start Date : " + entry.StartDate);
                    Debug.WriteLine("End Date : " + entry.EndDate);

                    Debug.WriteLine("Interrupt Time" + entry.InterruptTime);
                    Debug.WriteLine("Logged Time " + entry.LoggedTime);
                    Debug.WriteLine("ID " + entry.Id);
                    Debug.WriteLine("**********************");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestTimeLogsWithId(string taskId)
        {
            try
            {
                var timeLogEntries = await _ctrl.GetTimeLogs("mock", 0, null, null, taskId, null);

                Debug.WriteLine("** LIST OF Timelog **");
                Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var proj in timeLogEntries)
                {
                    Debug.WriteLine("Task Name : " + proj.Task.FullName);
                    Debug.WriteLine("Start Date : " + proj.StartDate);
                    Debug.WriteLine("End Date : " + proj.EndDate);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }
        public async Task<int> TestAddATimeLog()
        {
            int id;
            try
            {
                var tr = await _ctrl.AddATimeLog("INST-szewf0", "Testing a comment", DateTime.UtcNow.Subtract(new TimeSpan(5,0,0,0)), "305", 32, 6, true);
                Debug.WriteLine("** Added a new Time Log entry **");
                Debug.WriteLine(tr.Id);
                id = tr.Id;
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
                id = 0;
            }

            await TestGetATimeLog("" + id);

            return id;

        }
        public async Task TestGetATimeLog(string timelogid)
        {
            try
            {

                var value = await _ctrl.GetTimeLog("INST-szewf0", timelogid);
                Debug.WriteLine("Task Name :" + value.Task.FullName);
                Debug.WriteLine(value.Id);
                Debug.WriteLine("Logged Time :" + value.LoggedTime);
                Debug.WriteLine("Interrupt Time :"+value.InterruptTime);
                //Debug.WriteLine(value.StartDate);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(value.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(value.EndDate));
                Debug.WriteLine(value.Comment);


            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }
      

        public async void TestDeleteATimeLog(int? val)
        {
            string timeLogId;
            if (!val.HasValue)
            {
                timeLogId = "" + await TestAddATimeLog();
            }
            else
                timeLogId = "" + val.Value;

            var tr = await _ctrl.DeleteTimeLog("INST-szewf0", timeLogId);
            try
            {
                Debug.WriteLine("** Deleting the Time Log entry **");
                Debug.WriteLine("Status :" + tr.Stat);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }




        public async void TestUpdateATimeLogUpdateComment(string id)
        {
            string timeLogId;

            if (id == null)
                timeLogId = "" + await TestAddATimeLog();
            else
                timeLogId = id;

            var tr =
                await
                    _ctrl.UpdateTimeLog("INST-szewf0", timeLogId, "Testing updating the comment", null, null,null,null,true);
            try
            {
                Debug.WriteLine("** Updated the Time Log Comment **");
                Debug.WriteLine(tr.Task.FullName);
                
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.LoggedTime);
                Debug.WriteLine(tr.InterruptTime);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.EndDate));
                Debug.WriteLine(tr.Comment);

                
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestUpdateATimeLogUpdateStartDate(string id)
        {
            string timeLogId;

            if (id == null)
                timeLogId = "" + await TestAddATimeLog();
            else
                timeLogId = id;

            var tr =
                await
                    _ctrl.UpdateTimeLog("INST-szewf0", timeLogId, null, DateTime.UtcNow, null, null, null, true);
            try
            {
                Debug.WriteLine("** Updated the Time Log Start Date **");
                Debug.WriteLine(tr.Task.Id);
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.LoggedTime);
                Debug.WriteLine(tr.InterruptTime);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.EndDate));
                Debug.WriteLine(tr.Comment);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestUpdateATimeLogUpdateTaskId(string id)
        {
            string timeLogId;

            if (id == null)
                timeLogId = "" + await TestAddATimeLog();
            else
                timeLogId = id;

            var tr =
                await
                    _ctrl.UpdateTimeLog("INST-szewf0", timeLogId, null,null, "306", null, null, true);
            try
            {
                Debug.WriteLine("** Updated the Time Log Task Id **");
                Debug.WriteLine(tr.Task.Id);
                Debug.WriteLine(tr.Task.FullName);
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.LoggedTime);
                Debug.WriteLine(tr.InterruptTime);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.EndDate));
                Debug.WriteLine(tr.Comment);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestUpdateATimeLogUpdateLoggedTime(string id)
        {
            string timeLogId;

            if (id == null)
                timeLogId = "" + await TestAddATimeLog();
            else
                timeLogId = id;

           
            try
            {
                var tr =
               await
                   _ctrl.UpdateTimeLog("INST-szewf0", timeLogId, null, null, null, 20, null, true);
                Debug.WriteLine("** Updated the Time Log Logged Time **");
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.LoggedTime);
                Debug.WriteLine(tr.InterruptTime);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.EndDate));
                Debug.WriteLine(tr.Comment);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestUpdateATimeLogUpdateInterruptTime(string id)
        {
            string timeLogId;

            if (id == null)
                timeLogId = "" + await TestAddATimeLog();
            else
                timeLogId = id;

            var tr =
                await
                    _ctrl.UpdateTimeLog("INST-szewf0", timeLogId, null, null, null,null ,20, true);
            try
            {
                Debug.WriteLine("** Updated the Time Log Interrupt Time **");
                Debug.WriteLine(tr.Id);
                Debug.WriteLine("Logged Time : "+ tr.LoggedTime);
                Debug.WriteLine("Intterupt Time :"+tr.InterruptTime);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.EndDate));
                Debug.WriteLine(tr.Comment);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }



        public async void TestUpdateATimeLogUpdateLoggedTimeNotOpen(string id)
        {
            string timeLogId;

            if (id == null)
                timeLogId = "" + await TestAddATimeLog();
            else
                timeLogId = id;

            var tr =
                await
                    _ctrl.UpdateTimeLog("INST-szewf0", timeLogId, null, null, null, 20, null, false);
            try
            {
                Debug.WriteLine("** Updated the Time Logged Time with False **");
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.LoggedTime);
                Debug.WriteLine(tr.InterruptTime);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.EndDate));
                Debug.WriteLine(tr.Comment);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void TestUpdateATimeLogUpdateInterruptTimeNotOpen(string id)
        {
            string timeLogId;

            if (id == null)
                timeLogId = "" + await TestAddATimeLog();
            else
                timeLogId = id;

            var tr =
                await
                    _ctrl.UpdateTimeLog("INST-szewf0", timeLogId, null, null, null, null, 20, false);
            try
            {
                Debug.WriteLine("** Updated the Time Log Interrupt Time with False **");
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.LoggedTime);
                Debug.WriteLine(tr.InterruptTime);
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.StartDate));
                Debug.WriteLine(Util.GetInstance().GetLocalTime(tr.EndDate));
                Debug.WriteLine(tr.Comment);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }
        }

        
        public async void TestMarkTaskComplete(string taskId)
        {

            var tr =
                await
                    _ctrl.UpdateATask("INST-szewf0", taskId, null, DateTime.UtcNow,false);

            try
            {
                Debug.WriteLine("** Updated the Task and marked it as complete **");
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.FullName);
                Debug.WriteLine(tr.CompletionDate);
                Debug.WriteLine(tr.ActualTime);
                Debug.WriteLine(tr.EstimatedTime);
                Debug.WriteLine(tr.Note);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }



        }

        public async void TestMarkTaskIncomplete(string taskId)
        {
            var tr =
             await
                 _ctrl.UpdateATask("INST-szewf0", taskId, null,null,true);

            try
            {
                Debug.WriteLine("** Updated the Task and marked it as incomplete **");
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.FullName);
                Debug.WriteLine(tr.CompletionDate);
                Debug.WriteLine(tr.ActualTime);
                Debug.WriteLine(tr.EstimatedTime);
                Debug.WriteLine(tr.Note);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }

        }

        public async void TestUpdateEstimatedTime(string taskId, double estimatedTime)
        {
            var tr =
          await
              _ctrl.UpdateATask("INST-szewf0", taskId, estimatedTime, null,false);

            try
            {
                Debug.WriteLine("** Updated the Task and changed the estimated time **");
                Debug.WriteLine(tr.Id);
                Debug.WriteLine(tr.FullName);
                Debug.WriteLine(tr.CompletionDate);
                Debug.WriteLine(tr.ActualTime);
                Debug.WriteLine(tr.EstimatedTime);
                Debug.WriteLine(tr.Note);
            }
            catch (Exception e)
            {
                Debug.WriteLine("We are in an error state :" + e);
            }

        }




    }
}