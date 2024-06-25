using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Trelloo.Models;

namespace Trelloo.Controllers
{
    public class HomeController : Controller
    {
        private ToDoContext context;
        public HomeController(ToDoContext ctx) => context = ctx;
        public IActionResult Index(string id)
        {
            var filetes = new Filters(id);
            ViewBag.Filters = filetes;
            ViewBag.Categories = context.categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<Todo> query = context.ToDos
                .Include(t => t.Category).Include(t => t.Status);

            if(filetes.HasCategory)
            {
                query = query.Where(t => t.CategoryId == filetes.CategoryId);
            }

            if (filetes.HasDue)
            {
                var today = DateTime.Today;
                if (filetes.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filetes.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (filetes.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);
                }
            }

            if (filetes.HasStatus)
            {
                query = query.Where(t => t.StatusId == filetes.StatusId);
            }

            var tasks = query.OrderBy(t => t.DueDate).ToList();
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = context.categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            var task = new Todo { StatusId = "open" };
            return View(task);
        }

        [HttpPost]
        public IActionResult Add(Todo task) 
        {
            if (ModelState.IsValid)
            {
                context.ToDos.Add(task);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = context.categories.ToList();
                ViewBag.Statuses = context.Statuses.ToList();
                return View(task);
            }
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join("-", filter);
            return RedirectToAction("Index", new {ID = id});
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute]string id, Todo selected)
        {
            selected = context.ToDos.Find(selected.Id)!;
            if(selected != null)
            {
                selected.StatusId = "closed";
                context.SaveChanges();
            }

            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            var toDelete = context.ToDos.Where(t => t.StatusId == "closed").ToList();
            foreach (var task in toDelete)
            {
                context.ToDos.Remove(task);
            }
            context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }
    }
}