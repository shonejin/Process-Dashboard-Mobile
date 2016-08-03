#region
using System;
using System.Text;
#endregion
namespace ProcessDashboard
{
    internal class Settings
    {
        private static Settings _instance;
        //TODO: Remove b4 production
        private readonly string _baseurl = " https://pdes.tuma-solutions.com/";
        private readonly string _password;

        private readonly string _username;

        private Settings()
        {
            CheckWifi = false;
            //TODO: Remove this before shipping to production
            _username = "test";
            _password = "test";
        }

        public bool CheckWifi { get; set; }
        //TODO: Remove b4 production
        //public string Dataset => "mock";
        public string Dataset => "INST-szewf0";

        public string AuthHeader
        {
            get
            {
                var authData = $"{_username}:{_password}";
                return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            }
        }
        //public string Dataset => "INST-szewf0";

        public string GetApiAddress()
        {
            return _baseurl;
        }

        public static Settings GetInstance()
        {
            return _instance ?? (_instance = new Settings());
        }
    }
}