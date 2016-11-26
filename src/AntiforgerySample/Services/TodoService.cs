using AntiforgerySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiforgerySample.Services
{
    public class TodoService: ITodoService
    {
        private List<TodoModel> todos = new List<TodoModel>
        {
            new TodoModel { Id = Guid.NewGuid().ToString(), Title = "Learn ASP.Net Core" },
            new TodoModel { Id = Guid.NewGuid().ToString(), Title = "Read about XSRF" }
        };

        public IEnumerable<TodoModel> Get() => todos;

        public TodoModel Create(TodoModel todo)
        {
            todo.Id = Guid.NewGuid().ToString();
            todos.Add(todo);
            return todo;
        }

        public void Update(string id, TodoModel updatedTodo)
        {
            var todo = todos.SingleOrDefault(t => t.Id == id);
            if (todo == null) throw new ArgumentException("Id does not exists");

            todo.Done = updatedTodo.Done;
            todo.Title = updatedTodo.Title;
        }
    }
}
