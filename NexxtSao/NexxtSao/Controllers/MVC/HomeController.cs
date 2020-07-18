using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NexxtSao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NexxtSao.Controllers
{
    public class HomeController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (user != null)
            {
                var db2 = new NexxtSaoContext();
                var companyUp = db2.Companies.Find(user.CompanyId);
                bool comActivo = companyUp.Activo;
                db2.Dispose();

                DateTime hoy = DateTime.Today;
                DateTime current = companyUp.DateHasta;
                if (comActivo == false)   //Verificacion de la Comañia si esta activa o no
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    ModelState.AddModelError("Error", "La Cuenta Caduco o esta Bloqueada !!!");

                    return RedirectToAction("Login", "Account");
                }

                if (current <= hoy)  //Verificacion de la compañia si la Fecha esta Vencida
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    ModelState.AddModelError("Error", "La Cuenta Caduco o esta Bloqueada !!!");

                    return RedirectToAction("Login", "Account");
                }
            }

            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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