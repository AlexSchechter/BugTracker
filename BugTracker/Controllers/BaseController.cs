﻿using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
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

        protected readonly List<string> DemoEmails 
            = new List<string> {"robb@stark.com", "petyr@littlefinger.com", "arya@stark.com", "cersei@lannister.com"};
        protected ApplicationDbContext db = new ApplicationDbContext();
        protected RoleManager<IdentityRole> roleManager;
        private ApplicationUserManager _userManager;

        public BaseController()
        {          
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        protected ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected ApplicationUser GetUserInfo()
        {           
            return db.Users.Find(User.Identity.GetUserId());

        }

        protected bool CanCommentOrAttach(Ticket ticket)
        {
            string userId = User.Identity.GetUserId();
            return GetRole() == UserRole.Admin || ticket.Project.ManagerId == userId || ticket.DeveloperId == userId
                || ticket.SubmittedById == userId;
        }

        protected bool CanCommentOrAttach(int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            string userId = User.Identity.GetUserId();
            return GetRole() == UserRole.Admin || ticket.Project.ManagerId == userId || ticket.DeveloperId == userId
                || ticket.SubmittedById == userId;
        }

        public UserRole GetRole()
        {
            string roleId = UserManager.FindById(GetUserInfo().Id).Roles.First().RoleId;           
            return (UserRole)Enum.Parse(typeof(UserRole), roleManager.FindById(roleId).Name);
        }


        public UserRole? GetRole(string userId)
        {
            if (userId == null)
                return null;

            string roleId = UserManager.FindById(userId).Roles.First().RoleId;
            return (UserRole)Enum.Parse(typeof(UserRole), roleManager.FindById(roleId).Name);
        }

        public string GetRoleString(string userId)
        {
            string roleId = UserManager.FindById(userId).Roles.First().RoleId;
            return roleManager.FindById(roleId).Name;
        }

        public IEnumerable<ApplicationUser> GetDevelopers()
        {           
            return db.Database.SqlQuery<ApplicationUser>("EXEC GetDevelopers");
        }

        public IEnumerable<ApplicationUser> GetProjectManagers()
        {
            return db.Users.ToList().Where(u => UserManager.IsInRole(u.Id, "ProjectManager"));            
        }

        public IEnumerable<ApplicationUser> GetAdmins()
        {
            return db.Users.ToList().Where(u => UserManager.IsInRole(u.Id, "Admin"));
        }

        public IEnumerable<ApplicationUser> GetSubmitters()
        {
            return db.Users.ToList().Where(u => UserManager.IsInRole(u.Id, "Submitter"));
        }

        public IEnumerable<Project> GetProjectsForDeveloper(string developerId)
        {           
            return db.Database.SqlQuery<Project>("EXEC ProjectsForDeveloper @developerId", new SqlParameter("developerId", developerId));
        }

        public IEnumerable<Project> GetProjectsForProjectManager(string managerId)
        {
            return db.Database.SqlQuery<Project>("EXEC ProjectsForProjectManager @managerId", new SqlParameter("managerId", managerId));
        }

        public IEnumerable<Ticket> GetTicketsForProjectManager(string managerId)
        {
            return db.Database.SqlQuery<Ticket>("EXEC TicketsForProjectManager @managerId", new SqlParameter("managerId", managerId));
        }

        public IEnumerable<ApplicationUser> GetDevelopersForProject(int projectId)
        {
            return db.Database.SqlQuery<ApplicationUser>("EXEC GetDevelopersForProject @projectId", new SqlParameter("projectId", projectId));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}