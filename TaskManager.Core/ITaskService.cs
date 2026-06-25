using System;
using System.Collections.Generic;

namespace TaskManager.Core
{
    public interface ITaskService
    {
        TaskItem AddTask(TaskItem task);
        List<TaskItem> GetAllTasks();
        List<TaskItem> FilterByStatus(TaskItemStatus status);
        List<TaskItem> Search(string query);
        bool UpdateTask(TaskItem updatedTask);
        bool DeleteTask(Guid id);
        List<TaskItem> SortByPriority();
        List<TaskItem> SortByDeadline();
        (int total, int completed, int overdue) GetStatistics();
        void LoadTasks(List<TaskItem> tasks);
    }
}