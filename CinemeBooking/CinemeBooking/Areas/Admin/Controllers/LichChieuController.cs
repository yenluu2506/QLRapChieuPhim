using CinemeBooking.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Filter = CinemeBooking.Models.Common.Filter;

namespace CinemeBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LichChieuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/LichChieu
        public ActionResult Index(string Searchtext, int? SelectedPhongChieus, int? SelectedPhim, int? page)
        {
            ViewBag.PhongChieu = new SelectList(db.PhongChieux.ToList(), "id", "TenPhong");
            ViewBag.Phim = new SelectList(db.Phims.ToList(), "id", "TenPhim");

            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }

            IEnumerable<LichChieu> items = db.LichChieux.OrderByDescending(x => x.id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.Phim.TenPhim).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.Phim.TenPhim).Contains(searchKeyword) ||
                                         x.Phim.TenPhim.Contains(Searchtext));

            }
            if (SelectedPhongChieus.HasValue)
            {
                items = items.Where(x => x.PhongChieu.id == SelectedPhongChieus.Value);
            }
            if (SelectedPhim.HasValue)
            {
                items = items.Where(x => x.Phim.id == SelectedPhim.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);
        }
        public ActionResult Add()
        {
            ViewBag.Phim = new SelectList(db.Phims.ToList(), "id", "TenPhim");
            ViewBag.Phong = new SelectList(db.PhongChieux.ToList(), "id", "TenPhong");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(LichChieu temp, string[] selectedPhim, string[] selectedPhongChieu)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem đã chọn ít nhất một phim và một phòng chiếu
                if (selectedPhim != null && selectedPhim.Length > 0 && selectedPhongChieu != null && selectedPhongChieu.Length > 0)
                {
                    // Lấy id phim và id phòng chiếu đã chọn
                    int selectedPhimId = int.Parse(selectedPhim[0]);
                    int selectedPhongId = int.Parse(selectedPhongChieu[0]);

                    // Lấy phim và phòng đã chọn từ CSDL
                    Phim selectedPhimObj = db.Phims.FirstOrDefault(p => p.id == selectedPhimId);
                    PhongChieu selectedPhongObj = db.PhongChieux.FirstOrDefault(ph => ph.id == selectedPhongId);

                    // Kiểm tra xem phim và phòng đã lấy từ CSDL có tồn tại hay không
                    if (selectedPhimObj != null && selectedPhongObj != null)
                    {
                        // Gán phim và phòng đã chọn vào đối tượng LichChieu
                        temp.Phim = selectedPhimObj;
                        temp.PhongChieu = selectedPhongObj;

                        db.LichChieux.Add(temp);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                ModelState.AddModelError("", "Vui lòng chọn phim và phòng chiếu.");

                // Load lại SelectList khi có lỗi
                ViewBag.Phim = new SelectList(db.Phims.ToList(), "id", "TenPhim");
                ViewBag.Phong = new SelectList(db.PhongChieux.ToList(), "id", "TenPhong");
            }

            return View(temp);
        }

        public ActionResult Edit(int? id)
        {

            // Lấy danh sách các phim từ CSDL và chọn phim tương ứng với đối tượng LichChieu
            ViewBag.Phim = new SelectList(db.Phims.ToList(), "id", "TenPhim");
            ViewBag.PhongChieu = new SelectList(db.PhongChieux.ToList(), "id", "TenPhong");
            var item = db.LichChieux.Find(id);

            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LichChieu temp, string[] selectedPhim, string[] selectedPhongChieu)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem đã chọn ít nhất một phim và một phòng chiếu
                if (selectedPhim != null && selectedPhim.Length > 0 && selectedPhongChieu != null && selectedPhongChieu.Length > 0)
                {
                    // Lấy id phim và id phòng chiếu đã chọn
                    int selectedPhimId = int.Parse(selectedPhim[0]);
                    int selectedPhongId = int.Parse(selectedPhongChieu[0]);

                    // Lấy phim và phòng đã chọn từ CSDL
                    Phim selectedPhimObj = db.Phims.FirstOrDefault(p => p.id == selectedPhimId);
                    PhongChieu selectedPhongObj = db.PhongChieux.FirstOrDefault(ph => ph.id == selectedPhongId);

                    // Kiểm tra xem phim và phòng đã lấy từ CSDL có tồn tại hay không
                    if (selectedPhimObj != null && selectedPhongObj != null)
                    {
                        // Tạo một đối tượng LichChieu mới với cùng khóa chính
                        LichChieu existingLichChieu = db.LichChieux.Find(temp.id);

                        // Cập nhật các thuộc tính của đối tượng LichChieu cũ
                        existingLichChieu.NgayChieu = temp.NgayChieu;
                        existingLichChieu.GioBatDau = temp.GioBatDau;
                        existingLichChieu.PhuPhi = temp.PhuPhi;
                        existingLichChieu.TrangThai = temp.TrangThai;

                        // Gán phim và phòng đã chọn vào đối tượng LichChieu
                        existingLichChieu.Phim = selectedPhimObj;
                        existingLichChieu.PhongChieu = selectedPhongObj;

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                ModelState.AddModelError("", "Vui lòng chọn phim và phòng chiếu.");

                // Load lại SelectList khi có lỗi
                ViewBag.Phim = new SelectList(db.Phims.ToList(), "id", "TenPhim");
                ViewBag.PhongChieu = new SelectList(db.PhongChieux.ToList(), "id", "TenPhong");
            }

            return View(temp);
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.LichChieux.Find(id);
            if (item != null)
            {
                db.LichChieux.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.LichChieux.Find(id);
            if (item != null)
            {
                item.TrangThai = !item.TrangThai;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.TrangThai });
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
                        var obj = db.LichChieux.Find(Convert.ToInt32(item));
                        db.LichChieux.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}