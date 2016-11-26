using AntiforgerySample.Models;
using AntiforgerySample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiforgerySample.Controllers
{    

    [Route("/api/todos")]
    [AutoValidateAntiforgeryToken]
    public class TodoController: Controller
    {
        private ITodoService service;
        public TodoController(ITodoService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetTodos()
        {
            return Json(service.Get().Where(t => !t.Done));
        }

        [HttpPut]
        public IActionResult CreateTodo([FromBody]TodoModel model)
        {
            return Json(service.Create(model));
        }

        [HttpPost]
        [Route("{id}")]
        public IActionResult UpdateTodo(string id, [FromBody]TodoModel model)
        {
            service.Update(id, model);
            return new OkResult();
        }
    }
}
