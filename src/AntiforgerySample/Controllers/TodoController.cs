using AntiforgerySample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiforgerySample.Controllers
{    

    [Route("/api/todos")]
    public class TodoController: Controller
    {
        private static List<TodoModel> todos = new List<TodoModel>
        {
            new TodoModel { Id = Guid.NewGuid().ToString(), Title = "Learn ASP.Net Core" },
            new TodoModel { Id = Guid.NewGuid().ToString(), Title = "Read about XSRF" }
        };

        [HttpGet]
        public IActionResult GetTodos()
        {
            return Json(todos.Where(t => !t.Done));
        }

        [HttpPut]
        public IActionResult CreateTodo([FromBody]TodoModel model)
        {
            model.Id = Guid.NewGuid().ToString();
            todos.Add(model);
            return Json(model);
        }

        [HttpPost]
        [Route("{id}")]
        public IActionResult UpdateTodo(string id, [FromBody]TodoModel model)
        {
            var todo = todos.SingleOrDefault(t => t.Id == id);
            if (todo == null) return StatusCode(404);

            todo.Done = model.Done;
            todo.Title = model.Title;
            return new OkResult();
        }
    }
}
