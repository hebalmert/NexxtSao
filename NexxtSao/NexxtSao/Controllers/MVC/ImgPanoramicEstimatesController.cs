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

    public class ImgPanoramicEstimatesController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: ImgPanoramicEstimates
        public ActionResult Index()
        {
            var imgPanoramicEstimates = db.ImgPanoramicEstimates
                .Include(i => i.Client)
                .Include(i => i.Estimate);

            return View(imgPanoramicEstimates.ToList());
        }

        // GET: ImgPanoramicEstimates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPanoramicEstimate = db.ImgPanoramicEstimates.Find(id);
            if (imgPanoramicEstimate == null)
            {
                return HttpNotFound();
            }
            return View(imgPanoramicEstimate);
        }

        // GET: ImgPanoramicEstimates/Create
        public ActionResult Create(int idcliente, int idestimate)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var imgpanoramicestimate = new ImgPanoramicEstimate
            {
                CompanyId = user.CompanyId,
                EstimateId = idestimate,
                ClientId = idcliente,
                Date = DateTime.UtcNow
            };

            return View(imgpanoramicestimate);
        }

        // POST: ImgPanoramicEstimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ImgPanoramicEstimate imgPanoramicEstimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ImgPanoramicEstimates.Add(imgPanoramicEstimate);
                    db.SaveChanges();

                    if (imgPanoramicEstimate.PhotoFile != null)
                    {
                        var folder = "~/Content/EvolutionPano";
                        var file = string.Format("{0}.jpg", imgPanoramicEstimate.ImgPanoramicEstimateId);
                        var response = FilesHelper.UploadPhoto(imgPanoramicEstimate.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            imgPanoramicEstimate.Photo = pic;
                            db.Entry(imgPanoramicEstimate).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }


                    return RedirectToAction("Details", new { id = imgPanoramicEstimate.ImgPanoramicEstimateId });

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

            return View(imgPanoramicEstimate);
        }

        // GET: ImgPanoramicEstimates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPanoramicEstimate = db.ImgPanoramicEstimates.Find(id);
            if (imgPanoramicEstimate == null)
            {
                return HttpNotFound();
            }

            return View(imgPanoramicEstimate);
        }

        // POST: ImgPanoramicEstimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ImgPanoramicEstimate imgPanoramicEstimate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imgPanoramicEstimate.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/EvolutionPano";
                        var file = string.Format("{0}.jpg", imgPanoramicEstimate.ImgPanoramicEstimateId);
                        var response = FilesHelper.UploadPhoto(imgPanoramicEstimate.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            imgPanoramicEstimate.Photo = pic;
                        }
                    }

                    db.Entry(imgPanoramicEstimate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = imgPanoramicEstimate.ImgPanoramicEstimateId });
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

            return View(imgPanoramicEstimate);
        }

        // GET: ImgPanoramicEstimates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPanoramicEstimate = db.ImgPanoramicEstimates.Find(id);
            if (imgPanoramicEstimate == null)
            {
                return HttpNotFound();
            }
            return View(imgPanoramicEstimate);
        }

        // POST: ImgPanoramicEstimates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var imgPanoramicEstimate = db.ImgPanoramicEstimates.Find(id);
            var foto = string.Empty;
            foto = imgPanoramicEstimate.Photo;
            try
            {
                if (foto != null || string.IsNullOrEmpty(foto))
                {
                    var response = FilesHelper.DeletePhoto(foto);
                    if (response == true)
                    {
                        db.ImgPanoramicEstimates.Remove(imgPanoramicEstimate);
                        db.SaveChanges();
                    }
                    else
                    {
                        var ex = new Exception();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(imgPanoramicEstimate);
                    }
                }
                else
                {
                    db.ImgPanoramicEstimates.Remove(imgPanoramicEstimate);
                    db.SaveChanges();
                }

                return RedirectToAction("Details", "Evolutions", new { id = imgPanoramicEstimate.ClientId });
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
            return View(imgPanoramicEstimate);
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
