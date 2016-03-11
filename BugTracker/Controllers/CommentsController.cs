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
            
            if (!CanCommentOrAttach(await db.Tickets.FindAsync(ticketId)))
                return RedirectToAction("Index", new { ticketId = ticketId });

            return View(await db.Comments.Include(c => c.CreatedBy).Include(c => c.Ticket).ToListAsync());
        }

        // GET: Comments/Create
        public async Task<ActionResult> Create(int? ticketId)
        {
            if (ticketId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!CanCommentOrAttach(await db.Tickets.FindAsync(ticketId)))
                return RedirectToAction("Details", "Tickets", new { ticketId = ticketId });

            return View(new Comment { TicketId = (int)ticketId });
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
