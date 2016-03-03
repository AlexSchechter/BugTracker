using System;

namespace BugTracker.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string CreatedById { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Date { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}