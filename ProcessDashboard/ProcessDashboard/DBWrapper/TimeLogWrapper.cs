#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProcessDashboard.Model;
using SQLite;
#endregion
namespace ProcessDashboard.DBWrapper
{
    public class TimeLogWrapper
    {
        private readonly SQLiteConnection _db;

        public TimeLogWrapper(SQLiteConnection db)
        {
            _db = db;
        }

        public bool CreateTable()
        {
            try
            {
                _db.CreateTable<TimeLogEntryModel>();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool InsertMultipleRecords(List<TimeLogEntryModel> entries)
        {
            _db.RunInTransaction(() =>
            {
                // database calls inside the transaction
                foreach (var tem in entries)
                {
                    _db.Insert(tem);
                }
            });
            return true;
        }

        public bool InsertRecord(TimeLogEntryModel timelogentry)
        {
            try
            {
                _db.Insert(timelogentry);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public List<TimeLogEntryModel> GetAllRecords()
        {
            try
            {
                var table = _db.Table<TimeLogEntryModel>().ToList();

                return table;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public TimeLogEntryModel GetRecord(string rowId)
        {
            try
            {
                var item = _db.Get<TimeLogEntryModel>(rowId);
                return item;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public bool UpdateRecord(string rowId, Hashtable ht)
        {
            try
            {
                var item = _db.Get<TimeLogEntryModel>(rowId);

                if (ht.ContainsKey("TimeLogId"))
                {
                    item.TimeLogId = ht["TimeLogId"].ToString();
                }
                if (ht.ContainsKey("TaskId"))
                {
                    item.TaskId = ht["TaskId"].ToString();
                }
                if (ht.ContainsKey("StartDate"))
                {
                    item.StartDate = DateTime.Parse(ht["StartDate"].ToString());
                }
                if (ht.ContainsKey("ElapsedTime"))
                {
                    //Change has happened to an elapsed time
                    //TODO: Add elapsed thingy
                    item.ElapsedTime = double.Parse(ht["ElapsedTime"].ToString());
                }
                if (ht.ContainsKey("InterruptTime"))
                {
                    item.InterruptTime = double.Parse(ht["InterruptTime"].ToString());
                }
                if (ht.ContainsKey("Comment"))
                {
                    item.Comment = ht["Comment"].ToString();
                }
                if (ht.ContainsKey("IsOpen"))
                {
                    item.IsOpen = bool.Parse(ht["IsOpen"].ToString());
                }
                if (ht.ContainsKey("ChangeFlag"))
                {
                    item.ChangeFlag = ht["ChangeFlag"].ToString()[0];
                }
                item.EditTimestamp = DateTime.Now;
                _db.Update(item);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool DeleteRecord(string rowId)
        {
            try
            {
                _db.Delete<TimeLogEntryModel>(rowId);
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