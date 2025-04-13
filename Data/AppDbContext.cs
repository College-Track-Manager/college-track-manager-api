using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;

namespace CollegeTrackAPI.Data
{
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<StudentRegistration> Registrations { get; set; }

}
}