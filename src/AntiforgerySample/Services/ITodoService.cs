using AntiforgerySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiforgerySample.Services
{
    public interface ITodoService
    {
        IEnumerable<TodoModel> Get();

        TodoModel Create(TodoModel todo);

        void Update(string id, TodoModel updatedTodo);
    }
}
