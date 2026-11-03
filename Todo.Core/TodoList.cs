using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Todo.Core
{
    public class TodoList
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new()
        {
            WriteIndented = true
        };

        private readonly List<TodoItem> _items = new();

        public IReadOnlyList<TodoItem> Items => _items.AsReadOnly();

        public TodoItem Add(string title)
        {
            var item = new TodoItem(title);
            _items.Add(item);
            return item;
        }

        public bool Remove(Guid id) => _items.RemoveAll(i => i.Id == id) > 0;

        public IEnumerable<TodoItem> Find(string substring) =>
            _items.Where(i => i.Title.Contains(substring ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        public int Count => _items.Count;

        public void Save(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or whitespace", nameof(path));

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize<IReadOnlyList<TodoItem>>(_items, s_jsonOptions);
            File.WriteAllText(path, json);
        }

        public void Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or whitespace", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("Todo file not found", path);

            var json = File.ReadAllText(path);
            var items = JsonSerializer.Deserialize<List<TodoItem>>(json, s_jsonOptions);

            _items.Clear();
            if (items != null)
            {
                _items.AddRange(items);
            }
        }
    }
}
