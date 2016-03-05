using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectViewModel
    {
        public int ProjectId { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [Display(Name = "Assigned Developers")]
        public MultiSelectList Developers { get; set; }
        public string[] SelectedDevelopers { get; set; }
        [Display(Name = "Project Manager")]
        public SelectList ProjectManagers { get; set; }
        public string SelectedProjectManager { get; set; }
    }

}