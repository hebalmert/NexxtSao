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

    public class ImgOrthodonsController : Controller
    {
        private NexxtSaoContext db = new NexxtSaoContext();

        // GET: ImgOrthodons
        public ActionResult Index(int? idcliente)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var imgOrthodons = db.ImgOrthodons.Where(c => c.CompanyId == user.CompanyId && c.ClientId == idcliente)
                .Include(i => i.Client);

            return View(imgOrthodons.ToList());
        }

        // GET: ImgOrthodons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgOrthodon = db.ImgOrthodons.Find(id);
            if (imgOrthodon == null)
            {
                return HttpNotFound();
            }
            return View(imgOrthodon);
        }

        // GET: ImgOrthodons/Create
        public ActionResult Create(int idcliente)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var imgorthodon = new ImgOrthodon
            {
                CompanyId = user.CompanyId,
                ClientId = idcliente,
                Date = DateTime.UtcNow
            };

            return View(imgorthodon);
        }

        // POST: ImgOrthodons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ImgOrthodon imgOrthodon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ImgOrthodons.Add(imgOrthodon);
                    db.SaveChanges();

                    if (imgOrthodon.PhotoFile != null)
                    {
                        var folder = "~/Content/OrthoPhoto";
                        var file = string.Format("{0}.jpg", imgOrthodon.ImgOrthodonId);
                        var response = FilesHelper.UploadPhoto(imgOrthodon.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            imgOrthodon.Photo = pic;
                            db.Entry(imgOrthodon).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Details", new { id = imgOrthodon.ImgOrthodonId });
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

            return View(imgOrthodon);
        }

        // GET: ImgOrthodons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgOrthodon = db.ImgOrthodons.Find(id);
            if (imgOrthodon == null)
            {
                return HttpNotFound();
            }

            return View(imgOrthodon);
        }

        // POST: ImgOrthodons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ImgOrthodon imgOrthodon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imgOrthodon.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/OrthoPhoto";
                        var file = string.Format("{0}.jpg", imgOrthodon.ImgOrthodonId);
                        var response = FilesHelper.UploadPhoto(imgOrthodon.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            imgOrthodon.Photo = pic;
                        }
                    }

                    db.Entry(imgOrthodon).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = imgOrthodon.ImgOrthodonId });
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

            return View(imgOrthodon);
        }

        // GET: ImgOrthodons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var imgOrthodon = db.ImgOrthodons.Find(id);
            if (imgOrthodon == null)
            {
                return HttpNotFound();
            }
            return View(imgOrthodon);
        }

        // POST: ImgOrthodons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var imgOrthodon = db.ImgOrthodons.Find(id);
            try
            {
                db.ImgOrthodons.Remove(imgOrthodon);
                db.SaveChanges();

                return RedirectToAction("Details", "Orthodontics", new { id = imgOrthodon.ClientId });
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
            return View(imgOrthodon);
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
