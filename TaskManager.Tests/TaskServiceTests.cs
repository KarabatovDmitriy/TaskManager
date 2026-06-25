using System;
using System.Linq;
using Xunit;
using TaskManager.Core;

namespace TaskManager.Tests
{
    public class TaskServiceTests
    {
        [Fact]
        public void AddTask_ValidTask_ShouldAddToList()
        {
            // Arrange
            ITaskService service = new TaskService();
            TaskItem task = new TaskItem { Title = "Тестовая задача" };

            // Act
            service.AddTask(task);

            // Assert
            Assert.Single(service.GetAllTasks());
        }

        [Fact]
        public void AddTask_EmptyTitle_ShouldThrowException()
        {
            // Arrange
            ITaskService service = new TaskService();
            TaskItem task = new TaskItem { Title = "" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.AddTask(task));
        }

        [Fact]
        public void FilterByStatus_ShouldReturnCorrectTasks()
        {
            // Arrange
            ITaskService service = new TaskService();
            service.AddTask(new TaskItem { Title = "Задача 1", Status = TaskItemStatus.Новая });
            service.AddTask(new TaskItem { Title = "Задача 2", Status = TaskItemStatus.Завершена });

            // Act
            var completed = service.FilterByStatus(TaskItemStatus.Завершена);

            // Assert
            Assert.Single(completed);
            Assert.Equal("Задача 2", completed.First().Title);
        }

        [Fact]
        public void Search_ShouldFindByTitle()
        {
            // Arrange
            ITaskService service = new TaskService();
            service.AddTask(new TaskItem { Title = "Купить молоко" });
            service.AddTask(new TaskItem { Title = "Полить цветы" });

            // Act
            var results = service.Search("молоко");

            // Assert
            Assert.Single(results);
        }

        [Fact]
        public void DeleteTask_ExistingId_ShouldRemoveTask()
        {
            // Arrange
            ITaskService service = new TaskService();
            TaskItem task = new TaskItem { Title = "Удаляемая задача" };
            service.AddTask(task);

            // Act
            bool result = service.DeleteTask(task.Id);

            // Assert
            Assert.True(result);
            Assert.Empty(service.GetAllTasks());
        }

        [Fact]
        public void GetStatistics_ShouldCountCorrectly()
        {
            // Arrange
            ITaskService service = new TaskService();
            service.AddTask(new TaskItem { Title = "Новая", Status = TaskItemStatus.Новая });
            service.AddTask(new TaskItem { Title = "Завершённая", Status = TaskItemStatus.Завершена });
            service.AddTask(new TaskItem
            {
                Title = "Просроченная",
                Status = TaskItemStatus.Новая,
                Deadline = DateTime.Now.AddDays(-5)
            });

            // Act
            var stats = service.GetStatistics();

            // Assert
            Assert.Equal(3, stats.total);
            Assert.Equal(1, stats.completed);
            Assert.Equal(1, stats.overdue);
        }
    }
}