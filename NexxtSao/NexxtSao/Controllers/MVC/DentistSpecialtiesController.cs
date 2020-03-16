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
    [Authorize(Roles = "User")]

    public class DentistSpecialtiesController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: DentistSpecialties
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var dentistSpecialties = db.DentistSpecialties.Where(c => c.CompanyId == user.CompanyId);

            return View(dentistSpecialties.OrderByDescending(o=> o.Especialidad).ToList());
        }

        // GET: DentistSpecialties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentistSpecialty = db.DentistSpecialties.Find(id);
            if (dentistSpecialty == null)
            {
                return HttpNotFound();
            }
            return View(dentistSpecialty);
        }

        // GET: DentistSpecialties/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var especialidad = new DentistSpecialty { CompanyId = user.CompanyId };

            return View(especialidad);
        }

        // POST: DentistSpecialties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DentistSpecialty dentistSpecialty)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.DentistSpecialties.Add(dentistSpecialty);
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

            return View(dentistSpecialty);
        }

        // GET: DentistSpecialties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentistSpecialty = db.DentistSpecialties.Find(id);
            if (dentistSpecialty == null)
            {
                return HttpNotFound();
            }

            return View(dentistSpecialty);
        }

        // POST: DentistSpecialties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DentistSpecialty dentistSpecialty)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(dentistSpecialty).State = EntityState.Modified;
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

            return View(dentistSpecialty);
        }

        // GET: DentistSpecialties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentistSpecialty = db.DentistSpecialties.Find(id);
            if (dentistSpecialty == null)
            {
                return HttpNotFound();
            }
            return View(dentistSpecialty);
        }

        // POST: DentistSpecialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var dentistSpecialty = db.DentistSpecialties.Find(id);
            db.DentistSpecialties.Remove(dentistSpecialty);
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
            return View(dentistSpecialty);
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
