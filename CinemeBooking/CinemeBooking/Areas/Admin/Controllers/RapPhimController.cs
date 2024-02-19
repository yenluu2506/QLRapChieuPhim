using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CinemeBooking.Models;
using PagedList;
using Filter = CinemeBooking.Models.Common.Filter;

namespace CinemeBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class RapPhimController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/RapPhims
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<RapPhim> items = db.RapPhims.OrderByDescending(x => x.id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TenRap).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TenRap).Contains(searchKeyword) ||
                                         x.TenRap.Contains(Searchtext));

            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);

        }

        public ActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RapPhim temp)
        {
            if (ModelState.IsValid)
            {
                db.RapPhims.Add(temp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temp);
        }

        public ActionResult Edit(int? id)
        {
            var item = db.RapPhims.Find(id);
            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RapPhim temp)
        {
            if (ModelState.IsValid)
            {
                db.RapPhims.Attach(temp);
                db.Entry(temp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temp);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.RapPhims.Find(id);
            if (item != null)
            {
                db.RapPhims.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.RapPhims.Find(Convert.ToInt32(item));
                        db.RapPhims.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
