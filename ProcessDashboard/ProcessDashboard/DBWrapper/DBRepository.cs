using System;
using System.Diagnostics;
using System.IO;
using SQLite;

namespace ProcessDashboard.DBWrapper
{
    public class DBManager
    {
        private static DBManager instance;

        public ProjectWrapper pw;
        public TaskWrapper tw;
        public TimeLogWrapper tlw;

        private static SQLiteConnection db;

        private const string DB_NAME = "pdash.db3";

        // Private Constructor for Singleton Class.
        private DBManager()
        {
            createConnection();

            pw = new ProjectWrapper(db);
            tw = new TaskWrapper(db);
            tlw = new TimeLogWrapper(db);

            
            //Create the table
            pw.createTable();
            tw.createTable();
            tlw.createTable();
        }

        public static DBManager getInstance()
        {
            return instance ?? (instance = new DBManager());
        }

        //Create the Database
        public static bool createDB()
        {
            try {
                createConnection();
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
        private static void createConnection()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DB_NAME);
            db = new SQLiteConnection(dbPath);
        }
    }
}
