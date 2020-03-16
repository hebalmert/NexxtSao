﻿using System;
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

    public class DentistsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Dentists/Delete/5
        public ActionResult DeletePercentage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentistpercentage = db.DentistPercentages.Find(id);
            if (dentistpercentage == null)
            {
                return HttpNotFound();
            }
            return PartialView(dentistpercentage);
        }

        // POST: Dentists/Delete/5
        [HttpPost, ActionName("DeletePercentage")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePercentageConfirmed(int id)
        {
            var dentistpercentage = db.DentistPercentages.Find(id);
            db.DentistPercentages.Remove(dentistpercentage);
            try
            {
                db.SaveChanges();
                return RedirectToAction("Details", new { id = dentistpercentage.DentistId});
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
            return PartialView(dentistpercentage);
        }

        // GET: Dentists/Edit/5
        public ActionResult EditPercentage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentistporcentaje = db.DentistPercentages.Find(id);
            if (dentistporcentaje == null)
            {
                return HttpNotFound();
            }

            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(dentistporcentaje.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", dentistporcentaje.TreatmentCategoryId);

            return PartialView(dentistporcentaje);
        }

        // POST: Dentists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPercentage(DentistPercentage dentistporcentaje)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(dentistporcentaje).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = dentistporcentaje.DentistId});
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

            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(dentistporcentaje.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", dentistporcentaje.TreatmentCategoryId);

            return PartialView(dentistporcentaje);
        }


        // GET: Dentists/Create
        public ActionResult AddPercentage(int id, int co) //id = DentistId y co = CompanyId
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var porcentajes = new DentistPercentage
            {
                CompanyId = co,
                DentistId = id
            };

            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(co), "TreatmentCategoryId", "CategoriaTratamiento");

            return PartialView(porcentajes);
        }

        // POST: Dentists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPercentage(DentistPercentage dentistPercentage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.DentistPercentages.Add(dentistPercentage);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id= dentistPercentage.DentistId});
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

            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(dentistPercentage.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", dentistPercentage.TreatmentCategoryId);

            return PartialView(dentistPercentage);
        }


        // GET: Dentists
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var dentists = db.Dentists.Where(c => c.CompanyId == user.CompanyId)
                .Include(d => d.City)
                .Include(d => d.DentistSpecialty)
                .Include(d => d.Identification)
                .Include(d => d.Zone);
            return View(dentists.OrderByDescending(o=> o.Odontologo).ToList());
        }

        // GET: Dentists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentist = db.Dentists.Find(id);
            if (dentist == null)
            {
                return HttpNotFound();
            }
            return View(dentist);
        }

        // GET: Dentists/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var dentistas = new Dentist
            {
                CompanyId = user.CompanyId,
                Date = DateTime.Today,
                Activo = true
            };

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(user.CompanyId), "CityId", "Ciudad");
            ViewBag.DentistSpecialtyId = new SelectList(ComboHelper.GetDentistSpecialty(user.CompanyId), "DentistSpecialtyId", "Especialidad");
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(user.CompanyId), "IdentificationId", "TipoDocumento");
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(user.CompanyId), "ZoneId", "Zona");
            return View(dentistas);
        }

        // POST: Dentists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dentist dentist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Dentists.Add(dentist);
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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(dentist.CompanyId), "CityId", "Ciudad", dentist.CityId);
            ViewBag.DentistSpecialtyId = new SelectList(ComboHelper.GetDentistSpecialty(dentist.CompanyId), "DentistSpecialtyId", "Especialidad", dentist.DentistSpecialtyId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(dentist.CompanyId), "IdentificationId", "TipoDocumento", dentist.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(dentist.CompanyId), "ZoneId", "Zona", dentist.ZoneId);
            return View(dentist);
        }

        // GET: Dentists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentist = db.Dentists.Find(id);
            if (dentist == null)
            {
                return HttpNotFound();
            }

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(dentist.CompanyId), "CityId", "Ciudad", dentist.CityId);
            ViewBag.DentistSpecialtyId = new SelectList(ComboHelper.GetDentistSpecialty(dentist.CompanyId), "DentistSpecialtyId", "Especialidad", dentist.DentistSpecialtyId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(dentist.CompanyId), "IdentificationId", "TipoDocumento", dentist.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(dentist.CompanyId), "ZoneId", "Zona", dentist.ZoneId);

            return View(dentist);
        }

        // POST: Dentists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Dentist dentist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(dentist).State = EntityState.Modified;
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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(dentist.CompanyId), "CityId", "Ciudad", dentist.CityId);
            ViewBag.DentistSpecialtyId = new SelectList(ComboHelper.GetDentistSpecialty(dentist.CompanyId), "DentistSpecialtyId", "Especialidad", dentist.DentistSpecialtyId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(dentist.CompanyId), "IdentificationId", "TipoDocumento", dentist.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(dentist.CompanyId), "ZoneId", "Zona", dentist.ZoneId);

            return View(dentist);
        }

        // GET: Dentists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dentist = db.Dentists.Find(id);
            if (dentist == null)
            {
                return HttpNotFound();
            }
            return View(dentist);
        }

        // POST: Dentists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var dentist = db.Dentists.Find(id);
            db.Dentists.Remove(dentist);
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
            return View(dentist);
        }

        public JsonResult GetZone(int cityId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var zones = db.Zones.Where(c => c.CityId == cityId);
            return Json(zones);
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
