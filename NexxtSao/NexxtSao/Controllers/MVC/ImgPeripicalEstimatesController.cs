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
    [Authorize(Roles = "User")]

    public class ImgPeripicalEstimatesController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: ImgPeripicalEstimates
        public ActionResult Index()
        {
            var imgPeripicalEstimates = db.ImgPeripicalEstimates
                .Include(i => i.Client)
                .Include(i => i.Estimate);
            return View(imgPeripicalEstimates.ToList());
        }

        // GET: ImgPeripicalEstimates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPeripicalEstimate = db.ImgPeripicalEstimates.Find(id);
            if (imgPeripicalEstimate == null)
            {
                return HttpNotFound();
            }
            return View(imgPeripicalEstimate);
        }

        // GET: ImgPeripicalEstimates/Create
        public ActionResult Create(int idcliente, int idestimate)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var imgperipicalestimate = new ImgPeripicalEstimate
            {
                CompanyId = user.CompanyId,
                EstimateId = idestimate,
                ClientId = idcliente,
                Date = DateTime.UtcNow
            };

            return View(imgperipicalestimate);
        }

        // POST: ImgPeripicalEstimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ImgPeripicalEstimate imgPeripicalEstimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ImgPeripicalEstimates.Add(imgPeripicalEstimate);
                    db.SaveChanges();

                    if (imgPeripicalEstimate.PhotoFile != null)
                    {
                        var folder = "~/Content/EvolutionPeri";
                        var file = string.Format("{0}.jpg", imgPeripicalEstimate.ImgPeripicalEstimateId);
                        var response = FilesHelper.UploadPhoto(imgPeripicalEstimate.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            imgPeripicalEstimate.Photo = pic;
                            db.Entry(imgPeripicalEstimate).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Details", new { id = imgPeripicalEstimate.ImgPeripicalEstimateId });
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

            return View(imgPeripicalEstimate);
        }

        // GET: ImgPeripicalEstimates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPeripicalEstimate = db.ImgPeripicalEstimates.Find(id);
            if (imgPeripicalEstimate == null)
            {
                return HttpNotFound();
            }

            return View(imgPeripicalEstimate);
        }

        // POST: ImgPeripicalEstimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ImgPeripicalEstimate imgPeripicalEstimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imgPeripicalEstimate.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/EvolutionPeri";
                        var file = string.Format("{0}.jpg", imgPeripicalEstimate.ImgPeripicalEstimateId);
                        var response = FilesHelper.UploadPhoto(imgPeripicalEstimate.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            imgPeripicalEstimate.Photo = pic;
                        }
                    }

                    db.Entry(imgPeripicalEstimate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = imgPeripicalEstimate.ImgPeripicalEstimateId });
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

            return View(imgPeripicalEstimate);
        }

        // GET: ImgPeripicalEstimates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPeripicalEstimate = db.ImgPeripicalEstimates.Find(id);
            if (imgPeripicalEstimate == null)
            {
                return HttpNotFound();
            }
            return View(imgPeripicalEstimate);
        }

        // POST: ImgPeripicalEstimates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var imgPeripicalEstimate = db.ImgPeripicalEstimates.Find(id);
            var foto = string.Empty;
            foto = imgPeripicalEstimate.Photo;
            try
            {
                if (foto != null || string.IsNullOrEmpty(foto))
                {
                    var response = FilesHelper.DeletePhoto(foto);
                    if (response == true)
                    {
                        db.ImgPeripicalEstimates.Remove(imgPeripicalEstimate);
                        db.SaveChanges();
                    }
                    else
                    {
                        var ex = new Exception();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(imgPeripicalEstimate);
                    }
                }
                else
                {
                    db.ImgPeripicalEstimates.Remove(imgPeripicalEstimate);
                    db.SaveChanges();
                }

                return RedirectToAction("Details", "Evolutions", new { id = imgPeripicalEstimate.ClientId });
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
            return View(imgPeripicalEstimate);
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
