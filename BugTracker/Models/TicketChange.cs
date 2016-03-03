using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace BugTracker.Models
{
    public class TicketChange
    {
        public int Id { get; set; }
        public TicketPriority OldPriority { get; set; }
        public TicketPriority NewPriority { get; set; }
        public TicketType OldType { get; set; }
        public TicketType NewType { get; set; }
        public TicketStatus OldStatus { get; set; }
        public TicketStatus NewStatus { get; set; }
        public string ChangedById { get; set; }
        public DateTimeOffset Date { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser ChangedBy { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}