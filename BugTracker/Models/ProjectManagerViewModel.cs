using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectManagerViewModel
    {
        public List<Project> OwnProjects { get; set; }
        public List<Ticket> CriticalTickets { get; set; }
        public List<Ticket> UnassignedTickets { get; set; }
        public List<Ticket> FinishedTickets { get; set; }
    }
}