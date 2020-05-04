using NexxtSao.Classes;
using NexxtSao.Models;
using NexxtSao.Models.MVC;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NexxtSao.Controllers.MVC
{
    [Authorize(Roles = "User, Dentist")]

    public class DirectPaymentsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Dentists/Details/5
        public ActionResult PrintPayment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pagosgenerales = db.PaymentsGenerals.Find(id);
            if (pagosgenerales == null)
            {
                return HttpNotFound();
            }
            return View(pagosgenerales);
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
                return View(clients.OrderBy(o => o.Cliente).ToList().ToPagedList((int)page, 5));
            }
            else
            {
                var clients = db.Clients.Where(c => c.CompanyId == user.CompanyId  && c.ClientId == 0)
                .Include(c => c.City)
                .Include(c => c.Identification)
                .Include(c => c.Zone);
                return View(clients.OrderBy(o => o.Cliente).ToList().ToPagedList((int)page, 5));
            }
        }


        // GET: DirectPayments/Create
        public ActionResult Create(int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var paydirect = new DirectPayment
            {
                CompanyId = user.CompanyId,
                ClientId = id,
                Date = DateTime.UtcNow
            };

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio");
            ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(user.CompanyId), "TreatmentId", "Servicio");
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(user.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento");

            return View(paydirect);
        }

        // POST: DirectPayments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DirectPayment directPayment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.DirectPayments.Add(directPayment);

                    int Recep = 0;
                    int sum = 0;
                    var db2 = new NexxtSaoContext();
                    var db3 = new NexxtSaoContext();

                    var register = db2.Registers.Where(c => c.CompanyId == directPayment.CompanyId).FirstOrDefault();
                    Recep = register.NotaCobro;
                    sum = Recep + 1;
                    register.NotaCobro = sum;
                    db2.Entry(register).State = EntityState.Modified;
                    db2.SaveChanges();

                    var tratamiento = db3.Treatments.Find(directPayment.TreatmentId);

                    directPayment.NotaCobro = Convert.ToString(sum);
                    directPayment.Tratamiento = tratamiento.Servicio;

                    db3.Dispose();
                    db2.Dispose();

                    db.SaveChanges();

                    // Incrustar nuevo registro en la tabla de DirectGeneral
                    ////////////////////////////////////////////////////////
                    double rateprofesional = directPayment.TasaProfesional;
                    double porcentajeDentist = 0;
                    double tasapro = 0;
                    double TotalOriginal = directPayment.Total;
                    porcentajeDentist = rateprofesional + 1;
                    tasapro = (porcentajeDentist * TotalOriginal) - TotalOriginal;

                    var encabezado = db.HeadTexts.Where(h => h.CompanyId == directPayment.CompanyId).FirstOrDefault();
                    var nombre = db.Clients.Find(directPayment.ClientId);

                    var paymentgeneral = new PaymentsGeneral
                    {
                        CompanyId = directPayment.CompanyId,
                        Date = directPayment.Date,
                        NotaCobro = directPayment.NotaCobro,
                        DentistId = directPayment.DentistId,
                        TasaProfesional = directPayment.TasaProfesional,
                        ClientId = directPayment.ClientId,
                        Cliente = nombre.Cliente,
                        Tratamiento = directPayment.Tratamiento,
                        Tasa = directPayment.Tasa,
                        Precio = directPayment.Precio,
                        Cantidad = directPayment.Cantidad,
                        Total = directPayment.Total,
                        PagoProfesional = tasapro,
                        HeadTextId = encabezado.HeadTextId
                    };
                    db.PaymentsGenerals.Add(paymentgeneral);
                    db.SaveChanges();
                    db.Dispose();
                    /////////////////////////////////////////////////////////
                    ///
                    return RedirectToAction("PrintPayment", new { id = paymentgeneral.PaymentsGeneralId});
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_DoubleData);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(directPayment.CompanyId), "DentistId", "Odontologo", directPayment.DentistId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio", directPayment.LevelPriceId);
            ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(directPayment.CompanyId), "TreatmentId", "Servicio", directPayment.TreatmentId);
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(directPayment.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", directPayment.TreatmentCategoryId);

            return View(directPayment);
        }


        public JsonResult GetTratamiento(int CategoryId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var tratamiento = db.Treatments.Where(c => c.TreatmentCategoryId == CategoryId);
            return Json(tratamiento);
        }

        public JsonResult GetDentist(int dentistId, int categoryId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            double tasa = 0;
            var porcentaje = db.DentistPercentages.Where(c=> c.DentistId == dentistId && c.TreatmentCategoryId == categoryId).FirstOrDefault();
            if (porcentaje == null)
            {
                tasa = 0;
            }
            else 
            {
                tasa = porcentaje.Porcentaje;
            }
           
            return Json(tasa);
        }

        public JsonResult GetPrecio(int tratamientoId, int LevelPriceId)
        {
            decimal precio = 0;

            db.Configuration.ProxyCreationEnabled = false;
            if (tratamientoId == 0)
            {
                return Json(precio);
            }
            var tratamiento = db.Treatments.Find(tratamientoId);
            int Lprice = LevelPriceId;

            if (Lprice == 1)
            {
                precio = tratamiento.Precio1;
            }
            if (Lprice == 2)
            {
                precio = tratamiento.Precio2;
            }
            if (Lprice == 3)
            {
                precio = tratamiento.Precio3;
            }

            return Json(precio);
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