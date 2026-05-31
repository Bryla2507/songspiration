namespace SongSpiration.Models.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public Guid ReportedUserId { get; set; }
        public User ReportedUser { get; set; }
        public Guid ReportingUserId { get; set; }
        public User ReportingUser { get; set; }
        public string Content { get; set; }
    }
}
