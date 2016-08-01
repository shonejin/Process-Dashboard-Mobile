#region
using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
#endregion
// ReSharper disable UnusedMember.Local
namespace ProcessDashboard.Model
{
    public class TimeLogEntryModel
    {
        [PrimaryKey, Column("row_id")]
        public int RowId { get; set; }

        [Column("timeLogID")]
        public string TimeLogId { get; set; }

        [ForeignKey(typeof(TaskModel))]
        public string TaskId { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("elapsed_time")]
        public double ElapsedTime { get; set; }

        [Column("interrupt_time")]
        public double InterruptTime { get; set; }

        [Column("comment")]
        public string Comment { get; set; }

        [Column("is_open")]
        public bool IsOpen { get; set; }

        [Column("edit_timestamp")]
        public DateTime EditTimestamp { get; set; }

        // Allowed values 'A','M','D'
        [Column("change_flag")]
        public char ChangeFlag { get; set; }
    }
}