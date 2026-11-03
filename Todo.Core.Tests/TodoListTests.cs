using System;
using System.IO;
using System.Linq;
using Todo.Core;
using Xunit;

namespace Todo.Core.Tests
{
    public class TodoListTests
    {
        [Fact]
        public void Add_IncreasesCount()
        {
            var list = new TodoList();
            list.Add(" task ");
            Assert.Equal(1, list.Count);
            Assert.Equal("task", list.Items.First().Title);
        }

        [Fact]
        public void Remove_ById_Works()
        {
            var list = new TodoList();
            var item = list.Add("a");
            Assert.True(list.Remove(item.Id));
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void Find_ReturnsMatches()
        {
            var list = new TodoList();
            list.Add("Buy milk");
            list.Add("Read book");
            var found = list.Find("buy").ToList();
            Assert.Single(found);
            Assert.Equal("Buy milk", found[0].Title);
        }

        [Fact]
        public void SaveAndLoad_PreservesTasksAndState()
        {
            var originalList = new TodoList();
            var item1 = originalList.Add("Задача 1");
            var item2 = originalList.Add("Задача 2");
            item1.MarkDone();

            var tempFilePath = Path.Combine(Path.GetTempPath(), $"todos_{Guid.NewGuid():N}.json");

            try
            {
                originalList.Save(tempFilePath);
                Assert.True(File.Exists(tempFilePath));

                var loadedList = new TodoList();
                loadedList.Load(tempFilePath);

                Assert.Equal(2, loadedList.Count);

                var loadedItem1 = loadedList.Items.First(x => x.Id == item1.Id);
                Assert.Equal("Задача 1", loadedItem1.Title);
                Assert.True(loadedItem1.IsDone);

                var loadedItem2 = loadedList.Items.First(x => x.Id == item2.Id);
                Assert.Equal("Задача 2", loadedItem2.Title);
                Assert.False(loadedItem2.IsDone);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Save_ThrowsArgumentException_WhenPathIsEmpty()
        {
            var list = new TodoList();
            Assert.Throws<ArgumentException>(() => list.Save(""));
        }

        [Fact]
        public void Load_ThrowsFileNotFoundException_WhenFileDoesNotExist()
        {
            var list = new TodoList();
            var nonExistentPath = Path.Combine(Path.GetTempPath(), $"not_found_{Guid.NewGuid():N}.json");
            Assert.Throws<FileNotFoundException>(() => list.Load(nonExistentPath));
        }
    }
}
