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
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}