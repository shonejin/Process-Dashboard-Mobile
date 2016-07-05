using System;
using System.Collections.Generic;
using System.Linq;
using Java.Util;
using ProcessDashboard.Model;
using SQLite;

namespace ProcessDashboard.DBWrapper
{
    public class ProjectWrapper
    {
        SQLiteConnection db;

        public ProjectWrapper(SQLiteConnection db)
        {
            this.db = db;
        }

        public bool createTable()
        {
            try
            {
                db.CreateTable<ProjectModel>();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool insertRecord(ProjectModel projectModel)
        {
            try
            {
                db.Insert(projectModel);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool insertOrUpdateRecord(ProjectModel projectModel)
        {
            try
            {
                db.InsertOrReplace(projectModel);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool insertMultipleRecords(List<ProjectModel> entries)
        {
                // database calls inside the transaction
                foreach (ProjectModel pm in entries)
                {
                    db.InsertOrReplace(pm);
                }

            return true;
        }
        public List<ProjectModel> GetAllRecords()
        {
            try
            {
                var table = db.Table<ProjectModel>().ToList();
                // Can we directly return table ??
                return table;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public ProjectModel getRecord(string projectID)
        {
            try
            {
                var item = db.Get<ProjectModel>(projectID);
                return item;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public bool updateRecord(string projectID, Hashtable ht)
        {
            try
            {
                var item = db.Get<ProjectModel>(projectID);

                if (ht.ContainsKey("Name"))
                {
                    item.Name = ht.Get("Name").ToString();
                }
                if (ht.ContainsKey("CreationDate"))
                {
                    item.CreationDate = DateTime.Parse(ht.Get("CreationDate").ToString());
                }
                if (ht.ContainsKey("IsActive"))
                {
                    item.IsActive = bool.Parse(ht.Get("IsActive").ToString());
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

        public bool deleteRecord(string projectID)
        {
            try
            {
                db.Delete<ProjectModel>(projectID);
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
