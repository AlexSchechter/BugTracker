using System;
using System.Collections.Generic;

namespace BugTracker.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        public virtual ApplicationUser Manager { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}