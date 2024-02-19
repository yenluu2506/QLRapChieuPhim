using CinemeBooking.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Filter = CinemeBooking.Models.Common.Filter;

namespace CinemeBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class PhongChieuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/PhongChieu
        public ActionResult Index(string Searchtext, int? SelectedRap, int? SelectedMH, int? page)
        {
            ViewBag.Rap = new SelectList(db.RapPhims.ToList(), "id", "TenRap");
            ViewBag.ManHinh = new SelectList(db.LoaiManHinhs.ToList(), "id", "TenMH");

            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<PhongChieu> items = db.PhongChieux.OrderByDescending(x => x.id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TenPhong).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TenPhong).Contains(searchKeyword));

            }
            if (SelectedRap.HasValue)
            {
                items = items.Where(x => x.RapPhim.id == SelectedRap.Value);
            }
            if (SelectedMH.HasValue)
            {
                items = items.Where(x => x.LoaiManHinh.id == SelectedMH.Value);
            }

            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);
        }

        public ActionResult GetPhongChieuByRap(int rapId)
        {
            var phongChieuList = db.PhongChieux.Where(x => x.RapPhim.id == rapId).ToList();
            return PartialView("_PhongChieuList", phongChieuList);
        }


        public ActionResult Add()
        {
            ViewBag.ManHinh = new SelectList(db.LoaiManHinhs.ToList(), "id", "TenMH");
            ViewBag.Rap = new SelectList(db.RapPhims.ToList(), "id", "TenRap");
            ViewBag.selectedManHinh = new List<SelectListItem>();
            ViewBag.selectedRap = new List<SelectListItem>();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PhongChieu temp, string[] selectedManHinh, string[] selectedRap)
        {

            if (ModelState.IsValid)
            {
                selectedManHinh = selectedManHinh.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (selectedManHinh.Length > 0)
                {
                    temp.LoaiManHinh = db.LoaiManHinhs.FirstOrDefault(t => selectedManHinh.Contains(t.id.ToString()));
                }
                if (selectedManHinh.Length > 0)
                {
                    temp.RapPhim = db.RapPhims.FirstOrDefault(t => selectedRap.Contains(t.id.ToString()));
                }

                db.PhongChieux.Add(temp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temp);
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.Rap = new SelectList(db.RapPhims.ToList(), "id", "TenRap");
            ViewBag.ManHinh = new SelectList(db.LoaiManHinhs.ToList(), "id", "TenMH");
            var item = db.PhongChieux.Find(id);
            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhongChieu temp)
        {
            if (ModelState.IsValid)
            {
                db.PhongChieux.Attach(temp);
                db.Entry(temp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temp);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.PhongChieux.Find(id);
            if (item != null)
            {
                db.PhongChieux.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.PhongChieux.Find(id);
            if (item != null)
            {
                item.TinhTrang = !item.TinhTrang;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.TinhTrang });
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
                        var obj = db.PhongChieux.Find(Convert.ToInt32(item));
                        db.PhongChieux.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
