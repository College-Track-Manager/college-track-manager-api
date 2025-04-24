using System.ComponentModel.DataAnnotations;

namespace CollegeTrackAPI.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public string UserEmail { get; set; } = "";

        public string Action { get; set; } = "";

        public string TableName { get; set; } = "";

        public string? RecordId { get; set; }

        public string? Details { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

}
