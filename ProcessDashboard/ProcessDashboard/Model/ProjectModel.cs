using System;
using SQLite;

namespace ProcessDashboard.Model
{
    [Table("project")]
    public class ProjectModel
    {
        [PrimaryKey,Column("project_id")]
        public string Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }

        [Column("creation_date")]
        public DateTime CreationDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
