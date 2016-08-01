#region
using System;
using System.Text;
#endregion
namespace ProcessDashboard
{
    internal class Settings
    {
        private static Settings _instance;
        
		/*
        private readonly string _baseurl = " https://pdes.tuma-solutions.com/";
        private readonly string _password = "test";
        private readonly string _username = "test";
        public string Dataset {
            get { return "INST-szewf0"; }
        }
		*/

        public bool CheckWifi { get; set; }
        
        public static Settings GetInstance()
        {
            return _instance ?? (_instance = new Settings());
        }
    }
}