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

    public class OrthodonticsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Estimates/Create
        public ActionResult DeleteOrthodon(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ortodon = db.Orthodontics.Find(id);
            db.Orthodontics.Remove(ortodon);
            db.SaveChanges();

            if (ortodon == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Details", new { id = ortodon.ClientId });
        }

        // GET: Estimates/Create
        public ActionResult CloseOrthodontic(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ortodon = db.Orthodontics.Find(id);
            if (ortodon == null)
            {
                return HttpNotFound();
            }
            ortodon.Cerrado = true;
            db.Entry(ortodon).State = EntityState.Modified;
            db.SaveChanges();

            //buscamos la cantidad de Item que hay en OrthoDetail
            var itemOrthoDetail = db.OrthodonticDetails.Where(c => c.OrthodonticId == id)
                .Include(i=> i.Orthodontic)
                .ToList();               
            
            if (itemOrthoDetail == null)
            {
                return RedirectToAction("Visit", new { id = ortodon.OrthodonticId });
            }
            else
            {
                foreach (var item in itemOrthoDetail)
                {
                    //Se busca el consecutivo Nota Cobro para enviar a la tabla de PaymentGeneral
                    int dato = 0;
                    int nDato = 0;

                    var registro = db.Registers.Where(r => r.CompanyId == item.CompanyId).FirstOrDefault();
                    dato = registro.NotaCobro;
                    nDato = dato + 1;
                    registro.NotaCobro = nDato;
                    db.Entry(registro).State = EntityState.Modified;
                    db.SaveChanges();

                    //Busqueda de Datos Secundarios como el encabezado y la tasa profesional en base a la categoria de tratamiento
                    //var ortodonPrin = db.Orthodontics.Find(orthodon);
                    var encabezado = db.HeadTexts.Where(h => h.CompanyId == item.CompanyId).FirstOrDefault();
                    var nombre = db.Clients.Find(item.Orthodontic.ClientId);
                    var categoria = db.OrthodonticDetails.Where(t => t.OrthodonticDetailId == item.OrthodonticDetailId).FirstOrDefault();
                    var tasapro = db.DentistPercentages.Where(d => d.DentistId == item.Orthodontic.DentistId && d.TreatmentCategoryId == item.TreatmentCategoryId).FirstOrDefault();
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    double rateprofesional = 0;
                    if (tasapro != null)
                    {
                        rateprofesional = tasapro.Porcentaje;
                    }
                    else
                    {
                        rateprofesional = 0;
                    }

                    double porcentajeDentist = 0;
                    double Pagopro = 0;
                    double TotalOriginal = item.Total;
                    porcentajeDentist = rateprofesional + 1;
                    Pagopro = (porcentajeDentist * TotalOriginal) - TotalOriginal;

                    var pagogeneral = new PaymentsGeneral
                    {
                        CompanyId = item.CompanyId,
                        Date = item.Orthodontic.Date,
                        NotaCobro = Convert.ToString(nDato),
                        DentistId = item.Orthodontic.DentistId,
                        TasaProfesional = rateprofesional,
                        ClientId = item.Orthodontic.ClientId,
                        Cliente = nombre.Cliente,
                        Tratamiento = item.Tratamiento,
                        Tasa = categoria.Tasa,
                        Precio = item.Total,
                        Cantidad = 1,
                        Total = item.Total,
                        PagoProfesional = Pagopro,
                        HeadTextId = encabezado.HeadTextId
                    };
                    db.PaymentsGenerals.Add(pagogeneral);
                    db.SaveChanges();
                }

            }

            return RedirectToAction("Visit", new { id = ortodon.OrthodonticId });
        }


        // GET: Estimates/Create
        public ActionResult Visit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ortodon = db.Orthodontics.Find(id);
            if (ortodon == null)
            {
                return HttpNotFound();
            }
            return View(ortodon);
        }

        // Post: DeletePlan/Borrar
        public ActionResult DeleteOrthodonDetail(int id)
        {
            var orthodon = id;

            try
            {

                var current = db.OrthodonticDetails.Find(id);
                var currenTotal = current.Total;
                var currenAbono = current.Abono;
                var currenSaldo = current.Saldo;

                if (current.Abono > 0)
                {
                    ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_NoDoleteItem));
                    return View("Visit", new { id = orthodon });
                }

                var orthodonPrincipal = db.Orthodontics.Find(current.OrthodonticId);
                var VTotal = orthodonPrincipal.Total;
                double sum = 0;
                sum = VTotal - currenTotal;

                orthodonPrincipal.Total = sum;
                orthodonPrincipal.SubTotal = sum;
                db.Entry(orthodonPrincipal).State = EntityState.Modified;
                db.SaveChanges();


                db.OrthodonticDetails.Remove(current);
                db.SaveChanges();

                return RedirectToAction("Visit", new { id = current.OrthodonticId });
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
            return RedirectToAction("Visit", new { id = orthodon });
        }

        // GET: Estimates/Edit/5
        public ActionResult EditOrthodontic(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orthodetalle = db.OrthodonticDetails.Where(c => c.OrthodonticDetailId == id)
                .Include(e => e.Treatment)
                .Include(e => e.TreatmentCategory).FirstOrDefault();

            if (orthodetalle == null)
            {
                return HttpNotFound();
            }

            var orthodondetalle = new OrthodonticDeatilAdd
            {
                CompanyId = orthodetalle.CompanyId,
                OrthodonticId = orthodetalle.OrthodonticId,
                OrthodonticDetailId = orthodetalle.OrthodonticDetailId,
                Diente = orthodetalle.Diente,
                TreatmentCategoryId = orthodetalle.TreatmentCategoryId,
                Categoria = orthodetalle.TreatmentCategory.CategoriaTratamiento,
                LevelPriceId = orthodetalle.LevelPriceId,
                tratamiento = orthodetalle.Treatment.Servicio,
                TreatmentId = orthodetalle.TreatmentId,
                Tasa = orthodetalle.Tasa,
                Unitario = orthodetalle.Unitario,
                Cantidad = orthodetalle.Cantidad,
                Total = orthodetalle.Total
            };

            //ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(estimatedetall.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", estimatedetall.TreatmentCategoryId);
            //ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(estimatedetall.CompanyId), "TreatmentId", "Servicio", estimatedetall.TreatmentId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio", orthodetalle.LevelPriceId);

            return PartialView(orthodondetalle);
        }

        // POST: Estimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrthodontic(OrthodonticDeatilAdd orthodetailadd)
        {
            //orthodetailadd.LevelPriceId = 1;

            if (ModelState.IsValid)
            {
                try
                {
                    var currenDetails = db.OrthodonticDetails.Find(orthodetailadd.OrthodonticDetailId);
                    var currenTotal = currenDetails.Total;
                    var currenCantidad = currenDetails.Cantidad;
                    var currenAbono = currenDetails.Abono;
                    var currenSaldo = currenDetails.Saldo;

                    if (orthodetailadd.Total < currenDetails.Abono)
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_NoTotalMenorAbono));
                        return View(orthodetailadd);
                    };

                    var Ntotal = orthodetailadd.Total;
                    var Nsaldo = Ntotal - currenAbono;

                    currenDetails.Cantidad = orthodetailadd.Cantidad;
                    currenDetails.Total = orthodetailadd.Total;
                    currenDetails.Abono = currenAbono;
                    currenDetails.Saldo = Nsaldo;

                    db.Entry(currenDetails).State = EntityState.Modified;
                    db.SaveChanges();

                    var orthodonUpdate = db.Orthodontics.Find(orthodetailadd.OrthodonticId);
                    double rest = 0;
                    rest = orthodonUpdate.Total - currenTotal;
                    rest +=  Ntotal;
                    orthodonUpdate.SubTotal = rest;
                    orthodonUpdate.Iva = 0;
                    orthodonUpdate.Total = rest;
                    db.Entry(orthodonUpdate).State = EntityState.Modified;
                    db.SaveChanges();


                    return RedirectToAction("Visit", new { id = orthodetailadd.OrthodonticId });
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

            return PartialView(orthodetailadd);
        }


        // GET: Estimates/Create
        public ActionResult AddOrthodontic(int id, int de)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var ortodonciaadd = new OrthodonticDeatilAdd
            {
                CompanyId = user.CompanyId,
                OrthodonticId = id,
                Tasa = 0,
                Unitario = 0,
                Cantidad = 0,
                Total = 0
            };

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo", de);
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(user.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento");
            ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(user.CompanyId), "TreatmentId", "Servicio");
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio");

            return PartialView(ortodonciaadd);
        }

        // POST: Estimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrthodontic(OrthodonticDeatilAdd orthodonticDetailadd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tratamiento = db.Treatments.Find(orthodonticDetailadd.TreatmentId);

                    var ortodonciadetail = new OrthodonticDetail
                    {
                        CompanyId = tratamiento.CompanyId,
                        OrthodonticId = orthodonticDetailadd.OrthodonticId,
                        TreatmentCategoryId = orthodonticDetailadd.TreatmentCategoryId,
                        TreatmentId = orthodonticDetailadd.TreatmentId,
                        Tratamiento = tratamiento.Servicio,
                        Diente = orthodonticDetailadd.Diente,
                        LevelPriceId = orthodonticDetailadd.LevelPriceId,
                        Tasa = orthodonticDetailadd.Tasa,
                        Unitario = orthodonticDetailadd.Unitario,
                        Cantidad = orthodonticDetailadd.Cantidad,
                        Total = orthodonticDetailadd.Total,
                        Abono = 0,
                        Saldo = orthodonticDetailadd.Total
                    };

                    db.OrthodonticDetails.Add(ortodonciadetail);
                    db.SaveChanges();

                    var currentotal = db.Orthodontics.Find(orthodonticDetailadd.OrthodonticId);
                    double t1 = currentotal.Total;
                    double t2 = orthodonticDetailadd.Total;
                    double sum = 0;
                    sum = t1 + t2;
                    currentotal.SubTotal = sum;
                    currentotal.Total = sum;

                    db.Entry(currentotal).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Visit", new { id = orthodonticDetailadd.OrthodonticId });
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(orthodonticDetailadd.CompanyId), "DentistId", "Odontologo", orthodonticDetailadd.DentistId);
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(orthodonticDetailadd.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", orthodonticDetailadd.TreatmentCategoryId);
            ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(orthodonticDetailadd.CompanyId), "TreatmentId", "Servicio", orthodonticDetailadd.TreatmentId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio", orthodonticDetailadd.LevelPriceId);

            return PartialView(orthodonticDetailadd);
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
                var clients = db.Clients.Where(c => c.CompanyId == user.CompanyId && c.ClientId == 0)
                .Include(c => c.City)
                .Include(c => c.Identification)
                .Include(c => c.Zone);
                return View(clients.OrderBy(o => o.Cliente).ToList().ToPagedList((int)page, 10));
            }
        }


        // GET: Estimates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cliente = db.Clients.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Estimates/Create
        public ActionResult Create(int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var ConsulOrtodoncia = new Orthodontic
            {
                CompanyId = user.CompanyId,
                Date = DateTime.UtcNow,
                ClientId = id,
                SubTotal = 0,
                Iva = 0,
                Total = 0
            };

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");

            return View(ConsulOrtodoncia);
        }

        // POST: Estimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Orthodontic ortodontic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var encabezado = db.HeadTexts.Where(e => e.CompanyId == ortodontic.CompanyId).FirstOrDefault();

                    var register = db.Registers.Where(c => c.CompanyId == ortodontic.CompanyId).FirstOrDefault();
                    int ortodon = register.Ortodoncia;
                    int sum = ortodon + 1;
                    register.Ortodoncia = sum;
                    db.Entry(register).State = EntityState.Modified;
                    db.SaveChanges();

                    ortodontic.Ortodoncia = sum;
                    ortodontic.HeadTextId = encabezado.HeadTextId;

                    db.Orthodontics.Add(ortodontic);
                    db.SaveChanges();
                    return RedirectToAction("Visit", new { id = ortodontic.OrthodonticId });
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(ortodontic.CompanyId), "DentistId", "Odontologo", ortodontic.DentistId);

            return View(ortodontic);
        }

        // GET: Estimates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ortodon = db.Orthodontics.Find(id);
            if (ortodon == null)
            {
                return HttpNotFound();
            }

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(ortodon.CompanyId), "DentistId", "Odontologo", ortodon.DentistId);

            return PartialView(ortodon);
        }

        // POST: Estimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Orthodontic ortodon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(ortodon).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Visit", new { id = ortodon.OrthodonticId });
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(ortodon.CompanyId), "DentistId", "Odontologo", ortodon.DentistId);

            return PartialView(ortodon);
        }

        public JsonResult GetTreatment(int treatmentcategoryid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var tratamiento = db.Treatments.Where(c => c.TreatmentCategoryId == treatmentcategoryid);
            return Json(tratamiento);
        }

        public JsonResult GetPrecio(int treatmentid, int levelpriceid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var receptions = db.Treatments.Find(treatmentid);
            int Lprice = levelpriceid;
            decimal precio = 0;
            if (receptions != null)
            {
                if (Lprice == 1)
                {
                    precio = receptions.Precio1;
                }
                if (Lprice == 2)
                {
                    precio = receptions.Precio2;
                }
                if (Lprice == 3)
                {
                    precio = receptions.Precio3;
                }
            }
            else
            {
                precio = 0;
            }
            return Json(precio);
        }

        public JsonResult GetPorcentaje(int dentistId, int categoryId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var tasapro = db.DentistPercentages.Where(d => d.DentistId == dentistId && d.TreatmentCategoryId == categoryId).FirstOrDefault();
            if (tasapro != null)
            {
                double rate = tasapro.Porcentaje;
                return Json(rate);
            }
            else
            {
                double rate = 0;
                return Json(rate);
            }
        }

        // GET: Orthodontic
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