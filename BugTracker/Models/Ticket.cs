﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public enum TicketStatus { Open, Resolved, Unresolved, Closed  }
    public enum TicketType { Bug, Enhancement, Design, Refactoring }
    public enum TicketPriority { Low, Standard, High, Critical }

    public class Ticket
    {     
        public Ticket()
        {
            TicketChanges = new HashSet<TicketChange>();
            Comments = new HashSet<Comment>();
            Attachments = new HashSet<Attachment>();
        }

        public int Id { get; set; }
        public string SubmittedById { get; set; }
        public string DeveloperId { get; set; }
        public int ProjectId { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketType Type { get; set; }
        public TicketPriority Priority { get; set; }

        [InverseProperty("AssignedTickets")]
        public virtual ApplicationUser Developer { get; set; }
        [InverseProperty("SubmittedTickets")]
        public virtual ApplicationUser SubmittedBy { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<TicketChange> TicketChanges { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
