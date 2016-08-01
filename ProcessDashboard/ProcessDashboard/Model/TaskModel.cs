#region
using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
#endregion
// ReSharper disable UnusedMember.Local
namespace ProcessDashboard.Model
{
    public class TaskModel
    {
        [PrimaryKey, Column("task_id")]
        public string TaskId { get; set; }

        [Column("task_name")]
        public string TaskName { get; set; }

        [ForeignKey(typeof(ProjectModel))]
        public string ProjectId { get; set; }

        [Column("completion_date")]
        public DateTime CompletionDate { get; set; }

        [Column("estimated_time")]
        public double EstimatedTime { get; set; }

        [Column("actual_time")]
        public double ActualTime { get; set; }

        [Column("task_note")]
        public string TaskNote { get; set; }

        [Column("project_ordinal")]
        public int ProjectOrdinal { get; set; }

        [Column("recent_ordinal")]
        public int RecentOrdinal { get; set; }
    }
}