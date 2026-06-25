using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskManager.Core
{
    public class FileService
    {
        public async Task SaveToFileAsync(string filePath, List<TaskItem> tasks)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(tasks, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<TaskItem>> LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<TaskItem>();

            string json = await File.ReadAllTextAsync(filePath);

            List<TaskItem> tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);

            return tasks ?? new List<TaskItem>();
        }
    }
}
