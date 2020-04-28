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

    public class ImgPanoramicsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: ImgPanoramics
        public ActionResult Index(int? idcliente)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var imgPanoramics = db.ImgPanoramics.Where(c => c.CompanyId == user.CompanyId && c.ClientId == idcliente)
                .Include(i => i.Client);

            return View(imgPanoramics.ToList());
        }

        // GET: ImgPanoramics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPanoramic = db.ImgPanoramics.Find(id);
            if (imgPanoramic == null)
            {
                return HttpNotFound();
            }
            return View(imgPanoramic);
        }

        // GET: ImgPanoramics/Create
        public ActionResult Create(int idcliente)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var imgpanoramica = new ImgPanoramic
            {
                CompanyId = user.CompanyId,
                ClientId = idcliente,
                Date = DateTime.UtcNow
            };

            return View(imgpanoramica);
        }

        // POST: ImgPanoramics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ImgPanoramic imgPanoramic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ImgPanoramics.Add(imgPanoramic);
                    db.SaveChanges();

                    if (imgPanoramic.PhotoFile != null)
                    {
                        var folder = "~/Content/OrthoPano";
                        var file = string.Format("{0}.jpg", imgPanoramic.ImgPanoramicId);
                        var response = FilesHelper.UploadPhoto(imgPanoramic.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            imgPanoramic.Photo = pic;
                            db.Entry(imgPanoramic).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }


                    return RedirectToAction("Details", new { id = imgPanoramic.ImgPanoramicId});
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

            return View(imgPanoramic);
        }

        // GET: ImgPanoramics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPanoramic = db.ImgPanoramics.Find(id);
            if (imgPanoramic == null)
            {
                return HttpNotFound();
            }

            return View(imgPanoramic);
        }

        // POST: ImgPanoramics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ImgPanoramic imgPanoramic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imgPanoramic.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/OrthoPano";
                        var file = string.Format("{0}.jpg", imgPanoramic.ImgPanoramicId);
                        var response = FilesHelper.UploadPhoto(imgPanoramic.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            imgPanoramic.Photo = pic;
                        }
                    }

                    db.Entry(imgPanoramic).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = imgPanoramic.ImgPanoramicId });
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

            return View(imgPanoramic);
        }

        // GET: ImgPanoramics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgPanoramic = db.ImgPanoramics.Find(id);
            if (imgPanoramic == null)
            {
                return HttpNotFound();
            }
            return View(imgPanoramic);
        }

        // POST: ImgPanoramics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var imgPanoramic = db.ImgPanoramics.Find(id);
            var foto = string.Empty;
            foto = imgPanoramic.Photo;
            try
            {
                if (foto != null || string.IsNullOrEmpty(foto))
                {
                    var response = FilesHelper.DeletePhoto(foto);
                    if (response == true)
                    {
                        db.ImgPanoramics.Remove(imgPanoramic);
                        db.SaveChanges();
                    }
                    else
                    {
                        var ex = new Exception();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(imgPanoramic);
                    }
                }
                else 
                {
                    db.ImgPanoramics.Remove(imgPanoramic);
                    db.SaveChanges();
                }

                return RedirectToAction("Details", "Orthodontics", new { id = imgPanoramic.ClientId });
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
            return View(imgPanoramic);
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
