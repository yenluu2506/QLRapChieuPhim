using CinemeBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CinemeBooking.Controllers
{
    public class PhimController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Movie
        public ActionResult Index()
        {
            List<Phim> PhimList = db.Phims.ToList();
            return View(PhimList);
        }

        public ActionResult Details(int? id)
        {
            List<Phim> PhimList = db.Phims.ToList();
            Phim phim = db.Phims.Find(id);

            // Lấy danh sách thể loại đã chọn của phim
            var tl = phim.TheLoais.Select(t => t.TenTheLoai.ToString()).ToArray();
            string theLoais = string.Join(", ", tl);

            // Lấy danh sách lịch chiếu của phim và tìm rạp chiếu
            IEnumerable<LichChieu> lichChieu = phim.LichChieux.Where(lc => lc.NgayChieu == DateTime.Today);
            List<int> phongIds = lichChieu.Select(lc => lc.PhongChieu.id).ToList();

            // Truy vấn danh sách phòng tương ứng
            IEnumerable<PhongChieu> danhSachPhongTuongUng = db.PhongChieux.Where(p => phongIds.Contains(p.id)).ToList();

            // Lấy danh sách ID rạp từ danh sách phòng tương ứng
            List<int> rapIds = danhSachPhongTuongUng.Select(p => p.RapPhim.id).ToList();

            // Lấy ra các rạp chiếu tương ứng
            IEnumerable<RapPhim> rapChieu = db.RapPhims.Where(r => rapIds.Contains(r.id)).ToList();

            //IEnumerable<LoaiManHinh> loaiMH = db.PhongChieux.Where(r => phongIds.Contains(r.id)).ToList();

            ViewBag.PhimList = PhimList;
            ViewBag.TheLoais = theLoais;
            ViewBag.RapChieu = rapChieu;
            ViewBag.LichChieu = lichChieu;
            ViewBag.first = "true";


            return View(phim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int? id, string selectedThanhPho, DateTime selectedDate, int? selectedRapChieuId)
        {
            List<Phim> PhimList = db.Phims.ToList();
            Phim phim = db.Phims.Find(id);

            // Lấy danh sách thể loại đã chọn của phim
            var tl = phim.TheLoais.Select(t => t.TenTheLoai.ToString()).ToArray();
            string theLoais = string.Join(", ", tl);

            IEnumerable<LichChieu> lichChieu = phim.LichChieux.Where(lc => lc.NgayChieu.Date == selectedDate);
            List<int> phongIds = lichChieu.Select(lc => lc.PhongChieu.id).ToList();

            // Truy vấn danh sách phòng tương ứng
            IEnumerable<PhongChieu> danhSachPhongTuongUng = db.PhongChieux.Where(p => phongIds.Contains(p.id)).ToList();

            // Lấy danh sách ID rạp từ danh sách phòng tương ứng
            List<int> rapIds = danhSachPhongTuongUng.Select(p => p.RapPhim.id).ToList();

            // Lấy ra các rạp chiếu tương ứng
            IEnumerable<RapPhim> rapChieu = db.RapPhims.Where(r => rapIds.Contains(r.id)).ToList();

            IEnumerable<RapPhim> rapChieuDaChon;
            if (!string.IsNullOrEmpty(selectedThanhPho) && selectedRapChieuId.HasValue)
            {
                rapChieuDaChon = rapChieu.Where(rc => rc.ThanhPho.Contains(selectedThanhPho) && rc.id == selectedRapChieuId);
            }
            else if (!string.IsNullOrEmpty(selectedThanhPho))
            {
                rapChieuDaChon = rapChieu.Where(rc => rc.ThanhPho.Contains(selectedThanhPho));
            }
            else if (selectedRapChieuId.HasValue)
            {
                rapChieuDaChon = rapChieu.Where(rc => rc.id == selectedRapChieuId);
            }
            else
            {
                rapChieuDaChon = rapChieu;
            }

            ViewBag.PhimList = PhimList;
            ViewBag.TheLoais = theLoais;
            ViewBag.RapChieu = rapChieuDaChon;
            ViewBag.LichChieu = lichChieu;
            ViewBag.selectedDate = selectedDate;


            return View(phim);
        }
    }
}