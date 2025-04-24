using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;
using System.Text.Json;

namespace CollegeTrackAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Your other DbSets (custom tables) go here
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentRegistration> Registrations { get; set; }
        
        public DbSet<AuditLog> AuditLogs { get; set; }


        // Optional: You can add more custom DbSets here if needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Track-StudentRegistration one-to-many
            modelBuilder.Entity<StudentRegistration>()
                .HasOne(sr => sr.Track)
                .WithMany(t => t.Registrations)
                .HasForeignKey(sr => sr.TrackId);

            // Track-Course many-to-many using join table TrackCourse
            modelBuilder.Entity<TrackCourse>()
                .HasKey(tc => new { tc.TrackId, tc.CourseId });

            modelBuilder.Entity<TrackCourse>()
                .HasOne(tc => tc.Track)
                .WithMany(t => t.TrackCourses)
                .HasForeignKey(tc => tc.TrackId);

            modelBuilder.Entity<TrackCourse>()
                .HasOne(tc => tc.Course)
                .WithMany(c => c.TrackCourses)
                .HasForeignKey(tc => tc.CourseId);

            // Handle List<string> Requirements using JSON conversion (more robust than delimited strings)

            modelBuilder.Entity<Track>()
    .Property(t => t.Requirements)
    .HasConversion(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
    );
        }


    }
}
