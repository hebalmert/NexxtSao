﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using NexxtSao.Classes;
using NexxtSao.Models;
using NexxtSao.Models.MVC;
namespace NexxtSao.Controllers.MVC
{
    [Authorize(Roles = "User, Dentist")]
    public class EstimatesController : Controller
    {

        private NexxtSaoContext db = new NexxtSaoContext();
        // Post: DeletePlan/Borrar
        public ActionResult DeleteEstimateDetail(int id)
        {
            var estimate = id;
            try
            {
                var current = db.EstimateDetails.Find(id);
                var currenTotal = current.Total;
                var currenAbono = current.Abono;
                var currenSaldo = current.Saldo;
                if (current.Abono > 0)
                {
                    ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_NoDoleteItem));
                    return View("Detail", new { id = estimate});
                }
                var estimadoPrincipal = db.Estimates.Find(current.EstimateId);
                var VTotal = estimadoPrincipal.Total;
                double sum = 0;
                sum = VTotal - currenTotal;
                estimadoPrincipal.Total = sum;
                estimadoPrincipal.SubTotal = sum;
                db.Entry(estimadoPrincipal).State = EntityState.Modified;
                db.SaveChanges();
                db.EstimateDetails.Remove(current);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = current.EstimateId });
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
            return RedirectToAction("Details", new { id = estimate});
        }


        // GET: Estimates/Edit/5
        public ActionResult EditEstimate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var estimatedetall = db.EstimateDetails.Where(c => c.EstimateDetailId == id)
                .Include(e => e.Treatment)
                .Include(e => e.TreatmentCategory).FirstOrDefault();
            if (estimatedetall == null)
            {
                return HttpNotFound();
            }
            var estimatedetailadd = new EstimateDetailAdd
            {
                CompanyId = estimatedetall.CompanyId,
                EstimateId = estimatedetall.EstimateId,
                EstimateDetailId = estimatedetall.EstimateDetailId,
                TreatmentCategoryId = estimatedetall.TreatmentCategoryId,
                Categoria = estimatedetall.TreatmentCategory.CategoriaTratamiento,
                tratamiento = estimatedetall.Treatment.Servicio,
                TreatmentId = estimatedetall.TreatmentId,
                Diente = estimatedetall.Diente,
                Tasa = estimatedetall.Tasa,
                Unitario = estimatedetall.Unitario,
                Cantidad = estimatedetall.Cantidad,
                Total = estimatedetall.Total
            };
            //ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(estimatedetall.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", estimatedetall.TreatmentCategoryId);
            //ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(estimatedetall.CompanyId), "TreatmentId", "Servicio", estimatedetall.TreatmentId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio", estimatedetall.LevelPriceId);
            return PartialView(estimatedetailadd);
        }


        // POST: Estimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEstimate(EstimateDetailAdd estimatedetailadd)
        {
            estimatedetailadd.LevelPriceId = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    var currenDetails = db.EstimateDetails.Find(estimatedetailadd.EstimateDetailId);
                    var currenTotal = currenDetails.Total;
                    var currenCantidad = currenDetails.Cantidad;
                    var currenAbono = currenDetails.Abono;
                    var currenSaldo = currenDetails.Saldo;
                    var currentdiente = estimatedetailadd.Diente;
                    if (estimatedetailadd.Total < currenDetails.Abono)
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_NoTotalMenorAbono));
                        return View(estimatedetailadd);
                    };
                    var Ntotal = estimatedetailadd.Total;
                    var Nsaldo = Ntotal - currenAbono;
                    currenDetails.Diente = currentdiente;
                    currenDetails.Cantidad = estimatedetailadd.Cantidad;
                    currenDetails.Total = estimatedetailadd.Total;
                    currenDetails.Abono = currenAbono;
                    currenDetails.Saldo = Nsaldo;
                    db.Entry(currenDetails).State = EntityState.Modified;
                    db.SaveChanges();
                    var estimateUpdate = db.Estimates.Find(estimatedetailadd.EstimateId);
                    estimateUpdate.SubTotal = estimatedetailadd.Total;
                    estimateUpdate.Iva = 0;
                    estimateUpdate.Total = estimatedetailadd.Total;
                    db.Entry(estimateUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = estimatedetailadd.EstimateId });
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
            return PartialView(estimatedetailadd);
        }


        // GET: Estimates/Create
        public ActionResult AddEstimate(int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var estimadoadd = new EstimateDetailAdd
            {
                CompanyId = user.CompanyId,
                EstimateId = id,
                Tasa = 0,
                Unitario = 0,
                Cantidad = 0,
                Total = 0
            };
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(user.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento");
            ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(user.CompanyId), "TreatmentId", "Servicio");
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio");
            return PartialView(estimadoadd);
        }


        // POST: Estimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEstimate(EstimateDetailAdd estimatedetailadd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tratamiento = db.Treatments.Find(estimatedetailadd.TreatmentId);
                    var estimadodetail = new EstimateDetail
                    {
                        CompanyId = tratamiento.CompanyId,
                        EstimateId = estimatedetailadd.EstimateId,
                        TreatmentCategoryId = estimatedetailadd.TreatmentCategoryId,
                        TreatmentId = estimatedetailadd.TreatmentId,
                        Tratamiento = tratamiento.Servicio,
                        Diente = estimatedetailadd.Diente,
                        LevelPriceId = estimatedetailadd.LevelPriceId,
                        Tasa = estimatedetailadd.Tasa,
                        Unitario = estimatedetailadd.Unitario,
                        Cantidad = estimatedetailadd.Cantidad,
                        Total = estimatedetailadd.Total,
                        Abono = 0,
                        Saldo = estimatedetailadd.Total
                    };
                    db.EstimateDetails.Add(estimadodetail);
                    db.SaveChanges();
                    var currentotal = db.Estimates.Find(estimatedetailadd.EstimateId);
                    double t1 = currentotal.Total;
                    double t2 = estimatedetailadd.Total;
                    double sum = 0;
                    sum = t1 + t2;
                    currentotal.SubTotal = sum;
                    currentotal.Total = sum;
                    db.Entry(currentotal).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = estimatedetailadd.EstimateId });
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
            ViewBag.TreatmentCategoryId = new SelectList(ComboHelper.GetTreatmentCategory(estimatedetailadd.CompanyId), "TreatmentCategoryId", "CategoriaTratamiento", estimatedetailadd.TreatmentCategoryId);
            ViewBag.TreatmentId = new SelectList(ComboHelper.GetTreatmen(estimatedetailadd.CompanyId), "TreatmentId", "Servicio", estimatedetailadd.TreatmentId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetPrice(), "LevelPriceId", "NivelPrecio", estimatedetailadd.LevelPriceId);
            return PartialView(estimatedetailadd);
        }


        // GET: Estimates
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var estimates = db.Estimates.Where(c => c.CompanyId == user.CompanyId)
                .Include(e => e.Client);
            return View(estimates.ToList());
        }


        // GET: Estimates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }
            return View(estimate);
        }


        // GET: Estimates/Create
        public ActionResult Create(int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var estimado = new Estimate
            {
                CompanyId = user.CompanyId,
                Date = DateTime.UtcNow,
                ClientId = id,
                SubTotal = 0,
                Iva = 0,
                Total = 0
            };
            return View(estimado);
        }


        // POST: Estimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estimate estimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var encabezado = db.HeadTexts.Where(e => e.CompanyId == estimate.CompanyId).FirstOrDefault();
                    var register = db.Registers.Where(c => c.CompanyId == estimate.CompanyId).FirstOrDefault();
                    int estimado = register.Estimate;
                    int sum = estimado + 1;
                    register.Estimate = sum;
                    db.Entry(register).State = EntityState.Modified;
                    db.SaveChanges();
                    estimate.Estimado = sum;
                    estimate.HeadTextId = encabezado.HeadTextId;
                    db.Estimates.Add(estimate);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = estimate.EstimateId });
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
            return View(estimate);
        }


        // GET: Estimates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }
            return View(estimate);
        }


        // POST: Estimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Estimate estimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(estimate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = estimate.EstimateId });
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
            return View(estimate);
        }


        // GET: Estimates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }
            return View(estimate);
        }


        // POST: Estimates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var estimate = db.Estimates.Find(id);
            db.Estimates.Remove(estimate);
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
            return View(estimate);
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
            return Json(precio);
        }


        // GET: Users/Details/id/Odontodiagrama/Create
        public ActionResult CreateOdontodiagrama(int id, int idestimate)
        {
            var cliente = db.Clients.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            var viewDiagrama = new Odontodiagrama
            {
                ClientId = cliente.ClientId,
                EstimateId = idestimate
            };
            return View(viewDiagrama);
        }


        // POST: Users/Odontodiagrama/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CreateOdontodiagrama(int id, String data)
        {
            var db1 = new NexxtSaoContext();
            var Iestimate = db.Estimates.Where(c => c.ClientId == id).FirstOrDefault();
            int userId = id;
            int odontoDiagramaId = 0;
            string description = "";
            // Crear Odontodiagrama
            var odontoDiagrama = new Odontodiagrama
            {
                ClientId = userId,
                EstimateId = Iestimate.EstimateId,
                DateEntry = DateTime.Today
            };
            db1.Odontodiagramas.Add(odontoDiagrama);
            try
            {
                db1.SaveChanges();
                // Guardar el ID
                odontoDiagramaId = odontoDiagrama.OdontodiagramaId;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index")
                     )
                {
                    ModelState.AddModelError(string.Empty, "Existe un registro con el mismo valor.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                return View();
            }
            // Parsear Data para poderla recorrer como un JSON
            JArray jsonPreservar = JArray.Parse(data);
            foreach (JObject jsonOperaciones in jsonPreservar.Children<JObject>())
            {
                double tooth = 0;
                string color = "";
                //Aqui para poder identificar las propiedades y sus valores
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    string propiedad = jsonOPropiedades.Name;
                    if (propiedad.Equals("id"))
                    {
                        tooth = Convert.ToDouble(jsonOPropiedades.Value);
                    }
                    if (propiedad.Equals("color"))
                    {
                        color = Convert.ToString(jsonOPropiedades.Value);
                    }
                    if (propiedad.Equals("description"))
                    {
                        description = Convert.ToString(jsonOPropiedades.Value);
                    }
                }
                // Verificar que los valores no sean nulos para guardar registro en modelo Diagrama
                if (odontoDiagramaId != 0 && tooth != 0 && color != "")
                {
                    var diagrama = new Diagram
                    {
                        OdontodiagramaId = odontoDiagramaId,
                        ToothNumber = tooth,
                        Color = color,
                        Active = true
                    };
                    db.Diagrams.Add(diagrama);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null &&
                                ex.InnerException.InnerException != null &&
                                ex.InnerException.InnerException.Message.Contains("_Index")
                             )
                        {
                            ModelState.AddModelError(string.Empty, "Existe un registro con el mismo valor.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.Message);
                        }
                        return View();
                    }
                }
            }
            if (description != "")
            {
                odontoDiagrama.Description = description;
                db1.Entry(odontoDiagrama).State = EntityState.Modified;
                try
                {
                    db1.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                            ex.InnerException.InnerException != null &&
                            ex.InnerException.InnerException.Message.Contains("_Index")
                         )
                    {
                        ModelState.AddModelError(string.Empty, "Existe un registro con el mismo valor.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    return View();
                }
            }
            return View();
        }


        // GET: Load Odontodigrama data and Diagram view (empty)
        public ActionResult OdontodoView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var odontodiagrama = db.Odontodiagramas.Find(id);
            if (odontodiagrama == null)
            {
                return HttpNotFound();
            }
            return View(odontodiagrama);
        }


        // GET: Send Json Data to fill diagrams
        public ActionResult TheethJson(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var odontodiagrama = db.Odontodiagramas.Find(id);
            if (odontodiagrama == null)
            {
                return HttpNotFound();
            }
            var diagramas = db.Diagrams
                .Where(d => d.OdontodiagramaId == id)
                .Select(w => new JsonType()
                {
                    ToothNumber = w.ToothNumber,
                    Color = w.Color
                }).ToList<JsonType>(); ;
            return Json(diagramas,
                JsonRequestBehavior.AllowGet);
        }


        public class JsonType
        {
            public double ToothNumber { get; set; }
            public string Color { get; set; }
            public JsonType() { }
        }


        // POST: Borra Ondontodiagrama y los diagramas a los que pertenezca
        [HttpPost]
        public ActionResult OdontodoView(int id)
        {
            var Idiagram = db.Diagrams.Where(c => c.OdontodiagramaId == id).ToList();
            if (Idiagram.Count != 0)
            {
                foreach (var item in Idiagram)
                {
                    db.Diagrams.Remove(item);
                }                
                db.SaveChanges();
            }

            var odontodiagrama = db.Odontodiagramas.Find(id);
            db.Odontodiagramas.Remove(odontodiagrama);
            db.SaveChanges();

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