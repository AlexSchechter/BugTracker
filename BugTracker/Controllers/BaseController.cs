using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public IEnumerable<ApplicationUser> GetDevelopers()
        {
            //List<HouseholdAccount> model = db.Database.SqlQuery<HouseholdAccount>("EXEC GetHouseholdAccountsForHousehold @householdId", new SqlParameter("householdId", household.Id));
            //string id = db.Users.FirstOrDefault(u => u.Email == "Daeneris@Targaryen.com").Id;
            //int result = db.Database.SqlQuery<int>("EXEC DeveloperInProject @developerId, @projectId, @result", new SqlParameter("developerId", id), new SqlParameter("projectId", 2), new SqlParameter("result", 0)).First();
            return db.Database.SqlQuery<ApplicationUser>("EXEC GetDevelopers");
            //return db.Users.ToList().Where(userManager.IsInRole(u.Id, "Developer"));
        }

        public IEnumerable<ApplicationUser> GetProjectManagers()
        {
            return db.Users.ToList().Where(u => userManager.IsInRole(u.Id, "ProjectManager"));            
        }

        public IEnumerable<Project> GetProjectsForDeveloper(string developerId)
        {
            return db.Database.SqlQuery<Project>("EXEC ProjectsForDeveloper @developerId", new SqlParameter("developerId", developerId));
        }

        public IEnumerable<Project> GetProjectsForProjectManager(string managerId)
        {
            return db.Database.SqlQuery<Project>("EXEC ProjectsForProjectManager @managerId", new SqlParameter("managerId", managerId));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}