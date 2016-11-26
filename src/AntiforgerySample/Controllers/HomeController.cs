using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntiforgerySample.Services;
using AntiforgerySample.Models;

namespace AntiforgerySample.Controllers
{
    public class HomeController : Controller
    {
        private ITodoService service;
        public HomeController(ITodoService service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTodo([Bind("Title, Done")]TodoModel model)
        {
            service.Create(model);
            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
