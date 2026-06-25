using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManager.Core
{
    public class TaskService : ITaskService
    {
        private List<TaskItem> _tasks = new List<TaskItem>();

        public TaskItem AddTask(TaskItem task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Название задачи не может быть пустым");

            task.Id = Guid.NewGuid();
            _tasks.Add(task);
            return task;
        }

        public List<TaskItem> GetAllTasks()
        {
            return new List<TaskItem>(_tasks);
        }

        public List<TaskItem> FilterByStatus(TaskItemStatus status)
        {
            return _tasks.Where(t => t.Status == status).ToList();
        }

        public List<TaskItem> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAllTasks();

            string lowerQuery = query.ToLower();

            return _tasks.Where(t =>
                t.Title.ToLower().Contains(lowerQuery) ||
                t.Description.ToLower().Contains(lowerQuery)
            ).ToList();
        }

        public bool UpdateTask(TaskItem updatedTask)
        {
            TaskItem existing = _tasks.FirstOrDefault(t => t.Id == updatedTask.Id);

            if (existing == null)
                return false;

            existing.Title = updatedTask.Title;
            existing.Description = updatedTask.Description;
            existing.Priority = updatedTask.Priority;
            existing.Deadline = updatedTask.Deadline;
            existing.Status = updatedTask.Status;
            existing.IsImportant = updatedTask.IsImportant;

            return true;
        }

        public bool DeleteTask(Guid id)
        {
            TaskItem task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return false;

            _tasks.Remove(task);
            return true;
        }

        public List<TaskItem> SortByPriority()
        {
            return _tasks.OrderByDescending(t => t.Priority).ToList();
        }

        public List<TaskItem> SortByDeadline()
        {
            return _tasks.OrderBy(t => t.Deadline).ToList();
        }

        public (int total, int completed, int overdue) GetStatistics()
        {
            int total = _tasks.Count;
            int completed = _tasks.Count(t => t.Status == TaskItemStatus.Завершена);
            int overdue = _tasks.Count(t => t.IsOverdue);

            return (total, completed, overdue);
        }

        public void LoadTasks(List<TaskItem> tasks)
        {
            _tasks.Clear();
            if (tasks != null)
                _tasks.AddRange(tasks);
        }
    }
}