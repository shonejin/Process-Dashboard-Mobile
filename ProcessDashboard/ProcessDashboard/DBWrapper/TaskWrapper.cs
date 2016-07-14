using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SQLite;
using ProcessDashboard.Model;

// DB Wrapper for Timelog
// Wrapping up the services

namespace ProcessDashboard.DBWrapper
{
    public class TaskWrapper
    {
        SQLiteConnection db;

        public TaskWrapper(SQLiteConnection db)
        {
            this.db = db;
        }

        public bool createTable()
        {
            try
            {
                db.CreateTable<TaskModel>();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool insertRecord(TaskModel taskModel)
        {
            try
            {
                db.Insert(taskModel);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool insertMultipleRecords(List<TaskModel> entries)
        {
            db.RunInTransaction(() =>
            {
                // database calls inside the transaction
                foreach (TaskModel tm in entries)
                {
                    db.Insert(tm);
                }

            });
            return true;
        }

        public List<TaskModel> GetAllRecords()
        {
            try
            {
                var table = db.Table<TaskModel>().ToList();
                
                return table;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public TaskModel getRecord(string taskID)
        {
            try
            {
                var item = db.Get<TaskModel>(taskID);
                return item;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public bool updateRecord(string taskID, Hashtable ht)
        {
            try
            {
                var item = db.Get<TaskModel>(taskID);

                if (ht.ContainsKey("TaskName"))
                {
                    item.TaskName = ht["TaskName"].ToString();
                }
                if (ht.ContainsKey("ProjectId"))
                {
                    item.ProjectId = ht["ProjectId"].ToString();
                }
                if (ht.ContainsKey("EstimatedTime"))
                {
                    item.EstimatedTime = double.Parse(ht["EstimatedTime"].ToString());
                }
                if (ht.ContainsKey("ActualTime"))
                {
                    item.ActualTime = double.Parse(ht["ActualTime"].ToString());
                }
                if (ht.ContainsKey("TaskNote"))
                {
                    item.TaskNote = (ht["TaskNote"].ToString());
                }
                if (ht.ContainsKey("ProjectOrdinal"))
                {
                    item.ProjectOrdinal = int.Parse(ht["ProjectOrdinal"].ToString());
                }
                if (ht.ContainsKey("RecentOrdinal"))
                {
                    item.ProjectOrdinal = int.Parse(ht["RecentOrdinal"].ToString());
                }
                if (ht.ContainsKey("CompletionDate"))
                {
                    //TODO: Add a new entry to EditToTaskCompletionDate
                    item.CompletionDate = DateTime.Parse(ht["CompletionDate"].ToString());
                }
                db.Update(item);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool deleteRecord(string taskID)
        {
            try
            {
                db.Delete<TaskModel>(taskID);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }

        }

      

    }
}
