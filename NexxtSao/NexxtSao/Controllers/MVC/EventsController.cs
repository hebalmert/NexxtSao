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
    [Authorize(Roles = "User, Dentist")]

    public class EventsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();


        // GET: Events/Delete/5
        public ActionResult PrintDate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Events.Where(r => r.EventId == id).FirstOrDefault();

            if (evento == null)
            {
                return HttpNotFound();
            }
            var docu = db.Identifications.Where(i => i.CompanyId == evento.CompanyId).FirstOrDefault();
            var header = db.HeadTexts.Where(i => i.CompanyId == evento.CompanyId).FirstOrDefault();
            var imagen = db.Companies.Where(i => i.CompanyId == evento.CompanyId).FirstOrDefault();

            var printview = new EventPrint
            {
                Fecha = DateTime.UtcNow,
                DentistId = evento.DentistId,
                Odontologo = evento.Odontologo,
                Cliente = evento.Cliente,
                Hora = evento.Hour.Hora,
                Start = evento.Start,
                Subject = evento.Subject,
                Description = evento.Description,
                TipoDocumento = docu.TipoDocumento,
                HeadText = header.TextoEncabezado,
                Logo = imagen.Logo,
                Compania = imagen.Name,
                Rif = imagen.Rif
            };

            return View(printview);
        }

        // GET: Events/Delete/5
        public ActionResult Asistio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var evento = db.Events.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            evento.Asistencia = true;
            db.Entry(evento).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { fecha = evento.Start, dentistaid = evento.DentistId });
        }

        [HttpPost]
        public JsonResult Search2(string Prefix2)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var profesionales = (from dent in db.Dentists
                                 where dent.Odontologo.StartsWith(Prefix2) && dent.CompanyId == user.CompanyId
                                 select new
                                 {
                                     label = dent.Odontologo,
                                     val = dent.DentistId
                                 }).ToList();

            return Json(profesionales);

        }

        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var clientes = (from client in db.Clients
                            where client.Cliente.StartsWith(Prefix) && client.CompanyId == user.CompanyId
                            select new
                            {
                                label = client.Cliente,
                                val = client.ClientId
                            }).ToList();

            return Json(clientes);

        }

        // GET: Events
        public ActionResult Index(DateTime? fecha, int? dentistaid)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (fecha !=null && dentistaid != 0 && dentistaid != null)
            {
                var events = db.Events.Where(c => c.CompanyId == user.CompanyId && c.Start == fecha && c.DentistId == dentistaid)
                    .Include(e => e.Client)
                    .Include(e => e.Dentist);

                return View(events.OrderBy(o=> o.Hour.Orden).ToList());
            }
            else
            {

                var events = db.Events.Where(c => c.CompanyId == user.CompanyId && c.DentistId == 0)
                    .Include(e => e.Client)
                    .Include(e => e.Dentist);

                return View(events.OrderBy(o => o.Hour.Orden).ToList());
            }
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Events.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }

            return View(evento);
        }

        // GET: Events/Create
        public ActionResult Create(int? clienteId)
        {
            if (clienteId == null)
            {
                return RedirectToAction("Index");
            }
            
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nombrecliente = db.Clients.Find(clienteId);
            var evento = new Event 
            { 
                CompanyId = user.CompanyId,
                ClientId = nombrecliente.ClientId,
                Cliente = nombrecliente.Cliente,
                Start = DateTime.UtcNow,
            };

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora");
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate");

            return View(evento);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event evento)
        {
            var odontologo = db.Dentists.Find(evento.DentistId);
            evento.Odontologo = odontologo.Odontologo;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Events.Add(evento);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { fecha = evento.Start, dentistaid = evento.DentistId });
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(evento.CompanyId), "DentistId", "Odontologo", evento.DentistId);
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora", evento.HourId);
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate", evento.ColorId);

            return View(evento);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Events.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(evento.CompanyId), "DentistId", "Odontologo", evento.DentistId);
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora", evento.HourId);
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate", evento.ColorId);

            return View(evento);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event evento)
        {
            var odontologos = db.Dentists.Find(evento.DentistId);
            evento.Odontologo = odontologos.Odontologo;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(evento).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index", new { fecha = evento.Start, dentistaid = evento.DentistId });
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(evento.CompanyId), "DentistId", "Odontologo", evento.DentistId);
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora", evento.HourId);
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate", evento.ColorId);

            return View(evento);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Events.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var evento = db.Events.Find(id);
            try
            {
                db.Events.Remove(evento);
                db.SaveChanges();

                return RedirectToAction("Index", new { fecha = evento.Start, dentistaid = evento.DentistId });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ModelState.AddModelError(string.Empty, ("El Registo no puede ser Eliminado, porque tiene relacion con otros Datos"));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(evento);
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
