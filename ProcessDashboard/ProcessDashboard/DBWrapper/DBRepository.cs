using System;
using System.Diagnostics;
using System.IO;
using SQLite;

namespace ProcessDashboard.DBWrapper
{
    public class DbManager
    {
        private static DbManager _instance;

        public ProjectWrapper Pw;
        public TaskWrapper Tw;
        public TimeLogWrapper Tlw;

        private static SQLiteConnection _db;

        private const string DbName = "pdash.db3";

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
            try {
                CreateConnection();
                Debug.WriteLine("DB Created");
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine("DB Creation failed : " + e.Message);
                return false;
            }
        }

        // Create the connection to the DB.
        private static void CreateConnection()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DbName);
            _db = new SQLiteConnection(dbPath);
        }
    }
}
