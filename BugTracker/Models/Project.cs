using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Developers = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        [InverseProperty("ManagedProjects")]
        public virtual ApplicationUser Manager { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        [InverseProperty("AssignedProjects")]
        public virtual ICollection<ApplicationUser> Developers { get; set; }
    }
}