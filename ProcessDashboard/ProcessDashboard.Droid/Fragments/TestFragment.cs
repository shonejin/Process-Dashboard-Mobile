#region
using System;
using Android.App;
using Android.OS;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class TestFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Debug.WriteLine("On Create");
            RetainInstance = true;

            var dslr = new DataSetLocationResolver();
            string url, id;
            dslr.ResolveFromToken("go.yn-hk1", out url, out id);

            Debug.WriteLine("URL is :" + url);
            Debug.WriteLine("Id is :" + id);

            try
            {
                var ct = new ControllerTest(((MainActivity)Activity).Ctrl);

                //ct.TestListOfProjects();
                //ct.TestRecentTasks();
                //ct.TestListOfTasks();
                //ct.TestSingleTask();
                //                ct.TestListOfTimeLogs();
                //                ct.TestListOfTimeLogsWithMaxResults();
                //                ct.TestListOfTimeLogsWithprojectId();
                //                ct.TestListOfTimeLogsWithstartDateFrom();
                //                ct.TestListOfTimeLogsWithstartDateTo();
                                ct.TestListOfTimeLogsWithtaskId();
                //ct.TestAddATimeLog();
                
                //ct.TestGetATimeLog("-71");
                //ct.TestDeleteATimeLog(-71);

                string tid = "-87";
                //ct.TestUpdateATimeLogUpdateComment(tid);
                //ct.TestUpdateATimeLogUpdateInterruptTimeNotOpen(tid);
                ct.TestUpdateATimeLogUpdateLoggedTime(tid);
                // ct.TestUpdateATimeLogUpdateStartDate(tid);
                //ct.TestUpdateATimeLogUpdateTaskId(tid); 
                //ct.TestUpdateATimeLogUpdateInterruptTime(tid);
                //ct.TestUpdateATimeLogUpdateLoggedTimeNotOpen(tid);
               // ct.TestUpdateEstimatedTime("330", 125);
                //ct.TestMarkTaskComplete("330");
               // ct.TestMarkTaskIncomplete("330");


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
                Debug.WriteLine("Some other weird exception");
            }

            // Create your fragment here
        }
    }
}