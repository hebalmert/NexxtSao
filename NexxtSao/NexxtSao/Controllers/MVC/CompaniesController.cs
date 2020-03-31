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
    [Authorize(Roles = "Admin")]

    public class CompaniesController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: Companies
        public ActionResult Index()
        {
            var companies = db.Companies.Include(c => c.Country);
            return View(companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            var companias = new Company
            {
                DateDesde = DateTime.Today,
                DateHasta = DateTime.Today,
                Activo = true
            };
            ViewBag.CountryId = new SelectList(ComboHelper.GetCountries(), "CountryId", "Pais");
            return View(companias);
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {              
                try
                {
                    db.Companies.Add(company);
                    db.SaveChanges();

                    if (company.LogoFile != null)
                    {
                        var folder = "~/Content/Logos";
                        var file = string.Format("{0}.jpg", company.CompanyId);
                        var response = FilesHelper.UploadPhoto(company.LogoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            company.Logo = pic;
                            db.Entry(company).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    // Se crea el Registro de Control de Consecutivos de la Compania
                    var registro = new Register
                    {
                        CompanyId = company.CompanyId,
                        Estimate = 0,
                        History = 0,
                        NotaCobro = 0
                    };
                    db.Registers.Add(registro);
                    db.SaveChanges();
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    // Se crea el HeadText de Control de Consecutivos de la Compania
                    var headtext = new HeadText
                    {
                        CompanyId = company.CompanyId,
                        TextoEncabezado = @Resources.Resource.Msg_Compny_CreateHeadtext
                    };

                    db.HeadTexts.Add(headtext);
                    db.SaveChanges();
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    // Se crea el SupportText de Control de Consecutivos de la Compania
                    var identification = new Identification
                    {
                        CompanyId = company.CompanyId,
                        TipoDocumento = "Nit/CI"
                    };

                    db.Identifications.Add(identification);
                    db.SaveChanges();
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    // Se crea el SupportText de Control de Consecutivos de la Compania
                    var tax = new Tax
                    {
                        CompanyId = company.CompanyId,
                        Impuesto = "IVA 0 %",
                        Rate = 0
                    };

                    db.Taxes.Add(tax);
                    db.SaveChanges();
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


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

            ViewBag.CountryId = new SelectList(ComboHelper.GetCountries(), "CountryId", "Pais", company.CountryId);
            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }

            ViewBag.CountryId = new SelectList(ComboHelper.GetCountries(), "CountryId", "Pais", company.CountryId);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {                
                try
                {
                    if (company.LogoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/Logos";
                        var file = string.Format("{0}.jpg", company.CompanyId);
                        var response = FilesHelper.UploadPhoto(company.LogoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            company.Logo = pic;
                        }
                    }

                    db.Entry(company).State = EntityState.Modified;
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

            ViewBag.CountryId = new SelectList(ComboHelper.GetCountries(), "CountryId", "Pais", company.CountryId);
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var company = db.Companies.Find(id);
            db.Companies.Remove(company);
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
            return View(company);
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
