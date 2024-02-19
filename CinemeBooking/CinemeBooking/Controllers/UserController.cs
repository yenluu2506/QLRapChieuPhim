using System.Web.Mvc;

namespace CinemeBooking.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        public ActionResult Login()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}