namespace BugTracker.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string CreatedById { get; set; }
        public string Description { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}