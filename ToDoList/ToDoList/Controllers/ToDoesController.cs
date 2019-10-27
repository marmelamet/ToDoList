using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDoList.DAL.Model;

namespace ToDoList.Controllers
{
    public class ToDoesController : Controller
    {
        private readonly DbContext ctx;
        private readonly DbSet<Todo> todoRepo;
        public ToDoesController(DbContext _ctx)
        {
            ctx = _ctx;
            todoRepo = _ctx.Set<Todo>();
        }
        // GET: ToDoes
        public ActionResult Index()
        {
            var list = todoRepo.ToList();
            return View(list);
        }

        // GET: ToDoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo toDo = todoRepo.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // GET: ToDoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BittiMi,Baslik,Aciklama,Tarih")] Todo toDo)
        {
            if (ModelState.IsValid)
            {
               
                
                toDo.KullaniciId = "";
                todoRepo.Add(toDo);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toDo);
        }

        // GET: ToDoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo toDo = todoRepo.AsNoTracking().FirstOrDefault(c => c.ID == id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BittiMi,Baslik,Aciklama,Tarih")] Todo toDo)
        {
            if (ModelState.IsValid)
            {
                //todoRepo.Attach(toDo);
                ctx.Entry<Todo>(toDo).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDo);
        }

        // GET: ToDoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo toDo = todoRepo.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Todo toDo = todoRepo.Find(id);
            todoRepo.Remove(toDo);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult CheckEvent()
        {
            var checkTime = DateTime.Now.AddMinutes(5);
            var events = todoRepo.Where(c => c.Bittimi == false && c.Tarih <= checkTime);
            if (events.Count()>0)
            {
                var list = events.ToList();
                var eventCount = list.Count();
                foreach (var item in events)
                {
                    item.Bittimi = true;
                    ctx.Entry<Todo>(item).State = EntityState.Modified;
                }
                ctx.SaveChanges();
                return Json(new { success = true,EventCount= eventCount, Events = list });
            }
            return Json(new { success = true, EventCount = 0 });
        }
    }
}
