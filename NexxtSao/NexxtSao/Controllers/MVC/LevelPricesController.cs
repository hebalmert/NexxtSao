using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NexxtSao.Models;

namespace NexxtSao.Controllers
{
    [Authorize(Roles = "Admin")]

    public class LevelPricesController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: LevelPrices
        public ActionResult Index()
        {
            return View(db.LevelPrices.ToList());
        }

        // GET: LevelPrices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var levelPrice = db.LevelPrices.Find(id);
            if (levelPrice == null)
            {
                return HttpNotFound();
            }
            return View(levelPrice);
        }

        // GET: LevelPrices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LevelPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LevelPriceId,NivelPrecio")] LevelPrice levelPrice)
        {
            if (ModelState.IsValid)
            {
                db.LevelPrices.Add(levelPrice);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_DoubleData));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            return View(levelPrice);
        }

        // GET: LevelPrices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var levelPrice = db.LevelPrices.Find(id);
            if (levelPrice == null)
            {
                return HttpNotFound();
            }
            return View(levelPrice);
        }

        // POST: LevelPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LevelPriceId,NivelPrecio")] LevelPrice levelPrice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(levelPrice).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_DoubleData));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }
            return View(levelPrice);
        }

        // GET: LevelPrices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var levelPrice = db.LevelPrices.Find(id);
            if (levelPrice == null)
            {
                return HttpNotFound();
            }
            return View(levelPrice);
        }

        // POST: LevelPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var levelPrice = db.LevelPrices.Find(id);
            db.LevelPrices.Remove(levelPrice);
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_Relationship));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(levelPrice);
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
