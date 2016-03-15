using System;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string OldDeveloperId { get; set; }
        public string NewDeveloperId { get; set; }
        public string ChangedById { get; set; }
        public DateTimeOffset Date { get; set; }
        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }
        [InverseProperty("OldDevelopers")]      
        public virtual ApplicationUser OldDeveloper { get; set; }
        [InverseProperty("NewDevelopers")]
        public virtual ApplicationUser NewDeveloper { get; set; }
        [InverseProperty("TicketChanges")]
        public virtual ApplicationUser ChangedBy { get; set; }
       
    }
}