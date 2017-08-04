using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Management;

namespace testingv.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            string path = Server.MapPath("~/Content/HtmlPage1.html");
            management.DealerHelper.SendInvoice("vimalkrishna17@rediffmail.com", path);
            
            
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}