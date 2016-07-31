#region
using System;
#endregion
namespace ProcessDashboard
{
    internal class Util
    {
        private static Util _instance;

        private string _dateTimePattern = "yyyy-MM-dd\'T\'HH:mm:ssZ";

        private bool fake = true;

        private Util()
        {
        }

        public static Util GetInstance()
        {
            return _instance ?? (_instance = new Util());
        }

        public DateTime GetLocalTime(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime;
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime.ToLocalTime();
            // Assuming that the input is in UTC time here
            return dateTime.ToLocalTime();
        }
        // The format to be sent to the server is always assumed to be UTC for simplicity.
        public DateTime GetServerTime(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime.Kind == DateTimeKind.Local)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            // Assuming that the input is in local time here
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public string GetEditTimeStamp()
        {
            if (fake)
                return DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 0, 0)).ToString(_dateTimePattern);
            else
                return DateTime.UtcNow.ToString(_dateTimePattern);
        }


        

        /// <summary>
        ///     /// This method returns the string format of the datetime object, which can be sent to the server
        /// </summary>
        /// <param name="input"> The Date Time Input </param>
        /// <returns> String representation of the given input in UTC time</returns>
        public string GetServerTimeString(DateTime? input)
        {
            if (!input.HasValue)
                return null;
            // This gives the input time in UTC
            var value = GetServerTime(input.Value);
            return value.ToString(_dateTimePattern);
        }
    }
}