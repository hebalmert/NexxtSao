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
    public class PayDentistsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: PayProfessionals/Edit/5
        public ActionResult Adds(int idPay, int idProfe, int idCompany, string NPago) //el Id es PayProfessionalId
        {
            var directgenerallist = db.PaymentsGenerals.Where(c => c.CompanyId == idCompany && c.DentistId == idProfe && c.Facturado == false).ToList();
            if (directgenerallist.Count != 0)
            {
                var db2 = new NexxtSaoContext();
                var db3 = new NexxtSaoContext();
                var db4 = new NexxtSaoContext();
                double sum = 0;         //To save all the payprofessional and can save it in PayProfessional
                string sumTickets = null;
                foreach (var item in directgenerallist)
                {

                    var NewDetail = new PayDentistDetail
                    {
                        PayDentistId = idPay,
                        CompanyId = idCompany,
                        PaymentsGeneralId = item.PaymentsGeneralId,
                        Date = item.Date,
                        NotaCobro = item.NotaCobro,
                        PagoProfesional = item.PagoProfesional
                    };

                    sum += item.PagoProfesional;
                    if (sumTickets == null)
                    {
                        sumTickets = item.NotaCobro;
                    }
                    else
                    {
                        sumTickets = sumTickets + " - " + item.NotaCobro;
                    }
                    db2.PayDentistDetails.Add(NewDetail);

                    var DirectGeneralUpdate = db3.PaymentsGenerals.Find(item.PaymentsGeneralId);
                    DirectGeneralUpdate.Facturado = true;
                    DirectGeneralUpdate.FacturadoDate = DateTime.Today;
                    DirectGeneralUpdate.ComprobantePago = NPago;
                }
                var payprofessionalUpdate = db4.PayDentists.Find(idPay);
                payprofessionalUpdate.PagoProfesional = sum;
                payprofessionalUpdate.Detalle = sumTickets;
                db4.Entry(payprofessionalUpdate).State = EntityState.Modified;
                db4.SaveChanges();

                db2.SaveChanges();
                db3.SaveChanges();
                db2.Dispose();
                db3.Dispose();
                db4.Dispose();
            }

            return RedirectToAction("Details", new { id = idPay });
        }

        // GET: PayDentists
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var payDentists = db.PayDentists.Where(c => c.CompanyId == user.CompanyId)
                .Include(p => p.Dentist);

            return View(payDentists.OrderByDescending(o=> o.Dentist.Odontologo).ToList());
        }

        // GET: PayDentists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payDentist = db.PayDentists.Find(id);
            if (payDentist == null)
            {
                return HttpNotFound();
            }
            return View(payDentist);
        }

        // GET: PayDentists/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var paydentis = new PayDentist
            {
                CompanyId = user.CompanyId,
                Date = DateTime.Today
            };

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(user.CompanyId), "DentistId", "Odontologo");
            return View(paydentis);
        }

        // POST: PayDentists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayDentist payDentist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Chech for register in the table DirectGeneral
                    var proregister = db.PaymentsGenerals.Where(p => p.DentistId == payDentist.DentistId && p.Facturado == false).ToList();
                    if (proregister.Count == 0)
                    {
                        ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_NoRegisterToContinue);
                        ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(payDentist.CompanyId), "DentistId", "Odontologo", payDentist.DentistId);
                        return View(payDentist);
                    }
                    // End the Chekout register in Table DirectGeneral

                    //Create a New Register consecutive to Payment Note
                    var db3 = new NexxtSaoContext();
                    int Recep = 0;
                    int sum = 0;
                    var register = db3.Registers.Where(c => c.CompanyId == payDentist.CompanyId).FirstOrDefault();
                    Recep = register.NotaPago;
                    sum = Recep + 1;
                    register.NotaPago = sum;
                    db3.Entry(register).State = EntityState.Modified;
                    db3.SaveChanges();
                    db3.Dispose();
                    //Close the new register

                    payDentist.NotaPago = Convert.ToString(sum);
                    db.PayDentists.Add(payDentist);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = payDentist.PayDentistId });
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(payDentist.CompanyId), "DentistId", "Odontologo", payDentist.DentistId);
            
            return View(payDentist);
        }

        // GET: PayDentists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payDentist = db.PayDentists.Find(id);
            if (payDentist == null)
            {
                return HttpNotFound();
            }

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(payDentist.CompanyId), "DentistId", "Odontologo", payDentist.DentistId);
            return View(payDentist);
        }

        // POST: PayDentists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayDentist payDentist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(payDentist).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
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

            ViewBag.DentistId = new SelectList(ComboHelper.GetDentist(payDentist.CompanyId), "DentistId", "Odontologo", payDentist.DentistId);
            
            return View(payDentist);
        }

        // GET: PayDentists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payDentist = db.PayDentists.Find(id);
            if (payDentist == null)
            {
                return HttpNotFound();
            }
            return View(payDentist);
        }

        // POST: PayDentists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var payDentist = db.PayDentists.Find(id);
            db.PayDentists.Remove(payDentist);
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
                    ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_Relationship);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(payDentist);
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
