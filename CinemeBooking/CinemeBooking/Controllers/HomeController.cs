using CinemeBooking.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CinemeBooking.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private List<Phim> PhimList = new List<Phim>();
        public ActionResult Index()
        {
            PhimList = db.Phims.ToList();   
            return View(PhimList);
        }
    }
}