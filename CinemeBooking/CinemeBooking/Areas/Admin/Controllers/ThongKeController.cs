using CinemeBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemeBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ThongKeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ThongKe
        public ActionResult Index()
        {
            return View();
        }

        /*[HttpGet]
        public ActionResult GetThongKe(string fromDate, string toDate) {
            var query = from o in db.Ves
            return Json(true);        
        }*/
    }
}