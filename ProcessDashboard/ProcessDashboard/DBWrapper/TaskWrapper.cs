#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProcessDashboard.Model;
using SQLite;
#endregion
// DB Wrapper for Timelog
// Wrapping up the services
namespace ProcessDashboard.DBWrapper
{
    public class TaskWrapper
    {
        private readonly SQLiteConnection _db;

        public TaskWrapper(SQLiteConnection db)
        {
            _db = db;
        }

        public bool CreateTable()
        {
            try
            {
                _db.CreateTable<TaskModel>();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool InsertRecord(TaskModel taskModel)
        {
            try
            {
                _db.Insert(taskModel);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool InsertMultipleRecords(List<TaskModel> entries)
        {
            _db.RunInTransaction(() =>
            {
                // database calls inside the transaction
                foreach (var tm in entries)
                {
                    _db.Insert(tm);
                }
            });
            return true;
        }

        public List<TaskModel> GetAllRecords()
        {
            try
            {
                var table = _db.Table<TaskModel>().ToList();

                return table;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public TaskModel GetRecord(string taskId)
        {
            try
            {
                var item = _db.Get<TaskModel>(taskId);
                return item;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public bool UpdateRecord(string taskId, Hashtable ht)
        {
            try
            {
                var item = _db.Get<TaskModel>(taskId);

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
                    item.TaskNote = ht["TaskNote"].ToString();
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
                _db.Update(item);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool DeleteRecord(string taskId)
        {
            try
            {
                _db.Delete<TaskModel>(taskId);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}