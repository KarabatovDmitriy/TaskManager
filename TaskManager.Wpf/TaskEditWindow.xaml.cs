using System;
using System.Windows;
using TaskManager.Core;

namespace TaskManager.Wpf
{
    public partial class TaskEditWindow : Window
    {
        public TaskItem Task { get; private set; }
        private bool _isEditMode;

        // Конструктор для добавления новой задачи
        public TaskEditWindow()
        {
            InitializeComponent();
            Task = new TaskItem();
            _isEditMode = false;

            // Установка значений по умолчанию
            PriorityBox.SelectedIndex = 1; // Средний
            StatusBox.SelectedIndex = 0;   // Новая
            DeadlinePicker.SelectedDate = DateTime.Now.AddDays(1);
        }

        // Конструктор для редактирования существующей задачи
        public TaskEditWindow(TaskItem task)
        {
            InitializeComponent();
            Task = new TaskItem
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                Deadline = task.Deadline,
                Status = task.Status,
                IsImportant = task.IsImportant
            };
            _isEditMode = true;

            // Заполняем поля
            TitleBox.Text = task.Title;
            DescriptionBox.Text = task.Description;
            PriorityBox.SelectedIndex = (int)task.Priority;
            StatusBox.SelectedIndex = (int)task.Status;
            DeadlinePicker.SelectedDate = task.Deadline;
            ImportantCheck.IsChecked = task.IsImportant;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка названия
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Введите название задачи!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Заполняем объект Task
            Task.Title = TitleBox.Text;
            Task.Description = DescriptionBox.Text ?? string.Empty;
            Task.Priority = (TaskPriority)PriorityBox.SelectedIndex;
            Task.Status = (TaskItemStatus)StatusBox.SelectedIndex;
            Task.Deadline = DeadlinePicker.SelectedDate ?? DateTime.Now.AddDays(1);
            Task.IsImportant = ImportantCheck.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}