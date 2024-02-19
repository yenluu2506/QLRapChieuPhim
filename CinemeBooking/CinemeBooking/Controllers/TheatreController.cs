using System.Web.Mvc;

namespace CinemeBooking.Controllers
{
    public class TheatreController : Controller
    {
        // GET: Theatre
        public ActionResult Index()
        {
            return View();
        }
    }
}