using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessDashboard.DBWrapper;
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
            if (entries.Count == 0)
                return null;

            List<Project> output = new List<Project>();

            foreach (ProjectModel pm in entries)
            {
                Project temp = new Project() {creationDate = pm.CreationDate, id = pm.Id, name = pm.Name};
                output.Add(temp);
            }


            return output;

        }

        public List<ProjectModel> toProjectModelList(List<Project> entries)
        {
            if (entries.Count == 0)
                return null;

            List<ProjectModel> output = new List<ProjectModel>();

            foreach (Project pm in entries)
            {
                ProjectModel temp = new ProjectModel() { CreationDate = pm.creationDate, Id = pm.id, Name = pm.name };
                output.Add(temp);
            }

            return output;
        }

        public Project toProject(ProjectModel entry)
        {
            return new Project() { creationDate = entry.CreationDate, id = entry.Id, name = entry.Name };
        }

        public ProjectModel toProjectModel(Project entry)
        {
            return new ProjectModel() { CreationDate = entry.creationDate, Id = entry.id, Name = entry.name }; 
        }

        /*
         * Task and Task Model Mappers
         */
        public List<Task> toTaskList(List<TaskModel> entries)
        {

            if (entries.Count == 0)
                return null;

            List<Task> output = new List<Task>();

            ProjectModel pm = DBManager.getInstance().pw.getRecord(entries.First().ProjectId);
            Project p = toProject(pm);
            foreach (TaskModel tm in entries)
            {
                Task temp = new Task() { id  = tm.TaskId, actualTime = tm.ActualTime, completionDate = tm.CompletionDate,estimatedTime = tm.EstimatedTime, fullName = tm.TaskName, project = p, taskNote = tm.TaskNote};
                output.Add(temp);
            }


            return output;


        }

        public List<TaskModel> toTaskModelList(List<Task> entries)
        {
            if (entries==null || entries.Count == 0)
                return null;
            System.Diagnostics.Debug.WriteLine("Mapper :"+" 1");
            List<TaskModel> output = new List<TaskModel>();
            System.Diagnostics.Debug.WriteLine("Mapper :" + " 2");
            foreach (Task tm in entries)
            {
                try
                {
                    TaskModel temp = new TaskModel()
                    {
                        TaskId = tm.id ?? null,
                        ActualTime = tm.actualTime,
                        CompletionDate = tm.completionDate,
                        EstimatedTime = tm.estimatedTime,
                        TaskName = tm.fullName ?? null,
                        ProjectId = tm.project.id ?? null,
                        TaskNote = tm.taskNote ?? null
                    };
                    output.Add(temp);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            System.Diagnostics.Debug.WriteLine("Mapper :" + " 4");
            if(output==null) { System.Diagnostics.Debug.WriteLine("Output is null!!");}
            return output;
        }

        public Task toTask(TaskModel entry)
        {
            Project p = toProject(DBManager.getInstance().pw.getRecord(entry.ProjectId));
            return new Task() { id = entry.TaskId, actualTime = entry.ActualTime, completionDate = entry.CompletionDate, estimatedTime = entry.EstimatedTime, fullName = entry.TaskName, project = p, taskNote = entry.TaskNote };
        }

        public TaskModel toTaskModel(Task entry)
        {
            return new TaskModel() { TaskId = entry.id, ActualTime = entry.actualTime, CompletionDate = entry.completionDate, EstimatedTime = entry.estimatedTime, TaskName = entry.fullName, ProjectId = entry.project.id, TaskNote = entry.taskNote };
        }


        /*
         * TimeLogEntry Mapper
         * 
         */

        public List<TimeLogEntry> toTimeLogEntryList (List<TimeLogEntryModel> entries)
        {
            if (entries.Count == 0)
                return null;

            List<TimeLogEntry> output = new List<TimeLogEntry>();

            Task t = toTask(DBManager.getInstance().tw.getRecord(entries.First().TaskId));

            foreach (TimeLogEntryModel tm in entries)
            {
                TimeLogEntry temp = new TimeLogEntry() { id = tm.RowId, interruptTime = tm.InterruptTime, startDate = tm.StartDate, task = t, endDate = tm.EndDate, loggedTime = tm.ElapsedTime};
                output.Add(temp);
            }


            return output;

        }

        public List<TimeLogEntryModel> toTimeLogEntryModelList(List<TimeLogEntry> entries)
        {
            if (entries.Count == 0)
                return null;

            List<TimeLogEntryModel> output = new List<TimeLogEntryModel>();

            foreach (TimeLogEntry tm in entries)
            {
                TimeLogEntryModel temp = new TimeLogEntryModel() { RowId = tm.id, InterruptTime = tm.interruptTime, StartDate = tm.startDate, TaskId = tm.task.id, EndDate = tm.endDate, ElapsedTime = tm.loggedTime };
                output.Add(temp);
            }


            return output;
        }

        public TimeLogEntry toTimeLogEntry (TimeLogEntryModel entry)
        {
            Task t = toTask(DBManager.getInstance().tw.getRecord(entry.TaskId));
            return new TimeLogEntry() { id = entry.RowId, interruptTime = entry.InterruptTime, startDate = entry.StartDate, task = t, endDate = entry.EndDate, loggedTime = entry.ElapsedTime };
        }

        public TimeLogEntryModel toTimeLogEntryModel (TimeLogEntry entry)
        {
            return new TimeLogEntryModel() { RowId = entry.id, InterruptTime = entry.interruptTime, StartDate = entry.startDate, TaskId = entry.task.id, EndDate = entry.endDate, ElapsedTime = entry.loggedTime };}



    }
}
