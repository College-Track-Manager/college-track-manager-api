using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;

namespace CollegeTrackAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Your other DbSets (custom tables) go here
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentRegistration> Registrations { get; set; }

        // Optional: You can add more custom DbSets here if needed
    }
}
