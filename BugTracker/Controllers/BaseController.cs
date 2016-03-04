using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class BaseController : Controller
    {    
        protected ApplicationDbContext db = new ApplicationDbContext();
        protected UserManager<ApplicationUser> userManager;
        protected RoleManager<IdentityRole> roleManager;

        public BaseController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        protected ApplicationUser GetUserInfo()
        {
            return db.Users.Find(User.Identity.GetUserId());
        }

        public UserRole GetRole()
        {
            string roleId = userManager.FindById(GetUserInfo().Id).Roles.First().RoleId;
            return (UserRole)Enum.Parse(typeof(UserRole), roleManager.FindById(roleId).Name);
        }

        public UserRole GetRole(string userId)
        {
            string roleId = userManager.FindById(userId).Roles.First().RoleId;
            return (UserRole)Enum.Parse(typeof(UserRole), roleManager.FindById(roleId).Name);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}