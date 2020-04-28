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

    public class ImgFotoEstimatesController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: ImgFotoEstimates
        public ActionResult Index()
        {
            var imgFotoEstimates = db.ImgFotoEstimates
                .Include(i => i.Client)
                .Include(i => i.Estimate);
            return View(imgFotoEstimates.ToList());
        }

        // GET: ImgFotoEstimates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgFotoEstimate = db.ImgFotoEstimates.Find(id);
            if (imgFotoEstimate == null)
            {
                return HttpNotFound();
            }
            return View(imgFotoEstimate);
        }

        // GET: ImgFotoEstimates/Create
        public ActionResult Create(int idcliente, int idestimate)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var imgFotoestimate = new ImgFotoEstimate
            {
                CompanyId = user.CompanyId,
                EstimateId = idestimate,
                ClientId = idcliente,
                Date = DateTime.UtcNow
            };

            return View(imgFotoestimate);
        }

        // POST: ImgFotoEstimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ImgFotoEstimate imgFotoEstimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ImgFotoEstimates.Add(imgFotoEstimate);
                    db.SaveChanges();

                    if (imgFotoEstimate.PhotoFile != null)
                    {
                        var folder = "~/Content/EvolutionFoto";
                        var file = string.Format("{0}.jpg", imgFotoEstimate.ImgFotoEstimateId);
                        var response = FilesHelper.UploadPhoto(imgFotoEstimate.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            imgFotoEstimate.Photo = pic;
                            db.Entry(imgFotoEstimate).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Details", new { id = imgFotoEstimate.ImgFotoEstimateId });
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

            return View(imgFotoEstimate);
        }

        // GET: ImgFotoEstimates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgFotoEstimate = db.ImgFotoEstimates.Find(id);
            if (imgFotoEstimate == null)
            {
                return HttpNotFound();
            }

            return View(imgFotoEstimate);
        }

        // POST: ImgFotoEstimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ImgFotoEstimate imgFotoEstimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imgFotoEstimate.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/EvolutionFoto";
                        var file = string.Format("{0}.jpg", imgFotoEstimate.ImgFotoEstimateId);
                        var response = FilesHelper.UploadPhoto(imgFotoEstimate.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            imgFotoEstimate.Photo = pic;
                        }
                    }

                    db.Entry(imgFotoEstimate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = imgFotoEstimate.ImgFotoEstimateId });
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

            return View(imgFotoEstimate);
        }

        // GET: ImgFotoEstimates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgFotoEstimate = db.ImgFotoEstimates.Find(id);
            if (imgFotoEstimate == null)
            {
                return HttpNotFound();
            }
            return View(imgFotoEstimate);
        }

        // POST: ImgFotoEstimates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var imgFotoEstimate = db.ImgFotoEstimates.Find(id);
            var foto = string.Empty;
            foto = imgFotoEstimate.Photo;
            try
            {
                if (foto != null || string.IsNullOrEmpty(foto))
                {
                    var response = FilesHelper.DeletePhoto(foto);
                    if (response == true)
                    {
                        db.ImgFotoEstimates.Remove(imgFotoEstimate);
                        db.SaveChanges();
                    }
                    else
                    {
                        var ex = new Exception();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(imgFotoEstimate);
                    }
                }
                else
                {
                    db.ImgFotoEstimates.Remove(imgFotoEstimate);
                    db.SaveChanges();
                }

                return RedirectToAction("Details", "Evolutions", new { id = imgFotoEstimate.ClientId });
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
            return View(imgFotoEstimate);
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
