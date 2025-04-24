using System.Security.Claims;
using CollegeTrackAPI.Data;
using CollegeTrackAPI.Models;

namespace CollegeTrackAPI.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(ClaimsPrincipal user, string action, string table, string? recordId = null, string? details = null)
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";

            var log = new AuditLog
            {
                UserEmail = email,
                Action = action,
                TableName = table,
                RecordId = recordId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogActionAsync(string email, string action, string table, string? recordId = null, string? details = null)
        {
            var log = new AuditLog
            {
                UserEmail = email ?? "Unknown",
                Action = action,
                TableName = table,
                RecordId = recordId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

    }
}
