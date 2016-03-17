using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : BaseController
    {
        // GET: Tickets/Index
        public async Task<ActionResult> Index(string selectedUserId, int? projectId)
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.Role = GetRole();
            if (projectId != null)
            {
                ViewBag.Header = String.Concat("List of Tickets for ", db.Projects.Find(projectId).Name, " Project ");
                ViewBag.ownTicketsOnly = false;
                return View(await db.Tickets.Where(t => t.ProjectId == projectId).ToListAsync());
            }

            ApplicationUser selectedUser = db.Users.Find(selectedUserId);
            string selectedUserFullName = (selectedUserId == null)? null: String.Concat(selectedUser.FirstName, " ", selectedUser.LastName);
            UserRole? selectedUserRole = (selectedUserId == null) ? null : GetRole(selectedUserId);
            switch (selectedUserRole)
            {
                case UserRole.Admin:
                case UserRole.Submitter:
                    ViewBag.Header = String.Concat("List of Tickets Submitted by ", selectedUserFullName);
                    return View(await db.Tickets.Where(t => t.SubmittedById == selectedUserId).ToListAsync());

                case UserRole.ProjectManager:
                    ViewBag.Header = String.Concat("List of Tickets Managed by ", selectedUserFullName);
                    var projects = db.Projects.Where(p => p.ManagerId == selectedUserId);
                    List<Ticket> tickets = new List<Ticket>();
                    foreach (Project project in projects)
                        tickets.AddRange(await db.Tickets.Where(t => t.ProjectId == project.Id).ToListAsync());
                    return View(tickets);

                case UserRole.Developer:
                    ViewBag.Header = String.Concat("List of Tickets Assigned to ", selectedUserFullName);
                    return View(await db.Tickets.Where(t => t.DeveloperId == selectedUserId).ToListAsync());

                default:
                    ViewBag.Header = "List of all tickets";
                    return View(await db.Tickets.ToListAsync());
            }
        }

        // GET: Tickets/Details/5
        public async Task<ActionResult> Details(int? ticketId)
        {
            if (ticketId == null)
                return RedirectToAction("Index");
            
            Ticket ticket = await db.Tickets.FindAsync(ticketId);
            if (ticket == null)          
                return HttpNotFound();
            
            ApplicationUser user = GetUserInfo();
            UserRole userRole = GetRole();
            ViewBag.Role = userRole;
            ViewBag.CanCreate = CanCommentOrAttach(ticket);

            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProjectId,Title,Description,Type,Priority")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                
                ticket.CreationDate = DateTimeOffset.Now;
                ticket.Status = TicketStatus.Open;
                ticket.SubmittedById = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin,ProjectManager,Developer")]
        public async Task<ActionResult> Edit(int? ticketId)
        {
            if (ticketId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Role = GetRole();
            Ticket ticket = await db.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeveloperId = new SelectList(GetDevelopersForProject(ticket.ProjectId), "Id", "UserName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager,Developer")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SubmittedById,DeveloperId,ProjectId,CreationDate,Title,Description,Status,Type,Priority")] Ticket updatedTicket)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext db2 = new ApplicationDbContext();
                Ticket originalTicket = await db2.Tickets.FindAsync(updatedTicket.Id);

                if (originalTicket.DeveloperId == updatedTicket.DeveloperId
                    && originalTicket.Priority == updatedTicket.Priority
                    && originalTicket.Status == updatedTicket.Status && originalTicket.Type == updatedTicket.Type)
                {
                    return RedirectToAction("Index");
                }

                if (originalTicket.DeveloperId != updatedTicket.DeveloperId)
                {
                    var callbackUrl = Url.Action("Details", "Tickets", new { ticketId = updatedTicket.Id }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(updatedTicket.DeveloperId, "You have been assigend to a ticket", "View details of the ticket <a href=\"" + callbackUrl + "\">here</a>");                  
                }
                   
                TicketChange changes = new TicketChange
                {
                    TicketId = originalTicket.Id,
                    ChangedById = User.Identity.GetUserId(),
                    Date = DateTimeOffset.Now,
                    OldDeveloperId = originalTicket.DeveloperId,
                    NewDeveloperId = updatedTicket.DeveloperId,
                    OldPriority = originalTicket.Priority,
                    NewPriority = updatedTicket.Priority,
                    OldStatus = originalTicket.Status,
                    NewStatus = updatedTicket.Status,
                    OldType = originalTicket.Type,
                    NewType = updatedTicket.Type
                };

                db.Entry(updatedTicket).State = EntityState.Modified;
                db.TicketChanges.Add(changes);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");            
            }
            ViewBag.DeveloperId = new SelectList(GetDevelopersForProject(updatedTicket.ProjectId), "Id", "FirstName", updatedTicket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", updatedTicket.ProjectId);
            return View(updatedTicket);
        }
     
        //GET: Tickets/Changes
        public async Task<ActionResult> Changes (int? ticketId)
        {
            if (ticketId == null)
                return RedirectToAction("Index");

            ViewBag.Ticket = (await db.Tickets.FindAsync((int)ticketId)).Title;        
            return View(await db.TicketChanges.Where(t => t.TicketId == ticketId).OrderByDescending(t => t.Date).ToListAsync());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
