using System;

namespace TaskManager.Core
{
    public enum TaskItemStatus
    {
        Новая,
        ВПроцессе,
        Завершена
    }

    public enum TaskPriority
    {
        Низкий,
        Средний,
        Высокий,
        Критический
    }

    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Средний;
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(1);
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Новая;
        public bool IsImportant { get; set; } = false;

        public bool IsOverdue
        {
            get
            {
                if (Status == TaskItemStatus.Завершена)
                    return false;
                return DateTime.Now > Deadline;
            }
        }

        public override string ToString()
        {
            return $"{Title} [{Status}]";
        }
    }
}
