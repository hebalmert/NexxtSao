using Newtonsoft.Json.Linq;
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

    public class EvolutionsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: HeadTexts/Create
        public ActionResult ViewDetail(int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var detalles = db.EvolutionDetails.Find(id);

            return PartialView(detalles);
        }

        // GET: HeadTexts/Create
        public ActionResult AddComment(int idEstimate, int idEstimateDetail, string idDiente, double idsaldo, int idcliente, string idTratamiento)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var evolucionclientes = new EvolutionDetailAdd
            {
                CompanyId = user.CompanyId,
                Date = DateTime.UtcNow,
                ClientId = idcliente,
                EstimateId = idEstimate,
                EstimateDetailId = idEstimateDetail,
                Tratamiento = idTratamiento,
                Diente = idDiente,
                Saldo = idsaldo,
                Abono = 0
            };

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");

            return PartialView(evolucionclientes);
        }

        // POST: HeadTexts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(EvolutionDetailAdd evolutionDetailsadd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Agregar el registro en  la Tabla de Evoluciones Detalles
                    var evolucion = new EvolutionDetails
                    { 
                        CompanyId = evolutionDetailsadd.CompanyId,
                        EstimateId = evolutionDetailsadd.EstimateId,
                        EstimateDetailId = evolutionDetailsadd.EstimateDetailId,
                        Diente = evolutionDetailsadd.Diente,
                        Date = evolutionDetailsadd.Date,
                        DentistId = evolutionDetailsadd.DentistId,
                        Detalle = evolutionDetailsadd.Detalle,
                        Abono = evolutionDetailsadd.Abono
                    };
                    db.EvolutionDetails.Add(evolucion);
                    db.SaveChanges();
                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    //Consultamos Saldos del Estimado Detalle para actualizar segun Abono
                    if (evolutionDetailsadd.Abono > 0)
                    {
                        var currentestimado = db.EstimateDetails.Find(evolutionDetailsadd.EstimateDetailId);
                        double cSaldo = currentestimado.Saldo;
                        double cAbono = currentestimado.Abono;
                        double nAbono = evolutionDetailsadd.Abono;
                        double rest = 0;
                        double sum = 0;

                        //Se calcula el nuevo saldo que se guardar como cSaldo
                        rest = cSaldo - nAbono;
                        cSaldo = rest;

                        // Se Calcula el total abonado y se guardara como cAbono = sum
                        sum = cAbono + nAbono;
                        cAbono = sum;

                        currentestimado.Saldo = cSaldo;
                        currentestimado.Abono = cAbono;

                        db.Entry(currentestimado).State = EntityState.Modified;
                        db.SaveChanges();


                        //Se busca el consecutivo Nota Cobro para enviar a la tabla de PaymentGeneral
                        int dato = 0;
                        int nDato = 0;

                        var registro = db.Registers.Where(r => r.CompanyId == evolutionDetailsadd.CompanyId).FirstOrDefault();
                        dato = registro.NotaCobro;
                        nDato = dato + 1;
                        registro.NotaCobro = nDato;
                        db.Entry(registro).State = EntityState.Modified;
                        db.SaveChanges();

                        //Busqueda de Datos Secundarios como el encabezado y la tasa profesional en base a la categoria de tratamiento
                        var encabezado = db.HeadTexts.Where(h => h.CompanyId == evolutionDetailsadd.CompanyId).FirstOrDefault();
                        var nombre = db.Clients.Find(evolutionDetailsadd.ClientId);
                        var categoria = db.EstimateDetails.Where(t => t.EstimateDetailId == evolutionDetailsadd.EstimateDetailId).FirstOrDefault();
                        //var tasapro = db.DentistPercentages.Where(d => d.DentistId == evolutionDetailsadd.DentistId && d.TreatmentCategoryId == categoria.TreatmentCategoryId).FirstOrDefault();
                        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



                        double rateprofesional = evolutionDetailsadd.Porcentaje;
                        double porcentajeDentist = 0;
                        double Pagopro = 0;
                        double TotalOriginal = evolutionDetailsadd.Abono;
                        porcentajeDentist = rateprofesional + 1;
                        Pagopro = (porcentajeDentist * TotalOriginal) - TotalOriginal;

                        var pagogeneral = new PaymentsGeneral
                        {
                            CompanyId = evolutionDetailsadd.CompanyId,
                            Date = evolutionDetailsadd.Date,
                            NotaCobro = Convert.ToString(nDato),
                            DentistId = evolutionDetailsadd.DentistId,
                            TasaProfesional = evolutionDetailsadd.Porcentaje,
                            ClientId = evolutionDetailsadd.ClientId,
                            Cliente = nombre.Cliente,
                            Tratamiento = evolutionDetailsadd.Tratamiento,
                            Tasa = categoria.Tasa,
                            Precio = evolutionDetailsadd.Abono,
                            Cantidad = 1,
                            Total = evolutionDetailsadd.Abono,
                            PagoProfesional = Pagopro,
                            HeadTextId = encabezado.HeadTextId
                        };
                        db.PaymentsGenerals.Add(pagogeneral);
                        db.SaveChanges();
                    }
                    //:::::::::::::::::::::::::::::::::::::::::::


                    return RedirectToAction("Details", new { id = evolutionDetailsadd .ClientId});
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(evolutionDetailsadd.CompanyId), "DentistId", "Odontologo", evolutionDetailsadd.DentistId);
            return PartialView(evolutionDetailsadd);
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
        public ActionResult Details(int? id)  //El Id trae el ID del cliente, hay que buscar en Estimados el presupuesto del cliente
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var estimate = db.Estimates.Where(c=> c.ClientId == id).FirstOrDefault();
            if (estimate == null)
            {
                return RedirectToAction("Details", "Clients", new { id = id});
            }
            return View(estimate);
        }


        public JsonResult GetPorcentaje(int dentistId, int estimatedetailId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var categoria = db.EstimateDetails.Where(t => t.EstimateDetailId == estimatedetailId).FirstOrDefault();
            var tasapro = db.DentistPercentages.Where(d => d.DentistId == dentistId && d.TreatmentCategoryId == categoria.TreatmentCategoryId).FirstOrDefault();
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