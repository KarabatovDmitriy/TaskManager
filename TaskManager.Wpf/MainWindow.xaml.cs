using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using TaskManager.Core;

namespace TaskManager.Wpf
{
    public partial class MainWindow : Window
    {
        private ITaskService _taskService = new TaskService();
        private FileService _fileService = new FileService();

        public MainWindow()
        {
            InitializeComponent();

            // Заполняем выпадающий список статусов
            StatusFilter.ItemsSource = new List<string>
            {
                "Все",
                "Новая",
                "ВПроцессе",
                "Завершена"
            };
            StatusFilter.SelectedIndex = 0;

            // Загружаем данные при старте
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            TasksGrid.ItemsSource = null;
            TasksGrid.ItemsSource = _taskService.GetAllTasks();
            UpdateStats();
        }

        private void UpdateStats()
        {
            var stats = _taskService.GetStatistics();
            StatsBlock.Text = $"Всего задач: {stats.total} | " +
                             $"Завершено: {stats.completed} | " +
                             $"Просрочено: {stats.overdue}";
        }

        // Поиск по названию или описанию
        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = SearchBox.Text;

            if (string.IsNullOrWhiteSpace(query))
            {
                TasksGrid.ItemsSource = _taskService.GetAllTasks();
            }
            else
            {
                TasksGrid.ItemsSource = _taskService.Search(query);
            }
        }

        // Фильтр по статусу
        private void StatusFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selected = StatusFilter.SelectedItem as string;

            if (selected == null || selected == "Все")
            {
                TasksGrid.ItemsSource = _taskService.GetAllTasks();
                return;
            }

            // Преобразуем строку в enum
            if (Enum.TryParse(selected, out TaskItemStatus status))
            {
                TasksGrid.ItemsSource = _taskService.FilterByStatus(status);
            }
        }

        // Кнопка "Добавить"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TaskEditWindow window = new TaskEditWindow();
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                try
                {
                    _taskService.AddTask(window.Task);
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Кнопка "Изменить"
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selected = TasksGrid.SelectedItem as TaskItem;

            if (selected == null)
            {
                MessageBox.Show("Выберите задачу для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TaskEditWindow window = new TaskEditWindow(selected);
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                _taskService.UpdateTask(window.Task);
                RefreshGrid();
            }
        }

        // Кнопка "Удалить"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selected = TasksGrid.SelectedItem as TaskItem;

            if (selected == null)
            {
                MessageBox.Show("Выберите задачу для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Удалить задачу \"{selected.Title}\"?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _taskService.DeleteTask(selected.Id);
                RefreshGrid();
            }
        }

        // Кнопка "Сохранить"
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json",
                FileName = "tasks.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _fileService.SaveToFileAsync(dialog.FileName, _taskService.GetAllTasks());
                    MessageBox.Show("Задачи сохранены!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Кнопка "Загрузить"
        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    List<TaskItem> tasks = await _fileService.LoadFromFileAsync(dialog.FileName);
                    _taskService.LoadTasks(tasks);
                    RefreshGrid();
                    MessageBox.Show($"Загружено задач: {tasks.Count}", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}