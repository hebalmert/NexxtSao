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
using PagedList;

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

        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var odontologos = (from dentist in db.Dentists
                            where dentist.Odontologo.StartsWith(Prefix) && dentist.CompanyId == user.CompanyId
                            select new
                            {
                                label = dentist.Odontologo,
                                val = dentist.DentistId
                            }).ToList();

            return Json(odontologos);

        }

        // GET: Dentists
        public ActionResult Index(int? DentistId, int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            if (DentistId != null)
            {
                var dentists = db.Dentists.Where(c => c.CompanyId == user.CompanyId && c.DentistId == DentistId)
                .Include(d => d.City)
                .Include(d => d.DentistSpecialty)
                .Include(d => d.Identification)
                .Include(d => d.Zone);
                return View(dentists.OrderBy(o => o.Odontologo).ToList().ToPagedList((int)page, 10));
            }
            else
            {
                var dentists = db.Dentists.Where(c => c.CompanyId == user.CompanyId)
                .Include(d => d.City)
                .Include(d => d.DentistSpecialty)
                .Include(d => d.Identification)
                .Include(d => d.Zone);
                return View(dentists.OrderBy(o => o.Odontologo).ToList().ToPagedList((int)page, 10));
            }
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
                    dentist.Odontologo = dentist.FirstName + " " + dentist.LastName;

                    db.Dentists.Add(dentist);
                    db.SaveChanges();

                    UsersHelper.CreateUserASP(dentist.UserName, "Dentist");

                    if (dentist.PhotoFile != null)
                    {
                        var folder = "~/Content/Dentist";
                        var file = string.Format("{0}.jpg", dentist.DentistId);
                        var response = FilesHelper.UploadPhoto(dentist.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            dentist.Photo = pic;
                            db.Entry(dentist).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    var usuario = new User
                    {
                        UserName = dentist.UserName,
                        FirstName = dentist.FirstName,
                        LastName = dentist.LastName,
                        Phone = dentist.Movil,
                        Address = dentist.Address,
                        Puesto = "Dentist",
                        CompanyId = dentist.CompanyId
                    };
                    db.Users.Add(usuario);
                    db.SaveChanges();
                    db.Dispose();

                    return RedirectToAction("Details", new { id = dentist.DentistId});
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
                    dentist.Odontologo = dentist.FirstName + " " + dentist.LastName;
                    db.Entry(dentist).State = EntityState.Modified;

                    //Se verifica si la foto cambio para actualizarla
                    if (dentist.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/Dentist";
                        var file = string.Format("{0}.jpg", dentist.DentistId);
                        var response = FilesHelper.UploadPhoto(dentist.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            dentist.Photo = pic;
                        }
                    }
                    //Fin Foto

                    if (dentist.Activo == true)
                    {
                        var db2 = new NexxtSaoContext();
                        var currentTech = db2.Dentists.Find(dentist.DentistId);
                        if (currentTech.UserName != dentist.UserName)
                        {
                            var db3 = new NexxtSaoContext();
                            var usuarios = db3.Users.Where(c => c.UserName == currentTech.UserName && c.FirstName == currentTech.FirstName && c.LastName == currentTech.LastName).FirstOrDefault();
                            if (usuarios != null)
                            {
                                usuarios.UserName = dentist.UserName;
                                db3.Entry(usuarios).State = EntityState.Modified;
                                db3.SaveChanges();
                                db3.Dispose();
                            }
                            else
                            {
                                var db4 = new NexxtSaoContext();
                                var usuario = new User
                                {
                                    UserName = dentist.UserName,
                                    FirstName = dentist.FirstName,
                                    LastName = dentist.LastName,
                                    Phone = dentist.Movil,
                                    Address = dentist.Address,
                                    Puesto = "Dentist",
                                    CompanyId = dentist.CompanyId
                                };
                                db4.Users.Add(usuario);
                                db4.SaveChanges();
                                db4.Dispose();
                                UsersHelper.CreateUserASP(dentist.UserName, "Dentist");
                            }
                            UsersHelper.UpdateUserName(currentTech.UserName, dentist.UserName);
                        }
                        else
                        {
                            var db3 = new NexxtSaoContext();
                            var usuarios = db3.Users.Where(c => c.UserName == currentTech.UserName && c.FirstName == currentTech.FirstName && c.LastName == currentTech.LastName).FirstOrDefault();
                            if (usuarios != null)
                            {
                                usuarios.UserName = dentist.UserName;
                                db3.Entry(usuarios).State = EntityState.Modified;
                                db3.SaveChanges();
                                db3.Dispose();
                            }
                            else
                            {
                                var db4 = new NexxtSaoContext();
                                var usuario = new User
                                {
                                    UserName = dentist.UserName,
                                    FirstName = dentist.FirstName,
                                    LastName = dentist.LastName,
                                    Phone = dentist.Movil,
                                    Address = dentist.Address,
                                    Puesto = "Dentist",
                                    CompanyId = dentist.CompanyId
                                };
                                db4.Users.Add(usuario);
                                db4.SaveChanges();
                                db4.Dispose();
                                UsersHelper.CreateUserASP(dentist.UserName, "Dentist");
                            }
                            UsersHelper.UpdateUserName(currentTech.UserName, dentist.UserName);
                        }
                    }

                    if (dentist.Activo == false)
                    {
                        var db5 = new NexxtSaoContext();
                        var currentTech2 = db5.Dentists.Find(dentist.DentistId);

                        var db6 = new NexxtSaoContext();
                        var usuarios = db6.Users.Where(c => c.UserName == currentTech2.UserName && c.FirstName == currentTech2.FirstName && c.LastName == currentTech2.LastName).FirstOrDefault();
                        if (usuarios != null)
                        {
                            db6.Users.Remove(usuarios);
                            db6.SaveChanges();
                            db6.Dispose();
                        }
                        UsersHelper.DeleteUser(dentist.UserName);
                        //UsersHelper.UpdateUserName(currentTech2.UserName, technical.UserName);

                        db5.Dispose();
                    }

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
            var foto = string.Empty;
            foto = dentist.Photo;
            try
            {
                if (foto != null || string.IsNullOrEmpty(foto))
                {
                    var response = FilesHelper.DeletePhoto(foto);
                    if (response == true)
                    {
                        db.Dentists.Remove(dentist);
                        db.SaveChanges();
                    }
                    else
                    {
                        var ex = new Exception();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(dentist);
                    }
                }
                else
                {
                    db.Dentists.Remove(dentist);
                    db.SaveChanges();
                }
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
