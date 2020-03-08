using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Internal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*allico}", new { allico = @".*\.ico(/.*)?" });

            //routes.MapRoute(name: "About", url: "about", defaults: new { Controller = "Home", Action = "about" });
            //routes.MapRoute(name: "Services", url: "services", defaults: new { Controller = "Home", Action = "services" });
            //routes.MapRoute(name: "Portfolio", url: "portfolio", defaults: new { Controller = "Portfolio", Action = "Index" });

            routes.MapRoute(
                name: "Languages",
                url: "{language}/{controller}/{action}/{id}",
                defaults:
                new
                {
                    controller = "Home",
                    action = "Index",
                    language = "",
                    id = UrlParameter.Optional
                },
                constraints: new { language = @"(en)|(ro)" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
