using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessDashboard
{
    class Settings
    {
        private static Settings instance;

        private string baseurl;

        public string authHeader
        {
            get
            {
                var authData = string.Format("{0}:{1}", "test", "test");
                return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            }
        }

        private Settings()
        {

        }



        public static Settings GetInstance()
        {
            return instance ?? (instance = new Settings());
        }


    }
}
