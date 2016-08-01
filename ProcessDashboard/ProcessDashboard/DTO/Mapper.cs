#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.Model;
#endregion
namespace ProcessDashboard.DTO
{
    internal class Mapper
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
        public List<Project> ToProjectList(List<ProjectModel> entries)
        {
            if (entries.Count == 0)
                return null;

            var output = new List<Project>();

            foreach (var pm in entries)
            {
                var temp = new Project {CreationDate = pm.CreationDate, Id = pm.Id, Name = pm.Name};
                output.Add(temp);
            }

            return output;
        }

        public List<ProjectModel> ToProjectModelList(List<Project> entries)
        {
            if (entries.Count == 0)
                return null;

            var output = new List<ProjectModel>();

            foreach (var pm in entries)
            {
                var temp = new ProjectModel {CreationDate = pm.CreationDate, Id = pm.Id, Name = pm.Name};
                output.Add(temp);
            }

            return output;
        }

        public Project ToProject(ProjectModel entry)
        {
            return new Project {CreationDate = entry.CreationDate, Id = entry.Id, Name = entry.Name};
        }

        public ProjectModel ToProjectModel(Project entry)
        {
            return new ProjectModel {CreationDate = entry.CreationDate, Id = entry.Id, Name = entry.Name};
        }

        /*
         * Task and Task Model Mappers
         */
        public List<Task> ToTaskList(List<TaskModel> entries)
        {
            if (entries.Count == 0)
                return null;

            var output = new List<Task>();

            var pm = DbManager.GetInstance().Pw.GetRecord(entries.First().ProjectId);
            var p = ToProject(pm);
            foreach (var tm in entries)
            {
                var temp = new Task
                {
                    Id = tm.TaskId,
                    ActualTime = tm.ActualTime,
                    CompletionDate = tm.CompletionDate,
                    EstimatedTime = tm.EstimatedTime,
                    FullName = tm.TaskName,
                    Project = p,
                    TaskNote = tm.TaskNote
                };
                output.Add(temp);
            }

            return output;
        }

        public List<TaskModel> ToTaskModelList(List<Task> entries)
        {
            if (entries == null || entries.Count == 0)
                return null;
            Debug.WriteLine("Mapper :" + " 1");
            var output = new List<TaskModel>();
            Debug.WriteLine("Mapper :" + " 2");
            foreach (var tm in entries)
            {
                try
                {
                    var temp = new TaskModel
                    {
                        TaskId = tm.Id,
                        ActualTime = tm.ActualTime,
                        CompletionDate = tm.CompletionDate.GetValueOrDefault(),
                        EstimatedTime = tm.EstimatedTime,
                        TaskName = tm.FullName,
                        ProjectId = tm.Project.Id,
                        TaskNote = tm.TaskNote
                    };
                    output.Add(temp);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            Debug.WriteLine("Mapper :" + " 4");
            return output;
        }

        public Task ToTask(TaskModel entry)
        {
            var p = ToProject(DbManager.GetInstance().Pw.GetRecord(entry.ProjectId));
            return new Task
            {
                Id = entry.TaskId,
                ActualTime = entry.ActualTime,
                CompletionDate = entry.CompletionDate,
                EstimatedTime = entry.EstimatedTime,
                FullName = entry.TaskName,
                Project = p,
                TaskNote = entry.TaskNote
            };
        }

        public TaskModel ToTaskModel(Task entry)
        {
            return new TaskModel
            {
                TaskId = entry.Id,
                ActualTime = entry.ActualTime,
                CompletionDate = entry.CompletionDate.GetValueOrDefault(),
                EstimatedTime = entry.EstimatedTime,
                TaskName = entry.FullName,
                ProjectId = entry.Project.Id,
                TaskNote = entry.TaskNote
            };
        }

        /*
         * TimeLogEntry Mapper
         * 
         */

        public List<TimeLogEntry> ToTimeLogEntryList(List<TimeLogEntryModel> entries)
        {
            if (entries.Count == 0)
                return null;

            var output = new List<TimeLogEntry>();

            var t = ToTask(DbManager.GetInstance().Tw.GetRecord(entries.First().TaskId));

            foreach (var tm in entries)
            {
                var temp = new TimeLogEntry
                {
                    Id = tm.RowId,
                    InterruptTime = tm.InterruptTime,
                    StartDate = tm.StartDate,
                    Task = t,
                    EndDate = tm.EndDate,
                    LoggedTime = tm.ElapsedTime
                };
                output.Add(temp);
            }

            return output;
        }

        public List<TimeLogEntryModel> ToTimeLogEntryModelList(List<TimeLogEntry> entries)
        {
            if (entries.Count == 0)
                return null;

            var output = new List<TimeLogEntryModel>();

            foreach (var tm in entries)
            {
                var temp = new TimeLogEntryModel
                {
                    RowId = tm.Id,
                    InterruptTime = tm.InterruptTime,
                    StartDate = tm.StartDate,
                    TaskId = tm.Task.Id,
                    EndDate = tm.EndDate,
                    ElapsedTime = tm.LoggedTime
                };
                output.Add(temp);
            }

            return output;
        }

        public TimeLogEntry ToTimeLogEntry(TimeLogEntryModel entry)
        {
            var t = ToTask(DbManager.GetInstance().Tw.GetRecord(entry.TaskId));
            return new TimeLogEntry
            {
                Id = entry.RowId,
                InterruptTime = entry.InterruptTime,
                StartDate = entry.StartDate,
                Task = t,
                EndDate = entry.EndDate,
                LoggedTime = entry.ElapsedTime
            };
        }

        public TimeLogEntryModel ToTimeLogEntryModel(TimeLogEntry entry)
        {
            return new TimeLogEntryModel
            {
                RowId = entry.Id,
                InterruptTime = entry.InterruptTime,
                StartDate = entry.StartDate,
                TaskId = entry.Task.Id,
                EndDate = entry.EndDate,
                ElapsedTime = entry.LoggedTime
            };
        }
    }
}