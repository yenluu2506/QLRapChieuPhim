using CinemeBooking.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Filter = CinemeBooking.Models.Common.Filter;

namespace CinemeBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class PhimController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Phim
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }


            IEnumerable<Phim> items = db.Phims.OrderByDescending(x => x.id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TenPhim).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TenPhim).Contains(searchKeyword) ||
                                         x.TenPhim.Contains(Searchtext));

            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);
        }

        public ActionResult Add()
        {
            // Tạo SelectList
            SelectList select_tl = new SelectList(db.TheLoais.ToList(), "id", "TenTheLoai");
            ViewBag.selectedTheLoais = new List<SelectListItem>();
            // Set vào ViewBag
            ViewBag.list_TheLoai = select_tl;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Phim temp, string[] selectedTheLoais)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                // Loại bỏ phần tử rỗng (giá trị "")
                selectedTheLoais = selectedTheLoais.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (selectedTheLoais.Length > 0)
                {
                    temp.TheLoais = db.TheLoais.Where(t => selectedTheLoais.Contains(t.id.ToString())).ToList();
                }
                else
                {
                    temp.TheLoais = new List<TheLoai>();
                }
                db.Phims.Add(temp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temp);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin phim từ cơ sở dữ liệu
            Phim phim = db.Phims.Find(id);
            if (phim == null)
            {
                return HttpNotFound();
            }

            // Tạo SelectList cho thể loại
            SelectList select_tl = new SelectList(db.TheLoais.ToList(), "id", "TenTheLoai");
            ViewBag.list_TheLoai = select_tl;

            // Lấy danh sách thể loại đã chọn của phim
            var selectedTheLoais = phim.TheLoais.Select(t => t.id.ToString()).ToArray();
            ViewBag.selectedTheLoais = selectedTheLoais;

            return View(phim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Phim temp, string[] selectedTheLoais)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin phim gốc từ cơ sở dữ liệu
                var phim = db.Phims.Include(p => p.TheLoais).FirstOrDefault(p => p.id == temp.id);
                if (phim == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin phim
                phim.TenPhim = temp.TenPhim;
                phim.MoTa = temp.MoTa;
                phim.ThoiLuong = temp.ThoiLuong;
                phim.NgayKhoiChieu = temp.NgayKhoiChieu;
                phim.NgayKetThuc = temp.NgayKetThuc;
                phim.QuocGia = temp.QuocGia;
                phim.DaoDien = temp.DaoDien;
                phim.DienVien = temp.DienVien;
                phim.NamSX = temp.NamSX;
                phim.ApPhich = temp.ApPhich;
                phim.TrangThai = temp.TrangThai;

                // Loại bỏ phần tử rỗng (giá trị "")
                selectedTheLoais = selectedTheLoais.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                // Cập nhật danh sách thể loại của phim
                if (selectedTheLoais.Length > 0)
                {
                    // Xóa tất cả các thể loại hiện tại
                    phim.TheLoais.Clear();

                    // Thêm từng thể loại mới vào danh sách
                    foreach (var theLoaiId in selectedTheLoais)
                    {
                        var theLoai = db.TheLoais.Find(int.Parse(theLoaiId));
                        if (theLoai != null)
                        {
                            phim.TheLoais.Add(theLoai);
                        }
                    }
                }
                else
                {
                    // Xóa tất cả các thể loại
                    phim.TheLoais.Clear();
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(temp);
        }




        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Phims.Find(id);
            if (item != null)
            {
                db.Phims.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Phims.Find(id);
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
                        var obj = db.Phims.Find(Convert.ToInt32(item));
                        db.Phims.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}