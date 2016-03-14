using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [Authorize]
    public class CommentsController : BaseController
    {      
        // GET: Comments
        public async Task<ActionResult> Index(int? ticketId)
        {
            if (ticketId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Ticket ticket = await db.Tickets.FindAsync(ticketId);
            ViewBag.Ticket = ticket.Title;
            ViewBag.Project = ticket.Project.Name;
            ViewBag.TicketId = ticketId;
            ViewBag.CanCreate = CanCommentOrAttach(ticket);

            return View(await db.Comments.Where(c => c.TicketId == ticketId).Include(c => c.CreatedBy).Include(c => c.Ticket)
                .OrderByDescending(c => c.Date).ToListAsync());
        }

        // GET: Comments/Create
        public async Task<ActionResult> Create(int? ticketId)
        {
            if (ticketId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Ticket ticket = await db.Tickets.FindAsync(ticketId);

            if (!CanCommentOrAttach(ticket))
                return RedirectToAction("Details", "Tickets", new { ticketId = ticketId });

            ViewBag.TicketTitle = ticket.Title;
            ViewBag.Project = ticket.Project.Name;

            return View(new Comment { TicketId = (int)ticketId });
        }

        // POST: Comments/Create      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Body,TicketId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Date = DateTimeOffset.Now;
                comment.CreatedById = GetUserInfo().Id;
                db.Comments.Add(comment);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Tickets", new { ticketId = comment.TicketId });
            }
            return View(comment);
        }       
    }
}
