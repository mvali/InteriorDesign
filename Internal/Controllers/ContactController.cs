using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Internal.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult ContactAjax()
        {
            return PartialView(new Internal.Contact());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactAjax(Internal.Contact task)
        {
            if (ModelState.IsValid)
            {
                var data1 = task.Name + ", " + task.Email;

                var bRet = task.SendEmail();
                TempData["Status"] = "emailSent";

                return PartialView("ContactAjax", task);
            }
            else
            {
                TempData["Status"] = "Message err from controller";
                return PartialView("ContactAjax", task);
                //return PartialView("contactResult", task);
            }
        }

    }
}