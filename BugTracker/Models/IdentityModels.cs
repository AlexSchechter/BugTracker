using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser

    {
        public ApplicationUser()
        {
            Comments = new HashSet<Comment>();
            Attachments = new HashSet<Attachment>();
            ManagedProjects = new HashSet<Project>();
            AssignedProjects = new HashSet<Project>();
            AssignedTickets = new HashSet<Ticket>();
            SubmittedTickets = new HashSet<Ticket>();
            TicketChanges = new HashSet<TicketChange>();
            OldDevelopers = new HashSet<TicketChange>();
            NewDevelopers = new HashSet<TicketChange>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [InverseProperty("CreatedBy")]
        public virtual ICollection<Comment> Comments { get; set; }
        [InverseProperty("CreatedBy")]
        public virtual ICollection<Attachment> Attachments { get; set; }
        [InverseProperty("Manager")]
        public virtual ICollection<Project> ManagedProjects { get; set; }
        [InverseProperty("Developers")]
        public virtual ICollection<Project> AssignedProjects { get; set; }
        [InverseProperty("Developer")]
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
        [InverseProperty("SubmittedBy")]
        public virtual ICollection<Ticket> SubmittedTickets { get; set; }
        [InverseProperty("ChangedBy")]
        public virtual ICollection<TicketChange> TicketChanges { get; set; }
        [InverseProperty("OldDeveloper")]
        public virtual ICollection<TicketChange> OldDevelopers { get; set; }
        [InverseProperty("NewDeveloper")]
        public virtual ICollection<TicketChange> NewDevelopers { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("AzureConnection", throwIfV1Schema: false)
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketChange> TicketChanges { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
    }
}