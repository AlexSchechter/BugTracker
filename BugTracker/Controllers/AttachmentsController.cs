using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.IO;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize]
    public class AttachmentsController : BaseController
    {
     
        // GET: Attachments
        public async Task<ActionResult> Index(int? ticketId)
        {
            if (ticketId == null)
                return RedirectToAction("Index", "Tickets");

            Ticket ticket = await db.Tickets.FindAsync(ticketId);
            ViewBag.CanCreate = CanCommentOrAttach(ticket);
            ViewBag.Ticket = ticket.Title;
            ViewBag.Project = ticket.Project.Name;
            ViewBag.TicketId = ticket.Id;
            return View(await db.Attachments.Where(a => a.TicketId == ticketId).Include(a => a.CreatedBy)
                .Include(a => a.Ticket).ToListAsync());
        }

        // GET: Attachments/Create
        public ActionResult Create()
        {
            ViewBag.CreatedById = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmittedById");
            return View();
        }

        // POST: Attachments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int ticketId, string description, HttpPostedFileBase fileUpload)
        {
            if (!ModelState.IsValid || !CanCommentOrAttach(ticketId))
                return RedirectToAction("Details", "Tickets", new { ticketID = ticketId });

            var fileName = Path.GetFileName(fileUpload.FileName);
            fileUpload.SaveAs(Path.Combine(Server.MapPath("~/assets/Attachments/"), fileName));
            db.Attachments.Add(new Attachment
                {
                    CreatedById = User.Identity.GetUserId(),
                    Description = description,
                    Date = DateTimeOffset.Now,
                    TicketId = ticketId,
                    Url = string.Concat("/assets/Attachements/", fileName)
                });
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Attachments", new { ticketId = ticketId});                  
        }     
    }
}
