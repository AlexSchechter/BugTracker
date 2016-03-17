using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    
    [RequireHttps]
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            UserRole role = GetRole();           
            return RedirectToAction(role.ToString(), "Dashboard");
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