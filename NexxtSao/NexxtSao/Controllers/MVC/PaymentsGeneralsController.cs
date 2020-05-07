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
    public class PaymentsGeneralsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: PaymentsGenerals
        public ActionResult Index(int? DentistId)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (DentistId != 0)
            {
                var directGenerals = db.PaymentsGenerals.Where(c => c.CompanyId == user.CompanyId && c.Facturado == false && c.DentistId == DentistId)
                    .Include(d => d.Client)
                    .Include(d => d.Dentist);
                ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");
                return View(directGenerals.OrderByDescending(t => t.Date).ToList());
            }
            else
            {
                var directGenerals = db.PaymentsGenerals.Where(c => c.CompanyId == user.CompanyId && c.Facturado == false)
                    .Include(d => d.Client)
                    .Include(d => d.Dentist);
                ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");
                return View(directGenerals.OrderByDescending(t => t.Date).ToList());
            }
        }

        // GET: PaymentsGenerals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var paymentsGeneral = db.PaymentsGenerals.Find(id);
            if (paymentsGeneral == null)
            {
                return HttpNotFound();
            }
            return View(paymentsGeneral);
        }

        // GET: PaymentsGenerals/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Photo");
            ViewBag.DentistId = new SelectList(db.Dentists, "DentistId", "Odontologo");
            ViewBag.HeadTextId = new SelectList(db.HeadTexts, "HeadTextId", "TextoEncabezado");
            return View();
        }

        // POST: PaymentsGenerals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PaymentsGeneral paymentsGeneral)
        {
            if (ModelState.IsValid)
            {
                db.PaymentsGenerals.Add(paymentsGeneral);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Photo", paymentsGeneral.ClientId);
            ViewBag.DentistId = new SelectList(db.Dentists, "DentistId", "Odontologo", paymentsGeneral.DentistId);
            ViewBag.HeadTextId = new SelectList(db.HeadTexts, "HeadTextId", "TextoEncabezado", paymentsGeneral.HeadTextId);
            return View(paymentsGeneral);
        }

        // GET: PaymentsGenerals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentsGeneral paymentsGeneral = db.PaymentsGenerals.Find(id);
            if (paymentsGeneral == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Photo", paymentsGeneral.ClientId);
            ViewBag.DentistId = new SelectList(db.Dentists, "DentistId", "Odontologo", paymentsGeneral.DentistId);
            ViewBag.HeadTextId = new SelectList(db.HeadTexts, "HeadTextId", "TextoEncabezado", paymentsGeneral.HeadTextId);
            return View(paymentsGeneral);
        }

        // POST: PaymentsGenerals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PaymentsGeneral paymentsGeneral)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymentsGeneral).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Photo", paymentsGeneral.ClientId);
            ViewBag.DentistId = new SelectList(db.Dentists, "DentistId", "Odontologo", paymentsGeneral.DentistId);
            ViewBag.HeadTextId = new SelectList(db.HeadTexts, "HeadTextId", "TextoEncabezado", paymentsGeneral.HeadTextId);
            return View(paymentsGeneral);
        }

        // GET: PaymentsGenerals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentsGeneral paymentsGeneral = db.PaymentsGenerals.Find(id);
            if (paymentsGeneral == null)
            {
                return HttpNotFound();
            }
            return View(paymentsGeneral);
        }

        // POST: PaymentsGenerals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PaymentsGeneral paymentsGeneral = db.PaymentsGenerals.Find(id);
            db.PaymentsGenerals.Remove(paymentsGeneral);
            db.SaveChanges();
            return RedirectToAction("Index");
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
