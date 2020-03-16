using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NexxtSao.Classes;
using NexxtSao.Models;
using NexxtSao.Models.MVC;

namespace NexxtSao.Controllers.MVC
{
    [Authorize(Roles = "User")]

    public class TreatmentsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Treatments
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var treatments = db.Treatments.Where(c => c.CompanyId == user.CompanyId)
                .Include(t => t.Tax)
                .Include(t => t.TreatmentCategory);
            return View(treatments.OrderByDescending(o=> o.TreatmentCategory.CategoriaTratamiento).ThenByDescending(o=> o.Servicio).ToList());
        }

        // GET: Treatments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var treatment = db.Treatments.Find(id);
            if (treatment == null)
            {
                return HttpNotFound();
            }
            return View(treatment);
        }

        // GET: Treatments/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var tratamiento = new Treatment
            {
                CompanyId = user.CompanyId
            };

            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(user.CompanyId), "TaxId", "Impuesto");
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(user.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento");
            
            return View(tratamiento);
        }

        // POST: Treatments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Treatments.Add(treatment);
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

            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(treatment.CompanyId), "TaxId", "Impuesto", treatment.TaxId);
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(treatment.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", treatment.TreatmentCategoryId);
            return View(treatment);
        }

        // GET: Treatments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var treatment = db.Treatments.Find(id);
            if (treatment == null)
            {
                return HttpNotFound();
            }

            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(treatment.CompanyId), "TaxId", "Impuesto", treatment.TaxId);
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(treatment.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", treatment.TreatmentCategoryId);
            return View(treatment);
        }

        // POST: Treatments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(treatment).State = EntityState.Modified;
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

            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(treatment.CompanyId), "TaxId", "Impuesto", treatment.TaxId);
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(treatment.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", treatment.TreatmentCategoryId);
            return View(treatment);
        }

        // GET: Treatments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var treatment = db.Treatments.Find(id);
            if (treatment == null)
            {
                return HttpNotFound();
            }
            return View(treatment);
        }

        // POST: Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var treatment = db.Treatments.Find(id);
            db.Treatments.Remove(treatment);
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
            return View(treatment);
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
