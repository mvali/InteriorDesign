using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Internal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Base64Encode()
        {
            var qs = Utils.Rq("c");
            //var retV = Utils.Base64Encode(qs); // quick encode trick :) no one will know except me
            //Log.AddLogEntry("-" + retV);
            return PartialView("Error");
        }
        public ActionResult Base64Decode()
        {
            var qs = Utils.Rq("c");
            //var retV = Utils.Base64Decode(qs); // quick decode trick :) no one will know except me
            //Log.AddLogEntry("-" + retV);
            return PartialView("Error");
        }
    }
}