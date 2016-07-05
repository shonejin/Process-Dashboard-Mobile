using System;
using System.Collections.Generic;
using System.Text;
using ProcessDashboard.Model;

namespace ProcessDashboard.DTO
{
    class Mapper
    {
        public static Mapper Instance;

        private Mapper()
        {

        }

        public static Mapper GetInstance()
        {
            return Instance ?? (Instance = new Mapper());
        }
        /*
         *Project and Project Model based Mappers 
         */
        public List<Project> toProjectList(List<ProjectModel> entries)
        {
            return null;
        }

        public List<ProjectModel> toProjectModelList(List<Project> entries)
        {
            return null;
        }

        public Project toProject(ProjectModel entry)
        {
            return null;
        }

        public ProjectModel toProjectModel(Project entry)
        {
            return null;
        }

        /*
         * Task and Task Model Mappers
         */
        public List<Task> toTaskList(List<TaskModel> entries)
        {
            return null;
        }

        public List<TaskModel> toTaskModelList(List<Task> entries)
        {
            return null;
        }

        public Task toTask(TaskModel entry)
        {
            return null;
        }

        public TaskModel toTaskModel(Task entry)
        {
            return null;
        }


        /*
         * TimeLogEntry Mapper
         * 
         */

        public List<TimeLogEntry> toTimeLogEntryList (List<TimeLogEntryModel> entries)
        {
            return null;
        }

        public List<TimeLogEntryModel> toTimeLogEntryModelList(List<TimeLogEntry> entries)
        {
            return null;
        }

        public TimeLogEntry toTimeLogEntry (TimeLogEntryModel entry)
        {
            return null;
        }

        public TimeLogEntryModel toTimeLogEntryModel (TimeLogEntry entry)
        {
            return null;
        }



    }
}
