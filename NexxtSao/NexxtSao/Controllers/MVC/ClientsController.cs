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

    public class ClientsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Clients
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var clients = db.Clients.Where(c => c.CompanyId == user.CompanyId)
                .Include(c => c.City)
                .Include(c => c.Identification)
                .Include(c => c.Zone);
            return View(clients.OrderByDescending(o=> o.Cliente).ToList());
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var clientes = new Client
            {
                CompanyId = user.CompanyId,
                Date = DateTime.Today,
                Nacimiento = DateTime.Today,
                Activo = true
            };

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(user.CompanyId), "CityId", "Ciudad");
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(user.CompanyId), "IdentificationId", "TipoDocumento");
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(user.CompanyId), "ZoneId", "Zona");

            return View(clientes);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Client client)
        {
            string n1 = client.FirstName;
            string n2 = client.LastName;
            client.Cliente = n1 + " " + n2;

            if (ModelState.IsValid)
            {               
                try
                {
                    db.Clients.Add(client);
                    db.SaveChanges();
                    
                    if (client.PhotoFile != null)
                    {
                        var folder = "~/Content/Patients";
                        var file = string.Format("{0}.jpg", client.ClientId);
                        var response = FilesHelper.UploadPhoto(client.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            client.Photo = pic;
                            db.Entry(client).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(client.CompanyId), "CityId", "Ciudad", client.CityId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(client.CompanyId), "ZoneId", "Zona", client.ZoneId);
            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(client.CompanyId), "CityId", "Ciudad", client.CityId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(client.CompanyId), "ZoneId", "Zona", client.ZoneId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Client client)
        {
            string n1 = client.FirstName;
            string n2 = client.LastName;
            client.Cliente = n1 + " " + n2;

            if (ModelState.IsValid)
            {
                try
                {
                    if (client.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/Patients";
                        var file = string.Format("{0}.jpg", client.ClientId);
                        var response = FilesHelper.UploadPhoto(client.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            client.Photo = pic;
                        }
                    }

                    db.Entry(client).State = EntityState.Modified;
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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(client.CompanyId), "CityId", "Ciudad", client.CityId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentification(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(client.CompanyId), "ZoneId", "Zona", client.ZoneId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var client = db.Clients.Find(id);
            db.Clients.Remove(client);
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
            return View(client);
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
