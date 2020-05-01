using NexxtSao.Classes;
using NexxtSao.Models;
using NexxtSao.Models.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NexxtSao.Controllers.MVC
{
    [Authorize(Roles = "User")]  // [Authorize(Roles = "User, Profe")] para usarse por dos perfiles de usuarios


    public class CalendarController : Controller
    {

        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Calendar
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DentistUp = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");
            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");
            ViewBag.ClientId = new SelectList(ComboHelper.GetClient(user.CompanyId), "ClientId", "Cliente");

            return View();
        }

        public JsonResult SaveEvent(Event e)
        {
            var status = false;

            db.Configuration.ProxyCreationEnabled = false;
            var dentista = db.Dentists.Find(e.DentistId);
            var clientes = db.Clients.Find(e.ClientId);

            {
                if (e.EventId > 0)
                {
                    //Update the event

                    var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                    var v = db.Events.Where(a => a.EventId == e.EventId).FirstOrDefault();
                    if (v != null)
                    {
                        v.CompanyId = user.CompanyId;
                        v.DentistId = e.DentistId;
                        v.Odontologo = dentista.Odontologo;
                        v.ClientId = e.ClientId;
                        v.Cliente = clientes.Cliente;
                        v.Subject = e.Subject;
                        v.Start = TimeZoneInfo.ConvertTimeFromUtc(e.Start, ComboHelper.GetTimeZone());
                        v.End = TimeZoneInfo.ConvertTimeFromUtc(e.End, ComboHelper.GetTimeZone());
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                    //DateTime date1 = e.Start;
                    //TimeZoneInfo tz = TimeZoneInfo.CreateCustomTimeZone("COLOMBIA", new TimeSpan(-3, 0, 0), "Colombia", "Colombia");
                    //DateTime custDateTime1 = TimeZoneInfo.ConvertTimeFromUtc(date1, tz);

                    var nuevoEvento = new Event
                    {
                        CompanyId = user.CompanyId,
                        DentistId = e.DentistId,
                        Odontologo = dentista.Odontologo,
                        ClientId = e.ClientId,
                        Cliente = clientes.Cliente,
                        Subject = e.Subject,
                        Start = TimeZoneInfo.ConvertTimeFromUtc(e.Start, ComboHelper.GetTimeZone()),
                        End = TimeZoneInfo.ConvertTimeFromUtc(e.End, ComboHelper.GetTimeZone()),
                        Description = e.Description,
                        IsFullDay = e.IsFullDay,
                        ThemeColor = e.ThemeColor,
                    };
                    db.Events.Add(nuevoEvento);
                }
                db.SaveChanges();

                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int EventId)
        {
            var status = false;
            db.Configuration.ProxyCreationEnabled = false;
            {
                var v = db.Events.Where(a => a.EventId == EventId).FirstOrDefault();
                if (v != null)
                {
                    db.Events.Remove(v);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult GetEvents(int? professionalId)
        {
            {
                db.Configuration.ProxyCreationEnabled = false;
                var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var compania = user.CompanyId;
                
                if (professionalId > 0)
                {
                    var events = db.Events.Where(p => p.DentistId == professionalId && p.CompanyId == compania).ToList();
                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var events = db.Events.Where(p => p.CompanyId == compania && p.DentistId == 0).ToList();
                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
        }

        public JsonResult CallProfesional()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var compania = user.CompanyId;
           
            var profe = db.Dentists.Where(c => c.CompanyId == compania);
            return Json(profe);
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