#region
using System;
using System.Diagnostics;
using System.IO;
using SQLite;
#endregion
namespace ProcessDashboard.DBWrapper
{
    public class DbManager
    {
        private const string DbName = "pdash.db3";
        private static DbManager _instance;

        private static SQLiteConnection _db;

        public ProjectWrapper Pw;
        public TimeLogWrapper Tlw;
        public TaskWrapper Tw;

        // Private Constructor for Singleton Class.
        private DbManager()
        {
            CreateConnection();

            Pw = new ProjectWrapper(_db);
            Tw = new TaskWrapper(_db);
            Tlw = new TimeLogWrapper(_db);

            //Create the table
            Pw.CreateTable();
            Tw.CreateTable();
            Tlw.CreateTable();
        }

        public static DbManager GetInstance()
        {
            return _instance ?? (_instance = new DbManager());
        }

        //Create the Database
        public static bool CreateDb()
        {
            try
            {
                CreateConnection();
                Debug.WriteLine("DB Created");
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("DB Creation failed : " + e.Message);
                return false;
            }
        }

        // Create the connection to the DB.
        private static void CreateConnection()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DbName);
            _db = new SQLiteConnection(dbPath);
        }
    }
}