using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: Dashboard/Admin
        public async Task<ActionResult> Admin()
        {          
            return View(new AdminViewModel
            {
                CriticalTickets = await db.Tickets.Where(t => t.Priority == TicketPriority.Critical)
                    .OrderByDescending(t => t.CreationDate).ToListAsync(),
                Projects = await db.Projects.OrderByDescending(p =>p.Tickets.Where(t => t.Status != TicketStatus.Closed).Count())
                    .Take(6).ToListAsync(),
                AdminPopulation = GetAdmins().Count(),
                PMPopulation = GetProjectManagers().Count(),
                DeveloperPopulation = GetDevelopers().Count(),
                SubmitterPopulation = GetSubmitters().Count()
            });      
        }

        // GET: Dashboard/ProjectManager
        [Authorize(Roles = "ProjectManager")]
        public async Task<ActionResult> ProjectManager()
        {
            string userId = User.Identity.GetUserId();
            return View(new ProjectManagerViewModel
            {
                OwnProjects = await db.Projects.Where(p => p.ManagerId == userId)
                    .OrderByDescending(p => p.Tickets.Where(t => t.Status != TicketStatus.Closed).Count()).ToListAsync(),
                CriticalTickets = await db.Tickets.Where(t => t.Priority == TicketPriority.Critical && t.Project.ManagerId == userId)
                    .OrderByDescending(t => t.CreationDate).ToListAsync(),
                UnassignedTickets = await db.Tickets.Where(t => t.DeveloperId == null & t.Project.ManagerId == userId)
                    .OrderByDescending(t => t.CreationDate).ToListAsync(),
                FinishedTickets = await db.Tickets.Where(t => (t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Unresolved)
                    && t.Project.ManagerId == userId).OrderByDescending(t => t.CreationDate).ToListAsync()
            });
        }

        //GET: Dashboard/Developer
        [Authorize(Roles = "Developer")]
        public async Task<ActionResult> Developer()
        {
            string userId = User.Identity.GetUserId();
            return View(await db.Tickets.Where(t => t.DeveloperId == userId).OrderByDescending(t => t.CreationDate).ToListAsync());

        }

        //GET: Dashboard/Submitter
        [Authorize(Roles = "Submitter")]
        public async Task<ActionResult> Submitter()
        {
            string userId = User.Identity.GetUserId();
            return View(await db.Tickets.Where(t => t.SubmittedById == userId).OrderByDescending(t => t.CreationDate).ToListAsync());
        }     
    }
}