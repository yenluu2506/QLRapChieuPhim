using System.Web.Mvc;
using System.Web.Routing;

namespace CinemeBooking
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

           routes.MapRoute(
               name: "vnpay_return",
               url: "vnpay_return",
               defaults: new { controller = "BookTicket", action = "VnpayReturn", alias = UrlParameter.Optional },
               namespaces: new[] { "CinemeBooking.Controllers" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CinemeBooking.Controllers" }

            );
        }
    }
}
