using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NexxtSao.Models;
using NexxtSao.Models.MVC;

namespace NexxtSao.Controllers.MVC
{
    [Authorize(Roles = "Admin")]

    public class HoursController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Hours
        public ActionResult Index()
        {
            return View(db.Hours.OrderBy(o=> o.Orden).ToList());
        }

        // GET: Hours/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hour = db.Hours.Find(id);
            if (hour == null)
            {
                return HttpNotFound();
            }
            return View(hour);
        }

        // GET: Hours/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Hour hour)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Hours.Add(hour);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_DoubleData);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            return View(hour);
        }

        // GET: Hours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hour = db.Hours.Find(id);
            if (hour == null)
            {
                return HttpNotFound();
            }
            return View(hour);
        }

        // POST: Hours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Hour hour)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(hour).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_DoubleData);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }
            return View(hour);
        }

        // GET: Hours/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hour hour = db.Hours.Find(id);
            if (hour == null)
            {
                return HttpNotFound();
            }
            return View(hour);
        }

        // POST: Hours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var hour = db.Hours.Find(id);
            try
            {
                db.Hours.Remove(hour);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_Relationship);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(hour);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
