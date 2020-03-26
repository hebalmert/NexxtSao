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

    public class ClientsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Clients/Edit/5
        public ActionResult EditHistory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var clientehistoria = db.ClientHistories.Find(id);
            if (clientehistoria == null)
            {
                return HttpNotFound();
            }

            return View(clientehistoria);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHistory(ClientHistory clienthistory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(clienthistory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = clienthistory.ClientId });
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

            return View(clienthistory);
        }


        // GET: Clients/Create
        public ActionResult AddHistory(int id, int co)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var historia = new ClientHistory
            {
                CompanyId = co,
                ClientId = id,
                Date = DateTime.UtcNow
            };

            return View(historia);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHistory(ClientHistory clienthistory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ClientHistories.Add(clienthistory);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = clienthistory.ClientId });
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

            return View(clienthistory);
        }

        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var odontologos = (from paciente in db.Clients
                               where paciente.Cliente.StartsWith(Prefix) && paciente.CompanyId == user.CompanyId
                               select new
                               {
                                   label = paciente.Cliente,
                                   val = paciente.ClientId
                               }).ToList();

            return Json(odontologos);

        }

        // GET: Clients
        public ActionResult Index(int? clienteid, int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            if (clienteid != null)
            {
                var clients = db.Clients.Where(c => c.CompanyId == user.CompanyId && c.ClientId == clienteid)
                .Include(c => c.City)
                .Include(c => c.Identification)
                .Include(c => c.Zone);
                return View(clients.OrderBy(o => o.Cliente).ToList().ToPagedList((int)page, 10));
            }
            else
            {
                var clients = db.Clients.Where(c => c.CompanyId == user.CompanyId)
                .Include(c => c.City)
                .Include(c => c.Identification)
                .Include(c => c.Zone);
                return View(clients.OrderBy(o => o.Cliente).ToList().ToPagedList((int)page, 10));
            }
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
