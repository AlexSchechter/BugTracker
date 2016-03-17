using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class AdminViewModel
    {
        public List<Project> Projects { get; set; }
        public List<Ticket> CriticalTickets { get; set; }
        public int AdminPopulation { get; set; }
        public int PMPopulation { get; set; }
        public int DeveloperPopulation { get; set; }
        public int SubmitterPopulation { get; set; }
    }
}