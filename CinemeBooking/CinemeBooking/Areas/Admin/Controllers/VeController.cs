using CinemeBooking.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemeBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class VeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Ve
        public ActionResult Index(int? page)
        {

            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }

            IEnumerable<Ve> items = db.Ves.OrderByDescending(x => x.id);
            /*if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TenPhim).Contains(searchKeyword) ||
                                         x.TenPhim.Contains(Searchtext));

            }*/
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;

            var veList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Vé người lớn" },
                new SelectListItem { Value = "1", Text = "Vé học sinh sinh viên" },
                new SelectListItem { Value = "2", Text = "Vé trẻ em" }
            };

            ViewBag.VeList = veList;

            var lichChieuIds = items.Select(v => v.LichChieu.id).ToList();
            var lichChieus = db.LichChieux.Where(l => lichChieuIds.Contains(l.id)).ToList();

     
            return View(items);
        }

        public ActionResult Add()
        {
            // Lấy data
            // Lấy toàn bộ thể loại:
            var tl = db.LichChieux.ToList();
            // Tạo SelectList
            SelectList select_lc = new SelectList(tl, "id", "NgayChieu", "GioBatDau");
            // Set vào ViewBag
            ViewBag.list_LichChieu = select_lc;

            var pc = db.PhongChieux.ToList();
            // Tạo SelectList
            SelectList select_pc = new SelectList(pc, "id", "TenPhong");
            // Set vào ViewBag
            ViewBag.list_LichChieu = select_pc;

            var g = db.Ghes.ToList();
            // Tạo SelectList
            SelectList select_g = new SelectList(g, "id", "MaGhe");
            // Set vào ViewBag
            ViewBag.list_LichChieu = select_g;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Ve temp)
        {
            if (ModelState.IsValid)
            {
                db.Ves.Add(temp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temp);
        }

        public ActionResult Edit(int id)
        {
            var item = db.Ves.Find(id);
            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ve temp)
        {
            if (ModelState.IsValid)
            {
                db.Ves.Attach(temp);
                db.Entry(temp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temp);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Ves.Find(id);
            if (item != null)
            {
                db.Ves.Remove(item);
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
                        var obj = db.Ves.Find(Convert.ToInt32(item));
                        db.Ves.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}